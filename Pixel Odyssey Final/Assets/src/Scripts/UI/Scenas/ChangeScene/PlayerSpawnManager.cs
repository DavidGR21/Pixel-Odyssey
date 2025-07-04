using UnityEngine;

namespace Assets.src.Scripts.Scenas.ChangeScene
{
/// <summary>
/// Clase encargada de gestionar el punto de spawn del jugador al cambiar de escena.
/// Permite definir un punto de spawn específico para el jugador en la siguiente escena.
/// Este punto se puede utilizar para teletransportar al jugador a una ubicación específica al iniciar la nueva escena.
/// </summary>
    public static class PlayerSpawnManager
    {
        public static string nextSpawnPoint;
    }

}
