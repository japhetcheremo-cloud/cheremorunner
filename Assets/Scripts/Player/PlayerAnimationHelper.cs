using UnityEngine;

namespace CheremoRunner.Player
{
    public class PlayerAnimationHelper : MonoBehaviour
    {
        [Header("Limbs")]
        [SerializeField] private Transform torso;
        [SerializeField] private Transform head;
        [SerializeField] private Transform leftArm;
        [SerializeField] private Transform rightArm;
        [SerializeField] private Transform leftLeg;
        [SerializeField] private Transform rightLeg;

        private PlayerController playerController;
        private float runCycle = 0f;

        private void Start()
        {
            playerController = GetComponent<PlayerController>();
            FindLimbsIfNull();
        }

        public void FindLimbsIfNull()
        {
            if (torso == null) torso = transform.Find("Torso");
            if (head == null) head = transform.Find("Head");
            if (leftArm == null) leftArm = transform.Find("LeftArm");
            if (rightArm == null) rightArm = transform.Find("RightArm");
            if (leftLeg == null) leftLeg = transform.Find("LeftLeg");
            if (rightLeg == null) rightLeg = transform.Find("RightLeg");
        }

        private void Update()
        {
            if (playerController == null || playerController.IsDead) return;

            float speed = playerController.ForwardSpeed;
            float dt = Time.deltaTime;

            runCycle += dt * speed * 2.8f;

            float legAngle = Mathf.Sin(runCycle) * 35.0f;
            float armAngle = -legAngle * 0.9f;

            if (leftLeg != null) leftLeg.localRotation = Quaternion.Euler(legAngle, 0, 0);
            if (rightLeg != null) rightLeg.localRotation = Quaternion.Euler(-legAngle, 0, 0);

            if (leftArm != null) leftArm.localRotation = Quaternion.Euler(armAngle, 0, 0);
            if (rightArm != null) rightArm.localRotation = Quaternion.Euler(-armAngle, 0, 0);

            // Subtle torso bounce
            if (torso != null)
            {
                float bounce = Mathf.Abs(Mathf.Sin(runCycle * 2f)) * 0.05f;
                torso.localPosition = new Vector3(0, 1.1f + bounce, 0);
            }
        }
    }
}
