using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JShibo.NLP.Utility
{
    /**
 * 文本工具类
 */
    public class TextUtility
    {

        /**
         * 单字节
         */
        public static int CT_SINGLE = 5;// SINGLE byte

        /**
         * 分隔符"!,.?()[]{}+=
         */
        public static int CT_DELIMITER = CT_SINGLE + 1;// delimiter

        /**
         * 中文字符
         */
        public static int CT_CHINESE = CT_SINGLE + 2;// Chinese Char

        /**
         * 字母
         */
        public static int CT_LETTER = CT_SINGLE + 3;// HanYu Pinyin

        /**
         * 数字
         */
        public static int CT_NUM = CT_SINGLE + 4;// HanYu Pinyin

        /**
         * 序号
         */
        public static int CT_INDEX = CT_SINGLE + 5;// HanYu Pinyin

        /**
         * 其他
         */
        public static int CT_OTHER = CT_SINGLE + 12;// Other

        public static int charType(char c)
        {
            return charType(c.ToString());
        }

        /**
         * 判断字符类型
         * @param str
         * @return
         */
        public static int charType(String str)
        {
            if (str != null && str.Length > 0)
            {
                if ("零○〇一二两三四五六七八九十廿百千万亿壹贰叁肆伍陆柒捌玖拾佰仟".Contains(str)) return CT_NUM;
                byte[] b;
                try
                {
                    b = Encoding.GetEncoding("GBK").GetBytes(str);
                }
                catch (Exception e)
                {
                    b = Encoding.UTF8.GetBytes(str);
                    throw;
                    //e.printStackTrace();
                }
                byte b1 = b[0];
                byte b2 = (byte)(b.Length > 1 ? b[1] : 0);
                int ub1 = getUnsigned(b1);
                int ub2 = getUnsigned(b2);
                if (ub1 < 128)
                {
                    if (' ' == b1) return CT_OTHER;
                    if ("*\"!,.?()[]{}+=/\\;:|".IndexOf((char)b1) != -1)
                        return CT_DELIMITER;
                    if ("0123456789".IndexOf((char)b1) != -1)
                        return CT_NUM;
                    return CT_SINGLE;
                }
                else if (ub1 == 162)
                    return CT_INDEX;
                else if (ub1 == 163 && ub2 > 175 && ub2 < 186)
                    return CT_NUM;
                else if (ub1 == 163
                        && (ub2 >= 193 && ub2 <= 218 || ub2 >= 225
                        && ub2 <= 250))
                    return CT_LETTER;
                else if (ub1 == 161 || ub1 == 163)
                    return CT_DELIMITER;
                else if (ub1 >= 176 && ub1 <= 247)
                    return CT_CHINESE;

            }
            return CT_OTHER;
        }

        /**
         * 是否全是中文
         * @param str
         * @return
         */
        public static bool isAllChinese(String str)
        {
            //return str.matches("[\\u4E00-\\u9FA5]+");
            return Regex.IsMatch(str, "[\\u4E00-\\u9FA5]+");
        }
        /**
         * 是否全部不是中文
         * @param sString
         * @return
         */
        public static bool isAllNonChinese(byte[] sString)
        {
            int nLen = sString.Length;
            int i = 0;

            while (i < nLen)
            {
                if (getUnsigned(sString[i]) < 248 && getUnsigned(sString[i]) > 175)
                    return false;
                if (sString[i] < 0)
                    i += 2;
                else
                    i += 1;
            }
            return true;
        }

        /**
         * 是否全是单字节
         * @param str
         * @return
         */
        public static bool isAllSingleByte(String str)
        {
            if (str != null)
            {
                int len = str.Length;
                int i = 0;
                byte[] b;
                try
                {
                    b = Encoding.GetEncoding("GBK").GetBytes(str);
                }
                catch (Exception e)
                {
                    //e.printStackTrace();
                    b = Encoding.UTF8.GetBytes(str);
                }
                while (i < len && b[i] < 128)
                {
                    i++;
                }
                if (i < len)
                    return false;
                return true;
            }
            return false;
        }

        /**
         * 把表示数字含义的字符串转你成整形
         *
         * @param str 要转换的字符串
         * @return 如果是有意义的整数，则返回此整数值。否则，返回-1。
         */
        public static int cint(String str)
        {
            if (str != null)
                try
                {
                    int i = int.Parse(str);
                    return i;
                }
                catch (Exception e)
                {

                }

            return -1;
        }
        /**
         * 是否全是数字
         * @param str
         * @return
         */
        public static bool isAllNum(String str)
        {

            if (str != null)
            {
                int i = 0;
                String temp = str + " ";
                // 判断开头是否是+-之类的符号
                if ("±+—-＋".IndexOf(temp.Substring(0, 1)) != -1)
                    i++;
                /** 如果是全角的０１２３４５６７８９ 字符* */
                while (i < str.Length && "０１２３４５６７８９".IndexOf(str.Substring(i, i + 1)) != -1)
                    i++;

                // Get middle delimiter such as .
                if (i < str.Length)
                {
                    String s = str.Substring(i, i + 1);
                    if ("∶·．／".IndexOf(s) != -1 || ".".Equals(s) || "/".Equals(s))
                    {// 98．1％
                        i++;
                        while (i + 1 < str.Length && "０１２３４５６７８９".IndexOf(str.Substring(i + 1, i + 2)) != -1)

                            i++;
                    }
                }

                if (i >= str.Length)
                    return true;

                while (i < str.Length && cint(str.Substring(i, i + 1)) >= 0
                        && cint(str.Substring(i, i + 1)) <= 9)
                    i++;
                // Get middle delimiter such as .
                if (i < str.Length)
                {
                    String s = str.Substring(i, i + 1);
                    if ("∶·．／".IndexOf(s) != -1 || ".".Equals(s) || "/".Equals(s))
                    {// 98．1％
                        i++;
                        while (i + 1 < str.Length && "0123456789".IndexOf(str.Substring(i + 1, i + 2)) != -1)
                            i++;
                    }
                }

                if (i < str.Length)
                {

                    if ("百千万亿佰仟％‰".IndexOf(str.Substring(i, i + 1)) == -1
                            && !"%".Equals(str.Substring(i, i + 1)))
                        i--;
                }
                if (i >= str.Length)
                    return true;
            }
            return false;
        }
        /**
         * 是否全是序号
         * @param sString
         * @return
         */
        public static bool isAllIndex(byte[] sString)
        {
            int nLen = sString.Length;
            int i = 0;

            while (i < nLen - 1 && getUnsigned(sString[i]) == 162)
            {
                i += 2;
            }
            if (i >= nLen)
                return true;
            while (i < nLen && (sString[i] > 'A' - 1 && sString[i] < 'Z' + 1)
                    || (sString[i] > 'a' - 1 && sString[i] < 'z' + 1))
            {// single
             // byte
             // number
             // char
                i += 1;
            }

            if (i < nLen)
                return false;
            return true;

        }

        /**
         * 是否全为英文
         *
         * @param text
         * @return
         */
        public static bool isAllLetter(String text)
        {
            for (int i = 0; i < text.Length; ++i)
            {
                char c = text[i];
                if ((((c < 'a' || c > 'z')) && ((c < 'A' || c > 'Z'))))
                {
                    return false;
                }
            }

            return true;
        }

        /**
         * 是否全为英文或字母
         *
         * @param text
         * @return
         */
        public static bool isAllLetterOrNum(String text)
        {
            for (int i = 0; i < text.Length; ++i)
            {
                char c = text[i];
                if ((((c < 'a' || c > 'z')) && ((c < 'A' || c > 'Z')) && ((c < '0' || c > '9'))))
                {
                    return false;
                }
            }

            return true;
        }

        /**
         * 是否全是分隔符
         * @param sString
         * @return
         */
        public static bool isAllDelimiter(byte[] sString)
        {
            int nLen = sString.Length;
            int i = 0;

            while (i < nLen - 1 && (getUnsigned(sString[i]) == 161 || getUnsigned(sString[i]) == 163))
            {
                i += 2;
            }
            if (i < nLen)
                return false;
            return true;
        }

        /**
         * 是否全是中国数字
         * @param word
         * @return
         */
        public static bool isAllChineseNum(String word)
        {// 百分之五点六的人早上八点十八分起床

            String chineseNum = "零○一二两三四五六七八九十廿百千万亿壹贰叁肆伍陆柒捌玖拾佰仟∶·．／点";//
            String prefix = "几数第上成";

            if (word != null)
            {
                String temp = word + " ";
                for (int i = 0; i < word.Length; i++)
                {

                    if (temp.IndexOf("分之", i) != -1)// 百分之五
                    {
                        i += 2;
                        continue;
                    }

                    String tchar = temp.Substring(i, i + 1);
                    if (chineseNum.IndexOf(tchar) == -1 && (i != 0 || prefix.IndexOf(tchar) == -1))
                        return false;
                }
                return true;
            }

            return false;
        }


        /**
         * 得到字符集的字符在字符串中出现的次数
         *
         * @param charSet
         * @param word
         * @return
         */
        public static int getCharCount(String charSet, String word)
        {
            int nCount = 0;

            if (word != null)
            {
                String temp = word + " ";
                for (int i = 0; i < word.Length; i++)
                {
                    String s = temp.Substring(i, i + 1);
                    if (charSet.IndexOf(s) != -1)
                        nCount++;
                }
            }

            return nCount;
        }


        /**
         * 获取字节对应的无符号整型数
         *
         * @param b
         * @return
         */
        public static int getUnsigned(byte b)
        {
            if (b > 0)
                return (int)b;
            else
                return (b & 0x7F + 128);
        }

        /**
         * 判断字符串是否是年份
         *
         * @param snum
         * @return
         */
        public static bool isYearTime(String snum)
        {
            if (snum != null)
            {
                int len = snum.Length;
                String first = snum.Substring(0, 1);

                // 1992年, 98年,06年
                if (isAllSingleByte(snum)
                        && (len == 4 || len == 2 && (cint(first) > 4 || cint(first) == 0)))
                    return true;
                if (isAllNum(snum) && (len >= 6 || len == 4 && "０５６７８９".IndexOf(first) != -1))
                    return true;
                if (getCharCount("零○一二三四五六七八九壹贰叁肆伍陆柒捌玖", snum) == len && len >= 2)
                    return true;
                if (len == 4 && getCharCount("千仟零○", snum) == 2)// 二仟零二年
                    return true;
                if (len == 1 && getCharCount("千仟", snum) == 1)
                    return true;
                if (len == 2 && getCharCount("甲乙丙丁戊己庚辛壬癸", snum) == 1
                        && getCharCount("子丑寅卯辰巳午未申酉戌亥", snum.Substring(1)) == 1)
                    return true;
            }
            return false;
        }

        /**
         * 判断一个字符串的所有字符是否在另一个字符串集合中
         *
         * @param aggr 字符串集合
         * @param str  需要判断的字符串
         * @return
         */
        public static bool isInAggregate(String aggr, String str)
        {
            if (aggr != null && str != null)
            {
                str += "1";
                for (int i = 0; i < str.Length; i++)
                {
                    String s = str.Substring(i, i + 1);
                    if (aggr.IndexOf(s) == -1)
                        return false;
                }
                return true;
            }

            return false;
        }

        /**
         * 判断该字符串是否是半角字符
         *
         * @param str
         * @return
         */
        public static bool isDBCCase(String str)
        {
            if (str != null)
            {
                str += " ";
                for (int i = 0; i < str.Length; i++)
                {
                    String s = str.Substring(i, i + 1);
                    int length = 0;
                    try
                    {
                        length = Encoding.GetEncoding("GBK").GetBytes(s).Length;
                    }
                    catch (Exception e)
                    {
                        //e.printStackTrace();
                        length = Encoding.UTF8.GetBytes(s).Length;
                    }
                    if (length != 1)
                        return false;
                }

                return true;
            }

            return false;
        }

        /**
         * 判断该字符串是否是全角字符
         *
         * @param str
         * @return
         */
        public static bool isSBCCase(String str)
        {
            if (str != null)
            {
                str += " ";
                for (int i = 0; i < str.Length; i++)
                {
                    String s = str.Substring(i, i + 1);
                    int length = 0;
                    try
                    {
                        length = Encoding.GetEncoding("GBK").GetBytes(s).Length;
                    }
                    catch (Exception e)
                    {
                        //e.printStackTrace();
                        length = Encoding.UTF8.GetBytes(s).Length;
                    }
                    if (length != 2)
                        return false;
                }

                return true;
            }

            return false;
        }

        /**
         * 判断是否是一个连字符（分隔符）
         *
         * @param str
         * @return
         */
        public static bool isDelimiter(String str)
        {
            if (str != null && ("-".Equals(str) || "－".Equals(str)))
                return true;
            else
                return false;
        }

        public static bool isUnknownWord(String word)
        {
            if (word != null && word.IndexOf("未##") == 0)
                return true;
            else
                return false;
        }

        /**
         * 防止频率为0发生除零错误
         *
         * @param frequency
         * @return
         */
        public static double nonZero(double frequency)
        {
            if (frequency == 0) return 1e-3;

            return frequency;
        }

        /**
         * 转换long型为char数组
         *
         * @param x
         */
        public static char[] long2char(long x)
        {
            char[] c = new char[4];
            c[0] = (char)(x >> 48);
            c[1] = (char)(x >> 32);
            c[2] = (char)(x >> 16);
            c[3] = (char)(x);
            return c;
        }

        /**
         * 转换long类型为string
         *
         * @param x
         * @return
         */
        public static String long2String(long x)
        {
            char[] cArray = long2char(x);
            StringBuilder sbResult = new StringBuilder(cArray.Length);
            foreach (char c in cArray)
            {
                sbResult.Append(c);
            }
            return sbResult.ToString();
        }

        /**
         * 将异常转为字符串
         *
         * @param e
         * @return
         */
        public static String exceptionToString(Exception e)
        {
            //StringWriter sw = new StringWriter();
            //PrintWriter pw = new PrintWriter(sw);
            //e.printStackTrace(pw);
            //return sw.toString();
            return e.ToString();
        }

        /**
         * 判断某个字符是否为汉字
         *
         * @param c 需要判断的字符
         * @return 是汉字返回true，否则返回false
         */
        public static bool isChinese(char c)
        {
            String regex = "[\\u4e00-\\u9fa5]";
            return Regex.IsMatch(c.ToString(), regex);
        }

        /**
         * 统计 keyword 在 srcText 中的出现次数
         *
         * @param keyword
         * @param srcText
         * @return
         */
        public static int count(String keyword, String srcText)
        {
            int count = 0;
            int leng = srcText.Length;
            int j = 0;
            for (int i = 0; i < leng; i++)
            {
                if (srcText[i] == keyword[j])
                {
                    j++;
                    if (j == keyword.Length)
                    {
                        count++;
                        j = 0;
                    }
                }
                else
                {
                    i = i - j;// should rollback when not match
                    j = 0;
                }
            }

            return count;
        }

        /**
         * 简单好用的写String方式
         *
         * @param s
         * @param out
         * @throws IOException
         */
        public static void writeString(String s, BinaryWriter o)//, DataOutputStream out)
        {
    //    out.writeInt(s.length());
    //    for (char c : s.toCharArray())
    //    {
    //        out.writeChar(c);
    //}
}

/**
 * 判断字符串是否为空（null和空格）
 *
 * @param cs
 * @return
 */
public static bool isBlank(char[] cs)
{
            int strLen = 0;
    if (cs == null || (strLen = cs.Length) == 0)
    {
        return true;
    }
    for (int i = 0; i < strLen; i++)
    {
        if (!Char.IsWhiteSpace(cs[i]))
        {
            return false;
        }
    }
    return true;
}

public static String join(String delimiter, List<String> stringCollection)
{
    StringBuilder sb = new StringBuilder(stringCollection.Count * (16 + delimiter.Length));
    foreach (String str in stringCollection)
    {
        sb.Append(str).Append(delimiter);
    }

    return sb.ToString();
}
}

}
