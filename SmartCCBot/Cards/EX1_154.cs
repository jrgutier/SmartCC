using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Wrath
namespace HREngine.Bots
{
    [Serializable]
public class EX1_154 : Card
    {
		public EX1_154() : base()
        {
            
        }
		
        public EX1_154(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            ChoiceOneTarget = true;
            ChoiceTwoTarget = true;
            TargetTypeOnPlay = TargetType.MINION_ENEMY;
            HasChoices = true;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0,int choice = 0)
        {
            base.OnPlay(ref board, target,index);

            if(choice == 1)
            {
                target.Damage(3 + board.GetSpellPower(), ref board);
            }
            else if(choice == 2)
            {
                target.Damage(1 + board.GetSpellPower(), ref board);
                board.FriendCardDraw++;
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
