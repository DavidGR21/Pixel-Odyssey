using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.src.Scripts.Player.States
{
    /// <summary>
    /// Clase que representa el contexto de movimiento del jugador.
    /// Esta clase gestiona el estado actual del jugador y permite transiciones entre diferentes estados de movimiento.
    /// Contiene una referencia al jugador y al estado actual, y proporciona métodos para actualizar el estado y realizar transiciones.
    /// </summary>
    // PlayerMovementContext.cs
    public class PlayerMovementContext
    {
        public MovementPlayer Player { get; }
        public IPlayerMovementState CurrentState { get; private set; }

        public PlayerMovementContext(MovementPlayer player)
        {
            Player = player;
        }

        public void TransitionTo(IPlayerMovementState state)
        {
            CurrentState?.ExitState(this);
            CurrentState = state;
            CurrentState?.EnterState(this);
        }

        public void Update() => CurrentState?.Update(this);
        public void FixedUpdate() => CurrentState?.FixedUpdate(this);
    }
}
