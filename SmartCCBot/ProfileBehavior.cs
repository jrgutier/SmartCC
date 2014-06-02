using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HREngine.Bots
{
    [Serializable]
    public class ProfileBehavior
    {

        public ProfileBehavior()
        {

        }

        public virtual List<Card> HandleMulligan(List<Card> Choices)
        {
            return Choices;
        }

        public virtual bool ShouldPlayMoreMinions(Board board)
        {
            return true;
        }

        public virtual bool ShouldAttackWithWeapon(Board board)
        {
            return true;
        }

        public virtual bool ShouldAttackTargetWithWeapon(Card weapon,Card target)
        {
            return true;
        }
     
    }
}
