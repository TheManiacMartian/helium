using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Martian.Helium
{
    public class WaitForInputExecutor : IHeliumNodeExecutor<WaitForInputRuntimeNode>
    {
        /// <summary>
        /// All we do is await the input task from the <see cref="HeliumInput"/> connected to the <see cref="HeliumDirector"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async Awaitable ExecuteNodeAsync(WaitForInputRuntimeNode node, HeliumDirector ctx, CancellationToken token)
        {
            try
            {
                await ctx.Input.InputDetected(token);
            }
            catch(OperationCanceledException)
            {
                // it was canceled so nothing happens!
            }
        }
    }
}
