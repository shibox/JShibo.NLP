using JShibo.NLP.Algoritm;
using JShibo.NLP.Corpus.Dictionary.Item;
using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Dictionary.NR;
using JShibo.NLP.Seg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Recognition.NR
{
    /**
 * 人名识别
 * @author hankcs
 */
    public class PersonRecognition
    {
        public static bool Recognition(List<Vertex> pWordSegResult, WordNet wordNetOptimum, WordNet wordNetAll)
        {
            List<EnumItem<Corpus.Tag.NR>> roleTagList = roleObserve(pWordSegResult);
            if (HanLP.Config.DEBUG)
            {
                StringBuilder sbLog = new StringBuilder();
                Iterator<Vertex> iterator = pWordSegResult.iterator();
                foreach (EnumItem<Corpus.Tag.NR> nrEnumItem in roleTagList)
                {
                    sbLog.Append('[');
                    sbLog.Append(iterator.next().realWord);
                    sbLog.Append(' ');
                    sbLog.Append(nrEnumItem);
                    sbLog.Append(']');
                }
                System.out.printf("人名角色观察：%s\n", sbLog.ToString());
            }
            List<Corpus.Tag.NR> nrList = viterbiComputeSimply(roleTagList);
            if (HanLP.Config.DEBUG)
            {
                StringBuilder sbLog = new StringBuilder();
                Iterator<Vertex> iterator = pWordSegResult.iterator();
                sbLog.Append('[');
                foreach (Corpus.Tag.NR nr in nrList)
                {
                    sbLog.Append(iterator.next().realWord);
                    sbLog.Append('/');
                    sbLog.Append(nr);
                    sbLog.Append(" ,");
                }
                if (sbLog.Length > 1) sbLog.delete(sbLog.Length - 2, sbLog.Length);
                sbLog.Append(']');
                System.out.printf("人名角色标注：%s\n", sbLog.ToString());
            }

            PersonDictionary.parsePattern(nrList, pWordSegResult, wordNetOptimum, wordNetAll);
            return true;
        }

        /**
         * 角色观察(从模型中加载所有词语对应的所有角色,允许进行一些规则补充)
         * @param wordSegResult 粗分结果
         * @return
         */
        public static List<EnumItem<Corpus.Tag.NR>> roleObserve(List<Vertex> wordSegResult)
        {
            List<EnumItem<Corpus.Tag.NR>> tagList = new List<EnumItem<Corpus.Tag.NR>>();
            foreach (Vertex vertex in wordSegResult)
            {
                EnumItem<Corpus.Tag.NR> nrEnumItem = PersonDictionary.dictionary.get(vertex.realWord);
                if (nrEnumItem == null)
                {
                    switch (vertex.guessNature())
                    {
                        case Nature.nr:
                            {
                                // 有些双名实际上可以构成更长的三名
                                if (vertex.getAttribute().totalFrequency <= 1000 && vertex.realWord.Length == 2)
                                {
                                    nrEnumItem = new EnumItem<Corpus.Tag.NR>(Corpus.Tag.NR.X, (int)Corpus.Tag.NR.G);
                                }
                                else nrEnumItem = new EnumItem<Corpus.Tag.NR>(Corpus.Tag.NR.A, PersonDictionary.transformMatrixDictionary.getTotalFrequency(Corpus.Tag.NR.A));
                            }
                            break;
                        case  Nature.nnt:
                            {
                                // 姓+职位
                                nrEnumItem = new EnumItem<Corpus.Tag.NR>(Corpus.Tag.NR.G, (int)Corpus.Tag.NR.K);
                            }
                            break;
                        default:
                            {
                                nrEnumItem = new EnumItem<Corpus.Tag.NR>(Corpus.Tag.NR.A, PersonDictionary.transformMatrixDictionary.getTotalFrequency(Corpus.Tag.NR.A));
                            }
                            break;
                    }
                }
                tagList.Add(nrEnumItem);
            }
            return tagList;
        }

        /**
         * 维特比算法求解最优标签
         * @param roleTagList
         * @return
         */
        public static List<Corpus.Tag.NR> viterbiCompute(List<EnumItem<Corpus.Tag.NR>> roleTagList)
        {
            return Viterbi.computeEnum(roleTagList, PersonDictionary.transformMatrixDictionary);
        }

        /**
         * 简化的"维特比算法"求解最优标签
         * @param roleTagList
         * @return
         */
        public static List<Corpus.Tag.NR> viterbiComputeSimply(List<EnumItem<Corpus.Tag.NR>> roleTagList)
        {
            return Viterbi.computeEnumSimply(roleTagList, PersonDictionary.transformMatrixDictionary);
        }
    }
}
