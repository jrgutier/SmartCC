using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Ethereal Arcanist
namespace HREngine.Bots
{
    [Serializable]
public class EX1_274 : Card
    {
		public EX1_274() : base()
        {
            
        }
		
        public EX1_274(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnEndTurn(Board board)
        {
            if (board.Secret.Count > 0)
            {
                board.GetCard(Id).currentAtk += 2;
                board.GetCard(Id).maxHealth += 2;
                board.GetCard(Id).CurrentHealth += 2;

            }
               
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
