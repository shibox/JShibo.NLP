﻿using JShibo.NLP.Algoritm.Ahocorasick.interval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Algoritm.Ahocorasick.trie
{
    /**
 * 一个模式串匹配结果
 */
    public class Emit : Interval , Intervalable
    {
    /**
     * 匹配到的模式串
     */
    private String keyword;

    /**
     * 构造一个模式串匹配结果
     * @param start 起点
     * @param end 重点
     * @param keyword 模式串
     */
    public Emit(int start, int end, String keyword)
            :base(start, end)
    {
        
        this.keyword = keyword;
    }

    /**
     * 获取对应的模式串
     * @return 模式串
     */
    public String getKeyword()
    {
        return this.keyword;
    }

    
    public override String ToString()
    {
        return base.ToString() + "=" + this.keyword;
    }
}
}
