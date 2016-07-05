
using JShibo.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Tests
{
    public class ShiboWordSegmentTests
    {
        #region 测试方法集

        public static void Seg()
        {
            DoubleArrayTire<int> tire =ShiboWordSegment.GetTireSort();
            List<string> words = tire.Seg("");
            foreach (string word in words)
                Console.WriteLine(word);
            Console.ReadLine();

        }

        #endregion

        

    }
}
