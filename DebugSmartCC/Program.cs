using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Concurrent;
using HREngine.Bots;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace DebugSmartCC
{


    class Program
    {
        static void Main(string[] args)
        {
            Console.BufferHeight = Int16.MaxValue - 1; // ***** Alters the BufferHeight *****

            CardTemplate.DatabasePath = "";
            CardTemplate.LoadAll();

            Console.ReadLine();


            Simulation s = new Simulation();

            Board root = new Board();
          /*
              root.HeroEnnemy = Card.Create("HERO_04", false, 0);
                root.HeroFriend = Card.Create("HERO_04", true, 1);
                root.HeroFriend.CurrentHealth = 30;
                root.HeroEnnemy.CurrentHealth = 7;

              //  root.Ability = Card.Create("CS2_049", true, 545);

                //root.Hand.Add(Card.Create("CS2_112", true, 2));
                //root.Hand.Add(Card.Create("EX1_408", true, 3));
                //root.Hand.Add(Card.Create("EX1_575", true, 4));
                //root.Hand.Add(Card.Create("GAME_005", true, 5));

                root.ManaAvailable = 20;
                Card c = Card.Create("NEW1_033", false, 3, 0);
                c.Silence();
                 root.MinionEnnemy.Add(c);
                 root.MinionEnnemy.Add(Card.Create("CS2_203", false, 4, 0));
            //    root.MinionFriend.Add(Card.Create("EX1_008", false, 54, 0));

                //root.MinionFriend.Add(Card.Create("EX1_393", false, 96, 1));
               
            */
            
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("seed.seed", FileMode.Open, FileAccess.Read, FileShare.None);
            root  = formatter.Deserialize(stream) as Board;
            stream.Close();
            
            s.CreateLogFolder();
            s.SeedSimulation(root);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            s.Simulate();

            stopWatch.Stop();
            Console.WriteLine("Simulation stopped after :" + (stopWatch.ElapsedMilliseconds / 1000.0f).ToString());
            Console.ReadLine();
        }
    }
}
