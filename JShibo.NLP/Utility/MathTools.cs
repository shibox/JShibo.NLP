using JShibo.NLP.Dictionary;
using JShibo.NLP.Seg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Utility
{
    /**
 * @author hankcs
 */
    public class MathTools
    {
        /**
         * 从一个词到另一个词的词的花费
         *
         * @param from 前面的词
         * @param to   后面的词
         * @return 分数
         */
        public static double calculateWeight(Vertex from, Vertex to)
        {
            int frequency = from.getAttribute().totalFrequency;
            if (frequency == 0)
            {
                frequency = 1;  // 防止发生除零错误
            }
            //        int nTwoWordsFreq = BiGramDictionary.getBiFrequency(from.word, to.word);
            int nTwoWordsFreq = CoreBiGramTableDictionary.getBiFrequency(from.wordID, to.wordID);
            double value = -Math.Log(Predefine.dSmoothingPara * frequency / (Predefine.MAX_FREQUENCY) + (1 - Predefine.dSmoothingPara) * ((1 - Predefine.dTemp) * nTwoWordsFreq / frequency + Predefine.dTemp));
            if (value < 0.0)
            {
                value = -value;
            }
            //        logger.info(String.format("%5s frequency:%6d, %s nTwoWordsFreq:%3d, weight:%.2f", from.word, frequency, from.word + "@" + to.word, nTwoWordsFreq, value));
            return value;
        }
    }
}
