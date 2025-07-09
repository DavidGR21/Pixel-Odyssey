using UnityEngine;
/// <summary>
/// Esta interfaz define el comportamiento de ataque para enemigos que pueden atacar al jugador.
/// Se Implementa esta interfaz en enemigos que tengan un ataque cuerpo a cuerpo o a distancia.
/// </summary>
public class AttackBehavior : IEnemyBehavior
{
    public void Execute(Enemy enemy)
    {
        if (enemy is IMeleeEnemy meleeEnemy)
        {
            meleeEnemy.Attack();
        }
    }
}

