using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Holy Nova
namespace HREngine.Bots
{
    [Serializable]
public class CS1_112 : Card
    {
		public CS1_112() : base()
        {
            
        }
		
        public CS1_112(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
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
                c.Heal(2 + board.GetSpellPower() * board.HealFactor, ref board);
            }
            foreach (Card c in board.MinionEnemy)
            {
                c.Damage(2 + board.GetSpellPower(), ref board);
            }
            board.HeroFriend.Heal(2 + board.GetSpellPower() * board.HealFactor, ref board);
            board.HeroEnemy.Damage(2 + board.GetSpellPower(), ref board);

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
