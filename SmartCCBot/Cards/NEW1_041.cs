using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Stampeding Kodo
namespace HREngine.Bots
{
    [Serializable]
public class NEW1_041 : Card
    {
		public NEW1_041() : base()
        {
            
        }
		
        public NEW1_041(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);

            List<Card> targets = new List<Card>();
            foreach(Card c in board.MinionEnemy)
            {
                if (c.CurrentAtk <= 2)
                    targets.Add(c);
            }
            if (targets.Count > 1)
                board.Resimulate();
            else if(targets.Count  == 1)
            {
                board.RemoveCardFromBoard(targets[0].Id);
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
