using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Ancient of Lore
namespace HREngine.Bots
{
    [Serializable]
public class NEW1_008 : Card
    {
		public NEW1_008() : base()
        {
            
        }
		
        public NEW1_008(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            HasChoices = true;
            ChoiceTwoTarget = true;
            TargetTypeOnPlay = TargetType.ALL;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0,int choice = 0)
        {
            base.OnPlay(ref board, target,index);
            board.SpawnMinion(template.Id, index, CurrentCost);
            if(choice == 1)
            {
                board.FriendCardDraw += 2;
            }
            else if(choice == 2)
            {
                if(target != null)
                {
                    target.Heal(5, ref board);
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
