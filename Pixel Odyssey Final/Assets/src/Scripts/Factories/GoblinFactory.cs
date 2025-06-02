using UnityEngine;

public class GoblinFactory
{
    private GameObject goblinPrefab;

    public GoblinFactory(GameObject prefab)
    {
        goblinPrefab = prefab;
    }

    public Enemy CreateEnemy(Vector3 position)
    {
        if (goblinPrefab == null)
        {
            Debug.LogError("OgrePrefab no esta asignado en OgreFactory.");
            return null;
        }

        GameObject goblinInstance = Object.Instantiate(goblinPrefab, position, Quaternion.identity);
        Enemy enemy = goblinInstance.GetComponent<Enemy>();
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