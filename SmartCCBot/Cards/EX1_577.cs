using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//The Beast
namespace HREngine.Bots
{
    [Serializable]
public class EX1_577 : Card
    {
		public EX1_577() : base()
        {
            
        }
		
        public EX1_577(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
        }

        public override void OnDeath(ref Board board)
        {
            base.OnDeath(ref board);
            board.AddCardToBoard("EX1_finkle", !IsFriend);
            board.Resimulate();
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
