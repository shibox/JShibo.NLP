using JShibo.NLP.Corpus.Dictionary.Item;
using JShibo.NLP.Dictionary.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.NT
{
    /**
 * 一个好用的地名词典
 *
 * @author hankcs
 */
    public class NTDictionary : CommonDictionary<EnumItem<Corpus.Tag.NT>>
    {

        protected override EnumItem<Corpus.Tag.NT>[] onLoadValue(String path)
        {
            //EnumItem<Corpus.Tag.NT>[] valueArray = loadDat(path + ".value.dat");
            //if (valueArray != null)
            //{
            //    return valueArray;
            //}
            //List<EnumItem<Corpus.Tag.NT>> valueList = new LinkedList<EnumItem<NT>>();
            //try
            //{
            //    BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8"));
            //    String line;
            //    while ((line = br.readLine()) != null)
            //    {
            //        Map.Entry<String, Map.Entry<String, Integer>[]> args = EnumItem.create(line);
            //        EnumItem<NT> NSEnumItem = new EnumItem<NT>();
            //        for (Map.Entry<String, Integer> e : args.getValue())
            //        {
            //            NSEnumItem.labelMap.put(NT.valueOf(e.getKey()), e.getValue());
            //        }
            //        valueList.add(NSEnumItem);
            //    }
            //    br.close();
            //}
            //catch (Exception e)
            //{
            //    logger.warning("读取" + path + "失败" + e);
            //}
            //valueArray = valueList.toArray(new EnumItem[0]);
            //return valueArray;
            return null;
        }


        protected override bool onSaveValue(EnumItem<Corpus.Tag.NT>[] valueArray, String path)
        {
            return saveDat(path + ".value.dat", valueArray);
        }

        private EnumItem<Corpus.Tag.NT>[] loadDat(String path)
        {
            //byte[] bytes = IOUtil.readBytes(path);
            //if (bytes == null) return null;
            //NT[] values = NT.values();
            //int index = 0;
            //int size = ByteUtil.bytesHighFirstToInt(bytes, index);
            //index += 4;
            //EnumItem<NT>[] valueArray = new EnumItem[size];
            //for (int i = 0; i < size; ++i)
            //{
            //    int currentSize = ByteUtil.bytesHighFirstToInt(bytes, index);
            //    index += 4;
            //    EnumItem<NT> item = new EnumItem<NT>();
            //    for (int j = 0; j < currentSize; ++j)
            //    {
            //        NT tag = values[ByteUtil.bytesHighFirstToInt(bytes, index)];
            //        index += 4;
            //        int frequency = ByteUtil.bytesHighFirstToInt(bytes, index);
            //        index += 4;
            //        item.labelMap.put(tag, frequency);
            //    }
            //    valueArray[i] = item;
            //}
            //return valueArray;
            return null;
        }

        private bool saveDat(String path, EnumItem<Corpus.Tag.NT>[] valueArray)
        {
            //try
            //{
            //    DataOutputStream out = new DataOutputStream(new FileOutputStream(path));
            //    out.writeInt(valueArray.length);
            //    for (EnumItem<NT> item : valueArray)
            //    {
            //        out.writeInt(item.labelMap.size());
            //        for (Map.Entry<NT, Integer> entry : item.labelMap.entrySet())
            //        {
            //            out.writeInt(entry.getKey().ordinal());
            //            out.writeInt(entry.getValue());
            //        }
            //    }
            //    out.close();
            //}
            //catch (Exception e)
            //{
            //    logger.warning("保存失败" + e);
            //    return false;
            //}
            return true;
        }
    }
}
