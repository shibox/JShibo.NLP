﻿//using JShibo.NLP.Collection.AhoCorasick;
//using JShibo.NLP.Collection.Trie;
//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace JShibo.NLP.Dictionary.PY
//{
//    /**
// * @author hankcs
// */
//    public class PinyinDictionary
//    {
//        static AhoCorasickDoubleArrayTrie<Pinyin[]> trie = new AhoCorasickDoubleArrayTrie<Pinyin[]>();
//        public static Pinyin[] pinyins = Integer2PinyinConverter.pinyins;

//    static PinyinDictionary()
//    {
//    //    long start = System.currentTimeMillis();
//    //    if (!load(HanLP.Config.PinyinDictionaryPath))
//    //    {
//    //        throw new IllegalArgumentException("拼音词典" + HanLP.Config.PinyinDictionaryPath + "加载失败");
//    //}

//    //logger.info("拼音词典" + HanLP.Config.PinyinDictionaryPath + "加载成功，耗时" + (System.currentTimeMillis() - start) + "ms");
//    }

///**
// * 读取词典
// * @param path
// * @return
// */
//static bool load(String path)
//{
//    //if (loadDat(path)) return true;
//    //// 从文本中载入并且尝试生成dat
//    //StringDictionary dictionary = new StringDictionary("=");
//    //if (!dictionary.load(path)) return false;
//    //TreeMap<String, Pinyin[]> map = new TreeMap<String, Pinyin[]>();
//    //for (Map.Entry<String, String> entry : dictionary.entrySet())
//    //{
//    //    String[] args = entry.getValue().split(",");
//    //    Pinyin[] pinyinValue = new Pinyin[args.length];
//    //    for (int i = 0; i < pinyinValue.length; ++i)
//    //    {
//    //        try
//    //        {
//    //            Pinyin pinyin = Pinyin.valueOf(args[i]);
//    //            pinyinValue[i] = pinyin;
//    //        }
//    //        catch (IllegalArgumentException e)
//    //        {
//    //            logger.severe("读取拼音词典" + path + "失败，问题出在【" + entry + "】，异常是" + e);
//    //            return false;
//    //        }
//    //    }
//    //    map.put(entry.getKey(), pinyinValue);
//    //}
//    //trie.build(map);
//    //logger.info("正在缓存双数组" + path);
//    //saveDat(path, trie, map.entrySet());
//    return true;
//}

//static bool loadDat(String path)
//{
//    //ByteArray byteArray = ByteArray.createByteArray(path + Predefine.BIN_EXT);
//    //if (byteArray == null) return false;
//    //int size = byteArray.nextInt();
//    //Pinyin[][] valueArray = new Pinyin[size][];
//    //for (int i = 0; i < valueArray.length; ++i)
//    //{
//    //    int length = byteArray.nextInt();
//    //    valueArray[i] = new Pinyin[length];
//    //    for (int j = 0; j < length; ++j)
//    //    {
//    //        valueArray[i][j] = pinyins[byteArray.nextInt()];
//    //    }
//    //}
//    //if (!trie.load(byteArray, valueArray)) return false;
//    return true;
//}

//static bool saveDat(String path, AhoCorasickDoubleArrayTrie<Pinyin[]> trie, HashSet<KeyValuePair<String, Pinyin[]>> entrySet)
//{
//    //try
//    //{
//    //    DataOutputStream out = new DataOutputStream(new FileOutputStream(path + Predefine.BIN_EXT));
//    //        out.writeInt(entrySet.size());
//    //    for (Map.Entry<String, Pinyin[]> entry : entrySet)
//    //    {
//    //        Pinyin[] value = entry.getValue();
//    //            out.writeInt(value.length);
//    //        for (Pinyin pinyin : value)
//    //        {
//    //                out.writeInt(pinyin.ordinal());
//    //        }
//    //    }
//    //    trie.save(out);
//    //        out.close();
//    //}
//    //catch (Exception e)
//    //{
//    //    logger.warning("缓存值dat" + path + "失败");
//    //    return false;
//    //}

