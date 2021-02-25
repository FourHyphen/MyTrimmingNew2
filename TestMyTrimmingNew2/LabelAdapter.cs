using RM.Friendly.WPFStandardControls;
using System;
using System.Reflection;

namespace TestMyTrimmingNew2
{
    public class LabelAdapter : DisplayControl
    {
        public string LabelName { get; }

        public LabelAdapter(string labelName)
        {
            LabelName = labelName;
        }

        public string Content(IWPFDependencyObjectCollection<System.Windows.DependencyObject> logicalTree)
        {
            var label = logicalTree.ByType<System.Windows.Controls.Label>().ByName(LabelName).Single();
            if (label == null)
            {
                Failure(MethodBase.GetCurrentMethod().Name, LabelName);
            }

            string str = label.ToString().Replace("System.Windows.Controls.Label: ", "");
            str = str.Replace("System.Windows.Controls.Label", "");    // 文字列が空の場合の対応
            return str;
        }

        public int ContentNum(IWPFDependencyObjectCollection<System.Windows.DependencyObject> logicalTree)
        {
            string str = Content(logicalTree);
            str = str.Trim();
            return int.Parse(str);
        }
    }
}
