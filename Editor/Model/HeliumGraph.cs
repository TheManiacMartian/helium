using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using Unity.GraphToolkit.Editor;
using System.Linq;

namespace Martian.Helium.Editor
{
    [Graph(ASSET_EXTENSION)]
    [Serializable]
    internal class HeliumGraph : Graph
    {
        /// <summary>
        /// File extension used for helium graphs.
        /// </summary>
        public const string ASSET_EXTENSION = "helium";

        [MenuItem("Assets/Create/Martian/Helium/Helium Graph")]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<HeliumGraph>("New Helium Graph");
        }

        public override void OnGraphChanged(GraphLogger graphLogger)
        {
            base.OnGraphChanged(graphLogger);

            ValidateGraph(graphLogger);
        }

        /// <summary>
        /// Checks the graph for any errors or warnings specfic to helium.
        /// </summary>
        /// <param name="info"></param>
        private void ValidateGraph(GraphLogger info)
        {
            // check that there is only one entry node
            List<Entry> entries = GetNodes().OfType<Entry>().ToList();

            switch (entries.Count)
            {
                case 0:
                    info.LogError("Add an Entry Node in your Helium graph.", this);
                    break;
                case >= 1:
                    {
                        foreach (var startNode in entries.Skip(1))
                        {
                            info.LogWarning($"Helium only supports one Entry Node per graph. Only the first created one will be used.", startNode);
                        }
                        break;
                    }
            }

        }
    }
}
