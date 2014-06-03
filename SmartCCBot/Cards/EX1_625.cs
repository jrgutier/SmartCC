using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Shadowform
namespace HREngine.Bots
{
    [Serializable]
public class EX1_625 : Card
    {
		public EX1_625() : base()
        {
            
        }
		
        public EX1_625(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0,int choice = 0)
        {
            base.OnPlay(ref board, target,index);
            if(board.Ability == null)
            {
                board.Ability = Card.Create("EX1_625t", true, 999999);
                board.Resimulate();
                return;
            }
            if (board.Ability.template.Id == "CS1h_001")
            {

                board.Ability = Card.Create("EX1_625t", true, board.Ability.Id);
                board.Resimulate();
                return;

            }
            else if (board.Ability.template.Id == "EX1_625t")
            {
                board.Ability = Card.Create("EX1_625t2", true, board.Ability.Id);
                board.Resimulate();
                return;
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
