using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Cone of Cold
namespace HREngine.Bots
{
    [Serializable]
public class EX1_275 : Card
    {
		public EX1_275() : base()
        {
            
        }
		
        public EX1_275(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
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
                Card left = null;
                Card right = null;

                foreach(Card c in board.MinionEnemy)
                {
                    if (c.Index == target.Index - 1)
                        left = c;
                    if (c.Index == target.Index + 1)
                        right = c;
                }

                target.Damage(2 + board.GetSpellPower(), ref board);
                target.IsFrozen = true;

                if(right != null)
                {
                    right.IsFrozen = true;
                    right.Damage(2 + board.GetSpellPower(), ref board);
                }
                if (left != null)
                {
                    left.IsFrozen = true;
                    left.Damage(2 + board.GetSpellPower(), ref board);
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
