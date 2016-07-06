using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Seg.Common;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.NR
{
    /**
 * 音译人名识别
 * @author hankcs
 */
    public class TranslatedPersonRecognition
    {
        /**
         * 执行识别
         * @param segResult 粗分结果
         * @param wordNetOptimum 粗分结果对应的词图
         * @param wordNetAll 全词图
         */
        public static void Recognition(List<Vertex> segResult, WordNet wordNetOptimum, WordNet wordNetAll)
        {
            StringBuilder sbName = new StringBuilder();
            int appendTimes = 0;
            ListIterator<Vertex> listIterator = segResult.listIterator();
            listIterator.next();
            int line = 1;
            int activeLine = 1;
            while (listIterator.hasNext())
            {
                Vertex vertex = listIterator.next();
                if (appendTimes > 0)
                {
                    if (vertex.guessNature() == Nature.nrf || TranslatedPersonDictionary.containsKey(vertex.realWord))
                    {
                        sbName.Append(vertex.realWord);
                        ++appendTimes;
                    }
                    else
                    {
                        // 识别结束
                        if (appendTimes > 1)
                        {
                            if (HanLP.Config.DEBUG)
                            {
                                //System.out.println("音译人名识别出：" + sbName.ToString());
                            }
                            wordNetOptimum.insert(activeLine, new Vertex(Predefine.TAG_PEOPLE, sbName.ToString(), new CoreDictionary.Attribute(Nature.nrf), NRConstant.WORD_ID), wordNetAll);
                        }
                        sbName.Length = 0;
                        appendTimes = 0;
                    }
                }
                else
                {
                    // nrf和nsf触发识别
                    if (vertex.guessNature() == Nature.nrf || vertex.getNature() == Nature.nsf
                            //                        || TranslatedPersonDictionary.containsKey(vertex.realWord)
                            )
                    {
                        sbName.Append(vertex.realWord);
                        ++appendTimes;
                        activeLine = line;
                    }
                }

                line += vertex.realWord.Length;
            }
        }
    }
}
