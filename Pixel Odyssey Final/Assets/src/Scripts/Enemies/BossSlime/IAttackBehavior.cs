using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.src.Scripts.Enemies.BossSlime
{
    public interface IAttackBehavior
    {
        bool CanAttack();
        void ExecuteAttack();
    }
}
