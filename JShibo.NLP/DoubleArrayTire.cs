using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP
{
    public class DoubleArrayTire<T>
    {
        #region 字段

        private static int BUF_SIZE = 16384;
        private static int UNIT_SIZE = 8; // size of int + int

        protected int[] checkArray;
        protected int[] baseArray;
        private bool[] usedArray;
        protected int size;
        private int allocSize;
        private List<String> key;
        private int keySize;
        private int[] length;
        private int[] value;
        protected T[] v;
        private int progress;
        private int nextCheckPos;
        public byte[] Frqs;
        int error;

        #endregion

        #region 构建

        private class Node
        {
            public int code;
            public int depth;
            public int left;
            public int right;

            public override string ToString()
            {
                return "Node{" +
                        "code=" + code +
                        ", depth=" + depth +
                        ", left=" + left +
                        ", right=" + right +
                        '}';
            }
        }

        private int Resize(int newSize)
        {
            int[] base2 = new int[newSize];
            int[] check2 = new int[newSize];
            bool[] used2 = new bool[newSize];
            if (allocSize > 0)
            {
                Buffer.BlockCopy(baseArray, 0, base2, 0, allocSize * 4);
                Buffer.BlockCopy(checkArray, 0, check2, 0, allocSize * 4);
                Buffer.BlockCopy(usedArray, 0, used2, 0, allocSize);
            }
            baseArray = base2;
            checkArray = check2;
            usedArray = used2;
            return allocSize = newSize;
        }

        /// <summary>
        /// 获取直接相连的子节点
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="siblings">（子）兄弟节点</param>
        /// <returns>兄弟节点个数</returns>
        private int Fetch(Node parent, List<Node> siblings)
        {
            if (error < 0)
                return 0;

            int prev = 0;

            for (int i = parent.left; i < parent.right; i++)
            {
                if ((length != null ? length[i] : key[i].Length) < parent.depth)
                    continue;

                String tmp = key[i];

                int cur = 0;
                if ((length != null ? length[i] : tmp.Length) != parent.depth)
                    cur = (int)tmp[parent.depth] + 1;

                if (prev > cur)
                {
                    error = -3;
                    return 0;
                }

                if (cur != prev || siblings.Count == 0)
                {
                    Node tmp_node = new Node();
                    tmp_node.depth = parent.depth + 1;
                    tmp_node.code = cur;
                    tmp_node.left = i;
                    if (siblings.Count != 0)
                        siblings[siblings.Count - 1].right = i;

                    siblings.Add(tmp_node);
                }
                prev = cur;
            }

            if (siblings.Count != 0)
                siblings[siblings.Count - 1].right = parent.right;

            return siblings.Count;
        }

        /// <summary>
        /// 插入节点
        /// </summary>
        /// <param name="siblings">等待插入的兄弟节点</param>
        /// <returns>插入位置</returns>
        private int Insert(List<Node> siblings)
        {
            if (error < 0)
                return 0;

            int begin = 0;
            int pos = Math.Max(siblings[0].code + 1, nextCheckPos) - 1;
            int nonzero_num = 0;
            int first = 0;

            if (allocSize <= pos)
                Resize(pos + 1);

            outer:
            // 此循环体的目标是找出满足base[begin + a1...an]  == 0的n个空闲空间,a1...an是siblings中的n个节点
            while (true)
            {
                pos++;
                if (allocSize <= pos)
                    Resize(pos + 1);
                if (checkArray[pos] != 0)
                {
                    nonzero_num++;
                    continue;
                }
                else if (first == 0)
                {
                    nextCheckPos = pos;
                    first = 1;
                }

                begin = pos - siblings[0].code; // 当前位置离第一个兄弟节点的距离
                if (allocSize <= (begin + siblings[siblings.Count - 1].code))
                {
                    // progress can be zero // 防止progress产生除零错误
                    double l = (1.05 > 1.0 * keySize / (progress + 1)) ? 1.05 : 1.0
                            * keySize / (progress + 1);
                    Resize((int)(allocSize * l));
                }

                if (usedArray[begin])
                    continue;
                for (int i = 1; i < siblings.Count; i++)
                    if (checkArray[begin + siblings[i].code] != 0)
                        //continue outer;
                        //continue;
                        goto outer;
                break;
            }

            // 从位置 next_check_pos 开始到 pos 间，如果已占用的空间在95%以上，下次插入节点时，直接从 pos 位置处开始查找
            if (1.0 * nonzero_num / (pos - nextCheckPos + 1) >= 0.95)
                nextCheckPos = pos;

            usedArray[begin] = true;
            size = (size > begin + siblings[siblings.Count - 1].code + 1) ? size
                    : begin + siblings[siblings.Count - 1].code + 1;

            for (int i = 0; i < siblings.Count; i++)
            {
                checkArray[begin + siblings[i].code] = begin;
            }

            for (int i = 0; i < siblings.Count; i++)
            {
                List<Node> new_siblings = new List<Node>();
                if (Fetch(siblings[i], new_siblings) == 0)  // 一个词的终止且不为其他词的前缀
                {
                    baseArray[begin + siblings[i].code] = (value != null) ? (-value[siblings
                            [i].left] - 1) : (-siblings[i].left - 1);

                    if (value != null && (-value[siblings[i].left] - 1) >= 0)
                    {
                        error = -2;
                        return 0;
                    }
                    progress++;
                }
                else
                {
                    int h = Insert(new_siblings);   // dfs
                    baseArray[begin + siblings[i].code] = h;
                }
            }
            return begin;
        }

        public int Build(List<String> key, List<T> value)
        {
            return Build(key, null, null, key.Count);
        }

        public int Build(List<String> key, T[] value)
        {
            v = value;
            return Build(key, null, null, key.Count);
        }

        /**
         * 构建DAT
         *
         * @param entrySet 注意此entrySet一定要是字典序的！否则会失败
         * @return
         */
        public int Build(HashSet<KeyValuePair<String, T>> entrySet)
        {
            List<String> keyList = new List<String>(entrySet.Count);
            List<T> valueList = new List<T>(entrySet.Count);
            foreach (KeyValuePair<String, T> entry in entrySet)
            {
                keyList.Add(entry.Key);
                valueList.Add(entry.Value);
            }

            return Build(keyList, valueList);
        }

        /**
         * 唯一的构建方法
         *
         * @param _key     值set，必须字典序
         * @param _length  对应每个key的长度，留空动态获取
         * @param _value   每个key对应的值，留空使用key的下标作为值
         * @param _keySize key的长度，应该设为_key.size
         * @return 是否出错
         */
        public int Build(List<String> _key, int[] _length, int[] _value, int _keySize)
        {
            if (_keySize > _key.Count || _key == null)
                return 0;
            key = _key;
            length = _length;
            keySize = _keySize;
            value = _value;
            progress = 0;

            Resize(65536 * 32); // 32个双字节

            baseArray[0] = 1;
            nextCheckPos = 0;

            Node root_node = new Node();
            root_node.left = 0;
            root_node.right = keySize;
            root_node.depth = 0;

            List<Node> siblings = new List<Node>();
            Fetch(root_node, siblings);
            Insert(siblings);

            // size += (1 << 8 * 2) + 1; // ???
            // if (size >= allocSize) resize (size);

            usedArray = null;
            key = null;
            length = null;

            return error;
        }

        #endregion

        #region 构造函数

        public DoubleArrayTire()
        {
            checkArray = null;
            baseArray = null;
            usedArray = null;
            size = 0;
            allocSize = 0;
            error = 0;
        }

        #endregion

        #region other

        void Clear()
        {
            checkArray = null;
            baseArray = null;
            usedArray = null;
            allocSize = 0;
            size = 0;
        }

        //public int getUnitSize()
        //{
        //    return UNIT_SIZE;
        //}

        //public int getSize()
        //{
        //    return size;
        //}

        //public int getTotalSize()
        //{
        //    return size * UNIT_SIZE;
        //}

        public int GetNonzeroSize()
        {
            int result = 0;
            for (int i = 0; i < checkArray.Length; ++i)
                if (checkArray[i] != 0)
                    ++result;
            return result;
        }

        public void Free()
        {
            usedArray = null;
        }

        #endregion

        #region File

        public bool Open(String fileName)
        {
            if (File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                BinaryReader bw = new BinaryReader(fs);
                size = bw.ReadInt32();
                byte[] bytes = new byte[size * 4];
                bw.Read(bytes, 0, bytes.Length);
                baseArray = new int[size];
                Buffer.BlockCopy(bytes, 0, baseArray, 0, bytes.Length);

                bw.Read(bytes, 0, bytes.Length);
                checkArray = new int[size];
                Buffer.BlockCopy(bytes, 0, checkArray, 0, bytes.Length);

                if (typeof(T) == typeof(int))
                {
                    int b = bw.ReadInt32();
                    v = new T[b];
                    bw.Read(bytes, 0, v.Length * 4);
                    Buffer.BlockCopy(bytes, 0, v, 0, v.Length * 4);
                }
                bw.Close();
                fs.Close();
                return true;
            }
            return false;
        }

        public bool Save(String fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            Console.WriteLine(size + "  " + baseArray.Length);
            bw.Write(baseArray.Length);
            byte[] bytes = new byte[baseArray.Length * 4];
            Buffer.BlockCopy(baseArray, 0, bytes, 0, bytes.Length);
            bw.Write(bytes, 0, bytes.Length);
            Buffer.BlockCopy(checkArray, 0, bytes, 0, bytes.Length);
            bw.Write(bytes, 0, bytes.Length);
            if (v is int[])
            {
                bw.Write(v.Length);
                Buffer.BlockCopy(v, 0, bytes, 0, v.Length * 4);
                bw.Write(bytes, 0, v.Length * 4);
            }
            bw.Close();
            fs.Close();
            return true;
        }

        public bool Load(String path, List<T> value)
        {
            if (!LoadBaseAndCheck(path)) return false;
            v = (T[])value.ToArray();
            return true;
        }

        /**
         * 从磁盘加载，需要额外提供值
         *
         * @param path
         * @param value
         * @return
         */
        public bool Load(String path, T[] value)
        {
            if (!LoadBaseAndCheckByFileChannel(path)) return false;
            v = value;
            return true;
        }

        //public bool load(ByteArray byteArray, V[] value)
        //{
        //    if (byteArray == null) return false;
        //    size = byteArray.nextInt();
        //    @base = new int[size + 65535];   // 多留一些，防止越界
        //    check = new int[size + 65535];
        //    for (int i = 0; i < size; i++)
        //    {
        //        base[i] = byteArray.nextInt();
        //        check[i] = byteArray.nextInt();
        //    }
        //    v = value;
        //    return true;
        //}

        /**
         * 载入双数组，但是不提供值，此时本trie相当于一个set
         *
         * @param path
         * @return
         */
        public bool Load(String path)
        {
            return LoadBaseAndCheckByFileChannel(path);
        }

        /**
         * 从磁盘加载双数组
         *
         * @param path
         * @return
         */
        private bool LoadBaseAndCheck(String path)
        {
            //try
            //{
            //    DataInputStream in = new DataInputStream(new BufferedInputStream(new FileInputStream(path)));
            //    size = in.readInt();
            //    @base = new int[size + 65535];   // 多留一些，防止越界
            //    check = new int[size + 65535];
            //    for (int i = 0; i < size; i++)
            //    {
            //        base[i] = in.readInt();
            //        check[i] = in.readInt();
            //    }
            //}
            //catch (Exception e)
            //{
            //    return false;
            //}
            return true;
        }

        private bool LoadBaseAndCheckByFileChannel(String path)
        {
            //try
            //{
            //    FileInputStream fis = new FileInputStream(path);
            //    // 1.从FileInputStream对象获取文件通道FileChannel
            //    FileChannel channel = fis.getChannel();
            //    int fileSize = (int)channel.Count;

            //    // 2.从通道读取文件内容
            //    ByteBuffer byteBuffer = ByteBuffer.allocate(fileSize);

            //    // channel.read(ByteBuffer) 方法就类似于 inputstream.read(byte)
            //    // 每次read都将读取 allocate 个字节到ByteBuffer
            //    channel.read(byteBuffer);
            //    // 注意先调用flip方法反转Buffer,再从Buffer读取数据
            //    byteBuffer.flip();
            //    // 有几种方式可以操作ByteBuffer
            //    // 可以将当前Buffer包含的字节数组全部读取出来
            //    byte[] bytes = byteBuffer.array();
            //    byteBuffer.clear();
            //    // 关闭通道和文件流
            //    channel.close();
            //    fis.close();

            //    int index = 0;
            //    size = ByteUtil.bytesHighFirstToInt(bytes, index);
            //    index += 4;
            //    @base = new int[size + 65535];   // 多留一些，防止越界
            //    check = new int[size + 65535];
            //    for (int i = 0; i < size; i++)
            //    {
            //        base[i] = ByteUtil.bytesHighFirstToInt(bytes, index);
            //        index += 4;
            //        check[i] = ByteUtil.bytesHighFirstToInt(bytes, index);
            //        index += 4;
            //    }
            //}
            //catch (Exception e)
            //{
            //    return false;
            //}
            return true;
        }

        #endregion

        #region Match

        /// <summary>
        /// 完全匹配
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int ExactMatchSearch(String key)
        {
            return ExactMatchSearch(key, 0, 0, 0);
        }

        /**
         * 精确查询
         *
         * @param keyChars 键的char数组
         * @param pos      char数组的起始位置
         * @param len      键的长度
         * @param nodePos  开始查找的位置（本参数允许从非根节点查询）
         * @return 查到的节点代表的value ID，负数表示不存在
         */
        public int ExactMatchSearch(String key, int pos, int len, int nodePos)
        {
            int result = -1;
            int b = baseArray[nodePos];
            int p;
            for (int i = pos; i < len; i++)
            {
                p = b + key[i] + 1;
                if (b == checkArray[p])
                    b = baseArray[p];
                else
                    return result;
            }
            int n = baseArray[b];
            if (b == checkArray[b] && n < 0)
                result = -n - 1;
            return result;

            //if (len <= 0)
            //    len = key.Length;
            //if (nodePos <= 0)
            //    nodePos = 0;

            //int result = -1;

            //char[] keyChars = key.ToCharArray();

            //int b = baseArray[nodePos];
            //int p;

            //for (int i = pos; i < len; i++)
            //{
            //    p = b + (int)(keyChars[i]) + 1;
            //    if (b == checkArray[p])
            //        b = baseArray[p];
            //    else
            //        return result;
            //}

            //p = b;
            //int n = baseArray[p];
            //if (b == checkArray[p] && n < 0)
            //{
            //    result = -n - 1;
            //}
            //return result;
        }

        public List<int> commonPrefixSearch(String key)
        {
            return commonPrefixSearch(key, 0, 0, 0);
        }

        /**
         * 前缀查询
         *
         * @param key     查询字串
         * @param pos     字串的开始位置
         * @param len     字串长度
         * @param nodePos base中的开始位置
         * @return 一个含有所有下标的list
         */
        public List<int> commonPrefixSearch(String key, int pos, int len, int nodePos)
        {
            if (len <= 0)
                len = key.Length;
            if (nodePos <= 0)
                nodePos = 0;
            List<int> result = new List<int>();
            int b = baseArray[nodePos];
            int n;
            int p;
            for (int i = pos; i < len; i++)
            {
                p = b + key[i] + 1;    // 状态转移 p = base[char[i-1]] + char[i] + 1
                if (b == checkArray[p])                  // base[char[i-1]] == check[base[char[i-1]] + char[i] + 1]
                    b = baseArray[p];
                else
                    return result;
                p = b;
                n = baseArray[p];
                if (b == checkArray[p] && n < 0)         // base[p] == check[p] && base[p] < 0 查到一个词
                {
                    result.Add(-n - 1);
                }
            }
            return result;
        }

        /**
         * 前缀查询，包含值
         *
         * @param key 键
         * @return 键值对列表
         * @deprecated 最好用优化版的
         */
        //public LinkedList<Map.Entry<String, V>> commonPrefixSearchWithValue(String key)
        //{
        //    int len = key.Length;
        //    LinkedList<Map.Entry<String, V>> result = new LinkedList<Map.Entry<String, V>>();
        //    char[] keyChars = key.toCharArray();
        //    int b = base[0];
        //    int n;
        //    int p;

        //    for (int i = 0; i < len; ++i)
        //    {
        //        p = b;
        //        n = base[p];
        //        if (b == check[p] && n < 0)         // base[p] == check[p] && base[p] < 0 查到一个词
        //        {
        //            result.Add(new AbstractMap.SimpleEntry<String, V>(new String(keyChars, 0, i), v[-n - 1]));
        //        }

        //        p = b + (int)(keyChars[i]) + 1;    // 状态转移 p = base[char[i-1]] + char[i] + 1
        //                                           // 下面这句可能产生下标越界，不如改为if (p < size && b == check[p])，或者多分配一些内存
        //        if (b == check[p])                  // base[char[i-1]] == check[base[char[i-1]] + char[i] + 1]
        //            b = base[p];
        //        else
        //            return result;
        //    }

        //    p = b;
        //    n = base[p];

        //    if (b == check[p] && n < 0)
        //    {
        //        result.Add(new AbstractMap.SimpleEntry<String, V>(key, v[-n - 1]));
        //    }

        //    return result;
        //}

        /**
         * 优化的前缀查询，可以复用字符数组
         *
         * @param keyChars
         * @param begin
         * @return
         */
        //public LinkedList<Map.Entry<String, V>> commonPrefixSearchWithValue(char[] keyChars, int begin)
        //{
        //    int len = keyChars.length;
        //    LinkedList<Map.Entry<String, V>> result = new LinkedList<Map.Entry<String, V>>();
        //    int b = base[0];
        //    int n;
        //    int p;

        //    for (int i = begin; i < len; ++i)
        //    {
        //        p = b;
        //        n = base[p];
        //        if (b == check[p] && n < 0)         // base[p] == check[p] && base[p] < 0 查到一个词
        //        {
        //            result.Add(new AbstractMap.SimpleEntry<String, V>(new String(keyChars, begin, i - begin), v[-n - 1]));
        //        }

        //        p = b + (int)(keyChars[i]) + 1;    // 状态转移 p = base[char[i-1]] + char[i] + 1
        //                                           // 下面这句可能产生下标越界，不如改为if (p < size && b == check[p])，或者多分配一些内存
        //        if (b == check[p])                  // base[char[i-1]] == check[base[char[i-1]] + char[i] + 1]
        //            b = base[p];
        //        else
        //            return result;
        //    }

        //    p = b;
        //    n = base[p];

        //    if (b == check[p] && n < 0)
        //    {
        //        result.Add(new AbstractMap.SimpleEntry<String, V>(new String(keyChars, begin, len - begin), v[-n - 1]));
        //    }

        //    return result;
        //}

        #endregion

        #region 扩展

        public String toString()
        {
            //        String infoIndex    = "i    = ";
            //        String infoChar     = "char = ";
            //        String info@base     = "@base = ";
            //        String infoCheck    = "check= ";
            //        for (int i = 0; i < base.length; ++i)
            //        {
            //            if (base[i] != 0 || check[i] != 0)
            //            {
            //                infoChar  += "    " + (i == check[i] ? " ×" : (char)(i - check[i] - 1));
            //                infoIndex += " " + String.format("%5d", i);
            //                info@base  += " " +  String.format("%5d", base[i]);
            //                infoCheck += " " + String.format("%5d", check[i]);
            //            }
            //        }
            return "DoubleArrayTrie{" +
                    //                "\n" + infoChar +
                    //                "\n" + infoIndex +
                    //                "\n" + info@base +
                    //                "\n" + infoCheck + "\n" +
                    //                "check=" + Arrays.toString(check) +
                    //                ", base=" + Arrays.toString(base) +
                    //                ", used=" + Arrays.toString(used) +
                    "size=" + size +
                    ", allocSize=" + allocSize +
                    ", key=" + key +
                    ", keySize=" + keySize +
                    //                ", length=" + Arrays.toString(length) +
                    //                ", value=" + Arrays.toString(value) +
                    ", progress=" + progress +
                    ", nextCheckPos=" + nextCheckPos +
                    ", error_=" + error +
                    '}';
        }

        /**
         * 精确查询
         *
         * @param key 键
         * @return 值
         */
        public T Get(String key)
        {
            int index = ExactMatchSearch(key);
            if (index >= 0)
                return v[index];
            return default(T);
        }

        public bool ContainsKey(String key)
        {
            return ExactMatchSearch(key) >= 0;
        }

        /**
         * 沿着节点转移状态
         *
         * @param path
         * @return
         */
        protected int transition(string path)
        {
            int b = baseArray[0];
            int p;
            for (int i = 0; i < path.Length; ++i)
            {
                p = b + (int)(path[i]) + 1;
                if (b == checkArray[p])
                    b = baseArray[p];
                else
                    return -1;
            }
            p = b;
            return p;
        }

        /**
         * 沿着路径转移状态
         *
         * @param path 路径
         * @param from 起点（根起点为base[0]=1）
         * @return 转移后的状态（双数组下标）
         */
        public int transition(String path, int from)
        {
            int b = from;
            int p;

            for (int i = 0; i < path.Length; ++i)
            {
                p = b + (int)(path[i]) + 1;
                if (b == checkArray[p])
                    b = baseArray[p];
                else
                    return -1;
            }

            p = b;
            return p;
        }

        /**
         * 转移状态
         * @param c
         * @param from
         * @return
         */
        public int transition(char c, int from)
        {
            int b = from;
            int p;

            p = b + (int)(c) + 1;
            if (b == checkArray[p])
                b = baseArray[p];
            else
                return -1;

            return b;
        }

        /**
         * 检查状态是否对应输出
         *
         * @param state 双数组下标
         * @return 对应的值，null表示不输出
         */
        public T Output(int state)
        {
            if (state < 0) return default(T);
            int n = baseArray[state];
            if (state == checkArray[state] && n < 0)
            {
                return v[-n - 1];
            }
            return default(T);
        }

        /**
         * 一个搜索工具（注意，当调用next()返回false后不应该继续调用next()，除非reset状态）
         */
        //public class Searcher
        //{
        //    /**
        //     * key的起点
        //     */
        //    public int begin;
        //    /**
        //     * key的长度
        //     */
        //    public int length;
        //    /**
        //     * key的字典序坐标
        //     */
        //    public int index;
        //    /**
        //     * key对应的value
        //     */
        //    public V value;
        //    /**
        //     * 传入的字符数组
        //     */
        //    private char[] charArray;
        //    /**
        //     * 上一个node位置
        //     */
        //    private int last;
        //    /**
        //     * 上一个字符的下标
        //     */
        //    private int i;
        //    /**
        //     * charArray的长度，效率起见，开个变量
        //     */
        //    private int arrayLength;

        //    /**
        //     * 构造一个双数组搜索工具
        //     *
        //     * @param offset    搜索的起始位置
        //     * @param charArray 搜索的目标字符数组
        //     */
        //    public Searcher(int offset, char[] charArray)
        //    {
        //        this.charArray = charArray;
        //        i = offset;
        //        last = @base[0];
        //        arrayLength = charArray.Length;
        //        // A trick，如果文本长度为0的话，调用next()时，会带来越界的问题。
        //        // 所以我要在第一次调用next()的时候触发begin == arrayLength进而返回false。
        //        // 当然也可以改成begin >= arrayLength，不过我觉得操作符>=的效率低于==
        //        if (arrayLength == 0) begin = -1;
        //        else begin = offset;
        //    }

        //    /**
        //     * 取出下一个命中输出
        //     *
        //     * @return 是否命中，当返回false表示搜索结束，否则使用公开的成员读取命中的详细信息
        //     */
        //    public bool next()
        //    {
        //        int b = last;
        //        int n;
        //        int p;

        //        for (; ; ++i)
        //        {
        //            if (i == arrayLength)               // 指针到头了，将起点往前挪一个，重新开始，状态归零
        //            {
        //                ++begin;
        //                if (begin == arrayLength) break;
        //                i = begin;
        //                b = @base[0];
        //            }
        //            p = b + (int)(charArray[i]) + 1;   // 状态转移 p = base[char[i-1]] + char[i] + 1
        //            if (b == check[p])                  // base[char[i-1]] == check[base[char[i-1]] + char[i] + 1]
        //                b = @base[p];                    // 转移成功
        //            else
        //            {
        //                i = begin;                      // 转移失败，也将起点往前挪一个，重新开始，状态归零
        //                ++begin;
        //                if (begin == arrayLength) break;
        //                b = @base[0];
        //                continue;
        //            }
        //            p = b;
        //            n = @base[p];
        //            if (b == check[p] && n < 0)         // base[p] == check[p] && base[p] < 0 查到一个词
        //            {
        //                length = i - begin + 1;
        //                index = -n - 1;
        //                value = v[index];
        //                last = b;
        //                ++i;
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //}

        //public Searcher getSearcher(String text, int offset)
        //{
        //    return new Searcher(offset, text.ToCharArray());
        //}

        //public Searcher getSearcher(char[] text, int offset)
        //{
        //    return new Searcher(offset, text);
        //}

        /**
         * 转移状态
         *
         * @param current
         * @param c
         * @return
         */
        protected int Transition(int current, char c)
        {
            int b = baseArray[current];
            int p;

            p = b + c + 1;
            if (b == checkArray[p])
                b = baseArray[p];
            else
                return -1;

            p = b;
            return p;
        }

        /**
         * 更新某个键对应的值
         *
         * @param key   键
         * @param value 值
         * @return 是否成功（失败的原因是没有这个键）
         */
        public bool Set(String key, T value)
        {
            int index = ExactMatchSearch(key);
            if (index >= 0)
            {
                v[index] = value;
                return true;
            }

            return false;
        }

        /**
         * 从值数组中提取下标为index的值<br>
         * 注意为了效率，此处不进行参数校验
         *
         * @param index 下标
         * @return 值
         */
        public T Get(int index)
        {
            return v[index];
        }

        public T[] GetValues()
        {
            return v;
        }

        #endregion

        #region 分词方法

        public List<string> SegmentToQuery1(String key)
        {
            int pos = 0, len = key.Length, nodePos = 0;
            List<string> result = new List<string>();
            int b = baseArray[nodePos], n, p, k = 0;
            for (int i = pos; i < len; i++)
            {
                p = b + key[i] + 1;    // 状态转移 p = base[char[i-1]] + char[i] + 1
                if (b == checkArray[p])                  // base[char[i-1]] == check[base[char[i-1]] + char[i] + 1]
                    b = baseArray[p];
                else
                {
                    result.Add(key.Substring(k, i - k));
                    k = i;
                    b = 1;
                    i--;
                    continue;
                }
                p = b;
                n = baseArray[p];
                if (b == checkArray[p] && n < 0)         // base[p] == check[p] && base[p] < 0 查到一个词
                {
                    //result.Add(-n - 1);
                    Output(-n - 1);
                    result.Add(key.Substring(k, i - k + 1) + "  " + Output(-n - 1));
                    k = i + 1;
                    b = 1;
                }
            }
            return result;
        }

        public List<string> Seg(string key)
        {
            int pos = 0, len = key.Length;
            List<string> result = new List<string>();
            int b = 1, p = 0, n, k = 0;
            int kps = 0, kp = 0, sid = 0;
            start:
            for (int i = pos; i < len;)
            {
                p = b + key[i] + 1;
                if (b == checkArray[p])
                {
                    b = baseArray[p];
                    n = baseArray[b];
                    if (b == checkArray[b] && n < 0)
                    {
                        kps = k;
                        kp = i;
                        sid = -n - 1;
                    }
                    i++;
                }
                else
                {
                    if (kp != 0)
                    {
                        //result.Add(key.Substring(kps, kp - k + 1) + "   " + v[sid]);
                        result.Add(key.Substring(kps, kp - k + 1));
                        k = kps + kp - k + 1;
                        b = 1;
                        //kp = 0;
                        i = k;
                    }
                    else
                    {
                        if (i == k)
                        {
                            i++;
                            result.Add(key.Substring(k, i - k));
                            k = i;
                            b = 1;
                        }
                        else
                        {
                            result.Add(key.Substring(k, 1));
                            k++;
                            i = k;
                            b = 1;
                        }
                    }
                }
            }
            if (kp != 0)
            {
                //result.Add(key.Substring(kps, kp - k + 1) + "   " + v[sid]);
                result.Add(key.Substring(kps, kp - k + 1));
                k = kps + kp - k + 1;
                b = 1;
                kp = 0;
                pos = k;
                sid = 0;
                goto start;
            }
            else if (sid != 0)
                result.Add(key.Substring(kps, kp - k + 1));
            return result;
        }

        public int Seg(string key, T[] buffer)
        {
            int bufferPos = 0;
            int pos = 0, len = key.Length;
            //List<string> result = new List<string>();
            int b = 1, p = 0, n, k = 0;
            int kps = 0, kp = 0, sid = 0;
            start:
            for (int i = pos; i < len;)
            {
                p = b + key[i] + 1;
                if (b == checkArray[p])
                {
                    b = baseArray[p];
                    n = baseArray[b];
                    if (b == checkArray[b] && n < 0)
                    {
                        kps = k;
                        kp = i;
                        sid = -n - 1;
                    }
                    i++;
                }
                else
                {
                    if (kp != 0)
                    {
                        buffer[bufferPos] = v[sid];
                        bufferPos++;
                        k = kps + kp - k + 1;
                        b = 1;
                        kp = 0;
                        i = k;
                    }
                    else
                    {
                        if (i == k)
                        {
                            i++;
                            //result.Add(key.Substring(k, i - k));
                            k = i;
                            b = 1;
                        }
                        else
                        {
                            //result.Add(key.Substring(k, 1));
                            k++;
                            i = k;
                            b = 1;
                        }
                    }
                }
            }
            if (kp != 0)
            {
                buffer[bufferPos] = v[sid];
                bufferPos++;
                k = kps + kp - k + 1;
                b = 1;
                kp = 0;
                pos = k;
                sid = 0;
                goto start;
            }
            else if (sid != 0)
            {
                buffer[bufferPos] = v[sid];
                bufferPos++;
            }
            return bufferPos;
        }

        public int Seg(string key, T[] buffer, byte[] frqs)
        {
            int bufferPos = 0;
            int pos = 0, len = key.Length;
            //List<string> result = new List<string>();
            int b = 1, p = 0, n, k = 0;
            int kps = 0, kp = 0, sid = 0;
            start:
            for (int i = pos; i < len;)
            {
                p = b + key[i] + 1;
                if (b == checkArray[p])
                {
                    b = baseArray[p];
                    n = baseArray[b];
                    if (b == checkArray[b] && n < 0)
                    {
                        kps = k;
                        kp = i;
                        sid = -n - 1;
                    }
                    i++;
                }
                else
                {
                    if (kp != 0)
                    {
                        if (frqs[sid] == 0)
                        {
                            buffer[bufferPos] = v[sid];
                            bufferPos++;
                        }
                        else
                            frqs[sid]++;
                        k = kps + kp - k + 1;
                        b = 1;
                        kp = 0;
                        i = k;
                    }
                    else
                    {
                        if (i == k)
                        {
                            i++;
                            //result.Add(key.Substring(k, i - k));
                            if (frqs[sid] == 0)
                            {
                                buffer[bufferPos] = v[sid];
                                bufferPos++;
                            }
                            else
                                frqs[sid]++;
                            k = i;
                            b = 1;
                        }
                        else
                        {
                            //result.Add(key.Substring(k, 1));
                            if (frqs[sid] == 0)
                            {
                                buffer[bufferPos] = v[sid];
                                bufferPos++;
                            }
                            else
                                frqs[sid]++;
                            k++;
                            i = k;
                            b = 1;
                        }
                    }
                }
            }
            if (kp != 0)
            {
                //buffer[bufferPos] = v[sid];
                //bufferPos++;
                if (frqs[sid] == 0)
                {
                    buffer[bufferPos] = v[sid];
                    bufferPos++;
                }
                else
                    frqs[sid]++;
                k = kps + kp - k + 1;
                b = 1;
                kp = 0;
                pos = k;
                sid = 0;
                goto start;
            }
            else if (sid != 0)
            {
                if (frqs[sid] == 0)
                {
                    buffer[bufferPos] = v[sid];
                    bufferPos++;
                }
                else
                    frqs[sid]++;
            }
            return bufferPos;
        }

        public unsafe int SegUnsafe(string key, T[] buffer, byte[] frqs)
        {
            int bufferPos = 0;
            int pos = 0, len = key.Length;
            int b = 1, p = 0, n, k = 0;
            int kps = 0, kp = 0, sid = 0;

            fixed (int* bpd = &baseArray[0], cpd = &checkArray[0])
            {
                fixed (byte* fpd = &frqs[0])
                {
                    fixed (char* kpd = key)
                    {
                        start:
                        for (int i = pos; i < len;)
                        {
                            p = b + kpd[i] + 1;
                            if (b == cpd[p])
                            {
                                b = bpd[p];
                                n = bpd[b];
                                if (b == cpd[b] && n < 0)
                                {
                                    kps = k;
                                    kp = i;
                                    sid = -n - 1;
                                }
                                i++;
                            }
                            else
                            {
                                if (kp != 0)
                                {
                                    if (fpd[sid] == 0)
                                    {
                                        buffer[bufferPos] = v[sid];
                                        bufferPos++;
                                    }
                                    else
                                        fpd[sid]++;
                                    k = kps + kp - k + 1;
                                    b = 1;
                                    kp = 0;
                                    i = k;
                                }
                                else
                                {

                                    if (i == k)
                                    {
                                        i++;
                                        //result.Add(key.Substring(k, i - k));
                                        if (fpd[sid] == 0)
                                        {
                                            buffer[bufferPos] = v[sid];
                                            bufferPos++;
                                        }
                                        else
                                            fpd[sid]++;
                                        k = i;
                                        b = 1;
                                    }
                                    else
                                    {
                                        //result.Add(key.Substring(k, 1));
                                        if (fpd[sid] == 0)
                                        {
                                            buffer[bufferPos] = v[sid];
                                            bufferPos++;
                                        }
                                        else
                                            fpd[sid]++;
                                        k++;
                                        i = k;
                                        b = 1;
                                    }
                                }
                            }
                        }
                        if (kp != 0)
                        {
                            if (fpd[sid] == 0)
                            {
                                buffer[bufferPos] = v[sid];
                                bufferPos++;
                            }
                            else
                                fpd[sid]++;
                            k = kps + kp - k + 1;
                            b = 1;
                            kp = 0;
                            pos = k;
                            sid = 0;
                            goto start;
                        }
                        else if (sid != 0)
                        {
                            if (fpd[sid] == 0)
                            {
                                buffer[bufferPos] = v[sid];
                                bufferPos++;
                            }
                            else
                                fpd[sid]++;
                        }
                    }
                }

            }
            return bufferPos;
        }

        public int SegMulti(string key, T[] buffer)
        {
            return 0;
        }

        #endregion

        
    }
}
