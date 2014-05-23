using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Spiteful Smith
namespace HREngine.Bots
{
    [Serializable]
    public class CS2_221 : Card
    {
        public CS2_221()
            : base()
        {

        }

        public CS2_221(CardTemplate newTemplate, bool isFriend, int id)
            : base(newTemplate, isFriend, id)
        {

        }

        public override void Init()
        {
            base.Init();
            HasEnrage = true;
        }

        public override void OnUpdate(Board board)
        {
            if (IsEnraged)
            {
                if (board.WeaponFriend != null)
                {
                    board.WeaponFriend.currentAtk += 2;
                }
            }
            else
            {
                if (board.WeaponFriend != null)
                {
                    board.WeaponFriend.currentAtk -= 2;
                }
            }
            base.OnUpdate(board);
        }

        public override void OnPlay(ref Board board, Card target = null, int index = 0)
        {
            base.OnPlay(ref board, target, index);
        }

        public override void OnEnrage(bool enraged, ref Board board)
        {

        }

        public override void OnDeath(ref Board board)
        {
            base.OnDeath(ref board);
            if (IsEnraged)
            {
                if (board.WeaponFriend != null)
                {
                    board.WeaponFriend.currentAtk -= 2;
                }
            }
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
