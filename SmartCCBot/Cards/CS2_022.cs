using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Polymorph
namespace HREngine.Bots
{
    [Serializable]
public class CS2_022 : Card
    {
		public CS2_022() : base()
        {
            
        }
		
        public CS2_022(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.MINION_ENEMY;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if(target != null)
            {
                board.MinionEnemy.Remove(target);
                board.MinionEnemy.Add(Card.Create("CS2_tk1", false, target.Id));
                board.Resimulate();
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
