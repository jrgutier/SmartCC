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

        public Card ChoiceTarget { get; set; }



        private string CurrentFolder { get; set; }

        public Action GetNextAction()
        {
            if (ActionStack.Count == 1 && ActionStack[0].Type == Action.ActionType.RESIMULATE)
                ActionStack.Clear();
            if (ActionStack.Count == 0 && !NeedCalculation)
            {
                NeedCalculation = true;
                return new Action(Action.ActionType.END_TURN, null);
            }

            Action ActionToDo = ActionStack[0];
            ActionStack.Remove(ActionToDo);

            return ActionToDo;
        }

        public void InsertChoiceAction(int choice)
        {
            ActionStack.Insert(0, new Action(Action.ActionType.CHOICE, null, null, 0, choice));
        }
        public void InsertTargetAction(Card target)
        {
            ActionStack.Insert(0, new Action(Action.ActionType.TARGET, null, target));
        }

        //             List<Thread> BgThreads = new List<Thread>();
        // int nbThread = 0;

        public Simulation()
        {
            root = null;
            ActionStack = new List<Action>();
            NeedCalculation = true;
            SimuCount = 0;
            ChoiceTarget = null;

            /*for (int i = 0; i < nbThread; i++)
            {
                SimulationThread thread = new SimulationThread();
                Thread threadl = new Thread(new ParameterizedThreadStart(thread.Calculate));
                BgThreads.Add(threadl);
            }*/
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
            DateTime time = DateTime.Now;
            string format = "dd-MM-yy HH-mm-ss";
            string nameFolder = time.ToString(format);
            System.IO.Directory.CreateDirectory(CardTemplate.DatabasePath + "" + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "SmartCC" + Path.DirectorySeparatorChar + "Logs" + Path.DirectorySeparatorChar + "" + nameFolder);
            CurrentFolder = CardTemplate.DatabasePath + "" + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "SmartCC" + Path.DirectorySeparatorChar + "Logs" + Path.DirectorySeparatorChar + "" + nameFolder;
        }

        public void SerializeRoot()
        {
            if (TurnCount > 40)
                return;
            Stream stream = new FileStream(CurrentFolder + "" + Path.DirectorySeparatorChar + "Turn" + TurnCount.ToString() + "_" + SimuCount.ToString() + ".seed", FileMode.Create, FileAccess.Write, FileShare.None);
            byte[] mem = Debugger.Serialize(root);
            stream.Write(mem, 0, mem.GetLength(0));
            stream.Close();
        }

        public void Log(string msg)
        {
            if (TurnCount > 40)
                return;
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
            int maxWide = 3000;
            int maxBoards = 2000;

            int maxWideT = 5000;
            int maxBoardsT = 3000;

            int skipped = 0;
            root.Update();
            bool tryToSkipEqualBoards = true;
            Board bestBoard = root;
            Log("ROOTBOARD : ");
            Log(root.ToString());
            Log("");
            Console.WriteLine(root.ToString());
            bool foundearly = false;

            List<Board> Roots = new List<Board>();
            List<Board> Childs = new List<Board>();
            if (threaded)
            {
                foreach (HREngine.Bots.Action a in root.CalculateAvailableActions())
                {
                    Board tmp = root.ExecuteAction(a);
                    Roots.Add(tmp);
                    Childs.Add(tmp);
                }

                Childs.Add(root);

               

                while (Roots.Count > 0)
                {
                    wide = Roots.Count;
                    if (Roots.Count > maxWideT)
                        wide = maxWideT;

                    float widePerTree = 0;
                    float wideTree = 0;
                    widePerTree =2;

                    ManualResetEvent[] doneEvents = new ManualResetEvent[wide];

                    for (int i = 0; i < wide; i++)
                    {
                        doneEvents[i] = new ManualResetEvent(false);
                        SimulationThread thread = new SimulationThread();

                        ThreadPool.QueueUserWorkItem(thread.Calculate, (object)new SimulationThreadStart(Roots[i], ref Childs, doneEvents[i], widePerTree));
                    }
                    foreach (var e in doneEvents)
                        e.WaitOne();

                    bool foundLethal = false;
                    foreach (Board baa in Childs)
                    {
                        if (baa == null)
                            continue;

                        if(baa.GetValue() > 10000)
                        {
                            foundLethal = true;
                            bestBoard = baa;
                            break;
                        }



                        Board endBoard = Board.Clone(baa);
                        endBoard.EndTurn();

                        bestBoard.CalculateEnemyTurn();
                        if (bestBoard.EnemyTurnWorseBoard != null)
                        {
                            endBoard.CalculateEnemyTurn();
                            Board worstBoard = endBoard.EnemyTurnWorseBoard;

                            if (worstBoard == null)
                                worstBoard = endBoard;

                            if (worstBoard.GetValue() > bestBoard.EnemyTurnWorseBoard.GetValue())
                            {
                                bestBoard = endBoard;
                            }
                            else if (worstBoard.GetValue() == bestBoard.EnemyTurnWorseBoard.GetValue())
                            {
                                if (endBoard.GetValue() > bestBoard.GetValue())
                                {
                                    bestBoard = endBoard;
                                }
                            }

                        }
                        else
                        {
                            if (endBoard.GetValue() > bestBoard.GetValue())
                            {
                                bestBoard = endBoard;
                            }
                        }
                    }

                    if (foundLethal)
                        break;
                    Roots.Clear();
                    int boardsAdded = 0;
                    //Childs.RemoveAll(item => item == null);
                    //Childs.Sort((x, y) => y.GetValue().CompareTo(x.GetValue()));

                    foreach (Board bbb in Childs)
                    {
                        if (bbb != null)
                        {
                            Roots.Add(bbb);
                            boardsAdded++;
                            if (boardsAdded > maxBoardsT)
                                break;
                        }
                           
                    }
                    Childs.Clear();
                }


            }
            else
            {
                float widePerTree = 0;
                float wideTree = 0;
                while (boards.Count != 0)
                {
                    if (depth >= maxDepth)
                        break;

                    wide = 0;
                    skipped = 0;
                    List<Board> childs = new List<Board>();

                    widePerTree = maxWide / boards.Count;

                    foreach (Board b in boards)
                    {
                        wideTree = 0;

                        List<Action> actions = b.CalculateAvailableActions();
                        foreach (Action a in actions)
                        {
                            if (wide > maxWide)
                                break;
                            if (wideTree >= widePerTree)
                                break;

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
                                        wideTree++;
                                        wide++;
                                        childs.Add(bb);
                                    }
                                    else
                                    {
                                        skipped++;
                                    }
                                }
                                else
                                {
                                    wideTree++;
                                    wide++;
                                    childs.Add(bb);
                                }
                            }
                        }
                        if (foundearly)
                            break;
                    }


                    if (!foundearly)
                    {
                        List<Board> bestBoards = new List<Board>();

                        int limit = maxBoards;
                        if (childs.Count < maxBoards)
                            limit = childs.Count;

                        childs.Sort((x, y) => y.GetValue().CompareTo(x.GetValue()));
                        childs = new List<Board>(childs.GetRange(0, limit));
                        
                        foreach (Board baa in childs)
                        {

                            Board endBoard = Board.Clone(baa);
                            endBoard.EndTurn();

                            bestBoard.CalculateEnemyTurn();
                            if (bestBoard.EnemyTurnWorseBoard != null)
                            {
                                endBoard.CalculateEnemyTurn();
                                Board worstBoard = endBoard.EnemyTurnWorseBoard;

                                if (worstBoard == null)
                                    worstBoard = endBoard;

                                if (worstBoard.GetValue() > bestBoard.EnemyTurnWorseBoard.GetValue())
                                {
                                    bestBoard = endBoard;
                                }
                                else if (worstBoard.GetValue() == bestBoard.EnemyTurnWorseBoard.GetValue())
                                {
                                    if (endBoard.GetValue() > bestBoard.GetValue())
                                    {
                                        bestBoard = endBoard;
                                    }
                                }

                            }
                            else
                            {
                                if (endBoard.GetValue() > bestBoard.GetValue())
                                {
                                    bestBoard = endBoard;
                                }
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
                            if (root.MinionFriend.Count < 7)
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

                }

                foreach (Action a in bestBoard.ActionsStack)
                {
                    if (a != actionPrior || a.Type == Action.ActionType.RESIMULATE)
                    {
                        if (a.Type == Action.ActionType.RESIMULATE && finalStack[finalStack.Count - 1].Type == Action.ActionType.RESIMULATE)
                            continue;

                        finalStack.Add(a);
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


            foreach (HREngine.Bots.Action a in ActionStack)
            {
                Log(a.ToString());

                Console.WriteLine(a.ToString());
            }

        }
    }
    class SimulationThreadStart
    {
        public Board root = null;
        public List<Board> output = null;
        public ManualResetEvent doneEvent;
        public float maxWidePerTree = 0;

        public SimulationThreadStart(Board root, ref List<Board> output, ManualResetEvent doneEvent,float maxWidePerTree)
        {
            this.root = root;
            this.output = output;
            this.doneEvent = doneEvent;
            this.maxWidePerTree = maxWidePerTree;

        }
    }

    class SimulationThread
    {
        Board root = null;
        List<Board> output = null;
        public ManualResetEvent doneEvent;
        public float maxWidePerTree = 0;
        public static volatile bool FoundLethal = false;
        public SimulationThread()
        {
        }

        public void Calculate(object start)
        {

            SimulationThreadStart st = (SimulationThreadStart)start;
            root = st.root;
            output = st.output;
            doneEvent = st.doneEvent;
            maxWidePerTree = st.maxWidePerTree;
            if(FoundLethal)
            {
                doneEvent.Set();
                return;
            }


            int wide = 0;
            int skipped = 0;

            List<Board> boards = new List<Board>();

            boards.Add(root);
            root.Update();


            bool tryToSkipEqualBoards = false;
            Board bestBoard = null;

            wide = 0;
            skipped = 0;
            List<Board> childs = new List<Board>();

            foreach (Board b in boards)
            {
                if (FoundLethal)
                {
                    doneEvent.Set();
                    return;
                }
                if (wide > maxWidePerTree)
                    break;
                List<Action> actions = b.CalculateAvailableActions();
                foreach (Action a in actions)
                {
                    if (FoundLethal)
                    {
                        doneEvent.Set();
                        return;
                    }
                    if (wide > maxWidePerTree)
                        break;

                    Board bb = b.ExecuteAction(a);

                    if (bb != null)
                    {
                        if (bb.GetValue() > 10000)
                        {
                            FoundLethal = true;
                            bestBoard = bb;
                            if (bestBoard != null)
                            {
                                output.Add(bestBoard);
                            }
                            doneEvent.Set();
                            return;
                        }
                        if (tryToSkipEqualBoards)
                        {
                            bool found = false;
                            foreach (Board lol in output.ToArray())
                            {
                              
                                if (bb.Equals(lol))
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                wide++;
                                childs.Add(bb);
                            }
                            else
                            {
                                skipped++;
                            }
                        }
                        else
                        {
                            wide++;
                            childs.Add(bb);
                        }
                    }
                }
            }

            foreach (Board b in childs)
            {
                if (b != null)
                    output.Add(b);
            }
            doneEvent.Set();

        }
    }
}
