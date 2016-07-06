using JShibo.NLP.Collection.Trie;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.NR
{
    /**
 * 翻译人名词典，储存和识别翻译人名
 * @author hankcs
 */
    public class TranslatedPersonDictionary
    {
        static String path = HanLP.Config.TranslatedPersonDictionaryPath;
        static DoubleArrayTrie<Boolean> trie;

        static TranslatedPersonDictionary()
        {
            //    long start = System.currentTimeMillis();
            //    if (!load())
            //    {
            //        throw new IllegalArgumentException("音译人名词典" + path + "加载失败");
            //}

            //logger.info("音译人名词典" + path + "加载成功，耗时" + (System.currentTimeMillis() - start) + "ms");
        }

        static bool load()
        {
            //trie = new DoubleArrayTrie<Boolean>();
            //if (loadDat()) return true;
            //try
            //{
            //    StreamReader br = new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8"));
            //    String line;
            //    TreeMap<String, Boolean> map = new TreeMap<String, Boolean>();
            //    TreeMap<Character, Integer> charFrequencyMap = new TreeMap<Character, Integer>();
            //    while ((line = br.readLine()) != null)
            //    {
            //        map.put(line, true);
            //        // 音译人名常用字词典自动生成
            //        for (char c : line.toCharArray())
            //        {
            //            // 排除一些过于常用的字
            //            if ("不赞".indexOf(c) >= 0) continue;
            //            Integer f = charFrequencyMap.get(c);
            //            if (f == null) f = 0;
            //            charFrequencyMap.put(c, f + 1);
            //        }
            //    }
            //    br.close();
            //    map.put(String.valueOf('·'), true);
            //    //            map.put(String.valueOf('-'), true);
            //    //            map.put(String.valueOf('—'), true);
            //    // 将常用字也加进去
            //    for (Map.Entry<Character, Integer> entry : charFrequencyMap.entrySet())
            //    {
            //        if (entry.getValue() < 10) continue;
            //        map.put(String.valueOf(entry.getKey()), true);
            //    }
            //    logger.info("音译人名词典" + path + "开始构建双数组……");
            //    trie.build(map);
            //    logger.info("音译人名词典" + path + "开始编译DAT文件……");
            //    logger.info("音译人名词典" + path + "编译结果：" + saveDat(map));
            //}
            //catch (Exception e)
            //{
            //    logger.severe("自定义词典" + path + "读取错误！" + e);
            //    return false;
            //}

            return true;
        }

        /**
         * 保存dat到磁盘
         * @param map
         * @return
         */
        static bool saveDat(Dictionary<String, Boolean> map)
        {
            //return trie.save(path + Predefine.TRIE_EXT);
            return true;
        }

        static bool loadDat()
        {
            //return trie.load(path + Predefine.TRIE_EXT);
            return true;
        }

        /**
         * 是否包含key
         * @param key
         * @return
         */
        public static bool containsKey(String key)
        {
            return trie.ContainsKey(key);
        }

        /**
         * 时报包含key，且key至少长length
         * @param key
         * @param length
         * @return
         */
        public static bool containsKey(String key, int length)
        {
            if (!trie.ContainsKey(key)) return false;
            return key.Length >= length;
        }
    }
}
