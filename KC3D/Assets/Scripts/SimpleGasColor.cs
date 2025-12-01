using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleGasColor : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject gasParticlePrefab;
    public float spawnInterval = 0.3f;
    public float moveSpeed = 2f;

    [Header("Точки и цвета")]
    public List<GasPoint> gasPoints = new List<GasPoint>();

    [System.Serializable]
    public class GasPoint
    {
        public Transform point;
        public Color gasColor = Color.white;
    }

    void Start()
    {
        StartCoroutine(SpawnGas());
    }

    IEnumerator SpawnGas()
    {
        while (true)
        {
            if (gasParticlePrefab != null && gasPoints.Count > 1)
            {
                GameObject gas = Instantiate(gasParticlePrefab);
                StartCoroutine(MoveGas(gas));
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator MoveGas(GameObject gas)
    {
        // Двигаемся от точки к точке
        for (int i = 0; i < gasPoints.Count - 1; i++)
        {
            Transform start = gasPoints[i].point;
            Transform end = gasPoints[i + 1].point;
            Color segmentColor = gasPoints[i].gasColor;

            if (start == null || end == null) continue;

            float distance = Vector3.Distance(start.position, end.position);
            float travelTime = distance / moveSpeed;
            float elapsedTime = 0f;

            while (elapsedTime < travelTime)
            {
                if (gas == null) yield break;

                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / travelTime;

                // Движение
                gas.transform.position = Vector3.Lerp(start.position, end.position, progress);

                // Поворот
                Vector3 direction = (end.position - start.position).normalized;
                if (direction != Vector3.zero)
                    gas.transform.rotation = Quaternion.LookRotation(direction);

                // Цвет газа
                gas.GetComponent<Renderer>().material.color = segmentColor;

                yield return null;
            }
        }

        Destroy(gas);
    }
}