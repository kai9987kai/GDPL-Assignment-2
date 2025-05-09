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
        [SerializeField] private GameObject orbPrefab;
        [SerializeField] private Transform launchPoint;

        [Header("Aim Parameters")]
        [SerializeField, Range(0f, 89f)] private float elevationDeg = 45f;
        [SerializeField, Range(-180f, 180f)] private float azimuthDeg = 0f;
        [SerializeField, Range(0f, 10f)] private float launchPower = 5f;

        [Header("Flight Settings")]
        [SerializeField] private float maxFlightTime = 10f;
        #endregion

        #region Public Accessors
        public float ElevationDeg => elevationDeg;
        public float AzimuthDeg => azimuthDeg;
        public float LaunchPower => launchPower;
        #endregion

        private GameObject currentOrb;
        private float launchTime;
        private bool hasLaunched;

        void Awake()
        {
            if (orbPrefab == null || launchPoint == null)
                Debug.LogError("[OrbFlinger] Missing references!");
        }

        void Start() => SpawnOrb();

        void Update()
        {
            HandleAimInput();
            if (!hasLaunched && Input.GetKeyDown(KeyCode.Space))
                FireOrb();

            if (hasLaunched && Time.time - launchTime > maxFlightTime)
                RespawnOrb();
        }

        private void HandleAimInput()
        {
            elevationDeg = Mathf.Clamp(
                elevationDeg + Input.GetAxis("Vertical") * Time.deltaTime * 30f,
                0f, 89f);
            azimuthDeg = Mathf.Clamp(
                azimuthDeg + Input.GetAxis("Horizontal") * Time.deltaTime * 60f,
                -180f, 180f);

            if (Input.GetKey(KeyCode.E)) launchPower = Mathf.Min(10f, launchPower + Time.deltaTime * 5f);
            if (Input.GetKey(KeyCode.Q)) launchPower = Mathf.Max(0f, launchPower - Time.deltaTime * 5f);

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
            Debug.Log($"[OrbFlinger] Fired→ Elev:{elevationDeg:F0},Az:{azimuthDeg:F0},Pw:{launchPower:F1}");
        }

        private void SpawnOrb()
        {
            currentOrb = Instantiate(orbPrefab, launchPoint.position, Quaternion.identity);
            currentOrb.GetComponent<OrbTracker>().Initialize(this);
            hasLaunched = false;
        }

        public void RespawnOrb()
        {
            Destroy(currentOrb);
            SpawnOrb();
            GameManager.Instance.ConsumeShot();
        }
    }
}
