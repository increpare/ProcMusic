using UnityEngine;
using System.Collections;

public class Moog : MonoBehaviour {
	
	
	private double[][] fA = new double[][]{
		new double[5],
		new double[5]
	};
	private double[] fOld = new double[2];
	private double fQ;
	private double f2vg;
	private double fAcr;
	private double fF;
	private double fFs;
	
	public float Frequency=20000.0f;
	public float Resonance=0.3f;
	
	private float _Frequency=20000.0f;
	private float _Resonance=0.3f;

	
	const double i2 = 40000.0;
	const double i2v = 1/20000.0;
	const double noise = 1E-10;
	const double noi = 1E-10*((1.0/0x10000)/0x10000);
	const float mTwo = -2;
	const float c3 = 3;
	const float c6 = 6;
	const float c12 = 12;
	
	private double ipi = 4*System.Math.Atan(1);
	
	private float[] noisevals;
	private int noiseindex;
	
	void Awake()
	{
		fQ=1;
		fF=1000;
		fFs=AudioSettings.outputSampleRate;		
		
		noisevals = new float[48000];
		for (int i=0;i<48000;i++)
		{
			noisevals[i]=Random.Range(-1.0f,1.0f);
		}
		
		FrequencyCalc();		
		ResonanceCalc();
	}
	
	private void FrequencyCalc()
	{
		double fFc;
		double fFcr;
		
		if (Frequency<0)
		{
			Frequency=0;
			_Frequency=0;
		}
		fF=Frequency;		
		fFc=  fF/fFs;
		// frequency & amplitude correction
		fFcr = 1.8730*(fFc*fFc*fFc) + 0.4955*(fFc*fFc) - 0.6490*fFc + 0.9988;
		fAcr = -3.9364*(fFc*fFc) + 1.8409*fFc + 0.9968;
		f2vg = i2*(1-System.Math.Exp(-ipi*fFcr*fFc)); // Filter Tuning				
	}
	
	private void ResonanceCalc()
	{
		 fQ=(double)Resonance;
	}
	
	void Update()
	{
		if (fFs<=0)
			return;
		
		if (Frequency!=_Frequency)
		{
			_Frequency=Frequency;
			FrequencyCalc();
		}
		if (Resonance!=_Resonance)
		{
			Resonance = Mathf.Clamp(Resonance,0,1);
			_Resonance=Resonance;
			ResonanceCalc();
		}
	}
		
	float filter (float I, int channel)
	{
	 // cascade of 4 1st order sections
		fA[channel][0]=fA[channel][0]+f2vg*(System.Math.Tanh((I+(noise*noisevals[noiseindex])-2*fQ*fAcr*fOld[channel])*i2v)-System.Math.Tanh(fA[channel][1]*i2v));
		noiseindex = (noiseindex+1)%noisevals.Length;
		// fA[channel][1]=fA[channel][1]+(f2vg*(System.Math.Tanh((I+(noise*Random)-2*fQ*fOld[channel]*fAcr)*i2v)-System.Math.Tanh(fA[channel][1]*i2v)));
		fA[channel][1]=fA[channel][1]+f2vg*(System.Math.Tanh(fA[channel][0]*i2v)-System.Math.Tanh(fA[channel][1]*i2v));
		fA[channel][2]=fA[channel][2]+f2vg*(System.Math.Tanh(fA[channel][1]*i2v)-System.Math.Tanh(fA[channel][2]*i2v));
		fA[channel][3]=fA[channel][3]+f2vg*(System.Math.Tanh(fA[channel][2]*i2v)-System.Math.Tanh(fA[channel][3]*i2v));
		
		// 1/2-sample delay for phase compensation
		fOld[channel]=fA[channel][3]+fA[channel][4];
		fA[channel][4]=fA[channel][3];
		
		// oversampling
		fA[channel][0]=fA[channel][0]+f2vg*(System.Math.Tanh((-2*fQ*fAcr*fOld[channel])*i2v)-System.Math.Tanh(fA[channel][0]*i2v));
		fA[channel][1]=fA[channel][1]+f2vg*(System.Math.Tanh(fA[channel][0]*i2v)-System.Math.Tanh(fA[channel][1]*i2v));
		fA[channel][2]=fA[channel][2]+f2vg*(System.Math.Tanh(fA[channel][1]*i2v)-System.Math.Tanh(fA[channel][2]*i2v));
		fA[channel][3]=fA[channel][3]+f2vg*(System.Math.Tanh(fA[channel][2]*i2v)-System.Math.Tanh(fA[channel][3]*i2v));
		
		fOld[channel]=fA[channel][3]+fA[channel][4];
		fA[channel][4]=fA[channel][3];
		
		return (float)fOld[channel];
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
