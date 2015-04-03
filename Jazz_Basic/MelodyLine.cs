using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MelodyLine : GenericMelody
{
		
	public MelodyLine(List<int> notes,List<int> accompanying, float tempo_bpm) 
	{
		voices = new List<int>();
		frequencies = new List<float>();
		durations = new List<float>();
		volumes = new List<float>();
		float accompanyvol = Random.Range(0.4f,1.0f);
		int dottingbehaviour = Random.Range(0,5);//0,1,2 - normal, 3 - dotted, 4 - triplet
		float half;
		if (dottingbehaviour<=2)
		{
			half=60.0f/2.0f;
		}
		else if (dottingbehaviour==3)
		{
			half=60.0f*3.0f/4.0f;
		}
		else //if (dottingbehaviour==4)
		{
			half=60.0f*2.0f/3.0f;
		}
		
		for (int i=0;i<notes.Count;i+=2)
		{
			
			if (accompanying[i/2]!=666)
			{
				frequencies.Add(Utils.Utility.NoteToFreq(accompanying[i/2]));
				volumes.Add(accompanyvol);
				voices.Add(1);
				durations.Add(0);
			}
			
			if (notes[i+1]==666)
			{
				if (notes[i]==666)
				{
					durations[durations.Count-1]+=60.0f/tempo_bpm;
				}
				else
				{
					voices.Add(0);				
					volumes.Add(1.0F);
					frequencies.Add(Utils.Utility.NoteToFreq(notes[i]));			
					durations.Add(60.0f/tempo_bpm);
				}
			}
			else
			{
				voices.Add(0);				
				frequencies.Add(Utils.Utility.NoteToFreq(notes[i]));			
				volumes.Add(1.0F);		
				durations.Add(half/tempo_bpm);
				voices.Add(0);
				frequencies.Add(Utils.Utility.NoteToFreq(notes[i+1]));
				volumes.Add(1.0F);			
				durations.Add((60.0f-half)/tempo_bpm);
			}
		}		
	}
	
}
