/* Dictionary.cs
 * Author: Rod Howell
 * Modified by: Samuel McGowan
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KansasStateUniversity.TreeViewer2;
using Ksu.Cis300.ImmutableBinaryTrees;

namespace Ksu.Cis300.NameLookup
{
    /// <summary>
    /// A generic dictionary in which keys must implement IComparable.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class Dictionary<TKey, TValue> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// The keys and values in the dictionary.
        /// </summary>
        private BinaryTreeNode<KeyValuePair<TKey, TValue>> _elements = null;

        /// <summary>
        /// Gets a drawing of the underlying binary search tree.
        /// </summary>
        public TreeForm Drawing => new TreeForm(_elements, 100);

        /// <summary>
        /// Checks to see if the given key is null, and if so, throws an
        /// ArgumentNullException.
        /// </summary>
        /// <param name="key">The key to check.</param>
        private static void CheckKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Finds the given key in the given binary search tree.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="t">The binary search tree.</param>
        /// <returns>The node containing key, or null if the key is not found.</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Find(TKey key, BinaryTreeNode<KeyValuePair<TKey, TValue>> t)
        {
            if (t == null)
            {
                return null;
            }
            else
            {
                int comp = key.CompareTo(t.Data.Key);
                if (comp == 0)
                {
                    return t;
                }
                else if (comp < 0)
                {
                    return Find(key, t.LeftChild);
                }
                else
                {
                    return Find(key, t.RightChild);
                }
            }
        }

        /// <summary>
        /// Builds the binary search tree that results from adding the given key and value to the given tree.
        /// If the tree already contains the given key, throws an ArgumentException.
        /// </summary>
        /// <param name="t">The binary search tree.</param>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        /// <returns>The binary search tree that results from adding k and v to t.</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Add(BinaryTreeNode<KeyValuePair<TKey, TValue>> t, TKey k, TValue v)
        {
            if (t == null)
            {
                return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(k, v), null, null);
            }
            else
            {
                int comp = k.CompareTo(t.Data.Key);
                if (comp == 0)
                {
                    throw new ArgumentException();
                }
                else if (comp < 0)
                {
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, Add(t.LeftChild, k, v), t.RightChild);
                }
                else
                {
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, t.LeftChild, Add(t.RightChild, k, v));
                }
            }
        }

        /// <summary>
        /// Tries to get the value associated with the given key.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value associated with k, or the default value if
        /// k is not in the dictionary.</param>
        /// <returns>Whether k was found as a key in the dictionary.</returns>
        public bool TryGetValue(TKey k, out TValue v)
        {
            CheckKey(k);
            BinaryTreeNode<KeyValuePair<TKey, TValue>> p = Find(k, _elements);
            if (p == null)
            {
                v = default(TValue);
                return false;
            }
            else
            {
                v = p.Data.Value;
                return true;
            }
        }

        /// <summary>
        /// Adds the given key with the given associated value.
        /// If the given key is already in the dictionary, throws an
        /// InvalidOperationException.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        public void Add(TKey k, TValue v)
        {
            CheckKey(k);
            _elements = Add(_elements, k, v);
        }

        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> RemoveMininumKey(BinaryTreeNode<KeyValuePair<TKey, TValue>> t, out KeyValuePair<TKey, TValue> min)
        {
            if (t.LeftChild == null)
            {
                min = t.Data;
                return (t.RightChild);
            }
            else
            {
                BinaryTreeNode<KeyValuePair<TKey, TValue>> left = RemoveMininumKey(t.LeftChild, out min);
                return (new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, left, t.RightChild));
            }
        }

        /// <summary>
        /// Returns the result of removing the node containing the given key from the given binary search tree
        /// </summary>
        /// <param name="key">The key you want removed</param>
        /// <param name="t">The tree you are wanting k to be removed from</param>
        /// <param name="removed">Output of whether or not it was removed</param>
        /// <returns>The tree with the key removed</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Remove(TKey key, BinaryTreeNode<KeyValuePair<TKey, TValue>> t, out bool removed)
        {
            //The tree is empty
            if (t == null)
            {
                removed = false;
                return (null);
            }
            else
            {
                int result = key.CompareTo(t.Data.Key);
                //The key we are looking for is at the root
                if (result == 0)
                {
                    removed = true;
                    //The left child is empty
                    if (t.LeftChild == null)
                    {
                        return (t.RightChild);
                    }
                    //The right child is empty
                    else if (t.RightChild == null)
                    {
                        return (t.LeftChild);
                    }
                    //Both children are non empty
                    else
                    {
                        KeyValuePair<TKey, TValue> min;
                        BinaryTreeNode<KeyValuePair<TKey, TValue>> right = RemoveMininumKey(t.RightChild, out min);
                        return (new BinaryTreeNode<KeyValuePair<TKey, TValue>>(min, t.LeftChild, right));
                    }
                }
                //The key we are looking for is less than the key at the root
                else if (result < 0)
                {
                    return (new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, Remove(key, t.LeftChild, out removed), t.RightChild));
                }
                //The key we are looking for is greater than the key at the root
                else
                {
                    return (new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, t.LeftChild, Remove(key, t.RightChild, out removed)));
                }
            }
        }

        /// <summary>
        /// Removes the given key and its associated value from the dictionary
        /// </summary>
        /// <param name="k">The key you want removed</param>
        /// <returns>True if the key is removed, false if not</returns>
        public bool Remove(TKey k)
        {
            CheckKey(k);
            bool removed;
            BinaryTreeNode<KeyValuePair<TKey, TValue>> tree = Remove(k, _elements, out removed);
            if(removed == true)
            {
                _elements = tree;
            }
            return (removed);
        }
    }
}
