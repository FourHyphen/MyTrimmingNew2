using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrimmingNew2
{
    public class CutLineCommandHistory
    {
        private List<CutLineCommand> CommandList { get; set; } = new List<CutLineCommand>();

        private int Index { get; set; } = 0;

        public CutLineParameter Execute(CutLineCommand command)
        {
            CutLineParameter after = command.CalcNewParameter();
            AddHistory(command);
            return after;
        }

        private void AddHistory(CutLineCommand command)
        {
            CommandList.Add(command);
            Index++;
        }

        public CutLineParameter Undo(int undoNum)
        {
            if (Index <= 1)
            {
                return CommandList[Index].Before;
            }

            Index -= undoNum;
            return CommandList[Index].Before;
        }

        public CutLineParameter Redo(int redoNum)
        {
            if (Index >= CommandList.Count - 1)
            {
                return CommandList[Index].After;
            }

            Index += redoNum;
            return CommandList[Index].After;
        }
    }
}
