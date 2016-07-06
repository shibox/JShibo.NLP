using JShibo.NLP.Collection.Trie;
using JShibo.NLP.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.Common
{
    /**
 * 通用的词典，对应固定格式的词典，但是标签可以泛型化
 *
 * @author hankcs
 */
    public abstract class CommonDictionary<V>
    {
        DoubleArrayTrie<V> trie;

        public bool load(String path)
        {
            //trie = new DoubleArrayTrie<V>();
            //long start = System.currentTimeMillis();
            //V[] valueArray = onLoadValue(path);
            //if (valueArray == null)
            //{
            //    logger.info("加载值" + path + ".value.dat失败，耗时" + (System.currentTimeMillis() - start) + "ms");
            //    return false;
            //}
            //logger.info("加载值" + path + ".value.dat成功，耗时" + (System.currentTimeMillis() - start) + "ms");
            //start = System.currentTimeMillis();
            //if (loadDat(path + ".trie.dat", valueArray))
            //{
            //    logger.info("加载键" + path + ".trie.dat成功，耗时" + (System.currentTimeMillis() - start) + "ms");
            //    return true;
            //}
            //List<String> keyList = new ArrayList<String>(valueArray.length);
            //try
            //{
            //    BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8"));
            //    String line;
            //    while ((line = br.readLine()) != null)
            //    {
            //        String[] paramArray = line.split("\\s");
            //        keyList.add(paramArray[0]);
            //    }
            //    br.close();
            //}
            //catch (Exception e)
            //{
            //    logger.warning("读取" + path + "失败" + e);
            //}
            //int resultCode = trie.build(keyList, valueArray);
            //if (resultCode != 0)
            //{
            //    logger.warning("trie建立失败" + resultCode + ",正在尝试排序后重载");
            //    TreeMap<String, V> map = new TreeMap<String, V>();
            //    for (int i = 0; i < valueArray.length; ++i)
            //    {
            //        map.put(keyList.get(i), valueArray[i]);
            //    }
            //    trie = new DoubleArrayTrie<V>();
            //    trie.build(map);
            //    int i = 0;
            //    for (V v : map.values())
            //    {
            //        valueArray[i++] = v;
            //    }
            //}
            //trie.save(path + ".trie.dat");
            //onSaveValue(valueArray, path);
            //logger.info(path + "加载成功");
            return true;
        }

        private bool loadDat(String path, V[] valueArray)
        {
            //if (trie.load(path, valueArray)) return true;
            return false;
        }

        /**
         * 查询一个单词
         *
         * @param key
         * @return 单词对应的条目
         */
        public V get(String key)
        {
            return trie.get(key);
        }

        /**
         * 是否含有键
         *
         * @param key
         * @return
         */
        public bool contains(String key)
        {
            return get(key) != null;
        }

        /**
         * 词典大小
         *
         * @return
         */
        public int size()
        {
            return trie.size();
        }

        /**
         * 排序这个词典
         *
         * @param path
         * @return
         */
        public static bool sort(String path)
        {
            Dictionary<String, String> map = new Dictionary<string, string>();
            //try
            //{
            //    BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8"));
            //    String line;
            //    while ((line = br.readLine()) != null)
            //    {
            //        String[] argArray = line.split("\\s");
            //        map.put(argArray[0], line);
            //    }
            //    br.close();
            //    // 输出它们
            //    BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(new FileOutputStream(path)));
            //    for (Map.Entry<String, String> entry : map.entrySet())
            //    {
            //        bw.write(entry.getValue());
            //        bw.newLine();
            //    }
            //    bw.close();
            //}
            //catch (Exception e)
            //{
            //    logger.warning("读取" + path + "失败" + e);
            //    return false;
            //}
            return true;
        }

        /**
         * 实现此方法来加载值
         *
         * @param path
         * @return
         */
        protected abstract V[] onLoadValue(String path);

        protected abstract bool onSaveValue(V[] valueArray, String path);

        public BaseSearcher<V> getSearcher(String text)
        {
            //return new Searcher(text);
            return null;
        }

        /**
         * 前缀搜索，长短都可匹配
         */
        public class Searcher : BaseSearcher<V>
    {
        /**
         * 分词从何处开始，这是一个状态
         */
        int begin;

        private List<KeyValuePair<String, V>> entryList;

        protected Searcher(char[] c)
                :base(c)
        {
            
        }

        protected Searcher(String text)
                :base(text)
        {
            
            entryList = new List<KeyValuePair<String, V>>();
        }

        
            public override KeyValuePair<String, V> next()
        {
            // 保证首次调用找到一个词语
            while (entryList.Count == 0 && begin < c.Length)
            {
                //entryList = trie.commonPrefixSearchWithValue(c, begin);
                ++begin;
            }
            // 之后调用仅在缓存用完的时候调用一次
            if (entryList.Count == 0 && begin < c.Length)
            {
                //entryList = trie.commonPrefixSearchWithValue(c, begin);
                ++begin;
            }
            if (entryList.Count == 0)
            {
                    return default(KeyValuePair<string, V>);
            }
            KeyValuePair<String, V> result = entryList[0];
            entryList.RemoveAt(0);
            offset = begin - 1;
            return result;
        }
    }
}
}
