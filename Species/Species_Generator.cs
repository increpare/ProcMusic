using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

public class Species_Generator : MelodyGenerator {
	
	public int voices=4;
	public int length=200;
	
	public int[] vertical_perfect = {0,7};//will do octave repetitions manually
	public int[] vertical_imperfect = {3,4,8,9};
	public int[] vertical_consonance;
	
	public int[] horizontal_perfect = {0,7,12,7-12,-12};
	public int[] horizontal_imperfect = {3,4,5,8,9,3-12,4-12,5-12,8-12,9-12};
	public int[] horizontal_consonance;
	
	
	// move by step where possible
	// mix similar/contrary motion
	// melodic leaps should be followed by step in other direction
	
	//NEVER
	// leaps of sixth, 7th, or augmented forth (what about minor sixth up?)
	// same perfect consonance twice in succession

	//Avoid
	// similar motion
	// sequences or motifs
	//two consecutive melodic leaps which outline a triad
	
	// max of 2 consecutive imperfects
	// jump in one direction should result in step down
	
	
	private int[,] scales = {
//		{1,1,1,1,1,1,1,1,1,1,1,1},
		{1,0,1,0,1,1,0,1,0,1,0,1},//major
		{0,1,0,1,0,0,1,0,1,0,1,0},//pentatonic
		{1,0,1,1,0,1,0,1,1,0,0,1},//harmonic minor
//		{1,0,1,1,0,1,0,1,1,0,1,0},//natural minor
//		{1,0,1,1,0,1,0,1,0,1,0,1},//melodic minor
//		{1,0,1,1,0,1,0,1,1,1,1,1},//anything minor
		{1,0,1,1,0,1,1,1,0,0,1,0},//jazzy
//		{1,0,1,0,1,0,1,0,1,0,1,0}//wholetone
	};
	
	private List<int> scale;
	private List<int> scale_key;
	private List<List<bool>> activevoices;
	bool canon;
	bool diatoniccanon;
	
	public List<List<int>> lines;
	
	// start on triad
	
	public Species_Generator()
	{
		// STEP 1, GENERATE SETS
		//ah god, will octave matter here?  I think it might :/
		
		List<int> v_perf = new List<int>();
		List<int> v_imperf = new List<int>();
		for (int o=-2;o<=2;o++)
		{
			foreach (int i in vertical_perfect)
			{
				v_perf.Add(i+12*o);
			}
			foreach (int i in vertical_imperfect)
			{
				v_imperf.Add(i+12*o);
			}
		}
		
		v_perf.Sort();
		v_imperf.Sort();
		vertical_perfect = v_perf.ToArray();
		vertical_imperfect = v_imperf.ToArray();
		
		List<int> h_cons = new List<int>();
		List<int> v_cons = new List<int>();
		
		h_cons.AddRange(horizontal_perfect);
		h_cons.AddRange(horizontal_imperfect);
		v_cons.AddRange(vertical_perfect);
		v_cons.AddRange(vertical_imperfect);
		
		h_cons.Sort();
		v_cons.Sort();
		
		horizontal_consonance=h_cons.ToArray();
		vertical_consonance=v_cons.ToArray();
			
	}
	
	private List<int> PossibleNewNotes(int curnote,int lowerbound,int upperbound)
	{
		//Debug.Log(curnote.ToString()+","+lowerbound.ToString()+","+upperbound.ToString());
		List<int> newnotes = new List<int>();
		for (int i=0;i<horizontal_consonance.Length;i++)
		{
			int newnote = curnote + horizontal_consonance[i];
			if (newnote>=lowerbound && newnote<=upperbound)
			{
				newnotes.Add(newnote);
			}
		}
		
		return newnotes;
	}
	
	private bool Dissonant_H(int interval)
	{
		int v = (interval%12);
		for (int i=0;i<horizontal_consonance.Length;i++)
		{
			if (horizontal_consonance[i]==v)
				return false;
		}
		return true;
	}
	
	private bool Dissonant_V(int interval)
	{
		int v = (interval%12);
		for (int i=0;i<vertical_consonance.Length;i++)
		{
			if (vertical_consonance[i]==v)
				return false;
		}
		return true;
	}
	
	private bool Perfect_V(int interval)
	{
		int v = (interval%12);
		for (int i=0;i<vertical_perfect.Length;i++)
		{
			if (vertical_perfect[i]==v)
				return true;
		}
		return false;
	}
	
