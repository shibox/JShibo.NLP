using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Seg.Common
{
    /**
 * 一个单词，用户可以直接访问此单词的全部属性
 * @author hankcs
 */
    public class Term
    {
        /**
         * 词语
         */
        public String word;

        /**
         * 词性
         */
        public Nature nature;

        /**
         * 在文本中的起始位置（需开启分词器的offset选项）
         */
        public int offset;

        /**
         * 构造一个单词
         * @param word 词语
         * @param nature 词性
         */
        public Term(String word, Nature nature)
        {
            this.word = word;
            this.nature = nature;
        }

        
        public override String ToString()
        {
            if (HanLP.Config.ShowTermNature)
                return word + "/" + nature;
            return word;
        }

        /**
         * 长度
         * @return
         */
        public int length()
        {
            return word.Length;
        }

        /**
         * 获取本词语在HanLP词库中的频次
         * @return 频次，0代表这是个OOV
         */
        public int getFrequency()
        {
            return LexiconUtility.getFrequency(word);
        }
    }
}
