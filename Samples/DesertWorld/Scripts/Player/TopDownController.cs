using UnityEngine;
using UnityEngine.InputSystem;

namespace Martian.Helium.Samples
{
    public class TopDownController : MonoBehaviour
    {

        [Header("Options")]
        public float MoveSpeed = 7f;

        // status
        private bool _canMove = true;

        // input
        private HeliumSampleInputActions _inputActions;
        public Vector2 MoveInput {  get; private set; }


        
        [Header("References")]
        [SerializeField] private Animator _anim;
        [SerializeField] private SpriteRenderer _sprite;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _inputActions = new HeliumSampleInputActions();

            if(_inputActions != null)
            {
                _inputActions.Player.Move.performed += MoveInputRecieved;
                _inputActions.Player.Move.canceled += MoveInputRecieved;
            }
        }

        private void OnEnable()
        {
            // Enables the input actions when the object is enabled.
            _inputActions.Enable();

            // subscribe to the helium graph events
            HeliumDirector.Instance.OnGraphStarted += OnHeliumGraphStart;
            HeliumDirector.Instance.OnGraphComplete += OnHeliumGraphEnd;

        }

        private void OnDisable()
        {
            // Disables the input actions when the object is disabled.
            _inputActions.Disable();

            // unsubscribe to the helium graph events
            HeliumDirector.Instance.OnGraphStarted -= OnHeliumGraphStart;
            HeliumDirector.Instance.OnGraphComplete -= OnHeliumGraphEnd;
        }

        #region Helium

        private void OnHeliumGraphStart()
        {
            MoveInput = Vector2.zero;
            _canMove = false;
        }

        private void OnHeliumGraphEnd()
        {
            _canMove = true;
        }

        #endregion

        #region Input

        private void MoveInputRecieved(InputAction.CallbackContext ctx)
        {
            if (!_canMove)
            {
                MoveInput = Vector2.zero;
                return;
            }

            MoveInput = ctx.ReadValue<Vector2>();
        }

        #endregion

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (!_canMove) 
            {
                _rb.linearVelocity = Vector2.zero;
                return; 
            }

            // add move acceleration
            Vector2 moveDir = MoveInput.normalized * MoveSpeed;
            _rb.linearVelocity = moveDir;
        }

        private void LateUpdate()
        {
            // update animator
            _anim.SetBool("Moving", MoveInput.magnitude > 0.1f);

            // flip sprite
            if(MoveInput.x  > 0.1f )
            {
                _sprite.flipX = false;
            }
            else if (MoveInput.x < -0.1f)
            {
                _sprite.flipX = true;

            }
        }
    }
}
