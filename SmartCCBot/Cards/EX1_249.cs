using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Baron Geddon
namespace HREngine.Bots
{
    [Serializable]
public class EX1_249 : Card
    {
		public EX1_249() : base()
        {
            
        }
		
        public EX1_249(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnEndTurn(Board board)
        {
            base.OnEndTurn(board);
            foreach(Card c in board.MinionEnemy)
            {
                c.Damage(2, ref board);
            }
            foreach(Card c in board.MinionFriend)
            {
                c.Damage(2, ref board);
            }
            board.HeroEnemy.Damage(2, ref board);
            board.HeroFriend.Damage(2, ref board);
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
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
