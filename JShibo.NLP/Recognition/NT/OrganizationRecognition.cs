using JShibo.NLP.Algoritm;
using JShibo.NLP.Corpus.Dictionary.Item;
using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Dictionary.NT;
using JShibo.NLP.Seg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Recognition.NT
{
    /**
 * 地址识别
 *
 * @author hankcs
 */
    public class OrganizationRecognition
    {
        public static bool Recognition(LinkedList<Vertex> pWordSegResult, WordNet wordNetOptimum, WordNet wordNetAll)
        {
            LinkedList<EnumItem<Corpus.Tag.NT>> roleTagList = roleTag(pWordSegResult, wordNetAll);
            if (HanLP.Config.DEBUG)
            {
                StringBuilder sbLog = new StringBuilder();
                //Iterator<Vertex> iterator = pWordSegResult.iterator();
                foreach (EnumItem<Corpus.Tag.NT> NTEnumItem in roleTagList)
                {
                    sbLog.Append('[');
                    //sbLog.Append(iterator.next().realWord);
                    sbLog.Append(' ');
                    sbLog.Append(NTEnumItem);
                    sbLog.Append(']');
                }
                Console.WriteLine("机构名角色观察：%s\n", sbLog.ToString());
            }
            LinkedList<Corpus.Tag.NT> NTList = viterbiExCompute(roleTagList);
            if (HanLP.Config.DEBUG)
            {
                StringBuilder sbLog = new StringBuilder();
                //Iterator<Vertex> iterator = pWordSegResult.iterator();
                sbLog.Append('[');
                foreach (Corpus.Tag.NT NT in NTList)
                {
                    //sbLog.Append(iterator.next().realWord);
                    sbLog.Append('/');
                    sbLog.Append(NT);
                    sbLog.Append(" ,");
                }
                if (sbLog.Length > 1) sbLog.Remove(sbLog.Length - 2, sbLog.Length);
                sbLog.Append(']');
                Console.WriteLine("机构名角色标注：%s\n", sbLog.ToString());
            }

            OrganizationDictionary.parsePattern(NTList, pWordSegResult, wordNetOptimum, wordNetAll);
            return true;
        }

        public static LinkedList<EnumItem<Corpus.Tag.NT>> roleTag(LinkedList<Vertex> vertexList, WordNet wordNetAll)
        {
            LinkedList<EnumItem<Corpus.Tag.NT>> tagList = new LinkedList<EnumItem<Corpus.Tag.NT>>();
            
            foreach (Vertex vertex in vertexList)
            {
                // 构成更长的
                Nature nature = vertex.guessNature();
                switch (nature)
                {
                    case Nature.nz:
                        {
                            if (vertex.getAttribute().totalFrequency <= 1000)
                            {
                                tagList.AddLast(new EnumItem<Corpus.Tag.NT>(Corpus.Tag.NT.F, 1000));
                            }
                            else break;
                        }
                        continue;
                    case Nature.ni:
                    case Nature.nic:
                    case Nature.nis:
                    case Nature.nit:
                        {
                            EnumItem<Corpus.Tag.NT> ntEnumItem = new EnumItem<Corpus.Tag.NT>(Corpus.Tag.NT.K, 1000);
                            ntEnumItem.addLabel(Corpus.Tag.NT.D, 1000);
                            tagList.AddLast(ntEnumItem);
                        }
                        continue;
                    case Nature.m:
                        {
                            EnumItem<Corpus.Tag.NT> ntEnumItem = new EnumItem<Corpus.Tag.NT>(Corpus.Tag.NT.M, 1000);
                            tagList.AddLast(ntEnumItem);
                        }
                        continue;
                }

                EnumItem<Corpus.Tag.NT> NTEnumItem = OrganizationDictionary.dictionary.get(vertex.word);  // 此处用等效词，更加精准
                if (NTEnumItem == null)
                {
                    NTEnumItem = new EnumItem<Corpus.Tag.NT>(Corpus.Tag.NT.Z, OrganizationDictionary.transformMatrixDictionary.getTotalFrequency(Corpus.Tag.NT.Z));
                }
                tagList.AddLast(NTEnumItem);
            }
            return tagList;
        }

        /**
         * 维特比算法求解最优标签
         *
         * @param roleTagList
         * @return
         */
        public static LinkedList<Corpus.Tag.NT> viterbiExCompute(LinkedList<EnumItem<Corpus.Tag.NT>> roleTagList)
        {
            return Viterbi.computeEnum(roleTagList, OrganizationDictionary.transformMatrixDictionary);
        }
    }
}
