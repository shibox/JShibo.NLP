using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Algoritm.Ahocorasick.interval
{
    /**
 * 区间接口
 */
    public interface Intervalable //: Comparable
    {
    /**
     * 起点
     * @return
     */
    int getStart();

    /**
     * 终点
     * @return
     */
    int getEnd();

    /**
     * 长度
     * @return
     */
    int size();

}
}
