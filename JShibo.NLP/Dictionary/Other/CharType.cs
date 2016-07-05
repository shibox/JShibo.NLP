using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.Other
{
    /**
 * 字符类型
 * @author hankcs
 */
    public class CharType
    {
        /**
         * 单字节
         */
        public const byte CT_SINGLE = 5;
        
        /**
         * 分隔符"!,.?()[]{}+=
         */
        public const byte CT_DELIMITER = CT_SINGLE + 1;

        /**
         * 中文字符
         */
        public const byte CT_CHINESE = CT_SINGLE + 2;

        /**
         * 字母
         */
        public const byte CT_LETTER = CT_SINGLE + 3;

        /**
         * 数字
         */
        public const byte CT_NUM = CT_SINGLE + 4;

        /**
         * 序号
         */
        public const byte CT_INDEX = CT_SINGLE + 5;

        /**
         * 其他
         */
        public const byte CT_OTHER = CT_SINGLE + 12;

        public static byte[] type;

        static CharType()
        {
            //type = new byte[65536];
            //logger.info("字符类型对应表开始加载 " + HanLP.Config.CharTypePath);
            //long start = System.currentTimeMillis();
            //ByteArray byteArray = ByteArray.createByteArray(HanLP.Config.CharTypePath);
            //if (byteArray == null)
            //{
            //    System.err.println("字符类型对应表加载失败：" + HanLP.Config.CharTypePath);
            //    System.exit(-1);
            //}
            //else
            //{
            //    while (byteArray.hasMore())
            //    {
            //        int b = byteArray.nextChar();
            //        int e = byteArray.nextChar();
            //        byte t = byteArray.nextByte();
            //        for (int i = b; i <= e; ++i)
            //        {
            //            type[i] = t;
            //        }
            //    }
            //    logger.info("字符类型对应表加载成功，耗时" + (System.currentTimeMillis() - start) + " ms");
            //}
        }

        /**
         * 获取字符的类型
         * @param c
         * @return
         */
        public static byte get(char c)
        {
            return type[(int)c];
        }
    }
}
