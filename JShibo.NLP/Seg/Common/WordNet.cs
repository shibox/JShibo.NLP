using JShibo.NLP.Corpus.Tag;
using JShibo.NLP.Dictionary;
using JShibo.NLP.Seg.NShort.Path;
using JShibo.NLP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Seg.Common
{
    /**
 * @author hankcs
 */
    public class WordNet
    {
        /**
         * 节点，每一行都是前缀词，跟图的表示方式不同
         */
        private LinkedList<Vertex>[] vertexes;

        /**
         * 共有多少个节点
         */
        int size;

        /**
         * 原始句子
         *
         * @deprecated 应当使用数组，这样比较快
         */
        public String sentence;

        /**
         * 原始句子对应的数组
         */
        public char[] charArray;

        /**
         * 为一个句子生成空白词网
         *
         * @param sentence 句子
         */
        public WordNet(String sentence)
            : this(sentence.ToCharArray())
        {
            
        }

        public WordNet(char[] charArray)
        {
            this.charArray = charArray;
            vertexes = new LinkedList<Vertex>[charArray.Length + 2];
            for (int i = 0; i < vertexes.Length; ++i)
            {
                vertexes[i] = new LinkedList<Vertex>();
            }
            vertexes[0].AddLast(Vertex.newB());
            vertexes[vertexes.Length - 1].AddLast(Vertex.newE());
            size = 2;
        }

        public WordNet(char[] charArray, List<Vertex> vertexList)
        {
            this.charArray = charArray;
            vertexes = new LinkedList<Vertex>[charArray.Length + 2];
            int i = 0;
            for (i = 0; i < vertexes.Length; ++i)
            {
                vertexes[i] = new LinkedList<Vertex>();
            }
            i = 0;
            foreach (Vertex vertex in vertexList)
            {
                vertexes[i].AddLast(vertex);
                ++size;
                i += vertex.realWord.Length;
            }
        }

        /**
         * 添加顶点
         *
         * @param line   行号
         * @param vertex 顶点
         */
        public void add(int line, Vertex vertex)
        {
            foreach (Vertex oldVertex in vertexes[line])
            {
                // 保证唯一性
                if (oldVertex.realWord.Length == vertex.realWord.Length) return;
            }
            vertexes[line].AddLast(vertex);
            ++size;
        }

        /**
         * 强行添加，替换已有的顶点
         *
         * @param line
         * @param vertex
         */
        public void push(int line, Vertex vertex)
        {
            //Iterator<Vertex> iterator = vertexes[line].iterator();
            //while (iterator.hasNext())
            //{
            //    if (iterator.next().realWord.length() == vertex.realWord.length())
            //    {
            //        iterator.remove();
            //        --size;
            //        break;
            //    }
            //}
            foreach (Vertex item in vertexes[line])
            {
                if (item.realWord.Length == vertex.realWord.Length)
                {
                    vertexes[line].Remove(item);
                    --size;
                    break;
                }
            }

            //List< Vertex>.Enumerator iterator = vertexes[line].GetEnumerator();
            //while (iterator.MoveNext())
            //{
            //    if (iterator.Current.realWord.Length == vertex.realWord.Length)
            //    {
            //        iterator.remove();
            //        --size;
            //        break;
            //    }
            //}
            vertexes[line].AddLast(vertex);
            ++size;
        }

        /**
         * 添加顶点，同时检查此顶点是否悬孤，如果悬孤则自动补全
         *
         * @param line
         * @param vertex
         * @param wordNetAll 这是一个完全的词图
         */
        public void insert(int line, Vertex vertex, WordNet wordNetAll)
        {
            foreach (Vertex oldVertex in vertexes[line])
            {
                // 保证唯一性
                if (oldVertex.realWord.Length == vertex.realWord.Length) return;
            }
            vertexes[line].AddLast(vertex);
            ++size;
            // 保证连接
            int l = 0;
            for (l = line - 1; l > 1; --l)
            {
                if (get(l, 1) == null)
                {
                    Vertex first = wordNetAll.getFirst(l);
                    if (first == null) break;
                    vertexes[l].AddLast(first);
                    ++size;
                    if (vertexes[l].Count > 1) break;
                }
                else
                {
                    break;
                }
            }
            // 首先保证这个词语可直达
            l = line + vertex.realWord.Length;
            if (get(l).Count == 0)
            {
                LinkedList<Vertex> targetLine = wordNetAll.get(l);
                if (targetLine == null || targetLine.Count == 0) return;
                vertexes[l].AddRange(targetLine);
                size += targetLine.Count;
            }
            // 直达之后一直往后
            for (++l; l < vertexes.Length; ++l)
            {
                if (get(l).Count == 0)
                {
                    Vertex first = wordNetAll.getFirst(l);
                    if (first == null) break;
                    vertexes[l].AddLast(first);
                    ++size;
                    if (vertexes[l].Count > 1) break;
                }
                else
                {
                    break;
                }
            }
        }

        /**
         * 全自动添加顶点
         *
         * @param vertexList
         */
        public void addAll(List<Vertex> vertexList)
        {
            int i = 0;
            foreach (Vertex vertex in vertexList)
            {
                add(i, vertex);
                i += vertex.realWord.Length;
            }
        }

        /**
         * 获取某一行的所有节点
         *
         * @param line 行号
         * @return 一个数组
         */
        public LinkedList<Vertex> get(int line)
        {
            return vertexes[line];
        }

        /**
         * 获取某一行的第一个节点
         *
         * @param line
         * @return
         */
        public Vertex getFirst(int line)
        {
            LinkedList<Vertex>.Enumerator iterator = vertexes[line].GetEnumerator();
            if (iterator.MoveNext()) return iterator.Current;

            return null;
        }

        /**
         * 获取某一行长度为length的节点
         *
         * @param line
         * @param length
         * @return
         */
        public Vertex get(int line, int length)
        {
            foreach (Vertex vertex in vertexes[line])
            {
                if (vertex.realWord.Length == length)
                {
                    return vertex;
                }
            }

            return null;
        }

        /**
         * 添加顶点，由原子分词顶点添加
         *
         * @param line
         * @param atomSegment
         */
        public void add(int line, List<AtomNode> atomSegment)
        {
            // 将原子部分存入m_segGraph
            int offset = 0;
            foreach (AtomNode atomNode in atomSegment)//Init the cost array
            {
                String sWord = atomNode.sWord;//init the word
                Nature nature = Nature.n;
                int id = -1;
                switch (atomNode.nPOS)
                {
                    case Predefine.CT_CHINESE:
                        break;
                    case Predefine.CT_INDEX:
                    case Predefine.CT_NUM:
                        nature = Nature.m;
                        sWord = "未##数";
                        id = CoreDictionary.M_WORD_ID;
                        break;
                    case Predefine.CT_DELIMITER:
                    case Predefine.CT_OTHER:
                        nature = Nature.w;
                        break;
                    case Predefine.CT_SINGLE://12021-2129-3121
                        nature = Nature.nx;
                        sWord = "未##串";
                        id = CoreDictionary.X_WORD_ID;
                        break;
                    default:
                        break;
                }
                // 这些通用符的量级都在10万左右
                add(line + offset, new Vertex(sWord, atomNode.sWord, new CoreDictionary.Attribute(nature, 10000), id));
                offset += atomNode.sWord.Length;
            }
        }

        public int Size()
        {
            return size;
        }

        /**
         * 获取顶点数组
         *
         * @return Vertex[] 按行优先列次之的顺序构造的顶点数组
         */
        private Vertex[] getVertexesLineFirst()
        {
            Vertex[] vertexes = new Vertex[size];
            int i = 0;
            foreach (LinkedList<Vertex> vertexList in this.vertexes)
            {
                foreach (Vertex v in vertexList)
                {
                    v.index = i;    // 设置id
                    vertexes[i++] = v;
                }
            }

            return vertexes;
        }

        /**
         * 词网转词图
         *
         * @return 词图
         */
        public Graph toGraph()
        {
            Graph graph = new Graph(getVertexesLineFirst());

            for (int row = 0; row < vertexes.Length - 1; ++row)
            {
                LinkedList<Vertex> vertexListFrom = vertexes[row];
                foreach (Vertex from in vertexListFrom)
                {
                    //assert from.realWord.length() > 0 : "空节点会导致死循环！";
                    int toIndex = row + from.realWord.Length;
                    foreach (Vertex to in vertexes[toIndex])
                    {
                        graph.connect(from.index, to.index, MathTools.calculateWeight(from, to));
                    }
                }
            }
            return graph;
        }

        
        public override String ToString()
        {
            //        return "Graph{" +
            //                "vertexes=" + Arrays.toString(vertexes) +
            //                '}';
            StringBuilder sb = new StringBuilder();
            int line = 0;
            foreach (LinkedList<Vertex> vertexList in vertexes)
            {
                sb.Append(line + ':' + vertexList.ToString()).Append("\n");
                line++;
            }
            return sb.ToString();
        }

        /**
         * 将连续的ns节点合并为一个
         */
        public void mergeContinuousNsIntoOne()
        {
            for (int row = 0; row < vertexes.Length - 1; ++row)
            {
                LinkedList<Vertex> vertexListFrom = vertexes[row];
                LinkedList<Vertex>.Enumerator listIteratorFrom = vertexListFrom.GetEnumerator();
                while (listIteratorFrom.MoveNext())
                {
                    Vertex from = listIteratorFrom.Current;
                    if (from.getNature() == Nature.ns)
                    {
                        int toIndex = row + from.realWord.Length;
                        LinkedList<Vertex>.Enumerator listIteratorTo = vertexes[toIndex].GetEnumerator();
                        while (listIteratorTo.MoveNext())
                        {
                            Vertex to = listIteratorTo.Current;
                            if (to.getNature() == Nature.ns)
                            {
                                // 我们不能直接改，因为很多条线路在公用指针
                                //                            from.realWord += to.realWord;
                                //logger.info("合并【" + from.realWord + "】和【" + to.realWord + "】");
                                //listIteratorFrom.set(Vertex.newAddressInstance(from.realWord + to.realWord));
                                //                            listIteratorTo.remove();
                                break;
                            }
                        }
                    }
                }
            }
        }

        /**
         * 清空词图
         */
        public void clear()
        {
            foreach (LinkedList<Vertex> vertexList in vertexes)
            {
                vertexList.Clear();
            }
            size = 0;
        }

        /**
         * 获取内部顶点表格，谨慎操作！
         *
         * @return
         */
        public LinkedList<Vertex>[] getVertexes()
        {
            return vertexes;
        }
    }
}
