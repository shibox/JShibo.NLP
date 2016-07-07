using JShibo.NLP.Algoritm;
using JShibo.NLP.Corpus.Dictionary.Item;
using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Dictionary.NS;
using JShibo.NLP.Seg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Recognition.NS
{
    /**
 * 地址识别
 * @author hankcs
 */
    public class PlaceRecognition
    {
        public static bool Recognition(LinkedList<Vertex> pWordSegResult, WordNet wordNetOptimum, WordNet wordNetAll)
        {
            LinkedList<EnumItem<Corpus.Tag.NS>> roleTagList = roleTag(pWordSegResult, wordNetAll);
            if (HanLP.Config.DEBUG)
            {
                StringBuilder sbLog = new StringBuilder();
                //Iterator<Vertex> iterator = pWordSegResult.iterator();
                foreach (EnumItem<Corpus.Tag.NS> NSEnumItem in roleTagList)
                {
                    sbLog.Append('[');
                    //sbLog.Append(iterator.next().realWord);
                    sbLog.Append(' ');
                    sbLog.Append(NSEnumItem);
                    sbLog.Append(']');
                }
                Console.WriteLine("地名角色观察：%s\n", sbLog.ToString());
            }
            List<Corpus.Tag.NS> NSList = viterbiExCompute(roleTagList);
            if (HanLP.Config.DEBUG)
            {
                StringBuilder sbLog = new StringBuilder();
                //Iterator<Vertex> iterator = pWordSegResult.iterator();
                sbLog.Append('[');
                foreach (Corpus.Tag.NS NS in NSList)
                {
                    //sbLog.Append(iterator.next().realWord);
                    sbLog.Append('/');
                    sbLog.Append(NS);
                    sbLog.Append(" ,");
                }
                if (sbLog.Length > 1) sbLog.Remove(sbLog.Length - 2, sbLog.Length);
                sbLog.Append(']');
                Console.WriteLine("地名角色标注：%s\n", sbLog.ToString());
            }

            PlaceDictionary.parsePattern(NSList, pWordSegResult, wordNetOptimum, wordNetAll);
            return true;
        }

        public static LinkedList<EnumItem<Corpus.Tag.NS>> roleTag(LinkedList<Vertex> vertexList, WordNet wordNetAll)
        {
            LinkedList<EnumItem<Corpus.Tag.NS>> tagList = new LinkedList<EnumItem<Corpus.Tag.NS>>();
            LinkedList<Vertex>.Enumerator listIterator = vertexList.GetEnumerator();
            //        int line = 0;
            while (listIterator.MoveNext())
            {
                Vertex vertex = listIterator.Current;
                // 构成更长的
                //            if (Nature.ns == vertex.getNature() && vertex.getAttribute().totalFrequency <= 1000)
                //            {
                //                String value = vertex.realWord;
                //                int longestSuffixLength = PlaceSuffixDictionary.dictionary.getLongestSuffixLength(value);
                //                int wordLength = value.length() - longestSuffixLength;
                //                if (longestSuffixLength != 0 && wordLength != 0)
                //                {
                //                    listIterator.remove();
                //                    for (int l = 0, tag = NS.D.ordinal(); l < wordLength; ++l, ++tag)
                //                    {
                //                        listIterator.add(wordNetAll.getFirst(line + l));
                //                        tagList.add(new EnumItem<>(NS.values()[tag], 1000));
                //                    }
                //                    listIterator.add(wordNetAll.get(line + wordLength, longestSuffixLength));
                //                    tagList.add(new EnumItem<>(NS.H, 1000));
                //                    line += vertex.realWord.length();
                //                    continue;
                //                }
                //            }
                if (Nature.ns == vertex.getNature() && vertex.getAttribute().totalFrequency <= 1000)
                {
                    if (vertex.realWord.Length < 3)               // 二字地名，认为其可以再接一个后缀或前缀
                        tagList.AddLast(new EnumItem<Corpus.Tag.NS>(Corpus.Tag.NS.H, (int)Corpus.Tag.NS.G));
                    else
                        tagList.AddLast(new EnumItem<Corpus.Tag.NS>(Corpus.Tag.NS.G));        // 否则只可以再加后缀
                    continue;
                }
                EnumItem<Corpus.Tag.NS> NSEnumItem = PlaceDictionary.dictionary.get(vertex.word);  // 此处用等效词，更加精准
                if (NSEnumItem == null)
                {
                    NSEnumItem = new EnumItem<Corpus.Tag.NS>(Corpus.Tag.NS.Z, PlaceDictionary.transformMatrixDictionary.getTotalFrequency(Corpus.Tag.NS.Z));
                }
                tagList.AddLast(NSEnumItem);
                //            line += vertex.realWord.length();
            }
            return tagList;
        }

        private static void insert(List<Vertex> listIterator, List<EnumItem<Corpus.Tag.NS>> tagList, WordNet wordNetAll, int line, Corpus.Tag.NS ns)
        {
            Vertex vertex = wordNetAll.getFirst(line);
            //assert vertex != null : "全词网居然有空白行！";
            listIterator.Add(vertex);
            tagList.Add(new EnumItem<Corpus.Tag.NS>(ns, 1000));
        }

        /**
         * 维特比算法求解最优标签
         * @param roleTagList
         * @return
         */
        public static LinkedList<Corpus.Tag.NS> viterbiExCompute(LinkedList<EnumItem<Corpus.Tag.NS>> roleTagList)
        {
            return Viterbi.computeEnum(roleTagList, PlaceDictionary.transformMatrixDictionary);
        }
    }
}
