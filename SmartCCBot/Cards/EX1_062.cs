using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Old Murk-Eye
namespace HREngine.Bots
{
    [Serializable]
public class EX1_062 : Card
    {
		public EX1_062() : base()
        {
            
        }
		
        public EX1_062(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            IsCharge = true;
        }

        public override void OnUpdate(Board board)
        {

            RemoveBuffById(Id);
            foreach(Card c in board.MinionFriend)
            {
                if(c.Race == CRace.MURLOC)
                    AddBuff(new Buff(1, 0, Id));
            }
            foreach (Card c in board.MinionEnemy)
            {
                if (c.Race == CRace.MURLOC)
                    AddBuff(new Buff(1, 0, Id));
            }

            base.OnUpdate(board);
        }
        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
        }

        public override void OnDeath(ref Board board)
        {

            base.OnDeath(ref board);
            foreach (Card c in board.MinionFriend)
            {
                if (c.Race == CRace.MURLOC)
                    RemoveBuffById(Id);

            }
            foreach (Card c in board.MinionEnemy)
            {
                if (c.Race == CRace.MURLOC)
                    RemoveBuffById(Id);

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
