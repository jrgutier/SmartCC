using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Bananas
namespace HREngine.Bots
{
    [Serializable]
public class EX1_014t : Card
    {
		public EX1_014t() : base()
        {
            
        }
		
        public EX1_014t(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.MINION_FRIEND;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if(target != null)
            {
                target.currentAtk += 1;
                target.maxHealth += 1;
                target.CurrentHealth += 1;
            }
        }

        public override void OnDeath(ref Board board)
        {
            base.OnDeath(ref board);
        }

        public override void OnPlayOtherMinion(ref Board board, Card Minion)
        {
            base.OnPlayOtherMinion(ref board, Minion);
        }

        public override void OnCastSpell(ref Board board, Card Spell)
        {
		    base.OnCastSpell(ref board, Spell);
        }
				
    }
}
