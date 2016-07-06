//using JShibo.NLP.Corpus.Tag;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace JShibo.NLP.Corpus.Util
//{
//    /**
// * 运行时动态增加词性工具
// *
// * @author hankcs
// */
//    public class CustomNatureUtility
//    {
//        static CustomNatureUtility()
//    {
//        //logger.warning("已激活自定义词性功能,由于采用了反射技术,用户需对本地环境的兼容性和稳定性负责!\n" +
//        //                       "如果用户代码X.java中有switch(nature)语句,需要调用CustomNatureUtility.registerSwitchClass(X.class)注册X这个类");
//    }
//    private static Dictionary<String, Nature> extraValueMap = new Dictionary<String, Nature>();

//    /**
//     * 动态增加词性工具
//     */
//    private static EnumBuster<Nature> enumBuster = new EnumBuster<Nature>(Nature.class,
//                                                                          CustomDictionary.class,
//                                                                          Vertex.class,
//                                                                          PersonRecognition.class,
//                                                                          OrganizationRecognition.class);

//    /**
//     * 增加词性
//     * @param name 词性名称
//     * @return 词性
//     */
//    public static Nature addNature(String name)
//    {
//        Nature customNature = extraValueMap.get(name);
//        if (customNature != null) return customNature;
//        customNature = enumBuster.make(name);
//        enumBuster.addByValue(customNature);
//        extraValueMap.put(name, customNature);

//        return customNature;
//    }

//    /**
//     * 注册switch(nature)语句类
//     * @param switchUsers 任何使用了switch(nature)语句的类
//     */
//    public static void registerSwitchClass(Class...switchUsers)
//    {
//        enumBuster.registerSwitchClass(switchUsers);
//    }

//    /**
//     * 还原对词性的全部修改
//     */
//    public static void restore()
//    {
//        enumBuster.restore();
//        extraValueMap.clear();
//    }
//}

//}
