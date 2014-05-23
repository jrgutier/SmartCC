using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Gruul
namespace HREngine.Bots
{
    [Serializable]
public class NEW1_038 : Card
    {
		public NEW1_038() : base()
        {
            
        }
		
        public NEW1_038(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void OnEndTurn(Board board)
        {
            base.OnEndTurn(board);
            currentAtk++;
            maxHealth++;
            CurrentHealth++;
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
