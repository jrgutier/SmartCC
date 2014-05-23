using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Holy Fire
namespace HREngine.Bots
{
    [Serializable]
public class EX1_624 : Card
    {
		public EX1_624() : base()
        {
            
        }
		
        public EX1_624(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.BOTH_ENEMY;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if(target != null)
            {
                target.Damage(5 + board.GetSpellPower(),ref board);
                board.HeroFriend.Heal(5 + board.GetSpellPower()*board.HealFactor, ref board);
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