	private void VetTripleShapes(ref List<int> possiblenewnotes, int back, int backback)
	{
//		Debug.Log("before vetting " + possiblenewnotes.Count.ToString());
		for (int i=possiblenewnotes.Count-1;i>=0;i--)
		{
			int candnote = possiblenewnotes[i];
			
			int da = back-backback;
			int db = candnote-back;
			
			bool removenote=false;
			//If writing two skips in the same direction—something which must be done only rarely—
			//the second must be smaller than the first, and the interval between the first and 
			//the third note may not be dissonant.
			if ((da>0) == (db>0) && (System.Math.Abs(da)>2) && (System.Math.Abs(db)>2))
			{
				if ((da<db))
					removenote=true;
				if (Dissonant_H(da+db))
					removenote=true;
			}
				
				
			//The interval of a tritone in three notes is to be avoided (for example, an ascending 
			//melodic motion F - A - B natural), 
			if (backback<back && back<candnote)
			{
				if (da+db == 6)
					removenote=true;
				
				
				//as is the interval of a seventh in three notes.
				if (da+db == 10 || da+db==11)
					removenote=true;
			}
			
			if (removenote)
			{
				possiblenewnotes.RemoveAt(i);
			}
				
				
		}
		//Debug.Log("after vetting " + possiblenewnotes.Count.ToString());
				
				
		//prioritizing
		//If writing a skip in one direction, it is best to proceed after the skip with motion in the other direction.
				
				
		// want to avoid repeated notes in leading melody?
	}
	
	private int extendcount;
	
	private bool ExtendMelodies(ref List<List<int>> melodies, int desiredlength, int lowerbound, int upperbound)
	{
		if (extendcount>100000)
			return false;
		
		int min = 100000;
		for(int i=0;i<melodies.Count;i++)
		{
			min = System.Math.Min(min,melodies[i].Count);
		}
						
		if (min==desiredlength)
			return true;
		
		
		List<int> minindices = new List<int>();
		for (int i=0;i<melodies.Count;i++)
		{
			if (melodies[i].Count==min)
				minindices.Add(i);
		}
		
		Utils.Utility.Shuffle(minindices);
		
		for (int i=0;i<minindices.Count;i++)
		{		
			if (ExtendMelody(ref melodies,minindices[i],desiredlength,lowerbound,upperbound))
			{
				return true;
			}
		}
		
		return false;
	}
	
	private bool NoteInScale(int n,int time)
	{
		return scales[scale[time],((((n+scale_key[time]+12)%12)+12)%12)]==1;
	}
	
	private int NearScaleNote(int n, int time)
	{
		int direction = Random.value>0.5 ? 1 : -1;
		while(true)
		{
			if (scales[scale[time],((((n+scale_key[time]+12)%12)+12)%12)]==1)
			{
				return n;
			}
			n+=direction;
		}
		
	}
	
	private void VetNoteInScale(ref List<int> possiblenewnotes, int time)
	{
		for (int i=possiblenewnotes.Count-1;i>=0;i--)
		{
			if (!NoteInScale(possiblenewnotes[i],time))
			{
				possiblenewnotes.RemoveAt(i);
			}
		}
	}
	
	private void VetVSlice(ref List<int> possiblenewnotes,List<int> vslice)
	{
		for (int i=possiblenewnotes.Count-1;i>=0;i--)
		{		
			int candnote = possiblenewnotes[i];
				
			bool removenote=false;
			
			for (int j=0;j<vslice.Count;j++)
			{
				int existingnote=vslice[j];
				int interval=System.Math.Abs(candnote-existingnote);
				if (Dissonant_V(interval) 
					|| interval==0//remove unisons
					)
				{
					removenote=true;
					break;
				}
			}
			
			if (removenote)
			{
				possiblenewnotes.RemoveAt(i);
			}
		}
	}
	
	private void VetParallels(
		ref List<int> possiblenewnotes,
		List<int> vslice,
		List<int> vsliceback,
		int lastnote)
	{
		for (int i=possiblenewnotes.Count-1;i>=0;i--)
		{
			int candnote = possiblenewnotes[i];
			
			bool removenote=false;
			
			for (int j=0;j<vslice.Count;j++)
			{
				int existingnote=vslice[j];
				int existingnoteback=vsliceback[j];
				if (existingnoteback==666)
					continue;
				
				int interval1=System.Math.Abs(candnote-existingnote);
				int interval2=System.Math.Abs(lastnote-existingnoteback);
				
				if (Perfect_V(interval1))
				{
					//no parallels
					if (interval1==interval2)
					{
						removenote=true;
						break;
					}				
				
					// no approaching by similar motion
					if ( (candnote<=lastnote) ==  (existingnote<=existingnoteback))
					{
						removenote=true;
						break;
					}
					
					//remove crossings
					if ((candnote<existingnote)!=(lastnote<existingnoteback))
					{
						removenote=true;
						break;
					}
				}
				
			}			
			
			if (removenote)
			{
				possiblenewnotes.RemoveAt(i);
			}
		}
	}
	
	
		
