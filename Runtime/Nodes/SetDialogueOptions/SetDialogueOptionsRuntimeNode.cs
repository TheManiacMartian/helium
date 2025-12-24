using System;
using System.Collections.Generic;
using UnityEngine;

namespace Martian.Helium
{
    [Serializable]
    public class SetDialogueOptionsRuntimeNode : HeliumRuntimeNode
    {
        [Serializable]
        public struct DialogueOption
        {
            public string Choice;
            public int OutputNodeIndex;
        }

        public List<DialogueOption> Options = new();
    }
}
