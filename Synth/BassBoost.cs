using UnityEngine;
using System.Collections;

public class BassBoost : MonoBehaviour {
	
	public float selectivity;
	public float gain2;
	public float ratio;
	
	private float[] cap = new float[2];

	float filter (float input, int channel)
	{
		float gain1 = 1.0f/(selectivity + 1.0f);
		
		cap[channel] = (input + cap[channel]*selectivity )*gain1;
		float sample = Mathf.Clamp((input + cap[channel]*ratio)*gain2,-1.0f,1.0f);
		
		return sample;
	}
	
	void OnAudioFilterRead (float[] data, int channels)
	{
		for (int i=0;i<data.Length;i = i+channels)
		{
			data[i]=filter(data[i],0);			
			
			if (channels == 2)
				data[i+1]=filter(data[i+1],1);			
		}
		
	}
	
	
}
