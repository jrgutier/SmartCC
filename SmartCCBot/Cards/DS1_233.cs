using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Mind Blast
namespace HREngine.Bots
{
    [Serializable]
public class DS1_233 : Card
    {
		public DS1_233() : base()
        {
            
        }
		
        public DS1_233(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);

            board.HeroEnemy.Damage(5 + board.GetSpellPower(), ref board);
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
