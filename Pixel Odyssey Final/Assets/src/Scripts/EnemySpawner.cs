using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject GoblinPrefab;
    private GoblinFactory GoblinFactory;

    public GameObject OgrePrefab;
    private OgreFactory OgreFactory;

    public GameObject MushroomPrefab;
    private MushroomFactory MushroomFactory;

    public GameObject skeletonPrefab;
    private SkeletonFactory SkeletonFactory;


    private List<Enemy> activeEnemies = new List<Enemy>();

    void Start()
    {
        if (OgrePrefab == null || GoblinPrefab == null)
        {
            Debug.LogError("OgrePrefab no est� asignado en el EnemySpawner.");
            return;
        }

        OgreFactory = new OgreFactory(OgrePrefab);
        //  SpawnOgre(new Vector3(1, (float)-1.4, 0)); // Posici�n inicial aproximada
        GoblinFactory = new GoblinFactory(GoblinPrefab);
        SpawnGoblin(new Vector3(2, (float)-1.73, 0)); // Posici�n inicial aproximada
        MushroomFactory = new MushroomFactory(MushroomPrefab);
        SpawnMushroom(new Vector3(7, (float)-1.73, 0)); 
        SkeletonFactory = new SkeletonFactory(skeletonPrefab);
        SpawnSkeleton(new Vector3(4, (float)-1.73, 0));
    }

    void Update()
    {
        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy != null)
            {
                enemy.UpdateBehavior();
            }
        }
    }
    //crear goblin factori spawnearlo asignar prametros etc
    public void SpawnGoblin(Vector3 position)
    {
        // Ajustar la posici�n al suelo usando Raycast
        Vector3 spawnPosition = AdjustToGround(position);
        Enemy newEnemy = GoblinFactory.CreateEnemy(spawnPosition);
        if (newEnemy != null)
        {
            activeEnemies.Add(newEnemy);
        }
        else
        {
            Debug.LogError("No se pudo crear un nuevo Goblin.");
        }
    }
    public void SpawnOgre(Vector3 position)
    {
        // Ajustar la posici�n al suelo usando Raycast
        Vector3 spawnPosition = AdjustToGround(position);
        Enemy newEnemy = OgreFactory.CreateEnemy(spawnPosition);
        if (newEnemy != null)
        {
            activeEnemies.Add(newEnemy);
        }
        else
        {
            Debug.LogError("No se pudo crear un nuevo Ogre.");
        }
    }
    public void SpawnMushroom(Vector3 position)
    {
        // Ajustar la posici�n al suelo usando Raycast
        Vector3 spawnPosition = AdjustToGround(position);
        Enemy newEnemy = MushroomFactory.CreateEnemy(spawnPosition);
        if (newEnemy != null)
        {
            activeEnemies.Add(newEnemy);
        }
        else
        {
            Debug.LogError("No se pudo crear un nuevo Mushroom.");
        }
    }
    public void SpawnSkeleton(Vector3 position)
    {
        // Ajustar la posici�n al suelo usando Raycast
        Vector3 spawnPosition = AdjustToGround(position);
        Enemy newEnemy = SkeletonFactory.CreateEnemy(spawnPosition);
        if (newEnemy != null)
        {
            activeEnemies.Add(newEnemy);
        }
        else
        {
            Debug.LogError("No se pudo crear un nuevo Skeleton.");
        }
    }

    private Vector3 AdjustToGround(Vector3 spawnPosition)
    {
        // Lanza un Raycast hacia abajo desde una altura suficiente
        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(spawnPosition.x, spawnPosition.y + 10f), // Comienza desde arriba
            Vector2.down,
            20f, // Distancia m�xima del Raycast
            LayerMask.GetMask("Ground") // Aseg�rate de que el suelo est� en la capa "Ground"
        );

        if (hit.collider != null)
        {
            // Ajusta la posici�n al punto de colisi�n con el suelo
            spawnPosition.y = hit.point.y + 0.5f; // Ajusta seg�n el tama�o del colisionador del enemigo
            spawnPosition.z = 0; // Asegura z=0 para 2D
            Debug.DrawRay(hit.point, Vector2.up * 1f, Color.green, 2f); // Visualiza el Raycast
        }
        else
        {
            Debug.LogWarning($"No se encontr� suelo en {spawnPosition}. Usando posici�n original.");
        }

        return spawnPosition;
    }
}

