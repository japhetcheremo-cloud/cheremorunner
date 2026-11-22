using UnityEngine;

namespace CheremoRunner.Gameplay
{
    /// <summary>
    /// Generates 3D meshes procedurally at runtime for all game objects.
    /// This avoids relying on external model files.
    /// </summary>
    public static class ProceduralMeshBuilder
    {
        #region Player Character

        public static GameObject CreateRunnerCharacter(Color bodyColor, Color accentColor, Color glowColor)
        {
            GameObject root = new GameObject("RunnerCharacter");

            // --- Torso ---
            GameObject torso = CreateCube("Torso", new Vector3(0.5f, 0.65f, 0.3f));
            torso.transform.SetParent(root.transform);
            torso.transform.localPosition = new Vector3(0, 1.1f, 0);
            SetMeshColor(torso, bodyColor);

            // --- Head ---
            GameObject head = CreateSphere("Head", 0.22f, 12);
            head.transform.SetParent(root.transform);
            head.transform.localPosition = new Vector3(0, 1.72f, 0);
            SetMeshColor(head, new Color(0.92f, 0.78f, 0.65f));

            // Visor / sunglasses
            GameObject visor = CreateCube("Visor", new Vector3(0.3f, 0.08f, 0.15f));
            visor.transform.SetParent(head.transform);
            visor.transform.localPosition = new Vector3(0, -0.02f, 0.12f);
            SetMeshColor(visor, glowColor);

            // --- Left Arm ---
            GameObject leftArm = CreateCube("LeftArm", new Vector3(0.14f, 0.55f, 0.14f));
            leftArm.transform.SetParent(root.transform);
            leftArm.transform.localPosition = new Vector3(-0.38f, 1.1f, 0);
            SetMeshColor(leftArm, accentColor);

            // --- Right Arm ---
            GameObject rightArm = CreateCube("RightArm", new Vector3(0.14f, 0.55f, 0.14f));
            rightArm.transform.SetParent(root.transform);
            rightArm.transform.localPosition = new Vector3(0.38f, 1.1f, 0);
            SetMeshColor(rightArm, accentColor);

            // --- Left Leg ---
            GameObject leftLeg = CreateCube("LeftLeg", new Vector3(0.18f, 0.6f, 0.18f));
            leftLeg.transform.SetParent(root.transform);
            leftLeg.transform.localPosition = new Vector3(-0.14f, 0.3f, 0);
            SetMeshColor(leftLeg, bodyColor * 0.75f);

            // --- Right Leg ---
            GameObject rightLeg = CreateCube("RightLeg", new Vector3(0.18f, 0.6f, 0.18f));
            rightLeg.transform.SetParent(root.transform);
            rightLeg.transform.localPosition = new Vector3(0.14f, 0.3f, 0);
            SetMeshColor(rightLeg, bodyColor * 0.75f);

            // --- Shoes ---
            GameObject leftShoe = CreateCube("LeftShoe", new Vector3(0.2f, 0.1f, 0.3f));
            leftShoe.transform.SetParent(root.transform);
            leftShoe.transform.localPosition = new Vector3(-0.14f, 0.05f, 0.05f);
            SetMeshColor(leftShoe, glowColor);

            GameObject rightShoe = CreateCube("RightShoe", new Vector3(0.2f, 0.1f, 0.3f));
            rightShoe.transform.SetParent(root.transform);
            rightShoe.transform.localPosition = new Vector3(0.14f, 0.05f, 0.05f);
            SetMeshColor(rightShoe, glowColor);

            // Add CharacterController
            CharacterController cc = root.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.center = new Vector3(0, 0.9f, 0);
            cc.radius = 0.35f;

            root.tag = Core.GameConstants.TAG_PLAYER;
            root.layer = LayerMask.NameToLayer("Default");

            return root;
        }

        #endregion

        #region Track Segment

