using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Sunfury Protector
namespace HREngine.Bots
{
    [Serializable]
public class EX1_058 : Card
    {
		public EX1_058() : base()
        {
            
        }
		
        public EX1_058(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
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

            foreach (Card c in board.MinionFriend)
            {
                if (c.Index == index - 1)
                    left = c;
                if (c.Index == index + 1)
                    right = c;
            }

            if (left != null)
            {
                left.IsTaunt = true;
            }
            if (right != null)
            {
                right.IsTaunt = true;
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
