using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

[RequireComponent(typeof(MusicSynth))]
public class MusicPlayer : MonoBehaviour {
		
	private float[] density = new float[]{1.0f,0.5f,0.5f,0.2f,0.2f,0.0f,0.0f,0.0f,0.1f,0.1f,0.0f,0.0f,0.1f,0.1f,0.2f,0.3f,0.5f};	
    
    //if this is<0, ignore density stuff
	public float densitywindowsize = 5.0f;
	
	private int densitywindow=0;
	
	private static List <MelodyGenerator> generators;
    public float quiettime=0.0f;

	public TextAsset midData;

    public void Start()
	{		
		generators = new List<MelodyGenerator>{
			//new Jazzy(),
			//new MarkovPlay(),
			//new Species_Generator(),
			//new MotivicGenerator(),
			new MidiFileGenerator(midData)
		};
		densitywindow=0;
		
		musicsynth = GetComponent<MusicSynth>();
		
//		generators.Add(new JazzAsync());	
		LoadMelody();
	}
	
	public void LoadMelody()
	{
		StopCoroutine("GenerateMelody");
		StopCoroutine("LoadMelody_corot");
		StartCoroutine("GenerateMelody",seedstr);
		seedstr=0;
	}	
	
	public void NewMelody()
	{
		StopCoroutine("GenerateMelody");
		StopCoroutine("LoadMelody_corot");
		StartCoroutine("GenerateMelody",0);
	}
	
	private static Dictionary<int,GenericMelody> musicCache = new Dictionary<int,GenericMelody>();
	
	public static void CacheMelody(int seed) {
		
		Random.seed = seed;
		
		MelodyGenerator generator = Utils.Utility.RandomElement(generators);
		
		musicCache[seed]=generator.GenerateMelody();
	}
	
	private IEnumerator GenerateMelody(int seed)
	{		
		
		if (seed==0)
			seed = System.DateTime.Now.Ticks.GetHashCode();
		
		curplaying=seed;
		
		Random.seed = seed;
			
		MelodyGenerator generator = Utils.Utility.RandomElement(generators);
			
		Debug.Log (generator + "," + seed);
		
		GenericMelody melody = musicCache.ContainsKey(seed)?musicCache[seed]:generator.GenerateMelody();	
		
		if (musicCache.ContainsKey(seed)==false) {
			musicCache[seed]=melody;
		}
		
//		melody.StripToLength(5);
		StopCoroutine("PlayMelody");
		StartCoroutine("PlayMelody",melody);
		yield break;
	}
	
	
	public int curplaying;
	
	public MusicSynth musicsynth;
	
	private	int pitchindex=0;
	private	int durationindex=0;
	private	int velocityindex=0;
	private	int voiceindex=0;		
	private	int channelindex=0;		
	
	private GenericMelody curmelody;
	
	public void ShuntMusicAlong()
	{
		StopCoroutine("PlayMelody");
	
		if (curmelody==null)
			return;
		
		pitchindex = (pitchindex+1);
		durationindex = (durationindex+1);
		velocityindex = (velocityindex+1);
		voiceindex = (voiceindex+1);
		
		if (pitchindex>=curmelody.frequencies.Count)
		{
			return;
		}
		StartCoroutine("PlayMelody",new GenericMelody());
	}
	
	private void ResetDensityIndex()
	{
		StopCoroutine("DensityAdjust");
		densitywindow=0;
		StartCoroutine("DensityAdjust");
	}
	
	private IEnumerator DensityAdjust()
    {
        if (densitywindowsize < 0)
        {
            yield break;
        }
		while(true)
		{
			yield return new WaitForSeconds(densitywindowsize);
			densitywindow = (densitywindow+1)%density.Length;
		}
	}
	
	private IEnumerator PlayMelody(GenericMelody melody)
	{
		ResetDensityIndex();
		
		if (melody.durations!=null)
		{
			pitchindex=0;
			durationindex=0;
			velocityindex=0;
			voiceindex=0;		
			channelindex=0;	
			curmelody=melody;
		}
		else
		{
			melody=curmelody;
		}
		
		while(true)
		{
            if (quiettime>0){
                yield return new WaitForSeconds(quiettime);
            }
			float duration = melody.durations[durationindex];
			
			if (pitchindex==0 || Random.Range(0.0f,1.0f)<=density[densitywindow])
			{
				musicsynth.freqs[channelindex] = melody.frequencies[pitchindex]*440.0f;
				musicsynth.vols[channelindex] = melody.volumes[velocityindex];
				musicsynth.attacks[channelindex] = musicsynth.defend;
				musicsynth.voice[channelindex] = melody.voices[voiceindex]%2;
				musicsynth.phase[channelindex]=0;
				musicsynth.increments[channelindex]=0;
			}
			
			if (duration>0)
			{
				yield return new WaitForSeconds(Mathf.Max(duration,quiettime));
			}
			
			pitchindex = (pitchindex+1);
			durationindex = (durationindex+1);
			velocityindex = (velocityindex+1);
			voiceindex = (voiceindex+1);
			
			if (pitchindex>=melody.frequencies.Count)
			{
				//yield break;
					
				pitchindex=0;
				durationindex=0;
				velocityindex=0;
				voiceindex=0;		
				channelindex=0;	
				curmelody=melody;
				yield return new WaitForSeconds(3);
					
			}
				
			channelindex = (channelindex + 1)%musicsynth.freqs.Length;
		}
	}
	
	public int seedstr=0;
	
	public void Update()
	{
        if (quiettime > 0)
        {
            quiettime-=Time.deltaTime;
            if (quiettime < 0)
            {
                quiettime = 0;
            }
        }
//#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Space))//&&Application.isEditor)
		{
			NewMelody();
		}
//#endif

		if (seedstr!=0)
		{
			LoadMelody ();
		}
	}
}
