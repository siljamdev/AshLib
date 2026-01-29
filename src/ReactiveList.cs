using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace AshLib.Lists;

public class ReactiveList<T> : IList<T>, System.Collections.IList, IReadOnlyList<T>
{
	List<T> list; //internal list
	
	Action? OnChanged;
	
	public int Capacity{
		get{
			return list.Capacity;
		}
		set{
			list.Capacity = value;
		}
	}
	
	public int Length{
		get{
			return list.Count;
		}
	}
	
	public int Count{
		get{
			return list.Count;
		}
	}
	
	bool System.Collections.IList.IsFixedSize{
        get{
			return false;
		}
    }
	
	bool ICollection<T>.IsReadOnly{
        get{
			return false;
		}
    }
	
	bool System.Collections.IList.IsReadOnly{
        get{
			return false;
		}
    }
	
	bool System.Collections.ICollection.IsSynchronized{
        get{
			return false;
		}
    }
	
	object System.Collections.ICollection.SyncRoot{
		get{
			return ((System.Collections.ICollection)list).SyncRoot;
		}
	}
	
	public T this[int index]{
		get{
			return list[index];
		}
		set{
			list[index] = value;
			OnChanged?.Invoke();
		}
	}
	
	object System.Collections.IList.this[int index]{
		get{
			return this[index];
		}
		set{
			((System.Collections.IList)list)[index] = value;
			OnChanged?.Invoke();
		}
	}
	
	public ReactiveList(Action? onChange = null){
		list = new List<T>();
		OnChanged = onChange;
	}
	
	public ReactiveList(int capacity, Action? onChange = null){
		list = new List<T>(capacity);
		OnChanged = onChange;
	}
	
	public ReactiveList(IEnumerable<T> collection, Action? onChange = null){
		list = new List<T>(collection);
		OnChanged = onChange;
	}
	
	public void Add(T item){
		list.Add(item);
		OnChanged?.Invoke();
	}
	
	int System.Collections.IList.Add(object item){
		int t = ((System.Collections.IList)list).Add(item);
		OnChanged?.Invoke();
		return t;
	}
	
	public void AddRange(IEnumerable<T> collection){
		list.AddRange(collection);
		OnChanged?.Invoke();
	}
	
	public ReadOnlyCollection<T> AsReadOnly(){
		return list.AsReadOnly();
	}
	
	public int BinarySearch(int index, int count, T item, IComparer<T> comparer){
		return list.BinarySearch(index, count, item, comparer);
	}
	
	public int BinarySearch(T item){
		return list.BinarySearch(item);
	}
	
	public int BinarySearch(T item, IComparer<T> comparer){
		return list.BinarySearch(item, comparer);
	}
	
	public void Clear(){
		list.Clear();
		OnChanged?.Invoke();
	}
	
	public bool Contains(T item){
		return list.Contains(item);
	}
	
	bool System.Collections.IList.Contains(object item){
		return ((System.Collections.IList)list).Contains(item);
	}
	
