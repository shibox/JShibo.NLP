using JShibo.NLP.Collection.Trie;
using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Dictionary;
using JShibo.NLP.Dictionary.Other;
using JShibo.NLP.Seg.Common;
using JShibo.NLP.Seg.NShort.Path;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algoritm;

namespace JShibo.NLP.Seg
{
    /**
 * 基于词语NGram模型的分词器基类
 *
 * @author hankcs
 */
    public abstract class WordBasedGenerativeModelSegment : Segment
    {

    public WordBasedGenerativeModelSegment()
            :base()
    {
        
    }

    /**
     * 对粗分结果执行一些规则上的合并拆分等等，同时合成新词网
     *
     * @param linkedArray    粗分结果
     * @param wordNetOptimum 合并了所有粗分结果的词网
     */
    protected static void GenerateWord(List<Vertex> linkedArray, WordNet wordNetOptimum)
    {
        fixResultByRule(linkedArray);

        //--------------------------------------------------------------------
        // 建造新词网
        wordNetOptimum.addAll(linkedArray);
    }

    /**
     * 通过规则修正一些结果
     *
     * @param linkedArray
     */
    protected static void fixResultByRule(List<Vertex> linkedArray)
    {

        //--------------------------------------------------------------------
        //Merge all seperate continue num into one number
        mergeContinueNumIntoOne(linkedArray);

        //--------------------------------------------------------------------
        //The delimiter "－－"
        ChangeDelimiterPOS(linkedArray);

        //--------------------------------------------------------------------
        //如果前一个词是数字，当前词以“－”或“-”开始，并且不止这一个字符，
        //那么将此“－”符号从当前词中分离出来。
        //例如 “3 / -4 / 月”需要拆分成“3 / - / 4 / 月”
        SplitMiddleSlashFromDigitalWords(linkedArray);

        //--------------------------------------------------------------------
        //1、如果当前词是数字，下一个词是“月、日、时、分、秒、月份”中的一个，则合并,且当前词词性是时间
        //2、如果当前词是可以作为年份的数字，下一个词是“年”，则合并，词性为时间，否则为数字。
        //3、如果最后一个汉字是"点" ，则认为当前数字是时间
        //4、如果当前串最后一个汉字不是"∶·．／"和半角的'.''/'，那么是数
        //5、当前串最后一个汉字是"∶·．／"和半角的'.''/'，且长度大于1，那么去掉最后一个字符。例如"1."
        CheckDateElements(linkedArray);

    }

    static void ChangeDelimiterPOS(List<Vertex> linkedArray)
    {
        foreach (Vertex vertex in linkedArray)
        {
            if (vertex.realWord.Equals("－－") || vertex.realWord.Equals("—") || vertex.realWord.Equals("-"))
            {
                vertex.confirmNature(Nature.w);
            }
        }
    }

    //====================================================================
    //如果前一个词是数字，当前词以“－”或“-”开始，并且不止这一个字符，
    //那么将此“－”符号从当前词中分离出来。
    //例如 “3-4 / 月”需要拆分成“3 / - / 4 / 月”
    //====================================================================
    private static void SplitMiddleSlashFromDigitalWords(List<Vertex> linkedArray)
    {
        if (linkedArray.Count < 2)
            return;

        ListIterator<Vertex> listIterator = linkedArray.listIterator();
        Vertex next = listIterator.next();
        Vertex current = next;
        while (listIterator.hasNext())
        {
            next = listIterator.next();
            //            System.out.println("current:" + current + " next:" + next);
            Nature currentNature = current.getNature();
            if (currentNature == Nature.nx && (next.hasNature(Nature.q) || next.hasNature(Nature.n)))
            {
                String[] param = current.realWord.Split(new char[] { '-' }, 1);
                if (param.Length == 2)
                {
                    if (TextUtility.isAllNum(param[0]) && TextUtility.isAllNum(param[1]))
                    {
                        current = current.copy();
                        current.realWord = param[0];
                        current.confirmNature(Nature.m);
                        listIterator.previous();
                        listIterator.previous();
                        listIterator.set(current);
                        listIterator.next();
                        listIterator.add(Vertex.newPunctuationInstance("-"));
                        listIterator.add(Vertex.newNumberInstance(param[1]));
                    }
                }
            }
            current = next;
        }

        //        logger.trace("杠号识别后：" + Graph.parseResult(linkedArray));
    }

