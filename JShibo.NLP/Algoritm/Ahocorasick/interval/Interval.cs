using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Algoritm.Ahocorasick.interval
{
    /**
 * 区间
 */
    public class Interval : Intervalable, IComparable<Interval>
    {
        /**
         * 起点
         */
        private int start;
        /**
         * 终点
         */
        private int end;

        /**
         * 构造一个区间
         * @param start
         * @param end
         */
        public Interval(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public int getStart()
        {
            return this.start;
        }

        public int getEnd()
        {
            return this.end;
        }

        public int size()
        {
            return end - start + 1;
        }

        /**
         * 是否与另一个区间交叉（有一部分重叠）
         * @param other
         * @return
         */
        public bool overlapsWith(Interval other)
        {
            return this.start <= other.getEnd() &&
                    this.end >= other.getStart();
        }

        /**
         * 区间是否覆盖了这个点
         * @param point
         * @return
         */
        public bool overlapsWith(int point)
        {
            return this.start <= point && point <= this.end;
        }


        public override bool Equals(Object o)
        {
            if (!(o is Intervalable))
            {
                return false;
            }
            Intervalable other = (Intervalable)o;
            return this.start == other.getStart() &&
                    this.end == other.getEnd();
        }


        public override int GetHashCode()
        {
            return this.start % 100 + this.end % 100;
        }

        public override String ToString()
        {
            return this.start + ":" + this.end;
        }

        public int CompareTo(Interval o)
        {
            if (!(o is Intervalable))
            {
                return -1;
            }
            Intervalable other = (Intervalable)o;
            int comparison = this.start - other.getStart();
            return comparison != 0 ? comparison : this.end - other.getEnd();
        }
    }
}
