using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Sylvanas Windrunner
namespace HREngine.Bots
{
    [Serializable]
    public class EX1_016 : Card
    {
        public EX1_016()
            : base()
        {

        }

        public EX1_016(CardTemplate newTemplate, bool isFriend, int id)
            : base(newTemplate, isFriend, id)
        {

        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null, int index = 0)
        {
            base.OnPlay(ref board, target, index);
        }

        public override void OnDeath(ref Board board)
        {
            base.OnDeath(ref board);


            if (IsFriend)
            {
                Card worstMinion = null;
                foreach (Card c in board.MinionEnemy)
                {
                    if (worstMinion == null)
                        worstMinion = c;
                    if (c.GetValue(board) < worstMinion.GetValue(board))
                        worstMinion = c;
                }

                if (worstMinion != null)
                {
                    board.RemoveCardFromBoard(worstMinion.Id);
                    board.AddCardToBoard(worstMinion.template.Id, true);
                }
                    


            }
            else
            {
                Card bestMinion = null;
                foreach (Card c in board.MinionFriend)
                {
                    if (bestMinion == null)
                        bestMinion = c;
                    if (c.GetValue(board) > bestMinion.GetValue(board))
                        bestMinion = c;
                }

                if (bestMinion != null)
                {
                    board.RemoveCardFromBoard(bestMinion.Id);
                    board.AddCardToBoard(bestMinion.template.Id, false);
                }
                    

            }




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
