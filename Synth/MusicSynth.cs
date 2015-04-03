using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MusicSynth : MonoBehaviour
{		
	public float[] freqs;
	public float[] vols;
	public float[] attacks;
	public int[] voice;
	public float[] increments;
	public float[] phase;
	
	public float noisechannel_vol = 0;
	public float noisechannel_decay = 1.0f;
		
	private float sampling_frequency = 48000.0f;
	public float gain = 0.05f;	
	public float decay = 1.0f;
	public float defend = 0.0f;
	public float attack = 1.0f;
	
	
	private int noiseint = 0;
	private float[] noisevals;
	
	public float lpf_min = 1000;
	public float lpf_max = 22000;
	private AudioLowPassFilter lpf;
	
	
	void Awake()
	{
		sampling_frequency=AudioSettings.outputSampleRate;
		lpf = GetComponent<AudioLowPassFilter>();
		noisevals = new float[48000];
		for (int i=0;i<48000;i++)
		{
			noisevals[i]=Random.Range(-1.0f,1.0f);
		}
	}

	void OnAudioFilterRead (float[] data, int channels)
	{

		int voicecount = freqs.Length;
		

		for (int i=0;i<voicecount;i++)
		{
			float frequency = freqs[i];			
			increments[i] = frequency*2 / sampling_frequency;		
		}
		for (int i=0;i<data.Length;i = i+channels)
		{
			float val =0;
			for(int j=0;j<voicecount;j++)
			{
				phase[j] = phase[j] + increments[j];
				vols[j]=Mathf.Max(vols[j]-decay/sampling_frequency,0);
				attacks[j]=Mathf.Max (attacks[j]-attack/sampling_frequency,0.0f);
				
				float volcomponent=Mathf.Min (vols[j],1-attacks[j]);
				
				//switch(voice[j]%2)
				switch(voice[j])	
				{
				case 0:
					if (phase[j]<0)
					{
						val += (float)(gain * (2*phase[j]+1) * volcomponent )/voicecount;
					}
					else
					{
						val += (float)(gain * (1-2*phase[j]) * volcomponent )/voicecount;
					}
					break;
				case 1:
					val += gain*(float)System.Math.Sin(phase[j]*Mathf.PI)*volcomponent/voicecount;
					break;/*
				case 2:
					val += gain*phase[j]*volcomponent/voicecount;
					break;
				case 3:
					if (phase[j]>0.5)
						val+=gain*volcomponent/voicecount;
					break;
				case 4:
					if (phase[j]>0.75)
						val+=gain*volcomponent/voicecount;
					break;
				default:
					Debug.Log ("EEK");
					break;	*/				
				}
					
				if (phase[j] > 1)
					phase[j]=-1;
			}			
			
			if (noisechannel_vol>0)
			{
				noiseint = (noiseint+1)%48000;
				noisechannel_vol = Mathf.Max(noisechannel_vol-noisechannel_decay/sampling_frequency,0);
				val += gain*noisechannel_vol*noisevals[noiseint];
			}
			
			//adding to overlay on top of existing sounds if there are any			
			data[i]+=val;			
			
			if (channels == 2){
				data [i+1]+=val;
			}
		}

	}

	private int lpf_direction=1;
	public float lpf_period=30;
	void Update()
	{
		
		float lpf_amount = (lpf_max-lpf_min)/lpf_period;
		
		if (lpf_direction>0)
		{
			lpf.cutoffFrequency=Mathf.Clamp(lpf.cutoffFrequency+lpf_amount*Time.deltaTime,lpf_min,lpf_max);
		}
		else
		{
			lpf.cutoffFrequency=Mathf.Clamp(lpf.cutoffFrequency-lpf_amount*Time.deltaTime,lpf_min,lpf_max);
		}
		
		if (lpf.cutoffFrequency>=lpf_max)
		{
			lpf_direction=-1;
		}
		else if (lpf.cutoffFrequency<=lpf_min)
		{
			lpf_direction=1;
		}
		
	}

}
