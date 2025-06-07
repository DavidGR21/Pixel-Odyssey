using UnityEngine;

public class SkeletonFactory
{
    private GameObject skeletonPrefab;

    public SkeletonFactory(GameObject prefab)
    {
        skeletonPrefab = prefab;
    }

    public Enemy CreateEnemy(Vector3 position)
    {
        if (skeletonPrefab == null)
        {
            Debug.LogError("skeletonPrefab no esta asignado en skeletonPrefab.");
            return null;
        }

        GameObject skeletonInstance = Object.Instantiate(skeletonPrefab, position, Quaternion.identity);
        Enemy enemy = skeletonInstance.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Initialize();
        }
        else
        {
            Debug.LogError("El prefab no tiene el componente Enemy.");
        }
        return enemy;
    }
}