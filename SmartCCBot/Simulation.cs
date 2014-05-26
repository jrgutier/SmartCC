using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace HREngine.Bots
{
    public class Simulation
    {
        public bool NeedCalculation { get; set; }

        public List<Action> ActionStack { get; set; }
        public Board root { get; set; }
        public int TurnCount { get; set; }

        public int SimuCount { get; set; }



        private string CurrentFolder { get; set; }

        public Action GetNextAction()
        {
            if (ActionStack.Count == 0 && !NeedCalculation)
            {
                NeedCalculation = true;
                return new Action(Action.ActionType.END_TURN, null);
            }

            Action ActionToDo = ActionStack[0];
            ActionStack.Remove(ActionToDo);

            return ActionToDo;
        }

        public Simulation()
        {
            root = null;
            ActionStack = new List<Action>();
            NeedCalculation = true;
            SimuCount = 0;
        }

        public bool SeedSimulation(Board b)
        {
            if (root != null)
                return false;
            root = b;
            return true;
        }

        public void CreateLogFolder()
        {
            string nameFolder = DateTime.Now.ToString().Replace("/", "-").Replace(":", "-");
            System.IO.Directory.CreateDirectory(CardTemplate.DatabasePath + "" + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "SmartCC" + Path.DirectorySeparatorChar + "Logs" + Path.DirectorySeparatorChar + "" + nameFolder);
            CurrentFolder = CardTemplate.DatabasePath + "" + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "SmartCC" + Path.DirectorySeparatorChar + "Logs" + Path.DirectorySeparatorChar + "" + nameFolder;
        }

        public void SerializeRoot()
        {
            Stream stream = new FileStream(CurrentFolder + "" + Path.DirectorySeparatorChar + "Turn" + TurnCount.ToString() + "_" + SimuCount.ToString() + ".seed", FileMode.Create, FileAccess.Write, FileShare.None);
            byte[] mem = Debugger.Serialize(root);
            stream.Write(mem, 0, mem.GetLength(0));
            stream.Close();
        }

        public void Log(string msg)
        {
            StreamWriter sw = new StreamWriter(CurrentFolder + "" + Path.DirectorySeparatorChar + "Turn" + TurnCount.ToString() + ".log", true);
            sw.WriteLine(msg);
            sw.Close();
        }


        public void Simulate(bool threaded)
        {

            SerializeRoot();
            Console.WriteLine();
            NeedCalculation = false;

            List<Board> boards = new List<Board>();
            boards.Add(root);
            int wide = 0;
            int depth = 0;
            int maxDepth = 15;
            int maxWide = 10000;
            int skipped = 0;
            root.Update();
            bool tryToSkipEqualBoards = true;
            Board bestBoard = root;
            Log("ROOTBOARD : ");
            Log(root.ToString());
            Log("");
            Console.WriteLine(root.ToString());
            bool foundearly = false;

            if (threaded)
            {
               /* using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew("testmap", 100000))
                {
                    /*
                    bool mutexCreated;
                    Mutex mutex = new Mutex(true, "testmapmutex", out mutexCreated);
                    using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                    {
                        BinaryWriter writer = new BinaryWriter(stream);
                        byte[] mem = HREngine.Bots.Debugger.Serialize(bestBoard);
                        writer.Write(mem.GetLength(0));
                        writer.Write(mem);
                        
                    }
                    mutex.ReleaseMutex();
                    */
                    Stream stream = new FileStream(CardTemplate.DatabasePath + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "SmartCC" + Path.DirectorySeparatorChar + "Threads" + Path.DirectorySeparatorChar + "board.seed", FileMode.Create, FileAccess.Write, FileShare.None);
                    byte[] mem = HREngine.Bots.Debugger.Serialize(bestBoard);
                    stream.Write(mem, 0, mem.GetLength(0));
                    stream.Close();
                    // Use ProcessStartInfo class
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = false;
                    startInfo.FileName = CardTemplate.DatabasePath + "" + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "ExternalSimulationThread.exe";
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    startInfo.Arguments = "\"" + CardTemplate.DatabasePath + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "SmartCC" + Path.DirectorySeparatorChar + "Threads" + Path.DirectorySeparatorChar + "board.seed" + "\"";

                    try
                    {
                        // Start the process with the info we specified.
                        // Call WaitForExit and then the using statement will close.
                        using (Process exeProcess = Process.Start(startInfo))
                        {
                            exeProcess.WaitForExit();
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Compiler error");
                    }
                    bestBoard = HREngine.Bots.Debugger.BinaryDeSerialize(File.ReadAllBytes(CardTemplate.DatabasePath + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "SmartCC" + Path.DirectorySeparatorChar + "Threads" + Path.DirectorySeparatorChar + "board.seed.out")) as Board;

/*
                    mutex.WaitOne();
                    using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        int index = reader.ReadInt32();
                        byte[] mem = reader.ReadBytes(index);
                        bestBoard = HREngine.Bots.Debugger.BinaryDeSerialize(mem) as Board;
                    }
                    mutex.ReleaseMutex();
 * */
               // }
         
            }
            else
            {
                while (boards.Count != 0)
                {
                    if (depth >= maxDepth)
                        break;

                    wide = 0;
                    skipped = 0;
                    int childsCount = 0;
                    List<Board> childs = new List<Board>();


                    foreach (Board b in boards)
                    {
                       
                       
                        List<Action> actions = b.CalculateAvailableActions();

                        foreach (Action a in actions)
                        {
                            if (wide >= maxWide)
                                break;
                            wide++;
                            childsCount++;

                            Board bb = b.ExecuteAction(a);
                           /* 
                             Console.WriteLine(a.ToString());
                             Console.WriteLine("**************************************");
                             Console.WriteLine(bb.ToString());
                             */
                            if (bb != null)
                            {
                                if (bb.GetValue() > 10000)
                                {
                                    bestBoard = bb;
                                    foundearly = true;
                                    break;
                                }

                                if (tryToSkipEqualBoards)
                                {
                                    bool found = false;
                                    foreach (Board lol in childs)
                                    {
                                        if (bb.Equals(lol))
                                        {
                                            found = true;
                                            break;
                                        }
                                    }

                                    if (!found)
                                    {
                                        childs.Add(bb);
                                    }
                                    else
                                    {
                                        skipped++;
                                    }
                                }
                                else
                                {
                                    childs.Add(bb);
                                }
                            }
                        }
                        if (foundearly)
                            break;
                    }


                    if (!foundearly)
                    {
                        foreach (Board baa in childs)
                        {
                            Board endBoard = Board.Clone(baa);
                            endBoard.EndTurn();
                            if (endBoard.GetValue() > bestBoard.GetValue())
                            {
                                bestBoard = endBoard;
                            }
                        }
                    }
                    else
                    {
                        Log("Found early at : " + depth.ToString() + " | " + wide.ToString());
                        Console.WriteLine("Found Early");
                        break;

                    }

                    Log("Simulation :" + depth.ToString() + " | " + wide.ToString() + " | " + skipped.ToString());
                    Console.WriteLine("Simulation :" + depth.ToString() + " | " + wide.ToString() + " | " + skipped.ToString());
                    boards.Clear();
                    boards = childs;
                    depth++;
                }
            }


            Action actionPrior = null;
            foreach (Action acc in bestBoard.ActionsStack)
            {
                if (actionPrior == null && acc.Actor != null)
                {
                    if (acc.Actor.Behavior.GetPriorityPlay(bestBoard) > 1 && acc.Type != Action.ActionType.MINION_ATTACK && acc.Type != Action.ActionType.HERO_ATTACK)
                    {
                        Console.WriteLine("Action priori found");
                        if (acc.Type == Action.ActionType.CAST_MINION && acc.Actor.Behavior.ShouldBePlayed(root))
                        {
                            if (bestBoard.MinionFriend.Count < 7)
                                actionPrior = acc;

                        }
                        else if(acc.Actor.Behavior.ShouldBePlayed(root))
                        {
                            actionPrior = acc;

                        }
                    }
                }
            }


            List<Action> finalStack = new List<Action>();
            if (actionPrior != null)
            {
                finalStack.Add(actionPrior);
                if (bestBoard.ActionsStack.IndexOf(actionPrior) + 2 <= bestBoard.ActionsStack.Count)
                {
                    if (bestBoard.ActionsStack[bestBoard.ActionsStack.IndexOf(actionPrior) + 1] != null)
                    {
                        if (bestBoard.ActionsStack[bestBoard.ActionsStack.IndexOf(actionPrior) + 1].Type == Action.ActionType.RESIMULATE)
                        {
                            finalStack.Add(new Action(Action.ActionType.RESIMULATE, null));
                        }

                    }
                    foreach (Action a in bestBoard.ActionsStack)
                    {
                        if (!finalStack.Contains(a))
                        {
                            finalStack.Add(a);
                        }
                    }
                }


            }
            else
            {
                finalStack = bestBoard.ActionsStack;
            }




            ActionStack = finalStack;
            Log("");
            Log("");
            Log("");

            Log("");
            Log("BEST BOARD FOUND");
            Log(bestBoard.ToString());
            Console.WriteLine("---------------------------------");
            Console.WriteLine(bestBoard.ToString());
            Console.WriteLine("---------------------------------");
            
            Log("");
            Log("Actions:");

            foreach (HREngine.Bots.Action a in ActionStack)
            {
                Log(a.ToString());

                Console.WriteLine(a.ToString());
            }

        }
    }
}
