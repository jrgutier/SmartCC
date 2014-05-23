using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Al\'Akir the Windlord
namespace HREngine.Bots
{
    [Serializable]
public class NEW1_010 : Card
    {
		public NEW1_010() : base()
        {
            
        }
		
        public NEW1_010(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            IsTaunt = true;
            IsWindfury = true;
            IsDivineShield = true;
            IsCharge = true;
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