	public ReactiveList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter){
		return new ReactiveList<TOutput>(list.ConvertAll(converter));
	}
	
	public void CopyTo(T[] array){
		list.CopyTo(array);
	}
	
	void System.Collections.ICollection.CopyTo(Array array, int arrayIndex){
		((System.Collections.IList)list).CopyTo(array, arrayIndex);
	}
	
	public void CopyTo(int index, T[] array, int arrayIndex, int count){
		list.CopyTo(index, array, arrayIndex, count);
	}
	
	public void CopyTo(T[] array, int arrayIndex){
		list.CopyTo(array, arrayIndex);
	}
	
	public bool Exists(Predicate<T> match){
		return list.Exists(match);
	}
	
	public T Find(Predicate<T> match){
		return list.Find(match);
	}
	
	public ReactiveList<T> FindAll(Predicate<T> match){
		return new ReactiveList<T>(list.FindAll(match));
	}
	
	public int FindIndex(Predicate<T> match){
		return list.FindIndex(match);
	}
	
	public int FindIndex(int startIndex, Predicate<T> match){
		return list.FindIndex(startIndex, match);
	}
	
	public int FindIndex(int startIndex, int count, Predicate<T> match){
		return list.FindIndex(startIndex, count, match);
	}
	
	public T FindLast(Predicate<T> match){
		return FindLast(match);
	}
	
	public int FindLastIndex(Predicate<T> match){
		return list.FindLastIndex(match);
	}
	
	public int FindLastIndex(int startIndex, Predicate<T> match){
		return list.FindLastIndex(startIndex, match);
	}
	
	public int FindLastIndex(int startIndex, int count, Predicate<T> match){
		return list.FindLastIndex(startIndex, count, match);
	}
	
	public void ForEach(Action<T> action){
		list.ForEach(action);
		OnChanged?.Invoke();
	}
	
	IEnumerator<T> IEnumerable<T>.GetEnumerator(){
        return ((IEnumerable<T>)list).GetEnumerator();
    }
	
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator(){
        return ((System.Collections.IEnumerable)list).GetEnumerator();
    }
	
	public ReactiveList<T> GetRange(int index, int count){
		return new ReactiveList<T>(list.GetRange(index, count));
	}
	
	public int IndexOf(T item){
		return list.IndexOf(item);
	}
	
	int System.Collections.IList.IndexOf(object item){
		return ((System.Collections.IList)list).IndexOf(item);
	}
	
	public int IndexOf(T item, int index){
		return list.IndexOf(item, index);
	}
	
	public int IndexOf(T item, int index, int count){
		return list.IndexOf(item, index, count);
	}
	
	public void Insert(int index, T item){
		list.Insert(index, item);
		OnChanged?.Invoke();
	}
	
	void System.Collections.IList.Insert(int index, Object item){
		((System.Collections.IList)list).Insert(index, item);
		OnChanged?.Invoke();
	}
	
	public void InsertRange(int index, IEnumerable<T> collection){
		list.InsertRange(index, collection);
		OnChanged?.Invoke();
	}
	
	public int LastIndexOf(T item){
		return list.LastIndexOf(item);
	}
	
	public int LastIndexOf(T item, int index){
		return list.LastIndexOf(item, index);
	}
	
	public int LastIndexOf(T item, int index, int count){
		return list.LastIndexOf(item, index, count);
	}
	
	public bool Remove(T item){
		bool b = list.Remove(item);
		OnChanged?.Invoke();
		return b;
	}
	
	void System.Collections.IList.Remove(object item){
		((System.Collections.IList)list).Remove(item);
		OnChanged?.Invoke();
	}
	
	public int RemoveAll(Predicate<T> match){
		int n = list.RemoveAll(match);
		if(n > 0){
			OnChanged?.Invoke();
		}
		return n;
	}
	
	public void RemoveAt(int index){
		list.RemoveAt(index);
		OnChanged?.Invoke();
	}
	
	public void RemoveRange(int index, int count){
		list.RemoveRange(index, count);
		OnChanged?.Invoke();
	}
	
	public void Move(int index, int newIndex){
		T item = list[index];
		list.RemoveAt(index);
		list.Insert(newIndex, item);
		OnChanged?.Invoke();
	}
	
	public void Reverse(){
		list.Reverse();
		OnChanged?.Invoke();
	}
	
	public void Reverse(int index, int count){
		list.Reverse(index, count);
		OnChanged?.Invoke();
	}
	
	public void Sort(){
		list.Sort();
		OnChanged?.Invoke();
	}
	
	public void Sort(IComparer<T> comparer){
		list.Sort(comparer);
		OnChanged?.Invoke();
	}
	
	public void Sort(int index, int count, IComparer<T> comparer){
		list.Sort(index, count, comparer);
		OnChanged?.Invoke();
	}
	
	public void Sort(Comparison<T> comparison){
		list.Sort(comparison);
		OnChanged?.Invoke();
	}
	
	public T[] ToArray(){
		return list.ToArray();
	}
	
	public void TrimExcess(){
		list.TrimExcess();
	}
	
	public bool TrueForAll(Predicate<T> match){
		return list.TrueForAll(match);
	}
}