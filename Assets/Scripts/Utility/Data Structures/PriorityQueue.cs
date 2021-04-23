#region Copyright � ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

// Microsoft Reciprocal License (Ms-RL)
//
// This license governs use of the accompanying software. If you use the software, you accept this
// license. If you do not accept the license, do not use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same
// meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// copyright license to reproduce its contribution, prepare derivative works of its contribution,
// and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or
// otherwise dispose of its contribution in the software or derivative works of the contribution in
// the software.
//
// 3. Conditions and Limitations
// (A) Reciprocal Grants- For any file you distribute that contains code from the software (in
// source code or binary format), you must provide recipients the source code to that file along
// with a copy of this license, which license will govern that file. You may license other files
// that are entirely your own work and do not contain code from the software under any terms you
// choose.
// (B) No Trademark License- This license does not grant you rights to use any contributors' name,
// logo, or trademarks.
// (C) If you bring a patent claim against any contributor over patents that you claim are
// infringed by the software, your patent license from such contributor to the software ends
// automatically.
// (D) If you distribute any portion of the software, you must retain all copyright, patent,
// trademark, and attribution notices that are present in the software.
// (E) If you distribute any portion of the software in source code form, you may do so only under
// this license by including a complete copy of this license with your distribution. If you
// distribute any portion of the software in compiled or object code form, you may only do so under
// a license that complies with this license.
// (F) The software is licensed "as-is." You bear the risk of using it. The contributors give no
// express warranties, guarantees or conditions. You may have additional consumer rights under your
// local laws which this license cannot change. To the extent permitted under your local laws, the
// contributors exclude the implied warranties of merchantability, fitness for a particular purpose
// and non-infringement.

#endregion Copyright � ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

#region Probably derived from a tutorial by Jim Mischel. Copyright status unknown.

// I suspect this is derived from a tutorial by Jim Mischel.

