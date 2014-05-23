using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Ancient Mage
namespace HREngine.Bots
{
    [Serializable]
public class EX1_584 : Card
    {
		public EX1_584() : base()
        {
            
        }
		
        public EX1_584(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TestAllIndexOnPlay = true;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);

            if (index < 7 && index != board.MinionFriend.Count - 1)
                board.GetMinionByIndex(index + 1, true).SpellPower++;
            if (index > 0)
                board.GetMinionByIndex(index - 1, true).SpellPower++;
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
