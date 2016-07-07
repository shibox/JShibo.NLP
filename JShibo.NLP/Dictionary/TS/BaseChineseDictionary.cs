using JShibo.NLP.Collection.AhoCorasick;
using JShibo.NLP.Collection.Trie;
using JShibo.NLP.Dictionary;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.TS
{
    public class BaseChineseDictionary
    {
        /**
         * 将path的内容载入trie中
         * @param path
         * @param trie
         * @return
         */
        static bool load(String path, AhoCorasickDoubleArrayTrie<String> trie)
        {
            return load(path, trie, false);
        }

        /**
         * 读取词典
         * @param path
         * @param trie
         * @param reverse 是否将其翻转
         * @return
         */
        static bool load(String path, AhoCorasickDoubleArrayTrie<String> trie, bool reverse)
        {
            //String datPath = path;
            //if (reverse)
            //{
            //    datPath += Predefine.REVERSE_EXT;
            //}
            //if (loadDat(datPath, trie)) return true;
            //// 从文本中载入并且尝试生成dat
            //StringDictionary dictionary = new StringDictionary("=");
            //if (!dictionary.load(path)) return false;
            //if (reverse) dictionary = dictionary.reverse();
            //HashSet<KeyValuePair<String, String>> entrySet = dictionary;
            //dictionary<String, String> map = new Dictionary<String, String>();
            //for (Map.Entry<String, String> entry : entrySet)
            //{
            //    map.put(entry.getKey(), entry.getValue());
            //}
            //logger.info("正在构建AhoCorasickDoubleArrayTrie，来源：" + path);
            //trie.build(map);
            //logger.info("正在缓存双数组" + datPath);
            //saveDat(datPath, trie, entrySet);
            return true;
        }

        static bool loadDat(String path, AhoCorasickDoubleArrayTrie<String> trie)
        {
            //ByteArray byteArray = ByteArray.createByteArray(path + Predefine.BIN_EXT);
            //if (byteArray == null) return false;
            //int size = byteArray.nextInt();
            //String[] valueArray = new String[size];
            //for (int i = 0; i < valueArray.length; ++i)
            //{
            //    valueArray[i] = byteArray.nextString();
            //}
            //trie.load(byteArray, valueArray);
            return true;
        }

        static bool saveDat(String path, AhoCorasickDoubleArrayTrie<String> trie, HashSet<KeyValuePair<String, String>> entrySet)
        {
            //try
            //{
            //    DataOutputStream out = new DataOutputStream(new FileOutputStream(path + Predefine.BIN_EXT));
            //out.writeInt(entrySet.size());
            //    for (Map.Entry<String, String> entry : entrySet)
            //    {
            //        char[] charArray = entry.getValue().toCharArray();
            //    out.writeInt(charArray.length);
            //        for (char c : charArray)
            //        {
            //        out.writeChar(c);
            //        }
            //    }
            //    trie.save(out);
            //out.close();
            //}
            //catch (Exception e)
            //{
            //    logger.warning("缓存值dat" + path + "失败");
            //    return false;
            //}

            return true;
        }

        //public static BaseSearcher getSearcher(char[] charArray, DoubleArrayTrie<String> trie)
        //{
        //    return new Searcher(charArray, trie);
        //}

        protected static String segLongest(char[] charArray, DoubleArrayTrie<String> trie)
        {
            //StringBuilder sb = new StringBuilder(charArray.Length);
            //BaseSearcher searcher = getSearcher(charArray, trie);
            //KeyValuePair<String, String> entry;
            //int p = 0;  // 当前处理到什么位置
            //int offset;
            //while ((entry = searcher.next()) != null)
            //{
            //    offset = searcher.getOffset();
            //    // 补足没查到的词
            //    while (p < offset)
            //    {
            //        sb.Append(charArray[p]);
            //        ++p;
            //    }
            //    sb.Append(entry.Value);
            //    p = offset + entry.Key.Length;
            //}
            //// 补足没查到的词
            //while (p < charArray.Length)
            //{
            //    sb.Append(charArray[p]);
            //    ++p;
            //}
            //return sb.ToString();
            return null;
        }

        protected static String segLongest(char[] charArray, AhoCorasickDoubleArrayTrie<String> trie)
        {
            //            final String[] wordNet = new String[charArray.length];
            //            final int[] lengthNet = new int[charArray.length];
            //            trie.parseText(charArray, new AhoCorasickDoubleArrayTrie.IHit<String>()
            //        {
            //            @Override
            //            public void hit(int begin, int end, String value)
            //        {
            //            int length = end - begin;
            //            if (length > lengthNet[begin])
            //            {
            //                wordNet[begin] = value;
            //                lengthNet[begin] = length;
            //            }
            //        }
            //    });
            //        StringBuilder sb = new StringBuilder(charArray.length);
            //        for (int offset = 0; offset<wordNet.length; )
            //        {
            //            if (wordNet[offset] == null)
            //            {
            //                sb.append(charArray[offset]);
            //                ++offset;
            //                continue;
            //            }
            //sb.append(wordNet[offset]);
            //            offset += lengthNet[offset];
            //        }
            //        return sb.toString();
            return null;
        }

        /**
         * 最长分词
         */
        public class Searcher : BaseSearcher<String>
        {
            /**
             * 分词从何处开始，这是一个状态
             */
            int begin;

            DoubleArrayTrie<String> trie;

            protected Searcher(char[] c, DoubleArrayTrie<String> trie)
                    : base(c)
            {

                this.trie = trie;
            }

            protected Searcher(String text, DoubleArrayTrie<String> trie)
                            : base(text)
            {

                this.trie = trie;
            }


            public override KeyValuePair<String, String> next()
            {
                //// 保证首次调用找到一个词语
                //KeyValuePair<String, String> result;
                //while (begin < c.Length)
                //{
                //    LinkedList<KeyValuePair<String, String>> entryList = trie.commonPrefixSearchWithValue(c, begin);
                //    if (entryList.Count == 0)
                //    {
                //        ++begin;
                //    }
                //    else
                //    {
                //        result = entryList.getLast();
                //        offset = begin;
                //        begin += result.Key.Length;
                //        break;
                //    }
                //}
                //if (result == null)
                //{
                //    return null;
                //}
                //return result;
                return default(KeyValuePair<string, string>);
            }
        }
    }
}
