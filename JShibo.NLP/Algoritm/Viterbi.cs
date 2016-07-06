using JShibo.NLP.Corpus.Dictionary.Item;
using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Dictionary;
using JShibo.NLP.Seg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Algoritm
{
    /**
 * 维特比算法
 *
 * @author hankcs
 */
    public class Viterbi
    {
        /**
         * 求解HMM模型，所有概率请提前取对数
         *
         * @param obs     观测序列
         * @param states  隐状态
         * @param start_p 初始概率（隐状态）
         * @param trans_p 转移概率（隐状态）
         * @param emit_p  发射概率 （隐状态表现为显状态的概率）
         * @return 最可能的序列
         */
        public static int[] compute(int[] obs, int[] states, double[] start_p, double[][] trans_p, double[][] emit_p)
        {
            int _max_states_value = 0;
            foreach (int s in states)
            {
                _max_states_value = Math.Max(_max_states_value, s);
            }
            ++_max_states_value;
            double[,] V = new double[obs.Length,_max_states_value];
            int[,] path = new int[_max_states_value,obs.Length];

            foreach (int y in states)
            {
                V[0,y] = start_p[y] + emit_p[y][obs[0]];
                path[y,0] = y;
            }
            double prob = 0;
            int state = 0;
            for (int t = 1; t < obs.Length; ++t)
            {
                int[,] newpath = new int[_max_states_value,obs.Length];

                foreach (int y in states)
                {
                    prob = Double.MaxValue;
                    
                    foreach (int y0 in states)
                    {
                        double nprob = V[t - 1,y0] + trans_p[y0][y] + emit_p[y][obs[t]];
                        if (nprob < prob)
                        {
                            prob = nprob;
                            state = y0;
                            // 记录最大概率
                            V[t,y] = prob;
                            // 记录路径
                            System.arraycopy(path[state], 0, newpath[y], 0, t);
                            newpath[y,t] = y;
                        }
                    }
                }

                path = newpath;
            }

            prob = Double.MaxValue;
            state = 0;
            foreach (int y in states)
            {
                if (V[obs.Length - 1,y] < prob)
                {
                    prob = V[obs.Length - 1,y];
                    state = y;
                }
            }

            return path[state];
        }

        /**
         * 特化版的求解HMM模型
         *
         * @param vertexList                包含Vertex.B节点的路径
         * @param transformMatrixDictionary 词典对应的转移矩阵
         */
        public static void compute(List<Vertex> vertexList, TransformMatrixDictionary<Nature> transformMatrixDictionary)
        {
            int length = vertexList.Count - 1;
            double[][] cost = new double[2][];  // 滚动数组
            Iterator<Vertex> iterator = vertexList.iterator();
            Vertex start = iterator.next();
            Nature pre = start.attribute.nature[0];
            // 第一个是确定的
            //        start.confirmNature(pre);
            // 第二个也可以简单地算出来
            Vertex preItem;
            Nature[] preTagSet;
            {
                Vertex item = iterator.next();
                cost[0] = new double[item.attribute.nature.Length];
                int j = 0;
                int curIndex = 0;
                foreach (Nature cur in item.attribute.nature)
                {
                    cost[0][j] = transformMatrixDictionary.transititon_probability[pre.ordinal()][cur.ordinal()] - Math.Log((item.attribute.frequency[curIndex] + 1e-8) / transformMatrixDictionary.getTotalFrequency(cur));
                    ++j;
                    ++curIndex;
                }
                preTagSet = item.attribute.nature;
                preItem = item;
            }
            // 第三个开始复杂一些
            for (int i = 1; i < length; ++i)
            {
                int index_i = i & 1;
                int index_i_1 = 1 - index_i;
                Vertex item = iterator.next();
                cost[index_i] = new double[item.attribute.nature.Length];
                double perfect_cost_line = Double.MaxValue;
                int k = 0;
                Nature[] curTagSet = item.attribute.nature;
                foreach (Nature cur in curTagSet)
                {
                    cost[index_i][k] = Double.MaxValue;
                    int j = 0;
                    foreach (Nature p in preTagSet)
                    {
                        double now = cost[index_i_1][j] + transformMatrixDictionary.transititon_probability[p.ordinal()][cur.ordinal()] - Math.Log((item.attribute.frequency[k] + 1e-8) / transformMatrixDictionary.getTotalFrequency(cur));
                        if (now < cost[index_i][k])
                        {
                            cost[index_i][k] = now;
                            if (now < perfect_cost_line)
                            {
                                perfect_cost_line = now;
                                pre = p;
                            }
                        }
                        ++j;
                    }
                    ++k;
                }
                preItem.confirmNature(pre);
                preTagSet = curTagSet;
                preItem = item;
            }
        }

        /**
         * 标准版的Viterbi算法，查准率高，效率稍低
         *
         * @param roleTagList               观测序列
         * @param transformMatrixDictionary 转移矩阵
         * @param <E>                       EnumItem的具体类型
         * @return 预测结果
         */
        public static  List<E> computeEnum<E>(List<EnumItem<E>> roleTagList, TransformMatrixDictionary<E> transformMatrixDictionary)
        {
            int length = roleTagList.Count - 1;
            List<E> tagList = new List<E>(roleTagList.Count);
            double[][] cost = new double[2][];  // 滚动数组
            Iterator<EnumItem<E>> iterator = roleTagList.iterator();
            EnumItem<E> start = iterator.next();
            E pre = start.labelMap.entrySet().iterator().next().getKey();
            // 第一个是确定的
            tagList.Add(pre);
            // 第二个也可以简单地算出来
            HashSet<E> preTagSet;
            {
                EnumItem<E> item = iterator.next();
                cost[0] = new double[item.labelMap.Count];
                int j = 0;
                foreach (E cur in item.labelMap)
                {
                    cost[0][j] = transformMatrixDictionary.transititon_probability[pre.ordinal()][cur.ordinal()] - Math.Log((item.getFrequency(cur) + 1e-8) / transformMatrixDictionary.getTotalFrequency(cur));
                    ++j;
                }
                preTagSet = item.labelMap.keySet();
            }
            // 第三个开始复杂一些
            for (int i = 1; i < length; ++i)
            {
                int index_i = i & 1;
                int index_i_1 = 1 - index_i;
                EnumItem<E> item = iterator.next();
                cost[index_i] = new double[item.labelMap.Count];
                double perfect_cost_line = Double.MaxValue;
                int k = 0;
                HashSet<E> curTagSet = item.labelMap.keySet();
                foreach (E cur in curTagSet)
                {
                    cost[index_i][k] = Double.MaxValue;
                    int j = 0;
                    foreach (E p in preTagSet)
                    {
                        double now = cost[index_i_1][j] + transformMatrixDictionary.transititon_probability[p.ordinal()][cur.ordinal()] - Math.Log((item.getFrequency(cur) + 1e-8) / transformMatrixDictionary.getTotalFrequency(cur));
                        if (now < cost[index_i][k])
                        {
                            cost[index_i][k] = now;
                            if (now < perfect_cost_line)
                            {
                                perfect_cost_line = now;
                                pre = p;
                            }
                        }
                        ++j;
                    }
                    ++k;
                }
                tagList.Add(pre);
                preTagSet = curTagSet;
            }
            tagList.Add(tagList[0]);    // 对于最后一个##末##
            return tagList;
        }

        /**
         * 仅仅利用了转移矩阵的“维特比”算法
         *
         * @param roleTagList               观测序列
         * @param transformMatrixDictionary 转移矩阵
         * @param <E>                       EnumItem的具体类型
         * @return 预测结果
         */
        public static List<E> computeEnumSimply<E>(List<EnumItem<E>> roleTagList, TransformMatrixDictionary<E> transformMatrixDictionary)
        {
            int length = roleTagList.Count - 1;
            List<E> tagList = new List<E>();
            Iterator<EnumItem<E>> iterator = roleTagList.iterator();
            EnumItem<E> start = iterator.next();
            E pre = start.labelMap.entrySet().iterator().next().getKey();
            E perfect_tag = pre;
            // 第一个是确定的
            tagList.Add(pre);
            for (int i = 0; i < length; ++i)
            {
                double perfect_cost = Double.MaxValue;
                EnumItem<E> item = iterator.next();
                foreach (E cur in item.labelMap)
                {
                    double now = transformMatrixDictionary.transititon_probability[pre.ordinal()][cur.ordinal()] - Math.Log((item.getFrequency(cur) + 1e-8) / transformMatrixDictionary.getTotalFrequency(cur));
                    if (perfect_cost > now)
                    {
                        perfect_cost = now;
                        perfect_tag = cur;
                    }
                }
                pre = perfect_tag;
                tagList.Add(pre);
            }
            return tagList;
        }
    }
}
