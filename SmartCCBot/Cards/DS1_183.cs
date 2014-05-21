using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Multi-Shot
namespace HREngine.Bots
{
    [Serializable]
public class DS1_183 : Card
    {
		public DS1_183() : base()
        {
            
        }
		
        public DS1_183(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
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
            board.MinionEnemy[0].Damage(3, ref board);
            board.MinionEnemy[1].Damage(3, ref board);
            board.Resimulate();
        }

        public override void OnCastSpell(ref Board board, Card Spell)
        {
		    base.OnCastSpell(ref board, Spell);
        }
		
		public override bool ShouldBePlayed(Board board)
        {
            if(board.MinionEnemy.Count < 2)
            {
                return false;
            }
            return true;
        }

        public override bool ShouldAttack(Board board)
        {
            return true;
        }

        public override int GetPriorityAttack(ref Board board)
        {
            return 1;
        }

        public override int GetPriorityPlay()
        {
            return 1;
        }
		
    }
}