	private void VetRepeatedChords(
		ref List<int> possiblenewnotes,
		List<int> vslice,
		List<int> vsliceback,
		int lastnote)
	{
		for (int i=0;i<vslice.Count;i++)
		{
			if (vslice[i]!=vsliceback[i])
			{
				//Debug.Log(vslice[i].ToString() + "-"+vsliceback[i].ToString());
				return;
			}
		}
		
		//Debug.Log("Vetting");
		
		for (int i=possiblenewnotes.Count-1;i>=0;i--)
		{
			bool removenote=false;
			if (possiblenewnotes[i]==lastnote)
			{
				removenote=true;
			}
			
			if (removenote)
			{
				possiblenewnotes.RemoveAt(i);
			}
		}
	}
	
	private int[] temporaldisplacement3 = {0,8,16};
	
	private int[] temporaldisplacement4 = {0,8,16,24};		
	
	private int[] temporaldisplacement;
	int leadingcanonvoice;

	
	private int[][] canonconfiguration = {
		new int[]{0,-5,-12,-17} ,
		new int[]{0,-3,-7,-8,-15},
		new int[]{0,-8,-12,-15},
		new int[]{0,-7,-12,-15}
	};
	
	
	private bool ExtendMelody(ref List<List<int>> melodies,int melodyindex,int desiredlength,int lowerbound, int upperbound)
	{							
		extendcount++;
	
		if (extendcount>100000)
			return false;
		
		int newnotepos = melodies[melodyindex].Count;
		
		if (canon)
		{
			if (melodyindex!=leadingcanonvoice)
			{
				int notetoadd;
				//if should have started yet			
				if (newnotepos>=temporaldisplacement[melodyindex])
				{
					notetoadd = melodies[leadingcanonvoice][newnotepos-temporaldisplacement[melodyindex]]+canonconfiguration[canonform][melodyindex];
					if (diatoniccanon)
					{
						notetoadd=NearScaleNote(notetoadd,newnotepos);
					}
				}
				else
				{
					notetoadd=666;
				}
				
				melodies[melodyindex].Add(notetoadd);
				if (ExtendMelodies(ref melodies,desiredlength,lowerbound,upperbound))
				{
					return true;
				}
				else
				{
					melodies[melodyindex].RemoveAt(melodies[melodyindex].Count-1);
					return false;
				}
				
			}
		}
		else
		{
			if (!activevoices[melodyindex][newnotepos])
			{
				melodies[melodyindex].Add(666);
				if (ExtendMelodies(ref melodies,desiredlength,lowerbound,upperbound))
				{
					return true;
				}
				else
				{
					melodies[melodyindex].RemoveAt(melodies[melodyindex].Count-1);
					return false;
				}
			}
		}
		
		int boundoffset=-9*melodyindex;
		
		List<int> possiblenewnotes = new List<int>();
		if (newnotepos==0 ||melodies[melodyindex][newnotepos-1]==666)
		{
			for (int i=lowerbound+boundoffset;i<=upperbound+boundoffset;i++)
			{
				possiblenewnotes.Add(i);
			}
		}
		else
		{
			possiblenewnotes = PossibleNewNotes(melodies[melodyindex][newnotepos-1],lowerbound+boundoffset,upperbound+boundoffset);			
		}
		
		VetNoteInScale(ref possiblenewnotes,newnotepos);
		
		Utils.Utility.Shuffle(possiblenewnotes);
		
		//if can look back two notes
		if (newnotepos>1)
		{
			VetTripleShapes(ref possiblenewnotes,melodies[melodyindex][newnotepos-1],melodies[melodyindex][newnotepos-2]);
		}
		
		//vet verticle harmonies
		List<int> vslice = new List<int>();
		List<int> vsliceback = new List<int>();
		for (int i=0;i<melodies.Count;i++)
		{
			if (i==melodyindex)
				continue;
			
			if (melodies[i].Count>newnotepos)
			{
				int newnote = melodies[i][newnotepos];
				if (newnote!=666)
				{
					vslice.Add(melodies[i][newnotepos]);
					
					
					if (newnotepos>0)
					{
						vsliceback.Add(melodies[i][newnotepos-1]);
					}
				}
				
			}
		}
		
		VetVSlice(ref possiblenewnotes,vslice);
		
		if (newnotepos>0)
		{
			VetParallels(ref possiblenewnotes,vslice,vsliceback,melodies[melodyindex][newnotepos-1]);
			
			if (vslice.Count==melodies.Count-1)
			{
				VetRepeatedChords(ref possiblenewnotes,vslice,vsliceback,melodies[melodyindex][newnotepos-1]);
			}
		}		
		
		for (int i=0;i<possiblenewnotes.Count;i++)
		{		
			melodies[melodyindex].Add(possiblenewnotes[i]);
			
//			Debug.Log("adding note 2 " + note.ToString());
			
			if (ExtendMelodies(ref melodies,desiredlength,lowerbound,upperbound))
			{
				return true;
			}
			else
			{
				melodies[melodyindex].RemoveAt(melodies[melodyindex].Count-1);
			}
		}
		return false;		
	}
	
