using UnityEngine;

namespace MyGame
{
    /// <summary>
    /// Watches orb collisions & position to notify GameManager.
    /// </summary>
    public class OrbTracker : MonoBehaviour
    {
        private OrbFlinger flinger;

        public void Initialize(OrbFlinger owner)
        {
            flinger = owner;
            GetComponent<Rigidbody>().isKinematic = true;
        }

        void OnCollisionEnter(Collision collision)
        {
            var tag = collision.gameObject.tag;
            if (tag == "Target")
            {
                Destroy(collision.gameObject);
                GameManager.Instance.RegisterHit();
                flinger.RespawnOrb();
            }
            else if (tag == "ExtraShot")
            {
                Destroy(collision.gameObject);
                GameManager.Instance.AddExtraShots(3);
                flinger.RespawnOrb();
            }
        }

        void Update()
        {
            if (transform.position.y < -5f)
                flinger.RespawnOrb();
        }
    }
}
