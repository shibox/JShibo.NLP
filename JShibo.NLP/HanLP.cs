﻿using JShibo.NLP.Dictionary.PY;
using JShibo.NLP.Seg;
using JShibo.NLP.Seg.Common;
using JShibo.NLP.Seg.Viterbi;
using JShibo.NLP.Summary;
using JShibo.NLP.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP
{
    /**
 * HanLP: Han Language Processing <br>
 * 汉语言处理包 <br>
 * 常用接口工具类
 *
 * @author hankcs
 */
    public class HanLP
    {
        /**
         * 库的全局配置，既可以用代码修改，也可以通过hanlp.properties配置（按照 变量名=值 的形式）
         */
        public static class Config
        {
            /**
             * 开发模式
             */
            public static bool DEBUG = false;
            /**
             * 核心词典路径
             */
            public static String CoreDictionaryPath = "data/dictionary/CoreNatureDictionary.txt";
            /**
             * 核心词典词性转移矩阵路径
             */
            public static String CoreDictionaryTransformMatrixDictionaryPath = "data/dictionary/CoreNatureDictionary.tr.txt";
            /**
             * 用户自定义词典路径
             */
            public static String[] CustomDictionaryPath = new String[] { "data/dictionary/custom/CustomDictionary.txt" };
            /**
             * 2元语法词典路径
             */
            public static String BiGramDictionaryPath = "data/dictionary/CoreNatureDictionary.ngram.txt";

            /**
             * 停用词词典路径
             */
            public static String CoreStopWordDictionaryPath = "data/dictionary/stopwords.txt";
            /**
             * 同义词词典路径
             */
            public static String CoreSynonymDictionaryDictionaryPath = "data/dictionary/synonym/CoreSynonym.txt";
            /**
             * 人名词典路径
             */
            public static String PersonDictionaryPath = "data/dictionary/person/nr.txt";
            /**
             * 人名词典转移矩阵路径
             */
            public static String PersonDictionaryTrPath = "data/dictionary/person/nr.tr.txt";
            /**
             * 地名词典路径
             */
            public static String PlaceDictionaryPath = "data/dictionary/place/ns.txt";
            /**
             * 地名词典转移矩阵路径
             */
            public static String PlaceDictionaryTrPath = "data/dictionary/place/ns.tr.txt";
            /**
             * 地名词典路径
             */
            public static String OrganizationDictionaryPath = "data/dictionary/organization/nt.txt";
            /**
             * 地名词典转移矩阵路径
             */
            public static String OrganizationDictionaryTrPath = "data/dictionary/organization/nt.tr.txt";
            /**
             * 繁简词典路径
             */
            public static String TraditionalChineseDictionaryPath = "data/dictionary/tc/TraditionalChinese.txt";
            /**
             * 声母韵母语调词典
             */
            public static String SYTDictionaryPath = "data/dictionary/pinyin/SYTDictionary.txt";

            /**
             * 拼音词典路径
             */
            public static String PinyinDictionaryPath = "data/dictionary/pinyin/pinyin.txt";

            /**
             * 音译人名词典
             */
            public static String TranslatedPersonDictionaryPath = "data/dictionary/person/nrf.txt";

            /**
             * 日本人名词典路径
             */
            public static String JapanesePersonDictionaryPath = "data/dictionary/person/nrj.txt";

            /**
             * 字符类型对应表
             */
            public static String CharTypePath = "data/dictionary/other/CharType.dat.yes";

            /**
             * 字符正规化表（全角转半角，繁体转简体）
             */
            public static String CharTablePath = "data/dictionary/other/CharTable.bin.yes";

            /**
             * 词-词性-依存关系模型
             */
            public static String WordNatureModelPath = "data/model/dependency/WordNature.txt";

            /**
             * 最大熵-依存关系模型
             */
            public static String MaxEntModelPath = "data/model/dependency/MaxEntModel.txt";
            /**
             * 神经网络依存模型路径
             */
            public static String NNParserModelPath = "data/model/dependency/NNParserModel.txt";
            /**
             * CRF分词模型
             */
            public static String CRFSegmentModelPath = "data/model/segment/CRFSegmentModel.txt";
            /**
             * HMM分词模型
             */
            public static String HMMSegmentModelPath = "data/model/segment/HMMSegmentModel.bin";
            /**
             * CRF依存模型
             */
            public static String CRFDependencyModelPath = "data/model/dependency/CRFDependencyModelMini.txt";
            /**
             * 分词结果是否展示词性
             */
            public static bool ShowTermNature = true;
            /**
             * 是否执行字符正规化（繁体->简体，全角->半角，大写->小写），切换配置后必须删CustomDictionary.txt.bin缓存
             */
            public static bool Normalization = false;

            static Config()
            {
                //                // 自动读取配置
                //                Properties p = new Properties();
                //                try
                //                {
                //                    ClassLoader loader = Thread.currentThread().getContextClassLoader();
                //                    if (loader == null)
                //                    {  // IKVM (v.0.44.0.5) doesn't set context classloader
                //                        loader = HanLP.Config.class.getClassLoader();
                //        }
                //        p.load(new InputStreamReader(Predefine.HANLP_PROPERTIES_PATH == null ?

                //                loader.getResourceAsStream("hanlp.properties") :
                //                        new FileInputStream(Predefine.HANLP_PROPERTIES_PATH)
                //                            , "UTF-8"));
                //                String root = p.getProperty("root", "").replaceAll("\\\\", "/");
                //                if (!root.endsWith("/")) root += "/";
                //                CoreDictionaryPath = root + p.getProperty("CoreDictionaryPath", CoreDictionaryPath);
                //                CoreDictionaryTransformMatrixDictionaryPath = root + p.getProperty("CoreDictionaryTransformMatrixDictionaryPath", CoreDictionaryTransformMatrixDictionaryPath);
                //                BiGramDictionaryPath = root + p.getProperty("BiGramDictionaryPath", BiGramDictionaryPath);
                //                CoreStopWordDictionaryPath = root + p.getProperty("CoreStopWordDictionaryPath", CoreStopWordDictionaryPath);
                //                CoreSynonymDictionaryDictionaryPath = root + p.getProperty("CoreSynonymDictionaryDictionaryPath", CoreSynonymDictionaryDictionaryPath);
                //                PersonDictionaryPath = root + p.getProperty("PersonDictionaryPath", PersonDictionaryPath);
                //                PersonDictionaryTrPath = root + p.getProperty("PersonDictionaryTrPath", PersonDictionaryTrPath);
                //                String[] pathArray = p.getProperty("CustomDictionaryPath", "dictionary/custom/CustomDictionary.txt").split(";");
                //        String prePath = root;
                //                for (int i = 0; i<pathArray.length; ++i)
                //                {
                //                    if (pathArray[i].startsWith(" "))
                //                    {
                //                        pathArray[i] = prePath + pathArray[i].trim();
                //    }
                //                    else
                //                    {
                //                        pathArray[i] = root + pathArray[i];
                //                        int lastSplash = pathArray[i].lastIndexOf('/');
                //                        if (lastSplash != -1)
                //                        {
                //                            prePath = pathArray[i].substring(0, lastSplash + 1);
                //}
                //                    }
                //                }
                //                CustomDictionaryPath = pathArray;
                //                TraditionalChineseDictionaryPath = root + p.getProperty("TraditionalChineseDictionaryPath", TraditionalChineseDictionaryPath);
                //                SYTDictionaryPath = root + p.getProperty("SYTDictionaryPath", SYTDictionaryPath);
                //                PinyinDictionaryPath = root + p.getProperty("PinyinDictionaryPath", PinyinDictionaryPath);
                //                TranslatedPersonDictionaryPath = root + p.getProperty("TranslatedPersonDictionaryPath", TranslatedPersonDictionaryPath);
                //                JapanesePersonDictionaryPath = root + p.getProperty("JapanesePersonDictionaryPath", JapanesePersonDictionaryPath);
                //                PlaceDictionaryPath = root + p.getProperty("PlaceDictionaryPath", PlaceDictionaryPath);
                //                PlaceDictionaryTrPath = root + p.getProperty("PlaceDictionaryTrPath", PlaceDictionaryTrPath);
                //                OrganizationDictionaryPath = root + p.getProperty("OrganizationDictionaryPath", OrganizationDictionaryPath);
                //                OrganizationDictionaryTrPath = root + p.getProperty("OrganizationDictionaryTrPath", OrganizationDictionaryTrPath);
                //                CharTypePath = root + p.getProperty("CharTypePath", CharTypePath);
                //                CharTablePath = root + p.getProperty("CharTablePath", CharTablePath);
                //                WordNatureModelPath = root + p.getProperty("WordNatureModelPath", WordNatureModelPath);
                //                MaxEntModelPath = root + p.getProperty("MaxEntModelPath", MaxEntModelPath);
                //                NNParserModelPath = root + p.getProperty("NNParserModelPath", NNParserModelPath);
                //                CRFSegmentModelPath = root + p.getProperty("CRFSegmentModelPath", CRFSegmentModelPath);
                //                CRFDependencyModelPath = root + p.getProperty("CRFDependencyModelPath", CRFDependencyModelPath);
                //                HMMSegmentModelPath = root + p.getProperty("HMMSegmentModelPath", HMMSegmentModelPath);
                //                ShowTermNature = "true".equals(p.getProperty("ShowTermNature", "true"));
                //                Normalization = "true".equals(p.getProperty("Normalization", "false"));
                //            }
                //            catch (Exception e)
                //            {
                //                StringBuilder sbInfo = new StringBuilder("========Tips========\n请将HanLP.properties放在下列目录：\n"); // 打印一些友好的tips
                //String classPath = (String)System.getProperties().get("java.class.path");
                //                if (classPath != null)
                //                {
                //                    for (String path : classPath.split(File.pathSeparator))
                //                    {
                //                        if (new File(path).isDirectory())
                //                        {
                //                            sbInfo.append(path).append('\n');
                //                        }
                //                    }
                //                }
                //                sbInfo.append("Web项目则请放到下列目录：\n" +
                //                                      "Webapp/WEB-INF/lib\n" +
                //                                      "Webapp/WEB-INF/classes\n" +
                //                                      "Appserver/lib\n" +
                //                                      "JRE/lib\n");
                //                sbInfo.append("并且编辑root=PARENT/path/to/your/data\n");
                //                sbInfo.append("现在HanLP将尝试从").append(System.getProperties().get("user.dir")).append("读取data……");
                //logger.severe("没有找到HanLP.properties，可能会导致找不到data\n" + sbInfo);
                //            }
            }

            /**
             * 开启调试模式(会降低性能)
             */
            public static void enableDebug()
            {
                enableDebug(true);
            }

            /**
             * 开启调试模式(会降低性能)
             * @param enable
             */
            public static void enableDebug(bool enable)
            {
                DEBUG = enable;
                if (DEBUG)
                {
                    //logger.setLevel(Level.ALL);
                }
                else
                {
                    //logger.setLevel(Level.OFF);
                }
            }
        }

        /**
         * 工具类，不需要生成实例
         */
        private HanLP() { }

        /**
         * 简转繁
         *
         * @param traditionalChineseString 繁体中文
         * @return 简体中文
         */
        //public static String convertToSimplifiedChinese(String traditionalChineseString)
        //{
        //    return TraditionalChineseDictionary.convertToSimplifiedChinese(traditionalChineseString.toCharArray());
        //}

        /**
         * 繁转简
         *
         * @param simplifiedChineseString 简体中文
         * @return 繁体中文
         */
        //public static String convertToTraditionalChinese(String simplifiedChineseString)
        //{
        //    return SimplifiedChineseDictionary.convertToTraditionalChinese(simplifiedChineseString.toCharArray());
        //}

        /**
         * 转化为拼音
         *
         * @param text 文本
         * @param separator 分隔符
         * @param remainNone 有些字没有拼音（如标点），是否保留它们（用none表示）
         * @return 一个字符串，由[拼音][分隔符][拼音]构成
         */
        //public static String convertToPinyinString(String text, String separator, bool remainNone)
        //{
        //    List<Pinyin> pinyinList = PinyinDictionary.convertToPinyin(text, remainNone);
        //    int length = pinyinList.Count;
        //    StringBuilder sb = new StringBuilder(length * (5 + separator.Length));
        //    int i = 1;
        //    foreach (Pinyin pinyin in pinyinList)
        //    {
        //        sb.Append(pinyin.getPinyinWithoutTone());
        //        if (i < length)
        //        {
        //            sb.Append(separator);
        //        }
        //        ++i;
        //    }
        //    return sb.ToString();
        //}

        /**
         * 转化为拼音
         *
         * @param text 待解析的文本
         * @return 一个拼音列表
         */
        //public static List<Pinyin> convertToPinyinList(String text)
        //{
        //    return PinyinDictionary.convertToPinyin(text);
        //}

        /**
         * 转化为拼音（首字母）
         *
         * @param text 文本
         * @param separator 分隔符
         * @param remainNone 有些字没有拼音（如标点），是否保留它们（用none表示）
         * @return 一个字符串，由[首字母][分隔符][首字母]构成
         */
        //public static String convertToPinyinFirstCharString(String text, String separator, bool remainNone)
        //{
        //    List<Pinyin> pinyinList = PinyinDictionary.convertToPinyin(text, remainNone);
        //    int length = pinyinList.Count;
        //    StringBuilder sb = new StringBuilder(length * (1 + separator.Length));
        //    int i = 1;
        //    foreach (Pinyin pinyin in pinyinList)
        //    {
        //        sb.Append(pinyin.getFirstChar());
        //        if (i < length)
        //        {
        //            sb.Append(separator);
        //        }
        //        ++i;
        //    }
        //    return sb.ToString();
        //}

        /**
         * 分词
         *
         * @param text 文本
         * @return 切分后的单词
         */
        public static List<Term> segment(String text)
        {
            return StandardTokenizer.segment(text.ToCharArray());
        }

        /**
         * 创建一个分词器<br>
         * 这是一个工厂方法<br>
         * 与直接new一个分词器相比，使用本方法的好处是，以后HanLP升级了，总能用上最合适的分词器
         * @return 一个分词器
         */
        public static Segment newSegment()
        {
            return new ViterbiSegment();   // Viterbi分词器是目前效率和效果的最佳平衡
        }

        /**
         * 依存文法分析
         * @param sentence 待分析的句子
         * @return CoNLL格式的依存关系树
         */
        //public static CoNLLSentence parseDependency(String sentence)
        //{
        //    return NeuralNetworkDependencyParser.compute(sentence);
        //}

        /**
         * 提取短语
         * @param text 文本
         * @param size 需要多少个短语
         * @return 一个短语列表，大小 <= size
         */
        //public static List<String> extractPhrase(String text, int size)
        //{
        //    IPhraseExtractor extractor = new MutualInformationEntropyPhraseExtractor();
        //    return extractor.extractPhrase(text, size);
        //}

        /**
         * 提取关键词
         * @param document 文档内容
         * @param size 希望提取几个关键词
         * @return 一个列表
         */
        //public static List<String> extractKeyword(String document, int size)
        //{
        //    return TextRankKeyword.getKeywordList(document, size);
        //}

        /**
         * 自动摘要
         * @param document 目标文档
         * @param size 需要的关键句的个数
         * @return 关键句列表
         */
        public static List<String> extractSummary(String document, int size)
        {
            return TextRankSentence.getTopSentenceList(document, size);
        }

        /**
         * 自动摘要
         * @param document 目标文档
         * @param max_length 需要摘要的长度
         * @return 摘要文本
         */
        public static String getSummary(String document, int max_length)
        {
            // Parameter size in this method refers to the string length of the summary required;
            // The actual length of the summary generated may be short than the required length, but never longer;
            return TextRankSentence.getSummary(document, max_length);
        }
    }
}
