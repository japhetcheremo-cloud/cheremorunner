using UnityEngine;
using System.Collections.Generic;
using CheremoRunner.Core;
using CheremoRunner.Gameplay;

namespace CheremoRunner.Environment
{
    public class TrackSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerTransform;

        private float spawnZ = 0.0f;
        private List<GameObject> activeSegments = new List<GameObject>();
        private const string POOL_TRACK = "TrackSegment";
        private const string POOL_JUMP = "BarrierJump";
        private const string POOL_SLIDE = "BarrierSlide";
        private const string POOL_ROADBLOCK = "Roadblock";
        private const string POOL_DRONE = "MovingDrone";
        private const string POOL_COIN = "Coin";
        private const string POOL_PU_MAGNET = "Powerup_Magnet";
        private const string POOL_PU_SHIELD = "Powerup_Shield";
        private const string POOL_PU_MULT = "Powerup_Multiplier";

        private void Start()
        {
            InitializePools();
            ResetTrack();
        }

        private void InitializePools()
        {
            if (ObjectPool.Instance == null) return;

            // Generate procedural template prefabs
            GameObject trackPrefab = ProceduralMeshBuilder.CreateTrackSegment(GameConstants.TRACK_SEGMENT_LENGTH);
            trackPrefab.AddComponent<TrackSegment>();
            ObjectPool.Instance.InitializePool(POOL_TRACK, trackPrefab, 10);
            Destroy(trackPrefab);

            GameObject jumpPrefab = ProceduralMeshBuilder.CreateBarrierJump();
            ObjectPool.Instance.InitializePool(POOL_JUMP, jumpPrefab, 12);
            Destroy(jumpPrefab);

            GameObject slidePrefab = ProceduralMeshBuilder.CreateBarrierSlide();
            ObjectPool.Instance.InitializePool(POOL_SLIDE, slidePrefab, 12);
            Destroy(slidePrefab);

            GameObject roadblockPrefab = ProceduralMeshBuilder.CreateRoadblock();
            ObjectPool.Instance.InitializePool(POOL_ROADBLOCK, roadblockPrefab, 12);
            Destroy(roadblockPrefab);

            GameObject dronePrefab = ProceduralMeshBuilder.CreateMovingDrone();
            ObjectPool.Instance.InitializePool(POOL_DRONE, dronePrefab, 8);
            Destroy(dronePrefab);

            GameObject coinPrefab = ProceduralMeshBuilder.CreateCoinPrefab();
            ObjectPool.Instance.InitializePool(POOL_COIN, coinPrefab, 60);
            Destroy(coinPrefab);

            GameObject puMag = ProceduralMeshBuilder.CreatePowerupPrefab(PowerupType.CoinMagnet);
            ObjectPool.Instance.InitializePool(POOL_PU_MAGNET, puMag, 4);
            Destroy(puMag);

            GameObject puShld = ProceduralMeshBuilder.CreatePowerupPrefab(PowerupType.Shield);
            ObjectPool.Instance.InitializePool(POOL_PU_SHIELD, puShld, 4);
            Destroy(puShld);

            GameObject puMult = ProceduralMeshBuilder.CreatePowerupPrefab(PowerupType.ScoreMultiplier2X);
            ObjectPool.Instance.InitializePool(POOL_PU_MULT, puMult, 4);
            Destroy(puMult);
        }

        private void Update()
        {
            if (playerTransform == null) return;

            // Check if we need to spawn a new segment ahead
            float distanceToFront = spawnZ - playerTransform.position.z;
            if (distanceToFront < GameConstants.TRACK_SEGMENT_LENGTH * 4)
            {
                SpawnTrackSegment();
            }

            // Recycle segments behind the player
            if (activeSegments.Count > 0)
            {
                GameObject oldestSegment = activeSegments[0];
                if (playerTransform.position.z - oldestSegment.transform.position.z > GameConstants.DESPAWN_DISTANCE_BEHIND_PLAYER)
                {
                    activeSegments.RemoveAt(0);
                    ObjectPool.Instance.ReturnToPool(POOL_TRACK, oldestSegment);
                }
            }

            // Magnet coin attraction
            if (GameManager.Instance != null && GameManager.Instance.IsMagnetActive)
            {
                AttractNearbyCoins();
            }
        }

