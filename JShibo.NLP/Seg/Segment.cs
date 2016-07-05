using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Dictionary.Other;
using JShibo.NLP.Seg.Common;
using JShibo.NLP.Seg.NShort.Path;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Seg
{
    public abstract class Segment
    {
        /**
         * 分词器配置
         */
        protected Config config;

        /**
         * 构造一个分词器
         */
        public Segment()
        {
            config = new Config();
        }

        /**
         * 原子分词
         *
         * @param charArray
         * @param start     从start开始（包含）
         * @param end       到end结束（不包含end）
         * @return 一个列表，代表从start到from的所有字构成的原子节点
         */
        protected static List<AtomNode> atomSegment(char[] charArray, int start, int end)
        {
            
            List<AtomNode> atomSegment = new List<AtomNode>();
            int pCur = start, nCurType, nNextType;
            StringBuilder sb = new StringBuilder();
            char c;

            int[] charTypeArray = new int[end - start];

            // 生成对应单个汉字的字符类型数组
            for (int i = 0; i < charTypeArray.Length; ++i)
            {
                c = charArray[i + start];
                charTypeArray[i] = CharType.get(c);

                if (c == '.' && i + start < (charArray.Length - 1) && CharType.get(charArray[i + start + 1]) == CharType.CT_NUM)
                    charTypeArray[i] = CharType.CT_NUM;
                else if (c == '.' && i + start < (charArray.Length - 1) && charArray[i + start + 1] >= '0' && charArray[i + start + 1] <= '9')
                    charTypeArray[i] = CharType.CT_SINGLE;
                else if (charTypeArray[i] == CharType.CT_LETTER)
                    charTypeArray[i] = CharType.CT_SINGLE;
            }

            // 根据字符类型数组中的内容完成原子切割
            while (pCur < end)
            {
                nCurType = charTypeArray[pCur - start];

                if (nCurType == CharType.CT_CHINESE || nCurType == CharType.CT_INDEX ||
                        nCurType == CharType.CT_DELIMITER || nCurType == CharType.CT_OTHER)
                {
                    String single = charArray[pCur].ToString();
                    if (single.Length != 0)
                        atomSegment.Add(new AtomNode(single, nCurType));
                    pCur++;
                }
                //如果是字符、数字或者后面跟随了数字的小数点“.”则一直取下去。
                else if (pCur < end - 1 && ((nCurType == CharType.CT_SINGLE) || nCurType == CharType.CT_NUM))
                {
                    sb.Remove(0, sb.Length);
                    sb.Append(charArray[pCur]);

                    bool reachEnd = true;
                    while (pCur < end - 1)
                    {
                        nNextType = charTypeArray[++pCur - start];

                        if (nNextType == nCurType)
                            sb.Append(charArray[pCur]);
                        else
                        {
                            reachEnd = false;
                            break;
                        }
                    }
                    atomSegment.Add(new AtomNode(sb.ToString(), nCurType));
                    if (reachEnd)
                        pCur++;
                }
                // 对于所有其它情况
                else
                {
                    atomSegment.Add(new AtomNode(charArray[pCur], nCurType));
                    pCur++;
                }
            }

            return atomSegment;
        }

        /**
         * 简易原子分词，将所有字放到一起作为一个词
         *
         * @param charArray
         * @param start
         * @param end
         * @return
         */
        protected static List<AtomNode> simpleAtomSegment(char[] charArray, int start, int end)
        {
            List<AtomNode> atomNodeList = new List<AtomNode>();
            atomNodeList.Add(new AtomNode(new String(charArray, start, end - start), Predefine.CT_LETTER));
            return atomNodeList;
        }

        /**s
         * 快速原子分词，希望用这个方法替换掉原来缓慢的方法
         *
         * @param charArray
         * @param start
         * @param end
         * @return
         */
        protected static List<AtomNode> quickAtomSegment(char[] charArray, int start, int end)
        {
            List<AtomNode> atomNodeList = new List<AtomNode>();
            int offsetAtom = start;
            int preType = CharType.get(charArray[offsetAtom]);
            int curType;
            while (++offsetAtom < end)
            {
                curType = CharType.get(charArray[offsetAtom]);
                if (curType != preType)
                {
                    // 浮点数识别
                    if (charArray[offsetAtom] == '.' && preType == CharType.CT_NUM)
                    {
                        while (++offsetAtom < end)
                        {
                            curType = CharType.get(charArray[offsetAtom]);
                            if (curType != CharType.CT_NUM) break;
                        }
                    }
                    atomNodeList.Add(new AtomNode(new String(charArray, start, offsetAtom - start), preType));
                    start = offsetAtom;
                }
                preType = curType;
            }
            if (offsetAtom == end)
                atomNodeList.Add(new AtomNode(new String(charArray, start, offsetAtom - start), preType));

            return atomNodeList;
        }

        /**
         * 使用用户词典合并粗分结果
         * @param vertexList 粗分结果
         * @return 合并后的结果
         */
        protected static List<Vertex> combineByCustomDictionary(List<Vertex> vertexList)
        {
            //Vertex[] wordNet = new Vertex[vertexList.Count];
            //vertexList.ToArray(wordNet);
            Vertex[] wordNet = vertexList.ToArray();
            // DAT合并
            DoubleArrayTrie<CoreDictionary.Attribute> dat = CustomDictionary.dat;
            for (int i = 0; i < wordNet.Length; ++i)
            {
                int state = 1;
                state = dat.transition(wordNet[i].realWord, state);
                if (state > 0)
                {
                    int start = i;
                    int to = i + 1;
                    int end = to;
                    CoreDictionary.Attribute value = dat.output(state);
                    for (; to < wordNet.Length; ++to)
                    {
                        state = dat.transition(wordNet[to].realWord, state);
                        if (state < 0) break;
                        CoreDictionary.Attribute output = dat.output(state);
                        if (output != null)
                        {
                            value = output;
                            end = to + 1;
                        }
                    }
                    if (value != null)
                    {
                        StringBuilder sbTerm = new StringBuilder();
                        for (int j = start; j < end; ++j)
                        {
                            sbTerm.Append(wordNet[j]);
                            wordNet[j] = null;
                        }
                        wordNet[i] = new Vertex(sbTerm.ToString(), value);
                        i = end - 1;
                    }
                }
            }
            // BinTrie合并
            if (CustomDictionary.trie != null)
            {
                for (int i = 0; i < wordNet.Length; ++i)
                {
                    if (wordNet[i] == null) continue;
                    BaseNode<CoreDictionary.Attribute> state = CustomDictionary.trie.transition(wordNet[i].realWord.ToCharArray(), 0);
                    if (state != null)
                    {
                        int start = i;
                        int to = i + 1;
                        int end = to;
                        CoreDictionary.Attribute value = state.getValue();
                        for (; to < wordNet.Length; ++to)
                        {
                            if (wordNet[to] == null) continue;
                            state = state.transition(wordNet[to].realWord.ToCharArray(), 0);
                            if (state == null) break;
                            if (state.getValue() != null)
                            {
                                value = state.getValue();
                                end = to + 1;
                            }
                        }
                        if (value != null)
                        {
                            StringBuilder sbTerm = new StringBuilder();
                            for (int j = start; j < end; ++j)
                            {
                                if (wordNet[j] == null) continue;
                                sbTerm.Append(wordNet[j]);
                                wordNet[j] = null;
                            }
                            wordNet[i] = new Vertex(sbTerm.ToString(), value);
                            i = end - 1;
                        }
                    }
                }
            }
            vertexList.Clear();
            foreach (Vertex vertex in wordNet)
            {
                if (vertex != null) vertexList.Add(vertex);
            }
            return vertexList;
        }

        /**
         * 合并数字
         * @param termList
         */
        protected void mergeNumberQuantifier(List<Vertex> termList, WordNet wordNetAll, Config config)
        {
            if (termList.Count < 4) return;
            StringBuilder sbQuantifier = new StringBuilder();
            ListIterator<Vertex> iterator = termList.listIterator();
            iterator.next();
            int line = 1;
            while (iterator.hasNext())
            {
                Vertex pre = iterator.next();
                if (pre.hasNature(Nature.m))
                {
                    sbQuantifier.Append(pre.realWord);
                    Vertex cur = null;
                    while (iterator.hasNext() && (cur = iterator.next()).hasNature(Nature.m))
                    {
                        sbQuantifier.Append(cur.realWord);
                        iterator.remove();
                        removeFromWordNet(cur, wordNetAll, line, sbQuantifier.Length);
                    }
                    if (cur != null)
                    {
                        if ((cur.hasNature(Nature.q) || cur.hasNature(Nature.qv) || cur.hasNature(Nature.qt)))
                        {
                            if (config.indexMode)
                            {
                                wordNetAll.add(line, new Vertex(sbQuantifier.ToString(), new CoreDictionary.Attribute(Nature.m)));
                            }
                            sbQuantifier.Append(cur.realWord);
                            iterator.remove();
                            removeFromWordNet(cur, wordNetAll, line, sbQuantifier.Length);
                        }
                        else
                        {
                            line += cur.realWord.Length;   // (cur = iterator.next()).hasNature(Nature.m) 最后一个next可能不含q词性
                        }
                    }
                    if (sbQuantifier.Length != pre.realWord.Length)
                    {
                        pre.realWord = sbQuantifier.ToString();
                        pre.word = Predefine.TAG_NUMBER;
                        pre.attribute = new CoreDictionary.Attribute(Nature.mq);
                        pre.wordID = CoreDictionary.M_WORD_ID;
                        sbQuantifier.Length = 0;
                    }
                }
                sbQuantifier.Length = 0;
                line += pre.realWord.Length;
            }
            //        System.out.println(wordNetAll);
        }

        /**
         * 将一个词语从词网中彻底抹除
         * @param cur 词语
         * @param wordNetAll 词网
         * @param line 当前扫描的行数
         * @param length 当前缓冲区的长度
         */
        private static void removeFromWordNet(Vertex cur, WordNet wordNetAll, int line, int length)
        {
            LinkedList<Vertex>[] vertexes = wordNetAll.getVertexes();
            // 将其从wordNet中删除
            foreach (Vertex vertex in vertexes[line + length])
            {
                if (vertex.from == cur)
                    vertex.from = null;
            }
            ListIterator<Vertex> iterator = vertexes[line + length - cur.realWord.Length].listIterator();
            while (iterator.hasNext())
            {
                Vertex vertex = iterator.next();
                if (vertex == cur) iterator.remove();
            }
        }

        /**
         * 分词<br>
         * 此方法是线程安全的
         *
         * @param text 待分词文本
         * @return 单词列表
         */
        public List<Term> seg(String text)
        {
            char[] charArray = text.ToCharArray();
            if (HanLP.Config.Normalization)
            {
                CharTable.normalization(charArray);
            }
            if (config.threadNumber > 1 && charArray.length > 10000)    // 小文本多线程没意义，反而变慢了
            {
                List<String> sentenceList = SentencesUtil.toSentenceList(charArray);
                String[] sentenceArray = new String[sentenceList.size()];
                sentenceList.toArray(sentenceArray);
                //noinspection unchecked
                List<Term>[] termListArray = new List<Term>(sentenceArray.Length);
                final int per = sentenceArray.Length / config.threadNumber;
                WorkThread[] threadArray = new WorkThread[config.threadNumber];
                for (int i = 0; i < config.threadNumber - 1; ++i)
                {
                    int from = i * per;
                    threadArray[i] = new WorkThread(sentenceArray, termListArray, from, from + per);
                    threadArray[i].start();
                }
                threadArray[config.threadNumber - 1] = new WorkThread(sentenceArray, termListArray, (config.threadNumber - 1) * per, sentenceArray.length);
                threadArray[config.threadNumber - 1].start();
                try
                {
                    foreach (WorkThread thread in threadArray)
                    {
                        thread.join();
                    }
                }
                catch (InterruptedException e)
                {
                    logger.severe("线程同步异常：" + TextUtility.exceptionToString(e));
                    return Collections.emptyList();
                }
                List<Term> termList = new LinkedList<Term>();
                if (config.offset || config.indexMode)  // 由于分割了句子，所以需要重新校正offset
                {
                    int sentenceOffset = 0;
                    for (int i = 0; i < sentenceArray.length; ++i)
                    {
                        for (Term term : termListArray[i])
                        {
                            term.offset += sentenceOffset;
                            termList.add(term);
                        }
                        sentenceOffset += sentenceArray[i].length();
                    }
                }
                else
                {
                    for (List<Term> list : termListArray)
                    {
                        termList.addAll(list);
                    }
                }

                return termList;
            }
            //        if (text.length() > 10000)  // 针对大文本，先拆成句子，后分词，避免内存峰值太大
            //        {
            //            List<Term> termList = new LinkedList<Term>();
            //            if (config.offset || config.indexMode)
            //            {
            //                int sentenceOffset = 0;
            //                for (String sentence : SentencesUtil.toSentenceList(charArray))
            //                {
            //                    List<Term> termOfSentence = segSentence(sentence.toCharArray());
            //                    for (Term term : termOfSentence)
            //                    {
            //                        term.offset += sentenceOffset;
            //                        termList.add(term);
            //                    }
            //                    sentenceOffset += sentence.length();
            //                }
            //            }
            //            else
            //            {
            //                for (String sentence : SentencesUtil.toSentenceList(charArray))
            //                {
            //                    termList.addAll(segSentence(sentence.toCharArray()));
            //                }
            //            }
            //
            //            return termList;
            //        }
            return segSentence(charArray);
        }

        /**
         * 分词
         *
         * @param text 待分词文本
         * @return 单词列表
         */
        public List<Term> seg(char[] text)
        {
            assert text != null;
            if (HanLP.Config.Normalization)
            {
                CharTable.normalization(text);
            }
            return segSentence(text);
        }

        /**
         * 分词断句 输出句子形式
         *
         * @param text 待分词句子
         * @return 句子列表，每个句子由一个单词列表组成
         */
        public List<List<Term>> seg2sentence(String text)
        {
            List<List<Term>> resultList = new LinkedList<List<Term>>();
            {
                for (String sentence : SentencesUtil.toSentenceList(text))
                {
                    resultList.add(segSentence(sentence.toCharArray()));
                }
            }

            return resultList;
        }

        /**
         * 给一个句子分词
         *
         * @param sentence 待分词句子
         * @return 单词列表
         */
        protected abstract List<Term> segSentence(char[] sentence);

        /**
         * 设为索引模式
         *
         * @return
         */
        public Segment enableIndexMode(boolean enable)
        {
            config.indexMode = enable;
            return this;
        }

        /**
         * 开启词性标注
         *
         * @param enable
         * @return
         */
        public Segment enablePartOfSpeechTagging(boolean enable)
        {
            config.speechTagging = enable;
            return this;
        }

        /**
         * 开启人名识别
         *
         * @param enable
         * @return
         */
        public Segment enableNameRecognize(boolean enable)
        {
            config.nameRecognize = enable;
            config.updateNerConfig();
            return this;
        }

        /**
         * 开启地名识别
         *
         * @param enable
         * @return
         */
        public Segment enablePlaceRecognize(boolean enable)
        {
            config.placeRecognize = enable;
            config.updateNerConfig();
            return this;
        }

        /**
         * 开启机构名识别
         *
         * @param enable
         * @return
         */
        public Segment enableOrganizationRecognize(boolean enable)
        {
            config.organizationRecognize = enable;
            config.updateNerConfig();
            return this;
        }

        /**
         * 是否启用用户词典
         *
         * @param enable
         */
        public Segment enableCustomDictionary(boolean enable)
        {
            config.useCustomDictionary = enable;
            return this;
        }

        /**
         * 是否启用音译人名识别
         *
         * @param enable
         */
        public Segment enableTranslatedNameRecognize(boolean enable)
        {
            config.translatedNameRecognize = enable;
            config.updateNerConfig();
            return this;
        }

        /**
         * 是否启用日本人名识别
         *
         * @param enable
         */
        public Segment enableJapaneseNameRecognize(boolean enable)
        {
            config.japaneseNameRecognize = enable;
            config.updateNerConfig();
            return this;
        }

        /**
         * 是否启用偏移量计算（开启后Term.offset才会被计算）
         *
         * @param enable
         * @return
         */
        public Segment enableOffset(boolean enable)
        {
            config.offset = enable;
            return this;
        }

        /**
         * 是否启用数词和数量词识别<br>
         *     即[二, 十, 一] => [二十一]，[十, 九, 元] => [十九元]
         * @param enable
         * @return
         */
        public Segment enableNumberQuantifierRecognize(boolean enable)
        {
            config.numberQuantifierRecognize = enable;
            return this;
        }

        /**
         * 是否启用所有的命名实体识别
         *
         * @param enable
         * @return
         */
        public Segment enableAllNamedEntityRecognize(boolean enable)
        {
            config.nameRecognize = enable;
            config.japaneseNameRecognize = enable;
            config.translatedNameRecognize = enable;
            config.placeRecognize = enable;
            config.organizationRecognize = enable;
            config.updateNerConfig();
            return this;
        }

        class WorkThread extends Thread
        {
            String []
            sentenceArray;
            List<Term>[]
            termListArray;
        int from;
        int to;

        public WorkThread(String[] sentenceArray, List<Term>[] termListArray, int from, int to)
        {
            this.sentenceArray = sentenceArray;
            this.termListArray = termListArray;
            this.from = from;
            this.to = to;
        }

        @Override
        public void run()
        {
            for (int i = from; i < to; ++i)
            {
                termListArray[i] = segSentence(sentenceArray[i].toCharArray());
            }
        }
    }

    /**
     * 开启多线程
     * @param enable true表示开启4个线程，false表示单线程
     * @return
     */
    public Segment enableMultithreading(boolean enable)
    {
        if (enable) config.threadNumber = 4;
        else config.threadNumber = 1;
        return this;
    }

    /**
     * 开启多线程
     * @param threadNumber 线程数量
     * @return
     */
    public Segment enableMultithreading(int threadNumber)
    {
        config.threadNumber = threadNumber;
        return this;
    }
}

