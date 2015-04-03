using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

public class JazzAsync : MelodyGenerator
{	

	public int sound_channels=5;
	
	public List<Scale> scales;
	public List<Scale> chords;
	public List<Progression> progressions;
		
	// Use this for initialization
	public JazzAsync () {
		//generate scales
		scales = Scale.LoadFromFile(Resources.Load("Texts/scaledata") as TextAsset);
		//generate chords
		chords = Scale.LoadFromFile(Resources.Load("Texts/chorddata") as TextAsset);
		
		/*
		string chordnames ="";
		for (int i=0;i<chords.Count;i++)
		{
			chordnames+=chords[i].name+",";
		}
		Debug.Log("chord names:"+chordnames);
		*/
		
		//generate progressions
		progressions = Progression.LoadFromFile(Resources.Load("Texts/progressiondata") as TextAsset,chords);
		
		// audio stuff
		
		
		
		//
		
		//Generate();
	}
	
	public override GenericMelody GenerateMelody()
	{
			JazzGenerator generator = new JazzGenerator(scales,chords,progressions);
			float tempo_bpm=Random.Range(60.0f,160.0f);
			MelodyLineAsync music = new MelodyLineAsync(generator.melody,generator.accompaniment,tempo_bpm);					
			return music;
	}
}
