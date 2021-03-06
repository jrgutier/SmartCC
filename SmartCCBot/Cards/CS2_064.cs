using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Dread Infernal
namespace HREngine.Bots
{
    [Serializable]
public class CS2_064 : Card
    {
		public CS2_064() : base()
        {
            
        }
		
        public CS2_064(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0,int choice = 0)
        {
            base.OnPlay(ref board, target,index);
            foreach(Card c in board.MinionEnemy)
            {
                c.Damage(1, ref board);
            }
            foreach (Card c in board.MinionFriend)
            {
                c.Damage(1, ref board);
            }
            board.HeroFriend.Damage(1, ref board);
            board.HeroEnemy.Damage(1, ref board);

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
