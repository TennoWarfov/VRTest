using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Obi.Samples
{
    public class FirstPersonLauncher : MonoBehaviour
    {
        //public ObiColliderGroup colliderGroup;
        public GameObject prefab;
        public float power = 2;

        // Update is called once per frame
        void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                GameObject projectile = Instantiate(prefab, ray.origin, Quaternion.identity);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.linearVelocity = ray.direction * power;
                }
            }
        }
    }
}
