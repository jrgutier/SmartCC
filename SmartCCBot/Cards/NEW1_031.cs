using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Animal Companion
namespace HREngine.Bots
{
    [Serializable]
public class NEW1_031 : Card
    {
		public NEW1_031() : base()
        {
            
        }
		
        public NEW1_031(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);

            board.AddCardToBoard("NEW1_033", true);

            board.Resimulate();
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
