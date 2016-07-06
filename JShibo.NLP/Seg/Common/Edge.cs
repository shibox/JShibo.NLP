using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Seg.Common
{
    /**
 * 基础边，不允许构造
 * @author hankcs
 */
    public class Edge
    {
        /**
         * 花费
         */
        public double weight;
        /**
         * 节点名字，调试用
         */
        public String name;

        protected Edge(double weight, String name)
        {
            this.weight = weight;
            this.name = name;
        }
    }
}
