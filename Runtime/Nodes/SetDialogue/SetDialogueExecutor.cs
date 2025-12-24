using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Martian.Helium
{
    public class SetDialogueExecutor : IHeliumNodeExecutor<SetDialogueRuntimeNode>
    {
        private readonly char[] _puncuationList = {'.', ',', '!', '?', ':', ';'};

        /// <summary>
        /// Executes the <see cref="SetDialogueRuntimeNode"/> node, setting the dialogue text and actor sprite settings.
        /// </summary>
        public async Awaitable ExecuteNodeAsync(SetDialogueRuntimeNode node, HeliumDirector ctx, CancellationToken token)
        {
            // create view info
            var viewInfo = new Dictionary<string, string>
            {
                {"line", "" },
                {"speakerName", node.Character.Name },
                {"speakerColor", ColorUtility.ToHtmlStringRGB(node.Character.Color) }
            };

            ctx.UpdateViewsInfo(viewInfo);

            await TypeTextWithSkip(node, ctx, viewInfo, token);

        }

        /// <summary>
        /// Types out the text into the views with the option to skip/finish on input gotten. 
        /// </summary>
        /// <returns></returns>
        private async Task TypeTextWithSkip(SetDialogueRuntimeNode node, HeliumDirector ctx, Dictionary<string, string> viewInfo, CancellationToken token)
        {
            var delayPerCharSeconds = ctx.GlobalCharDelayTime;
            var delayPerPuncSeconds = ctx.GlobalPuncDelayTime;
            var input = ctx.Input;

            viewInfo["line"] = "";

            var builder = new StringBuilder();

            var insideRichTag = false;

            // Start listening for skip input
            var skipInputDetected = input.InputDetected(token);

            foreach (var c in node.Line)
            {
                // Handle rich text tags (e.g., <b>, </i>)
                if (c == '<')
                    insideRichTag = true;

                builder.Append(c);

                if (c == '>')
                    insideRichTag = false;

                // Skip delay if rich text
                if (insideRichTag || char.IsWhiteSpace(c)) continue;

                viewInfo["line"] = builder.ToString();
                ctx.UpdateViewsInfo(viewInfo);

                // timer until next char appears
                var timer = 0f;
                while (timer < (_puncuationList.Contains(c) ? delayPerPuncSeconds : delayPerCharSeconds))
                {
                    if (skipInputDetected.IsCompleted)
                    {
                        viewInfo["line"] = node.Line;
                        ctx.UpdateViewsInfo(viewInfo);
                        return;
                    }
                    timer += Time.deltaTime;

                    try
                    {
                        await Awaitable.NextFrameAsync(token);
                    }
                    catch(OperationCanceledException)
                    {
                        break;
                    }
                }
            }

            viewInfo["line"] = node.Line;
        }
    }
}
