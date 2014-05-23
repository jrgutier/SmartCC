using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Scavenging Hyena
namespace HREngine.Bots
{
    [Serializable]
public class EX1_531 : Card
    {
		public EX1_531() : base()
        {
            
        }
		
        public EX1_531(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnOtherMinionDeath(ref Board board, bool friend,Card minion)
        {
            if (minion.Race == CRace.BEAST)
            {
                board.GetCard(Id).currentAtk += 2;
                board.GetCard(Id).maxHealth += 1;
                board.GetCard(Id).CurrentHealth += 1;
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
