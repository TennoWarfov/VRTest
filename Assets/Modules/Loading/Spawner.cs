using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Modules.Loading
{
    public class Spawner : MonoBehaviour
    {
        public GameObject[] prefabs;
        public int countPerPrefab = 10;
        public float radius = 2f;
        public float minDistance = 0.3f;
        public Transform center;

        private readonly List<Vector3> _spawnedPositions = new();
        private readonly List<GameObject> _spawnedPrefabs = new();

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Spawn();
            }
        }

        public void Spawn()
        {
            _spawnedPositions.Clear();

            int attempts = 0;
            int totalToSpawn = prefabs.Length * countPerPrefab;
            int spawned = 0;

            while (spawned < totalToSpawn && attempts < 5000)
            {
                attempts++;

                Vector3 pos = center.position + Random.insideUnitSphere * radius;

                if (!IsFarEnough(pos))
                    continue;

                int prefabIndex = spawned % prefabs.Length;
                GameObject prefab = prefabs[prefabIndex];

                var obj = Instantiate(prefab, pos, Quaternion.identity);
                obj.AddComponent<RandomVelocity>().Initialize(center);
                obj.transform.localEulerAngles = new Vector3(
                    Random.Range(0, 360),
                    Random.Range(0, 360),
                    Random.Range(0, 360)
                );

                _spawnedPositions.Add(pos);
                _spawnedPrefabs.Add(obj);
                spawned++;
            }
        }

        bool IsFarEnough(Vector3 newPos)
        {
            foreach (var pos in _spawnedPositions)
            {
                if (Vector3.Distance(pos, newPos) < minDistance)
                    return false;
            }
            return true;
        }

        public void Clear()
        {
            foreach (var obj in _spawnedPrefabs)
                Destroy(obj);

            _spawnedPrefabs.Clear();
        }
    }
}
