using UnityEngine;
using System.Collections;

public class Formant : MonoBehaviour {

	/* formant begin */
	public int vowelnum=0;
	
	private double[][] coeff = new double[][]{
		new double[]{ 3.11044e-06,
		8.943665402,	-36.83889529,	92.01697887,	-154.337906,	181.6233289,
		-151.8651235, 89.09614114,	-35.10298511,	8.388101016,	-0.923313471 ///A
		},
		new double[]{4.36215e-06,
		8.90438318,	-36.55179099,	91.05750846,	-152.422234,	179.1170248, ///E
		-149.6496211,87.78352223,	-34.60687431,	8.282228154,	-0.914150747
		},
		new double[]{ 3.33819e-06,
		8.893102966,	-36.49532826,	90.96543286,	-152.4545478,	179.4835618,
		-150.315433,	88.43409371,	-34.98612086,	8.407803364,	-0.932568035 ///I
		},
		new double[]{1.13572e-06,
		8.994734087,	-37.2084849,	93.22900521,	-156.6929844,	184.596544, ///O
		-154.3755513,	90.49663749,	-35.58964535,	8.478996281,	-0.929252233
		},
		new double[]{4.09431e-07,
		8.997322763,	-37.20218544,	93.11385476,	-156.2530937,	183.7080141, ///U
		-153.2631681,	89.59539726,	-35.12454591,	8.338655623,	-0.910251753
		}
	}; 
	
	private double[][] memory=new double[][]{new double[]{0,0,0,0,0,0,0,0,0,0},new double[]{0,0,0,0,0,0,0,0,0,0}};
	
	private float formantfilter(float input,int channel)
	{
		float res =	(float)(
					coeff[vowelnum][0] *input +
					coeff[vowelnum][1] *memory[channel][0] + 
					coeff[vowelnum][2] *memory[channel][1] +
					coeff[vowelnum][3] *memory[channel][2] +
					coeff[vowelnum][4] *memory[channel][3] +
					coeff[vowelnum][5] *memory[channel][4] +
					coeff[vowelnum][6] *memory[channel][5] +
					coeff[vowelnum][7] *memory[channel][6] +
					coeff[vowelnum][8] *memory[channel][7] +
					coeff[vowelnum][9] *memory[channel][8] +
					coeff[vowelnum][10] *memory[channel][9]);
		
		memory[channel][9]= memory[channel][8];
		memory[channel][8]= memory[channel][7];
		memory[channel][7]= memory[channel][6];
		memory[channel][6]= memory[channel][5];
		memory[channel][5]= memory[channel][4];
		memory[channel][4]= memory[channel][3];
		memory[channel][3]= memory[channel][2];
		memory[channel][2]= memory[channel][1];	
		memory[channel][1]= memory[channel][0];
		memory[channel][0]= res;
		
		return res;
	}
	
	/*formant end*/
	
	void OnAudioFilterRead (float[] data, int channels)
	{
		for (int i=0;i<data.Length;i = i+channels)
		{
			data[i]=formantfilter(data[i],0);			
			
			if (channels == 2)
				data[i+1]=formantfilter(data[i+1],1);			
		}
		
	}
	
	
}
