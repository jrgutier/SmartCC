using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Shadow Word: Death
namespace HREngine.Bots
{
    [Serializable]
public class EX1_622 : Card
    {
		public EX1_622() : base()
        {
            
        }
		
        public EX1_622(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.MINION_ENEMY;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if(target != null)
            {
                if(target.CurrentAtk > 4)
                {
                    board.MinionEnemy.Remove(target);
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
