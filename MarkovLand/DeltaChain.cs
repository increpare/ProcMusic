using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>  
/// Chain implements a markov chain for a type T  
/// allows the generation of sequences based on  
/// a sample set of T items  
/// </summary>  
/// <typeparam name="T">the type of elements</typeparam>  
public class DeltaChain<T>
{
	public Chain<KeyValuePair<T,T>> chain;

	/// <summary>  
	/// creates a new chain  
	/// </summary>  
	/// <param name="input">Sample set</param>  
	/// <param name="length">window size for sequences</param>  
	/// 
	public DeltaChain (IEnumerable<T> input, int length, SubtractDelegate _subtraction)
	{		
		subtraction=_subtraction;
		List<KeyValuePair<T,T>> diffs = GenerateDiffs(input);
		
		chain = new Chain<System.Collections.Generic.KeyValuePair<T, T>>(diffs,length/2);
	}
	
	public delegate T SubtractDelegate(T a, T b);
	public SubtractDelegate subtraction;

	List<KeyValuePair<T,T>> GenerateDiffs(IEnumerable<T> list) 
	{
		List<KeyValuePair<T,T>> diffs = new List<KeyValuePair<T, T>>();
		
		T last=default (T);
		bool first=true;
		foreach(T item in list)
		{
			if (!first)
			{				
				T diff = subtraction(item,last);
				diffs.Add(new KeyValuePair<T,T>(last,diff));
			}
			first=false;
			last=item;
		}
		return diffs;
	}
	/// <summary>  
	/// generate a new sequence based on the samples first entry  
	/// </summary>  
	/// <param name="max">maximum size of result</param>  
	/// <returns></returns>  
	public IEnumerable<T> Generate (int max)
	{
		IEnumerable<KeyValuePair<T,T>> deltalist = chain.Generate(max);
		
		List<T> results = new List<T>();
		foreach (KeyValuePair<T,T> x in deltalist)
		{
			results.Add(x.Key);
		}
		return results;
	}
}
