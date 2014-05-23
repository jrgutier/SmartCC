using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Angry Chicken
namespace HREngine.Bots
{
    [Serializable]
public class EX1_009 : Card
    {
		public EX1_009() : base()
        {
            
        }
		
        public EX1_009(CardTemplate newTemplate, bool isFriend, int id) : base(newTemplate,isFriend,id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            HasEnrage = true;
        }

        public override void OnEnrage(bool enraged, ref Board board)
        {
            if(enraged)
            {
                currentAtk += 5;

            }
            else
            {
                currentAtk -= 5;

            }
        }

        public override void OnUpdate(Board board)
        {
         
            
        }

        public override void OnPlay(ref Board board, Card target = null,int index = 0)
        {
            base.OnPlay(ref board, target,index);
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
