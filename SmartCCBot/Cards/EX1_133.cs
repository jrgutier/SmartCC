using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Perdition\'s Blade
namespace HREngine.Bots
{
    [Serializable]
public class EX1_133 : Card
    {
		public EX1_133() : base()
        {
            
        }
		
        public EX1_133(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.ALL;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0,int choice = 0)
        {
            base.OnPlay(ref board, target,index);
            if(target != null)
            {
                if(board.IsCombo())
                {
                    target.Damage(2, ref board);
                }
                else
                {
                    target.Damage(1, ref board);
                }
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
