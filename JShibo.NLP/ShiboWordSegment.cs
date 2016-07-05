using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP
{
    public class ShiboWordSegment
    {
        #region 词典构建

        public static DoubleArrayTire<int> GetTire()
        {
            DoubleArrayTire<int> tire = new DoubleArrayTire<int>();
            List<string> lines = File.ReadAllLines("Dictionary\\SearchWords.txt").ToList().Where(item => item.Length > 0).ToList();
            for (int i = 1; i < 65536; i++)
            {
                lines.Add(((char)i).ToString());
            }
            lines.Sort(SortAsc);
            int[] codes = new int[lines.Count];
            for (int i = 0; i < codes.Length; i++)
                codes[i] = i;
            tire.Build(lines, codes);
            return tire;
        }

        public static DoubleArrayTire<WordToken> GetTireStruct()
        {
            DoubleArrayTire<WordToken> tire = new DoubleArrayTire<WordToken>();
            List<string> lines = File.ReadAllLines("Dictionary\\SearchWords.txt").ToList().Where(it => it.Length > 0).ToList();
            for (int i = 1; i < 65536; i++)
            {
                lines.Add(((char)i).ToString());
            }
            //lines.Sort(StringSort.SortAsc);
            WordToken[] codes = new WordToken[lines.Count];
            for (int i = 0; i < codes.Length; i++)
                codes[i] = new WordToken(i, lines[i]);
            tire.Build(lines, codes);
            return tire;
        }

        public static DoubleArrayTire<int> GetTireSort()
        {
            DoubleArrayTire<int> tire = new DoubleArrayTire<int>();
            if (tire.Open("tire.bin") == false)
            {
                List<WordTokenResult> list = JsonConvert.DeserializeObject<List<WordTokenResult>>(File.ReadAllText("WordTokenResult.txt"));
                List<WordToken> tokens = new List<WordToken>(list.Count);
                HashSet<string> sets = new HashSet<string>();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Word != null && list[i].Word.Length > 0)
                    {
                        tokens.Add(new WordToken(i, list[i].Word));
                        sets.Add(list[i].Word);
                    }
                }

                List<string> liness = File.ReadAllLines("Dictionary\\SearchWords.txt").ToList().Where(it => it.Length > 0).ToList();
                foreach (string item in liness)
                {
                    if (sets.Contains(item) == false)
                    {
                        tokens.Add(new WordToken(tokens.Count + 1, item));
                        sets.Add(item);
                    }
                }
                for (int i = 1; i < 65536; i++)
                {
                    string item = ((char)i).ToString();
                    if (sets.Contains(item) == false)
                    {
                        tokens.Add(new WordToken(tokens.Count + 1, item));
                        sets.Add(item);
                    }
                }
                tokens.Sort();

                int[] codes = new int[tokens.Count];
                List<string> lines = new List<string>(tokens.Count);
                for (int i = 0; i < tokens.Count; i++)
                {
                    codes[i] = tokens[i].Code;
                    lines.Add(tokens[i].Word);
                }
                tire.Build(lines, codes);
                tire.Free();
                tire.Save("tire.bin");
            }
            return tire;
        }

        #endregion

        private static int SortAsc(string strA, string strB)
        {
            if (strA == null && strB == null)
                return 0;
            if (strA == null)
                return -1;
            if (strB == null)
                return 1;

            int len = 0;
            if (strA.Length >= strB.Length)
                len = strB.Length;
            else
                len = strA.Length;

            int result = 0;
            for (int i = 0; i < len; i++)
            {
                if (strA[i] < strB[i])
                {
                    result = -1;
                    break;
                }
                else if (strA[i] == strB[i])
                {
                    if (strA.Length < strB.Length)
                        result = -1;
                    else if (strA.Length == strB.Length)
                        result = 0;
                    else
                        result = 1;
                }
                else
                {
                    result = 1;
                    break;
                }
            }
            return result;
        }



    }
}
