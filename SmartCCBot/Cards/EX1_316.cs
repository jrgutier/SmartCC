using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Power Overwhelming
namespace HREngine.Bots
{
    [Serializable]
public class EX1_316 : Card
    {
		public EX1_316() : base()
        {
            
        }
		
        public EX1_316(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
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
                target.currentAtk += 4;
                target.CurrentHealth += 4;
                target.maxHealth += 4;
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
