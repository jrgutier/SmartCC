using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Frostwolf Warlord
namespace HREngine.Bots
{
    [Serializable]
public class CS2_226 : Card
    {
		public CS2_226() : base()
        {
            
        }
		
        public CS2_226(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            board.GetCard(Id).currentAtk +=board.MinionFriend.Count - 1;
            board.GetCard(Id).maxHealth += board.MinionFriend.Count - 1;
            board.GetCard(Id).CurrentHealth += board.MinionFriend.Count - 1;

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
