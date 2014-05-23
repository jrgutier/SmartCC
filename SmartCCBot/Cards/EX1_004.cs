using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Young Priestess
namespace HREngine.Bots
{
    [Serializable]
public class EX1_004 : Card
    {
		public EX1_004() : base()
        {
            
        }
		
        public EX1_004(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void OnEndTurn(Board board)
        {
            if (board.MinionFriend.Count == 1)
            {
                board.MinionFriend[0].maxHealth += 1;
                board.MinionFriend[0].CurrentHealth += 1;
            }
        }
        public override void Init()
        {
            base.Init();
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
