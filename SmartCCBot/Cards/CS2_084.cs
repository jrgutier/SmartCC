using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Hunter\'s Mark
namespace HREngine.Bots
{
    [Serializable]
public class CS2_084 : Card
    {
		public CS2_084() : base()
        {
            
        }
		
        public CS2_084(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
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
                foreach(Buff b in target.buffs)
                {
                    b.Hp = 0;
                }
                target.MaxHealth = 1;
                target.CurrentHealth = 1;
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
