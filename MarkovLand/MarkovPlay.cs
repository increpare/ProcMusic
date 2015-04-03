using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarkovPlay : MelodyGenerator {
	public int channels = 4;
	
	public int samplecount = 10;
	public int durationcount = 3;
	public int frequencycount = 3;
	 	
	public float minduration = 0.01f;
	public float maxduration = 4.0f;
	
	public float minfrequency = 0.25f;
	public float maxfrequency = 2.0f;
	
	public int leadin_samples = 20;
	public int leadin_frequency = 20;
	public int leadin_durations = 20;
	
	public int chainmemory = 5;
	
	public int levelmultiplier = 2;
	
	public bool proceedByDifferences = false;
		// for diatonic, means will proceed by differences
		// for non-diatonic, treats the differences between the selected frequences as differences
	private List<AudioClip> cliplist;

	private Chain<int> soundchain;
	private Chain<int> frequencychain;
	private Chain<int> durationchain;
		
	private List<int> voices;
	private List<float> frequencies;
	private List<float> durations;
	
	private List<int> soundhistory;
	private List<int> frequencyhistory;
	private List<int> durationhistory;
	
	public bool modulate;
	public float phraselength;//between 10 - 30 seconds
	private List<float> keychangehistory;
	
	public bool hasleadinstrument;
	public int leadinstrument;
	public float leadreplaceprob;
	public int leadspread;
	public int leadpasses;
	
	private int soundlocation;
	private int durationlocation;
	private int frequencylocation;
	
	public bool bars;
	public float barlength; // length of beat - 1-4 seconds, and no lower than lowest beat
	public float barprob;//prob of forcing a beat - should be quite high 80%->100%
	public float beatcount;//between 2 and6
	public float subbarprob;//prob of forcing a beat - should be quite high 80%->100%
	private float barlocation;
		
	public bool melodic;
	public bool diatonic;
	
	public int profile;
	public int scale;
	public int transposition;
	public float detune;
	
	public int[,] scales = {
		{1,1,1,1,1,1,1,1,1,1,1,1},
		{1,0,1,0,1,1,0,1,0,1,0,1},//major
		{0,1,0,1,0,0,1,0,1,0,1,0},//pentatonic
		{1,0,1,1,0,1,0,1,1,0,0,1},//harmonic minor
		{1,0,1,1,0,1,0,1,1,0,1,0},//natural minor
		{1,0,1,1,0,1,0,1,0,1,0,1},//melodic minor
		{1,0,1,1,0,1,1,1,0,0,1,0},//jazzy
		{1,0,1,0,1,0,1,0,1,0,1,0},//wholetone
		{1,0,0,0,1,0,0,1,0,0,0,0},//major chord
		{1,0,0,1,0,0,0,1,0,0,0,0},//minor arpeggio
		{1,0,0,0,1,0,0,1,0,0,1,0},//major seventh
		{1,0,1,0,1,0,0,1,0,0,0,0},//major w/ second
		{1,0,0,0,1,0,0,0,1,0,0,0},//augmented chord
		{1,0,0,1,0,0,1,0,0,1,0,0},//diminished chord
	};
	
	private GenericMelody melody;
	// Use this for initialization
	public MarkovPlay () {
		barlocation=0;		
		//profile=7;
		profile=0;
		
	}
	
	public override GenericMelody GenerateMelody()
	{
//		Debug.Log("RANDOM SEED : " + Random.seed.ToString());
		
		minduration=0.01f;
		maxduration=Random.Range(0.1f,2.0f);
		melodic = Random.Range(0,4)>0;
		diatonic = Random.Range(0,6)>0;//weight diatonic
		
	 	chainmemory = Random.Range(4,11);
		proceedByDifferences = Random.Range(0,2)==0;
		
		bars = Random.Range(3,1)>0;
		barprob = Random.Range(0.75f,1.0f);
		subbarprob = Random.Range(0.0f,barprob);
		beatcount = Random.Range(2,7);
		barlength = Random.Range(1.0f,5.0f);
/*		Debug.Log("bar length: " + barlength.ToString());
		Debug.Log("beat count: " + beatcount.ToString());
		Debug.Log("bar prob: " + barprob.ToString());
		Debug.Log("beat prob: " + subbarprob.ToString());
				 */
	/*	if (proceedByDifferences)
		{
			Debug.Log("differences");
		}
		else
		{
			Debug.Log("nondifferences");
		}*/
		
		if (melodic)
		{
//			Debug.Log("melodic");
			frequencycount=Random.Range(5,10);
			durationcount=Random.Range(2,3);
			samplecount=Random.Range(2,4);
		}
		else
		{
//			Debug.Log("not melodic");
			frequencycount=Random.Range(1,3);
			durationcount=Random.Range(2,5);
			samplecount=Random.Range(5,10);
		}
		
		detune = Random.Range(Mathf.Pow(2.0f,-5.0f/12.0f),Mathf.Pow(2.0f,5.0f/12.0f));
//		Debug.Log("detune by " + detune.ToString());
		
		if (diatonic)
		{
//			Debug.Log("diatonic");
			
			scale = Random.Range(0,scales.GetLength(0));
			transposition = Random.Range(0,12);
//			Debug.Log("Scale = "+scale.ToString());
//			Debug.Log("Transposition = "+transposition.ToString());
		}
		else
		{
//			Debug.Log("not diatonic");
		}		
		
		voices = new List<int>();
		for(int i=0;i<samplecount;i++)
		{
			voices.Add(Random.Range(0,5));
		}
		
		bool rhythmic = Random.Range(0,3)>=0;//weight for beats
		if (rhythmic)
		{
//			if (maxduration<0.5)
//				maxduration*=2;
//			Debug.Log("Beaty");
		}
		else
		{			
//			Debug.Log("Not Beaty");
		}
		durations = new List<float>();
		for (int i=0;i<durationcount;i++)
		{
			if (rhythmic)
			{
				float r=1.0f/Random.Range(1,5);
				if (profile!=6)
				{
					r*=maxduration*Random.Range(1,5);
				}
				
				durations.Add(r);
			}
			else
			{
				durations.Add(Random.Range(minduration,maxduration));
			}
		}
		durations.Sort();
		
		frequencies = new List<float>();
		
		if (diatonic)
		{
			int range = Random.Range(0,4);
			range=1;

//			Debug.Log("range = " + range.ToString());
			int low=0;
			int high=0;
			switch(range)
			{
			case 0:
				low=-27;
				high=-13;
				break;
			case 1:
				low=-20;
				high=0;
				break;
			case 2:
				low=-13;
				high=13;
				break;
			case 3:
				low=-27;
				high=13;
				break;			
			}
				
			for (int j=low;j<high;j++)
			{
				if (scales[scale,(j+transposition+48)%12]!=1)
				{
					continue;
				}
				
				float note = Mathf.Pow(2.0f,j/12.0f);
				frequencies.Add(note*detune);

			}			
		}
		else
		{
			for (int i=0;i<frequencycount;i++)
			{
				float f=Random.Range(minfrequency,maxfrequency);								
				
				frequencies.Add(f*detune);
			}
		}
		frequencies.Sort();//for the non-diatonic case - already sorted in diatonic - for proceedByDifferences
		
		durationhistory = new List<int>();
		for (int i=0;i<leadin_durations;i++)
		{
			durationhistory.Add(Random.Range(0,durations.Count));
		}
		
		
		frequencyhistory = new List<int>();
		for (int i=0;i<leadin_frequency;i++)
		{
			frequencyhistory.Add(Random.Range(0,frequencies.Count));
		}
		
		soundhistory = new List<int>();
		for (int i=0;i<leadin_samples;i++)
		{
			soundhistory.Add(Random.Range(0,voices.Count));
		}
		
		bool modulate = Random.Range(0,3)>0;
		phraselength = Random.Range(15.0F,35.0F);
		keychangehistory = new List<float>();
		
		keychangehistory.Add(1);
		
		if (modulate)
		{
			//public float phraselength;//between 10 - 30 seconds
			for (int i=0;i<10;i++)
			{
				keychangehistory.Add(Random.Range(0.75F,1.25F));
			}
		}
		
		//pregenerate first ten levels, fuck it
		
		for (int level=1;level<10;level++)
		{			
			//durations
			Chain<int> chain_durations = new Chain<int>(durationhistory,chainmemory);
			List<int> durations_adding = new List<int>();
			
			while (durations_adding.Count<level*levelmultiplier)
			{				
				 durations_adding.AddRange(chain_durations.Generate(level*levelmultiplier-durations_adding.Count));
			}
			durationhistory.AddRange(durations_adding);
					
					
			//frequencies
			if (proceedByDifferences)
			{
				DeltaChain<int> chain_frequencies = new DeltaChain<int>(frequencyhistory, chainmemory, Subtract.IntSubtract);
				List<int> frequencies_adding = new List<int>();
				
				while (frequencies_adding.Count<level*levelmultiplier)
				{				
					 frequencies_adding.AddRange(chain_frequencies.Generate(level*levelmultiplier-frequencies_adding.Count));
				}
				frequencyhistory.AddRange(frequencies_adding);
			}
			else
			{
				Chain<int> chain_frequencies = new Chain<int>(frequencyhistory, chainmemory);
				List<int> frequencies_adding = new List<int>();
				
				while (frequencies_adding.Count<level*levelmultiplier)
				{				
					 frequencies_adding.AddRange(chain_frequencies.Generate(level*levelmultiplier-frequencies_adding.Count));
				}
				frequencyhistory.AddRange(frequencies_adding);
			}
					
					
			//samples
			Chain<int> chain_sounds = new Chain<int>(soundhistory,chainmemory);
			List<int> sounds_adding = new List<int>();
			
			while (sounds_adding.Count<level*levelmultiplier)
			{				
				 sounds_adding.AddRange(chain_sounds.Generate(level*levelmultiplier-sounds_adding.Count));
			}
			soundhistory.AddRange(sounds_adding);
		}
		
		hasleadinstrument = Random.Range(0,4)>0;
		leadreplaceprob = Random.Range(0.6f,1.0f);
		leadinstrument = Random.Range(0,voices.Count);	
		leadspread = Random.Range(1,5);
		leadpasses = Random.Range(1,4);
//		Debug.Log("leadspread " + leadspread.ToString());
//		Debug.Log("leadpasses " + leadpasses.ToString());
//		Debug.Log("leadreplaceprob " + leadreplaceprob.ToString());
		if (hasleadinstrument && (frequencies.Count>2))
		{
			for (int t=0;t<leadpasses;t++)
			{
				for (int i=0;i<Mathf.Min(new int[]{soundhistory.Count,frequencyhistory.Count,durationhistory.Count});i++)
				{
					if (soundhistory[i]==leadinstrument && durationhistory[i]>0 && Random.value<leadreplaceprob)
					{
						durationhistory[i]--;
						durationhistory.Insert(i+1,durationhistory[i]);
						
						soundhistory.Insert(i+1,leadinstrument);
						
						frequencyhistory.Insert(i+1,frequencyhistory[i]);
							
						frequencyhistory[i+1] = Mathf.Clamp( frequencyhistory[i+1]+(Random.value<0.5f ? 1 : -1)*Random.Range(0,leadspread),0,frequencies.Count-1);					
							
						i++;
					}
				}
			}
		}
		
//		Debug.Log("history lengths : " + soundhistory.Count.ToString() + " , " + durationhistory.Count.ToString());
		soundlocation=0;
		frequencylocation=0;
		durationlocation=0;
		
		melody = new GenericMelody();
		melody.durations=new List<float>();
		melody.frequencies=new List<float>();
		melody.voices = new List<int>();
		melody.volumes = new List<float>();
		
		ProcessMusic();
		
		return melody;
	}
	
	private void ProcessMusic()
	{		
		float time=0;
		int keyindex=0;
		
		while(soundlocation<soundhistory.Count&&frequencylocation<frequencyhistory.Count&&durationlocation<durationhistory.Count)
		{
			int sound = soundhistory[soundlocation];
			int duration = durationhistory[durationlocation];
			int frequency = frequencyhistory[frequencylocation];
						
			time+=durations[duration];
			if (time>phraselength)
			{
				time=0;				
				keyindex=(keyindex+1)%keychangehistory.Count;
			}
			
			float pitch=frequencies[frequency]*keychangehistory[keyindex];
			int voice = voices[sound];
			
			float beatlength = durations[duration];
			
			if (bars)
			{
				float newbarlocation=barlocation+beatlength;
				for (int i=1;i<beatcount;i++)
				{
					float point = (barlength*i)/beatcount;
					if (barlocation<point && newbarlocation>point)
					{
						if (Random.value<subbarprob)
						{
							beatlength-=(barlocation-point);							
						}
						break;
					}
				}

				barlocation=newbarlocation;
				if (Random.value<barprob && barlocation>barlength)
				{
					beatlength-=(barlocation-barlength);
				}
				
				barlocation=0;
			}
									
			melody.volumes.Add(1.0F);
			melody.frequencies.Add(pitch);
			melody.durations.Add(beatlength);
			melody.voices.Add(voice);
			
			soundlocation = (soundlocation+1);
			durationlocation = (durationlocation+1);
			frequencylocation = (frequencylocation+1);
		}
	}
	
	
	static List<T> RandomSublist<T>(
        List<T> list,
        int count)
    {
		List<T> newlist  = new List<T>(list);
		
		for (int i=0;i<list.Count-count;i++)
		{
			int r = Random.Range(0,newlist.Count);
			newlist.RemoveAt(r);
		}
		return newlist;
    }

}
