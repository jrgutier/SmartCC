using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Bloodsail Corsair
namespace HREngine.Bots
{
    [Serializable]
public class NEW1_025 : Card
    {
		public NEW1_025() : base()
        {
            
        }
		
        public NEW1_025(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
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
                if (board.WeaponEnemy.CurrentDurability > 1)
                    board.WeaponEnemy.CurrentDurability--;
                else
                    board.WeaponEnemy = null;
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
