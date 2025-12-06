using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Martian.Helium.Editor
{
    [Serializable]
    internal class Entry : HeliumNode
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            // Entry is a special node that has no input, so we don't call DefineCommonPorts
            context.AddOutputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName("Graph Entry")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
