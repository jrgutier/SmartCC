using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Harrison Jones
namespace HREngine.Bots
{
    [Serializable]
public class EX1_558 : Card
    {
		public EX1_558() : base()
        {
            
        }
		
        public EX1_558(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if(board.WeaponEnemy != null)
            {
                board.FriendCardDraw += board.WeaponEnemy.CurrentDurability;
                board.WeaponEnemy = null;
                board.Resimulate();
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
