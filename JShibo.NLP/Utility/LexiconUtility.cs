using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Dictionary;
using JShibo.NLP.Seg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Utility
{
    /**
 * 跟词语与词性有关的工具类，可以全局动态修改HanLP内部词库
 *
 * @author hankcs
 */
    public class LexiconUtility
    {
        /**
         * 从HanLP的词库中提取某个单词的属性（包括核心词典和用户词典）
         *
         * @param word 单词
         * @return 包含词性与频次的信息
         */
        public static CoreDictionary.Attribute getAttribute(String word)
        {
            CoreDictionary.Attribute attribute = CoreDictionary.get(word);
            if (attribute != null) return attribute;
            return CustomDictionary.get(word);
        }

        /**
         * 从HanLP的词库中提取某个单词的属性（包括核心词典和用户词典）
         *
         * @param term 单词
         * @return 包含词性与频次的信息
         */
        public static CoreDictionary.Attribute getAttribute(Term term)
        {
            return getAttribute(term.word);
        }

        /**
         * 获取某个单词的词频
         * @param word
         * @return
         */
        public static int getFrequency(String word)
        {
            CoreDictionary.Attribute attribute = getAttribute(word);
            if (attribute == null) return 0;
            return attribute.totalFrequency;
        }

        /**
         * 设置某个单词的属性
         * @param word
         * @param attribute
         * @return
         */
        public static bool setAttribute(String word, CoreDictionary.Attribute attribute)
        {
            if (attribute == null) return false;

            if (CoreDictionary.trie.set(word, attribute)) return true;
            if (CustomDictionary.dat.set(word, attribute)) return true;
            CustomDictionary.trie.put(word, attribute);
            return true;
        }

        /**
         * 设置某个单词的属性
         * @param word
         * @param natures
         * @return
         */
        public static bool setAttribute(String word,params Nature[] natures)
        {
            if (natures == null) return false;

            CoreDictionary.Attribute attribute = new CoreDictionary.Attribute(natures, new int[natures.Length]);
            Arrays.fill(attribute.frequency, 1);

            return setAttribute(word, attribute);
        }

        /**
         * 设置某个单词的属性
         * @param word
         * @param natures
         * @return
         */
        public static bool setAttribute(String word,params String[] natures)
        {
            if (natures == null) return false;

            Nature[] natureArray = new Nature[natures.Length];
            for (int i = 0; i < natureArray.Length; i++)
            {
                natureArray[i] = Nature.create(natures[i]);
            }

            return setAttribute(word, natureArray);
        }


        /**
         * 设置某个单词的属性
         * @param word
         * @param natureWithFrequency
         * @return
         */
        public static bool setAttribute(String word, String natureWithFrequency)
        {
            CoreDictionary.Attribute attribute = CoreDictionary.Attribute.create(natureWithFrequency);
            return setAttribute(word, attribute);
        }

        /**
         * 将字符串词性转为Enum词性
         * @param name 词性名称
         * @param customNatureCollector 一个收集集合
         * @return 转换结果
         */
        public static Nature convertStringToNature(String name, LinkedHashSet<Nature> customNatureCollector)
        {
            try
            {
                return Nature.valueOf(name);
            }
            catch (Exception e)
            {
                Nature nature = CustomNatureUtility.addNature(name);
                if (customNatureCollector != null) customNatureCollector.add(nature);
                return nature;
            }
        }
    }

}
