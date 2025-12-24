using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Martian.Helium
{
    /// <summary>
    /// Interface for executor of helium node.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public interface IHeliumNodeExecutor<in TNode> where TNode : HeliumRuntimeNode
    {
        Awaitable ExecuteNodeAsync(TNode node, HeliumDirector ctx, CancellationToken token);
    }
}
