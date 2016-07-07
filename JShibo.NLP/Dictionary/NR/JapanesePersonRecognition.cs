using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Dictionary;
using JShibo.NLP.Recognition.NR;
using JShibo.NLP.Seg.Common;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.NR
{
    /**
 * 日本人名识别
 *
 * @author hankcs
 */
    public class JapanesePersonRecognition
    {
        /**
         * 执行识别
         *
         * @param segResult      粗分结果
         * @param wordNetOptimum 粗分结果对应的词图
         * @param wordNetAll     全词图
         */
        public static void Recognition(LinkedList<Vertex> segResult, WordNet wordNetOptimum, WordNet wordNetAll)
        {
            StringBuilder sbName = new StringBuilder();
            int appendTimes = 0;
            char[] charArray = wordNetAll.charArray;
            BaseSearcher<KeyValuePair<String, char>> searcher = JapanesePersonDictionary.getSearcher(charArray);
            KeyValuePair<String, char> entry;
            int activeLine = 1;
            int preOffset = 0;
            //while ((entry = searcher.next()) != null)
            //{
            //    char label = entry.Value;
            //    String key = entry.Key;
            //    int offset = searcher.getOffset();
            //    if (preOffset != offset)
            //    {
            //        if (appendTimes > 1 && sbName.Length > 2) // 日本人名最短为3字
            //        {
            //            insertName(sbName.ToString(), activeLine, wordNetOptimum, wordNetAll);
            //        }
            //        sbName.Length = 0;
            //        appendTimes = 0;
            //    }
            //    if (appendTimes == 0)
            //    {
            //        if (label == JapanesePersonDictionary.X)
            //        {
            //            sbName.Append(key);
            //            ++appendTimes;
            //            activeLine = offset + 1;
            //        }
            //    }
            //    else
            //    {
            //        if (label == JapanesePersonDictionary.M)
            //        {
            //            sbName.Append(key);
            //            ++appendTimes;
            //        }
            //        else
            //        {
            //            if (appendTimes > 1 && sbName.Length > 2)
            //            {
            //                insertName(sbName.ToString(), activeLine, wordNetOptimum, wordNetAll);
            //            }
            //            sbName.Length = 0;
            //            appendTimes = 0;
            //        }
            //    }
            //    preOffset = offset + key.Length;
            //}
            //if (sbName.Length > 0)
            //{
            //    if (appendTimes > 1)
            //    {
            //        insertName(sbName.ToString(), activeLine, wordNetOptimum, wordNetAll);
            //    }
            //}
        }

        /**
         * 是否是bad case
         * @param name
         * @return
         */
        public static bool isBadCase(String name)
        {
            char label = JapanesePersonDictionary.get(name);
            if (label == null) return false;
            return label.Equals(JapanesePersonDictionary.A);
        }

        /**
         * 插入日本人名
         * @param name
         * @param activeLine
         * @param wordNetOptimum
         * @param wordNetAll
         */
        private static void insertName(String name, int activeLine, WordNet wordNetOptimum, WordNet wordNetAll)
        {
            if (isBadCase(name)) return;
            wordNetOptimum.insert(activeLine, new Vertex(Predefine.TAG_PEOPLE, name, new CoreDictionary.Attribute(Nature.nrj), NRConstant.WORD_ID), wordNetAll);
        }
    }
}
