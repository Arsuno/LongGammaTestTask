using UnityEngine;

namespace _Project.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementController : MonoBehaviour
    {
        private const string HorizontalAxisName = "Horizontal";
        private const string VerticalAxisName = "Vertical";

        [SerializeField] private float _movementSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _jumpHeight = 1.5f;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private CharacterController _characterController;

        private Vector3 _velocity;
        private bool _isGrounded;

        private void Update()
        {
            UpdateGroundStatus();
            HandleMovement();
            HandleJumpAndGravity();
        }

        private void UpdateGroundStatus()
        {
            _isGrounded = _characterController.isGrounded;

            if (_isGrounded && _velocity.y < 0f)
                _velocity.y = -2f;
        }

        private void HandleMovement()
        {
            float horizontalAxis = Input.GetAxis(HorizontalAxisName);
            float verticalAxis = Input.GetAxis(VerticalAxisName);

            Vector3 input = new Vector3(horizontalAxis, 0f, verticalAxis).normalized;

            if (input.magnitude < 0.1f)
                return;

            float targetAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg
                                + _cameraTransform.eulerAngles.y;

            float angle = Mathf.LerpAngle(
                transform.eulerAngles.y,
                targetAngle,
                Time.deltaTime * _rotationSpeed
            );
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _characterController.Move(moveDirection * _movementSpeed * Time.deltaTime);
        }

        private void HandleJumpAndGravity()
        {
            if (Input.GetButtonDown("Jump") && _isGrounded)
                _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * Physics.gravity.y);

            _velocity.y += Physics.gravity.y * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }
}