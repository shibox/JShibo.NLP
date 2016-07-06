using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Corpus.Dictionary.Item
{
    /**
 * 对标签-频次的封装
 * @author hankcs
 */
    public class EnumItem<E> //: Enum<E>>
    {
        public Dictionary<E, int> labelMap;

        public EnumItem()
        {
            labelMap = new Dictionary<E, int>();
        }

        /**
         * 创建只有一个标签的条目
         * @param label
         * @param frequency
         */
        public EnumItem(E label, int frequency)
                : this()
        {
            labelMap.Add(label, frequency);
        }

        /**
         * 创建一个条目，其标签频次都是1，各标签由参数指定
         * @param labels
         */
        public EnumItem(params E[] labels)
                : this()
        {

            foreach (E label in labels)
            {
                labelMap.Add(label, 1);
            }
        }

        public void addLabel(E label)
        {
            int frequency = labelMap[label];
            if (frequency == null)
            {
                frequency = 1;
            }
            else
            {
                ++frequency;
            }

            labelMap.Add(label, frequency);
        }

        public void addLabel(E label, int frequency)
        {
            int innerFrequency = labelMap[label];
            if (innerFrequency == null)
            {
                innerFrequency = frequency;
            }
            else
            {
                innerFrequency += frequency;
            }

            labelMap.Add(label, innerFrequency);
        }

        public bool containsLabel(E label)
        {
            return labelMap.ContainsKey(label);
        }

        public int getFrequency(E label)
        {
            int frequency = labelMap[label];
            if (frequency == null) return 0;
            return frequency;
        }


        public override String ToString()
        {
            //        StringBuilder sb = new StringBuilder();
            //        List<KeyValuePair<E, int>> entries = new ArrayList<Map.Entry<E, Integer>>(labelMap.entrySet());
            //        Collections.sort(entries, new Comparator<Map.Entry<E, Integer>>()
            //        {
            //            @Override
            //            public int compare(Map.Entry<E, Integer> o1, Map.Entry<E, Integer> o2)
            //    {
            //        return -o1.getValue().compareTo(o2.getValue());
            //    }
            //});
            //        for (Map.Entry<E, Integer> entry : entries)
            //        {
            //            sb.append(entry.getKey());
            //            sb.append(' ');
            //            sb.append(entry.getValue());
            //            sb.append(' ');
            //        }
            //        return sb.toString();
            return string.Empty;
        }

        public static Dictionary<String, Dictionary<String, int>[]> create(String param)
        {
            if (param == null) return null;
            String[] array = param.Split(' ');
            return create(array);
        }


        public static Dictionary<String, Dictionary<String, int>[]> create(String[] param)
        {
            //if (param.Length % 2 == 0) return null;
            //int natureCount = (param.length - 1) / 2;
            //KeyValuePair<String, int>[] entries = (KeyValuePair<String, int>[])Array.newInstance(Map.Entry.class, natureCount);
            //    for (int i = 0; i<natureCount; ++i)
            //    {
            //        entries[i] = new AbstractMap.SimpleEntry<String, int>(param[1 + 2 * i], Integer.parseInt(param[2 + 2 * i]));
            //    }
            //    return new AbstractMap.SimpleEntry<String, KeyValuePair<String, int>[]>(param[0], entries);
            return null;
        }


    }
}
