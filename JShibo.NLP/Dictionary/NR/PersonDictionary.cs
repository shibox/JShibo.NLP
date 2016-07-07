using JShibo.NLP.Collection.AhoCorasick;
using JShibo.NLP.Corpus.Dictionary.Item;
using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Seg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.NR
{
    /**
 * 人名识别用的词典，实际上是对两个词典的包装
 *
 * @author hankcs
 */
    public class PersonDictionary
    {
        /**
         * 人名词典
         */
        public static NRDictionary dictionary;
        /**
         * 转移矩阵词典
         */
        public static TransformMatrixDictionary<Corpus.Tag.NR> transformMatrixDictionary;
        /**
         * AC算法用到的Trie树
         */
        public static AhoCorasickDoubleArrayTrie<NRPattern> trie;

        public static CoreDictionary.Attribute ATTRIBUTE = new CoreDictionary.Attribute(Nature.nr, 100);

        static PersonDictionary()
        {
            //        long start = System.currentTimeMillis();
            //        dictionary = new NRDictionary();
            //        if (!dictionary.load(HanLP.Config.PersonDictionaryPath))
            //        {
            //            System.err.println("人名词典加载失败：" + HanLP.Config.PersonDictionaryPath);
            //            System.exit(-1);
            //        }
            //    transformMatrixDictionary = new TransformMatrixDictionary<NR>(NR.class);
            //        transformMatrixDictionary.load(HanLP.Config.PersonDictionaryTrPath);
            //        trie = new AhoCorasickDoubleArrayTrie<NRPattern>();
            //        TreeMap<String, NRPattern> map = new TreeMap<String, NRPattern>();
            //        for (NRPattern pattern : NRPattern.values())
            //        {
            //            map.put(pattern.toString(), pattern);
            //        }
            //trie.build(map);
            //        logger.info(HanLP.Config.PersonDictionaryPath + "加载成功，耗时" + (System.currentTimeMillis() - start) + "ms");
        }

        /**
         * 模式匹配
         *
         * @param nrList         确定的标注序列
         * @param vertexList     原始的未加角色标注的序列
         * @param wordNetOptimum 待优化的图
         * @param wordNetAll     全词图
         */
        public static void parsePattern(List<Corpus.Tag.NR> nrList, LinkedList<Vertex> vertexList, WordNet wordNetOptimum, WordNet wordNetAll)
        {
            //    // 拆分UV
            //    ListIterator<Vertex> listIterator = vertexList.listIterator();
            //    StringBuilder sbPattern = new StringBuilder(nrList.Count);
            //            Corpus.Tag.NR preNR = Corpus.Tag.NR.A;
            //    bool backUp = false;
            //    int index = 0;
            //    foreach (Corpus.Tag.NR nr in nrList)
            //    {
            //        ++index;
            //        Vertex current = listIterator.next();
            //        //            logger.trace("{}/{}", current.realWord, nr);
            //        switch (nr)
            //        {
            //            case Corpus.Tag.NR.U:
            //                if (!backUp)
            //                {
            //                    vertexList = new List<Vertex>(vertexList);
            //                    listIterator = vertexList.listIterator(index);
            //                    backUp = true;
            //                }
            //                sbPattern.Append(Corpus.Tag.NR.K.ToString());
            //                sbPattern.Append(Corpus.Tag.NR.B.ToString());
            //                preNR = Corpus.Tag.NR.B;
            //                listIterator.previous();
            //                String nowK = current.realWord.Substring(0, current.realWord.Length - 1);
            //                String nowB = current.realWord.Substring(current.realWord.Length - 1);
            //                listIterator.set(new Vertex(nowK));
            //                listIterator.next();
            //                listIterator.add(new Vertex(nowB));
            //                continue;
            //            case Corpus.Tag.NR.V:
            //                if (!backUp)
            //                {
            //                    vertexList = new List<Vertex>(vertexList);
            //                    listIterator = vertexList.listIterator(index);
            //                    backUp = true;
            //                }
            //                if (preNR == Corpus.Tag.NR.B)
            //                {
            //                    sbPattern.Append(Corpus.Tag.NR.E.ToString());  //BE
            //                }
            //                else
            //                {
            //                    sbPattern.Append(Corpus.Tag.NR.D.ToString());  //CD
            //                }
            //                sbPattern.Append(Corpus.Tag.NR.L.ToString());
            //                // 对串也做一些修改
            //                listIterator.previous();
            //                String nowED = current.realWord.Substring(current.realWord.Length - 1);
            //                String nowL = current.realWord.Substring(0, current.realWord.Length - 1);
            //                listIterator.set(new Vertex(nowED));
            //                listIterator.add(new Vertex(nowL));
            //                listIterator.next();
            //                continue;
            //            default:
            //                sbPattern.Append(nr.ToString());
            //                break;
            //        }
            //        preNR = nr;
            //    }
            //    String pattern = sbPattern.ToString();
            //    //        logger.trace("模式串：{}", pattern);
            //    //        logger.trace("对应串：{}", vertexList);
            //    //        if (pattern.length() != vertexList.size())
            //    //        {
            //    //            logger.warn("人名识别模式串有bug", pattern, vertexList);
            //    //            return;
            //    //        }
            //    Vertex[] wordArray = vertexList.toArray(new Vertex[0]);
            //    int[] offsetArray = new int[wordArray.Length];
            //    offsetArray[0] = 0;
            //    for (int i = 1; i < wordArray.Length; ++i)
            //    {
            //        offsetArray[i] = offsetArray[i - 1] + wordArray[i - 1].realWord.Length;
            //    }
            //    trie.parseText(pattern, new AhoCorasickDoubleArrayTrie.IHit<NRPattern>()
            //        {
            //            @Override
            //            public void hit(int begin, int end, NRPattern value)
            //{
            //    //            logger.trace("匹配到：{}", keyword);
            //    StringBuilder sbName = new StringBuilder();
            //    for (int i = begin; i < end; ++i)
            //    {
            //        sbName.Append(wordArray[i].realWord);
            //    }
            //    String name = sbName.ToString();
            //    //            logger.trace("识别出：{}", name);
            //    // 对一些bad case做出调整
            //    switch (value)
            //    {
            //        case NRPattern.BCD:
            //            if (name[0] == name[2]) return; // 姓和最后一个名不可能相等的
            //                                                          //                        String cd = name.substring(1);
            //                                                          //                        if (CoreDictionary.contains(cd))
            //                                                          //                        {
            //                                                          //                            EnumItem<NR> item = PersonDictionary.dictionary.get(cd);
            //                                                          //                            if (item == null || !item.containsLabel(Z)) return; // 三字名字但是后两个字不在词典中，有很大可能性是误命中
            //                                                          //                        }
            //            break;
            //    }
            //    if (isBadCase(name)) return;

            //    // 正式算它是一个名字
            //    if (HanLP.Config.DEBUG)
            //    {
            //        System.out.printf("识别出人名：%s %s\n", name, value);
            //    }
            //    int offset = offsetArray[begin];
            //    wordNetOptimum.insert(offset, new Vertex(Predefine.TAG_PEOPLE, name, ATTRIBUTE, WORD_ID), wordNetAll);
            //}
            //        });
        }

        /**
         * 因为任何算法都无法解决100%的问题，总是有一些bad case，这些bad case会以“盖公章 A 1”的形式加入词典中<BR>
         * 这个方法返回人名是否是bad case
         *
         * @param name
         * @return
         */
        static bool isBadCase(String name)
        {
            EnumItem<Corpus.Tag.NR> nrEnumItem = dictionary.get(name);
            if (nrEnumItem == null) return false;
            return nrEnumItem.containsLabel(Corpus.Tag.NR.A);
        }


    }
}
