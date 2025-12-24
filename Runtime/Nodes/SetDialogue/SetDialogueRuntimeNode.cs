using System;
using UnityEngine;

namespace Martian.Helium
{
    [Serializable]
    public class SetDialogueRuntimeNode : HeliumRuntimeNode
    {
        public HeliumCharacter Character;
        public string Line;
    }
}
