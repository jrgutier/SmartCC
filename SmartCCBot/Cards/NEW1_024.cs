using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Captain Greenskin
namespace HREngine.Bots
{
    [Serializable]
public class NEW1_024 : Card
    {
		public NEW1_024() : base()
        {
            
        }
		
        public NEW1_024(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if(board.WeaponFriend != null)
            {
                board.WeaponFriend.CurrentAtk++;
                board.WeaponFriend.CurrentDurability++;
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
