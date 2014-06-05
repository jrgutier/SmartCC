using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HREngine.Bots
{
    [Serializable]
    public class Behavior
    {

        public enum KILLPRIORITY
        {
            LOW = 0,
            MEDIUM = 3,
            HIGH = 6,
            ULTRA = 10
        }

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

        public virtual KILLPRIORITY GetKillPriority(Board board)
        {
            return KILLPRIORITY.LOW;
        }

        public virtual int GetHandValue(Board board)
        {
            return 0;
        }
    }
}
