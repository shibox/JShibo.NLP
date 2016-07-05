using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP
{
    public struct WordToken : IComparable<WordToken>
    {
        public int Code;
        public string Word;

        public WordToken(int code, string word)
        {
            this.Code = code;
            this.Word = word;
        }

        public int CompareTo(WordToken other)
        {
            string strA = this.Word;
            string strB = other.Word;

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
            //int len = 0;
            //if (strA.Length >= strB.Length)
            //    len = strB.Length;
            //else
            //    len = strA.Length;

            //for (int i = 0; i < len; i++)
            //{
            //    if (strA[i] == strB[i])
            //    {
            //        if (i == len)
            //        {
            //            if (strA.Length < strB.Length)
            //                return -strB[len];
            //            else
            //                return strA[len];
            //        }
            //    }
            //    else
            //        return strA[i] - strB[i];
            //}
            //return 0;
        }
    }

    public class WordTokenResult : IComparable<WordTokenResult>
    {
        public int Code;
        public string Word;
        public int Count;

        public WordTokenResult()
        {

        }

        public WordTokenResult(int code, string word)
        {
            this.Code = code;
            this.Word = word;
        }

        public int CompareTo(WordTokenResult other)
        {
            return other.Count.CompareTo(this.Count);
        }
    }
}
