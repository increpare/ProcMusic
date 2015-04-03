using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

public class Motive 
{
	public List<int> data;
	
	public Motive(Motive othermotive)
	{
		data = new List<int>(othermotive.data);
	}
	
	//INCLUSIVE OF UPPER BOUND
	public Motive(int lowerbound, int upperbound, int length)
	{
		tbase=0;
		data = new List<int>();
		for (int i=0;i<length;i++)
		{
			data.Add(Random.Range(lowerbound,upperbound+1));
		}
	}
	
	public void Reverse()
	{
		data.Reverse();
	}
	
	public Motive Reversed()
	{
		Motive newmotive = new Motive(this);
		newmotive.Reverse();
		return newmotive;		
	}
	
	public void Dilate(int about, int amount)
	{
		for (int i=0;i<data.Count;i++)
		{
			data[i]=about + (data[i]-about)*amount;
		}
	}
	
	public Motive Dilated(int about, int amount)
	{
		Motive newmotive = new Motive(this);
		newmotive.Dilate(about,amount);
		return newmotive;
	}
	
	
	int tbase;
	
	public void Transpose(int t)
	{
		for (int i=0;i<data.Count;i++)
		{
			data[i]+=t;
		}
	}
	
	public void Transpose(int t, int min, int max)
	{
		tbase+=t;
		if (tbase<=max && t>=min)
		{
			for (int i=0;i<data.Count;i++)
			{
				data[i]+=t;
			}
		}
		else
		{
			tbase-=t;
		}
			
	}
	
	
	public Motive Transposed(int t, int min, int max)
	{
		Motive newmotive = new Motive(this);
		newmotive.Transpose(t,min,max);
		return newmotive;
	}
	
	public Motive Transposed(int t)
	{
		Motive newmotive = new Motive(this);
		newmotive.Transpose(t);
		return newmotive;
	}
	
	
	public void Invert()
	{
		int max = Mathf.Max(data.ToArray());
		int min = Mathf.Min(data.ToArray());
		
		for (int i=0;i<data.Count;i++)
		{
			data[i]=max-(data[i]-min);
		}
	}
	
	public Motive Inverted()
	{
		Motive newmotive = new Motive(this);
		newmotive.Invert();
		return newmotive;
	}
}

public class FMotive 
{
	public List<float> data;
	
	public FMotive(FMotive othermotive)
	{
		data = new List<float>(othermotive.data);
	}
	
	//INCLUSIVE OF UPPER BOUND
	public FMotive(float lowerbound, float upperbound, int length)
	{
		mbase=1;
		data = new List<float>();
		for (int i=0;i<length;i++)
		{
			data.Add(Random.Range(lowerbound,upperbound+1));
		}
	}
	
	float mbase;
	
	public void Multiply(float t, float min, float max)
	{
		mbase*=t;
		if (mbase<=max && t>=min)
		{
			for (int i=0;i<data.Count;i++)
			{
				data[i]*=t;
			}
		}
		else
		{
			mbase/=t;
		}
			
	}
	
	
	public FMotive Multiplied(float t, float min, float max)
	{
		FMotive newmotive = new FMotive(this);
		newmotive.Multiply(t,min,max);
		return newmotive;
	}
	
	public void Reverse()
	{
		data.Reverse();
	}
	
	public FMotive Reversed()
	{
		FMotive newmotive = new FMotive(this);
		newmotive.Reverse();
		return newmotive;		
	}
	
	public FMotive Dilated(float about, float amount)
	{
		FMotive newmotive = new FMotive(this);
		for (int i=0;i<data.Count;i++)
		{
			newmotive.data[i]=about + (newmotive.data[i]-about)*amount;
		}
		return newmotive;
	}
	
	public FMotive Transposed(float t)
	{
		FMotive newmotive = new FMotive(this);
		for (int i=0;i<data.Count;i++)
		{
			newmotive.data[i]+=t;
		}
		return newmotive;
	}
	
	public void Invert()
	{
		float max = Mathf.Max(data.ToArray());
		float min = Mathf.Min(data.ToArray());
		
		for (int i=0;i<data.Count;i++)
		{
			data[i]=max-(data[i]-min);
		}
	}
	
	public FMotive Inverted()
	{
		FMotive newmotive = new FMotive(this);
		newmotive.Invert();
		return newmotive;
	}
}

