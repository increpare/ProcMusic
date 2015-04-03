using UnityEngine;
using System.Collections;

public class LPFFourPole : MonoBehaviour {

private double[] coef = new double[9];
private double[][] d = new double[][]{new double[4],new double[4]};
public double gain = 0;
public double Freq = 1000;
private double samplerate = 44100;
	
private double _Freq = 1000;
	
	private void Recalc()
	{
		_Freq=Freq;		
		samplerate = AudioSettings.outputSampleRate;
		
		double k,p,q,a;
		double a0,a1,a2,a3,a4;
		
		k=(4.0*gain-3.0)/(gain+1.0);
		p=1.0-0.25*k;p*=p;
		
		// LP:
		a=1.0/(System.Math.Tan(System.Math.PI*_Freq/samplerate)*(1.0+p));
		p=1.0+a;
		q=1.0-a;
		        
		a0=1.0/(k+p*p*p*p);
		a1=4.0*(k+p*p*p*q);
		a2=6.0*(k+p*p*q*q);
		a3=4.0*(k+p*q*q*q);
		a4=    (k+q*q*q*q);
		p=a0*(k+1.0);
		        
		coef[0]=p;
		coef[1]=4.0*p;
		coef[2]=6.0*p;
		coef[3]=4.0*p;
		coef[4]=p;
		coef[5]=-a1*a0;
		coef[6]=-a2*a0;
		coef[7]=-a3*a0;
		coef[8]=-a4*a0;
	}
	
// calculating coefficients:
void Awake()
{
	Recalc();
}
/*	
// or HP:
a=tan(0.5*omega)/(1.0+p);
p=a+1.0;
q=a-1.0;
        
a0=1.0/(p*p*p*p+k);
a1=4.0*(p*p*p*q-k);
a2=6.0*(p*p*q*q+k);
a3=4.0*(p*q*q*q-k);
a4=    (q*q*q*q+k);
p=a0*(k+1.0);
        
coef[0]=p;
coef[1]=-4.0*p;
coef[2]=6.0*p;
coef[3]=-4.0*p;
coef[4]=p;
coef[5]=-a1*a0;
coef[6]=-a2*a0;
coef[7]=-a3*a0;
coef[8]=-a4*a0;
	 */
// per sample:
		
	float lpf(float input, int channel )
	{
		double output=coef[0]*input+d[channel][0];
		d[channel][0]=coef[1]*input+coef[5]*output+d[channel][1];
		d[channel][1]=coef[2]*input+coef[6]*output+d[channel][2];
		d[channel][2]=coef[3]*input+coef[7]*output+d[channel][3];
		d[channel][3]=coef[4]*input+coef[8]*output;
		return (float)output;
	}
		
	/*formant end*/
	
	void Update()
	{
		if (_Freq!=Freq)
		{
			Recalc();
		}		
	}
	
	void OnAudioFilterRead (float[] data, int channels)
	{
		
		for (int i=0;i<data.Length;i = i+channels)
		{
			data[i]=lpf(data[i],0);			
			
			if (channels == 2)
				data[i+1]=lpf(data[i+1],1);			
		}
		
	}
	
}
