﻿using JShibo.NLP.Dictionary.NR;
using JShibo.NLP.Recognition.NR;
using JShibo.NLP.Recognition.NS;
using JShibo.NLP.Recognition.NT;
using JShibo.NLP.Seg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Seg.Viterbi
{
    /**
 * Viterbi分词器<br>
 * 也是最短路分词，最短路求解采用Viterbi算法
 *
 * @author hankcs
 */
    public class ViterbiSegment : WordBasedGenerativeModelSegment
    {

        protected override List<Term> segSentence(char[] sentence)
        {
            //        long start = System.currentTimeMillis();
            WordNet wordNetAll = new WordNet(sentence);
            ////////////////生成词网////////////////////
            GenerateWordNet(wordNetAll);
            ///////////////生成词图////////////////////
            //        System.out.println("构图：" + (System.currentTimeMillis() - start));
            if (HanLP.Config.DEBUG)
            {
                //System.out.printf("粗分词网：\n%s\n", wordNetAll);
            }
            //        start = System.currentTimeMillis();
            LinkedList<Vertex> vertexList = viterbi(wordNetAll);
            //        System.out.println("最短路：" + (System.currentTimeMillis() - start));

            if (config.useCustomDictionary)
            {
                combineByCustomDictionary(vertexList);
            }

            if (HanLP.Config.DEBUG)
            {
                //System.out.println("粗分结果" + convert(vertexList, false));
            }

            // 数字识别
            if (config.numberQuantifierRecognize)
            {
                mergeNumberQuantifier(vertexList, wordNetAll, config);
            }

            // 实体命名识别
            if (config.ner)
            {
                WordNet wordNetOptimum = new WordNet(sentence, vertexList);
                int preSize = wordNetOptimum.Size();
                if (config.nameRecognize)
                {
                    PersonRecognition.Recognition(vertexList, wordNetOptimum, wordNetAll);
                }
                if (config.translatedNameRecognize)
                {
                    TranslatedPersonRecognition.Recognition(vertexList, wordNetOptimum, wordNetAll);
                }
                if (config.japaneseNameRecognize)
                {
                    JapanesePersonRecognition.Recognition(vertexList, wordNetOptimum, wordNetAll);
                }
                if (config.placeRecognize)
                {
                    PlaceRecognition.Recognition(vertexList, wordNetOptimum, wordNetAll);
                }
                if (config.organizationRecognize)
                {
                    // 层叠隐马模型——生成输出作为下一级隐马输入
                    vertexList = viterbi(wordNetOptimum);
                    wordNetOptimum.clear();
                    wordNetOptimum.addAll(vertexList);
                    preSize = wordNetOptimum.Size();
                    OrganizationRecognition.Recognition(vertexList, wordNetOptimum, wordNetAll);
                }
                if (wordNetOptimum.Size() != preSize)
                {
                    vertexList = viterbi(wordNetOptimum);
                    if (HanLP.Config.DEBUG)
                    {
                        Console.WriteLine("细分词网：\n%s\n", wordNetOptimum);
                    }
                }
            }

            // 如果是索引模式则全切分
            if (config.indexMode)
            {
                //return decorateResultForIndexMode(vertexList, wordNetAll);
            }

            // 是否标注词性
            if (config.speechTagging)
            {
                //speechTagging(vertexList);
            }

            return convert(vertexList, config.offset);
        }

        private static LinkedList<Vertex> viterbi(WordNet wordNet)
        {
            // 避免生成对象，优化速度
            LinkedList<Vertex>[] nodes = wordNet.getVertexes();
            LinkedList<Vertex> vertexList = new LinkedList<Vertex>();
            foreach (Vertex node in nodes[1])
            {
                node.updateFrom(nodes[0].First());
            }
            for (int i = 1; i < nodes.Length - 1; ++i)
            {
                LinkedList<Vertex> nodeArray = nodes[i];
                if (nodeArray == null) continue;
                foreach (Vertex node in nodeArray)
                {
                    if (node.from == null) continue;
                    foreach (Vertex to in nodes[i + node.realWord.Length])
                    {
                        to.updateFrom(node);
                    }
                }
            }
            Vertex from = nodes[nodes.Length - 1].First();
            while (from != null)
            {
                vertexList.AddFirst(from);
                from = from.from;
            }
            return vertexList;
        }

        /**
         * 第二次维特比，可以利用前一次的结果，降低复杂度
         *
         * @param wordNet
         * @return
         */
        //    private static List<Vertex> viterbiOptimal(WordNet wordNet)
        //    {
        //        LinkedList<Vertex> nodes[] = wordNet.getVertexes();
        //        LinkedList<Vertex> vertexList = new LinkedList<Vertex>();
        //        for (Vertex node : nodes[1])
        //        {
        //            if (node.isNew)
        //                node.updateFrom(nodes[0].getFirst());
        //        }
        //        for (int i = 1; i < nodes.length - 1; ++i)
        //        {
        //            LinkedList<Vertex> nodeArray = nodes[i];
        //            if (nodeArray == null) continue;
        //            for (Vertex node : nodeArray)
        //            {
        //                if (node.from == null) continue;
        //                if (node.isNew)
        //                {
        //                    for (Vertex to : nodes[i + node.realWord.length()])
        //                    {
        //                        to.updateFrom(node);
        //                    }
        //                }
        //            }
        //        }
        //        Vertex from = nodes[nodes.length - 1].getFirst();
        //        while (from != null)
        //        {
        //            vertexList.addFirst(from);
        //            from = from.from;
        //        }
        //        return vertexList;
        //    }
    }
}
