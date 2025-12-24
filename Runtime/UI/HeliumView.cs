using System.Collections.Generic;
using UnityEngine;

namespace Martian.Helium
{
    public abstract class HeliumView : MonoBehaviour
    {
        public abstract void UpdateInfo(Dictionary<string, string> info);
    }
}
