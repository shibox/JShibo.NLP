using JShibo.NLP.Corpus.Dictionary.Item;
using JShibo.NLP.Corpus.IO;
using JShibo.NLP.Dictionary.Common;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary.NR
{
    /**
 * 一个好用的人名词典
 *
 * @author hankcs
 */
    public class NRDictionary : CommonDictionary<EnumItem<Corpus.Tag.NR>>
{
        
    protected override EnumItem<Corpus.Tag.NR>[] onLoadValue(String path)
    {
            //EnumItem<Corpus.Tag.NR>[] valueArray = loadDat(path + ".value.dat");
            //if (valueArray != null)
            //{
            //    return valueArray;
            //}
            //List<EnumItem<Corpus.Tag.NR>> valueList = new LinkedList<EnumItem<NR>>();
            //String line = null;
            //try
            //{
            //    BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8"));
            //    while ((line = br.readLine()) != null)
            //    {
            //        Map.Entry<String, Map.Entry<String, Integer>[]> args = EnumItem.create(line);
            //        EnumItem<NR> nrEnumItem = new EnumItem<NR>();
            //        for (Map.Entry<String, Integer> e : args.getValue())
            //        {
            //            nrEnumItem.labelMap.put(NR.valueOf(e.getKey()), e.getValue());
            //        }
            //        valueList.add(nrEnumItem);
            //    }
            //    br.close();
            //}
            //catch (Exception e)
            //{
            //    logger.severe("读取" + path + "失败[" + e + "]\n该词典这一行格式不对：" + line);
            //    return null;
            //}
            //valueArray = valueList.toArray(new EnumItem[0]);
            //return valueArray;
            return null;
    }

    
    protected override bool onSaveValue(EnumItem<Corpus.Tag.NR>[] valueArray, String path)
    {
        return saveDat(path + ".value.dat", valueArray);
    }

    private EnumItem<Corpus.Tag.NR>[] loadDat(String path)
    {
        byte[] bytes = IOUtil.readBytes(path);
        if (bytes == null) return null;
            //Corpus.Tag.NR[] nrArray = Corpus.Tag.NR.values();
        int index = 0;
        int size = ByteUtil.bytesHighFirstToInt(bytes, index);
        index += 4;
            EnumItem<Corpus.Tag.NR>[] valueArray = new EnumItem<Corpus.Tag.NR>[size];
        for (int i = 0; i < size; ++i)
        {
            int currentSize = ByteUtil.bytesHighFirstToInt(bytes, index);
            index += 4;
            EnumItem<Corpus.Tag.NR> item = new EnumItem<Corpus.Tag.NR>();
            for (int j = 0; j < currentSize; ++j)
            {
                    //Corpus.Tag.NR nr = nrArray[ByteUtil.bytesHighFirstToInt(bytes, index)];
                index += 4;
                int frequency = ByteUtil.bytesHighFirstToInt(bytes, index);
                index += 4;
                //item.labelMap.Add(nr, frequency);
            }
            valueArray[i] = item;
        }
        return valueArray;
    }

    private bool saveDat(String path, EnumItem<Corpus.Tag.NR>[] valueArray)
    {
        //try
        //{
        //    DataOutputStream out = new DataOutputStream(new FileOutputStream(path));
        //    out.writeInt(valueArray.length);
        //    for (EnumItem<NR> item : valueArray)
        //    {
        //        out.writeInt(item.labelMap.size());
        //        for (Map.Entry<NR, Integer> entry : item.labelMap.entrySet())
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
