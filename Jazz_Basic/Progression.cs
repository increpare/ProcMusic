using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Progression {
	
	public const int progressionlength = 30;
	
	public List<BasedChord> chords;
	public Progression(string description,List<Scale> chordlist)
	{
		//Dm7-G7,Cmaj7
		string[] bars = description.Split('|');
		List<string> chords_str = new List<string>();
		
		foreach (string bar in bars)
		{
			if (bar.IndexOf(',')>=0)
			{
				string[] subbar = bar.Split(',');
				chords_str.Add(subbar[0]);
				chords_str.Add(subbar[1]);
			}
			else
			{
				chords_str.Add(bar);
				chords_str.Add(bar);
			}
		}
		
		chords = new List<BasedChord>();
		foreach (string chord_desc in chords_str)
		{
			string trimmed =chord_desc.Trim();
			if (trimmed!="")
			{
				chords.Add(BasedChord.Parse(trimmed,chordlist));
			}
		}
		
		while(chords.Count<progressionlength)
		{
			List<BasedChord> chordcopy = new List<BasedChord>(chords);
			chords.AddRange(chordcopy);
		}
		
	}
	
	public static List<Progression> LoadFromFile(TextAsset dat_scales,List<Scale> chordlist)
	{
		List<Progression> progressions = new List<Progression>();		
		string[] lines = dat_scales.text.Split('\n'); 		
		foreach (string l in lines)
		{
			if (l.Trim()!="")
			{
				progressions.Add(new Progression(l,chordlist));
			}
		}	
		return progressions;
	}
}
