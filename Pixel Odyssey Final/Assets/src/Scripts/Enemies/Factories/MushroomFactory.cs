using UnityEngine;

public class MushroomFactory : EnemyFactory
{
  [SerializeField] private GameObject mushroomPrefab;

    public MushroomFactory(GameObject prefab)
    {
        mushroomPrefab = prefab;
    }

    public override Enemy CreateEnemy(Vector3 position)
    {
        if (mushroomPrefab == null)
        {
            Debug.LogError("mushroomPrefab no está asignado en MushroomFactory.");
            return null;
        }

        GameObject mushroomInstance = Object.Instantiate(mushroomPrefab, position, Quaternion.identity);
        Mushroom mushroom = mushroomInstance.GetComponent<Mushroom>();
        if (mushroom != null)
        {
            // Inyección de dependencias
            var animator = mushroomInstance.GetComponent<IEnemyAnimator>();
            mushroom.InjectAnimator(animator);

            mushroom.Initialize();
            return mushroom;
        }
        else
        {
            Debug.LogError("El prefab no tiene el componente Mushroom.");
            return null;
        }
    }
}