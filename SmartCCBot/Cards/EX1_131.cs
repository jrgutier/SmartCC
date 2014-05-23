using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Defias Ringleader
namespace HREngine.Bots
{
    [Serializable]
public class EX1_131 : Card
    {
		public EX1_131() : base()
        {
            
        }
		
        public EX1_131(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if(board.IsCombo())
            {
                board.AddCardToBoard("EX1_131t", IsFriend);

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