        public static GameObject CreateTrackSegment(float length)
        {
            GameObject segment = new GameObject("TrackSegment");

            float roadWidth = 7.2f; // 3 lanes * 2.4f
            float sidewalkWidth = 2.5f;

            // --- Main Road ---
            GameObject road = CreateCube("Road", new Vector3(roadWidth, 0.1f, length));
            road.transform.SetParent(segment.transform);
            road.transform.localPosition = new Vector3(0, -0.05f, length / 2f);
            SetMeshColor(road, new Color(0.14f, 0.15f, 0.18f));
            road.tag = Core.GameConstants.TAG_GROUND;

            // --- Lane Divider Lines (dashed) ---
            float dividerSpacing = Core.GameConstants.LANE_DISTANCE;
            CreateLaneDividers(segment, -dividerSpacing / 2f, length);
            CreateLaneDividers(segment, dividerSpacing / 2f, length);

            // --- Edge Lines (solid neon) ---
            CreateEdgeLine(segment, -roadWidth / 2f, length, new Color(0, 0.86f, 1f));
            CreateEdgeLine(segment, roadWidth / 2f, length, new Color(0, 0.86f, 1f));

            // --- Sidewalks ---
            CreateSidewalk(segment, -(roadWidth / 2f + sidewalkWidth / 2f + 0.15f), sidewalkWidth, length);
            CreateSidewalk(segment, (roadWidth / 2f + sidewalkWidth / 2f + 0.15f), sidewalkWidth, length);

            // --- Buildings on both sides ---
            CreateBuildingRow(segment, -(roadWidth / 2f + sidewalkWidth + 2.5f), length, true);
            CreateBuildingRow(segment, (roadWidth / 2f + sidewalkWidth + 2.5f), length, false);

            // --- Street Lamps ---
            for (float z = 4f; z < length; z += 16f)
            {
                CreateStreetLamp(segment, -(roadWidth / 2f + 0.6f), z);
                CreateStreetLamp(segment, (roadWidth / 2f + 0.6f), z);
            }

            return segment;
        }

        private static void CreateLaneDividers(GameObject parent, float xPos, float trackLength)
        {
            float dashLength = 2.0f;
            float gapLength = 2.5f;
            for (float z = 0; z < trackLength; z += dashLength + gapLength)
            {
                GameObject dash = CreateCube("LaneDash", new Vector3(0.12f, 0.015f, dashLength));
                dash.transform.SetParent(parent.transform);
                dash.transform.localPosition = new Vector3(xPos, 0.001f, z + dashLength / 2f);
                SetMeshColor(dash, new Color(1f, 0.84f, 0, 0.9f));
            }
        }

        private static void CreateEdgeLine(GameObject parent, float xPos, float trackLength, Color color)
        {
            GameObject line = CreateCube("EdgeLine", new Vector3(0.1f, 0.02f, trackLength));
            line.transform.SetParent(parent.transform);
            line.transform.localPosition = new Vector3(xPos, 0.001f, trackLength / 2f);
            SetMeshColor(line, color);
            SetEmission(line, color, 1.5f);
        }

        private static void CreateSidewalk(GameObject parent, float xPos, float width, float length)
        {
            GameObject sw = CreateCube("Sidewalk", new Vector3(width, 0.15f, length));
            sw.transform.SetParent(parent.transform);
            sw.transform.localPosition = new Vector3(xPos, 0.025f, length / 2f);
            SetMeshColor(sw, new Color(0.3f, 0.32f, 0.36f));
        }

        private static void CreateBuildingRow(GameObject parent, float baseX, float trackLength, bool leftSide)
        {
            float z = 2f;
            int seed = Mathf.Abs((int)(baseX * 1000));
            System.Random rng = new System.Random(seed);

            while (z < trackLength - 4f)
            {
                float buildWidth = 2.5f + (float)(rng.NextDouble() * 3.5f);
                float buildHeight = 6.0f + (float)(rng.NextDouble() * 18.0f);
                float buildDepth = 3.0f + (float)(rng.NextDouble() * 2.5f);
                float xOffset = leftSide ? baseX - buildDepth / 2f : baseX + buildDepth / 2f;

                GameObject bld = CreateCube("Building", new Vector3(buildDepth, buildHeight, buildWidth));
                bld.transform.SetParent(parent.transform);
                bld.transform.localPosition = new Vector3(xOffset, buildHeight / 2f, z + buildWidth / 2f);

                // Dark city building color with slight variation
                float cv = 0.08f + (float)(rng.NextDouble() * 0.1f);
                SetMeshColor(bld, new Color(cv, cv * 1.1f, cv * 1.4f));

                // Window strip glow
                for (float wy = 2f; wy < buildHeight - 1f; wy += 2.2f)
                {
                    float face_x = leftSide ? xOffset + buildDepth / 2f + 0.01f : xOffset - buildDepth / 2f - 0.01f;
                    GameObject winRow = CreateCube("Windows", new Vector3(0.02f, 0.6f, buildWidth * 0.8f));
                    winRow.transform.SetParent(bld.transform);
                    winRow.transform.localPosition = new Vector3(face_x - bld.transform.localPosition.x,
                                                                   wy - buildHeight / 2f, 0);
                    Color wc = rng.NextDouble() < 0.5 ? new Color(0, 0.86f, 1f, 0.5f) : new Color(1f, 0.78f, 0.3f, 0.5f);
                    SetMeshColor(winRow, wc);
                    SetEmission(winRow, wc, 0.8f);
                }

                z += buildWidth + 0.8f + (float)(rng.NextDouble() * 2.0f);
            }
        }

