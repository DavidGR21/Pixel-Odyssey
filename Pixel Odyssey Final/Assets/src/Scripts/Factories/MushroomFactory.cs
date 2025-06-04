using UnityEngine;

public class MushroomFactory
{
    private GameObject mushroomPrefab;

    public MushroomFactory(GameObject prefab)
    {
        mushroomPrefab = prefab;
    }

    public Enemy CreateEnemy(Vector3 position)
    {
        if (mushroomPrefab == null)
        {
            Debug.LogError("MushroomPrefab no esta asignado en MushroomPrefab.");
            return null;
        }

        GameObject mushroomInstance = Object.Instantiate(mushroomPrefab, position, Quaternion.identity);
        Enemy enemy = mushroomInstance.GetComponent<Enemy>();
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