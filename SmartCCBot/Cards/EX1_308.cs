using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Soulfire
namespace HREngine.Bots
{
    [Serializable]
public class EX1_308 : Card
    {
        public EX1_308()
            : base()
        {

        }

        public EX1_308(CardTemplate newTemplate, bool isFriend, int id)
            : base(newTemplate, isFriend, id)
        {

        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.BOTH_ENNEMY;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            target.Damage(4 + board.GetSpellPower(), ref board);

            board.FriendCardDraw--;

            board.Resimulate();

        }

        public override void OnDeath(ref Board board)
        {
            base.OnDeath(ref board);
        }

        public override bool ShouldBePlayedOnTarget(Card target)
        {

                if (target.CurrentHealth > 4)
                    return false;

            return true;
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
            if (board.Hand.Count < 3 && board.Hand.Count >1)
                return false;
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
