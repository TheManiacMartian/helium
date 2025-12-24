using System.Collections.Generic;
using UnityEngine;

namespace Martian.Helium
{
    /// <summary>
    /// Runtime representation of the helium graph.
    /// </summary>
    public class HeliumRuntimeGraph : ScriptableObject
    {
        [SerializeReference]
        public HeliumRuntimeNode StartNode;

        [SerializeReference]
        public List<HeliumRuntimeNode> Nodes = new List<HeliumRuntimeNode>(); 

        public HeliumRuntimeNode FindNodeFromGUID(string guid)
        {
            foreach (HeliumRuntimeNode node in Nodes)
            {
                if(node.NodeGUID == guid) return node;
            }

            return null;
        }
    }
}
