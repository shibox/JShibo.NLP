using JShibo.NLP.Collection.Trie;
using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Dictionary;
using JShibo.NLP.Seg;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary
{
    /**
 * 使用DoubleArrayTrie实现的核心词典
 * @author hankcs
 */
    public class CoreDictionary
    {
        public static DoubleArrayTrie<Attribute> trie = new DoubleArrayTrie<Attribute>();
        public static String path = HanLP.Config.CoreDictionaryPath;
        public static int totalFrequency = 221894;

        // 自动加载词典
        static CoreDictionary()
        {
            //long start = System.currentTimeMillis();
            //if (!load(path))
            //{
            //    System.err.printf("核心词典%s加载失败\n", path);
            //    System.exit(-1);
            //}
            //else
            //{
            //    logger.info(path + "加载成功，" + trie.size() + "个词条，耗时" + (System.currentTimeMillis() - start) + "ms");
            //}
        }

        // 一些特殊的WORD_ID
        public static int NR_WORD_ID = getWordID(Predefine.TAG_PEOPLE);
        public static int NS_WORD_ID = getWordID(Predefine.TAG_PLACE);
        public static int NT_WORD_ID = getWordID(Predefine.TAG_GROUP);
        public static int T_WORD_ID = getWordID(Predefine.TAG_TIME);
        public static int X_WORD_ID = getWordID(Predefine.TAG_CLUSTER);
        public static int M_WORD_ID = getWordID(Predefine.TAG_NUMBER);
        public static int NX_WORD_ID = getWordID(Predefine.TAG_PROPER);

        private static bool load(String path)
        {
            //logger.info("核心词典开始加载:" + path);
            //if (loadDat(path)) return true;
            //TreeMap<String, CoreDictionary.Attribute> map = new TreeMap<String, Attribute>();
            //BufferedReader br = null;
            //try
            //{
            //    br = new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8"));
            //    String line;
            //    int MAX_FREQUENCY = 0;
            //    long start = System.currentTimeMillis();
            //    while ((line = br.readLine()) != null)
            //    {
            //        String param[] = line.split("\\s");
            //        int natureCount = (param.length - 1) / 2;
            //        CoreDictionary.Attribute attribute = new CoreDictionary.Attribute(natureCount);
            //        for (int i = 0; i < natureCount; ++i)
            //        {
            //            attribute.nature[i] = Enum.valueOf(Nature.class, param[1 + 2 * i]);
            //                attribute.frequency[i] = Integer.parseInt(param[2 + 2 * i]);
            //                attribute.totalFrequency += attribute.frequency[i];
            //            }
            //            map.put(param[0], attribute);
            //            MAX_FREQUENCY += attribute.totalFrequency;
            //        }
            //        logger.info("核心词典读入词条" + map.size() + " 全部频次" + MAX_FREQUENCY + "，耗时" + (System.currentTimeMillis() - start) + "ms");
            //        br.close();
            //        trie.build(map);
            //        logger.info("核心词典加载成功:" + trie.size() + "个词条，下面将写入缓存……");
            //        try
            //        {
            //            DataOutputStream out = new DataOutputStream(new FileOutputStream(path + Predefine.BIN_EXT));
            //            Collection<CoreDictionary.Attribute> attributeList = map.values();
            //            out.writeInt(attributeList.size());
            //            for (CoreDictionary.Attribute attribute : attributeList)
            //            {
            //                out.writeInt(attribute.totalFrequency);
            //                out.writeInt(attribute.nature.length);
            //                for (int i = 0; i<attribute.nature.length; ++i)
            //                {
            //                    out.writeInt(attribute.nature[i].ordinal());
            //                    out.writeInt(attribute.frequency[i]);
            //                }
            //            }
            //            trie.save(out);
            //            out.close();
            //        }
            //        catch (Exception e)
            //        {
            //            logger.warning("保存失败" + e);
            //            return false;
            //        }
            //    }
            //    catch (FileNotFoundException e)
            //    {
            //        logger.warning("核心词典" + path + "不存在！" + e);
            //        return false;
            //    }
            //    catch (IOException e)
            //    {
            //        logger.warning("核心词典" + path + "读取错误！" + e);
            //        return false;
            //    }

            return true;
        }

        /**
         * 从磁盘加载双数组
         *
         * @param path
         * @return
         */
        static bool loadDat(String path)
        {
            try
            {
                ByteArray byteArray = ByteArray.createByteArray(path + Predefine.BIN_EXT);
                if (byteArray == null) return false;
                int size = byteArray.nextInt();
                CoreDictionary.Attribute[] attributes = new CoreDictionary.Attribute[size];
                final Nature[] natureIndexArray = Nature.values();
                for (int i = 0; i < size; ++i)
                {
                    // 第一个是全部频次，第二个是词性个数
                    int currentTotalFrequency = byteArray.nextInt();
                    int length = byteArray.nextInt();
                    attributes[i] = new CoreDictionary.Attribute(length);
                    attributes[i].totalFrequency = currentTotalFrequency;
                    for (int j = 0; j < length; ++j)
                    {
                        attributes[i].nature[j] = natureIndexArray[byteArray.nextInt()];
                        attributes[i].frequency[j] = byteArray.nextInt();
                    }
                }
                if (!trie.load(byteArray, attributes) || byteArray.hasMore()) return false;
            }
            catch (Exception e)
            {
                logger.warning("读取失败，问题发生在" + e);
                return false;
            }
            return true;
        }

        /**
         * 获取条目
         * @param key
         * @return
         */
        public static Attribute get(String key)
        {
            return trie.get(key);
        }

        /**
         * 获取条目
         * @param wordID
         * @return
         */
        public static Attribute get(int wordID)
        {
            return trie.get(wordID);
        }

        /**
         * 获取词频
         *
         * @param term
         * @return
         */
        public static int getTermFrequency(String term)
        {
            Attribute attribute = get(term);
            if (attribute == null) return 0;
            return attribute.totalFrequency;
        }

        /**
         * 是否包含词语
         * @param key
         * @return
         */
        public static bool contains(String key)
        {
            return trie.get(key) != null;
        }

        /**
         * 核心词典中的词属性
         */
        public class Attribute //: Serializable
        {
            /**
             * 词性列表
             */
            public Nature[] nature;
            /**
             * 词性对应的词频
             */
            public int[] frequency;

            public int totalFrequency;

            // 几个预定义的变量

            //        public static Attribute NUMBER = new Attribute()

            public Attribute(int size)
            {
                nature = new Nature[size];
                frequency = new int[size];
            }

            public Attribute(Nature[] nature, int[] frequency)
            {
                this.nature = nature;
                this.frequency = frequency;
            }

            public Attribute(Nature nature, int frequency)
                    : this(1)
            {

                this.nature[0] = nature;
                this.frequency[0] = frequency;
                totalFrequency = frequency;
            }

            public Attribute(Nature[] nature, int[] frequency, int totalFrequency)
            {
                this.nature = nature;
                this.frequency = frequency;
                this.totalFrequency = totalFrequency;
            }

            /**
             * 使用单个词性，默认词频1000构造
             *
             * @param nature
             */
            public Attribute(Nature nature)
                    : this(nature, 1000)
            {

            }

            public static Attribute create(String natureWithFrequency)
            {
                try
                {
                    String[] param = natureWithFrequency.Split(' ');
                    int natureCount = param.Length / 2;
                    Attribute attribute = new Attribute(natureCount);
                    for (int i = 0; i < natureCount; ++i)
                    {
                        attribute.nature[i] = LexiconUtility.convertStringToNature(param[2 * i], null);
                        attribute.frequency[i] = int.Parse(param[1 + 2 * i]);
                        attribute.totalFrequency += attribute.frequency[i];
                    }
                    return attribute;
                }
                catch (Exception e)
                {
                    //logger.warning("使用字符串" + natureWithFrequency + "创建词条属性失败！" + TextUtility.exceptionToString(e));
                    return null;
                }
            }

            /**
             * 获取词性的词频
             *
             * @param nature 字符串词性
             * @return 词频
             * @deprecated 推荐使用Nature参数！
             */
            public int getNatureFrequency(String nature)
            {
                //            try
                //            {
                //                Nature pos = Enum.valueOf(Nature.class, nature);
                //            return getNatureFrequency(pos);
                //    }
                //        catch (IllegalArgumentException e)
                //        {
                //            return 0;
                //}
                return 0;
            }

            /**
             * 获取词性的词频
             *
             * @param nature 词性
             * @return 词频
             */
            public int getNatureFrequency(Nature nature)
            {
                int i = 0;
                foreach (Nature pos in this.nature)
                {
                    if (nature == pos)
                    {
                        return frequency[i];
                    }
                    ++i;
                }
                return 0;
            }

            /**
             * 是否有某个词性
             * @param nature
             * @return
             */
            public bool hasNature(Nature nature)
            {
                return getNatureFrequency(nature) > 0;
            }


            public String ToString()
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < nature.Length; ++i)
                {
                    sb.Append(nature[i]).Append(' ').Append(frequency[i]).Append(' ');
                }
                return sb.ToString();
            }
        }

        /**
         * 获取词语的ID
         * @param a
         * @return
         */
        public static int getWordID(String a)
        {
            return CoreDictionary.trie.exactMatchSearch(a);
        }
    }

}
