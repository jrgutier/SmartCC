using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Doomsayer
namespace HREngine.Bots
{
    [Serializable]
public class NEW1_021 : Card
    {
		public NEW1_021() : base()
        {
            
        }
		
        public NEW1_021(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void OnEndTurn(Board board)
        {
            base.OnEndTurn(board);
            if(!IsFriend)
            {
                foreach(Card c in board.MinionFriend.ToArray())
                {
                    board.RemoveCardFromBoard(c.Id);
                }
            }
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
