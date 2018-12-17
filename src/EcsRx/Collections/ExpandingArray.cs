using System;
using System.Collections;
using System.Collections.Generic;

namespace EcsRx.Collections
{
    public class ExpandingArray : IExpandingArray
    {
        public Array Data { get; private set; }
        public int Count { get; private set; }
        
        public ExpandingArray(Type type, int size)
        {
            Count = size;
            var dynamicArray = type.MakeArrayType();
            Data = (Array)Activator.CreateInstance(dynamicArray, size);
        }
        
        public IReadOnlyList<T> AsReadOnly<T>() => (IReadOnlyList<T>) Data;
        public T[] GetArray<T>() => (T[]) Data;
        
        public T GetItem<T>(int index) => GetArray<T>()[index];
        public object GetItem(int index) => Data.GetValue(index);
        
        public void SetItem<T>(int index, T value) => GetArray<T>()[index] = value;

        public void Expand(int amountToAdd)
        {
            var newCount = Data.Length + amountToAdd;
            var arrayType = Data.GetType();
            var newEntries = (Array)Activator.CreateInstance(arrayType, newCount);               
            Data.CopyTo(newEntries, 0);
            Data = newEntries;
            Count = newCount;
        }
    }
}

/*
public class Bag<T> : IEnumerable<T>
    {
        /// <summary>The elements.</summary>
        private T[] elements;

        /// <summary>Initializes a new instance of the <see cref="Bag{T}"/> class.</summary>
        /// <param name="capacity">The capacity.</param>
        public Bag(int capacity = 16)
        {
            this.elements = new T[capacity];
            this.Count = 0;
        }

        /// <summary>Gets the capacity.</summary>
        /// <value>The capacity.</value>
        public int Capacity
        {
            get
            {
                return this.elements.Length;
            }
        }

        /// <summary>Gets a value indicating whether this instance is empty.</summary>
        /// <value><see langword="true" /> if this instance is empty; otherwise, <see langword="false" />.</value>
        public bool IsEmpty
        {
            get
            {
                return this.Count == 0;
            }
        }

        /// <summary>Gets the size.</summary>
        /// <value>The size.</value>
        public int Count { get; private set; }

        /// <summary>Returns the element at the specified position in Bag.</summary>
        /// <param name="index">The index.</param>
        /// <returns>The element from the specified position in Bag.</returns>
        public T this[int index]
        {
            get
            {
                return this.elements[index];
            }

            set
            {
                if (index >= this.elements.Length)
                {
                    this.Grow(index * 2);
                    this.Count = index + 1;
                }
                else if (index >= this.Count)
                {
                    this.Count = index + 1;
                }

                this.elements[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified element to the end of this bag.
        /// If needed also increases the capacity of the bag.
        /// </summary>
        /// <param name="element">The element to be added to this list.</param>
        public void Add(T element)
        {
            // is size greater than capacity increase capacity
            if (this.Count == this.elements.Length)
            {
                this.Grow();
            }

            this.elements[this.Count] = element;
            ++this.Count;
        }

        /// <summary>Adds a range of elements into this bag.</summary>
        /// <param name="rangeOfElements">The elements to add.</param>
        public void AddRange(Bag<T> rangeOfElements)
        {
            for (int index = 0, j = rangeOfElements.Count; j > index; ++index)
            {
                this.Add(rangeOfElements.Get(index));
            }
        }

        /// <summary>
        /// Removes all of the elements from this bag.
        /// The bag will be empty after this call returns.
        /// </summary>
        public void Clear()
        {
            // Null all elements so garbage collector can clean up.
            for (int index = this.Count - 1; index >= 0; --index)
            {
                this.elements[index] = default(T);
            }

            this.Count = 0;
        }

        /// <summary>Determines whether bag contains the specified element.</summary>
        /// <param name="element">The element.</param>
        /// <returns><see langword="true"/> if bag contains the specified element; otherwise, <see langword="false"/>.</returns>
        public bool Contains(T element)
        {
            for (int index = this.Count - 1; index >= 0; --index)
            {
                if (element.Equals(this.elements[index]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Gets the specified index.</summary>
        /// <param name="index">The index.</param>
        /// <returns>The specified element.</returns>
        public T Get(int index)
        {
            return this.elements[index];
        }

        /// <summary>Removes the specified index.</summary>
        /// <param name="index">The index.</param>
        /// <returns>The removed element.</returns>
        public T Remove(int index)
        {
            // Make copy of element to remove so it can be returned.
            T result = this.elements[index];
            --this.Count;
            
            // Overwrite item to remove with last element.
            this.elements[index] = this.elements[this.Count];

            // Null last element, so garbage collector can do its work.
            this.elements[this.Count] = default(T);
            return result;
        }

        /// <summary>
        /// <para>Removes the first occurrence of the specified element from this Bag, if it is present.</para>
        /// <para>If the Bag does not contain the element, it is unchanged.</para>
        /// <para>Does this by overwriting it was last element then removing last element.</para>
        /// </summary>
        /// <param name="element">The element to be removed from this list, if present.</param>
        /// <returns><see langword="true"/> if this list contained the specified element, otherwise <see langword="false"/>.</returns>
        public bool Remove(T element)
        {
            for (int index = this.Count - 1; index >= 0; --index)
            {
                if (element.Equals(this.elements[index]))
                {
                    --this.Count;

                    // Overwrite item to remove with last element.
                    this.elements[index] = this.elements[this.Count];
                    this.elements[this.Count] = default(T);

                    return true;
                }
            }

            return false;
        }

        /// <summary>Removes all matching elements.</summary>
        /// <param name="bag">The bag.</param>
        /// <returns><see langword="true" /> if found matching elements, <see langword="false" /> otherwise.</returns>
        public bool RemoveAll(Bag<T> bag)
        {
            bool isResult = false;
            for (int index = bag.Count - 1; index >= 0; --index)
            {
                if (this.Remove(bag.Get(index)))
                {
                    isResult = true;
                }
            }

            return isResult;
        }

        /// <summary>Removes the last.</summary>
        /// <returns>The last element.</returns>
        public T RemoveLast()
        {
            if (this.Count > 0)
            {
                --this.Count;
                T result = this.elements[this.Count];

                // default(T) if class = null.
                this.elements[this.Count] = default(T);
                return result;
            }

            return default(T);
        }

        /// <summary>Sets the specified index.</summary>
        /// <param name="index">The index.</param>
        /// <param name="element">The element.</param>
        public void Set(int index, T element)
        {
            if (index >= this.elements.Length)
            {
                this.Grow(index * 2);
                this.Count = index + 1;
            }
            else if (index >= this.Count)
            {
                this.Count = index + 1;
            }

            this.elements[index] = element;
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerator`1" /> object that can be used to iterate through the collection.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new BagEnumerator<T>(this);
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerator`1" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BagEnumerator<T>(this);
        }

        /// <summary>Grows this instance.</summary>
        private void Grow()
        {
            this.Grow((int)(this.elements.Length * 1.5) + 1);
        }

        /// <summary>Grows the specified new capacity.</summary>
        /// <param name="newCapacity">The new capacity.</param>
        private void Grow(int newCapacity)
        {
            T[] oldElements = this.elements;
            this.elements = new T[newCapacity];
            Array.Copy(oldElements, 0, this.elements, 0, oldElements.Length);
        }
    }
*/