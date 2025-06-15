using UnityEngine;

public abstract class EnemyFactory
{
    public abstract Enemy CreateEnemy(Vector3 position);
}
