using UnityEngine;
using System.Collections;

public class SmoothSaturation : MonoBehaviour {
	
	public float a;//between 0 and 1
	
	float filter (float x, int channel)
	{
		if (x<a)
			return x;
		else if (x>a)
			return a+(x-a)/(1+Mathf.Pow((x-a)/(1-a),2.0f));
		else
			return  (a+1)/2;
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
