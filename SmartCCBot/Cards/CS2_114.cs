using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Cleave
namespace HREngine.Bots
{
    [Serializable]
public class CS2_114 : Card
    {
		public CS2_114() : base()
        {
            
        }
		
        public CS2_114(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0,int choice = 0)
        {
            base.OnPlay(ref board, target,index);

            Card first = null;
            Card second = null;


            foreach(Card c in board.MinionEnemy)
            {
                if (first == null)
                    first = c;
                if(first != null)
                {
                    if (c.CurrentHealth < first.CurrentHealth)
                        first = c;
                }
                
            }

            foreach(Card c in board.MinionEnemy)
            {
                if (second == null && c != first)
                    second = c;

                if(second != null)
                {
                    if (c.CurrentHealth <= second.CurrentHealth && c != first)
                        second = c;
                }
                
            }


            if (first == null || second == null)
                return;


            first.Damage(2, ref board);
            second.Damage(2, ref board);
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
