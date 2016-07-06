using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.NR
{
    /**
 * 人名识别中常用的一些常量
 * @author hankcs
 */
    public class NRConstant
    {
        /**
         * 本词典专注的词的ID
         */
        public static int WORD_ID = CoreDictionary.getWordID(Predefine.TAG_PEOPLE);
        /**
         * 本词典专注的词的属性
         */
        public static CoreDictionary.Attribute ATTRIBUTE = CoreDictionary.get(WORD_ID);
    }
}
