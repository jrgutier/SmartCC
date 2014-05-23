using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Cabal Shadow Priest
namespace HREngine.Bots
{
    [Serializable]
public class EX1_091 : Card
    {
		public EX1_091() : base()
        {
            
        }
		
        public EX1_091(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
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
                board.MinionEnemy.Remove(target);
                board.MinionFriend.Add(target);
            }
            board.Resimulate();
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
