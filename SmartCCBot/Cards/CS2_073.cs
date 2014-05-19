using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Cold Blood
namespace HREngine.Bots
{
    [Serializable]
public class CS2_073 : Card
    {
        public CS2_073()
            : base()
        {

        }

        public CS2_073(CardTemplate newTemplate, bool isFriend, int id)
            : base(newTemplate, isFriend, id)
        {

        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.MINION_FRIEND;
        }

        public override void OnPlay(ref Board board, Card target = null, int index = 0)
        {
            base.OnPlay(ref board, target, index);

            if (target != null)
            {
                if (board.IsCombo())
                {
                    target.AddBuff(new Buff(4, 0, Id));
                }
                else
                {
                    target.AddBuff(new Buff(2, 0, Id));
                }

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
