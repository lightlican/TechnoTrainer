using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OilFlowSystem : MonoBehaviour
{
    [Header("Настройки масла")]
    public GameObject oilParticlePrefab;
    public float spawnInterval = 0.8f;
    public float moveSpeed = 1.5f;

    [Header("Лимит объектов")]
    public int maxOilParticles = 10;
    public bool stopSpawningWhenLimitReached = false;

    [Header("Точки пути масла")]
    public List<Transform> oilPathPoints = new List<Transform>();

    [Header("Цвет масла")]
    public Color oilColor = new Color(0.55f, 0.27f, 0.07f);

    private List<GameObject> activeOilParticles = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnOil());
    }

    IEnumerator SpawnOil()
    {
        while (true)
        {
            // Проверяем лимит
            if (activeOilParticles.Count >= maxOilParticles)
            {
                if (stopSpawningWhenLimitReached)
                {
                    yield return new WaitUntil(() => activeOilParticles.Count < maxOilParticles);
                }
                else
                {
                    yield return new WaitForSeconds(spawnInterval);
                    continue;
                }
            }

            if (oilParticlePrefab != null && oilPathPoints.Count > 1)
            {
                GameObject oil = Instantiate(oilParticlePrefab);
                activeOilParticles.Add(oil);

                StartCoroutine(MoveOil(oil));
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator MoveOil(GameObject oil)
    {
        // Движемся от первой до последней точки
        for (int i = 0; i < oilPathPoints.Count - 1; i++)
        {
            Transform start = oilPathPoints[i];
            Transform end = oilPathPoints[i + 1];

            if (start == null || end == null)
            {
                // Если какая-то точка отсутствует - уничтожаем частицу
                RemoveAndDestroyOil(oil);
                yield break;
            }

            float distance = Vector3.Distance(start.position, end.position);
            float travelTime = distance / moveSpeed;
            float elapsedTime = 0f;

            while (elapsedTime < travelTime)
            {
                if (oil == null) yield break;

                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / travelTime;

                // Движение
                oil.transform.position = Vector3.Lerp(start.position, end.position, progress);

                // Поворот
                Vector3 direction = (end.position - start.position).normalized;
                if (direction != Vector3.zero)
                    oil.transform.rotation = Quaternion.LookRotation(direction);

                yield return null;
            }

            // Убеждаемся, что достигли конечной позиции
            if (oil != null)
                oil.transform.position = end.position;
        }

        // Достигли последней точки - уничтожаем частицу
        RemoveAndDestroyOil(oil);
    }

    // Метод для удаления и уничтожения частицы
    private void RemoveAndDestroyOil(GameObject oilParticle)
    {
        if (oilParticle != null)
        {
            activeOilParticles.Remove(oilParticle);
            Destroy(oilParticle);
        }
    }

    // Метод для принудительного уничтожения всех частиц
    public void ClearAllOilParticles()
    {
        foreach (GameObject oil in activeOilParticles)
        {
            if (oil != null)
                Destroy(oil);
        }
        activeOilParticles.Clear();
    }



    // Визуализация пути в редакторе
    void OnDrawGizmos()
    {
        if (oilPathPoints.Count > 1)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < oilPathPoints.Count - 1; i++)
            {
                if (oilPathPoints[i] != null && oilPathPoints[i + 1] != null)
                {
                    Gizmos.DrawLine(oilPathPoints[i].position, oilPathPoints[i + 1].position);
                }
            }
        }
    }
}