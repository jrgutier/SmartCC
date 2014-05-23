using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Equality
namespace HREngine.Bots
{
    [Serializable]
public class EX1_619 : Card
    {
		public EX1_619() : base()
        {
            
        }
		
        public EX1_619(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);

            foreach(Card c in board.MinionFriend)
            {
                c.CurrentHealth = 1;
            }
            foreach (Card c in board.MinionEnemy)
            {
                c.CurrentHealth = 1;
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
