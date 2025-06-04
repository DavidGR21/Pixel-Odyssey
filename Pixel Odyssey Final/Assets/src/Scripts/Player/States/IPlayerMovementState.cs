using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.src.Scripts.Player.States
{
    public interface IPlayerMovementState
    {
        void EnterState(PlayerMovementContext context);
        void Update(PlayerMovementContext context);
        void FixedUpdate(PlayerMovementContext context);
        void ExitState(PlayerMovementContext context);
    }
}
