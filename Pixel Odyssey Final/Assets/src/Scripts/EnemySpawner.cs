using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// EnemySpawner permite configurar desde el inspector los puntos de spawn y el tipo de enemigo a crear,
/// usando factories para mantener el desacoplamiento y la escalabilidad.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        [Tooltip("Tipo de enemigo (debe coincidir con la clave en el diccionario de factories, por ejemplo: 'Goblin', 'Mushroom')")]
        public string enemyType;
        [Tooltip("Posición relativa al EnemySpawner donde aparecerá el enemigo.")]
        public Vector3 position;
    }

    [Tooltip("Lista de puntos de spawn y tipos de enemigos para este nivel.")]
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    // Lista serializable para asignar factories desde el inspector
    [System.Serializable]
    public class FactoryEntry
    {
        public string enemyType;
        public MonoBehaviour factory; // Debe heredar de EnemyFactory
    }
    [Tooltip("Lista de factories disponibles para los tipos de enemigos.")]
    public List<FactoryEntry> factories = new List<FactoryEntry>();

    // Diccionario interno para acceso rápido
    private Dictionary<string, EnemyFactory> enemyFactories = new Dictionary<string, EnemyFactory>();

    private void Awake()
    {
        enemyFactories.Clear();
        foreach (var entry in factories)
        {
            // Cast explícito y comprobación
            var ef = entry.factory as EnemyFactory;
            if (!string.IsNullOrEmpty(entry.enemyType) && ef != null)
            {
                enemyFactories[entry.enemyType] = ef;
            }
        }
    }

    private void Start()
    {
        SpawnAll();
    }

    public void SpawnAll()
    {
        foreach (var point in spawnPoints)
        {
            if (enemyFactories.TryGetValue(point.enemyType, out var factory))
            {
                factory.CreateEnemy(point.position);
            }
            else
            {
                Debug.LogWarning($"No se encontró una factory para el tipo de enemigo '{point.enemyType}' en {gameObject.name}");
            }
        }
    }
}