	private int canonform;
	
	private void GenerateLines()
	{
		voices = Random.Range(3,5);
		scale= new List<int>();
		
		scale_key = new List<int>();
		
		List<int> canonentry;
		if (voices==3)
		{
			canonentry = new List<int>(temporaldisplacement3);
		}
		else
		{
			canonentry = new List<int>(temporaldisplacement4);
		}
		Utils.Utility.Shuffle(canonentry);
		temporaldisplacement=canonentry.ToArray();
		leadingcanonvoice=canonentry.IndexOf(0);
		
	
		canonform = Random.Range(0,canonconfiguration.Length);

		canon = Random.Range(0,4)==0;
		diatoniccanon = Random.Range(0,1)==0;
		int keyperiod = Random.Range(3,7)*4;
		int scaleperiod = keyperiod*Random.Range(2,4)*4;
		
		for (int i=0;i<length;i+=keyperiod)
		{
			int key = Random.Range(0,12);
			for (int j=0;j<keyperiod && scale_key.Count<length;j++)
			{
				scale_key.Add(key);				
			}
		}
		
		for (int i=0;i<length;i+=scaleperiod)
		{
			int key = Random.Range(0,scales.GetLength(0));
			for (int j=0;j<scaleperiod && scale.Count<length;j++)
			{
				scale.Add(key);				
			}
		}
		
		int voiceperiod = keyperiod*Random.Range(1,4);
		activevoices = new List<List<bool>>();
		for (int i=0;i<voices;i++)
		{
			activevoices.Add(new List<bool>());
		}
		
		List<int> curvoices = new List<int>();
		curvoices.Add(0);
		curvoices.Add(1);
		
		for (int i=0;i<length;i+=voiceperiod)
		{
			//Debug.Log(Random.value.ToString()+"voice period");
			if (curvoices.Count>2 && curvoices.Count<voices)
			{
				if (Random.value<0.5)
				{
			//		Debug.Log(Random.value.ToString()+"removing");
					//remove a random voice
					curvoices.RemoveAt(Random.Range(0,curvoices.Count));
				}
				else
				{
					//add a random voice
					int v=Random.Range(0,voices-curvoices.Count);
					for (int j=0;j<voices;j++)
					{
						if (curvoices.IndexOf(j)==-1)
						{
							v--;
														
							if (v==-1)
							{
								//Debug.Log(Random.value.ToString()+"adding");
								curvoices.Add(j);
							}
						}
					}
				}
			}
			else if (curvoices.Count>2)
			{		
				{
					//Debug.Log(Random.value.ToString()+"removing");
					//remove a random voice
					curvoices.RemoveAt(Random.Range(0,curvoices.Count));
				}
			}
			else
			{
				//Debug.Log(Random.value.ToString()+"adding");
				{
					//add a random voice
					int v=Random.Range(0,voices-curvoices.Count);
					for (int j=0;j<voices;j++)
					{
						if (curvoices.IndexOf(j)==-1)
						{
							v--;
														
							if (v==-1)
							{
								//Debug.Log(Random.value.ToString()+"adding");
								curvoices.Add(j);
							}
						}
					}
				}				
			}
			/*
			string s = ".;
			for (int j=0;j<curvoices.Count;j++)
			{
				s+=curvoices[j].ToString();
			}
			s+=";
			Debug.Log(s);*/
			for (int j=0;j<voiceperiod && activevoices[0].Count<length;j++)
			{
				for (int k=0;k<voices;k++)
				{
					activevoices[k].Add(curvoices.IndexOf(k)>=0);
				}
			}
		}
		
		for (int j=0;j<10;j++)
		{
			lines = new List<List<int>>();
			for (int i=0;i<voices;i++)
			{
				lines.Add(new List<int>());
			}
			
			int lowerbound = 0;
			int upperbound = 20;
						
			extendcount=0;
			//Debug.Log(Random.value.ToString()+" tick");
			if (ExtendMelodies(ref lines,length,lowerbound,upperbound))
			{
				SecondSpeciesPass();
				break;
			}
		}		
	}
	
