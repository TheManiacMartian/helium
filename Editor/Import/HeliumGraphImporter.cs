using UnityEditor.AssetImporters;
using Martian.Helium;
using UnityEngine;
using Unity.GraphToolkit.Editor;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace Martian.Helium.Editor
{
    /// <summary>
    /// A <see cref="ScriptedImporter"/> that imports the <see cref="HeliumGraph"/>
    /// </summary>
    [ScriptedImporter(1, HeliumGraph.ASSET_EXTENSION)]
    internal class HeliumGraphImporter : ScriptedImporter
    {
        /// <summary>
        /// Unity calls this whenever the asset is imported, this way we can import <see cref="HeliumGraph"/> as a <see cref="HeliumRuntimeGraph"/>
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnImportAsset(AssetImportContext ctx)
        {
            HeliumGraph graph = GraphDatabase.LoadGraphForImporter<HeliumGraph>(ctx.assetPath);

            // The `graph` may be null if the `GraphDatabase.LoadGraphForImporter` method
            // fails to load the asset from the specified `ctx.assetPath`.
            // This can occur under the following circumstances:
            // - The asset path is incorrect, or the asset does not exist at the specified location.
            // - The asset located at the specified path is not of type `HeliumGraph`.
            // - The asset file itself is problematic. For example, it is corrupted, or stored in an unsupported format.
            //
            // Best practice to deal with serialization is to thoroughly validate and safeguard against
            // impaired or incomplete data, to account for potential deserialization issues.
            if (graph == null)
            {
                Debug.LogError($"Failed to load Helium graph asset: {ctx.assetPath}");
                return;
            }

            // get all the nodes
            var allNodes = graph.GetNodes();

            // get the entry node 
            var entryNodeModel = allNodes.OfType<Entry>().FirstOrDefault();
            if (entryNodeModel == null)
            {
                // Don't log error since it is already being logged
                return;
            }

            // PASS 1
            // get all nodes and assign them a GUID and map them into our runtime nodes
            Dictionary<INode, List<HeliumRuntimeNode>> nodeMap = new();
            Dictionary<SetDialogueOptionsRuntimeNode, List<INode>> dialogueOptions = new();
            List<HeliumRuntimeNode> allRuntimeNodes = new();

            foreach ( var node in allNodes )
            {
                var runtimeNodeList = TranslateNodeModelToRuntimeNodes(node, dialogueOptions);

                if(runtimeNodeList == null)
                {
                    continue;
                }

                foreach (var r in runtimeNodeList)
                {
                    // generate a guid for the node to assign it a unique id
                    r.NodeGUID = GUID.Generate().ToString();
                    allRuntimeNodes.Add(r);
                }

                nodeMap[node] = runtimeNodeList;
            }

            // PASS 2
            // go back through all the runtime nodes and connect them with their outputs, this is easy now that we know all the ID's of each node
            foreach(var nodeKvp in nodeMap )
            {
                INode editorNode = nodeKvp.Key;
                List<HeliumRuntimeNode> runtimeNodeChain = nodeKvp.Value;
                var lastRuntimeNode = runtimeNodeChain.Last(); // the last node in a chain, example is set dialogue having (set dialogue > wait for input) as it's chain

                // add the outputs to all nodes that are not the last
                for (int i = 0; i < runtimeNodeChain.Count - 1; i++)
                {
                    runtimeNodeChain[i].OutputNodes.Add(runtimeNodeChain[i + 1].NodeGUID);
                }

                // TODO: add output ports of all block nodes attached
                if (nodeMap[editorNode].First().GetType() == typeof(SetDialogueOptionsRuntimeNode))
                {
                    var dialogueOptionsRuntimeNode = (SetDialogueOptionsRuntimeNode)nodeMap[editorNode].First();
                    var options = dialogueOptions[dialogueOptionsRuntimeNode];
                    foreach (var option in options)
                    {
                        dialogueOptionsRuntimeNode.OutputNodes.Add(nodeMap[option].First().NodeGUID);
                    }
                    
                }

                var nextEditorNode = GetOutNode(editorNode);

                if(nextEditorNode == null)
                {
                    continue;
                }

                // get the first of the next and attach it to the last of the current as an output
                var firstRuntimeNodeOfNext = nodeMap[nextEditorNode].First();
                lastRuntimeNode.OutputNodes.Add(firstRuntimeNodeOfNext.NodeGUID);
            }

            var startNode = GetOutNode(entryNodeModel);

            // build runtime asset by going through the graph and adding each node
            HeliumRuntimeGraph runtimeAsset = ScriptableObject.CreateInstance<HeliumRuntimeGraph>();

            // finally add all the nodes to the runtime asset
            runtimeAsset.Nodes.AddRange(allRuntimeNodes);

            runtimeAsset.StartNode = nodeMap[startNode].First();

            ctx.AddObjectToAsset("RuntimeAsset", runtimeAsset);
            ctx.SetMainObject(runtimeAsset);
        }

        /// <summary>
        /// Get the node attached the the 'Out' of this node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        static INode GetOutNode(INode node)
        {
            var outputPort = node.GetOutputPortByName(HeliumNode.EXECUTION_PORT_DEFAULT_NAME);
            var nextNodePort = outputPort.firstConnectedPort;
            var nextNode = nextNodePort?.GetNode();

            return nextNode;
        }

        /// <summary>
        /// This function translates a node from the graph into runtime nodes, we use multiple nodes due to the fact that some nodes may have extra functionalities, 
        /// such as a second delay or waiting for an input.
        /// </summary>
        /// <param name="nodeModel"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        static List<HeliumRuntimeNode> TranslateNodeModelToRuntimeNodes(INode nodeModel, Dictionary<SetDialogueOptionsRuntimeNode, List<INode>> dialogueOptions)
        {
            var returnedNodes = new List<HeliumRuntimeNode>();
            switch (nodeModel)
            {
                case SetDialogue setDialogueNodeModel:

                    returnedNodes.Add(new SetDialogueRuntimeNode
                    {
                        NodeType = "Set Dialogue",
                        Character = GetInputPortValue<HeliumCharacter>(setDialogueNodeModel.GetInputPortByName(SetDialogue.IN_PORT_CHARACTER_NAME)),
                        Line = GetInputPortValue<string>(setDialogueNodeModel.GetInputPortByName(SetDialogue.IN_PORT_LINE_NAME)),
                    });

                    // Insert a WaitForInputNode after dialogue to create the expected visual novel behaviour.
                    // This ensures narrative flow pauses until the player signals readiness to continue.
                    returnedNodes.Add(new WaitForInputRuntimeNode{ NodeType = "Wait For Input" });
                    break;

                case WaitForInput _:
                    returnedNodes.Add(new WaitForInputRuntimeNode { NodeType = "Wait For Input" });
                    break;

                case SetDialogueOptions setDialogueOptionsNodeModel:
                    
                    var runtimeNode = new SetDialogueOptionsRuntimeNode
                    {
                        NodeType = "Set Dialogue Options"
                    };

                    List<INode> options = new();

                    // loop through blocks and add them to the dialogue options dictionary
                    for(int i = 0; i < setDialogueOptionsNodeModel.blockCount; i++)
                    {
                        runtimeNode.Options.Add(new SetDialogueOptionsRuntimeNode.DialogueOption
                        {
                            Choice = GetInputPortValue<string>(setDialogueOptionsNodeModel.GetBlock(i).GetInputPortByName(DialogueOptionBlock.IN_PORT_OPTION_TEXT_NAME)),
                            OutputNodeIndex = i
                        });

                        var outputPort = setDialogueOptionsNodeModel.GetBlock(i).GetOutputPortByName(HeliumNode.EXECUTION_PORT_DEFAULT_NAME);
                        var nextNodePort = outputPort.firstConnectedPort;
                        var nextNode = nextNodePort?.GetNode();
                        options.Add(nextNode);
                    }

                    dialogueOptions.Add(runtimeNode, options);
                    returnedNodes.Add(runtimeNode);

                    break;

                default:
                    return null;
                    //throw new ArgumentException($"Unsupported node model type: {nodeModel.GetType()}");
            }

            return returnedNodes;
        }

        /// <summary>
        /// Gets the value of an input port on a node.
        /// <br/><br/>
        /// The value is obtained from (in priority order):<br/>
        /// 1. Connections to the port (variable nodes, constant nodes, wire portals)<br/>
        /// 2. Embedded value on the port<br/>
        /// 3. Default value of the port<br/>
        /// </summary>
        static T GetInputPortValue<T>(IPort port)
        {
            T value = default;

            // If port is connected to another node, get value from connection
            if (port.isConnected)
            {
                switch (port.firstConnectedPort.GetNode())
                {
                    case IVariableNode variableNode:
                        variableNode.variable.TryGetDefaultValue<T>(out value);
                        return value;
                    case IConstantNode constantNode:
                        constantNode.TryGetValue<T>(out value);
                        return value;
                    default:
                        break;
                }
            }
            else
            {
                // If port has embedded value, return it.
                // Otherwise, return the default value of the port
                port.TryGetValue(out value);
            }

            return value;
        }
    }
}