        private void AttractNearbyCoins()
        {
            Collider[] colliders = Physics.OverlapSphere(playerTransform.position, GameConstants.MAGNET_ATTRACT_RADIUS);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag(GameConstants.TAG_COIN))
                {
                    Vector3 coinPos = colliders[i].transform.position;
                    Vector3 dir = (playerTransform.position + Vector3.up * 0.9f - coinPos).normalized;
                    colliders[i].transform.position = Vector3.MoveTowards(coinPos, playerTransform.position + Vector3.up * 0.9f,
                                                                         GameConstants.MAGNET_ATTRACT_SPEED * Time.deltaTime);
                }
            }
        }

        public void ResetTrack()
        {
            // Return all active track segments
            for (int i = 0; i < activeSegments.Count; i++)
            {
                if (activeSegments[i] != null && ObjectPool.Instance != null)
                {
                    ObjectPool.Instance.ReturnToPool(POOL_TRACK, activeSegments[i]);
                }
            }
            activeSegments.Clear();
            spawnZ = 0.0f;

            // Spawn initial safe straight segments (no obstacles on first 2 segments)
            for (int i = 0; i < GameConstants.INITIAL_TRACK_SEGMENTS; i++)
            {
                SpawnTrackSegment(i < 2);
            }
        }

        private void SpawnTrackSegment(bool isSafe = false)
        {
            if (ObjectPool.Instance == null) return;

            Vector3 spawnPos = new Vector3(0, 0, spawnZ);
            GameObject segmentObj = ObjectPool.Instance.SpawnFromPool(POOL_TRACK, spawnPos, Quaternion.identity);

            if (segmentObj != null)
            {
                activeSegments.Add(segmentObj);
                TrackSegment seg = segmentObj.GetComponent<TrackSegment>();

                if (!isSafe && seg != null)
                {
                    PopulateSegment(seg, spawnZ);
                }
            }

            spawnZ += GameConstants.TRACK_SEGMENT_LENGTH;
        }

        private void PopulateSegment(TrackSegment segment, float segmentZ)
        {
            float[] laneX = { GameConstants.LANE_LEFT, GameConstants.LANE_MIDDLE, GameConstants.LANE_RIGHT };
            int pattern = Random.Range(0, 5);

            switch (pattern)
            {
                case 0:
                    // Coin trail in one lane, single obstacle in another
                    int coinLane = Random.Range(0, 3);
                    SpawnCoinTrail(segment, laneX[coinLane], segmentZ + 6f, 5);
                    int obsLane = (coinLane + 1) % 3;
                    SpawnObstacle(segment, GetRandomObstacleTag(), laneX[obsLane], segmentZ + 16f);
                    break;

                case 1:
                    // Double obstacle (leaves 1 clear lane)
                    int clearLane = Random.Range(0, 3);
                    for (int l = 0; l < 3; l++)
                    {
                        if (l != clearLane)
                        {
                            SpawnObstacle(segment, GetRandomObstacleTag(), laneX[l], segmentZ + 14f);
                        }
                        else
                        {
                            SpawnCoinTrail(segment, laneX[l], segmentZ + 10f, 4);
                        }
                    }
                    break;

                case 2:
                    // Hurdle row (can jump over)
                    for (int l = 0; l < 3; l++)
                    {
                        SpawnObstacle(segment, POOL_JUMP, laneX[l], segmentZ + 14f);
                    }
                    // Arc coins above hurdles
                    for (int l = 0; l < 3; l++)
                    {
                        SpawnCoin(segment, laneX[l], segmentZ + 14f, 1.8f);
                    }
                    break;

                case 3:
                    // Low overhead signs across 2 lanes (can slide under)
                    int safeLane = Random.Range(0, 3);
                    for (int l = 0; l < 3; l++)
                    {
                        if (l != safeLane)
                        {
                            SpawnObstacle(segment, POOL_SLIDE, laneX[l], segmentZ + 15f);
                        }
                    }
                    SpawnCoinTrail(segment, laneX[safeLane], segmentZ + 8f, 5);
                    break;

                case 4:
                    // Moving drone in middle or side lane, with powerup chance!
                    int droneLane = Random.Range(0, 3);
                    SpawnObstacle(segment, POOL_DRONE, laneX[droneLane], segmentZ + 18f);

                    // Chance for powerup
                    if (Random.value < 0.35f)
                    {
                        int puLane = (droneLane + 1) % 3;
                        SpawnRandomPowerup(segment, laneX[puLane], segmentZ + 12f);
                    }
                    else
                    {
                        int cLane = (droneLane + 1) % 3;
                        SpawnCoinTrail(segment, laneX[cLane], segmentZ + 8f, 4);
                    }
                    break;
            }
        }

        private string GetRandomObstacleTag()
        {
            float roll = Random.value;
            if (roll < 0.40f) return POOL_JUMP;
            if (roll < 0.70f) return POOL_ROADBLOCK;
            return POOL_SLIDE;
        }

        private void SpawnObstacle(TrackSegment segment, string poolTag, float x, float z)
        {
            GameObject obs = ObjectPool.Instance.SpawnFromPool(poolTag, new Vector3(x, 0, z), Quaternion.identity);
            if (obs != null)
            {
                segment.RegisterChildObject(obs);
            }
        }

        private void SpawnCoin(TrackSegment segment, float x, float z, float y = 0.5f)
        {
            GameObject coin = ObjectPool.Instance.SpawnFromPool(POOL_COIN, new Vector3(x, y, z), Quaternion.identity);
            if (coin != null)
            {
                segment.RegisterChildObject(coin);
            }
        }

        private void SpawnCoinTrail(TrackSegment segment, float x, float startZ, int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnCoin(segment, x, startZ + i * 2.2f);
            }
        }

        private void SpawnRandomPowerup(TrackSegment segment, float x, float z)
        {
            string[] pus = { POOL_PU_MAGNET, POOL_PU_SHIELD, POOL_PU_MULT };
            string chosen = pus[Random.Range(0, pus.Length)];
            GameObject pu = ObjectPool.Instance.SpawnFromPool(chosen, new Vector3(x, 0.8f, z), Quaternion.identity);
            if (pu != null)
            {
                segment.RegisterChildObject(pu);
            }
        }
    }
}
