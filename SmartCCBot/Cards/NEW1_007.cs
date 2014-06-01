using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Starfall
namespace HREngine.Bots
{
    [Serializable]
public class NEW1_007 : Card
    {
		public NEW1_007() : base()
        {
            
        }
		
        public NEW1_007(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            HasChoices = true;
            ChoiceOneTarget = true;
            TargetTypeOnPlay = TargetType.MINION_BOTH;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0,int choice = 0)
        {
            base.OnPlay(ref board, target,index);
            if(choice == 1)
            {
                if (target != null)
                    target.Damage(5 + board.GetSpellPower(), ref board);
            }
            else if(choice == 2)
            {
                foreach(Card c in board.MinionEnemy)
                {
                    c.Damage(2 + board.GetSpellPower(), ref board);
                }
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
