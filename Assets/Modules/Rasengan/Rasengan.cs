using UnityEngine;

namespace Modules.Rasengan
{
    public class Rasengan : MonoBehaviour
    {
        public Rigidbody rb { get; private set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            transform.localScale = Vector3.zero;
        }

        private void OnCollisionEnter(Collision _)
        {
            rb.AddExplosionForce(100, transform.position, 5);
            gameObject.SetActive(false);
        }
    }
}
