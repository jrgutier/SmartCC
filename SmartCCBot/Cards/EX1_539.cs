using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Kill Command
namespace HREngine.Bots
{
    [Serializable]
public class EX1_539 : Card
    {
		public EX1_539() : base()
        {
            
        }
		
        public EX1_539(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
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
            if(target != null)
            {
                int damage = 3;
                foreach(Card c in board.MinionFriend)
                {
                    if (c.Race == CRace.BEAST)
                    {
                        damage = 5;
                        break;
                    }
                }

                target.Damage(damage, ref board);
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
