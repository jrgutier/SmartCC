using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Deathwing
namespace HREngine.Bots
{
    [Serializable]
public class NEW1_030 : Card
    {
		public NEW1_030() : base()
        {
            
        }
		
        public NEW1_030(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0,int choice = 0)
        {
            base.OnPlay(ref board, target,index);
            board.Hand.Clear();
            board.MinionEnemy.Clear();
            board.MinionFriend.Clear();
            board.Resimulate();
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
