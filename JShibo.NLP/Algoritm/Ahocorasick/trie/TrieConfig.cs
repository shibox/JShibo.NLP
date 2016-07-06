using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Algoritm.Ahocorasick.trie
{
    /**
 * 配置
 */
    public class TrieConfig
    {
        /**
         * 允许重叠
         */
        private bool allowOverlaps = true;

        /**
         * 只保留最长匹配
         */
        public bool remainLongest = false;

        /**
         * 是否允许重叠
         *
         * @return
         */
        public bool isAllowOverlaps()
        {
            return allowOverlaps;
        }

        /**
         * 设置是否允许重叠
         *
         * @param allowOverlaps
         */
        public void setAllowOverlaps(bool allowOverlaps)
        {
            this.allowOverlaps = allowOverlaps;
        }
    }
}
