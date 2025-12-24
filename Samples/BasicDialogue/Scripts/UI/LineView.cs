using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Martian.Helium
{
    public class LineView : HeliumView
    {
        [SerializeField] private TextMeshProUGUI _lineText;

        private void Start()
        {
            // Hide line view at beginning
            gameObject.SetActive(false);
        }

        public override void UpdateInfo(Dictionary<string, string> info)
        {
            if(info.ContainsKey("line"))
            {
                // show the line of dialogue
                gameObject.SetActive(true);
                _lineText.text = info["line"];
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
