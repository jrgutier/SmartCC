using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Sword of Justice
namespace HREngine.Bots
{
    [Serializable]
public class EX1_366 : Card
    {
		public EX1_366() : base()
        {
            
        }
		
        public EX1_366(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
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
            if(board.WeaponFriend != null)
            {
                if(board.WeaponFriend.CurrentDurability > 0)
                {
                    Minion.currentAtk += 1;
                    Minion.CurrentHealth += 1;
                    Minion.maxHealth += 1;
                    board.WeaponFriend.CurrentDurability--;
                }
            }
        }

        public override void OnCastSpell(ref Board board, Card Spell)
        {
		    base.OnCastSpell(ref board, Spell);
        }
		

    }
}
