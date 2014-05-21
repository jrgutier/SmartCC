using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Mortal Coil
namespace HREngine.Bots
{
    [Serializable]
public class EX1_302 : Card
    {
		public EX1_302() : base()
        {
            
        }
		
        public EX1_302(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.MINION_Enemy;
        }

        public override bool ShouldBePlayedOnTarget(Card target)
        {
            if (target.CurrentHealth == 1)
                return true;
            return false;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if(target != null)
            {
                if (target.CurrentHealth == 1)
                    board.FriendCardDraw++;

                target.Damage(1 + board.GetSpellPower(), ref board);
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
		
		public override bool ShouldBePlayed(Board board)
        {
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
