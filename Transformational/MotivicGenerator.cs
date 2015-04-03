using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

public class MotivicGenerator : MelodyGenerator {
	
	public int voices=4;
	public int length=200;
		
	private List<List<ConcreteMotive>> motives;
	
	private List<List<NoteInfo>> notes;
	private List<FlatNoteInfo> flatnotes;
	
	public MotivicGenerator()
	{
		
			
	}
	
	private int separatinginterval;
	private int registeroffset;
	
	private int[,] scales = {
//		{1,1,1,1,1,1,1,1,1,1,1,1},
		{1,0,1,0,1,1,0,1,0,1,0,1},//major
		{0,1,0,1,0,0,1,0,1,0,1,0},//pentatonic
//		{1,0,1,1,0,1,0,1,1,0,0,1},//harmonic minor
//		{1,0,1,1,0,1,0,1,1,0,1,0},//natural minor
		{1,0,1,1,0,1,0,1,0,1,0,1},//melodic minor
//		{1,0,1,1,0,1,0,1,1,1,1,1},//anything minor
		{1,0,1,1,0,1,1,1,0,0,1,0},//jazzy
//		{1,0,1,0,1,0,1,0,1,0,1,0}//wholetone
	};
	
	private int NearScaleNote(int n, int scale, int scale_key)
	{
		int direction = Random.value>0.5 ? 1 : -1;
		while(true)
		{
			if (scales[scale,((((n+scale_key+12)%12)+12)%12)]==1)
			{
				return n;
			}
			n+=direction;
		}
		
	}	
		
	private void MakeMotives()
	{	

		motives = new List<List<ConcreteMotive>>();
		for (int i=0;i<voices;i++)
		{
			motives.Add(new List<ConcreteMotive>());			
		}
		
		ConcreteMotive mainmotive = new ConcreteMotive();
		int motivelength = Random.Range(4,7);
		mainmotive.pitchmotive = new Motive(-10,10,motivelength);
		mainmotive.durationmotive = new FMotive(1.0f,4.0f,motivelength);
		if (Random.value<0.6)
		{
			for (int i=0;i<mainmotive.durationmotive.data.Count;i++)
			{
				mainmotive.durationmotive.data[i]=Mathf.Floor(mainmotive.durationmotive.data[i]);
			}
		}
		mainmotive.volumemotive = new FMotive(0.15f,1.0f,motivelength);
		List<int> intervals = new List<int>(new int[]{0,7,9,12});
		separatinginterval = Utils.Utility.RandomElement(intervals);
		registeroffset = Random.Range(0,2);	
		List<ConcreteMotive> mainmotives = new List<ConcreteMotive>();
		for (int i=0;i<voices;i++)
		{
			mainmotives.Add(new ConcreteMotive(mainmotive));
		}
		
		for (int t=0;t<length;t++)
		{
			
			for (int v=0;v<voices;v++)
			{
				if (Random.Range(0,12)==0)
				{
					//do something!
					switch (Random.Range(0,3))
					{
					case 0:
						if (Random.value<0.5)
						{
							mainmotives[v].pitchmotive.Transpose(Random.Range(-9,9),-10,10);
						}
						else
						{
							mainmotives[v].pitchmotive.Invert();
						}
						break;
					case 1:
						if (Random.value<0.7)
						{
							mainmotives[v].durationmotive.Reverse();
						}
						else
						{
							mainmotives[v].durationmotive.Multiply(Random.Range(0.9f,0.1f),0.5f,2.0f);
							//mainmotive[v].durationmotive.
						}
						break;
					case 2:
						if (Random.value<0.5)
						{
							mainmotives[v].volumemotive.Reverse();
						}
						else
						{
							mainmotives[v].volumemotive.Invert();
						}
						break;
					}
				}
				ConcreteMotive specmotive = new ConcreteMotive(mainmotives[v]);
				specmotive.pitchmotive.Transpose(-separatinginterval*(v-registeroffset));
				motives[v].Add(specmotive);
			}
		}
	}
	
