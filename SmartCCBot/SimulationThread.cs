using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{
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
        static object sync = new Object();
        List<Board> input = null;
        List<Board> output = null;
        public SimulationThread()
        {
            
        }

        public void Calculate(object start)
        {
            SimulationThreadStart starter = start as SimulationThreadStart;
            this.input = starter.input;
            this.output = starter.output;

            Board BestBoard = null;

            while (input.Count > 0)
            {
                List<Board> childs = new List<Board>();
                foreach (Board b in input)
                {
                    foreach (Action a in b.CalculateAvailableActions())
                    {
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
