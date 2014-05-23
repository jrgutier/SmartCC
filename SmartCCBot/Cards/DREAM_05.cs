using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Nightmare
namespace HREngine.Bots
{
    [Serializable]
public class DREAM_05 : Card
    {
		public DREAM_05() : base()
        {
            
        }
		
        public DREAM_05(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.MINION_FRIEND;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if(target != null)
            {
                target.maxHealth += 5;
                target.CurrentHealth += 5;
                target.currentAtk += 5;
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
