using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.src.Scripts.Player.States
{
    // NormalMovementState.cs
    public class NormalMovementState : IPlayerMovementState
    {
        public void EnterState(PlayerMovementContext context) { }

        public void Update(PlayerMovementContext context)
        {
            var player = context.Player;

            if (player.attackScript == null || player.attackScript.canMove)
            {
                player.horizontalMovement = Input.GetAxisRaw("Horizontal") * player.velocityOfMovement;
                if (Input.GetButtonDown("Jump"))
                {
                    player.jump = true;
                }
            }
            else
            {
                player.horizontalMovement = 0f;
            }

            player.animator.SetFloat("Horizontal", Mathf.Abs(player.horizontalMovement));

            if (Input.GetKeyDown(KeyCode.C) && player.canDoDash)
            {
                context.TransitionTo(new DashMovementState());
            }
        }

        public void FixedUpdate(PlayerMovementContext context)
        {
            var player = context.Player;
            player.inFloor = Physics2D.OverlapBox(player.floorControler.position, player.boxDimension, 0f, player.isFloor);
            player.animator.SetBool("inFloor", player.inFloor);

            float moveAmount = player.horizontalMovement * player.velocityOfMovement;
            Vector2 targetVelocity = new Vector2(moveAmount, player.rb2d.linearVelocity.y);

            player.rb2d.linearVelocity = Vector2.SmoothDamp(
                player.rb2d.linearVelocity,
                targetVelocity,
                ref player.velocity,
                player.motionSmotothing);

            // Rotación del personaje
            if (player.horizontalMovement > 0 && !player.lookingRight)
            {
                player.spin();
            }
            else if (player.horizontalMovement < 0 && player.lookingRight)
            {
                player.spin();
            }

            // Salto
            if (player.inFloor && player.jump)
            {
                player.inFloor = false;
                player.rb2d.AddForce(new Vector2(0f, player.jumpForce), ForceMode2D.Impulse);
            }

            player.jump = false;
        }

        public void ExitState(PlayerMovementContext context) { }
    }
}
