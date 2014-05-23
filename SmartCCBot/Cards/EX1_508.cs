using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Grimscale Oracle
namespace HREngine.Bots
{
    [Serializable]
public class EX1_508 : Card
    {
		public EX1_508() : base()
        {
            
        }
		
        public EX1_508(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void OnUpdate(Board board)
        {
            if(IsFriend)
            {
                foreach(Card c in board.MinionFriend)
                {
                    if (c.Id == Id)
                        continue;
                    c.RemoveBuffById(Id);
                    if (c.Race == CRace.MURLOC && !IsSilenced)
                        c.AddBuff(new Buff(1, 0, Id));
                }
            }
            else
            {
                foreach (Card c in board.MinionEnemy)
                {
                    if (c.Id == Id)
                        continue;
                    c.RemoveBuffById(Id);
                    if (c.Race == CRace.MURLOC && !IsSilenced)
                        c.AddBuff(new Buff(1, 0, Id));
                }
            }
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
            if (IsFriend)
            {
                foreach (Card c in board.MinionFriend)
                {
                    if (c.Race == CRace.MURLOC)
                        c.RemoveBuffById(Id);
                }
            }
            else
            {
                foreach (Card c in board.MinionEnemy)
                {
                    if (c.Race == CRace.MURLOC)
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
