using System.Collections;
using UnityEngine;

namespace _Project.Scripts.UI
{
    [RequireComponent(typeof(Canvas))]
    public class BillboardCanvas : MonoBehaviour
    {
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private float _followDistance = 2f;
        [SerializeField] private float _minDistance = 1f;
        [SerializeField] private float _maxDistance = 3f;
        [SerializeField] private float _verticalOffset = 1f;
        [SerializeField] private float _moveDuration = 0.5f;

        [SerializeField, Range(1f, 180f)] private float _triggerAngle = 45f;
        
        private float _lastYaw;
        private Coroutine _moveRoutine;

        private void Start()
        {
            if (_playerTransform == null)
            {
                enabled = false;
                return;
            }

            _lastYaw = _playerTransform.eulerAngles.y;
            transform.position = CalculateTargetPosition();
            FacePlayer();
        }

        private void LateUpdate()
        {
            float currentYaw = _playerTransform.eulerAngles.y;
            float yawDelta = Mathf.Abs(Mathf.DeltaAngle(_lastYaw, currentYaw));

            Vector3 canvasPosFlat = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 playerPosFlat = new Vector3(_playerTransform.position.x, 0, _playerTransform.position.z);
            float currentDist = Vector3.Distance(playerPosFlat, canvasPosFlat);

            bool angleExceeded = yawDelta >= _triggerAngle;
            bool tooClose = currentDist < _minDistance;
            bool tooFar = currentDist > _maxDistance;

            if (angleExceeded || tooClose || tooFar)
            {
                if (angleExceeded)
                    _lastYaw = currentYaw;

                Vector3 targetPos = CalculateTargetPosition();
                if (_moveRoutine != null)
                    StopCoroutine(_moveRoutine);
                _moveRoutine = StartCoroutine(MoveTo(targetPos));
            }

            FacePlayer();
        }

        private Vector3 CalculateTargetPosition()
        {
            return _playerTransform.position
                   + _playerTransform.forward * _followDistance
                   + Vector3.up * _verticalOffset;
        }

        private IEnumerator MoveTo(Vector3 targetPos)
        {
            Vector3 startPos = transform.position;
            float elapsed = 0f;
            
            while (elapsed < _moveDuration)
            {
                float t = elapsed / _moveDuration;
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPos;
            _moveRoutine = null;
        }

        private void FacePlayer()
        {
            Vector3 dir = (transform.position - _playerTransform.position);
            dir.y = 0;
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}