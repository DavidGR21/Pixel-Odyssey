using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.src.Scripts.Player.States
{
    /// <summary>
    /// Clase que representa el estado de movimiento de carrera del jugador.
    /// Implementa la interfaz IPlayerMovementState para definir el comportamiento del jugador durante el dash.
    /// Este estado se activa cuando el jugador realiza un dash, permitiendo un movimiento rápido en una dirección específica.
    /// El jugador no puede moverse ni saltar durante el dash, y la gravedad se desactiva temporalmente.
    /// Después de un tiempo determinado, el jugador vuelve al estado normal de movimiento.
    /// </summary>
    // DashMovementState.cs
    public class DashMovementState : IPlayerMovementState
    {
        public void EnterState(PlayerMovementContext context)
        {
            var player = context.Player;
            player.StartCoroutine(DashCoroutine(context));
        }

        private IEnumerator DashCoroutine(PlayerMovementContext context)
        {
            var player = context.Player;
            player.canMove = false;
            player.canDoDash = false;
            player.rb2d.gravityScale = 0;
            player.rb2d.linearVelocity = new Vector2(player.dashSpeed * player.transform.localScale.x, 0);
            player.animator.SetTrigger("Dash");

            yield return new WaitForSeconds(player.dashTime);

            player.canMove = true;
            player.rb2d.gravityScale = player.initialGravity;
            yield return new WaitForSeconds(player.timeIntoDash);
            context.TransitionTo(new NormalMovementState());
            player.canDoDash = true;
        }

        public void Update(PlayerMovementContext context) { }
        public void FixedUpdate(PlayerMovementContext context) { }
        public void ExitState(PlayerMovementContext context) { }
    }
}
