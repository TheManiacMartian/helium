using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Martian.Helium.Editor
{
    [Serializable]
    internal abstract class HeliumNode : Node
    {
        public const string EXECUTION_PORT_DEFAULT_NAME = "ExecutionPort";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
