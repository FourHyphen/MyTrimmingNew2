using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMyTrimmingNew2
{
    class Common
    {
        public static string GetEnvironmentDirPath()
        {
            if (System.IO.Directory.Exists(Environment.CurrentDirectory + "/Resource"))
            {
                // テストスイートの場合、2回目以降？はすでに設定済み
                return Environment.CurrentDirectory;
            }

            string master = Environment.CurrentDirectory + "../../../";  // テストの単体実行時
            if (!System.IO.Directory.Exists(master + "/Resource"))
            {
                // テストスイートによる全テスト実行時
                master = Environment.CurrentDirectory + "../../../../TestMyTrimmingNew2";
            }

            return master;
        }

        public static string GetFilePathOfDependentEnvironment(string filePath)
        {
            return Environment.CurrentDirectory + filePath;
        }

        public static void AreEqualRound(double expected, double actual, int round = 2)
        {
            double expectedRound = Math.Round(expected, round);
            double actualRound = Math.Round(actual, round);
            Assert.AreEqual(expectedRound, actualRound);
        }
    }
}
