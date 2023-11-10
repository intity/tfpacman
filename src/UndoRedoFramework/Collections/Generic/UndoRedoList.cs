//
// Author: Siarhei Arkhipenka (c) 2006-2007
// E-mail: sbs-arhipenko@yandex.ru
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace UndoRedoFramework.Collections.Generic
{
    public class UndoRedoList<T> : IUndoRedoMember, IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        List<T> list;

        /// <summary>
        /// Initializes a new instance of the System.Collections.Generic.List<T> class
        /// that is empty and has the default initial capacity.
        /// </summary>
        public UndoRedoList()
        {
            list = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Generic.List<T> class
        /// that contains elements copied from the specified collection and has sufficient
        /// capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements are copied to the new list.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Collection is null.
        /// </exception>
        public UndoRedoList(IEnumerable<T> collection)
        {
            list = new List<T>(collection);
        }

        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can
        /// hold without resizing.
        /// </summary>
        /// <returns>
        /// The number of elements that the System.Collections.Generic.List<T> can contain
        /// before resizing is required.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// System.Collections.Generic.List<T>.Capacity is set to a value that is less
        /// than System.Collections.Generic.List<T>.Count.
        /// </exception>
        public int Capacity
        {
            get { return list.Capacity; }
            set { list.Capacity = value; }
        }

        /// <summary>
        /// Gets the number of elements actually contained in the System.Collections.Generic.List<T>.
        /// </summary>
        /// <returns>
        /// The number of elements actually contained in the System.Collections.Generic.List<T>.
        /// </returns>
        public int Count => list.Count;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to get or set.
        /// </param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is less than 0.-or-index is equal to or greater than System.Collections.Generic.List<T>.Count.
        /// </exception>
        public T this[int index]
        {
            get => list[index];
            set
            {
                Enlist();
                list[index] = value;
            }
        }

        /// <summary>
        /// Adds an object to the end of the System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="item">
        /// The object to be added to the end of the System.Collections.Generic.List<T>.
        /// The value can be null for reference types.
        /// </param>
        public void Add(T item)
        {
            Enlist();
            list.Add(item);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the System.Collections.Generic.List<T>.
        /// The collection itself cannot be null, but it can contain elements that are
        /// null, if type T is a reference type.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Collection is null.
        /// </exception>
        public void AddRange(IEnumerable<T> collection)
        {
            Enlist();
            list.AddRange(collection);
        }

        /// <summary>
        /// Returns a read-only System.Collections.Generic.IList<T> wrapper for the current
        /// collection.
        /// </summary>
        /// <returns>
        /// A System.Collections.Generic.ReadOnlyCollection`1 that acts as a read-only
        /// wrapper around the current System.Collections.Generic.List<T>.
        /// </returns>
        public ReadOnlyCollection<T> AsReadOnly()
        {
            return list.AsReadOnly();
        }

        /// <summary>
        /// Searches the entire sorted System.Collections.Generic.List<T> for an element
        /// using the default comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">
        /// The object to locate. The value can be null for reference types.
        /// </param>
        /// <returns>
        /// The zero-based index of item in the sorted System.Collections.Generic.List<T>,
        /// if item is found; otherwise, a negative number that is the bitwise complement
        /// of the index of the next element that is larger than item or, if there is
        /// no larger element, the bitwise complement of System.Collections.Generic.List<T>.Count.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// The default comparer System.Collections.Generic.Comparer<T>.Default cannot
        /// find an implementation of the System.IComparable<T> generic interface or
        /// the System.IComparable interface for type T.
        /// </exception>
        public int BinarySearch(T item)
        {
            return list.BinarySearch(item);
        }

        /// <summary>
        /// Searches the entire sorted System.Collections.Generic.List<T> for an element
        /// using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">
        /// The object to locate. The value can be null for reference types.
        /// </param>
        /// <param name="comparer">
        /// The System.Collections.Generic.IComparer<T> implementation to use when comparing
        /// elements.-or-null to use the default comparer System.Collections.Generic.Comparer<T>.Default.
        /// </param>
        /// <returns>
        /// The zero-based index of item in the sorted System.Collections.Generic.List<T>,
        /// if item is found; otherwise, a negative number that is the bitwise complement
        /// of the index of the next element that is larger than item or, if there is
        /// no larger element, the bitwise complement of System.Collections.Generic.List<T>.Count.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// comparer is null, and the default comparer System.Collections.Generic.Comparer<T>.Default
        /// cannot find an implementation of the System.IComparable<T> generic interface
        /// or the System.IComparable interface for type T.
        /// </exception>
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return list.BinarySearch(item, comparer);
        }

        /// <summary>
        /// Searches a range of elements in the sorted System.Collections.Generic.List<T>
        /// for an element using the specified comparer and returns the zero-based index
        /// of the element.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index of the range to search.
        /// </param>
        /// <param name="count">
        /// The length of the range to search.
        /// </param>
        /// <param name="item">
        /// The object to locate. The value can be null for reference types.
        /// </param>
        /// <param name="comparer">
        /// The System.Collections.Generic.IComparer<T> implementation to use when comparing
        /// elements, or null to use the default comparer System.Collections.Generic.Comparer<T>.Default.
        /// </param>
        /// <returns>
        /// The zero-based index of item in the sorted System.Collections.Generic.List<T>,
        /// if item is found; otherwise, a negative number that is the bitwise complement
        /// of the index of the next element that is larger than item or, if there is
        /// no larger element, the bitwise complement of System.Collections.Generic.List<T>.Count.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is less than 0.-or-count is less than 0.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// comparer is null, and the default comparer System.Collections.Generic.Comparer<T>.Default
        /// cannot find an implementation of the System.IComparable<T> generic interface
        /// or the System.IComparable interface for type T.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// index and count do not denote a valid range in the System.Collections.Generic.List<T>.
        /// </exception>
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return list.BinarySearch(index, count, item, comparer);
        }

        /// <summary>
        /// Removes all elements from the System.Collections.Generic.List<T>.
        /// </summary>
        public void Clear()
        {
            Enlist(false);
        }

        /// <summary>
        /// Determines whether an element is in the System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the System.Collections.Generic.List<T>. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// true if item is found in the System.Collections.Generic.List<T>; otherwise,
        /// false.
        /// </returns>
        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// Converts the elements in the current System.Collections.Generic.List<T> to
        /// another type, and returns a list containing the converted elements.
        /// </summary>
        /// <param name="converter">
        /// A System.Converter<TInput,TOutput> delegate that converts each element from
        /// one type to another type.
        /// </param>
        /// <returns>
        /// A System.Collections.Generic.List<T> of the target type containing the converted
        /// elements from the current System.Collections.Generic.List<T>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Converter is null.
        /// </exception>
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            return list.ConvertAll(converter);
        }

        /// <summary>
        /// Copies the entire System.Collections.Generic.List<T> to a compatible one-dimensional
        /// array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional System.Array that is the destination of the elements
        /// copied from System.Collections.Generic.List<T>. The System.Array must have
        /// zero-based indexing.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// The number of elements in the source System.Collections.Generic.List<T> is
        /// greater than the number of elements that the destination array can contain.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// array is null.
        /// </exception>
        public void CopyTo(T[] array)
        {
            list.CopyTo(array);
        }

        /// <summary>
        /// Copies the entire System.Collections.Generic.List<T> to a compatible one-dimensional
        /// array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional System.Array that is the destination of the elements
        /// copied from System.Collections.Generic.List<T>. The System.Array must have
        /// zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in array at which copying begins.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// arrayIndex is equal to or greater than the length of array.-or-The number
        /// of elements in the source System.Collections.Generic.List<T> is greater than
        /// the available space from arrayIndex to the end of the destination array.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// arrayIndex is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// array is null.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies a range of elements from the System.Collections.Generic.List<T> to
        /// a compatible one-dimensional array, starting at the specified index of the
        /// target array.
        /// </summary>
        /// <param name="index">
        /// The zero-based index in the source System.Collections.Generic.List<T> at
        /// which copying begins.
        /// </param>
        /// <param name="array">
        /// The one-dimensional System.Array that is the destination of the elements
        /// copied from System.Collections.Generic.List<T>. The System.Array must have
        /// zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in array at which copying begins.
        /// </param>
        /// <param name="count">
        /// The number of elements to copy.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// array is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is less than 0.-or-arrayIndex is less than 0.-or-count is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// index is equal to or greater than the System.Collections.Generic.List<T>.Count
        /// of the source System.Collections.Generic.List<T>.-or-arrayIndex is equal
        /// to or greater than the length of array.-or-The number of elements from index
        /// to the end of the source System.Collections.Generic.List<T> is greater than
        /// the available space from arrayIndex to the end of the destination array.
        /// </exception>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            list.CopyTo(index, array, arrayIndex, count);
        }

        /// <summary>
        /// Determines whether the System.Collections.Generic.List<T> contains elements
        /// that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        /// The System.Predicate<T> delegate that defines the conditions of the elements
        /// to search for.
        /// </param>
        /// <returns>
        /// true if the System.Collections.Generic.List<T> contains one or more elements
        /// that match the conditions defined by the specified predicate; otherwise,
        /// false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// match is null.
        /// </exception>
        public bool Exists(Predicate<T> match)
        {
            return list.Exists(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the first occurrence within the entire System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="match">
        /// The System.Predicate<T> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The first element that matches the conditions defined by the specified predicate,
        /// if found; otherwise, the default value for type T.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// match is null.
        /// </exception>
        public T Find(Predicate<T> match)
        {
            return list.Find(match);
        }

        /// <summary>
        /// Retrieves the all the elements that match the conditions defined by the specified
        /// predicate.
        /// </summary>
        /// <param name="match">
        /// The System.Predicate<T> delegate that defines the conditions of the elements
        /// to search for.
        /// </param>
        /// <returns>
        /// A System.Collections.Generic.List<T> containing all the elements that match
        /// the conditions defined by the specified predicate, if found; otherwise, an
        /// empty System.Collections.Generic.List<T>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// match is null.
        /// </exception>
        public List<T> FindAll(Predicate<T> match)
        {
            return list.FindAll(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the zero-based index of the first occurrence within
        /// the entire System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="match">
        /// The System.Predicate<T> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// match is null.
        /// </exception>
        public int FindIndex(Predicate<T> match)
        {
            return list.FindIndex(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the zero-based index of the first occurrence within
        /// the range of elements in the System.Collections.Generic.List<T> that extends
        /// from the specified index to the last element.
        /// </summary>
        /// <param name="startIndex">
        /// The zero-based starting index of the search.
        /// </param>
        /// <param name="match">
        /// The System.Predicate<T> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startIndex is outside the range of valid indexes for the System.Collections.Generic.List<T>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// match is null.
        /// </exception>
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return list.FindIndex(startIndex, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the zero-based index of the first occurrence within
        /// the range of elements in the System.Collections.Generic.List<T> that starts
        /// at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="startIndex">
        /// The zero-based starting index of the search.
        /// </param>
        /// <param name="count">
        /// The number of elements in the section to search.
        /// </param>
        /// <param name="match">
        /// The System.Predicate<T> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startIndex is outside the range of valid indexes for the System.Collections.Generic.List<T>.-or-count
        /// is less than 0.-or-startIndex and count do not specify a valid section in
        /// the System.Collections.Generic.List<T>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// match is null.
        /// </exception>
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return list.FindIndex(startIndex, count, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the last occurrence within the entire System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="match">
        /// The System.Predicate<T> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The last element that matches the conditions defined by the specified predicate,
        /// if found; otherwise, the default value for type T.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// match is null.
        /// </exception>
        public T FindLast(Predicate<T> match)
        {
            return list.FindLast(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the zero-based index of the last occurrence within
        /// the entire System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="match">
        /// The System.Predicate<T> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// match is null.
        /// </exception>
        public int FindLastIndex(Predicate<T> match)
        {
            return list.FindLastIndex(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the zero-based index of the last occurrence within
        /// the range of elements in the System.Collections.Generic.List<T> that extends
        /// from the first element to the specified index.
        /// </summary>
        /// <param name="startIndex">
        /// The zero-based starting index of the backward search.
        /// </param>
        /// <param name="match">
        /// The System.Predicate<T> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startIndex is outside the range of valid indexes for the System.Collections.Generic.List<T>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// match is null.
        /// </exception>
        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return list.FindLastIndex(startIndex, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// predicate, and returns the zero-based index of the last occurrence within
        /// the range of elements in the System.Collections.Generic.List<T> that contains
        /// the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="startIndex">
        /// The zero-based starting index of the backward search.
        /// </param>
        /// <param name="count">
        /// The number of elements in the section to search.
        /// </param>
        /// <param name="match">
        /// The System.Predicate<T> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startIndex is outside the range of valid indexes for the System.Collections.Generic.List<T>.-or-count
        /// is less than 0.-or-startIndex and count do not specify a valid section in
        /// the System.Collections.Generic.List<T>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// match is null.
        /// </exception>
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return list.FindLastIndex(startIndex, count, match);
        }

        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="action">
        /// The System.Action<T> delegate to perform on each element of the System.Collections.Generic.List<T>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// action is null.
        /// </exception>
        public void ForEach(Action<T> action)
        {
            // even if action modifies the list, the changes will be caught by appropriate changing member
            list.ForEach(action);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the System.Collections.Generic.List<T>.
        /// </summary>
        /// <returns>
        /// A System.Collections.Generic.List<T>.Enumerator for the System.Collections.Generic.List<T>.
        /// </returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <summary>
        /// Creates a shallow copy of a range of elements in the source System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="index">
        /// The zero-based System.Collections.Generic.List<T> index at which the range starts.
        /// </param>
        /// <param name="count">
        /// The number of elements in the range.
        /// </param>
        /// <returns>
        /// A shallow copy of a range of elements in the source System.Collections.Generic.List<T>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is less than 0.-or-count is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// index and count do not denote a valid range of elements in the System.Collections.Generic.List<T>.
        /// </exception>
        public List<T> GetRange(int index, int count)
        {
            return list.GetRange(index, count);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// first occurrence within the entire System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the System.Collections.Generic.List<T>. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the entire 
        /// System.Collections.Generic.List<T>, if found; otherwise, –1.
        /// </returns>
        public virtual int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// first occurrence within the range of elements in the System.Collections.Generic.List<T>
        /// that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the System.Collections.Generic.List<T>. The value
        /// can be null for reference types.
        /// </param>
        /// <param name="index">
        /// The zero-based starting index of the search.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the range of
        /// elements in the System.Collections.Generic.List<T> that extends from index
        /// to the last element, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is outside the range of valid indexes for the System.Collections.Generic.List<T>.
        /// </exception>
        public int IndexOf(T item, int index)
        {
            return list.IndexOf(item, index);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// first occurrence within the range of elements in the System.Collections.Generic.List<T>
        /// that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the System.Collections.Generic.List<T>. The value
        /// can be null for reference types.
        /// </param>
        /// <param name="index">
        /// The zero-based starting index of the search.
        /// </param>
        /// <param name="count">
        /// The number of elements in the section to search.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the range of
        /// elements in the System.Collections.Generic.List<T> that starts at index and
        /// contains count number of elements, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is outside the range of valid indexes for the System.Collections.Generic.List<T>.-or-count
        /// is less than 0.-or-index and count do not specify a valid section in the
        /// System.Collections.Generic.List<T>.
        /// </exception>
        public int IndexOf(T item, int index, int count)
        {
            return list.IndexOf(item, index, count);
        }

        /// <summary>
        /// Inserts an element into the System.Collections.Generic.List<T> at the specified
        /// index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which item should be inserted.
        /// </param>
        /// <param name="item">
        /// The object to insert. The value can be null for reference types.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is less than 0.-or-index is greater than System.Collections.Generic.List<T>.Count.
        /// </exception>
        public void Insert(int index, T item)
        {
            Enlist();
            list.Insert(index, item);
        }

        /// <summary>
        /// Inserts the elements of a collection into the System.Collections.Generic.List<T>
        /// at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which the new elements should be inserted.
        /// </param>
        /// <param name="collection">
        /// The collection whose elements should be inserted into the System.Collections.Generic.List<T>.
        /// The collection itself cannot be null, but it can contain elements that are
        /// null, if type T is a reference type.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is less than 0.-or-index is greater than System.Collections.Generic.List<T>.Count.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// collection is null.
        /// </exception>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            Enlist();
            list.InsertRange(index, collection);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// last occurrence within the entire System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the System.Collections.Generic.List<T>. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// The zero-based index of the last occurrence of item within the entire the
        /// System.Collections.Generic.List<T>, if found; otherwise, –1.
        /// </returns>
        public int LastIndexOf(T item)
        {
            return list.LastIndexOf(item);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// last occurrence within the range of elements in the System.Collections.Generic.List<T>
        /// that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the System.Collections.Generic.List<T>. The value
        /// can be null for reference types.
        /// </param>
        /// <param name="index">
        /// The zero-based starting index of the backward search.
        /// </param>
        /// <returns>
        /// The zero-based index of the last occurrence of item within the range of elements
        /// in the System.Collections.Generic.List<T> that extends from the first element
        /// to index, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is outside the range of valid indexes for the System.Collections.Generic.List<T>.
        /// </exception>
        public int LastIndexOf(T item, int index)
        {
            return list.LastIndexOf(item, index);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// last occurrence within the range of elements in the System.Collections.Generic.List<T>
        /// that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the System.Collections.Generic.List<T>. The value
        /// can be null for reference types.
        /// </param>
        /// <param name="index">
        /// The zero-based starting index of the backward search.
        /// </param>
        /// <param name="count">
        /// The number of elements in the section to search.
        /// </param>
        /// <returns>
        /// The zero-based index of the last occurrence of item within the range of elements
        /// in the System.Collections.Generic.List<T> that contains count number of elements
        /// and ends at index, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is outside the range of valid indexes for the System.Collections.Generic.List<T>.-or-count
        /// is less than 0.-or-index and count do not specify a valid section in the
        /// System.Collections.Generic.List<T>.
        /// </exception>
        public int LastIndexOf(T item, int index, int count)
        {
            return list.LastIndexOf(item, index, count);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="item">
        /// The object to remove from the System.Collections.Generic.List<T>. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// true if item is successfully removed; otherwise, false.  This method also
        /// returns false if item was not found in the System.Collections.Generic.List<T>.
        /// </returns>
        public bool Remove(T item)
        {
            Enlist();
            return list.Remove(item);
        }

        /// <summary>
        /// Removes the all the elements that match the conditions defined by the specified
        /// predicate.
        /// </summary>
        /// <param name="match">
        /// The System.Predicate<T> delegate that defines the conditions of the elements
        /// to remove.
        /// </param>
        /// <returns>
        /// The number of elements removed from the System.Collections.Generic.List<T>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// match is null.
        /// </exception>
        public int RemoveAll(Predicate<T> match)
        {
            Enlist();
            return list.RemoveAll(match);
        }

        /// <summary>
        /// Removes the element at the specified index of the System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to remove.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is less than 0.-or-index is equal to or greater than System.Collections.Generic.List<T>.Count.
        /// </exception>
        public void RemoveAt(int index)
        {
            Enlist();
            list.RemoveAt(index);
        }

        /// <summary>
        /// Removes a range of elements from the System.Collections.Generic.List<T>.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index of the range of elements to remove.
        /// </param>
        /// <param name="count">
        /// The number of elements to remove.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is less than 0.-or-count is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// index and count do not denote a valid range of elements in the System.Collections.Generic.List<T>.
        /// </exception>
        public void RemoveRange(int index, int count)
        {
            Enlist();
            list.RemoveRange(index, count);
        }

        /// <summary>
        /// Reverses the order of the elements in the entire System.Collections.Generic.List<T>.
        /// </summary>
        public void Reverse()
        {
            Enlist();
            list.Reverse();
        }

        /// <summary>
        /// Reverses the order of the elements in the specified range.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index of the range to reverse.
        /// </param>
        /// <param name="count">
        /// The number of elements in the range to reverse.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// index and count do not denote a valid range of elements in the System.Collections.Generic.List<T>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is less than 0.-or-count is less than 0.
        /// </exception>
        public void Reverse(int index, int count)
        {
            Enlist();
            list.Reverse(index, count);
        }

        /// <summary>
        /// Sorts the elements in the entire System.Collections.Generic.List<T> using
        /// the default comparer.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The default comparer System.Collections.Generic.Comparer<T>.Default cannot
        /// find an implementation of the System.IComparable<T> generic interface or
        /// the System.IComparable interface for type T.
        /// </exception>
        public void Sort()
        {
            Enlist();
            list.Sort();
        }

        /// <summary>
        /// Sorts the elements in the entire System.Collections.Generic.List<T> using
        /// the specified System.Comparison<T>.
        /// </summary>
        /// <param name="comparison">
        /// The System.Comparison<T> to use when comparing elements.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// The implementation of comparison caused an error during the sort. For example,
        /// comparison might not return 0 when comparing an item with itself.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// comparison is null.
        /// </exception>
        public void Sort(Comparison<T> comparison)
        {
            Enlist();
            list.Sort(comparison);
        }

        /// <summary>
        /// Sorts the elements in the entire System.Collections.Generic.List<T> using
        /// the specified comparer.
        /// </summary>
        /// <param name="comparer">
        /// The System.Collections.Generic.IComparer<T> implementation to use when comparing
        /// elements, or null to use the default comparer System.Collections.Generic.Comparer<T>.Default.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// The implementation of comparer caused an error during the sort. For example,
        /// comparer might not return 0 when comparing an item with itself.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// comparer is null, and the default comparer System.Collections.Generic.Comparer<T>.Default
        /// cannot find implementation of the System.IComparable<T> generic interface
        /// or the System.IComparable interface for type T.
        /// </exception>
        public void Sort(IComparer<T> comparer)
        {
            Enlist();
            list.Sort(comparer);
        }

        /// <summary>
        /// Sorts the elements in a range of elements in System.Collections.Generic.List<T>
        /// using the specified comparer.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index of the range to sort.
        /// </param>
        /// <param name="count">
        /// The length of the range to sort.
        /// </param>
        /// <param name="comparer">
        /// The System.Collections.Generic.IComparer<T> implementation to use when comparing
        /// elements, or null to use the default comparer System.Collections.Generic.Comparer<T>.Default.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// index and count do not specify a valid range in the System.Collections.Generic.List<T>.-or-The
        /// implementation of comparer caused an error during the sort. For example,
        /// comparer might not return 0 when comparing an item with itself.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is less than 0.-or-count is less than 0.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// comparer is null, and the default comparer System.Collections.Generic.Comparer<T>.Default
        /// cannot find implementation of the System.IComparable<T> generic interface
        /// or the System.IComparable interface for type T.
        /// </exception>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            Enlist();
            list.Sort(index, count, comparer);
        }

        /// <summary>
        /// Copies the elements of the System.Collections.Generic.List<T> to a new array.
        /// </summary>
        /// <returns>
        /// An array containing copies of the elements of the System.Collections.Generic.List<T>.
        /// </returns>
        public T[] ToArray()
        {
            return list.ToArray();
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the System.Collections.Generic.List<T>,
        /// if that number is less than a threshold value.
        /// </summary>
        public void TrimExcess()
        {
            // unused
        }

        /// <summary>
        /// Determines whether every element in the System.Collections.Generic.List<T>
        /// matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        /// The System.Predicate<T> delegate that defines the conditions to check against
        /// the elements.
        /// </param>
        /// <returns>
        /// true if every element in the System.Collections.Generic.List<T> matches the
        /// conditions defined by the specified predicate; otherwise, false. If the list
        /// has no elements, the return value is true.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// match is null.
        /// </exception>
        public bool TrueForAll(Predicate<T> match)
        {
            return list.TrueForAll(match);
        }

        #region Private Members
        void Enlist()
        {
            Enlist(true);
        }

        void Enlist(bool copyItems)
        {
            UndoRedoManager.AssertCommand();

            if (!UndoRedoManager.CurrentCommand.ContainsKey(this))
            {
                Change<List<T>> change = new Change<List<T>>
                {
                    OldState = list
                };
                UndoRedoManager.CurrentCommand[this] = change;
                if (copyItems)
                    list = new List<T>(list);
                else
                    list = new List<T>();
            }
        }

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)list).IsReadOnly;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }
        #endregion

        #region IUndoRedoMember Members
        void IUndoRedoMember.OnCommit(object change)
        {
            Debug.Assert(change != null);
            ((Change<List<T>>)change).NewState = list;
        }

        void IUndoRedoMember.OnUndo(object change)
        {
            Debug.Assert(change != null);
            list = ((Change<List<T>>)change).OldState;
        }

        void IUndoRedoMember.OnRedo(object change)
        {
            Debug.Assert(change != null);
            list = ((Change<List<T>>)change).NewState;
        }
        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)list).CopyTo((T[])array, index);
        }

        int ICollection.Count
        {
            get { return list.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)list).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)list).SyncRoot; }
        }

        #endregion

        #region IList Members
        int IList.Add(object value)
        {
            Enlist();
            return ((IList)list).Add((T)value);
        }

        void IList.Clear()
        {
            Enlist(false);
            ((IList)list).Clear();
        }

        bool IList.Contains(object value)
        {
            return ((IList)list).Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return ((IList)list).IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            Enlist();
            ((IList)list).Insert(index, (T)value);
        }

        bool IList.IsFixedSize => ((IList)list).IsFixedSize;

        bool IList.IsReadOnly => ((IList)list).IsReadOnly;

        void IList.Remove(object value)
        {
            Enlist();
            ((IList)list).Remove((T)value);
        }

        void IList.RemoveAt(int index)
        {
            Enlist();
            list.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                Enlist();
                list[index] = (T)value;
            }
        }
        #endregion
    }
}
