using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Blood Knight
namespace HREngine.Bots
{
    [Serializable]
public class EX1_590 : Card
    {
		public EX1_590() : base()
        {
            
        }
		
        public EX1_590(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            int shields = 0;

            foreach(Card c in board.MinionEnemy)
            {
                if(c.IsDivineShield)
                {
                    c.IsDivineShield = false;
                    shields++;
                }
            }
            foreach (Card c in board.MinionFriend)
            {
                if (c.IsDivineShield)
                {
                    c.IsDivineShield = false;
                    shields++;
                }
            }

            board.GetCard(Id).currentAtk += 3 * shields;
            board.GetCard(Id).CurrentHealth += 3 * shields;
            board.GetCard(Id).maxHealth += 3 * shields;
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
