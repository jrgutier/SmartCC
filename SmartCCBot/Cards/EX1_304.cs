using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Void Terror
namespace HREngine.Bots
{
    [Serializable]
public class EX1_304 : Card
    {
		public EX1_304() : base()
        {
            
        }
		
        public EX1_304(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TestAllIndexOnPlay = true;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);

            Card left = null;
            Card right = null;

            int atkBuff = 0;
            int healBuff = 0;

            foreach(Card c in board.MinionFriend)
            {
                if (c.Index == index - 1)
                    left = c;
                if (c.Index == Index + 1)
                    right = c;
            }

            if(left != null)
            {
                atkBuff += left.CurrentAtk;
                healBuff += left.CurrentHealth;
                board.RemoveCardFromBoard(left.Id);
            }
            if (right != null)
            {
                atkBuff += right.CurrentAtk;
                healBuff += right.CurrentHealth;
                board.RemoveCardFromBoard(right.Id);

            }

            board.GetCard(Id).currentAtk += atkBuff;
            board.GetCard(Id).maxHealth += healBuff;
            board.GetCard(Id).CurrentHealth += healBuff;

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
