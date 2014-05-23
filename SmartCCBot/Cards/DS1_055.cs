using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Darkscale Healer
namespace HREngine.Bots
{
    [Serializable]
public class DS1_055 : Card
    {
		public DS1_055() : base()
        {
            
        }
		
        public DS1_055(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
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
                c.Heal(2, ref board);
            }
            board.HeroFriend.Heal(2, ref board);
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
