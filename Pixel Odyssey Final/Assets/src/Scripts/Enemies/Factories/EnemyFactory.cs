using UnityEngine;

public abstract class EnemyFactory : MonoBehaviour
{
    public abstract Enemy CreateEnemy(Vector3 position);
}
