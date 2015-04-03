using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JazzGenerator {
	public int key_pitchclass;
	public int time_signature;
	public int bar_halfway;
	
	public List<int> melody;
	public List<int> accompaniment;
	
	public JazzGenerator(
		List<Scale> scales, 
		List<Scale> chords, 
		List<Progression> progressions)
	{
		Progression progression = progressions[Random.Range(0,progressions.Count)];
		key_pitchclass =  Random.Range(0,12);
		time_signature = Random.Range(3,11);
		
		CalcHalfway();
			
		bool firsthalf=true;
		int lastnote=-1000;
		int accompanimenttype=Random.Range(0,9);
		float motheaten = Random.Range(0.25f,1.0f);
//		Debug.Log("moth% = " + motheaten.ToString());
		melody = new List<int>();
		accompaniment = new List<int>();
		
		int[,] ranges = {{-12,0},{0,12},{-6,6},{6,18},{-18,-6}};
		int leadrange = Random.Range(0,ranges.GetLength(0));
		int accompanimentrange = Random.Range(0,ranges.GetLength(0));
			
		for (int i=0;i<progression.chords.Count;i++)
		{
			int notecount = firsthalf ? bar_halfway-1 : time_signature-(bar_halfway-1);
			for (int j=0;j<notecount;j++)
			{
				if (j==0)
				{
					accompaniment.Add(progression.chords[i].pitchclass);
				}
				else
				{
					switch(accompanimenttype)
					{
					case 0:
						accompaniment.Add(666);
						break;
					case 1:
						accompaniment.Add(666);
						break;
					case 2:
						accompaniment.Add(666);
						break;
					case 3:						
						accompaniment.Add(progression.chords[i].GetNote(ranges[accompanimentrange,0],ranges[accompanimentrange,1],j%3,chords));
						break;
					case 4:
						accompaniment.Add(progression.chords[i].GetNote(ranges[accompanimentrange,0],ranges[accompanimentrange,1],(2-j)%3,chords));
						break;
					case 5:						
						accompaniment.Add(progression.chords[i].GetNote(ranges[accompanimentrange,0],ranges[accompanimentrange,1],j%4,chords));
						break;
					case 6:
						accompaniment.Add(progression.chords[i].GetNote(ranges[accompanimentrange,0],ranges[accompanimentrange,1],(3-j)%4,chords));
						break;
					case 7:
						if (j==1)
						{
							accompaniment.Add(progression.chords[i].GetNote(ranges[accompanimentrange,0],ranges[accompanimentrange,1],j%3,chords));							
						}
						else
						{
							accompaniment.Add(666);							
						}
						break;
					case 8:
						if (j%2==0)
						{
							accompaniment.Add(progression.chords[i].GetNote(ranges[accompanimentrange,0],ranges[accompanimentrange,1],j%3,chords));														
						}
						else
						{
							accompaniment.Add(666);		
						}
						break;					
					}
				}
				
				int note = progression.chords[i].RandomNote(ranges[leadrange,0],ranges[leadrange,1],chords);
				if (note==lastnote)
				{
					note = progression.chords[i].RandomNote(ranges[leadrange,0],ranges[leadrange,1],chords);
				}
				
				if (j==0 || Random.value<motheaten)
				{
					melody.Add(note);
				}
				else
				{
					melody.Add(666);
				}
				
				lastnote=note;
			}
			
			firsthalf=!firsthalf;
		}
		
		// do second pass to add passing notes
		
		List<BasedScale> chordscales = new List<BasedScale>();
		for (int i=0;i<progression.chords.Count;i++)
		{
			BasedScale scale = progression.chords[i].FindScale(scales,chords,key_pitchclass);
			chordscales.Add(scale);
		}
		
		float passingchance = Random.Range(0.6f,0.1f);
		float auxilliarychance = Random.Range(0.6f,0.1f);
		float auxilliaryupperchance = Random.value;
		float extrachordchance = Random.Range(0.0f,0.25f);
		
		for (int i=0;i<(melody.Count-1);i+=2)
		{
			int notea = melody[i];
			int noteb = melody[i+1];
			
			if (notea==666||noteb==666)
			{
				melody.Insert(i+1,666);
				continue;
			}
			
			int bar = (i/2)/time_signature;
			int beat = (i/2)-bar*time_signature;
			int chord_index = beat<bar_halfway ? 2*bar : 2*bar+1;
			
			//Debug.Log(Random.value.ToString() + "chord index vs chord length " + bar.ToString()+"-"+beat.ToString()+" \t "+chord_index.ToString() + "," +  progression.chords.Count.ToString() + 
			//	" \t " + "barpos :" + beat.ToString() + "," + bar_halfway.ToString());
//			BasedChord chord = progression.chords[chord_index];
			BasedScale scale = chordscales[chord_index];
			
			int scaledistance = scale.ScaleDistance(scales,notea,noteb);
			if (Random.value<extrachordchance)
			{
				int lower = System.Math.Min(notea,noteb);
				int upper = System.Math.Max(notea,noteb);
				int note = scale.RandomNoteBetween(scales,lower-5,upper+5);
				if (note!=notea &&  note!=noteb)
				{
					melody.Insert(i+1,note);
				}
				else
				{
					melody.Insert(i+1,666);
				}
			}
			else if ( scaledistance ==2 && Random.value<=passingchance)
			{
				int notebetween = scale.RandomNoteBetween(scales,notea,noteb);
				melody.Insert(i+1,notebetween);
			}
			else if (scaledistance == 0 && Random.value<=auxilliarychance)
			{
				int auxilliary = scale.AuxilliaryNote(scales,notea,Random.value<auxilliaryupperchance);				
				melody.Insert(i+1,auxilliary);
			}
			else
			{
				melody.Insert(i+1,666);
			}
		}
		melody.Add(666);
		
		//Debug.Log("melody length : " + melody.Count.ToString() + " - " + accompaniment.Count.ToString());

	}
	
	void CalcHalfway()
	{
		int halfway;
		if (time_signature%2==0)
		{
			halfway = time_signature/2;
			if (Random.value<0.5)
			{
				if (Random.value<0.5)
				{
					halfway++;
				}
				else
				{
					halfway--;
				}
			}
		}
		else
		{
			if (Random.value<0.5)
			{
				halfway=time_signature/2;
			}
			else
			{
				halfway=time_signature/2+1;
			}
		}
		bar_halfway=halfway;
	}
		
}
