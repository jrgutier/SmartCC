using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Cenarius
namespace HREngine.Bots
{
    [Serializable]
public class EX1_573 : Card
    {
		public EX1_573() : base()
        {
            
        }
		
        public EX1_573(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
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
                    c.currentAtk += 2;
                    c.maxHealth += 2;
                    c.CurrentHealth+=2;
                }
                board.SpawnMinion(template.Id, index, CurrentCost);
            }
            else if(choice == 2)
            {
                board.AddCardToBoard("EX1_573t", true);
                board.SpawnMinion(template.Id, index, CurrentCost);
                board.AddCardToBoard("EX1_573t", true);

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
