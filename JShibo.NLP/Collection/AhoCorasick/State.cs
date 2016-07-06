﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Collection.AhoCorasick
{
    /**
 * <p>
 * 一个状态有如下几个功能
 * </p>
 * <p/>
 * <ul>
 * <li>success; 成功转移到另一个状态</li>
 * <li>failure; 不可顺着字符串跳转的话，则跳转到一个浅一点的节点</li>
 * <li>emits; 命中一个模式串</li>
 * </ul>
 * <p/>
 * <p>
 * 根节点稍有不同，根节点没有 failure 功能，它的“failure”指的是按照字符串路径转移到下一个状态。其他节点则都有failure状态。
 * </p>
 *
 * @author Robert Bor
 */
    public class State
    {

        /**
         * 模式串的长度，也是这个状态的深度
         */
        protected int depth;

        /**
         * fail 函数，如果没有匹配到，则跳转到此状态。
         */
        private State failure = null;

        /**
         * 只要这个状态可达，则记录模式串
         */
        private HashSet<int> emits = null;
        /**
         * goto 表，也称转移函数。根据字符串的下一个字符转移到下一个状态
         */
        private Dictionary<char, State> success = new Dictionary<char, State>();

        /**
         * 在双数组中的对应下标
         */
        private int index;

        /**
         * 构造深度为0的节点
         */
        public State()
            : this(0)
        {
            
        }

        /**
         * 构造深度为depth的节点
         * @param depth
         */
        public State(int depth)
        {
            this.depth = depth;
        }

        /**
         * 获取节点深度
         * @return
         */
        public int getDepth()
        {
            return this.depth;
        }

        /**
         * 添加一个匹配到的模式串（这个状态对应着这个模式串)
         * @param keyword
         */
        public void addEmit(int keyword)
        {
            if (this.emits == null)
            {
                //this.emits = new HashSet<int>(Collections.reverseOrder());
                this.emits = new HashSet<int>();
            }
            this.emits.Add(keyword);
        }

        /**
         * 获取最大的值
         * @return
         */
        public int getLargestValueId()
        {
            //if (emits == null || emits.Count == 0) return null;
            //return emits.iterator().next();

            if (emits == null || emits.Count == 0) return 0;
            return emits.Max();
        }

        /**
         * 添加一些匹配到的模式串
         * @param emits
         */
        public void addEmit(IEnumerable<int> emits)
        {
            foreach (int emit in emits)
            {
                addEmit(emit);
            }
        }

        /**
         * 获取这个节点代表的模式串（们）
         * @return
         */
        public IEnumerable<int> emit()
        {
            //return this.emits == null ? IEnumerable< int > emptyList() : this.emits;
            return this.emits;
        }

        /**
         * 是否是终止状态
         * @return
         */
        public bool isAcceptable()
        {
            return this.depth > 0 && this.emits != null;
        }

        /**
         * 获取failure状态
         * @return
         */
        public State Failure()
        {
            return this.failure;
        }

        /**
         * 设置failure状态
         * @param failState
         */
        public void setFailure(State failState, int[] fail)
        {
            this.failure = failState;
            fail[index] = failState.index;
        }

        /**
         * 转移到下一个状态
         * @param character 希望按此字符转移
         * @param ignoreRootState 是否忽略根节点，如果是根节点自己调用则应该是true，否则为false
         * @return 转移结果
         */
        private State nextState(char character, bool ignoreRootState)
        {
            State nextState = this.success[character];
            if (!ignoreRootState && nextState == null && this.depth == 0)
            {
                nextState = this;
            }
            return nextState;
        }

        /**
         * 按照character转移，根节点转移失败会返回自己（永远不会返回null）
         * @param character
         * @return
         */
        public State nextState(char character)
        {
            return nextState(character, false);
        }

        /**
         * 按照character转移，任何节点转移失败会返回null
         * @param character
         * @return
         */
        public State nextStateIgnoreRootState(char character)
        {
            return nextState(character, true);
        }

        public State addState(char character)
        {
            State nextState = nextStateIgnoreRootState(character);
            if (nextState == null)
            {
                nextState = new State(this.depth + 1);
                this.success.Add(character, nextState);
            }
            return nextState;
        }

        public IEnumerable<State> getStates()
        {
            return this.success.Values;
        }

        public IEnumerable<char> getTransitions()
        {
            return this.success.Keys;
        }

       
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("State{");
            sb.Append("depth=").Append(depth);
            sb.Append(", ID=").Append(index);
            sb.Append(", emits=").Append(emits);
            sb.Append(", success=").Append(success.Keys);
            sb.Append(", failureID=").Append(failure == null ? "-1" : failure.index.ToString());
            sb.Append(", failure=").Append(failure);
            sb.Append('}');
            return sb.ToString();
        }

        /**
         * 获取goto表
         * @return
         */
        public Dictionary<char, State> getSuccess()
        {
            return success;
        }

        public int getIndex()
        {
            return index;
        }

        public void setIndex(int index)
        {
            this.index = index;
        }
    }
}
