using UnityEngine;

namespace CheremoRunner.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;

        private CharacterController characterController;
        private Core.LaneIndex currentLane = Core.LaneIndex.Middle;
        private float targetXPosition = Core.GameConstants.LANE_MIDDLE;
        private float verticalVelocity = 0f;
        private bool isJumping = false;
        private bool isSliding = false;
        private float slideTimer = 0f;
        private bool isDead = false;
        private float forwardSpeed;

        // Collider scaling for sliding
        private float normalHeight = 1.8f;
        private float normalCenterY = 0.9f;
        private float slideHeight = 0.6f;
        private float slideCenterY = 0.3f;

        // Animation hashes
        private static readonly int AnimRun = Animator.StringToHash("IsRunning");
        private static readonly int AnimJump = Animator.StringToHash("Jump");
        private static readonly int AnimSlide = Animator.StringToHash("Slide");
        private static readonly int AnimDead = Animator.StringToHash("Dead");
        private static readonly int AnimLand = Animator.StringToHash("Land");
        private static readonly int AnimSpeedMultiplier = Animator.StringToHash("SpeedMultiplier");

        public float ForwardSpeed => forwardSpeed;
        public bool IsDead => isDead;
        public float DistanceTravelled { get; private set; }

        private bool hasShield = false;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
            {
                characterController = gameObject.AddComponent<CharacterController>();
            }

            characterController.height = normalHeight;
            characterController.center = new Vector3(0, normalCenterY, 0);
            characterController.radius = 0.4f;
            characterController.slopeLimit = 0f;
            characterController.stepOffset = 0f;
            characterController.skinWidth = 0.02f;

            forwardSpeed = Core.GameConstants.INITIAL_FORWARD_SPEED;
            currentLane = Core.LaneIndex.Middle;
            targetXPosition = Core.GameConstants.LANE_MIDDLE;

            SwipeDetector.OnSwipeLeft += HandleSwipeLeft;
            SwipeDetector.OnSwipeRight += HandleSwipeRight;
            SwipeDetector.OnSwipeUp += HandleSwipeUp;
            SwipeDetector.OnSwipeDown += HandleSwipeDown;

            SetAnimBool(AnimRun, true);
        }

        private void OnDestroy()
        {
            SwipeDetector.OnSwipeLeft -= HandleSwipeLeft;
            SwipeDetector.OnSwipeRight -= HandleSwipeRight;
            SwipeDetector.OnSwipeUp -= HandleSwipeUp;
            SwipeDetector.OnSwipeDown -= HandleSwipeDown;
        }

        private void Update()
        {
            if (isDead) return;

            float dt = Time.deltaTime;

            // Increase speed over time
            forwardSpeed = Mathf.Min(forwardSpeed + Core.GameConstants.SPEED_ACCELERATION_RATE * dt,
                                     Core.GameConstants.MAX_FORWARD_SPEED);

            // Forward movement
            Vector3 moveDir = Vector3.forward * forwardSpeed * dt;
            DistanceTravelled += forwardSpeed * dt;

            // Lateral lane movement
            float currentX = transform.position.x;
            float newX = Mathf.MoveTowards(currentX, targetXPosition, Core.GameConstants.LANE_CHANGE_SPEED * dt);
            moveDir.x = newX - currentX;

            // Vertical movement (gravity/jump)
            if (characterController.isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f; // Small downward to keep grounded
                if (isJumping)
                {
                    isJumping = false;
                    SetAnimTrigger(AnimLand);
                }
            }

            if (!characterController.isGrounded)
            {
                float grav = Core.GameConstants.GRAVITY;
                // Fast fall if past apex
                if (verticalVelocity < 0)
                    grav *= Core.GameConstants.FAST_FALL_GRAVITY_MULTIPLIER;
                verticalVelocity -= grav * dt;
            }

            moveDir.y = verticalVelocity * dt;

            // Sliding timer
            if (isSliding)
            {
                slideTimer -= dt;
                if (slideTimer <= 0f)
                {
                    EndSlide();
                }
            }

            characterController.Move(moveDir);

            // Clamp above ground
            if (transform.position.y < 0f)
            {
                transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            }

            // Update animation speed
            float speedRatio = forwardSpeed / Core.GameConstants.INITIAL_FORWARD_SPEED;
            SetAnimFloat(AnimSpeedMultiplier, Mathf.Clamp(speedRatio, 0.8f, 2.5f));
        }

        #region Swipe Handlers

        private void HandleSwipeLeft()
        {
            if (isDead) return;
            if (currentLane > Core.LaneIndex.Left)
            {
                currentLane--;
                targetXPosition = GetLaneXPosition(currentLane);
                Audio.AudioManager.Instance?.PlayClickSound();
            }
        }

        private void HandleSwipeRight()
        {
            if (isDead) return;
            if (currentLane < Core.LaneIndex.Right)
            {
                currentLane++;
                targetXPosition = GetLaneXPosition(currentLane);
                Audio.AudioManager.Instance?.PlayClickSound();
            }
        }

        private void HandleSwipeUp()
        {
            if (isDead || isJumping) return;
            if (characterController.isGrounded)
            {
                verticalVelocity = Core.GameConstants.JUMP_FORCE;
                isJumping = true;
                if (isSliding) EndSlide();
                SetAnimTrigger(AnimJump);
                Audio.AudioManager.Instance?.PlayJumpSound();
            }
        }

        private void HandleSwipeDown()
        {
            if (isDead || isSliding) return;
            if (isJumping)
            {
                // Fast-fall from air
                verticalVelocity = -Core.GameConstants.JUMP_FORCE * 1.5f;
            }
            else
            {
                StartSlide();
            }
        }

        #endregion

        #region Slide

        private void StartSlide()
        {
            isSliding = true;
            slideTimer = Core.GameConstants.SLIDE_DURATION;
            characterController.height = slideHeight;
            characterController.center = new Vector3(0, slideCenterY, 0);
            SetAnimTrigger(AnimSlide);
            Audio.AudioManager.Instance?.PlaySlideSound();
        }

        private void EndSlide()
        {
            isSliding = false;
            slideTimer = 0f;
            characterController.height = normalHeight;
            characterController.center = new Vector3(0, normalCenterY, 0);
        }

        #endregion

        #region Collision

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (isDead) return;

            if (hit.gameObject.CompareTag(Core.GameConstants.TAG_OBSTACLE))
            {
                if (hasShield)
                {
                    hasShield = false;
                    Destroy(hit.gameObject);
                    return;
                }
                Die();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isDead) return;

            if (other.CompareTag(Core.GameConstants.TAG_COIN))
            {
                CollectCoin(other.gameObject);
            }
            else if (other.CompareTag(Core.GameConstants.TAG_POWERUP))
            {
                CollectPowerup(other.gameObject);
            }
            else if (other.CompareTag(Core.GameConstants.TAG_OBSTACLE))
            {
                if (hasShield)
                {
                    hasShield = false;
                    other.gameObject.SetActive(false);
                    return;
                }
                Die();
            }
        }

        #endregion

        private void CollectCoin(GameObject coinObj)
        {
            coinObj.SetActive(false);
            GameManager.Instance?.AddCoin();
            Audio.AudioManager.Instance?.PlayCoinSound();
        }

        private void CollectPowerup(GameObject puObj)
        {
            var puBehavior = puObj.GetComponent<Gameplay.PowerupPickup>();
            if (puBehavior != null)
            {
                GameManager.Instance?.ActivatePowerup(puBehavior.PowerupType);
            }
            puObj.SetActive(false);
            Audio.AudioManager.Instance?.PlayPowerupSound();
        }

        public void ActivateShield()
        {
            hasShield = true;
        }

        public void DeactivateShield()
        {
            hasShield = false;
        }

        public void Die()
        {
            if (isDead) return;
            isDead = true;
            forwardSpeed = 0f;
            SetAnimBool(AnimRun, false);
            SetAnimTrigger(AnimDead);
            Audio.AudioManager.Instance?.PlayCrashSound();
            Audio.AudioManager.Instance?.TriggerHaptic(100);
            GameManager.Instance?.OnPlayerDied();
        }

        public void ResetPlayer()
        {
            isDead = false;
            isJumping = false;
            isSliding = false;
            hasShield = false;
            verticalVelocity = 0f;
            forwardSpeed = Core.GameConstants.INITIAL_FORWARD_SPEED;
            DistanceTravelled = 0f;
            currentLane = Core.LaneIndex.Middle;
            targetXPosition = Core.GameConstants.LANE_MIDDLE;

            characterController.enabled = false;
            transform.position = new Vector3(Core.GameConstants.LANE_MIDDLE, 0f, 0f);
            characterController.enabled = true;
            characterController.height = normalHeight;
            characterController.center = new Vector3(0, normalCenterY, 0);

            SetAnimBool(AnimRun, true);
        }

        private float GetLaneXPosition(Core.LaneIndex lane)
        {
            switch (lane)
            {
                case Core.LaneIndex.Left: return Core.GameConstants.LANE_LEFT;
                case Core.LaneIndex.Middle: return Core.GameConstants.LANE_MIDDLE;
                case Core.LaneIndex.Right: return Core.GameConstants.LANE_RIGHT;
                default: return Core.GameConstants.LANE_MIDDLE;
            }
        }

        // Safe animation helpers (handle missing animator gracefully)
        private void SetAnimBool(int hash, bool value)
        {
            if (animator != null) animator.SetBool(hash, value);
        }

        private void SetAnimTrigger(int hash)
        {
            if (animator != null) animator.SetTrigger(hash);
        }

        private void SetAnimFloat(int hash, float value)
        {
            if (animator != null) animator.SetFloat(hash, value);
        }
    }
}
