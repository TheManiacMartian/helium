using System;
using System.Collections.Generic;
using UnityEngine;

namespace Martian.Helium
{
    /// <summary>
    /// Base class for runtime nodes.
    /// </summary>
    [Serializable]
    public class HeliumRuntimeNode
    {
        public string NodeGUID, NodeType;
        public List<string> OutputNodes = new();
    }
}
