using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Martian.Helium.Samples
{
    public class SpeakerView : HeliumView
    {
        [SerializeField] private TextMeshProUGUI _speakerText;

        private void Start()
        {
            // Hide line view at beginning
            gameObject.SetActive(false);
        }

        public override void UpdateInfo(Dictionary<string, string> info)
        {
            if (info.ContainsKey("speakerName"))
            {
                if (info["speakerName"] != "")
                {
                    // show the line of dialogue
                    gameObject.SetActive(true);

                    // set the speaker color
                    if (info.ContainsKey("speakerColor"))
                    {
                        ColorUtility.TryParseHtmlString("#" + info["speakerColor"], out Color speakerColor);
                        _speakerText.color = speakerColor;
                    }

                    _speakerText.text = info["speakerName"];
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
