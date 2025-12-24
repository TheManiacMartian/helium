using UnityEngine;

namespace Martian.Helium.Samples
{
    public interface IPlayerInteraction
    {
        abstract void OnPlayerInteract(Transform playerTransform);

        abstract void OnPlayerHover(Transform playerTransform, bool isHovered);
    }
}
