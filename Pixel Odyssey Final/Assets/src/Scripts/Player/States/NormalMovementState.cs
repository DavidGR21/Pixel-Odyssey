using Assets.src.Scripts.Player.States;
using UnityEngine;

public class NormalMovementState : IPlayerMovementState
{
    public void EnterState(PlayerMovementContext context) { }

    public void Update(PlayerMovementContext context)
    {
        var player = context.Player;

        if (player.attackScript == null || player.attackScript.canMove)
        {
            player.horizontalMovement = Input.GetAxisRaw("Horizontal") * player.velocityOfMovement;

            // Si se presiona salto
            if (Input.GetButtonDown("Jump"))
            {
                player.jump = true;
                player.audioHandler.PlayJumpSound(); // ⬅️ Sonido de salto
            }
        }
        else
        {
            player.horizontalMovement = 0f;
        }

        // Sonido de pasos (loop si se mantiene presionada una dirección)
        if (Mathf.Abs(player.horizontalMovement) > 0.1f && player.inFloor)
        {
            player.audioHandler.PlayStepSound(loop: true);
        }
        else
        {
            player.audioHandler.StopStepSound();
        }

        player.animator.SetFloat("Horizontal", Mathf.Abs(player.horizontalMovement));

        if (Input.GetKeyDown(KeyCode.C) && player.canDoDash)
        {
            player.audioHandler.PlayDashSound(); // ⬅️ Sonido del dash
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

        if (player.horizontalMovement > 0 && !player.lookingRight)
        {
            player.spin();
        }
        else if (player.horizontalMovement < 0 && player.lookingRight)
        {
            player.spin();
        }

        if (player.inFloor && player.jump)
        {
            player.inFloor = false;
            player.rb2d.AddForce(new Vector2(0f, player.jumpForce), ForceMode2D.Impulse);
        }

        player.jump = false;
    }

    public void ExitState(PlayerMovementContext context)
    {
        // Detener pasos si se sale del estado
        context.Player.audioHandler.StopStepSound();
    }
}
