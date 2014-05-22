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
using System.Windows.Forms;
using SmartCompiler;
namespace HREngine.Bots
{


    class Program
    {
        static void Main(string[] args)
        {
            Console.BufferHeight = Int16.MaxValue - 1; // ***** Alters the BufferHeight *****

            CardTemplate.DatabasePath = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "/";

            CardTemplate.LoadAll();

            Console.ReadLine();

            StreamReader str = new StreamReader(CardTemplate.DatabasePath + "Bots/SmartCC/Config/useProfiles");
            string useDefaut = str.ReadLine();

            str.Close();

            if (useDefaut == "true")
            {
                Application.EnableVisualStyles();
                Application.Run(new ProfileSelector(CardTemplate.DatabasePath));
            }
            else
            {

                using (CodeCompiler compiler = new CodeCompiler(CardTemplate.DatabasePath + "Bots\\SmartCC\\Profiles\\Defaut\\", CardTemplate.DatabasePath))
                {
                    if (compiler.Compile())
                    {
                    }
                }

            }


            Simulation s = new Simulation();

            Board root = new Board();
            /*
                root.HeroEnemy = Card.Create("HERO_04", false, 0);
                  root.HeroFriend = Card.Create("HERO_04", true, 1);
                  root.HeroFriend.CurrentHealth = 30;
                  root.HeroEnemy.CurrentHealth = 7;

                //  root.Ability = Card.Create("CS2_049", true, 545);

                  //root.Hand.Add(Card.Create("CS2_112", true, 2));
                  //root.Hand.Add(Card.Create("EX1_408", true, 3));
                  //root.Hand.Add(Card.Create("EX1_575", true, 4));
                  //root.Hand.Add(Card.Create("GAME_005", true, 5));

                  root.ManaAvailable = 20;
                  Card c = Card.Create("NEW1_033", false, 3, 0);
                  c.Silence();
                   root.MinionEnemy.Add(c);
                   root.MinionEnemy.Add(Card.Create("CS2_203", false, 4, 0));
              //    root.MinionFriend.Add(Card.Create("EX1_008", false, 54, 0));

                  //root.MinionFriend.Add(Card.Create("EX1_393", false, 96, 1));
               
              */
            Assembly.LoadFile(CardTemplate.DatabasePath + "Bots/SmartCC/Profile.dll");
            root = HREngine.Bots.Debugger.BinaryDeSerialize(File.ReadAllBytes(CardTemplate.DatabasePath + "seed.Seed")) as Board;

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
