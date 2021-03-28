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
            command.CalcNewParameter();
            AddHistory(command);
            return command.After;
        }

        private void AddHistory(CutLineCommand command)
        {
            // 操作後のパラメーターに変化がない場合、その操作を保持しない
            if (command.After.Equals(command.Before))
            {
                return;
            }

            if (Index < GetLatestHistoryIndex())
            {
                // 履歴の途中に操作を差し込んだ場合、旧最新操作履歴を削除する
                int deleteNum = CommandList.Count - (Index + 1);    // = 現時点のIndexの次から全てを削除
                DeleteNewerHistory(deleteNum);
            }

            // パラメーター変更された操作を履歴の最新操作とする
            AddHistoryCore(command);
        }

        private int GetLatestHistoryIndex()
        {
            return CommandList.Count - 1;
        }

        private void DeleteNewerHistory(int deleteNum)
        {
            // 例
            // Index:                        ↓(= 1)
            // 履歴 :  [0] Init  [1] Key.Down  [2] Key.Down  [3] 右下点操作で縮小
            //                                ↑ここに操作を入れる場合、2つ削除する
            //  -> RemoveRange(2, deleteNum = 2)
            int deleteStart = CommandList.Count - deleteNum;
            CommandList.RemoveRange(deleteStart, deleteNum);
        }

        private void AddHistoryCore(CutLineCommand command)
        {
            CommandList.Add(command);
            Index++;
        }

        public CutLineParameter Undo(int undoNum)
        {
            Index = Math.Max(0, Index - undoNum);
            return CommandList[Index].After;
        }

        public CutLineParameter Redo(int redoNum)
        {
            Index = Math.Min(Index + redoNum, GetLatestHistoryIndex());
            return CommandList[Index].After;
        }
    }
}
