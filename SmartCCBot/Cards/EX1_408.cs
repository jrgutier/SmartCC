using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Mortal Strike
namespace HREngine.Bots
{
    [Serializable]
public class EX1_408 : Card
    {
		public EX1_408() : base()
        {
            
        }
		
        public EX1_408(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.ALL;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if(target != null)
            {
                if(board.HeroFriend.CurrentHealth > 12)
                {
                    target.Damage(4, ref board);
                }
                else
                {
                    target.Damage(6, ref board);
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
