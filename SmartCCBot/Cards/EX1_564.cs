using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Faceless Manipulator
namespace HREngine.Bots
{
    [Serializable]
public class EX1_564 : Card
    {
		public EX1_564() : base()
        {
            
        }
		
        public EX1_564(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.MINION_BOTH;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if(target != null)
            {
                Card c = Card.Clone(target);
                c.Id = Id;
                c.Index = index;
                c.IsTired = true;
                for(int i = 0 ; i < board.MinionFriend.Count ; i++)
                {
                    if(board.MinionFriend[i].Id == Id)
                    {
                        board.MinionFriend[i] = c;
                    }
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
