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
                int nbThread = Environment.ProcessorCount;
                List<Board> Roots = new List<Board>();
                List<Board> Childs = new List<Board>();

                foreach (HREngine.Bots.Action a in root.CalculateAvailableActions())
                {
                    Board tmp = root.ExecuteAction(a);
                    Roots.Add(tmp);
                    Childs.Add(tmp);
                }

                if (Roots.Count < nbThread)
                    nbThread = Roots.Count;

                Childs.Add(root);
                int[] tab = new int[nbThread];

                int roll = 0;
                //lazy dispatch(tired lol)
                for (int i = Roots.Count; i > 0; i--)
                {

                    tab[roll]++;

                    if (roll == nbThread - 1)
                        roll = 0;
                    else
                        roll++;
                }
                int maxWidePerThread = 1;
                if(nbThread > 0)
                {
                    maxWidePerThread = 10000 / nbThread;
                }
                
                Console.WriteLine("Max per thread :" + maxWidePerThread.ToString());
                bool useQuickSearch = true;
                int lastStartRange = 0;
                List<Thread> tt = new List<Thread>();
                for (int i = 0; i < nbThread; i++)
                {
                    List<Board> input = null;

                    input = Roots.GetRange(lastStartRange, tab[i]);
                    lastStartRange += tab[i];
                    if (i == 0 && useQuickSearch)
                    {
                        SimulationThread threadQuickSearch = new SimulationThread(maxWidePerThread/3);
                        Thread threadlQuickSearch = new Thread(new ParameterizedThreadStart(threadQuickSearch.Calculate));

                        List<Board> quickSearchBoards = new List<Board>();
                        foreach(Board v in input)
                        {
                            quickSearchBoards.Add(Board.Clone(v));
                        }

                        threadlQuickSearch.Start((object)new SimulationThreadStart(quickSearchBoards, ref Childs));
                        tt.Add(threadlQuickSearch);
                    }

                    SimulationThread thread = new SimulationThread(maxWidePerThread);
                    Thread threadl = new Thread(new ParameterizedThreadStart(thread.Calculate));
                    threadl.Start((object)new SimulationThreadStart(input, ref Childs));
                    tt.Add(threadl);
                }

                foreach (Thread t in tt)
                {
                    t.Join();
                }

                Board BestBoard = null;
                foreach (Board b in Childs)
                {
                    Board endBoard = Board.Clone(b);
                    endBoard.EndTurn();

                    if (BestBoard == null)
                        BestBoard = endBoard;
                    else if (endBoard.GetValue() > BestBoard.GetValue())
                        BestBoard = endBoard;
                }
                bestBoard = BestBoard;

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
                        else if (acc.Actor.Behavior.ShouldBePlayed(root))
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
    class SimulationThreadStart
    {
        public List<Board> input = null;
        public List<Board> output = null;
        public SimulationThreadStart(List<Board> input, ref List<Board> output)
        {
            this.input = input;
            this.output = output;
        }
    }

    class SimulationThread
    {
        static volatile bool ShouldStop = false;
        int maxWide = 0;
        List<Board> input = null;
        List<Board> output = null;
        public SimulationThread(int maxWide)
        {
            this.maxWide = maxWide;
        }

        public void Calculate(object start)
        {
            //ValuesInterface.LoadValuesFromFile();
            SimulationThreadStart starter = start as SimulationThreadStart;
            this.input = starter.input;
            this.output = starter.output;

            int wide = 0;
            if (input == null)
                return;
            if (output == null)
                return;
            Board BestBoard = null;

            List<Board> childaas = new List<Board>();

            while (input.Count > 0)
            {
                if (ShouldStop)
                    break;
                childaas.Clear();
                wide = 0;
                foreach (Board b in input)
                {
                    int boardWide = 0;
                    float maxWidePerBoard = maxWide/input.Count;
                    foreach (HREngine.Bots.Action a in b.CalculateAvailableActions())
                    {
                        boardWide++;
                        wide++;
                        Board bb = b.ExecuteAction(a);

                        bool found = false;
                        foreach (Board lol in childaas)
                        {
                            if (bb.Equals(lol))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            childaas.Add(bb);
                            if (bb.GetValue() > 10000)
                                ShouldStop = true;
                        }

                        if (boardWide > maxWidePerBoard)
                            break;
                        if (wide > maxWide)
                            break;
                        if (ShouldStop)
                            break;

                        if (BestBoard == null)
                            BestBoard = bb;
                    }
                    if (wide > maxWide)
                        break;
                    if (ShouldStop)
                        break;
                }
                foreach (Board baa in childaas)
                {
                    if (baa.GetValue() > BestBoard.GetValue())
                        BestBoard = baa;
                }
                input.Clear();
                foreach(Board aaa in childaas)
                {
                    input.Add(aaa);
                }
            }

            if (BestBoard != null)
            {
                output.Add(BestBoard);
            }
            return;
        }

    }
}
