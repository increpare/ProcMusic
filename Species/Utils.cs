using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utils
{
	class Maybe<T>
	{
		public static Maybe<T> Just(T _data)
		{
			Maybe<T> m = new Maybe<T>();
			m.isset=true;
			m._data=_data;
			return m;
		}
		
		public static Maybe<T> Nothing()
		{
			Maybe<T> m = new Maybe<T>();
			m.isset=false;
			m._data=default(T);
			return m;
		}
		
		public bool isset;
		public T _data;
		
		//what happened to getters/setters?
	}
	
	class Utility
	{
		public static T RandomElement<T>(List<T> list)
		{
			return list[Random.Range(0,list.Count)];
		}
		
		public static void Shuffle<E>(IList<E> list)
	    {
	        if (list.Count > 1)
	        {
	            for (int i = list.Count - 1; i >= 0; i--)
	            {
	                E tmp = list[i];
	                int randomIndex = Random.Range(0,i + 1);
	
	                //Swap elements
	                list[i] = list[randomIndex];
	                list[randomIndex] = tmp;
	            }
	        }
	    }
		
		public static List<E> Shuffled<E>(List<E> inputList)
		{
		     List<E> randomList = new List<E>();
		
		     int randomIndex = 0;
		     while (inputList.Count > 0)
		     {
		          randomIndex = Random.Range(0, inputList.Count); //Choose a random object in the list
		          randomList.Add(inputList[randomIndex]); //add it to the new, random list
		          inputList.RemoveAt(randomIndex); //remove to avoid duplicates
		     }
		
		     return randomList; //return the new random list
		}
		
		/*
		public static float NoteToFreq(int n)
		{
			return Mathf.Pow(2.0f, (float)n / 12.0f);
		}*/
		
		
	/*
	float[] scale = {
	Mathf.Pow (2,0),
	Mathf.Pow (2,1.0f/12.0f),
	Mathf.Pow (2,2.0f/12.0f),
	Mathf.Pow (2,3.0f/12.0f),
	Mathf.Pow (2,4.0f/12.0f),
	Mathf.Pow (2,5.0f/12.0f),
	Mathf.Pow (2,6.0f/12.0f),
	Mathf.Pow (2,7.0f/12.0f),
	Mathf.Pow (2,8.0f/12.0f),
	Mathf.Pow (2,9.0f/12.0f),
	Mathf.Pow (2,10.0f/12.0f),
	Mathf.Pow (2,11.0f/12.0f),
	};*/
		
		/*
	 //just
	private static float[] scale = {
		1,
		16.0f/15.0f,
		9.0f/8.0f,
		6.0f/5.0f,
		5.0f/4.0f,
		4.0f/3.0f,
		7.0f/5.0f,
		3.0f/2.0f,
		8.0f/5.0f,
		5.0f/3.0f,
		16.0f/9.0f,
		15.0f/8.0f		
	};*/
	
	
	/*
	float[] scale = {
		1.0f, 
		1.059463f, 
		494.0f/453.0f, 
		1.189207f, 
		635.0f/504.0f, 
		1.33484f, 
		1.414214f, 
		1.498307f, 
		18.0f/11.0f, 
		1.681793f, 
		1.781797f, 
		1.887749f	
	};*/
	
	/*
	float[] scale = {
		1.0f, 
		256/243.0f, 
		809/724.0f, 
		32/27.0f, 
		1.252827f, 
		4/3.0f,
		1024/729.0f, 
		1326/887.0f, 
		128/81.0f, 
		1034/619.0f, 
		16/9.0f, 
		1.879241f
	};*/
		
		//werkmeister III
	private static float[] scale = {
		1.0f,
		98.0f/93.0f,
		28.0f/25.0f,
		196.0f/165.0f,
			49.0f/39.0f,
			4.0f/3.0f,
			196.0f/139.0f,
			196.0f/131.0f,
			49.0f/31.0f,
			196.0f/117.0f,
			98.0f/55.0f,
			49.0f/26.0f
	};
		
	public static float NoteToFreq(int n)
	{
		int note = n%12;
		int octave = n/12;
		
		while (note<0)
		{
			note+=12;
			octave--;
		}
		
		return Mathf.Pow (2,octave)*scale[note];
//		return Mathf.Pow(2.0f, (float)n / 12.0f);
	}
	}
	
}