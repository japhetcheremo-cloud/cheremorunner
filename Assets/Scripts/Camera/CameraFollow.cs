using UnityEngine;

namespace CheremoRunner.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target Tracking")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 4.2f, -5.5f);
        [SerializeField] private Vector3 lookOffset = new Vector3(0f, 1.5f, 3.0f);
        [SerializeField] private float smoothSpeed = 10f;
        [SerializeField] private float xFollowSpeed = 6f;

        [Header("Dynamic FOV")]
        [SerializeField] private UnityEngine.Camera cam;
        [SerializeField] private float baseFOV = 62f;
        [SerializeField] private float maxFOV = 74f;

        // Screen Shake
        private float shakeDuration = 0f;
        private float shakeMagnitude = 0.3f;
        private Vector3 currentPos;

        private void Start()
        {
            if (cam == null) cam = GetComponent<UnityEngine.Camera>();
            if (cam == null) cam = UnityEngine.Camera.main;

            if (target != null)
            {
                currentPos = target.position + offset;
                transform.position = currentPos;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 targetPosition = target.position + offset;

            // Smooth X tracking so player lane change looks dynamic
            float newX = Mathf.Lerp(transform.position.x, targetPosition.x, Time.deltaTime * xFollowSpeed);
            float newY = Mathf.Lerp(transform.position.y, targetPosition.y, Time.deltaTime * smoothSpeed);
            float newZ = targetPosition.z; // Follow Z directly to keep distance

            Vector3 finalPos = new Vector3(newX, newY, newZ);

            // Apply screen shake
            if (shakeDuration > 0)
            {
                finalPos += Random.insideUnitSphere * shakeMagnitude;
                shakeDuration -= Time.deltaTime;
            }

            transform.position = finalPos;

            // Look slightly ahead of player
            Vector3 lookTarget = target.position + lookOffset;
            transform.LookAt(lookTarget);

            // Dynamic FOV based on forward speed
            var playerCtrl = target.GetComponent<Player.PlayerController>();
            if (playerCtrl != null && cam != null)
            {
                float speedRatio = Mathf.InverseLerp(Core.GameConstants.INITIAL_FORWARD_SPEED,
                                                      Core.GameConstants.MAX_FORWARD_SPEED,
                                                      playerCtrl.ForwardSpeed);
                cam.fieldOfView = Mathf.Lerp(baseFOV, maxFOV, speedRatio);
            }
        }

        public void TriggerShake(float duration = 0.3f, float magnitude = 0.4f)
        {
            shakeDuration = duration;
            shakeMagnitude = magnitude;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
