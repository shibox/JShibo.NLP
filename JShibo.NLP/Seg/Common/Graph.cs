using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Seg.Common
{
    public class Graph
    {
        /**
         * 顶点
         */
        public Vertex[] vertexes;

        /**
         * 边，到达下标i
         */
        public List<EdgeFrom>[] edgesTo;

        /**
         * 将一个词网转为词图
         * @param vertexes 顶点数组
         */
        public Graph(Vertex[] vertexes)
        {
            int size = vertexes.Length;
            this.vertexes = vertexes;
            edgesTo = new List<EdgeFrom>[size];
            for (int i = 0; i < size; ++i)
            {
                edgesTo[i] = new List<EdgeFrom>();
            }
        }

        /**
         * 连接两个节点
         * @param from 起点
         * @param to 终点
         * @param weight 花费
         */
        public void connect(int from, int to, double weight)
        {
            edgesTo[to].Add(new EdgeFrom(from, weight, vertexes[from].word + '@' + vertexes[to].word));
        }


        /**
         * 获取到达顶点to的边列表
         * @param to 到达顶点to
         * @return 到达顶点to的边列表
         */
        public List<EdgeFrom> getEdgeListTo(int to)
        {
            return edgesTo[to];
        }

       
    public override String ToString()
        {
            //return "Graph{" +
            //        "vertexes=" + Arrays.toString(vertexes) +
            //        ", edgesTo=" + Arrays.toString(edgesTo) +
            //        '}';
            return "";
        }

        public String printByTo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("========按终点打印========\n");
            for (int to = 0; to < edgesTo.Length; ++to)
            {
                List<EdgeFrom> edgeFromList = edgesTo[to];
                foreach (EdgeFrom edgeFrom in edgeFromList)
                {
                    sb.Append(String.Format("to:%3d, from:%3d, weight:%05.2f, word:%s\n", to, edgeFrom.from, edgeFrom.weight, edgeFrom.name));
                }
            }

            return sb.ToString();
        }

        /**
         * 根据节点下标数组解释出对应的路径
         * @param path
         * @return
         */
        public List<Vertex> parsePath(int[] path)
        {
            List<Vertex> vertexList = new List<Vertex>();
            foreach (int i in path)
            {
                vertexList.Add(vertexes[i]);
            }

            return vertexList;
        }

        /**
         * 从一个路径中转换出空格隔开的结果
         * @param path
         * @return
         */
        public static String parseResult(List<Vertex> path)
        {
            if (path.Count < 2)
            {
                throw new Exception("路径节点数小于2:" + path);
            }
            StringBuilder sb = new StringBuilder();

            for (int i = 1; i < path.Count - 1; ++i)
            {
                Vertex v = path[i];
                sb.Append(v.getRealWord() + " ");
            }

            return sb.ToString();
        }

        public Vertex[] getVertexes()
        {
            return vertexes;
        }

        public List<EdgeFrom>[] getEdgesTo()
        {
            return edgesTo;
        }
    }
}