	private void ProcessMotiveList()
	{
		notes = new List<List<NoteInfo>>();
		for (int i=0;i<voices;i++)
		{
			notes.Add(new List<NoteInfo>());
		}
		
		int scale = Random.Range(0,scales.GetLength(0));
		int scale_key = Random.Range(0,12);
//		Debug.Log(scale);

		for (int v=0;v<motives.Count;v++)
		{
			float curtime=0;
			for (int t=0;t<motives[v].Count;t++)
			{
				ConcreteMotive curmotive = motives[v][t];
				for (int n=0;n<curmotive.durationmotive.data.Count;n++)
				{
					NoteInfo note = new NoteInfo(
						curtime,
						Utils.Utility.NoteToFreq(NearScaleNote(curmotive.pitchmotive.data[n],scale,scale_key)),
						curmotive.volumemotive.data[n]
						);
					notes[v].Add(note);
					curtime+=curmotive.durationmotive.data[n];
				}
			}
		}
	}
	
	private void FlattenMotiveList()
	{		
		flatnotes = new List<FlatNoteInfo>();
		for (int v=0;v<notes.Count;v++)
		{
			for (int t=0;t<notes[v].Count;t++)
			{
				FlatNoteInfo note = new FlatNoteInfo(notes[v][t],v);
				flatnotes.Add(note);
			}
		}
		flatnotes.Sort(new SortFlatNotes());
	}
	
	public override GenericMelody GenerateMelody()
	{
		MakeMotives();
		ProcessMotiveList();
		FlattenMotiveList();
		
		//float tempo_bpm=Random.Range(60.0f,260.0f);
		
		List<int> sounds = new List<int>();
		for (int i=0;i<voices;i++)
		{
			sounds.Add(Random.Range(0,5));
		}
					
		GenericMelody melody = new GenericMelody();
		melody.frequencies= new List<float>();
		melody.voices = new List<int>();
		melody.volumes = new List<float>();
		melody.durations = new List<float>();
		
		float lasttime=0;
		for (int i=0;i<flatnotes.Count;i++)
		{
			float duration = flatnotes[i].onset-lasttime;
			
			melody.frequencies.Add(flatnotes[i].pitch);
			melody.voices.Add(sounds[flatnotes[i].voice]);
			melody.volumes.Add(flatnotes[i].volume);
			melody.durations.Add(duration);
			
			lasttime=flatnotes[i].onset;
		}	
		
		return melody;
	}
}

class FlatNoteInfo {
	public float onset;
	public float pitch;
	public float volume;
	public int voice;
	public FlatNoteInfo(float _onset, float _pitch, float _volume, int _voice)
	{
		onset=_onset;
		pitch=_pitch;
		volume=_volume;
		voice=_voice;
	}
	
	public FlatNoteInfo(NoteInfo noteinfo, int _voice)
	{
		onset=noteinfo.onset;
		pitch=noteinfo.pitch;
		volume=noteinfo.volume;
		voice=_voice;
	}
}
		
class SortFlatNotes : IComparer<FlatNoteInfo>
{
   int IComparer<FlatNoteInfo>.Compare(FlatNoteInfo a, FlatNoteInfo b)
   {
      FlatNoteInfo c1=(FlatNoteInfo)a;
      FlatNoteInfo c2=(FlatNoteInfo)b;
      if (c1.onset > c2.onset)
         return 1;
      if (c1.onset < c2.onset)
         return -1;
      else
         return 0;
   }
}
		
class NoteInfo {
	public float onset;
	public float pitch;
	public float volume;
	public NoteInfo(float _onset, float _pitch, float _volume)
	{
		onset=_onset;
		pitch=_pitch;
		volume=_volume;
	}
}

class ConcreteMotive {
	public Motive pitchmotive;
	public FMotive durationmotive;
	public FMotive volumemotive;
					
	public ConcreteMotive()
	{
	}
					
	public ConcreteMotive(ConcreteMotive other)
	{
		pitchmotive = new Motive(other.pitchmotive);
		durationmotive = new FMotive(other.durationmotive);
		volumemotive = new FMotive(other.volumemotive);
	}
}