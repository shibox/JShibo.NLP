using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Summary
{
    /**
 * 搜索相关性评分算法
 * @author hankcs
 */
    public class BM25
    {
        /**
         * 文档句子的个数
         */
        int D;

        /**
         * 文档句子的平均长度
         */
        double avgdl;

        /**
         * 拆分为[句子[单词]]形式的文档
         */
        List<List<String>> docs;

        /**
         * 文档中每个句子中的每个词与词频
         */
        Dictionary<String, int>[] f;

        /**
         * 文档中全部词语与出现在几个句子中
         */
        Dictionary<String, int> df;

        /**
         * IDF
         */
        Dictionary<String, Double> idf;

        /**
         * 调节因子
         */
        const float k1 = 1.5f;

        /**
         * 调节因子
         */
        const float b = 0.75f;

        public BM25(List<List<String>> docs)
        {
            this.docs = docs;
            D = docs.Count;
            foreach (List<String> sentence in docs)
            {
                avgdl += sentence.Count;
            }
            avgdl /= D;
            f = new Dictionary<string, int>[D];
            df = new Dictionary<String, int>();
            idf = new Dictionary<String, Double>();
            init();
        }

        /**
         * 在构造时初始化自己的所有参数
         */
        private void init()
        {
            int index = 0;
            foreach (List<String> sentence in docs)
            {
                Dictionary<String, int> tf = new Dictionary<String, int>();
                foreach (String word in sentence)
                {
                    int freq = tf[word];
                    freq = (freq == null ? 0 : freq) + 1;
                    tf.Add(word, freq);
                }
                f[index] = tf;
                foreach (KeyValuePair<String, int> entry in tf)
                {
                    String word = entry.Key;
                    int freq = df[word];
                    freq = (freq == null ? 0 : freq) + 1;
                    df.Add(word, freq);
                }
                ++index;
            }
            foreach (KeyValuePair<String, int> entry in df)
            {
                String word = entry.Key;
                int freq = entry.Value;
                idf.Add(word, Math.Log(D - freq + 0.5) - Math.Log(freq + 0.5));
            }
        }

        public double sim(List<String> sentence, int index)
        {
            double score = 0;
            foreach (String word in sentence)
            {
                if (!f[index].ContainsKey(word)) continue;
                int d = docs[index].Count;
                int wf = f[index][word];
                score += (idf[word] * wf * (k1 + 1)
                        / (wf + k1 * (1 - b + b * d
                                                    / avgdl)));
            }

            return score;
        }

        public double[] simAll(List<String> sentence)
        {
            double[] scores = new double[D];
            for (int i = 0; i < D; ++i)
            {
                scores[i] = sim(sentence, i);
            }
            return scores;
        }
    }

}
