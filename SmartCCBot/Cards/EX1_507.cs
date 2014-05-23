using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Murloc Warleader
namespace HREngine.Bots
{
    [Serializable]
    public class EX1_507 : Card
    {
        public EX1_507()
            : base()
        {

        }

        public override void OnUpdate(Board board)
        {
            base.OnUpdate(board);
            if(IsFriend)
            {
                foreach (Card c in board.MinionFriend)
                {
                    c.RemoveBuffById(Id);

                    if (c.Race == CRace.MURLOC && !IsSilenced)
                    {
                        if (c.Id == Id)
                            continue;
                        c.AddBuff(new Buff(2, 1, Id));
                    }
                }
            }
            else
            {
                foreach (Card c in board.MinionEnemy)
                {
                    c.RemoveBuffById(Id);

                    if (c.Race == CRace.MURLOC && !IsSilenced)
                    {
                        if (c.Id == Id)
                            continue;
                        c.AddBuff(new Buff(2, 1, Id));
                    }
                }
            }
            
            
        }

        public EX1_507(CardTemplate newTemplate, bool isFriend, int id)
            : base(newTemplate, isFriend, id)
        {

        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnPlay(ref Board board, Card target = null, int index = 0)
        {
            base.OnPlay(ref board, target, index);
        }

        public override void OnDeath(ref Board board)
        {
            base.OnDeath(ref board);
            if (IsFriend)
            {
                foreach (Card c in board.MinionFriend)
                {
                    c.RemoveBuffById(Id);

                }
            }
            else
            {
                foreach (Card c in board.MinionEnemy)
                {
                    c.RemoveBuffById(Id);
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
