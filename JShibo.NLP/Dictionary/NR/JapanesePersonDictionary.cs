﻿using JShibo.NLP.Collection.Trie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.NR
{
    public class JapanesePersonDictionary
    {
        static String path = HanLP.Config.JapanesePersonDictionaryPath;
        static DoubleArrayTrie<char> trie;
        /**
         * 姓
         */
        public static char X = 'x';
        /**
         * 名
         */
        public static char M = 'm';
        /**
         * bad case
         */
        public static char A = 'A';

        static JapanesePersonDictionary()
    {
    //    long start = System.currentTimeMillis();
    //    if (!load())
    //    {
    //        throw new IllegalArgumentException("日本人名词典" + path + "加载失败");
    //}

    //logger.info("日本人名词典" + HanLP.Config.PinyinDictionaryPath + "加载成功，耗时" + (System.currentTimeMillis() - start) + "ms");
    }

static bool load()
{
    //trie = new DoubleArrayTrie<char>();
    //if (loadDat()) return true;
    //try
    //{
    //    BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8"));
    //    String line;
    //    TreeMap<String, Character> map = new TreeMap<String, Character>();
    //    while ((line = br.readLine()) != null)
    //    {
    //        String[] param = line.split(" ", 2);
    //        map.put(param[0], param[1].charAt(0));
    //    }
    //    br.close();
    //    logger.info("日本人名词典" + path + "开始构建双数组……");
    //    trie.build(map);
    //    logger.info("日本人名词典" + path + "开始编译DAT文件……");
    //    logger.info("日本人名词典" + path + "编译结果：" + saveDat(map));
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
static bool saveDat(Dictionary<String, char> map)
{
            //try
            //{
            //    DataOutputStream out = new DataOutputStream(new FileOutputStream(path + Predefine.VALUE_EXT));
            //        out.writeInt(map.size());
            //    for (Character character : map.values())
            //    {
            //            out.writeChar(character);
            //    }
            //        out.close();
            //}
            //catch (Exception e)
            //{
            //    logger.warning("保存值" + path + Predefine.VALUE_EXT + "失败" + e);
            //    return false;
            //}
            //return trie.save(path + Predefine.TRIE_EXT);
            return trie;
}

static bool loadDat()
{
            //ByteArray byteArray = ByteArray.createByteArray(path + Predefine.VALUE_EXT);
            //if (byteArray == null) return false;
            //int size = byteArray.nextInt();
            //Character[] valueArray = new Character[size];
            //for (int i = 0; i < valueArray.length; ++i)
            //{
            //    valueArray[i] = byteArray.nextChar();
            //}
            //return trie.load(path + Predefine.TRIE_EXT, valueArray);
            return trie;
}

/**
 * 是否包含key
 * @param key
 * @return
 */
public static bool containsKey(String key)
{
    return trie.containsKey(key);
}

/**
 * 包含key，且key至少长length
 * @param key
 * @param length
 * @return
 */
public static bool containsKey(String key, int length)
{
    if (!trie.containsKey(key)) return false;
    return key.Length >= length;
}

public static char get(String key)
{
    return trie.get(key);
}

public static BaseSearcher getSearcher(char[] charArray)
{
    return new Searcher(charArray, trie);
}

/**
 * 最长分词
 */
public static class Searcher extends BaseSearcher<Character>
    {
        /**
         * 分词从何处开始，这是一个状态
         */
        int begin;

    DoubleArrayTrie<Character> trie;

        protected Searcher(char[] c, DoubleArrayTrie<Character> trie)
{
    super(c);
    this.trie = trie;
}

protected Searcher(String text, DoubleArrayTrie<Character> trie)
{
    super(text);
    this.trie = trie;
}

@Override
        public Map.Entry<String, Character> next()
{
    // 保证首次调用找到一个词语
    Map.Entry<String, Character> result = null;
    while (begin < c.length)
    {
        LinkedList<Map.Entry<String, Character>> entryList = trie.commonPrefixSearchWithValue(c, begin);
        if (entryList.size() == 0)
        {
            ++begin;
        }
        else
        {
            result = entryList.getLast();
            offset = begin;
            begin += result.getKey().length();
            break;
        }
    }
    if (result == null)
    {
        return null;
    }
    return result;
}
    }
}
}