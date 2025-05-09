using UnityEngine;

namespace MyGame
{
    /// <summary>
    /// Handles player aiming and firing of the orb.
    /// </summary>
    public class OrbFlinger : MonoBehaviour
    {
        #region Inspector Settings
        [Header("Prefabs & Spawn")]
        [Tooltip("Prefab for the orb projectile")]
        [SerializeField] private GameObject orbPrefab;

        [Tooltip("Where the orb spawns")]
        [SerializeField] private Transform launchPoint;

        [Header("Aim Parameters")]
        [SerializeField, Range(0f, 89f)] private float elevationDeg = 45f;
        [SerializeField, Range(-180f, 180f)] private float azimuthDeg   = 0f;
        [SerializeField, Range(0f, 10f)]  private float launchPower = 5f;

        [Header("Flight Settings")]
        [Tooltip("Time until orb auto‑respawns if nothing happens")]
        [SerializeField] private float maxFlightTime = 10f;
        #endregion

        private GameObject currentOrb;
        private float launchTime;
        private bool hasLaunched;

        void Awake()
        {
            if (orbPrefab == null || launchPoint == null)
                Debug.LogError("[OrbFlinger] Missing inspector reference!");
        }

        void Start()
        {
            SpawnOrb();
        }

        void Update()
        {
            HandleAimInput();

            if (!hasLaunched && Input.GetKeyDown(KeyCode.Space))
                FireOrb();

            if (hasLaunched && Time.time - launchTime > maxFlightTime)
                RespawnOrb();
        }

        #region Core Methods
        private void HandleAimInput()
        {
            elevationDeg = Mathf.Clamp(
                elevationDeg + Input.GetAxis("Vertical") * Time.deltaTime * 30f,
                0f, 89f
            );
            azimuthDeg = Mathf.Clamp(
                azimuthDeg + Input.GetAxis("Horizontal") * Time.deltaTime * 60f,
                -180f, 180f
            );
            // Q/E keys tweak power
            if (Input.GetKey(KeyCode.E)) launchPower = Mathf.Min(10f, launchPower + Time.deltaTime * 5f);
            if (Input.GetKey(KeyCode.Q)) launchPower = Mathf.Max(0f,  launchPower - Time.deltaTime * 5f);

            // Visualize current aim
            transform.rotation = Quaternion.Euler(elevationDeg, azimuthDeg, 0f);
        }

        private void FireOrb()
        {
            var rb = currentOrb.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            Vector3 dir = Quaternion.Euler(elevationDeg, azimuthDeg, 0f) * Vector3.forward;
            rb.AddForce(dir * launchPower * 100f);

            hasLaunched = true;
            launchTime = Time.time;

            Debug.Log($"[OrbFlinger] Fired! Elev={elevationDeg:F0}, Az={azimuthDeg:F0}, Pwr={launchPower:F1}");
        }

        private void SpawnOrb()
        {
            currentOrb = Instantiate(orbPrefab, launchPoint.position, Quaternion.identity);
            var tracker = currentOrb.GetComponent<OrbTracker>();
            tracker.Initialize(this);
            hasLaunched = false;
        }

        public void RespawnOrb()
        {
            Destroy(currentOrb);
            SpawnOrb();
            GameManager.Instance.ConsumeShot();
        }
        #endregion
    }
}
