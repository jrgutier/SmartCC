using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Flesheating Ghoul
namespace HREngine.Bots
{
    [Serializable]
public class tt_004 : Card
    {
		public tt_004() : base()
        {
            
        }
		
        public tt_004(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnOtherMinionDeath(ref Board board, bool friend, Card minion)
        {
            board.GetCard(Id).currentAtk++;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
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