    //====================================================================
    //1、如果当前词是数字，下一个词是“月、日、时、分、秒、月份”中的一个，则合并且当前词词性是时间
    //2、如果当前词是可以作为年份的数字，下一个词是“年”，则合并，词性为时间，否则为数字。
    //3、如果最后一个汉字是"点" ，则认为当前数字是时间
    //4、如果当前串最后一个汉字不是"∶·．／"和半角的'.''/'，那么是数
    //5、当前串最后一个汉字是"∶·．／"和半角的'.''/'，且长度大于1，那么去掉最后一个字符。例如"1."
    //====================================================================
    private static void CheckDateElements(List<Vertex> linkedArray)
    {
        if (linkedArray.Count < 2)
            return;
            List<Vertex>.Enumerator listIterator = linkedArray.GetEnumerator();
            listIterator.MoveNext();
            Vertex next = listIterator.Current;
        Vertex current = next;
        while (listIterator.MoveNext())
        {
                next = listIterator.Current;
            if (TextUtility.isAllNum(current.realWord) || TextUtility.isAllChineseNum(current.realWord))
            {
                //===== 1、如果当前词是数字，下一个词是“月、日、时、分、秒、月份”中的一个，则合并且当前词词性是时间
                String nextWord = next.realWord;
                if ((nextWord.Length == 1 && "月日时分秒".Contains(nextWord)) || (nextWord.Length == 2 && nextWord.Equals("月份")))
                {
                    current = Vertex.newTimeInstance(current.realWord + next.realWord);
                    //listIterator.previous();
                    //listIterator.previous();
                    //listIterator.set(current);
                    //listIterator.next();
                    //listIterator.next();
                    //listIterator.remove();
                }
                //===== 2、如果当前词是可以作为年份的数字，下一个词是“年”，则合并，词性为时间，否则为数字。
                else if (nextWord.Equals("年"))
                {
                    if (TextUtility.isYearTime(current.realWord))
                    {
                        current = Vertex.newTimeInstance(current.realWord + next.realWord);
                        //listIterator.previous();
                        //listIterator.previous();
                        //listIterator.set(current);
                        //listIterator.next();
                        //listIterator.next();
                        //listIterator.remove();
                    }
                    //===== 否则当前词就是数字了 =====
                    else
                    {
                        current.confirmNature(Nature.m);
                    }
                }
                else
                {
                    //===== 3、如果最后一个汉字是"点" ，则认为当前数字是时间
                    if (current.realWord.EndsWith("点"))
                    {
                        current.confirmNature(Nature.t, true);
                    }
                    else
                    {
                        char[] tmpCharArray = current.realWord.ToCharArray();
                        String lastChar = tmpCharArray[tmpCharArray.Length - 1].ToString();
                        //===== 4、如果当前串最后一个汉字不是"∶·．／"和半角的'.''/'，那么是数
                        if (!"∶·．／./".Contains(lastChar))
                        {
                            current.confirmNature(Nature.m, true);
                        }
                        //===== 5、当前串最后一个汉字是"∶·．／"和半角的'.''/'，且长度大于1，那么去掉最后一个字符。例如"1."
                        else if (current.realWord.Length > 1)
                        {
                            char last = current.realWord[current.realWord.Length - 1];
                            current = Vertex.newNumberInstance(current.realWord.Substring(0, current.realWord.Length - 1));
                            //listIterator.previous();
                            //listIterator.previous();
                            //listIterator.set(current);
                            //listIterator.next();
                            //listIterator.add(Vertex.newPunctuationInstance(String.valueOf(last)));
                        }
                    }
                }
            }
            current = next;
        }
        //        logger.trace("日期识别后：" + Graph.parseResult(linkedArray));
    }

    /**
     * 将一条路径转为最终结果
     *
     * @param vertexList
     * @param offsetEnabled 是否计算offset
     * @return
     */
    protected static List<Term> convert(List<Vertex> vertexList, bool offsetEnabled)
    {
        //assert vertexList != null;
        //assert vertexList.size() >= 2 : "这条路径不应当短于2" + vertexList.toString();
        int length = vertexList.Count - 2;
        List<Term> resultList = new List<Term>(length);
            List<Vertex>.Enumerator iterator = vertexList.GetEnumerator();
            iterator.MoveNext();
        //iterator.next();
        if (offsetEnabled)
        {
            int offset = 0;
            for (int i = 0; i < length; ++i)
            {
                    iterator.MoveNext();
                    Vertex vertex = iterator.Current;
                Term term = convert(vertex);
                term.offset = offset;
                offset += term.length();
                resultList.Add(term);
            }
        }
        else
        {
            for (int i = 0; i < length; ++i)
            {
                    iterator.MoveNext();
                    Vertex vertex = iterator.Current;
                Term term = convert(vertex);
                resultList.Add(term);
            }
        }
        return resultList;
    }

