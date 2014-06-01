using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Power of the Wild
namespace HREngine.Bots
{
    [Serializable]
public class EX1_160 : Card
    {
		public EX1_160() : base()
        {
            
        }
		
        public EX1_160(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            HasChoices = true;

        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0,int choice = 0)
        {
            base.OnPlay(ref board, target,index);
            if(choice == 1)
            {
                foreach(Card c in board.MinionFriend)
                {
                    c.currentAtk++;
                    c.maxHealth++;
                    c.CurrentHealth++;
                }
            }
            else if(choice == 2)
            {
                board.AddCardToBoard("EX1_160t", true);
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
