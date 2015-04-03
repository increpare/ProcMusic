using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MelodyLineAsync : GenericMelody
{
		
	public MelodyLineAsync(List<int> notes,List<int> accompanying, float tempo_bpm) 
	{
		tempo_bpm*=5;
		voices = new List<int>();
		frequencies = new List<float>();
		durations = new List<float>();
		volumes = new List<float>();
		float accompanyvol = Random.Range(0.4f,1.0f);
		
		float toremove = Random.Range(0.5f,0.95f);
		List<int> accompanying_depleted = new List<int>(accompanying);
		
		for(int i=accompanying_depleted.Count-1;i>=0;i--)
		{
			if (Random.Range(0.0f,1.0f)>toremove)
			{
				accompanying_depleted.RemoveAt(i);
			}
		}
		
		int diff = -12+Random.Range(0,3)*12;
		
		float speed1 = Random.Range(0.9f,1.1f);
		float speed2 = speed1 + Random.Range(0.01f,0.05f);
		
		int skip1 = Random.Range (0,4)*2+1;
		int skip2 = Random.Range (0,4)*2+1;
		
		int i1=-1;
		int i2=-1;
		float f1=0;
		float f2=0;
		while (i1<accompanying.Count*4)
		{
			float timetilnextnote1 = (1-f1)/speed1;
			float timetilnextnote2 = (1-f2)/speed2;
			
			float deltatime;
			if (timetilnextnote1<=timetilnextnote2)
			{
				deltatime = (1-f1)/speed1+0.000001f;
			}
			else
			{
				deltatime = (1-f2)/speed2+0.000001f;
			}
			
			f1+=deltatime*speed1;
			f2+=deltatime*speed2;
			
			if (f1>=1.0f)
			{
				f1-=1.0f;
				i1++;
				
				if (accompanying[(i1*skip1)%accompanying.Count]!=666)
				{
					frequencies.Add(Utils.Utility.NoteToFreq(accompanying[(i1*skip1)%accompanying.Count]));
					volumes.Add(accompanyvol);
					voices.Add(0);
					if (f2>=1.0f && accompanying[((i2*skip2)+1)%accompanying.Count]!=666)
					{
						durations.Add(0);
					}
					else
					{
						durations.Add(deltatime*60.0f/tempo_bpm);
					}
				}
			}
			
			if (f2>=1.0f)
			{
				f2-=1.0f;
				i2++;
				
				if (accompanying[(i2*skip2)%accompanying.Count]!=666)
				{				
					frequencies.Add(Utils.Utility.NoteToFreq(accompanying[(i2*skip2)%accompanying.Count]+diff));
					volumes.Add(accompanyvol);
					voices.Add(1);
					durations.Add(deltatime*60.0f/tempo_bpm);
				}
			}
		}
		
	}
	
}
