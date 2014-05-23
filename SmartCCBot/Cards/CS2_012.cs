using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Swipe
namespace HREngine.Bots
{
    [Serializable]
public class CS2_012 : Card
    {
		public CS2_012() : base()
        {
            
        }
		
        public CS2_012(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.BOTH_ENEMY;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if(target != null)
            {
                if(target.Type == CType.HERO)
                {
                    board.HeroEnemy.Damage(4 + board.GetSpellPower(), ref board);
                    foreach(Card c in board.MinionEnemy)
                    {
                        c.Damage(1 + board.GetSpellPower(), ref board);
                    }
                }
                else
                {
                    target.Damage(4 + board.GetSpellPower(), ref board);
                    foreach (Card c in board.MinionEnemy)
                    {
                        if (c.Id == target.Id)
                            continue;
                        c.Damage(1 + board.GetSpellPower(), ref board);
                    }
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
		
    }
}
