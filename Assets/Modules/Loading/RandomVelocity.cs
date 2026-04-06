using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Random = UnityEngine.Random;

namespace Modules.Loading
{
    public class RandomVelocity : MonoBehaviour
    {
        [Header("Perlin motion")]
        public float noiseScale = 0.15f;
        public float forceStrength = 0.04f;

        [Header("Rotation")]
        public float torqueStrength = 1f;
        public float rotationSmoothness = 2f;

        [Header("Velocity limits")]
        public float maxVelocity = 0.4f;
        public float maxAngularVelocity = 1.2f;

        [Header("Sphere constraint")]
        public Transform center;
        public float radius = 2f;
        public float returnForce = 0.08f;

        private Rigidbody rb;
        private XRGrabInteractable _grab;
        private Vector3 noiseOffset;
        private Vector3 currentTorque;
        private bool _canProcess = true;
        private CancellationTokenSource _cts;

        public void Initialize(Transform centerPoint)
        {
            rb = GetComponent<Rigidbody>();
            _grab = GetComponent<XRGrabInteractable>();
            center = centerPoint;

            _grab.selectEntered.AddListener(SelectEntered);
            _grab.selectExited.AddListener(SelectExited);

            rb.useGravity = false;
            rb.linearDamping = 0.2f;
            rb.angularDamping = 0.6f;

            // стабилизация инерции (уменьшает странные вращения)
            rb.inertiaTensor = Vector3.one;
            rb.inertiaTensorRotation = Quaternion.identity;

            noiseOffset = new Vector3(
                Random.value * 100f,
                Random.value * 100f,
                Random.value * 100f
            );
        }

        private void OnDestroy()
        {
            _grab.selectEntered.RemoveListener(SelectEntered);
            _grab.selectExited.RemoveListener(SelectExited);
        }

        private void SelectEntered(SelectEnterEventArgs arg0) => _cts?.Cancel();

        private async void SelectExited(SelectExitEventArgs arg0)
        {
            try
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = new CancellationTokenSource();
                await CanProcess(_cts.Token);
            }
            catch (Exception)
            {
                _canProcess = false;
            }
        }

        private async Task CanProcess(CancellationToken ct)
        {
            _canProcess = false;
            await Task.Delay(TimeSpan.FromSeconds(3), ct);
            _canProcess = true;
        }

        void FixedUpdate()
        {
            if (!_canProcess || _grab.isSelected || !center)
                return;

            ApplyPerlinMotion();
            ApplySmoothRotation();
            KeepInsideSphere();
            LimitVelocities();
        }

        void ApplyPerlinMotion()
        {
            float t = Time.time * noiseScale;

            Vector3 dir = new Vector3(
                Mathf.PerlinNoise(t + noiseOffset.x, noiseOffset.y) - 0.5f,
                Mathf.PerlinNoise(noiseOffset.x, t + noiseOffset.y) - 0.5f,
                Mathf.PerlinNoise(t + noiseOffset.z, t + noiseOffset.x) - 0.5f
            );

            rb.AddForce(dir * forceStrength, ForceMode.Acceleration);
        }

        void ApplySmoothRotation()
        {
            float t = Time.time * noiseScale;

            Vector3 targetTorque = new Vector3(
                Mathf.PerlinNoise(t + noiseOffset.z, noiseOffset.x) - 0.5f,
                Mathf.PerlinNoise(noiseOffset.y, t + noiseOffset.z) - 0.5f,
                Mathf.PerlinNoise(t + noiseOffset.x, t + noiseOffset.y) - 0.5f
            );

            // сглаживание (ключ к отсутствию рывков)
            currentTorque = Vector3.Lerp(
                currentTorque,
                targetTorque,
                Time.fixedDeltaTime * rotationSmoothness
            );

            rb.AddTorque(currentTorque * torqueStrength, ForceMode.Acceleration);
        }

        void KeepInsideSphere()
        {
            Vector3 toCenter = center.position - transform.position;
            float dist = toCenter.magnitude;

            if (dist > radius)
            {
                float strength = (dist - radius) / radius;

                rb.AddForce(toCenter.normalized * returnForce * strength, ForceMode.Acceleration);
            }
        }

        void LimitVelocities()
        {
            // мягкое ограничение линейной скорости
            if (rb.linearVelocity.magnitude > maxVelocity)
            {
                rb.linearVelocity = Vector3.Lerp(
                    rb.linearVelocity,
                    rb.linearVelocity.normalized * maxVelocity,
                    0.1f
                );
            }

            // мягкое ограничение угловой скорости
            if (rb.angularVelocity.magnitude > maxAngularVelocity)
            {
                rb.angularVelocity = Vector3.Lerp(
                    rb.angularVelocity,
                    rb.angularVelocity.normalized * maxAngularVelocity,
                    0.1f
                );
            }
        }
    }
}
