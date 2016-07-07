using JShibo.NLP.Seg;
using JShibo.NLP.Seg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Tokenizer
{
    /**
 * 可供自然语言处理用的分词器
 *
 * @author hankcs
 */
    public class NLPTokenizer
    {
        /**
         * 预置分词器
         */
        public static Segment SEGMENT = HanLP.newSegment().enableNameRecognize(true).enableTranslatedNameRecognize(true)
                .enableJapaneseNameRecognize(true).enablePlaceRecognize(true).enableOrganizationRecognize(true)
                .enablePartOfSpeechTagging(true);

        public static List<Term> segment(String text)
        {
            return SEGMENT.seg(text);
        }

        /**
         * 分词
         * @param text 文本
         * @return 分词结果
         */
        public static List<Term> segment(char[] text)
        {
            return SEGMENT.seg(text);
        }

        /**
         * 切分为句子形式
         * @param text 文本
         * @return 句子列表
         */
        public static List<List<Term>> seg2sentence(String text)
        {
            return SEGMENT.seg2sentence(text);
        }
    }
}