        private static void CreateStreetLamp(GameObject parent, float xPos, float zPos)
        {
            // Pole
            GameObject pole = CreateCube("LampPole", new Vector3(0.08f, 4.5f, 0.08f));
            pole.transform.SetParent(parent.transform);
            pole.transform.localPosition = new Vector3(xPos, 2.25f, zPos);
            SetMeshColor(pole, new Color(0.25f, 0.27f, 0.3f));

            // Lamp head
            GameObject lamp = CreateCube("LampHead", new Vector3(0.3f, 0.12f, 0.3f));
            lamp.transform.SetParent(pole.transform);
            lamp.transform.localPosition = new Vector3(0, 2.3f, 0);
            SetMeshColor(lamp, new Color(1f, 0.95f, 0.7f));
            SetEmission(lamp, new Color(1f, 0.9f, 0.6f), 2.5f);

            // Point light (expensive; only add on high quality)
            if (QualitySettings.GetQualityLevel() >= 2)
            {
                GameObject lightObj = new GameObject("LampLight");
                lightObj.transform.SetParent(lamp.transform);
                lightObj.transform.localPosition = Vector3.down * 0.1f;
                Light pointLight = lightObj.AddComponent<Light>();
                pointLight.type = LightType.Point;
                pointLight.range = 8f;
                pointLight.intensity = 1.2f;
                pointLight.color = new Color(1f, 0.92f, 0.7f);
                pointLight.shadows = LightShadows.None;
            }
        }

        #endregion

        #region Obstacles

        public static GameObject CreateBarrierJump()
        {
            GameObject obj = new GameObject("BarrierJump");

            // Horizontal bar at knee height
            GameObject bar = CreateCube("Bar", new Vector3(2.2f, 0.2f, 0.2f));
            bar.transform.SetParent(obj.transform);
            bar.transform.localPosition = new Vector3(0, 0.5f, 0);
            SetMeshColor(bar, new Color(1f, 0.3f, 0.15f));
            SetEmission(bar, new Color(1f, 0.2f, 0.1f), 1.0f);

            // Support posts
            for (int s = -1; s <= 1; s += 2)
            {
                GameObject post = CreateCube("Post", new Vector3(0.1f, 0.5f, 0.1f));
                post.transform.SetParent(obj.transform);
                post.transform.localPosition = new Vector3(s * 1.0f, 0.25f, 0);
                SetMeshColor(post, new Color(0.6f, 0.1f, 0.08f));
            }

            // Warning stripes on bar
            for (float sx = -0.9f; sx < 0.9f; sx += 0.4f)
            {
                GameObject stripe = CreateCube("Stripe", new Vector3(0.15f, 0.22f, 0.22f));
                stripe.transform.SetParent(bar.transform);
                stripe.transform.localPosition = new Vector3(sx, 0, 0);
                SetMeshColor(stripe, Color.yellow);
            }

            AddObstacleCollider(obj, new Vector3(2.2f, 0.9f, 0.3f), new Vector3(0, 0.45f, 0));
            obj.tag = Core.GameConstants.TAG_OBSTACLE;
            return obj;
        }

        public static GameObject CreateBarrierSlide()
        {
            GameObject obj = new GameObject("BarrierSlide");

            // Overhead horizontal sign/bar at chest height
            GameObject bar = CreateCube("OverheadBar", new Vector3(2.4f, 0.4f, 0.15f));
            bar.transform.SetParent(obj.transform);
            bar.transform.localPosition = new Vector3(0, 1.3f, 0);
            SetMeshColor(bar, new Color(0.9f, 0.5f, 0));
            SetEmission(bar, new Color(1f, 0.6f, 0), 0.6f);

            // Support columns
            for (int s = -1; s <= 1; s += 2)
            {
                GameObject col = CreateCube("Column", new Vector3(0.12f, 2.6f, 0.12f));
                col.transform.SetParent(obj.transform);
                col.transform.localPosition = new Vector3(s * 1.15f, 1.3f, 0);
                SetMeshColor(col, new Color(0.4f, 0.42f, 0.5f));
            }

            AddObstacleCollider(obj, new Vector3(2.4f, 1.2f, 0.3f), new Vector3(0, 1.4f, 0));
            obj.tag = Core.GameConstants.TAG_OBSTACLE;
            return obj;
        }

