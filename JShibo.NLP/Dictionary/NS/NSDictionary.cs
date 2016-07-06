using JShibo.NLP.Corpus.Dictionary.Item;
using JShibo.NLP.Corpus.IO;
using JShibo.NLP.Dictionary.Common;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.NS
{
    /**
 * 一个好用的地名词典
 *
 * @author hankcs
 */
    public class NSDictionary : CommonDictionary<EnumItem<Corpus.Tag.NS>>
    {

        protected override EnumItem<Corpus.Tag.NS>[] onLoadValue(String path)
        {
            //EnumItem<Corpus.Tag.NS>[] valueArray = loadDat(path + ".value.dat");
            //if (valueArray != null)
            //{
            //    return valueArray;
            //}
            //List<EnumItem<Corpus.Tag.NS>> valueList = new LinkedList<EnumItem<NS>>();
            //try
            //{
            //    BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8"));
            //    String line;
            //    while ((line = br.readLine()) != null)
            //    {
            //        Map.Entry<String, Map.Entry<String, Integer>[]> args = EnumItem.create(line);
            //        EnumItem<NS> NSEnumItem = new EnumItem<NS>();
            //        for (Map.Entry<String, Integer> e : args.getValue())
            //        {
            //            NSEnumItem.labelMap.put(NS.valueOf(e.getKey()), e.getValue());
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


        protected override bool onSaveValue(EnumItem<Corpus.Tag.NS>[] valueArray, String path)
        {
            return saveDat(path + ".value.dat", valueArray);
        }

        private EnumItem<Corpus.Tag.NS>[] loadDat(String path)
        {
            //byte[] bytes = IOUtil.readBytes(path);
            //if (bytes == null) return null;
            //NS[] NSArray = NS.values();
            //int index = 0;
            //int size = ByteUtil.bytesHighFirstToInt(bytes, index);
            //index += 4;
            //EnumItem<NS>[] valueArray = new EnumItem[size];
            //for (int i = 0; i < size; ++i)
            //{
            //    int currentSize = ByteUtil.bytesHighFirstToInt(bytes, index);
            //    index += 4;
            //    EnumItem<NS> item = new EnumItem<NS>();
            //    for (int j = 0; j < currentSize; ++j)
            //    {
            //        NS NS = NSArray[ByteUtil.bytesHighFirstToInt(bytes, index)];
            //        index += 4;
            //        int frequency = ByteUtil.bytesHighFirstToInt(bytes, index);
            //        index += 4;
            //        item.labelMap.put(NS, frequency);
            //    }
            //    valueArray[i] = item;
            //}
            //return valueArray;

            return null;
        }

        private bool saveDat(String path, EnumItem<Corpus.Tag.NS>[] valueArray)
        {
            //try
            //{
            //    DataOutputStream out = new DataOutputStream(new FileOutputStream(path));
            //    out.writeInt(valueArray.length);
            //    for (EnumItem<NS> item : valueArray)
            //    {
            //        out.writeInt(item.labelMap.size());
            //        for (Map.Entry<NS, Integer> entry : item.labelMap.entrySet())
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
