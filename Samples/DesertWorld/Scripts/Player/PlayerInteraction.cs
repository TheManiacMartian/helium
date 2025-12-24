using UnityEngine;
using UnityEngine.InputSystem;

namespace Martian.Helium.Samples
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerInteraction : MonoBehaviour
    {
        private bool _canInteract = true;

        private HeliumSampleInputActions _inputActions;

        private IPlayerInteraction _hoveredInteraction;

        private void Awake()
        {
            _inputActions = new HeliumSampleInputActions();

            if (_inputActions != null)
            {
                _inputActions.Player.Interact.performed += TryInteract;
            }
        }

        /// <summary>
        /// Interact with the current hovered interaction if we have one.
        /// </summary>
        private void TryInteract(InputAction.CallbackContext ctx)
        {
            if(!_canInteract) { return; }

            if(ctx.performed)
            {
                // try interacting
                if(_hoveredInteraction != null)
                {
                    _hoveredInteraction.OnPlayerInteract(transform);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IPlayerInteraction interaction;

            if(collision.TryGetComponent(out interaction)) 
            {
                // hover interaction
                HoverInteraction(interaction);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            IPlayerInteraction interaction;

            if (collision.TryGetComponent(out interaction))
            {
                // unhover interaction
                if(interaction == _hoveredInteraction)
                {
                    _hoveredInteraction.OnPlayerHover(transform, false);
                    _hoveredInteraction = null;
                }
            }
        }

        private void HoverInteraction(IPlayerInteraction interaction)
        {
            if(_hoveredInteraction != null)
            {
                _hoveredInteraction.OnPlayerHover(transform, false);
                _hoveredInteraction = null;
            }

            _hoveredInteraction = interaction;
            _hoveredInteraction.OnPlayerHover(transform, true);
        }

        private void OnEnable()
        {
            _inputActions.Enable();

            HeliumDirector.Instance.OnGraphStarted += OnHeliumGraphStart;
            HeliumDirector.Instance.OnGraphComplete += OnHeliumGraphEnd;
        }

        private void OnDisable()
        {
            _inputActions.Disable();

            HeliumDirector.Instance.OnGraphStarted -= OnHeliumGraphStart;
            HeliumDirector.Instance.OnGraphComplete -= OnHeliumGraphEnd;
        }

        private void OnHeliumGraphStart()
        {
            if(_hoveredInteraction != null)
            {
                _hoveredInteraction.OnPlayerHover(transform, false);
            }

            _canInteract = false;
        }

        private void OnHeliumGraphEnd()
        {
            if (_hoveredInteraction != null)
            {
                _hoveredInteraction.OnPlayerHover(transform, true);
            }

            _canInteract = true;
        }
    }
}
