using UnityEngine;

namespace _Project.Scripts.Player
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, -4f);
        [SerializeField, Range(1f, 50f)] private float _positionSmooth = 1f;
        [SerializeField, Range(1f, 50f)] private float _rotationSmooth = 20f;
        [SerializeField, Range(0f, 90f)] private float _minPitch = 10f;
        [SerializeField, Range(0f, 90f)] private float _maxPitch = 60f;
        
        private float _currentPitch = 20f;

        private void Update()
        {
            float mouseY = Input.GetAxis("Mouse Y");
            _currentPitch = Mathf.Clamp(_currentPitch - mouseY, _minPitch, _maxPitch);
        }

        private void LateUpdate()
        {
            Vector3 worldOffset = _target.TransformDirection(_offset);
            Vector3 desiredPosition = _target.position + worldOffset;

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                Time.deltaTime * _positionSmooth
            );

            Vector3 lookTarget = _target.position + Vector3.up * 1.5f;
            Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRotation,
                Time.deltaTime * _rotationSmooth
            );
        }
    }
}