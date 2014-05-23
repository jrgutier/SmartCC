using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HREngine.Bots
{
    [Serializable]
    public class Behavior
    {
        public Behavior()
        {

        }

        public virtual bool ShouldBePlayed(Board board)
        {
            return true;
        }

        public virtual bool ShouldAttack(Board board)
        {
            return true;
        }

        public virtual bool ShouldAttackTarget(Card target)
        {
            return true;
        }

        public virtual bool ShouldBePlayedOnTarget(Card target)
        {
            return true;
        }

        public virtual int GetPriorityPlay(Board board)
        {
            return 1;
        }

    }
}
