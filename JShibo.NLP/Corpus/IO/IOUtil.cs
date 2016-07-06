using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Dictionary;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Corpus.IO
{
    /**
 * 一些常用的IO操作
 *
 * @author hankcs
 */
    public class IOUtil
    {
        /**
         * 序列化对象
         *
         * @param o
         * @param path
         * @return
         */
        public static bool saveObjectTo(Object o, String path)
        {
            try
            {
                //ObjectOutputStream oos = new ObjectOutputStream(new FileOutputStream(path));
                //oos.writeObject(o);
                //oos.close();
            }
            catch (IOException e)
            {
                //logger.warning("在保存对象" + o + "到" + path + "时发生异常" + e);
                return false;
            }

            return true;
        }

        /**
         * 反序列化对象
         *
         * @param path
         * @return
         */
        public static Object readObjectFrom(String path)
        {
            //ObjectInputStream ois = null;
            //try
            //{
            //    ois = new ObjectInputStream(new FileInputStream(path));
            //    Object o = ois.readObject();
            //    ois.close();
            //    return o;
            //}
            //catch (Exception e)
            //{
            //    logger.warning("在从" + path + "读取对象时发生异常" + e);
            //}

            return null;
        }

        /**
         * 一次性读入纯文本
         *
         * @param path
         * @return
         */
        public static String readTxt(String path)
        {
            //if (path == null) return null;
            //File file = new File(path);
            //Long fileLength = file.length();
            //byte[] fileContent = new byte[fileLength.intValue()];
            //try
            //{
            //    FileInputStream in = new FileInputStream(file);
            //in.read(fileContent);
            //in.close();
            //}
            //catch (FileNotFoundException e)
            //{
            //    logger.warning("找不到" + path + e);
            //    return null;
            //}
            //catch (IOException e)
            //{
            //    logger.warning("读取" + path + "发生IO异常" + e);
            //    return null;
            //}

            //return new String(fileContent, Charset.forName("UTF-8"));

            return string.Empty;
        }

        public static List<String[]> readCsv(String path)
        {
            List<String[]> resultList = new List<String[]>();
            List<String> lineList = readLineList(path);
            foreach (String line in lineList)
            {
                resultList.Add(line.Split(','));
            }
            return resultList;
        }

        /**
         * 快速保存
         *
         * @param path
         * @param content
         * @return
         */
        public static bool saveTxt(String path, String content)
        {
            //try
            //{
            //    FileChannel fc = new FileOutputStream(path).getChannel();
            //    fc.write(ByteBuffer.wrap(content.getBytes()));
            //    fc.close();
            //}
            //catch (Exception e)
            //{
            //    logger.throwing("IOUtil", "saveTxt", e);
            //    logger.warning("IOUtil saveTxt 到" + path + "失败" + e.toString());
            //    return false;
            //}
            return true;
        }

        public static bool saveTxt(String path, StringBuilder content)
        {
            return saveTxt(path, content.ToString());
        }

        public static bool saveCollectionToTxt<T>(IEnumerable<T> collection, String path)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Object o in collection)
            {
                sb.Append(o);
                sb.Append('\n');
            }
            return saveTxt(path, sb.ToString());
        }

        /**
         * 将整个文件读取为字节数组
         *
         * @param path
         * @return
         */
        public static byte[] readBytes(String path)
        {
            //try
            //{
            //    FileInputStream fis = new FileInputStream(path);
            //    FileChannel channel = fis.getChannel();
            //    int fileSize = (int)channel.size();
            //    ByteBuffer byteBuffer = ByteBuffer.allocate(fileSize);
            //    channel.read(byteBuffer);
            //    byteBuffer.flip();
            //    byte[] bytes = byteBuffer.array();
            //    byteBuffer.clear();
            //    channel.close();
            //    fis.close();
            //    return bytes;
            //}
            //catch (Exception e)
            //{
            //    logger.warning("读取" + path + "时发生异常" + e);
            //}

            return null;
        }

        public static List<String> readLineList(String path)
        {
            //List<String> result = new List<String>();
            //String txt = readTxt(path);
            //if (txt == null) return result;
            //StringTokenizer tokenizer = new StringTokenizer(txt, "\n");
            //while (tokenizer.hasMoreTokens())
            //{
            //    result.Add(tokenizer.nextToken());
            //}

            //return result;

            return File.ReadAllLines(path).ToList();
        }

        /**
         * 用省内存的方式读取大文件
         *
         * @param path
         * @return
         */
        public static List<String> readLineListWithLessMemory(String path)
        {
            List<String> result = new List<String>();
            String line = null;
            try
            {
                StreamReader bw = new StreamReader(new FileStream(path, FileMode.Open), Encoding.GetEncoding("UTF-8"));
                while ((line = bw.ReadLine()) != null)
                {
                    result.Add(line);
                }
                bw.Close();
            }
            catch (Exception e)
            {
                //logger.warning("加载" + path + "失败，" + e);
            }

            return result;
        }

        public static bool saveMapToTxt(Dictionary<Object, Object> map, String path)
        {
            return saveMapToTxt(map, path, "=");
        }

        public static bool saveMapToTxt(Dictionary<Object, Object> map, String path, String separator)
        {
            map = new Dictionary<Object, Object>(map);
            return saveEntrySetToTxt(map.ToList<KeyValuePair<object, object>>(), path, separator);
        }

        public static bool saveEntrySetToTxt(List<KeyValuePair<Object, Object>> entrySet, String path, String separator)
        {
            StringBuilder sbOut = new StringBuilder();
            foreach (KeyValuePair<Object, Object> entry in entrySet)
            {
                sbOut.Append(entry.Key);
                sbOut.Append(separator);
                sbOut.Append(entry.Value);
                sbOut.Append('\n');
            }
            return saveTxt(path, sbOut.ToString());
        }

        /**
         * 获取文件所在目录的路径
         * @param path
         * @return
         */
        public static String dirname(String path)
        {
            int index = path.LastIndexOf('/');
            if (index == -1) return path;
            return path.Substring(0, index + 1);
        }

        //public static LineIterator readLine(String path)
        //{
        //    return new LineIterator(path);
        //}

        /**
         * 方便读取按行读取大文件
         */
        //    public static class LineIterator : IEnumerable<String>
        //{
        //        BufferedReader bw;
        //        String line;

        //    public LineIterator(String path)
        //    {
        //        try
        //        {
        //            bw = new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8"));
        //            line = bw.readLine();
        //        }
        //        catch (FileNotFoundException e)
        //        {
        //            logger.warning("文件" + path + "不存在，接下来的调用会返回null" + TextUtility.exceptionToString(e));
        //            bw = null;
        //        }
        //        catch (IOException e)
        //        {
        //            logger.warning("在读取过程中发生错误" + TextUtility.exceptionToString(e));
        //            bw = null;
        //        }
        //    }

        //    public void close()
        //    {
        //        if (bw == null) return;
        //        try
        //        {
        //            bw.close();
        //            bw = null;
        //        }
        //        catch (IOException e)
        //        {
        //            logger.warning("关闭文件失败" + TextUtility.exceptionToString(e));
        //        }
        //        return;
        //    }


        //        public boolean hasNext()
        //    {
        //        if (bw == null) return false;
        //        if (line == null)
        //        {
        //            try
        //            {
        //                bw.close();
        //                bw = null;
        //            }
        //            catch (IOException e)
        //            {
        //                logger.warning("关闭文件失败" + TextUtility.exceptionToString(e));
        //            }
        //            return false;
        //        }

        //        return true;
        //    }


        //        public String next()
        //    {
        //        String preLine = line;
        //        try
        //        {
        //            if (bw != null)
        //            {
        //                line = bw.readLine();
        //                if (line == null && bw != null)
        //                {
        //                    try
        //                    {
        //                        bw.close();
        //                        bw = null;
        //                    }
        //                    catch (IOException e)
        //                    {
        //                        logger.warning("关闭文件失败" + TextUtility.exceptionToString(e));
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                line = null;
        //            }
        //        }
        //        catch (IOException e)
        //        {
        //            //logger.warning("在读取过程中发生错误" + TextUtility.exceptionToString(e));
        //        }
        //        return preLine;
        //    }


        //        public void remove()
        //    {
        //        throw new Exception("只读，不可写！");
        //    }
        //}

        /**
         * 创建一个BufferedWriter
         *
         * @param path
         * @return
         * @throws FileNotFoundException
         * @throws UnsupportedEncodingException
         */
        public static StreamWriter newBufferedWriter(String path)
        {
            //return new BufferedWriter(new OutputStreamWriter(new FileOutputStream(path), "UTF-8"));
            return null;
        }

        /**
         * 创建一个BufferedReader
         * @param path
         * @return
         * @throws FileNotFoundException
         * @throws UnsupportedEncodingException
         */
        public static StreamReader newBufferedReader(String path)
        {
            //return new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8"));
            return null;
        }

        public static StreamWriter newBufferedWriter(String path, bool append)
        {
            //return new BufferedWriter(new OutputStreamWriter(new FileOutputStream(path, append), "UTF-8"));
            return null;
        }

        /**
         * 获取最后一个分隔符的后缀
         * @param name
         * @param delimiter
         * @return
         */
        public static String getSuffix(String name, String delimiter)
        {
            return name.Substring(name.LastIndexOf(delimiter) + 1);
        }

        /**
         * 写数组，用制表符分割
         * @param bw
         * @param params
         * @throws IOException
         */
        public static void writeLine(StreamWriter bw, String[] ps)
        {
            for (int i = 0; i < ps.Length - 1; i++)
            {
                bw.Write(ps[i]);
                bw.Write('\t');
            }
            bw.Write(ps[ps.Length - 1]);
        }

        /**
         * 加载词典，词典必须遵守HanLP核心词典格式
         * @param pathArray 词典路径，可以有任意个
         * @return 一个储存了词条的map
         * @throws IOException 异常表示加载失败
         */
        public static Dictionary<String, CoreDictionary.Attribute> loadDictionary(String[] pathArray)
        {
            Dictionary<String, CoreDictionary.Attribute> map = new Dictionary<String, CoreDictionary.Attribute>();
            //foreach (String path in pathArray)
            //{
            //    BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8"));
            //    loadDictionary(br, map);
            //}

            return map;
        }

        /**
         * 将一个BufferedReader中的词条加载到词典
         * @param br 源
         * @param storage 储存位置
         * @throws IOException 异常表示加载失败
         */
        public static void loadDictionary(StreamReader br, Dictionary<String, CoreDictionary.Attribute> storage)
        {
            String line;
            while ((line = br.ReadLine()) != null)
            {
                String[] param = line.Split(new string[] { "\\s" }, StringSplitOptions.None);
                int natureCount = (param.Length - 1) / 2;
                CoreDictionary.Attribute attribute = new CoreDictionary.Attribute(natureCount);
                //for (int i = 0; i < natureCount; ++i)
                //{
                //    attribute.nature[i] = Enum.valueOf(Nature.class, param[1 + 2 * i]);
                //        attribute.frequency[i] = Integer.parseInt(param[2 + 2 * i]);
                //        attribute.totalFrequency += attribute.frequency[i];
                //    }
                //    storage.put(param[0], attribute);
                //}
            }
            //br.close();
        }

        public static void writeCustomNature(BinaryWriter o, HashSet<Nature> customNatureCollector)
        {
            if (customNatureCollector.Count == 0) return;
            o.Write(-customNatureCollector.Count);
            foreach (Nature nature in customNatureCollector)
            {
                TextUtility.writeString(nature.ToString(), o);
            }
        }


