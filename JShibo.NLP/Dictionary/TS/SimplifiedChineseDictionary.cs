using JShibo.NLP.Collection.AhoCorasick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.TS
{
    /**
 * 简体=繁体词典
 * @author hankcs
 */
    public class SimplifiedChineseDictionary : BaseChineseDictionary
    {
        /**
         * 简体=繁体
         */
        static AhoCorasickDoubleArrayTrie<String> trie = new AhoCorasickDoubleArrayTrie<String>();

        static SimplifiedChineseDictionary()
        {
            //        long start = System.currentTimeMillis();
            //        if (!load(HanLP.Config.TraditionalChineseDictionaryPath, trie, true))
            //        {
            //            throw new IllegalArgumentException("简繁词典" + HanLP.Config.TraditionalChineseDictionaryPath + Predefine.REVERSE_EXT + "加载失败");
            //}

            //logger.info("简繁词典" + HanLP.Config.TraditionalChineseDictionaryPath + Predefine.REVERSE_EXT + "加载成功，耗时" + (System.currentTimeMillis() - start) + "ms");
        }

        public static String convertToTraditionalChinese(String simplifiedChineseString)
        {
            return segLongest(simplifiedChineseString.ToCharArray(), trie);
        }

        public static String convertToTraditionalChinese(char[] simplifiedChinese)
        {
            return segLongest(simplifiedChinese, trie);
        }

        public static String getTraditionalChinese(String simplifiedChinese)
        {
            return trie.get(simplifiedChinese);
        }
    }
}
