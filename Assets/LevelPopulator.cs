using UnityEngine;

namespace MyGame
{
    /// <summary>
    /// Randomly scatters targets, extras & obstacles on a platform.
    /// </summary>
    public class LevelPopulator : MonoBehaviour
    {
        [SerializeField] private GameObject[] spawnPrefabs;
        [SerializeField] private int spawnCount = 12;
        [SerializeField] private Vector3 areaMin, areaMax;

        void Start()
        {
            for (int i = 0; i < spawnCount; i++)
                ScatterOne();
        }

        private void ScatterOne()
        {
            int idx = Random.Range(0, spawnPrefabs.Length);
            Vector3 pos = new Vector3(
                Random.Range(areaMin.x, areaMax.x),
                Random.Range(areaMin.y, areaMax.y),
                Random.Range(areaMin.z, areaMax.z)
            );
            Instantiate(spawnPrefabs[idx], pos, Quaternion.identity);
        }
    }
}
