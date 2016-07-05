using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Seg.NShort.Path
{
    /**
 * 原子分词节点
 * @author hankcs
 */
    public class AtomNode
    {
        public String sWord;
        public int nPOS;

        public AtomNode(String sWord, int nPOS)
        {
            this.sWord = sWord;
            this.nPOS = nPOS;
        }

        public AtomNode(char c, int nPOS)
        {
            //this.sWord = String.valueOf(c);
            this.sWord = c.ToString();
            this.nPOS = nPOS;
        }

        /**
         * 原子的词性
         * @return
         */
        public Nature getNature()
        {
            Nature nature = Nature.nz;
            switch (nPOS)
            {
                case Predefine.CT_CHINESE:
                    break;
                case Predefine.CT_INDEX:
                case Predefine.CT_NUM:
                    nature = Nature.m;
                    sWord = "未##数";
                    break;
                case Predefine.CT_DELIMITER:
                    nature = Nature.w;
                    break;
                case Predefine.CT_LETTER:
                    nature = Nature.nx;
                    sWord = "未##串";
                    break;
                case Predefine.CT_SINGLE://12021-2129-3121
                    //if (Predefine.PATTERN_FLOAT_NUMBER.matcher(sWord).matches())//匹配浮点数
                    if (Predefine.PATTERN_FLOAT_NUMBER.Match(sWord).Success)//匹配浮点数
                    {
                        nature = Nature.m;
                        sWord = "未##数";
                    }
                    else
                    {
                        nature = Nature.nx;
                        sWord = "未##串";
                    }
                    break;
                default:
                    break;
            }
            return nature;
        }

        
        public String toString()
        {
            return "AtomNode{" +
                    "word='" + sWord + '\'' +
                    ", nature=" + nPOS +
                    '}';
        }

        public static Vertex convert(String word, int type)
        {
            String name = word;
            Nature nature = Nature.n;
            int dValue = 1;
            switch (type)
            {
                case Predefine.CT_CHINESE:
                    break;
                case Predefine.CT_INDEX:
                case Predefine.CT_NUM:
                    nature = Nature.m;
                    word = "未##数";
                    break;
                case Predefine.CT_DELIMITER:
                    nature = Nature.w;
                    break;
                case Predefine.CT_LETTER:
                    nature = Nature.nx;
                    word = "未##串";
                    break;
                case Predefine.CT_SINGLE://12021-2129-3121
                                         //                if (Pattern.compile("^(-?\\d+)(\\.\\d+)?$").matcher(word).matches())//匹配浮点数
                                         //                {
                                         //                    nature = Nature.m;
                                         //                    word = "未##数";
                                         //                } else
                                         //                {
                    nature = Nature.nx;
                    word = "未##串";
                    //                }
                    break;
                default:
                    break;
            }

            return new Vertex(word, name, new CoreDictionary.Attribute(nature, dValue));
        }
    }
}
