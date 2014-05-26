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
            ValuesInterface.LoadValuesFromFile();

            int indexMapped = 0;
            try
            {/*
                using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("testmap"))
                {

                    Mutex mutex = Mutex.OpenExisting("testmapmutex");
                    mutex.WaitOne();
                    using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        indexMapped = reader.ReadInt32();
                        byte[] mem = reader.ReadBytes(indexMapped);
                        root = HREngine.Bots.Debugger.BinaryDeSerialize(mem) as Board;
                    }
                    mutex.ReleaseMutex();
*/
                root = HREngine.Bots.Debugger.BinaryDeSerialize(File.ReadAllBytes(args[0])) as Board;
                int nbThread = 6;
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

                    int maxWidePerThread = 8000;
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
                            SimulationThread threadQuickSearch = new SimulationThread(1000);
                            Thread threadlQuickSearch = new Thread(new ParameterizedThreadStart(threadQuickSearch.Calculate));
                            threadlQuickSearch.Start((object)new SimulationThreadStart(input, ref Childs));
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
                        else if (endBoard.GetValue() == BestBoard.GetValue())
                            if (endBoard.FriendCardDraw > BestBoard.FriendCardDraw)
                                BestBoard = endBoard;
                    }

                    Stream stream = new FileStream(args[0] + ".out", FileMode.Create, FileAccess.Write, FileShare.None);
                    byte[] mem = HREngine.Bots.Debugger.Serialize(BestBoard);
                    stream.Write(mem, 0, mem.GetLength(0));
                    stream.Close();
                    /*mutex.WaitOne();

                    using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                    {
                        byte[] mem = HREngine.Bots.Debugger.Serialize(BestBoard);

                        BinaryWriter writer = new BinaryWriter(stream);
                        Console.WriteLine("write mem lengh" + mem.GetLength(0).ToString());
                        Console.WriteLine("write mem lengh" + mem.Length.ToString());
                        writer.Write(mem.Length);
                        writer.Write(mem);
                    }
                    mutex.ReleaseMutex();
                }*/
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Memory-mapped file does not exist. Run Process A first.");
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
        int maxWide = 10000;
        static bool ShouldStop = false;
        static object sync = new Object();
        List<Board> input = null;
        List<Board> output = null;
        public SimulationThread(int maxWide)
        {
            this.maxWide = maxWide;
        }

        public void Calculate(object start)
        {
            Console.WriteLine("Calculating stuff");
            lock (sync)
            {

                SimulationThreadStart starter = start as SimulationThreadStart;
                this.input = starter.input;
                this.output = starter.output;
            }
            int wide = 0;
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

                    lock (sync)
                    {
                        if (ShouldStop)
                            break;
                    }

                    foreach (HREngine.Bots.Action a in b.CalculateAvailableActions())
                    {
                        lock (sync)
                        {
                            if (ShouldStop)
                                break;
                        }


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

                if (BestBoard != null)
                {
                    if (BestBoard.GetValue() > 10000)
                    {
                        lock (sync)
                        {
                            ShouldStop = true;
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
