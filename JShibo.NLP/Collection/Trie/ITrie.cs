using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.NLP.Collection.Trie
{
    /**
 * trie树接口
 * @author hankcs
 */
    public interface ITrie<V>
    {
        int build(Dictionary<String, V> keyValueMap);
        //bool save(DataOutputStream out);
        //bool load(ByteArray byteArray, V[] value);
        V get(char[] key);
        V get(String key);
        V[] getValueArray(V[] a);
        bool containsKey(String key);
        int size();
    }
}
