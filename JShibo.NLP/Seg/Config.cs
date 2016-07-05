using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Seg
{
    public class Config
    {
        /**
         * 是否是索引分词（合理地最小分割）
         */
        public bool indexMode = false;
        /**
         * 是否识别中国人名
         */
        public bool nameRecognize = true;
        /**
         * 是否识别音译人名
         */
        public bool translatedNameRecognize = true;
        /**
         * 是否识别日本人名
         */
        public bool japaneseNameRecognize = false;
        /**
         * 是否识别地名
         */
        public bool placeRecognize = false;
        /**
         * 是否识别机构
         */
        public bool organizationRecognize = false;
        /**
         * 是否加载用户词典
         */
        public bool useCustomDictionary = true;
        /**
         * 词性标注
         */
        public bool speechTagging = false;
        /**
         * 命名实体识别是否至少有一项被激活
         */
        public bool ner = true;
        /**
         * 是否计算偏移量
         */
        public bool offset = false;
        /**
         * 是否识别数字和量词
         */
        public bool numberQuantifierRecognize = false;
        /**
         * 并行分词的线程数
         */
        public int threadNumber = 1;

        /**
         * 更新命名实体识别总开关
         */
        public void updateNerConfig()
        {
            ner = nameRecognize || translatedNameRecognize || japaneseNameRecognize || placeRecognize || organizationRecognize;
        }
    }
}
