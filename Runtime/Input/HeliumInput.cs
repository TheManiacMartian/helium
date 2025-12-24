using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Martian.Helium
{
    public class HeliumInput : MonoBehaviour
    {
        /// <summary>
        /// An <see cref="InputActionAsset"/> that defines the inputs for Helium.
        /// </summary>
        private HeliumInputActions _inputActions;

        /// <summary>
        /// A little fun way of having a task only complete when the button is pressed.
        /// </summary>
        private TaskCompletionSource<bool> _nextTcs;

        private void Awake()
        {
            _inputActions = new HeliumInputActions();
            if (_inputActions != null)
                _inputActions.Helium.Next.performed += OnNextPressed;
        }

        private void OnDestroy()
        {
            _inputActions.Helium.Next.performed -= OnNextPressed;
            _inputActions.Dispose();
        }

        /// <summary>
        /// When the 'Next' input action is performed, we set the <see cref="_nextTcs"/> task as completed.
        /// This <see cref="Task"/> can be awaited on by <see cref="IHeliumNodeExecutor{TNode}"/>s.
        /// </summary>
        private void OnNextPressed(InputAction.CallbackContext _) => _nextTcs?.TrySetResult(true);


        /// <summary>
        /// Enables the input actions when the object is enabled.
        /// </summary>
        public void EnableInput() => _inputActions.Enable();

        /// <summary>
        /// Disables the input actions when the object is disabled.
        /// </summary>
        public void DisableInput() => _inputActions.Disable();

        /// <summary>
        /// Creates a <see cref="TaskCompletionSource{TResult}"/> to wait for the next input event.
        /// <br/><br/>
        /// If there is already a <see cref="TaskCompletionSource{TResult}"/> created and it's not already completed,
        /// we just return it. This allows nodes to wait for the next input event without mistakenly waiting for input
        /// more than once.
        /// </summary>
        public Task InputDetected(CancellationToken token)
        {
            if (_nextTcs == null || _nextTcs.Task.IsCompleted)
            {
                _nextTcs = new TaskCompletionSource<bool>();
            }

            return _nextTcs?.Task.OrCancelledBy(token);
        }

    }
}
