using System;
using UnityEngine;

namespace CheremoRunner.Player
{
    public class SwipeDetector : MonoBehaviour
    {
        public static event Action OnSwipeLeft;
        public static event Action OnSwipeRight;
        public static event Action OnSwipeUp;
        public static event Action OnSwipeDown;
        public static event Action OnTap;

        [Header("Swipe Tuning")]
        [SerializeField] private float minSwipeDistancePixels = 45f;
        [SerializeField] private float maxSwipeTimeSeconds = 0.6f;

        private Vector2 fingerDownPosition;
        private Vector2 fingerUpPosition;
        private float fingerDownTime;
        private bool isSwiping = false;

        private void Update()
        {
            HandleTouchInput();
            HandleKeyboardInput();
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    fingerDownPosition = touch.position;
                    fingerUpPosition = touch.position;
                    fingerDownTime = Time.unscaledTime;
                    isSwiping = true;
                }
                else if (touch.phase == TouchPhase.Moved && isSwiping)
                {
                    fingerUpPosition = touch.position;
                    // Detect early swipe flick
                    if (Vector2.Distance(fingerDownPosition, fingerUpPosition) > minSwipeDistancePixels)
                    {
                        EvaluateSwipe();
                        isSwiping = false;
                    }
                }
                else if (touch.phase == TouchPhase.Ended && isSwiping)
                {
                    fingerUpPosition = touch.position;
                    EvaluateSwipe();
                    isSwiping = false;
                }
                else if (touch.phase == TouchPhase.Canceled)
                {
                    isSwiping = false;
                }
            }
        }

        private void HandleKeyboardInput()
        {
            // Keyboard fallbacks for desktop / editor testing
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnSwipeLeft?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnSwipeRight?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
            {
                OnSwipeUp?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                OnSwipeDown?.Invoke();
            }
        }

        private void EvaluateSwipe()
        {
            float duration = Time.unscaledTime - fingerDownTime;
            if (duration > maxSwipeTimeSeconds) return;

            Vector2 delta = fingerUpPosition - fingerDownPosition;
            float horizontalDistance = Mathf.Abs(delta.x);
            float verticalDistance = Mathf.Abs(delta.y);

            if (delta.magnitude < minSwipeDistancePixels)
            {
                OnTap?.Invoke();
                return;
            }

            if (horizontalDistance > verticalDistance)
            {
                if (delta.x > 0)
                    OnSwipeRight?.Invoke();
                else
                    OnSwipeLeft?.Invoke();
            }
            else
            {
                if (delta.y > 0)
                    OnSwipeUp?.Invoke();
                else
                    OnSwipeDown?.Invoke();
            }
        }
    }
}
