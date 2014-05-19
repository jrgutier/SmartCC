using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    [Serializable]
    public class Action
    {
        public enum ActionType
        {
            MINION_ATTACK = 0,
            HERO_ATTACK = 1,
            CAST_SPELL = 2,
            CAST_MINION = 3,
            CAST_WEAPON = 4,
            END_TURN = 5,
            CAST_ABILITY = 6,
            RESIMULATE = 7
        }

        public Card Target { get; set; }
        public Card Actor { get; set; }
        public ActionType Type { get; set; }
        public int Index { get; set; }

        public Action()
        {

        }
        public Action(ActionType actionType, Card actor, Card target = null, int index = 0)
        {
            Target = target;
            Actor = actor;
            Type = actionType;
            Index = index;
        }

        public override string ToString()
        {
            string typestr = string.Empty;

            switch (Type)
            {
                case ActionType.CAST_MINION:
                    typestr = "Cast_Minion";
                    break;
                case ActionType.CAST_SPELL:
                    typestr = "Cast_Spell";
                    break;
                case ActionType.CAST_WEAPON:
                    typestr = "Cast_Weapon";
                    break;
                case ActionType.HERO_ATTACK:
                    typestr = "Hero_Attack";
                    break;
                case ActionType.MINION_ATTACK:
                    typestr = "Minion_Attack";
                    break;
                case ActionType.CAST_ABILITY:
                    typestr = "Cast_Ability";
                    break;
                case ActionType.END_TURN:
                    typestr = "End_Turn";
                    break;
                case ActionType.RESIMULATE:
                    typestr = "Resimulate";
                    break;
            }


            string tmp = "Action[" + typestr + "] ->";
            if (Actor != null)
            {
                tmp += Actor.ToStringShort();
            }
            if (Target != null)
            {
                tmp += "->" + Target.ToStringShort();
            }

            if (Type == ActionType.CAST_MINION)
            {
                tmp += "INDEX : " + Index.ToString();
            }

            return tmp;
        }
        public override bool Equals(object obj)
        {
            Action c = obj as Action;
            if (c == null)
                return false;

            if (Type != c.Type)
                return false;


            if(Actor != null)
            {
                if (!Actor.Equals(c.Actor))
                    return false;
            }

            if(Target != null)
            {
                if (!Target.Equals(c.Target))
                    return false;
            }
            

            return true;
        }
    }
}
