using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Serialization;

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
            System.IO.Directory.CreateDirectory(CardTemplate.DatabasePath + "Bots/SmartCC/Logs/" + nameFolder);
            CurrentFolder = CardTemplate.DatabasePath + "Bots/SmartCC/Logs/" + nameFolder;
        }

        public void SerializeRoot()
        {
            Stream stream = new FileStream(CurrentFolder + "/Turn" + TurnCount.ToString() + "_" + SimuCount.ToString() + ".seed", FileMode.Create, FileAccess.Write, FileShare.None);
            byte[] mem = Debugger.Serialize(root);
            stream.Write(mem,0,mem.GetLength(0));
            stream.Close();
        }

        public void Log(string msg)
        {
            StreamWriter sw = new StreamWriter(CurrentFolder + "/Turn" + TurnCount.ToString() + ".log", true);
            sw.WriteLine(msg);
            sw.Close();
        }

        public void Simulate()
        {

            SerializeRoot();
            Console.WriteLine();
            NeedCalculation = false;

            List<Board> boards = new List<Board>();
            List<Board> AllBoards = new List<Board>();

            boards.Add(root);
            AllBoards.Add(root);
            int wide = 0;
            int depth = 0;
            int maxDepth = 10;
            int maxWide = 5000;
            int skipped = 0;
            root.Update();
            bool tryToSkipEqualBoards = true;
            Board bestBoard = root;
            Log("ROOTBOARD : ");
            Log(root.ToString());
            Log("");
            Console.WriteLine(root.ToString());
            bool foundearly = false;
            while (boards.Count != 0)
            {
                if (depth >= maxDepth)
                    break;
                if (wide >= maxWide)
                    break;
                wide = 0;
                skipped = 0;
                int childsCount = 0;
                List<Board> childs = new List<Board>();
                foreach (Board b in boards)
                {
                    if (wide >= maxWide)
                        break;
                    wide++;
                    childsCount++;

                    List<Action> actions = b.CalculateAvailableActions();

                    /*if(depth == 0)
                    {
                        bool containsCast = false;
                        foreach (Action a in actions)
                        {
                            if (a.Type == Action.ActionType.CAST_ABILITY || a.Type == Action.ActionType.CAST_MINION || a.Type == Action.ActionType.CAST_SPELL)
                            {
                                if (a.Actor.template.Id != "GAME_005")
                                {
                                    containsCast = true;
                                }
                            }
                        }
                        if (containsCast)
                        {
                            foreach (Action a in actions.ToArray())
                            {
                                if (a.Type != Action.ActionType.CAST_ABILITY && a.Type != Action.ActionType.CAST_MINION && a.Type != Action.ActionType.CAST_SPELL && a.Type != Action.ActionType.RESIMULATE)
                                {
                                    actions.Remove(a);
                                }
                            }
                        }
                    }
                   
 */

                    foreach (Action a in actions)
                    {
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
                if (wide >= maxWide)
                    break;

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

            Action actionPrior = null;
            foreach (Action acc in bestBoard.ActionsStack)
            {
                if (actionPrior == null && acc.Actor != null)
                {
                    if (acc.Actor.Behavior.GetPriorityPlay(bestBoard) > 1 && acc.Type != Action.ActionType.MINION_ATTACK && acc.Type != Action.ActionType.HERO_ATTACK)
                    {
                        Console.WriteLine("Action priori found");
                        if (acc.Type == Action.ActionType.CAST_MINION)
                        {
                            if (bestBoard.MinionFriend.Count < 7)
                                actionPrior = acc;

                        }
                        else
                        {
                            actionPrior = acc;

                        }
                    }

                }
                if (actionPrior != null && acc.Actor != null)
                {
                    if (actionPrior.Actor.Behavior.GetPriorityPlay(bestBoard) < acc.Actor.Behavior.GetPriorityPlay(bestBoard) && acc.Type != Action.ActionType.MINION_ATTACK && acc.Type != Action.ActionType.HERO_ATTACK)
                    {
                        if (acc.Type == Action.ActionType.CAST_MINION)
                        {
                            if (bestBoard.MinionFriend.Count < 7)
                                actionPrior = acc;

                        }
                        else
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
