using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Ysera Awakens
namespace HREngine.Bots
{
    [Serializable]
public class DREAM_02 : Card
    {
		public DREAM_02() : base()
        {
            
        }
		
        public DREAM_02(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            foreach(Card c in board.MinionEnemy)
            {
                c.Damage(5, ref board);
            }
            foreach(Card c in board.MinionFriend)
            {
                if (c.template.Id == "EX1_572")
                    continue;
                c.Damage(5, ref board);
            }
            board.HeroEnemy.Damage(5, ref board);
            board.HeroFriend.Damage(5, ref board);
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