        public static GameObject CreateRoadblock()
        {
            GameObject obj = new GameObject("Roadblock");

            // Tall concrete barrier
            GameObject barrier = CreateCube("TallBarrier", new Vector3(1.8f, 2.0f, 0.6f));
            barrier.transform.SetParent(obj.transform);
            barrier.transform.localPosition = new Vector3(0, 1.0f, 0);
            SetMeshColor(barrier, new Color(0.22f, 0.24f, 0.3f));

            // Hazard stripe
            GameObject hazard = CreateCube("HazardStripe", new Vector3(1.82f, 0.25f, 0.62f));
            hazard.transform.SetParent(obj.transform);
            hazard.transform.localPosition = new Vector3(0, 0.6f, 0);
            SetMeshColor(hazard, new Color(1f, 0.85f, 0));

            // Top warning light
            GameObject warnLight = CreateSphere("WarningLight", 0.12f, 8);
            warnLight.transform.SetParent(obj.transform);
            warnLight.transform.localPosition = new Vector3(0, 2.15f, 0);
            SetMeshColor(warnLight, new Color(1f, 0.1f, 0));
            SetEmission(warnLight, new Color(1f, 0, 0), 2.0f);

            AddObstacleCollider(obj, new Vector3(1.8f, 2.2f, 0.6f), new Vector3(0, 1.1f, 0));
            obj.tag = Core.GameConstants.TAG_OBSTACLE;
            return obj;
        }

        public static GameObject CreateMovingDrone()
        {
            GameObject obj = new GameObject("MovingDrone");

            // Hovering platform body
            GameObject body = CreateCube("DroneBody", new Vector3(1.0f, 0.3f, 1.4f));
            body.transform.SetParent(obj.transform);
            body.transform.localPosition = new Vector3(0, 1.0f, 0);
            SetMeshColor(body, new Color(0.15f, 0.15f, 0.2f));

            // Propeller arms
            for (int s = -1; s <= 1; s += 2)
            {
                GameObject arm = CreateCube("Arm", new Vector3(0.8f, 0.06f, 0.06f));
                arm.transform.SetParent(body.transform);
                arm.transform.localPosition = new Vector3(s * 0.3f, 0.18f, 0);
                SetMeshColor(arm, new Color(0.3f, 0.3f, 0.35f));

                GameObject propeller = CreateCube("Propeller", new Vector3(0.5f, 0.03f, 0.1f));
                propeller.transform.SetParent(arm.transform);
                propeller.transform.localPosition = new Vector3(s * 0.45f, 0.06f, 0);
                SetMeshColor(propeller, new Color(0, 0.9f, 1f, 0.6f));
            }

            // Glowing scanner underneath
            GameObject scanner = CreateCube("Scanner", new Vector3(0.6f, 0.05f, 0.8f));
            scanner.transform.SetParent(body.transform);
            scanner.transform.localPosition = new Vector3(0, -0.18f, 0);
            SetMeshColor(scanner, new Color(1f, 0.1f, 0.1f));
            SetEmission(scanner, new Color(1f, 0, 0), 2.0f);

            obj.AddComponent<MovingObstacle>();
            AddObstacleCollider(obj, new Vector3(1.2f, 0.8f, 1.6f), new Vector3(0, 1.0f, 0));
            obj.tag = Core.GameConstants.TAG_OBSTACLE;
            return obj;
        }

        #endregion

        #region Coins & Powerups

        public static GameObject CreateCoinPrefab()
        {
            GameObject obj = new GameObject("Coin");

            // Flat cylinder approximation with two faces
            GameObject disc = CreateCube("CoinDisc", new Vector3(0.45f, 0.45f, 0.08f));
            disc.transform.SetParent(obj.transform);
            disc.transform.localPosition = Vector3.zero;
            SetMeshColor(disc, new Color(1f, 0.84f, 0));
            SetEmission(disc, new Color(1f, 0.7f, 0), 1.2f);

            // Spin component
            obj.AddComponent<CoinSpin>();

            // Trigger collider
            SphereCollider sc = obj.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = 0.4f;
            sc.center = Vector3.zero;

            obj.tag = Core.GameConstants.TAG_COIN;
            return obj;
        }

