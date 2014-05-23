using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Edwin VanCleef
namespace HREngine.Bots
{
    [Serializable]
public class EX1_613 : Card
    {
		public EX1_613() : base()
        {
            
        }
		
        public EX1_613(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);

            if (!board.IsCombo())
                return;

            int cardsplayed = 0;
            foreach(Action a in board.ActionsStack)
            {
                if (a.Type == Action.ActionType.CAST_MINION || a.Type == Action.ActionType.CAST_SPELL || a.Type == Action.ActionType.CAST_WEAPON)
                {
                    cardsplayed++;
                }
            }

            board.GetCard(Id).currentAtk += 2 * cardsplayed;
            board.GetCard(Id).CurrentHealth += 2 * cardsplayed;
            board.GetCard(Id).maxHealth += 2 * cardsplayed;
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
