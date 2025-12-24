using System;
using UnityEngine;

namespace Martian.Helium.Editor
{
    [Serializable]
    internal class SetDialogue : HeliumNode
    {
        public const string IN_PORT_CHARACTER_NAME = "Character";
        public const string IN_PORT_LINE_NAME = "Line";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<HeliumCharacter>(IN_PORT_CHARACTER_NAME)
                .WithDisplayName("Character")
                .Build();

            context.AddInputPort<string>(IN_PORT_LINE_NAME)
                .WithDisplayName("Line")
                .Build();

            
        }
    }
}
