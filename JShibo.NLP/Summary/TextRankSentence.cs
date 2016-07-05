using JShibo.NLP.Seg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Summary
{
    /**
 * TextRank 自动摘要
 *
 * @author hankcs
 */
    public class TextRankSentence
    {
        /**
         * 阻尼系数（ＤａｍｐｉｎｇＦａｃｔｏｒ），一般取值为0.85
         */
        const double d = 0.85;
        /**
         * 最大迭代次数
         */
        const int max_iter = 200;
        const double min_diff = 0.001;
        /**
         * 文档句子的个数
         */
        int D;
        /**
         * 拆分为[句子[单词]]形式的文档
         */
        List<List<String>> docs;
        /**
         * 排序后的最终结果 score <-> index
         */
        Dictionary<Double, int> top;

        /**
         * 句子和其他句子的相关程度
         */
        double[][] weight;
        /**
         * 该句子和其他句子相关程度之和
         */
        double[] weight_sum;
        /**
         * 迭代之后收敛的权重
         */
        double[] vertex;

        /**
         * BM25相似度
         */
        BM25 bm25;

        public TextRankSentence(List<List<String>> docs)
        {
            this.docs = docs;
            bm25 = new BM25(docs);
            D = docs.Count;
            weight = new double[D][D];
            weight_sum = new double[D];
            vertex = new double[D];
            //top = new Dictionary<Double, int>(Collections.reverseOrder());
            solve();
        }

        private void solve()
        {
            int cnt = 0;
            foreach (List<String> sentence in docs)
            {
                double[] scores = bm25.simAll(sentence);
                //            System.out.println(Arrays.toString(scores));
                weight[cnt] = scores;
                weight_sum[cnt] = sum(scores) - scores[cnt]; // 减掉自己，自己跟自己肯定最相似
                vertex[cnt] = 1.0;
                ++cnt;
            }
            for (int _ = 0; _ < max_iter; ++_)
            {
                double[] m = new double[D];
                double max_diff = 0;
                for (int i = 0; i < D; ++i)
                {
                    m[i] = 1 - d;
                    for (int j = 0; j < D; ++j)
                    {
                        if (j == i || weight_sum[j] == 0) continue;
                        m[i] += (d * weight[j][i] / weight_sum[j] * vertex[j]);
                    }
                    double diff = Math.Abs(m[i] - vertex[i]);
                    if (diff > max_diff)
                    {
                        max_diff = diff;
                    }
                }
                vertex = m;
                if (max_diff <= min_diff) break;
            }
            // 我们来排个序吧
            for (int i = 0; i < D; ++i)
            {
                top.Add(vertex[i], i);
            }
        }

        /**
         * 获取前几个关键句子
         *
         * @param size 要几个
         * @return 关键句子的下标
         */
        public int[] getTopSentence(int size)
        {
            //Collection<int> values = top.values();
            //size = Math.Min(size, values.size());
            //int[] indexArray = new int[size];
            //Iterator<Integer> it = values.iterator();
            //for (int i = 0; i < size; ++i)
            //{
            //    indexArray[i] = it.next();
            //}
            //return indexArray;
            return null;
        }

        /**
         * 简单的求和
         *
         * @param array
         * @return
         */
        private static double sum(double[] array)
        {
            double total = 0;
            foreach (double v in array)
            {
                total += v;
            }
            return total;
        }

        public static void main(String[] args)
        {
            String document = "算法可大致分为基本算法、数据结构的算法、数论算法、计算几何的算法、图的算法、动态规划以及数值分析、加密算法、排序算法、检索算法、随机化算法、并行算法、厄米变形模型、随机森林算法。\n" +
                    "算法可以宽泛的分为三类，\n" +
                    "一，有限的确定性算法，这类算法在有限的一段时间内终止。他们可能要花很长时间来执行指定的任务，但仍将在一定的时间内终止。这类算法得出的结果常取决于输入值。\n" +
                    "二，有限的非确定算法，这类算法在有限的时间内终止。然而，对于一个（或一些）给定的数值，算法的结果并不是唯一的或确定的。\n" +
                    "三，无限的算法，是那些由于没有定义终止定义条件，或定义的条件无法由输入的数据满足而不终止运行的算法。通常，无限算法的产生是由于未能确定的定义终止条件。";
            //System.out.println(TextRankSentence.getTopSentenceList(document, 3));
        }

        /**
         * 将文章分割为句子
         *
         * @param document
         * @return
         */
        static List<String> spiltSentence(String document)
        {
            List<String> sentences = new List<String>();
            foreach (String line in document.Split("[\r\n]".ToCharArray()))
            {
                string swap = line.Trim();
                if (swap.Length == 0) continue;
                foreach (String sent in line.Split("[，,。:：“”？?！!；;]".ToCharArray()))
                {
                    string swap1 = sent.Trim();
                    if (swap1.Length == 0) continue;
                    sentences.Add(sent);
                }
            }

            return sentences;
        }

        /**
         * 将句子列表转化为文档
         *
         * @param sentenceList
         * @return
         */
        private static List<List<String>> convertSentenceListToDocument(List<String> sentenceList)
        {
            List<List<String>> docs = new List<List<String>>(sentenceList.Count);
            foreach (String sentence in sentenceList)
            {
                List<Term> termList = StandardTokenizer.segment(sentence.ToCharArray());
                List<String> wordList = new List<String>();
                foreach (Term term in termList)
                {
                    if (CoreStopWordDictionary.shouldInclude(term))
                    {
                        wordList.Add(term.word);
                    }
                }
                docs.Add(wordList);
            }
            return docs;
        }

        /**
         * 一句话调用接口
         *
         * @param document 目标文档
         * @param size     需要的关键句的个数
         * @return 关键句列表
         */
        public static List<String> getTopSentenceList(String document, int size)
        {
            List<String> sentenceList = spiltSentence(document);
            List<List<String>> docs = convertSentenceListToDocument(sentenceList);
            TextRankSentence textRank = new TextRankSentence(docs);
            int[] topSentence = textRank.getTopSentence(size);
            List<String> resultList = new List<String>();
            foreach (int i in topSentence)
            {
                resultList.Add(sentenceList[i]);
            }
            return resultList;
        }

        /**
         * 一句话调用接口
         *
         * @param document   目标文档
         * @param max_length 需要摘要的长度
         * @return 摘要文本
         */
        public static String getSummary(String document, int max_length)
        {
            List<String> sentenceList = spiltSentence(document);

            int sentence_count = sentenceList.Count;
            int document_length = document.Length;
            int sentence_length_avg = document_length / sentence_count;
            int size = max_length / sentence_length_avg + 1;
            List<List<String>> docs = convertSentenceListToDocument(sentenceList);
            TextRankSentence textRank = new TextRankSentence(docs);
            int[] topSentence = textRank.getTopSentence(size);
            List<String> resultList = new List<String>();
            foreach (int i in topSentence)
            {
                resultList.Add(sentenceList[i]);
            }

            resultList = permutation(resultList, sentenceList);
            resultList = pick_sentences(resultList, max_length);
            return TextUtility.join("。", resultList);
        }

        public static List<String> permutation(List<String> resultList, List<String> sentenceList)
        {
            int index_buffer_x;
            int index_buffer_y;
            String sen_x;
            String sen_y;
            int length = resultList.Count;
            // bubble sort derivative
            for (int i = 0; i < length; i++)
                for (int offset = 0; offset < length - i; offset++)
                {
                    sen_x = resultList[i];
                    sen_y = resultList[i + offset];
                    index_buffer_x = sentenceList.IndexOf(sen_x);
                    index_buffer_y = sentenceList.IndexOf(sen_y);
                    // if the sentence order in sentenceList does not conform that is in resultList, reverse it
                    if (index_buffer_x > index_buffer_y)
                    {
                        resultList[i]= sen_y;
                        resultList[i + offset]= sen_x;
                    }
                }

            return resultList;
        }

        public static List<String> pick_sentences(List<String> resultList, int max_length)
        {
            int length_counter = 0;
            int length_buffer;
            int length_jump;
            List<String> resultBuffer = new List<String>();
            for (int i = 0; i < resultList.Count; i++)
            {
                length_buffer = length_counter + resultList[i].Length;
                if (length_buffer <= max_length)
                {
                    resultBuffer.Add(resultList[i]);
                    length_counter += resultList[i].Length;
                }
                else if (i < (resultList.Count - 1))
                {
                    length_jump = length_counter + resultList[i + 1].Length;
                    if (length_jump <= max_length)
                    {
                        resultBuffer.Add(resultList[i + 1]);
                        length_counter += resultList[i + 1].Length;
                        i++;
                    }
                }
            }
            return resultBuffer;
        }

    }

}
