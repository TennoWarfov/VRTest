using System.Collections.Generic;
using UnityEngine;

namespace Modules.Softbody
{
    public class SoftbodyController : MonoBehaviour
    {
        [Header("Collider")]
        [SerializeField]
        private CapsuleCollider capsule;

        [Header("Deformation")]
        [SerializeField]
        private float deformationStrength = 1.0f;

        [SerializeField]
        private float deformationSpeed = 20f;

        [SerializeField]
        private float recoverySpeed = 5f;

        [SerializeField]
        private float maxDisplacement = 0.05f;

        private SkinnedMeshRenderer skinnedMeshRenderer;

        private Mesh runtimeMesh;
        private Mesh bakedMesh;

        private Vector3[] originalVertices;
        private Vector3[] currentVertices;

        private void Awake()
        {
            // skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            //
            // runtimeMesh = Instantiate(skinnedMeshRenderer.sharedMesh);
            //
            // originalVertices = runtimeMesh.vertices;
            // currentVertices = runtimeMesh.vertices;
            //
            // bakedMesh = new Mesh();
            //
            // skinnedMeshRenderer.sharedMesh = runtimeMesh;

            var points = distributePoints(100, 10f);
            foreach (var point in points)
            {
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = point;
            }
        }

        public static List<Vector3> distributePoints(int n, float length)
        {
            List<Vector3> points = new List<Vector3>();

            float goldenRatio = (1 + Mathf.Pow(5, 0.5f)) / 2f;

            for (int i = 0; i < n; i++)
            {
                float theta = 2 * Mathf.PI * i / goldenRatio;
                float phi = Mathf.Acos(1 - 2 * (i + 0.5f) / n);
                Vector3 point =
                    new Vector3(
                        Mathf.Cos(theta) * Mathf.Sin(phi),
                        Mathf.Sin(theta) * Mathf.Sin(phi),
                        Mathf.Cos(phi)
                    ) * length;
                points.Add(point);
            }

            return points;
        }

        // private void LateUpdate()
        // {
        //     if (capsule == null)
        //         return;
        //
        //     skinnedMeshRenderer.BakeMesh(bakedMesh);
        //
        //     Vector3[] bakedVertices = bakedMesh.vertices;
        //
        //     GetCapsuleData(
        //         capsule,
        //         out Vector3 capsuleStart,
        //         out Vector3 capsuleEnd,
        //         out float capsuleRadius
        //     );
        //
        //     for (int i = 0; i < bakedVertices.Length; i++)
        //     {
        //         Vector3 worldVertex = transform.TransformPoint(bakedVertices[i]);
        //
        //         Vector3 closestPointOnAxis = ClosestPointOnSegment(
        //             capsuleStart,
        //             capsuleEnd,
        //             worldVertex
        //         );
        //
        //         Vector3 delta = worldVertex - closestPointOnAxis;
        //
        //         float distanceToAxis = delta.magnitude;
        //
        //         bool insideCapsule = distanceToAxis < capsuleRadius;
        //
        //         if (insideCapsule)
        //         {
        //             float penetration = capsuleRadius - distanceToAxis;
        //
        //             float weight = penetration / capsuleRadius;
        //
        //             Vector3 pushDirection;
        //
        //             if (distanceToAxis > 0.0001f)
        //             {
        //                 pushDirection = delta.normalized;
        //             }
        //             else
        //             {
        //                 pushDirection = (worldVertex - capsule.transform.position).normalized;
        //             }
        //
        //             Vector3 localDirection = transform.InverseTransformDirection(pushDirection);
        //
        //             Vector3 targetOffset = localDirection * (penetration * deformationStrength);
        //
        //             Vector3 targetVertex = originalVertices[i] + targetOffset;
        //
        //             float displacement = Vector3.Distance(originalVertices[i], targetVertex);
        //
        //             if (displacement > maxDisplacement)
        //             {
        //                 targetVertex =
        //                     originalVertices[i]
        //                     + (targetVertex - originalVertices[i]).normalized * maxDisplacement;
        //             }
        //
        //             currentVertices[i] = Vector3.Lerp(
        //                 currentVertices[i],
        //                 targetVertex,
        //                 deformationSpeed * weight * Time.deltaTime
        //             );
        //         }
        //         else
        //         {
        //             currentVertices[i] = Vector3.Lerp(
        //                 currentVertices[i],
        //                 originalVertices[i],
        //                 recoverySpeed * Time.deltaTime
        //             );
        //         }
        //     }
        //
        //     runtimeMesh.vertices = currentVertices;
        //     runtimeMesh.RecalculateNormals();
        //     runtimeMesh.RecalculateBounds();
        // }

        private static Vector3 ClosestPointOnSegment(Vector3 a, Vector3 b, Vector3 point)
        {
            Vector3 ab = b - a;

            float t = Vector3.Dot(point - a, ab) / ab.sqrMagnitude;

            t = Mathf.Clamp01(t);

            return a + ab * t;
        }

        private static void GetCapsuleData(
            CapsuleCollider capsule,
            out Vector3 start,
            out Vector3 end,
            out float radius
        )
        {
            Transform t = capsule.transform;

            Vector3 center = t.TransformPoint(capsule.center);

            radius = capsule.radius * Mathf.Max(t.lossyScale.x, t.lossyScale.z);

            float height = Mathf.Max(capsule.height * Mathf.Abs(t.lossyScale.y), radius * 2f);

            float cylinderLength = height * 0.5f - radius;

            Vector3 direction;

            switch (capsule.direction)
            {
                case 0:
                    direction = t.right;
                    break;

                case 1:
                    direction = t.up;
                    break;

                case 2:
                    direction = t.forward;
                    break;

                default:
                    direction = t.up;
                    break;
            }

            start = center + direction * cylinderLength;
            end = center - direction * cylinderLength;
        }
    }
}
