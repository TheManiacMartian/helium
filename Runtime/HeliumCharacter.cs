using UnityEngine;

namespace Martian.Helium
{
    [CreateAssetMenu(fileName = "HeliumCharacter", menuName = "Martian/Helium/HeliumCharacter")]
    public class HeliumCharacter : ScriptableObject
    {
        [Header("Character Settings")]
        public string Name;
        public Color Color;
    }
}
