using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Shapeshift
namespace HREngine.Bots
{
    [Serializable]
public class CS2_017 : Card
    {
		public CS2_017() : base()
        {
            
        }
		
        public CS2_017(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            board.HeroFriend.TempAtk++;
            board.HeroFriend.CurrentArmor++;
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