	private void SecondSpeciesPass()
	{
		List<int> voicearray = new List<int>();
		for (int i=0;i<lines.Count;i++)
		{
			voicearray.Add(i);
		}
		
		for (int t=0;t<lines[0].Count-1;t+=2)
		{
			Utils.Utility.Shuffle(voicearray);
			
			for (int i=0;i<voicearray.Count;i++)
			{
				int newnote=lines[i][t];
				
				if (lines[i][t]==666||lines[i][t+1]==666)
				{
				}
				else
				{				
					//1 see if it qualifies for a passing note
					if (ScaleDistance(lines[i][t],lines[i][t+1],t/2)==2)
					{
						newnote = RandomNoteBetween(lines[i][t],lines[i][t+1],t/2);
						//Debug.Log(lines[i][t].ToString() + ","+newnote.ToString()+","+lines[i][t+1].ToString());
					}
					//2 calculate vertical slice
				}
				
				//Debug.Log("added note");
				lines[i].Insert(t+1,newnote);
			}
			
		}
	}
	
	
	private int ScaleDistance(int a, int b,int t)
	{
		//convert to scale space
		a+=scale_key[t];
		b+=scale_key[t];
		if (a>b)
		{
			int c;
			c=a;
			a=b;
			b=c;
		}
		
		int dist=0;
		//count number of 1s between a and b (inclusive of b)
		for (int i=a+1;i<=b;i++)
		{
			if (scales[scale[t],(((i)%12)+12)%12]==1)
			{
				dist++;
			}
		}
		return dist;
	}
	
	private int RandomNoteBetween(int a, int b, int t)
	{
		//convert to scale space
		a+=scale_key[t];
		b+=scale_key[t];
		if (a>b)
		{
			int c;
			c=a;
			a=b;
			b=c;
		}
		
		List<int> notesbetween = new List<int>();
		//count number of 1s between a and b (inclusive of b)
		for (int i=a+1;i<b;i++)
		{
			if (scales[scale[t],(((i)%12)+12)%12]==1)
			{
				notesbetween.Add(i-scale_key[t]);
			}
		}
		return Utils.Utility.RandomElement(notesbetween);
	}
	
	
	public override GenericMelody GenerateMelody()
	{
		GenerateLines();
		
		float tempo_bpm=Random.Range(60.0f,260.0f);
		
		//Debug.Log ("tempo = " + tempo_bpm );
		
		List<int> sounds = new List<int>();
		for (int i=0;i<lines.Count;i++)
		{
			sounds.Add(Random.Range(0,5));
		}
			
		float beatlength = 60.0f/(tempo_bpm);
		
		GenericMelody melody = new GenericMelody();
		melody.frequencies= new List<float>();
		melody.voices = new List<int>();
		melody.volumes = new List<float>();
		melody.durations = new List<float>();
		//Debug.Log("specifying " + melody.specifychannels.ToString());
		//for each vertical slice
		for (int j=0;j<lines[0].Count;j++)
		{
			for (int i=0;i<lines.Count;i++)
			{
				if (lines[i][j]!=666)
				{
					if ((j==0) || (lines[i][j]!=lines[i][j-1]))
					{
						melody.frequencies.Add(Utils.Utility.NoteToFreq(lines[i][j]));
						melody.voices.Add(sounds[i]);
						melody.volumes.Add(1.0f);
						melody.durations.Add(0.0f);
					}
				}
			}
			
			melody.durations[melody.durations.Count-1]+=beatlength;			
		}
		
		return melody;
	}
}
