using UnityEngine;

public class OgreFactory : EnemyFactory

{
   [SerializeField]  private GameObject ogrePrefab;

    public OgreFactory(GameObject prefab)
    {
        ogrePrefab = prefab;
    }

    public override Enemy CreateEnemy(Vector3 position)
    {
        if (ogrePrefab == null)
        {
            Debug.LogError("OgrePrefab no est� asignado en OgreFactory.");
            return null;
        }

        GameObject ogreInstance = Object.Instantiate(ogrePrefab, position, Quaternion.identity);
        Enemy enemy = ogreInstance.GetComponent<Enemy>();
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