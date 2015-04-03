using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>  
/// Chain implements a markov chain for a type T  
/// allows the generation of sequences based on  
/// a sample set of T items  
/// </summary>  
/// <typeparam name="T">the type of elements</typeparam>  
public class Chain<T>
{
	Link<T> root = new Link<T> (default(T));
	int length;

	/// <summary>  
	/// creates a new chain  
	/// </summary>  
	/// <param name="input">Sample set</param>  
	/// <param name="length">window size for sequences</param>  
	public Chain (IEnumerable<T> input, int length)
	{
		this.length = length;
		root.Process (input, length);
	}

	/// <summary>  
	/// generate a new sequence based on the samples first entry  
	/// </summary>  
	/// <param name="max">maximum size of result</param>  
	/// <returns></returns>  
	public IEnumerable<T> Generate (int max)
	{
		foreach (Link<T> next in root.Generate (root.SelectRandomLink ().Data, length, max))
			yield return next.Data;
	}

	/// <summary>  
	/// generate a new sequence based on the sample  
	/// </summary>  
	/// <param name="start">the item to start with</param>  
	/// <param name="max">maximum size of result</param>  
	/// <returns></returns>  
	public IEnumerable<T> Generate (T start, int max)
	{
		foreach (Link<T> next in root.Generate (start, length, max))
			yield return next.Data;
	}
}
