using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HREngine.Bots;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

namespace ExternalSimulationThread
{
    class Program
    {
        static object sync = new Object();

        static void Main(string[] args)
        {

            Board root = new Board();
            CardTemplate.DatabasePath = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0].Replace("\\Bots\\ExternalSimulationThread.exe", "") + "/");
            CardTemplate.LoadAll();
            root = HREngine.Bots.Debugger.BinaryDeSerialize(File.ReadAllBytes(args[0])) as Board;


            ValuesInterface.LoadValuesFromFile();
            int nbThread = 100;
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

            List<Thread> tt = new List<Thread>();
            for (int i = 0; i < nbThread; i++)
            {
                List<Board> input = null;
                if (i == nbThread - 1)
                {
                    input = Roots.GetRange(i * (Roots.Count / nbThread), (Roots.Count / nbThread) + (Roots.Count % nbThread));
                }
                else
                {
                    input = Roots.GetRange(i * (Roots.Count / nbThread), (Roots.Count / nbThread));
                }

                SimulationThread thread = new SimulationThread();
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
                else if (endBoard.GetValue() == BestBoard.GetValue())
                    if (endBoard.FriendCardDraw > BestBoard.FriendCardDraw)
                        BestBoard = endBoard;
            }

            Stream stream = new FileStream(args[0]+".out", FileMode.Create, FileAccess.Write, FileShare.None);
            byte[] mem = HREngine.Bots.Debugger.Serialize(BestBoard);
            stream.Write(mem, 0, mem.GetLength(0));
            stream.Close();

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
        int maxWide = 20000;
        static bool ShouldStop = false;
        static object sync = new Object();
        List<Board> input = null;
        List<Board> output = null;
        public SimulationThread()
        {

        }

        public void Calculate(object start)
        {
            int wide = 0;
            SimulationThreadStart starter = start as SimulationThreadStart;
            this.input = starter.input;
            this.output = starter.output;
            if (input == null)
                return;
            if (output == null)
                return;
            Board BestBoard = null;

            while (input.Count > 0)
            {
                wide = 0;
                lock (sync)
                {
                    if (ShouldStop)
                        break;
                }
                List<Board> childs = new List<Board>();
                foreach (Board b in input)
                {

                    foreach (HREngine.Bots.Action a in b.CalculateAvailableActions())
                    {
                        if (wide > maxWide)
                            break;

                        wide++;
                        Board bb = b.ExecuteAction(a);

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

                    }
                    if (wide > maxWide)
                        break;
                }

                foreach (Board b in childs)
                {
                    if (BestBoard == null)
                        BestBoard = b;
                    else if (b.GetValue() > BestBoard.GetValue())
                        BestBoard = b;
                    else if (b.GetValue() == BestBoard.GetValue())
                        if (b.FriendCardDraw > BestBoard.FriendCardDraw)
                            BestBoard = b;

                }

                if(BestBoard != null)
                {
                    if (BestBoard.GetValue() > 10000)
                    {
                        lock (sync)
                        {
                            ShouldStop = true;
                            output.Add(BestBoard);
                        }
                    }
                }
                
                input = childs;
            }

            if (BestBoard != null)
            {
                lock (sync)
                {
                    output.Add(BestBoard);
                }
            }

        }

    }
}
