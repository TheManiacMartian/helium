using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Martian.Helium.Editor
{
    [UseWithContext(typeof(SetDialogueOptions))]
    [Serializable]
    public class DialogueOptionBlock : BlockNode
    {
        public const string IN_PORT_OPTION_TEXT_NAME = "Option";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<string>(IN_PORT_OPTION_TEXT_NAME)
                .WithDisplayName("Option Text")
                .Build();

            context.AddOutputPort(HeliumNode.EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