//    return true;
//}

//public static Pinyin[] get(String key)
//{
//    return trie.get(key);
//}

///**
// * 转为拼音
// * @param text
// * @return List形式的拼音，对应每一个字（所谓字，指的是任意字符）
// */
//public static List<Pinyin> convertToPinyin(String text)
//{
//    return segLongest(text.ToCharArray(), trie);
//}

//public static List<Pinyin> convertToPinyin(String text, bool remainNone)
//{
//    return segLongest(text.ToCharArray(), trie, remainNone);
//}

///**
// * 转为拼音
// * @param text
// * @return 数组形式的拼音
// */
//public static Pinyin[] convertToPinyinArray(String text)
//{
//    return convertToPinyin(text).ToArray();
//}

//public static BaseSearcher getSearcher(char[] charArray, DoubleArrayTrie<Pinyin[]> trie)
//{
//    return new Searcher(charArray, trie);
//}

///**
// * 用最长分词算法匹配拼音
// * @param charArray
// * @param trie
// * @return
// */
//protected static List<Pinyin> segLongest(char[] charArray, AhoCorasickDoubleArrayTrie<Pinyin[]> trie)
//{
//    return segLongest(charArray, trie, true);
//}

//protected static List<Pinyin> segLongest(char[] charArray, AhoCorasickDoubleArrayTrie<Pinyin[]> trie, bool remainNone)
//{
//            //    Pinyin[][]
//            //wordNet = new Pinyin[charArray.Length][];
//            //        trie.parseText(charArray, new Collection.AhoCorasick.AhoCorasickDoubleArrayTrie.IHit<Pinyin[]>()
//            //        {

//            //            public void hit(int begin, int end, Pinyin[] value)
//            //{
//            //    int length = end - begin;
//            //    if (wordNet[begin] == null || length > wordNet[begin].length)
//            //    {
//            //        wordNet[begin] = length == 1 ? new Pinyin[] { value[0] } : value;
//            //    }
//            //}
//            //        });
//            //        List<Pinyin> pinyinList = new ArrayList<Pinyin>(charArray.length);
//            //        for (int offset = 0; offset<wordNet.length; )
//            //        {
//            //            if (wordNet[offset] == null)
//            //            {
//            //                if (remainNone)
//            //                {
//            //                    pinyinList.add(Pinyin.none5);
//            //                }
//            //                ++offset;
//            //                continue;
//            //            }
//            //            for (Pinyin pinyin : wordNet[offset])
//            //            {
//            //                pinyinList.add(pinyin);
//            //            }
//            //            offset += wordNet[offset].length;
//            //        }
//            //        return pinyinList;
//            return null;
//    }

//    public class Searcher : BaseSearcher<Pinyin[]>
//    {
//    /**
//     * 分词从何处开始，这是一个状态
//     */
//    int begin;

//    DoubleArrayTrie<Pinyin[]> trie;

//        protected Searcher(char[] c, DoubleArrayTrie<Pinyin[]> trie)
//                :base(c)
//{
    
//    this.trie = trie;
//}

//protected Searcher(String text, DoubleArrayTrie<Pinyin[]> trie)
//                :base(text)
//{
    
//    this.trie = trie;
//}


//        public KeyValuePair<String, Pinyin[]> next()
//{
//    // 保证首次调用找到一个词语
//    KeyValuePair<String, Pinyin[]> result = default(KeyValuePair<String, Pinyin[]>);
//    while (begin < c.Length)
//    {
//        LinkedList<KeyValuePair<String, Pinyin[]>> entryList = trie.commonPrefixSearchWithValue(c, begin);
//        if (entryList.Count == 0)
//        {
//            ++begin;
//        }
//        else
//        {
//            result = entryList.Last();
//            offset = begin;
//            begin += result.Key.Length;
//            break;
//        }
//    }
//    //if (result == null)
//    //{
//    //    return null;
//    //}
//    return result;
//}
//    }
//}
//}
