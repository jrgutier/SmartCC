using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Mark of Nature
namespace HREngine.Bots
{
    [Serializable]
public class EX1_155 : Card
    {
		public EX1_155() : base()
        {
            
        }
		
        public EX1_155(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            HasChoices = true;
            ChoiceOneTarget = true;
            ChoiceTwoTarget = true;
            TargetTypeOnPlay = TargetType.MINION_BOTH;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0,int choice = 0)
        {
            base.OnPlay(ref board, target,index);
            if(choice == 1)
            {
                if(target != null)
                {
                    target.currentAtk += 4;
                }
            }
            else if(choice == 2)
            {
                if (target != null)
                {
                    target.maxHealth += 4;
                    target.CurrentHealth += 4;
                    target.IsTaunt = true;
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
