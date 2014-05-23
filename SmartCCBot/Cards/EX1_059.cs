using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Crazed Alchemist
namespace HREngine.Bots
{
    [Serializable]
public class EX1_059 : Card
    {
		public EX1_059() : base()
        {
            
        }
		
        public EX1_059(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            TargetTypeOnPlay = TargetType.MINION_BOTH;
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
            if(target != null)
            {
                int tmp = target.CurrentAtk;

                target.CurrentAtk = target.CurrentHealth;
                target.CurrentHealth = tmp;
                for (int i = 0; i < target.buffs.Count; i++ )
                {
                    Buff buff = target.buffs.ElementAt(i);
                    target.buffs.Remove(target.buffs.ElementAt(i));

                    int t = buff.Hp;
                    buff.Hp = target.buffs.ElementAt(i).Atk;
                    buff.Atk = t;
                    target.buffs.Add(buff);
                }
                
                target.maxHealth = target.CurrentHealth;


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
