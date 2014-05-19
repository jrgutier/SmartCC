using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{
    [Serializable]
    public class Buff
    {
        public int Atk { get; set; }
        public int Hp { get; set; }
        public int OwnerId { get; set; }

        public Buff()
        {
            Atk = 0;
            Hp = 0;
            OwnerId = 0;
        }

        public Buff(int atk,int h, int id)
        {
            Atk = atk;
            Hp = h;
            OwnerId = id;
        }

        public static Buff GetBuffById(string id)
        {
            Buff b = null;

            if(id.Contains("EX1_508"))
            {
                b = new Buff();
                b.Atk = 1;
            }
            else if (id.Contains("NEW1_033"))
            {
                b = new Buff();
                b.Atk = 1;
            }
            else if (id.Contains("EX1_162"))
            {
                b = new Buff();
                b.Atk = 1;
            }
            else if (id.Contains("EX1_565"))
            {
                b = new Buff();
                b.Atk = 2;
            }
            else if (id.Contains("EX1_507"))
            {
                b = new Buff();
                b.Atk = 2;
                b.Hp = 1;
            }
            else if (id.Contains("CS2_122"))
            {
                b = new Buff();
                b.Atk = 1;
                b.Hp = 1;
            }
            else if (id.Contains("NEW1_027"))
            {
                b = new Buff();
                b.Atk = 1;
                b.Hp = 1;
            }
            else if (id.Contains("DS1_175"))
            {
                b = new Buff();
                b.Atk = 1;
            }
            
            return b;
        }


    }
}
