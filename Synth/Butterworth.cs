using UnityEngine;
using System.Collections;

public class Butterworth : MonoBehaviour {
		
	private float[] t = new float[4];
	private float[] coef = new float[4];
	private float[][] history = new float[][]{new float[4],new float[4]};
	private float gain;
	private float min_cutoff, max_cutoff;
		
	private const float BUDDA_Q_SCALE = 6.0f;
	
	void Awake()
	{
		history[0][0]=0;
		history[0][1]=0;
		history[0][2]=0;
		history[0][3]=0;
		history[1][0]=0;
		history[1][1]=0;
		history[1][2]=0;
		history[1][3]=0;
		
		SetSampleRate(AudioSettings.outputSampleRate);
		Set(22050.0f,0.0f);
	}
	
	private void SetSampleRate(float fs)
	{
		float pi = 4.0f * Mathf.Atan(1.0f);
		
		t[0] = 4.0f * fs * fs;
		t[1] = 8.0f * fs * fs;
		t[2] = 2.0f * fs;
		t[3] = pi / fs;
		
		min_cutoff = fs * 0.01f;
		max_cutoff = fs * 0.45f;
	}
		
	
	public float Cutoff=22050.0f;
	public float Q=0.0f;
		
	private float _Cutoff;
	private float _Q;
	void Update()
	{
		if (Cutoff!=_Cutoff || Q!=_Q)
		{
			Q=Mathf.Clamp(Q,0.0f,1.0f);
			Cutoff=Mathf.Clamp(Cutoff,min_cutoff,max_cutoff);
			_Cutoff=Cutoff;
			_Q=Q;
			Set (_Cutoff,Q);
		}
	}
	
	void Set(float cutoff, float q)
	{
		if (cutoff < this.min_cutoff)
			cutoff = this.min_cutoff;
		else if(cutoff > this.max_cutoff)
			cutoff = this.max_cutoff;
		
		if(q < 0.0f)
			q = 0.0f;
		else if(q > 1.0f)
			q = 1.0f;
		
		float wp = this.t[2] * Mathf.Tan(this.t[3] * cutoff);
		float bd, bd_tmp, b1, b2;
		
		q *= BUDDA_Q_SCALE;
		q += 1.0f;
		
		b1 = (0.765367f / q) / wp;
		b2 = 1.0f / (wp * wp);
		
		bd_tmp = this.t[0] * b2 + 1.0f;
		
		bd = 1.0f / (bd_tmp + this.t[2] * b1);
		
		this.gain = bd * 0.5f;
		
		this.coef[2] = (2.0f - this.t[1] * b2);
		
		this.coef[0] = this.coef[2] * bd;
		this.coef[1] = (bd_tmp - this.t[2] * b1) * bd;
		
		b1 = (1.847759f / q) / wp;
		
		bd = 1.0f / (bd_tmp + this.t[2] * b1);
		
		this.gain *= bd;
		this.coef[2] *= bd;
		this.coef[3] = (bd_tmp - this.t[2] * b1) * bd;
	}
		
	float filter (float input, int channel)
	{
		float output = input * this.gain;
		float new_hist;
		
		output -= this.history[channel][0] * this.coef[0];
		new_hist = output - this.history[channel][1] * this.coef[1];
		
		output = new_hist + this.history[channel][0] * 2.0f;
		output += this.history[channel][1];
		
		this.history[channel][1] = this.history[channel][0];
		this.history[channel][0] = new_hist;
		
		output -= this.history[channel][2] * this.coef[2];
		new_hist = output - this.history[channel][3] * this.coef[3];
		
		output = new_hist + this.history[channel][2] * 2.0f;
		output += this.history[channel][3];
		
		this.history[channel][3] = this.history[channel][2];
		this.history[channel][2] = new_hist;
		
		return output;
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
