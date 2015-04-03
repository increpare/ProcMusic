using UnityEngine;
using System.Collections;

public class Decimate : MonoBehaviour {

	public int bits = 16;//1..32
	public float rate = 0.5f;//0..1
	
	private float[]  cnt = new float[]{0.0f,0.0f};
	private float[] y = new float[]{0.0f,0.0f};
	
	public float filter(float input, int channel)
	{
		
		long m = 1 << (bits-1);
		
		//decimate
		cnt[channel] = cnt[channel]+rate;
		if (cnt[channel]>1)
		{
			cnt[channel]=cnt[channel]-1;
			y[channel]=(float)System.Math.Round(input*m)/m;
		}
		return y[channel];
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
