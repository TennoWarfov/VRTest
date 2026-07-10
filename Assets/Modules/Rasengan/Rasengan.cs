using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Modules.Rasengan
{
    public class Rasengan : MonoBehaviour
    {
        public Rigidbody rb { get; private set; }

        [SerializeField]
        private Transform circle;

        [SerializeField]
        private Transform shuriken;

        [SerializeField]
        private AnimationCurve curve;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        private async void Start()
        {
            try
            {
                await StartupAnimation();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in rasengan init {nameof(Rasengan)}: {e.Message}");
            }
        }

        private void OnCollisionEnter(Collision _)
        {
            rb.AddExplosionForce(100, transform.position, 100);
            gameObject.SetActive(false);
        }

        private async Task StartupAnimation()
        {
            circle.localScale = Vector3.zero;
            shuriken.localScale = Vector3.zero;

            await GrowingAnimation(circle);
            await Task.Delay(TimeSpan.FromSeconds(0.5f));
            await GrowingAnimation(shuriken);
        }

        private async Task GrowingAnimation(Transform tr)
        {
            var startScale = tr.localScale;
            var elapsedTime = 0f;
            const float duration = 2f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                tr.localScale = Vector3.Lerp(
                    startScale,
                    Vector3.one,
                    curve.Evaluate(elapsedTime / duration)
                );
                await Task.Yield();
            }
            tr.localScale = Vector3.Lerp(startScale, Vector3.one, curve.Evaluate(1f));
        }
    }
}
