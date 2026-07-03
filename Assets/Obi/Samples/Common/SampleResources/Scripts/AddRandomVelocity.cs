using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Obi.Samples
{
    [RequireComponent(typeof(ObiActor))]
    public class AddRandomVelocity : MonoBehaviour
    {
        public float intensity = 5;

        void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                GetComponent<ObiActor>()
                    .AddForce(Random.onUnitSphere * intensity, ForceMode.VelocityChange);
            }
        }
    }
}
