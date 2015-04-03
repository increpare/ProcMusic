using UnityEngine;
using System.Collections;

public class PulseQuad : MonoBehaviour {
	
	public float morph = 0.0f;
	public float pulse = 0.5f;
	
	float filter (float phase, int channel)
	{
		float a;
		float b;

		if( pulse < .5f )
			a = morph * pulse / 2.0f;
		else
			a = morph * ( 1.0f - pulse ) / 2.0f;
		
		if( phase < pulse )
		{
			if( phase < a )
			{
				b = phase / a - 1.0f;
				return 1.0f - b * b;
			}
			
			if( phase < pulse - a )
				return 1.0f;
			
			b = ( phase - pulse + a ) / a;
			return 1.0f - b * b;
		}
		
		if( phase < pulse + a )
		{
			b = ( phase - pulse ) / a - 1;
			return b * b - 1;
		}
		
		if( phase <= 1 - a )
			return -1;
		
		b = ( phase - 1 + a ) / a;
		return b * b - 1;
		
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
