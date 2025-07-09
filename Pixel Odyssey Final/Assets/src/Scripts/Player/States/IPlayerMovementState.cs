using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.src.Scripts.Player.States
{
/// <summary>
/// Interfaz que define el comportamiento de los estados de movimiento del jugador.
/// Esta interfaz debe ser implementada por cualquier clase que represente un estado de movimiento del jugador.
/// Define los métodos para entrar, actualizar, realizar actualizaciones físicas y salir del estado.
/// </summary>
    public interface IPlayerMovementState
    {
        void EnterState(PlayerMovementContext context);
        void Update(PlayerMovementContext context);
        void FixedUpdate(PlayerMovementContext context);
        void ExitState(PlayerMovementContext context);
    }
}
