using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Gladiator\'s Longbow
namespace HREngine.Bots
{
    [Serializable]
public class DS1_188 : Card
    {
		public DS1_188() : base()
        {
            
        }
		
        public DS1_188(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            board.ReplaceWeapon("DS1_188");
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
