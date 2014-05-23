using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Dire Wolf Alpha
namespace HREngine.Bots
{
    [Serializable]
public class EX1_162 : Card
    {
		public EX1_162() : base()
        {
            
        }


        public override void OnUpdate(Board board)
        {
            if (IsFriend)
            {
                Card left = null;
                Card right = null;
                foreach (Card c in board.MinionFriend)
                {
                    c.RemoveBuffById(Id);
                    if (c.Index == Index - 1)
                        left = c;
                    if (c.Index == Index + 1)
                        right = c;
                }

                if (left != null && !IsSilenced)
                    left.AddBuff(new Buff(1, 0, Id));
                if (right != null && !IsSilenced)
                    right.AddBuff(new Buff(1, 0, Id));

            }
            else
            {
                Card left = null;
                Card right = null;
                foreach (Card c in board.MinionEnemy)
                {
                    c.RemoveBuffById(Id);
                    if (c.Index == Index - 1)
                        left = c;
                    if (c.Index == Index + 1)
                        right = c;
                }

                if (left != null && !IsSilenced)
                    left.AddBuff(new Buff(1, 0, Id));
                if (right != null && !IsSilenced)
                    right.AddBuff(new Buff(1, 0, Id));
            }
        }
        public EX1_162(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
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
        }

        public override void OnDeath(ref Board board)
        {
            base.OnDeath(ref board);
            if (IsFriend)
            {
                foreach (Card c in board.MinionFriend)
                {
                    c.RemoveBuffById(Id);
                }

            }
            else
            {
                foreach (Card c in board.MinionFriend)
                {
                    c.RemoveBuffById(Id);
                }
            }
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
