using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

public class BasedScale {
	
	public int pitchclass;
	public int scaleindex;
	
	public BasedScale(int _pitchclass, int _scaleindex)
	{
		pitchclass=_pitchclass;
		scaleindex=_scaleindex;
	}
	
	public bool HasNote(List<Scale> scales,int n)
	{
		n-=pitchclass;
		return scales[scaleindex].notes[((n%12)+12)%12];
	}
	
	
	public string Print(List<Scale> scales)
	{
		string s = "scale  - ";
		for (int i=0;i<12;i++)//in real space
		{
			s += HasNote(scales,i) ? "1" : "0";
		}
		
		return s;
	}
	
	public bool HasChord(
		List<Scale> scales, 
		List<Scale> chords,
		BasedChord chord)
	{
		//chordnote+transform is in this scale's space
		bool contained=true;
		
		//Debug.Log( this.Print(scales) + "\n" + chord.Print(chords) );
		
		
		for (int i=0;i<chords[chord.chordindex].notes.Count;i++)
		{
			//for each note in the chord
			if (chords[chord.chordindex].notes[i])
			{
				if (!HasNote(scales,i+chord.pitchclass))
				{
					//Debug.Log("disagree at note " + i.ToString());
					contained=false;
					break;
				}
			}
		}
		
		return contained;
	}
	
	
	public int ScaleDistance(List<Scale> scales, int a, int b)
	{
		//convert to scale space
		a-=pitchclass;
		b-=pitchclass;
		if (a>b)
		{
			int c;
			c=a;
			a=b;
			b=c;
		}
		
		int dist=0;
		//count number of 1s between a and b (inclusive of b)
		for (int i=a+1;i<=b;i++)
		{
			if (scales[scaleindex].notes[(((i)%12)+12)%12])
				dist++;
		}
		return dist;
	}
	
	
	
	public int AuxilliaryNote(List<Scale> scales, int a, bool upper)
	{
		a-=pitchclass;
		while(true)
		{
			if (upper)
			{
				a++;
			}
			else
			{
				a--;
			}
			
			if (HasNote(scales,a))
				return a+pitchclass;
		}
	}
	
	public int RandomNoteBetween(List<Scale> scales, int a, int b)
	{
		//convert to scale space
		a-=pitchclass;
		b-=pitchclass;
		if (a>b)
		{
			int c;
			c=a;
			a=b;
			b=c;
		}
		
		List<int> notesbetween = new List<int>();
		//count number of 1s between a and b (inclusive of b)
		for (int i=a+1;i<b;i++)
		{
			if (scales[scaleindex].notes[(((i)%12)+12)%12])
				notesbetween.Add(i+pitchclass);
		}
		return Utils.Utility.RandomElement(notesbetween);
	}
}
