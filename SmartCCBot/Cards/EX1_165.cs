using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Druid of the Claw
namespace HREngine.Bots
{
    [Serializable]
public class EX1_165 : Card
    {
		public EX1_165() : base()
        {
            
        }
		
        public EX1_165(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
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
                board.SpawnMinion("EX1_165t1", index, CurrentCost);
            }
            else if(choice == 2)
            {
                board.SpawnMinion("EX1_165t2", index, CurrentCost);
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
