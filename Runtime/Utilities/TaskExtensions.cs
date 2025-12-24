using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Martian.Helium
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Apply cancellation support to non-cancelable task.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task OrCancelledBy(this Task task, CancellationToken token)
        {
            await Task.WhenAny
                (
                    task,
                    Task.Delay(-1, token) // wait forever until cancellation
                );
        }

        public static async Task<TResult> OrCancelledBy<TResult>(this Task<TResult> task, CancellationToken token)
        {
            async Task<TResult> WaitUntilCanceled(CancellationToken token)
            {
                await Task.Delay(-1, token); // wait forever until cancellation
                return default;
            }

            return await await Task.WhenAny(task, WaitUntilCanceled(token));
        }
    }
}
