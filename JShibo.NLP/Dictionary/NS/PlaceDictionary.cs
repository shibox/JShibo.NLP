using JShibo.NLP.Collection.AhoCorasick;
using JShibo.NLP.Corpus.Dictionary.Item;
using JShibo.NLP.Seg.Common;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.NS
{
    /**
 * 地名识别用的词典，实际上是对两个词典的包装
 *
 * @author hankcs
 */
    public class PlaceDictionary
    {
        /**
         * 地名词典
         */
        public static NSDictionary dictionary;
        /**
         * 转移矩阵词典
         */
        public static TransformMatrixDictionary<Corpus.Tag.NS> transformMatrixDictionary;
        /**
         * AC算法用到的Trie树
         */
        public static AhoCorasickDoubleArrayTrie<String> trie;

        /**
         * 本词典专注的词的ID
         */
        static int WORD_ID = CoreDictionary.getWordID(Predefine.TAG_PLACE);
        /**
         * 本词典专注的词的属性
         */
        static CoreDictionary.Attribute ATTRIBUTE = CoreDictionary.get(WORD_ID);

        static PlaceDictionary()
        {
            //long start = System.currentTimeMillis();
            //dictionary = new NSDictionary();
            //dictionary.load(HanLP.Config.PlaceDictionaryPath);
            //logger.info(HanLP.Config.PlaceDictionaryPath + "加载成功，耗时" + (System.currentTimeMillis() - start) + "ms");
            //transformMatrixDictionary = new TransformMatrixDictionary<NS>(NS.class);
            //transformMatrixDictionary.load(HanLP.Config.PlaceDictionaryTrPath);
            //trie = new AhoCorasickDoubleArrayTrie<String>();
            //TreeMap<String, String> patternMap = new TreeMap<String, String>();
            //patternMap.put("CH", "CH");
            //patternMap.put("CDH", "CDH");
            //patternMap.put("CDEH", "CDEH");
            //patternMap.put("GH", "GH");
            //trie.build(patternMap);
        }

        /**
         * 模式匹配
         *
         * @param nsList         确定的标注序列
         * @param vertexList     原始的未加角色标注的序列
         * @param wordNetOptimum 待优化的图
         * @param wordNetAll
         */
        public static void parsePattern(List<Corpus.Tag.NS> nsList, List<Vertex> vertexList, WordNet wordNetOptimum, WordNet wordNetAll)
        {
            //        ListIterator<Vertex> listIterator = vertexList.listIterator();
            StringBuilder sbPattern = new StringBuilder(nsList.Count);
            foreach (Corpus.Tag.NS ns in nsList)
            {
                sbPattern.Append(ns.ToString());
            }
            String pattern = sbPattern.ToString();
            //Vertex[] wordArray = vertexList.toArray(new Vertex[0]);
            //            Vertex[] wordArray = vertexList.ToArray();
            //            trie.parseText(pattern, new AhoCorasickDoubleArrayTrie.IHit<String>()
            //        {

            //            public void hit(int begin, int end, String value)
            //    {
            //        StringBuilder sbName = new StringBuilder();
            //        for (int i = begin; i < end; ++i)
            //        {
            //            sbName.Append(wordArray[i].realWord);
            //        }
            //        String name = sbName.ToString();
            //        // 对一些bad case做出调整
            //        if (isBadCase(name)) return;

            //        // 正式算它是一个名字
            //        if (HanLP.Config.DEBUG)
            //        {
            //            Console.WriteLine("识别出地名：%s %s\n", name, value);
            //        }
            //        int offset = 0;
            //        for (int i = 0; i < begin; ++i)
            //        {
            //            offset += wordArray[i].realWord.length();
            //        }
            //        wordNetOptimum.insert(offset, new Vertex(Predefine.TAG_PLACE, name, ATTRIBUTE, WORD_ID), wordNetAll);
            //    }
            //});
        }

        /**
         * 因为任何算法都无法解决100%的问题，总是有一些bad case，这些bad case会以“盖公章 A 1”的形式加入词典中<BR>
         * 这个方法返回是否是bad case
         *
         * @param name
         * @return
         */
        static bool isBadCase(String name)
        {
            EnumItem<Corpus.Tag.NS> nrEnumItem = dictionary.get(name);
            if (nrEnumItem == null) return false;
            return nrEnumItem.containsLabel(Corpus.Tag.NS.Z);
        }

    }
}
