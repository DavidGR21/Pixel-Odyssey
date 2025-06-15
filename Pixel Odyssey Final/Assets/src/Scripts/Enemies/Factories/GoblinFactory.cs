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
            Debug.LogError("goblinPrefab no está asignado en GoblinFactory.");
            return null;
        }

        GameObject goblinInstance = Object.Instantiate(goblinPrefab, position, Quaternion.identity);
        Goblin goblin = goblinInstance.GetComponent<Goblin>();
        if (goblin != null)
        {
            // Inyección de dependencias
            var animator = goblinInstance.GetComponent<IEnemyAnimator>();
            goblin.InjectAnimator(animator);

            goblin.Initialize();
            return goblin;
        }
        else
        {
            Debug.LogError("El prefab no tiene el componente Goblin.");
            return null;
        }
    }
}