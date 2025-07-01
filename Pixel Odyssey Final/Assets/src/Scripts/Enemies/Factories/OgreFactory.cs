using UnityEngine;
/// <summary>
/// Clase que implementa el patrón Factory para crear instancias de Ogre.
/// Esta clase hereda de EnemyFactory y se encarga de instanciar Ogres en una posición específica.
/// Utiliza inyección de dependencias para asignar el animador del enemigo.
/// </summary>
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
            Debug.LogError("OgrePrefab no esta asignado en OgreFactory.");
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