using UnityEngine;
using System.Collections;

public class Karlsen : MonoBehaviour {
	
public double CutoffFrequency = 0.3f;//between 0 and 1
public double Resonance = 0.1f;
	
	
			
	private double[][] buff = new double[][]{
		new double[4],
		new double[4]
	};

	float filter (float inpt, int channel)
	{
		double input = (double)inpt;
		double b_rscl = System.Math.Max(System.Math.Min(buff[channel][3],1.0),-1.0);
		input = (-b_rscl * Resonance) + input;
		buff[channel][0] = ((-buff[channel][0] + input) * CutoffFrequency) + buff[channel][0];
		buff[channel][1] = ((-buff[channel][1] + buff[channel][0]) * CutoffFrequency) + buff[channel][1];
		buff[channel][2] = ((-buff[channel][2] + buff[channel][1]) * CutoffFrequency) + buff[channel][2];
		buff[channel][3] = ((-buff[channel][3] + buff[channel][2]) * CutoffFrequency) + buff[channel][3];
		return (float)buff[channel][3];
	}
	
	void OnAudioFilterRead (float[] data, int channels)
	{
		CutoffFrequency = System.Math.Max(System.Math.Min(CutoffFrequency,1.0),0);
		Resonance = System.Math.Max(System.Math.Min(Resonance,5.0),-1.0);
		for (int i=0;i<data.Length;i = i+channels)
		{
			data[i]=filter(data[i],0);			
			
			if (channels == 2)
				data[i+1]=filter(data[i+1],1);			
		}
		
	}
	
	
}
