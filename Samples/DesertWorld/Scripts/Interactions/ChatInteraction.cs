using UnityEngine;

namespace Martian.Helium.Samples
{
    public class ChatInteraction : MonoBehaviour, IPlayerInteraction
    {
        [Header("Chat Settings")]
        [SerializeField] private HeliumRuntimeGraph _chatGraph;

        [Header("References")]
        [SerializeField] private Animator _anim;

        public void OnPlayerHover(Transform playerTransform, bool isHovered)
        {
            _anim.SetBool("IsHover", isHovered);
        }

        public void OnPlayerInteract(Transform playerTransform)
        {
            _anim.SetBool("IsHover", false);
            HeliumDirector.Instance.StartGraph(_chatGraph);
        }
    }
}
