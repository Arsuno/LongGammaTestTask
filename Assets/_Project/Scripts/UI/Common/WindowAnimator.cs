using System;
using System.Collections;
using UnityEngine;

namespace _Project.Scripts.UI.Common
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SimpleWindowAnimator : MonoBehaviour
    {
        [SerializeField] private float _fromScale = 0.2f;
        [SerializeField] private float _toScale = 1f;
        [SerializeField] private float _duration = 0.3f;
        [SerializeField] private AnimationCurve _curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void PlayShow(Action callback = null)
        {
            StopAllCoroutines();
            gameObject.SetActive(true);

            StartCoroutine(Animate(_fromScale, _toScale, 0f, 1f, false, callback));
        }

        public void PlayHide(Action callback = null)
        {
            StopAllCoroutines();
            StartCoroutine(Animate(_toScale, _fromScale, 1f, 0f, true, callback));
        }

        private IEnumerator Animate(
            float scaleStart, float scaleEnd,
            float alphaStart, float alphaEnd,
            bool deactivateOnEnd,
            Action callback = null
        )
        {
            float elapsed = 0f;
            _rectTransform.localScale = Vector3.one * scaleStart;
            if (_canvasGroup != null) _canvasGroup.alpha = alphaStart;

            while (elapsed < _duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / _duration);
                float eval = _curve.Evaluate(t);

                float scale = Mathf.Lerp(scaleStart, scaleEnd, eval);
                _rectTransform.localScale = Vector3.one * scale;

                if (_canvasGroup != null)
                    _canvasGroup.alpha = Mathf.Lerp(alphaStart, alphaEnd, eval);

                yield return null;
            }

            _rectTransform.localScale = Vector3.one * scaleEnd;
            if (_canvasGroup != null)
                _canvasGroup.alpha = alphaEnd;

            if (deactivateOnEnd)
                gameObject.SetActive(false);

            callback?.Invoke();
        }
    }
}