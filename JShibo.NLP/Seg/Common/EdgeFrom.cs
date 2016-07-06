using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Seg.Common
{
    /**
 * 记录了起点的边
 * @author hankcs
 */
    public class EdgeFrom : Edge
    {
        public int from;

        public EdgeFrom(int from, double weight, String name)
                : base(weight, name)
        {
            this.from = from;
        }


        public override String ToString()
        {
            return "EdgeFrom{" +
                    "from=" + from +
                    ", weight=" + weight +
                    ", name='" + name + '\'' +
                    '}';
        }
    }
}