    /**
     * 将一条路径转为最终结果
     *
     * @param vertexList
     * @return
     */
    protected static List<Term> convert(List<Vertex> vertexList)
    {
        return convert(vertexList, false);
    }

    /**
     * 生成二元词图
     *
     * @param wordNet
     * @return
     */
    protected static Graph GenerateBiGraph(WordNet wordNet)
    {
        return wordNet.toGraph();
    }

    /**
     * 原子分词
     *
     * @param sSentence
     * @param start
     * @param end
     * @return
     * @deprecated 应该使用字符数组的版本
     */
    private static List<AtomNode> AtomSegment(String sSentence, int start, int end)
    {
        if (end < start)
        {
            throw new Exception("start=" + start + " < end=" + end);
        }
        List<AtomNode> atomSegment = new List<AtomNode>();
        int pCur = 0, nCurType, nNextType;
        StringBuilder sb = new StringBuilder();
        char c;


        //==============================================================================================
        // by zhenyulu:
        //
        // TODO: 使用一系列正则表达式将句子中的完整成分（百分比、日期、电子邮件、URL等）预先提取出来
        //==============================================================================================

        char[] charArray = sSentence.Substring(start, end).ToCharArray();
        int[] charTypeArray = new int[charArray.Length];

        // 生成对应单个汉字的字符类型数组
        for (int i = 0; i < charArray.Length; ++i)
        {
            c = charArray[i];
            charTypeArray[i] = CharType.get(c);

            if (c == '.' && i < (charArray.Length - 1) && CharType.get(charArray[i + 1]) == Predefine.CT_NUM)
                charTypeArray[i] = Predefine.CT_NUM;
            else if (c == '.' && i < (charArray.Length - 1) && charArray[i + 1] >= '0' && charArray[i + 1] <= '9')
                charTypeArray[i] = Predefine.CT_SINGLE;
            else if (charTypeArray[i] == Predefine.CT_LETTER)
                charTypeArray[i] = Predefine.CT_SINGLE;
        }

        // 根据字符类型数组中的内容完成原子切割
        while (pCur < charArray.Length)
        {
            nCurType = charTypeArray[pCur];

            if (nCurType == Predefine.CT_CHINESE || nCurType == Predefine.CT_INDEX ||
                    nCurType == Predefine.CT_DELIMITER || nCurType == Predefine.CT_OTHER)
            {
                String single = charArray[pCur].ToString();
                if (single.Length != 0)
                    atomSegment.Add(new AtomNode(single, nCurType));
                pCur++;
            }
            //如果是字符、数字或者后面跟随了数字的小数点“.”则一直取下去。
            else if (pCur < charArray.Length - 1 && ((nCurType == Predefine.CT_SINGLE) || nCurType == Predefine.CT_NUM))
            {
                sb.Remove(0, sb.Length);
                sb.Append(charArray[pCur]);

                bool reachEnd = true;
                while (pCur < charArray.Length - 1)
                {
                    nNextType = charTypeArray[++pCur];

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

        //        logger.trace("原子分词:" + atomSegment);
        return atomSegment;
    }

    /**
     * 将连续的数字节点合并为一个
     *
     * @param linkedArray
     */
    private static void mergeContinueNumIntoOne(List<Vertex> linkedArray)
    {
        if (linkedArray.Count < 2)
            return;

            List<Vertex>.Enumerator listIterator = linkedArray.GetEnumerator();
            listIterator.MoveNext();
            Vertex next = listIterator.Current;
        Vertex current = next;
        while (listIterator.MoveNext())
        {
                next = listIterator.Current;
            //            System.out.println("current:" + current + " next:" + next);
            if ((TextUtility.isAllNum(current.realWord) || TextUtility.isAllChineseNum(current.realWord)) && (TextUtility.isAllNum(next.realWord) || TextUtility.isAllChineseNum(next.realWord)))
            {
                /////////// 这部分从逻辑上等同于current.realWord = current.realWord + next.realWord;
                // 但是current指针被几个路径共享，需要备份，不然修改了一处就修改了全局
                current = Vertex.newNumberInstance(current.realWord + next.realWord);
                //listIterator.previous();
                //listIterator.previous();
                //listIterator.set(current);
                //listIterator.next();
                //listIterator.next();
                /////////// end 这部分
                //                System.out.println("before:" + linkedArray);
                //listIterator.remove();
                //                System.out.println("after:" + linkedArray);
            }
            else
            {
                current = next;
            }
        }

        //        logger.trace("数字识别后：" + Graph.parseResult(linkedArray));
    }

    /**
     * 生成一元词网
     *
     * @param wordNetStorage
     */
    protected void GenerateWordNet(WordNet wordNetStorage)
    {
        char[] charArray = wordNetStorage.charArray;

        // 核心词典查询
        DoubleArrayTrie<CoreDictionary.Attribute>.Searcher searcher = CoreDictionary.trie.getSearcher(charArray, 0);
        while (searcher.next())
        {
            wordNetStorage.add(searcher.begin + 1, new Vertex(new String(charArray, searcher.begin, searcher.length), searcher.value, searcher.index));
        }
        // 用户词典查询
        //        if (config.useCustomDictionary)
        //        {
        //            searcher = CustomDictionary.dat.getSearcher(charArray, 0);
        //            while (searcher.next())
        //            {
        //                wordNetStorage.add(searcher.begin + 1, new Vertex(new String(charArray, searcher.begin, searcher.length), searcher.value));
        //            }
        //        }
        // 原子分词，保证图连通
        List<Vertex>[] vertexes = wordNetStorage.getVertexes();
        for (int i = 1; i < vertexes.Length;)
        {
            if (vertexes[i].Count == 0)
            {
                int j = i + 1;
                for (; j < vertexes.Length - 1; ++j)
                {
                    if (!(vertexes[j].Count == 0)) break;
                }
                wordNetStorage.add(i, quickAtomSegment(charArray, i - 1, j - 1));
                i = j;
            }
            else i += vertexes[i][vertexes[i].Count-1].realWord.Length;
        }
    }

    /**
     * 为了索引模式修饰结果
     *
     * @param vertexList
     * @param wordNetAll
     */
    protected static List<Term> decorateResultForIndexMode(List<Vertex> vertexList, WordNet wordNetAll)
    {
        List<Term> termList = new List<Term>();
        int line = 1;
            List<Vertex>.Enumerator listIterator = vertexList.GetEnumerator();
            listIterator.MoveNext();
        int length = vertexList.Count - 2;
        for (int i = 0; i < length; ++i)
        {
                listIterator.MoveNext();
                       Vertex vertex = listIterator.Current;
            Term termMain = convert(vertex);
            termList.Add(termMain);
            termMain.offset = line - 1;
            if (vertex.realWord.Length > 2)
            {
                // 过长词所在的行
                int currentLine = line;
                while (currentLine < line + vertex.realWord.Length)
                {
                    List<Vertex> vertexListCurrentLine = wordNetAll.get(currentLine);    // 这一行的词
                    foreach (Vertex smallVertex in vertexListCurrentLine) // 这一行的短词
                    {
                        if (
                                ((termMain.nature == Nature.mq && smallVertex.hasNature(Nature.q)) ||
                                        smallVertex.realWord.Length > 1)
                                        && smallVertex != vertex)
                        {
                            //listIterator.Add(smallVertex);
                            Term termSub = convert(smallVertex);
                            termSub.offset = currentLine - 1;
                            termList.Add(termSub);
                        }
                    }
                    ++currentLine;
                }
            }
                line += vertex.realWord.Length;
        }

        return termList;
    }

    /**
     * 将节点转为term
     *
     * @param vertex
     * @return
     */
    private static Term convert(Vertex vertex)
    {
        return new Term(vertex.realWord, vertex.guessNature());
    }

    /**
     * 词性标注
     *
     * @param vertexList
     */
    protected static void speechTagging(List<Vertex> vertexList)
    {
            Algoritm.Viterbi.compute(vertexList, CoreDictionaryTransformMatrixDictionary.transformMatrixDictionary);
    }

    //    protected static void performNamedEntityRecognize(List<Vertex> vertexList, WordNet wordNetOptimum, WordNet wordNetAll)
    //    {
    //    }
}
}