#endregion Probably derived from a tutorial by Jim Mischel. Copyright status unknown.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Utility.DataStructures {
    /// <summary>
    ///     A priority queue item whose value is of type
    ///     <typeparamref name="TValue" /> and whose priority is of type
    ///     <typeparamref name="TPriority" />
    /// </summary>
    [ComVisible(false)]
    public struct PriorityQueueItem<TValue, TPriority> {
        internal PriorityQueueItem(TValue val, TPriority pri){
            Value = val;
            Priority = pri;
        }

        /// <summary>
        ///     Accessor for <see cref="Priority" />
        /// </summary>
        public TPriority Priority { get; }

        /// <summary>
        ///     Accessor for <see cref="Value" />
        /// </summary>
        public TValue Value { get; }
    }

    /// <summary>
    ///     A Priority Queue class
    /// </summary>
    [ComVisible(false)]
    public class PriorityQueue<TValue, TPriority> : ICollection, IEnumerable<PriorityQueueItem<TValue, TPriority>> {
        /// <summary>
        ///     Priority order (highest first or lowest first)
        /// </summary>
        public enum PriorityOrder {
            HighFirst,
            LowFirst
        }

        private const int DEFAULT_CAPACITY = 16;
        private int _capacity;
        private Comparison<TPriority> _compareFunc;
        private PriorityQueueItem<TValue, TPriority>[] _items;
        private int _prioritySign;

        /// <summary>
        ///     Initializes a new instance of the PriorityQueue class that is empty,
        ///     has the default initial capacity, and uses the default comparer and
        ///     is ordered high first.
        ///     <see cref="IComparer" />.
        /// </summary>
        public PriorityQueue()
            : this(DEFAULT_CAPACITY, Comparer<TPriority>.Default, PriorityOrder.HighFirst){
        }

        /// <summary>
        ///     Initializes a new instance of the PriorityQueue class that is empty,
        ///     has the default initial capacity, and uses the default comparer and
        ///     is ordered as specified.
        ///     <see cref="IComparer" />.
        /// </summary>
        /// <param name="priorityOrder">
        ///     The priority order.
        /// </param>
        public PriorityQueue(PriorityOrder priorityOrder)
            : this(DEFAULT_CAPACITY, Comparer<TPriority>.Default, priorityOrder){
        }

        /// <summary>
        ///     Initializes a new instance of the PriorityQueue class that is empty,
        ///     has the given initial capacity, and uses the default comparer and
        ///     is ordered high first.
        /// </summary>
        /// <param name="initialCapacity">
        /// </param>
        public PriorityQueue(int initialCapacity)
            : this(initialCapacity, Comparer<TPriority>.Default, PriorityOrder.HighFirst){
        }

        /// <summary>
        ///     Initializes a new instance of the PriorityQueue class that is empty,
        ///     has the default initial capacity, and uses the given comparer and
        ///     is ordered high first.
        /// </summary>
        /// <param name="comparer">
        /// </param>
        public PriorityQueue(IComparer<TPriority> comparer)
            : this(DEFAULT_CAPACITY, comparer, PriorityOrder.HighFirst){
        }

        /// <summary>
        ///     Initializes a new instance of the PriorityQueue class that is empty,
        ///     has the given initial capacity, and uses the given comparer and
        ///     is ordered high first.
        /// </summary>
        /// <param name="initialCapacity">
        /// </param>
        /// <param name="comparer">
        /// </param>
        public PriorityQueue(int initialCapacity, IComparer<TPriority> comparer){
            Init(initialCapacity, comparer.Compare, PriorityOrder.HighFirst);
        }

        /// <summary>
        ///     Initializes a new instance of the PriorityQueue class that is empty,
        ///     has the default initial capacity, and uses the given comparison and
        ///     is ordered high first.
        /// </summary>
        /// <param name="comparison">
        /// </param>
        public PriorityQueue(Comparison<TPriority> comparison)
            : this(DEFAULT_CAPACITY, comparison, PriorityOrder.HighFirst){
        }

        /// <summary>
        ///     Initializes a new instance of the PriorityQueue class that is empty,
        ///     has the given initial capacity, and uses the given comparison and
        ///     is ordered high first.
        /// </summary>
        /// <param name="initialCapacity">
        /// </param>
        /// <param name="comparison">
        /// </param>
        public PriorityQueue(int initialCapacity, Comparison<TPriority> comparison){
            Init(initialCapacity, comparison, PriorityOrder.HighFirst);
        }

        /// <summary>
        ///     Initializes a new instance of the PriorityQueue class that is empty,
        ///     has the given initial capacity, and uses the default comparer and
        ///     has the given priority order.
        /// </summary>
        public PriorityQueue(int initialCapacity, PriorityOrder priorityOrder)
            : this(initialCapacity, Comparer<TPriority>.Default, priorityOrder){
        }

        /// <summary>
        ///     Initializes a new instance of the PriorityQueue class that is empty,
        ///     has the default initial capacity, and uses the given comparer and
        ///     has the given priority order.
        /// </summary>
        public PriorityQueue(IComparer<TPriority> comparer, PriorityOrder priorityOrder)
            : this(DEFAULT_CAPACITY, comparer, priorityOrder){
        }

        /// <summary>
        ///     Initializes a new instance of the PriorityQueue class that is empty,
        ///     has the given initial capacity, and uses the given comparer and
        ///     has the given priority order.
        /// </summary>
        public PriorityQueue(int initialCapacity, IComparer<TPriority> comparer, PriorityOrder priorityOrder){
            Init(initialCapacity, comparer.Compare, priorityOrder);
        }

        /// <summary>
        ///     Initializes a new instance of the PriorityQueue class that is empty,
        ///     has the default initial capacity, and uses the given comparison and
        ///     has the given priority order.
        /// </summary>
        public PriorityQueue(Comparison<TPriority> comparison, PriorityOrder priorityOrder)
            : this(DEFAULT_CAPACITY, comparison, priorityOrder){
        }

        /// <summary>
        ///     Initializes a new instance of the PriorityQueue class that is empty,
        ///     has the given initial capacity, and uses the given comparison and
        ///     has the given priority order.
        /// </summary>
        public PriorityQueue(int initialCapacity, Comparison<TPriority> comparison, PriorityOrder priorityOrder){
            Init(initialCapacity, comparison, priorityOrder);
        }

        /// <summary>
        ///     Capacity of the priority queue
        /// </summary>
        public int Capacity {
            get => _items.Length;

            set => SetCapacity(value);
        }

        /// <summary>
        ///     Gets number of items in the priority queue
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether access to this priority queue is
        ///     synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized => false;

        /// <summary>
        ///     Gets an object that can be used to synchronize access to this
        ///     priority queue.
        /// </summary>
        public object SyncRoot => _items.SyncRoot;

        /// <summary>
        ///     Copy to array starting at given index.
        /// </summary>
        public void CopyTo(Array array, int index){
            CopyTo((PriorityQueueItem<TValue, TPriority>[]) array, index);
        }

        IEnumerator IEnumerable.GetEnumerator(){
            return GetEnumerator();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through this priority queue.
        /// </summary>
        /// <returns>
        ///     an enumerator that iterates through this priority queue.
        /// </returns>
        public IEnumerator<PriorityQueueItem<TValue, TPriority>> GetEnumerator(){
            for (var i = 0; i < Count; i++){
                yield return _items[i];
            }
        }

        /// <summary>
        ///     Clear priority queue
        /// </summary>
        public void Clear(){
            for (var i = 0; i < Count; ++i){
                _items[i] = default;
            }

            Count = 0;
            TrimExcess();
        }

        /// <summary>
        ///     Tests if priority queue contains the given value
        /// </summary>
        /// <param name="value">
        /// </param>
        /// <returns>
        /// </returns>
        public bool Contains(TValue value){
            foreach (var x in _items){
                if (x.Value.Equals(value)) return true;
            }

            return false;
        }

        /// <summary>
        ///     Copy to array starting at given array index.
        /// </summary>
        public void CopyTo(PriorityQueueItem<TValue, TPriority>[] array, int arrayIndex){
            if (array == null) throw new ArgumentNullException("array");

            if (arrayIndex < 0) throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");

            if (array.Rank > 1) throw new ArgumentException("array is multidimensional.");

            if (Count == 0) return;

            if (arrayIndex >= array.Length)
                throw new ArgumentException("arrayIndex is equal to or greater than the length" + " of the array.");

            if (Count > array.Length - arrayIndex){
                throw new ArgumentException(
                    "The number of elements in the source ICollection is" +
                    " greater than the available space from arrayIndex to" + " the end of the destination array.");
            }

            for (var i = 0; i < Count; i++){
                array[arrayIndex + i] = _items[i];
            }
        }

        /// <summary>
        ///     Dequeue
        /// </summary>
        /// <returns>
        ///     item (value and priority)
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     empty queue
        /// </exception>
        public PriorityQueueItem<TValue, TPriority> Dequeue(){
            if (Count == 0) throw new InvalidOperationException("The queue is empty");

            return RemoveAt(0);
        }

        /// <summary>
        ///     Enqueue <paramref name="newItem" />
        /// </summary>
        public void Enqueue(PriorityQueueItem<TValue, TPriority> newItem){
            if (Count == _capacity)
                // need to increase capacity
                // grow by 50 percent
                SetCapacity(3 * Capacity / 2);

            var i = Count;
            ++Count;
            while (i > 0 && _prioritySign * _compareFunc(_items[(i - 1) / 2].Priority, newItem.Priority) < 0){
                _items[i] = _items[(i - 1) / 2];
                i = (i - 1) / 2;
            }

            _items[i] = newItem;

            // if (!VerifyQueue())
            // {
            //      Debug.Log("ERROR: Queue out of order!");
            // }
        }

        /// <summary>
        ///     Enqueue <paramref name="value" /> with <paramref name="priority" />
        /// </summary>
        public void Enqueue(TValue value, TPriority priority){
            Enqueue(new PriorityQueueItem<TValue, TPriority>(value, priority));
        }

        /// <summary>
        ///     Get (peek but not dequeue) first item
        /// </summary>
        /// <returns>
        ///     first item in the priority queue
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     empty queue
        /// </exception>
        public PriorityQueueItem<TValue, TPriority> Peek(){
            if (Count == 0) throw new InvalidOperationException("The queue is empty.");

            return _items[0];
        }

        /// <summary>
        ///     Removes the item with the specified value from the queue.
        ///     The passed equality comparison is used.
        /// </summary>
        /// <param name="item">
        ///     The item to be removed.
        /// </param>
        /// <param name="comparer">
        ///     An object that implements the <see cref="IEqualityComparer" />
        ///     interface for the type of item in the collection.
        /// </param>
        /// <exception cref="ApplicationException">The specified item is not in the queue.</exception>
        public void Remove(TValue item, IEqualityComparer comparer){
            // need to find the PriorityQueueItem that has the Data value of item
            for (var index = 0; index < Count; ++index){
                if (!comparer.Equals(item, _items[index].Value)) continue;

                RemoveAt(index);
                return;
            }

            throw new Exception("The specified item is not in the queue.");
        }

        /// <summary>
        ///     Removes the item with the specified value from the queue.
        ///     The default type comparison function is used.
        /// </summary>
        /// <param name="item">
        ///     The item to be removed.
        /// </param>
        public void Remove(TValue item){
            Remove(item, EqualityComparer<TValue>.Default);
        }

        public bool Remove(Predicate<TValue> match){
            if (match == null) throw new ArgumentNullException("match");

            var matchedAtLeastOne = false;

            for (var index = 0; index < Count; ++index){
                if (!match(_items[index].Value)) continue;
                RemoveAt(index);
                index--;
                matchedAtLeastOne = true;
            }

            return matchedAtLeastOne;
        }

        /// <summary>
        ///     Set the capacity to the actual number of items, if the current
        ///     number of items is less than 90 percent of the current capacity.
        /// </summary>
        public void TrimExcess(){
            if (Count < (float) 0.9 * _capacity) SetCapacity(Count);
        }

        /// <summary>
        ///     Function to check that the queue is coherent.
        /// </summary>
        public bool VerifyQueue(){
            var i = 0;
            while (i < Count / 2){
                var leftChild = 2 * i + 1;
                var rightChild = leftChild + 1;
                if (_prioritySign * _compareFunc(_items[i].Priority, _items[leftChild].Priority) < 0) return false;

                if (rightChild < Count &&
                    _prioritySign * _compareFunc(_items[i].Priority, _items[rightChild].Priority) < 0)
                    return false;

                ++i;
            }

            return true;
        }

        private void Init(int initialCapacity, Comparison<TPriority> comparison, PriorityOrder priorityOrder){
            Count = 0;
            _compareFunc = comparison;
            SetCapacity(initialCapacity);

            // multiplier to apply to result of compareFunc
            // 1 for high priority first, -1 for low priority first
            _prioritySign = priorityOrder == PriorityOrder.HighFirst ? 1 : -1;
        }

        private PriorityQueueItem<TValue, TPriority> RemoveAt(int index){
            var o = _items[index];
            --Count;

            // move the last item to fill the hole
            var tmp = _items[Count];

            // If you forget to clear this, you have a potential memory leak.
            _items[Count] = default;
            if (Count <= 0 || index == Count) return o;
            // If the new item is greater than its parent, bubble up.
            var i = index;
            var parent = (i - 1) / 2;
            while (_prioritySign * _compareFunc(tmp.Priority, _items[parent].Priority) > 0){
                _items[i] = _items[parent];
                i = parent;
                parent = (i - 1) / 2;
            }

            // if i == index, then we didn't move the item up
            if (i == index)
                // bubble down ...
            {
                while (i < Count / 2){
                    var j = 2 * i + 1;
                    if (j < Count - 1 &&
                        _prioritySign * _compareFunc(_items[j].Priority, _items[j + 1].Priority) < 0)
                        ++j;

                    if (_prioritySign * _compareFunc(_items[j].Priority, tmp.Priority) <= 0) break;

                    _items[i] = _items[j];
                    i = j;
                }
            }

            // Be sure to store the item in its place.
            _items[i] = tmp;

            // if (!VerifyQueue())
            // {
            //     Debug.Log("ERROR: Queue out of order!");
            // }
            return o;
        }

        private void SetCapacity(int newCapacity){
            var newCap = newCapacity;
            if (newCap < DEFAULT_CAPACITY) newCap = DEFAULT_CAPACITY;

            // throw exception if newCapacity < NumItems
            if (newCap < Count) throw new ArgumentOutOfRangeException("newCapacity", "New capacity is less than Count");

            _capacity = newCap;
            if (_items == null){
                _items = new PriorityQueueItem<TValue, TPriority>[newCap];
                return;
            }

            // Resize the array.
            Array.Resize(ref _items, newCap);
        }
    }
}