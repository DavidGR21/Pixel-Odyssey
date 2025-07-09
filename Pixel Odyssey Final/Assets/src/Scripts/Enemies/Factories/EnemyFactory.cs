using UnityEngine;
/// <summary>
/// Clase abstracta para la creación de enemigos. 
/// Se aplica patron Factory para instanciar enemigos en una posición específica.
/// Esta clase debe ser extendida por clases concretas que implementen la creación de enemigos específicos.
/// </summary>
public abstract class EnemyFactory : MonoBehaviour
{
    public abstract Enemy CreateEnemy(Vector3 position);
}
