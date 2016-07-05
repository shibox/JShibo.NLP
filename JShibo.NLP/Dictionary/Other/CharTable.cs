using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.Other
{
    /**
 * 字符正规化表
 * @author hankcs
 */
    public class CharTable
    {
        /**
         * 正规化使用的对应表
         */
        public static char[] CONVERT;

        static CharTable()
        {
            //        long start = System.currentTimeMillis();
            //        try
            //        {
            //            ObjectInputStream in = new ObjectInputStream(new FileInputStream(HanLP.Config.CharTablePath));
            //            CONVERT = (char[]) in.readObject();
            //            in.close();
            //    }
            //        catch (Exception e)
            //        {
            //            logger.severe("字符正规化表加载失败，原因如下：");
            //            e.printStackTrace();
            //            System.exit(-1);
            //        }

            //logger.info("字符正规化表加载成功：" + (System.currentTimeMillis() - start) + " ms");
        }

        /**
         * 将一个字符正规化
         * @param c 字符
         * @return 正规化后的字符
         */
        public static char convert(char c)
        {
            return CONVERT[c];
        }

        public static char[] convert(char[] charArray)
        {
            char[] result = new char[charArray.Length];
            for (int i = 0; i < charArray.Length; i++)
            {
                result[i] = CONVERT[charArray[i]];
            }

            return result;
        }

        public static String convert(String charArray)
        {
            //assert charArray != null;
            char[] result = new char[charArray.Length];
            for (int i = 0; i < charArray.Length; i++)
            {
                result[i] = CONVERT[charArray[i]];
            }

            return new String(result);
        }

        /**
         * 正规化一些字符（原地正规化）
         * @param charArray 字符
         */
        public static void normalization(char[] charArray)
        {
            //assert charArray != null;
            for (int i = 0; i < charArray.Length; i++)
            {
                charArray[i] = CONVERT[charArray[i]];
            }
        }
    }
}
