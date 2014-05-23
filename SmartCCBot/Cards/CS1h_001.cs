using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Lesser Heal
namespace HREngine.Bots
{
    [Serializable]
public class CS1h_001 : Card
    {
		public CS1h_001() : base()
        {
            
        }
		
        public CS1h_001(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.ALL;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if (target != null)
            {
                target.Heal(2 * board.HealFactor,ref board);
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
