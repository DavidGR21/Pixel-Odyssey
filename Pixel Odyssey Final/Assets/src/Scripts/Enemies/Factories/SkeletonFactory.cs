using UnityEngine;
/// <summary>
/// Clase que implementa el patrón Factory para crear instancias de Skeleton.
/// Esta clase hereda de EnemyFactory y se encarga de instanciar Skeletons en una posición específica.
/// Utiliza inyección de dependencias para asignar los animadores del enemigo.
/// </summary>
public class SkeletonFactory : EnemyFactory
{
    [SerializeField] private GameObject skeletonPrefab;

    public SkeletonFactory(GameObject prefab)
    {
        skeletonPrefab = prefab;
    }

    public override Enemy CreateEnemy(Vector3 position)
    {
        if (skeletonPrefab == null)
        {
            Debug.LogError("skeletonPrefab no esta asignado en skeletonPrefab.");
            return null;
        }

        GameObject skeletonInstance = Object.Instantiate(skeletonPrefab, position, Quaternion.identity);
        Skeleton skeleton = skeletonInstance.GetComponent<Skeleton>();
        if (skeleton != null)
        {
            // Inyección de dependencias
            var animator = skeletonInstance.GetComponent<IEnemyAnimator>();
            var shieldAnimator = skeletonInstance.GetComponent<IShieldEnemyAnimator>();
            skeleton.InjectAnimators(animator, shieldAnimator);

            skeleton.Initialize();
            return skeleton;
        }
        else
        {
            Debug.LogError("El prefab no tiene el componente Skeleton.");
            return null;
        }
    }
}