        public static GameObject CreatePowerupPrefab(Core.PowerupType type)
        {
            GameObject obj = new GameObject("Powerup_" + type.ToString());

            Color puColor;
            switch (type)
            {
                case Core.PowerupType.CoinMagnet:
                    puColor = new Color(0.2f, 0.6f, 1f);
                    break;
                case Core.PowerupType.Shield:
                    puColor = new Color(0.1f, 1f, 0.3f);
                    break;
                case Core.PowerupType.ScoreMultiplier2X:
                    puColor = new Color(1f, 0.4f, 0.9f);
                    break;
                default:
                    puColor = Color.white;
                    break;
            }

            // Glowing sphere
            GameObject sphere = CreateSphere("PUSphere", 0.35f, 12);
            sphere.transform.SetParent(obj.transform);
            SetMeshColor(sphere, puColor);
            SetEmission(sphere, puColor, 2.0f);

            // Inner cube
            GameObject inner = CreateCube("PUInner", new Vector3(0.25f, 0.25f, 0.25f));
            inner.transform.SetParent(obj.transform);
            SetMeshColor(inner, Color.white);

            obj.AddComponent<CoinSpin>(); // Reuse spin

            PowerupPickup pp = obj.AddComponent<PowerupPickup>();
            pp.PowerupType = type;

            SphereCollider sc = obj.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = 0.5f;
            sc.center = Vector3.zero;

            obj.tag = Core.GameConstants.TAG_POWERUP;
            return obj;
        }

        #endregion

        #region Primitive Helpers

        public static GameObject CreateCube(string name, Vector3 size)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.localScale = size;
            // Remove default collider; we add our own
            Object.Destroy(cube.GetComponent<Collider>());
            return cube;
        }

        public static GameObject CreateSphere(string name, float radius, int segments = 16)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = name;
            sphere.transform.localScale = Vector3.one * radius * 2f;
            Object.Destroy(sphere.GetComponent<Collider>());
            return sphere;
        }

        public static void SetMeshColor(GameObject obj, Color color)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                if (mat.shader.name == "Hidden/InternalErrorShader")
                {
                    mat = new Material(Shader.Find("Standard"));
                }
                mat.color = color;
                if (color.a < 1f)
                {
                    SetTransparentMode(mat);
                }
                rend.material = mat;
            }
        }

        public static void SetEmission(GameObject obj, Color emissionColor, float intensity)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null && rend.material != null)
            {
                Material mat = rend.material;
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emissionColor * intensity);
            }
        }

        private static void SetTransparentMode(Material mat)
        {
            mat.SetFloat("_Mode", 3); // Transparent
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }

        private static void AddObstacleCollider(GameObject obj, Vector3 size, Vector3 center)
        {
            BoxCollider bc = obj.AddComponent<BoxCollider>();
            bc.size = size;
            bc.center = center;
            bc.isTrigger = false;
        }

        #endregion
    }

    // === Coin Spin Component ===
    public class CoinSpin : MonoBehaviour
    {
        private float rotateSpeed;
        private float bobSpeed;
        private float bobHeight;
        private Vector3 startPos;

        private void OnEnable()
        {
            rotateSpeed = Random.Range(120f, 200f);
            bobSpeed = Random.Range(2.0f, 3.5f);
            bobHeight = Random.Range(0.08f, 0.18f);
            startPos = transform.position;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
            Vector3 pos = startPos;
            pos.y += Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = pos;
        }

        public void ResetPosition(Vector3 newPos)
        {
            startPos = newPos;
            transform.position = newPos;
        }
    }

    // === Powerup Pickup Component ===
    public class PowerupPickup : MonoBehaviour
    {
        public Core.PowerupType PowerupType;
    }

    // === Moving Obstacle Component ===
    public class MovingObstacle : MonoBehaviour, IPoolable
    {
        [SerializeField] private float lateralSpeed = 3.0f;
        [SerializeField] private float lateralRange = 2.4f;

        private float startX;
        private int direction = 1;

        public void OnSpawnFromPool()
        {
            startX = transform.position.x;
            direction = Random.value > 0.5f ? 1 : -1;
        }

        public void OnReturnToPool() { }

        private void Update()
        {
            float newX = transform.position.x + direction * lateralSpeed * Time.deltaTime;

            if (Mathf.Abs(newX - startX) > lateralRange)
            {
                direction *= -1;
                newX = startX + direction * lateralRange;
            }

            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }
}
