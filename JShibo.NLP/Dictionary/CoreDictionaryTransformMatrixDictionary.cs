using JShibo.NLP.Corpus.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary
{
    /**
 * 核心词典词性转移矩阵
 * @author hankcs
 */
    public class CoreDictionaryTransformMatrixDictionary
    {
        public static TransformMatrixDictionary<Nature> transformMatrixDictionary;
        static CoreDictionaryTransformMatrixDictionary()
        {
            transformMatrixDictionary = new TransformMatrixDictionary<Nature>(typeof(Nature));
            //long start = System.currentTimeMillis();
            if (!transformMatrixDictionary.load(HanLP.Config.CoreDictionaryTransformMatrixDictionaryPath))
            {
                //System.err.println("加载核心词典词性转移矩阵" + HanLP.Config.CoreDictionaryTransformMatrixDictionaryPath + "失败");
                //System.exit(-1);
            }
            else
            {
                //logger.info("加载核心词典词性转移矩阵" + HanLP.Config.CoreDictionaryTransformMatrixDictionaryPath + "成功，耗时：" + (System.currentTimeMillis() - start) + " ms");
            }
        }
    }
}
