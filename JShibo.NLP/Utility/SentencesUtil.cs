using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Seg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Utility
{
    /**
 * 文本断句
 *
 */
    public class SentencesUtil
    {
        /**
         * 将文本切割为句子
         * @param content
         * @return
         */
        public static List<String> toSentenceList(String content)
        {
            return toSentenceList(content.ToCharArray());
        }

        public static List<String> toSentenceList(char[] chars)
        {

            StringBuilder sb = new StringBuilder();

            List<String> sentences = new List<String>();

            for (int i = 0; i < chars.Length; ++i)
            {
                if (sb.Length == 0 && (Char.IsWhiteSpace(chars[i]) || chars[i] == ' '))
                {
                    continue;
                }

                sb.Append(chars[i]);
                switch (chars[i])
                {
                    case '.':
                        if (i < chars.Length - 1 && chars[i + 1] > 128)
                        {
                            insertIntoList(sb, sentences);
                            sb = new StringBuilder();
                        }
                        break;
                    case '…':
                        {
                            if (i < chars.Length - 1 && chars[i + 1] == '…')
                            {
                                sb.Append('…');
                                ++i;
                                insertIntoList(sb, sentences);
                                sb = new StringBuilder();
                            }
                        }
                        break;
                    case ' ':
                    case '	':
                    case ' ':
                    case '。':
                    case '，':
                    case ',':
                        insertIntoList(sb, sentences);
                        sb = new StringBuilder();
                        break;
                    case ';':
                    case '；':
                        insertIntoList(sb, sentences);
                        sb = new StringBuilder();
                        break;
                    case '!':
                    case '！':
                        insertIntoList(sb, sentences);
                        sb = new StringBuilder();
                        break;
                    case '?':
                    case '？':
                        insertIntoList(sb, sentences);
                        sb = new StringBuilder();
                        break;
                    case '\n':
                    case '\r':
                        insertIntoList(sb, sentences);
                        sb = new StringBuilder();
                        break;
                }
            }

            if (sb.Length > 0)
            {
                insertIntoList(sb, sentences);
            }

            return sentences;
        }

        private static void insertIntoList(StringBuilder sb, List<String> sentences)
        {
            String content = sb.ToString().Trim();
            if (content.Length > 0)
            {
                sentences.Add(content);
            }
        }

        /**
         * 句子中是否含有词性
         * @param sentence
         * @param nature
         * @return
         */
        public static bool hasNature(List<Term> sentence, Nature nature)
        {
            foreach (Term term in sentence)
            {
                if (term.nature == nature)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
