using UnityEngine;
using System.Collections;

public class FoldBack : MonoBehaviour {

	public float threshold;
	
	float filter (float input, int channel)
	{
		return System.Math.Abs(System.Math.Abs((input - threshold)% ( threshold*4)) - threshold*2) - threshold;	
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
