using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Ancient of War
namespace HREngine.Bots
{
    [Serializable]
public class EX1_178 : Card
    {
		public EX1_178() : base()
        {
            
        }
		
        public EX1_178(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
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

            Card c = board.SpawnMinion(template.Id, index, CurrentCost);

            if(choice == 1)
            {
                c.currentAtk += 5;
            }
            else if(choice == 2)
            {
                c.maxHealth += 5;
                c.CurrentHealth += 5;
                c.IsTaunt = true;
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
