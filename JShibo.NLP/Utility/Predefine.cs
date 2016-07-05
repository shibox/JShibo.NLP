﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;

namespace JShibo.NLP.Utility
{
    /**
 * 一些预定义的静态全局变量
 */
    public class Predefine
    {
        /**
         * hanlp.properties的路径，一般情况下位于classpath目录中。
         * 但在某些极端情况下（不标准的Java虚拟机，用户缺乏相关知识等），允许将其设为绝对路径
         */
        public static String HANLP_PROPERTIES_PATH;
        public const double MIN_PROBABILITY = 1e-10;
        public const int CT_SENTENCE_BEGIN = 1;        //Sentence begin
        public const int CT_SENTENCE_END = 4;          //Sentence ending
        public const int CT_SINGLE = 5;                //SINGLE byte
        public const int CT_DELIMITER = CT_SINGLE + 1; //delimiter
        public const int CT_CHINESE = CT_SINGLE + 2;   //Chinese Char
        public const int CT_LETTER = CT_SINGLE + 3;    //HanYu Pinyin
        public const int CT_NUM = CT_SINGLE + 4;       //HanYu Pinyin
        public const int CT_INDEX = CT_SINGLE + 5;     //HanYu Pinyin
        public const int CT_OTHER = CT_SINGLE + 12;    //Other
                                                              /**
                                                               * 浮点数正则
                                                               */
        public static Regex PATTERN_FLOAT_NUMBER = new Regex("^(-?\\d+)(\\.\\d+)?$",RegexOptions.Compiled);

    public static String POSTFIX_SINGLE =
        "坝邦堡城池村单岛道堤店洞渡队峰府冈港阁宫沟国海号河湖环集江礁角街井郡坑口矿里岭楼路门盟庙弄牌派坡铺旗桥区渠泉山省市水寺塔台滩坛堂厅亭屯湾屋溪峡县线乡巷洋窑营屿园苑院闸寨站镇州庄族陂庵町";

        public static readonly String[] POSTFIX_MUTIPLE = {"半岛","草原","城市","大堤","大公国","大桥","地区",
        "帝国","渡槽","港口","高速公路","高原","公路","公园","共和国","谷地","广场",
        "国道","海峡","胡同","机场","集镇","教区","街道","口岸","码头","煤矿",
        "牧场","农场","盆地","平原","丘陵","群岛","沙漠","沙洲","山脉","山丘",
        "水库","隧道","特区","铁路","新村","雪峰","盐场","盐湖","渔场","直辖市",
        "自治区","自治县","自治州"};

        //Translation type
        public static int TT_ENGLISH = 0;
        public static int TT_RUSSIAN = 1;
        public static int TT_JAPANESE = 2;

        //Seperator type
        public static String SEPERATOR_C_SENTENCE = "。！？：；…";
        public static String SEPERATOR_C_SUB_SENTENCE = "、，（）“”‘’";
        public static String SEPERATOR_E_SENTENCE = "!?:;";
        public static String SEPERATOR_E_SUB_SENTENCE = ",()*'";
        //注释：原来程序为",()\042'"，"\042"为10进制42好ASC字符，为*
        public static String SEPERATOR_LINK = "\n\r 　";

        //Seperator between two words
        public static String WORD_SEGMENTER = "@";

        public static int CC_NUM = 6768;

        //The number of Chinese Char,including 5 empty position between 3756-3761
        public static int WORD_MAXLENGTH = 100;
        public static int WT_DELIMITER = 0;
        public static int WT_CHINESE = 1;
        public static int WT_OTHER = 2;

        public static int MAX_WORDS = 650;
        public static int MAX_SEGMENT_NUM = 10;

        public const int MAX_FREQUENCY = 25146057; // 现在总词频25146057
                                                          /**
                                                           * Smoothing 平滑因子
                                                           */
        public const double dTemp = (double)1 / MAX_FREQUENCY + 0.00001;
        /**
         * 平滑参数
         */
        public const double dSmoothingPara = 0.1;
        /**
         * 地址 ns
         */
        public const String TAG_PLACE = "未##地";
        /**
         * 句子的开始 begin
         */
        public const String TAG_BIGIN = "始##始";
        /**
         * 其它
         */
        public const String TAG_OTHER = "未##它";
        /**
         * 团体名词 nt
         */
        public const String TAG_GROUP = "未##团";
        /**
         * 数词 m
         */
        public const String TAG_NUMBER = "未##数";
        /**
         * 数量词 mq （现在觉得应该和数词同等处理，比如一个人和一人都是合理的）
         */
        public const String TAG_QUANTIFIER = "未##量";
        /**
         * 专有名词 nx
         */
        public const String TAG_PROPER = "未##专";
        /**
         * 时间 t
         */
        public const String TAG_TIME = "未##时";
        /**
         * 字符串 x
         */
        public const String TAG_CLUSTER = "未##串";
        /**
         * 结束 end
         */
        public const String TAG_END = "末##末";
        /**
         * 人名 nr
         */
        public const String TAG_PEOPLE = "未##人";

        /**
         * 日志组件
         */
        static ILog logger = LogManager.GetLogger("JShibo.NLP");

        static Predefine()
        {
            //logger.setLevel(Level.WARNING);
        }

        /**
         * trie树文件后缀名
         */
        public const String TRIE_EXT = ".trie.dat";
    /**
     * 值文件后缀名
     */
    public const String VALUE_EXT = ".value.dat";

    /**
     * 逆转后缀名
     */
    public const String REVERSE_EXT = ".reverse";

    /**
     * 二进制文件后缀
     */
    public const String BIN_EXT = ".bin";
}
}
