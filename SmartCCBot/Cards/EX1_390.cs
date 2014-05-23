using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Tauren Warrior
namespace HREngine.Bots
{
    [Serializable]
    public class EX1_390 : Card
    {
        public EX1_390()
            : base()
        {

        }

        public EX1_390(CardTemplate newTemplate, bool isFriend, int id)
            : base(newTemplate, isFriend, id)
        {

        }

        public override void Init()
        {
            base.Init();
            HasEnrage = true;
            IsTaunt = true;
        }

        public override void OnEnrage(bool enraged, ref Board board)
        {
            if (enraged)
            {
                board.GetCard(Id).currentAtk += 3;
            }
            else
            {
                    board.GetCard(Id).currentAtk -= 3;

            }
        }

        public override void OnPlay(ref Board board, Card target = null, int index = 0)
        {
            base.OnPlay(ref board, target, index);
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
