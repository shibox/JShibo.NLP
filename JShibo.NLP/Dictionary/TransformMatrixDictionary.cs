﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Dictionary
{
    /**
 * 转移矩阵词典
 *
 * @param <E> 标签的枚举类型
 * @author hankcs
 */
    public class TransformMatrixDictionary<E>// extends Enum<E>>
{
        //Class<E> enumType;
        Type enumType;
        /**
         * 内部标签下标最大值不超过这个值，用于矩阵创建
         */
        public int ordinaryMax;

    public TransformMatrixDictionary(Type enumType)//(Class<E> enumType)
    {
        this.enumType = enumType;
    }

    /**
     * 储存转移矩阵
     */
    int[][] matrix;

    /**
     * 储存每个标签出现的次数
     */
    int[] total;

    /**
     * 所有标签出现的总次数
     */
    int totalFrequency;

    // HMM的五元组
    /**
     * 隐状态
     */
    public int[] states;
    //int[] observations;
    /**
     * 初始概率
     */
    public double[] start_probability;
    /**
     * 转移概率
     */
    public double[][] transititon_probability;

    public bool load(String path)
    {
        //try
        //{
        //    BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8"));
        //    // 第一行是矩阵的各个类型
        //    String line = br.readLine();
        //    String[] _param = line.split(",");
        //    // 为了制表方便，第一个label是废物，所以要抹掉它
        //    String[] labels = new String[_param.length - 1];
        //    System.arraycopy(_param, 1, labels, 0, labels.length);
        //    int[] ordinaryArray = new int[labels.length];
        //    ordinaryMax = 0;
        //    for (int i = 0; i < ordinaryArray.length; ++i)
        //    {
        //        ordinaryArray[i] = convert(labels[i]).ordinal();
        //        ordinaryMax = Math.max(ordinaryMax, ordinaryArray[i]);
        //    }
        //    ++ordinaryMax;
        //    matrix = new int[ordinaryMax][ordinaryMax];
        //    for (int i = 0; i < ordinaryMax; ++i)
        //    {
        //        for (int j = 0; j < ordinaryMax; ++j)
        //        {
        //            matrix[i][j] = 0;
        //        }
        //    }
        //    // 之后就描述了矩阵
        //    while ((line = br.readLine()) != null)
        //    {
        //        String[] paramArray = line.split(",");
        //        int currentOrdinary = convert(paramArray[0]).ordinal();
        //        for (int i = 0; i < ordinaryArray.length; ++i)
        //        {
        //            matrix[currentOrdinary][ordinaryArray[i]] = Integer.valueOf(paramArray[1 + i]);
        //        }
        //    }
        //    br.close();
        //    // 需要统计一下每个标签出现的次数
        //    total = new int[ordinaryMax];
        //    for (int j = 0; j < ordinaryMax; ++j)
        //    {
        //        total[j] = 0;
        //        for (int i = 0; i < ordinaryMax; ++i)
        //        {
        //            total[j] += matrix[i][j];
        //            total[j] += matrix[j][i];
        //        }
        //    }
        //    for (int j = 0; j < ordinaryMax; ++j)
        //    {
        //        total[j] -= matrix[j][j];
        //    }
        //    for (int j = 0; j < ordinaryMax; ++j)
        //    {
        //        totalFrequency += total[j];
        //    }
        //    // 下面计算HMM四元组
        //    states = ordinaryArray;
        //    start_probability = new double[ordinaryMax];
        //    for (int s : states)
        //    {
        //        double frequency = total[s] + 1e-8;
        //        start_probability[s] = -Math.log(frequency / totalFrequency);
        //    }
        //    transititon_probability = new double[ordinaryMax][ordinaryMax];
        //    for (int from : states)
        //    {
        //        for (int to : states)
        //        {
        //            double frequency = matrix[from][to] + 1e-8;
        //            transititon_probability[from][to] = -Math.log(frequency / total[from]);
        //            //                    System.out.println("from" + NR.values()[from] + " to" + NR.values()[to] + " = " + transititon_probability[from][to]);
        //        }
        //    }
        //}
        //catch (Exception e)
        //{
        //    logger.warning("读取" + path + "失败" + e);
        //}

        return true;
    }

    /**
     * 获取转移频次
     *
     * @param from
     * @param to
     * @return
     */
    public int getFrequency(String from, String to)
    {
        return getFrequency(convert(from), convert(to));
    }

    /**
     * 获取转移频次
     *
     * @param from
     * @param to
     * @return
     */
    public int getFrequency(E from, E to)
    {
            //return matrix[from.ordinal()][to.ordinal()];
            return 0;
    }

    /**
     * 获取e的总频次
     *
     * @param e
     * @return
     */
    public int getTotalFrequency(E e)
    {
            //return total[e.ordinal()];
            return 0;
    }

    /**
     * 获取所有标签的总频次
     *
     * @return
     */
    public int getTotalFrequency()
    {
        return totalFrequency;
    }

    protected E convert(String label)
    {
            //return Enum.valueOf(enumType, label);
            object o = Enum.Parse(enumType, label);
            return (E)o;
        }

    
    public override String ToString()
    {
            //final StringBuilder sb = new StringBuilder("TransformMatrixDictionary{");
            //sb.append("enumType=").append(enumType);
            //sb.append(", ordinaryMax=").append(ordinaryMax);
            //sb.append(", matrix=").append(Arrays.toString(matrix));
            //sb.append(", total=").append(Arrays.toString(total));
            //sb.append(", totalFrequency=").append(totalFrequency);
            //sb.append('}');
            //return sb.toString();
            return string.Empty;
    }
}
}
