
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scale {
	public string name;
	public List<bool> notes;
	public Scale(string description)
	{
		int tabplace = description.IndexOf('\t');
		name = description.Substring(0,tabplace);
		string scaledat = description.Substring(tabplace).Trim();
		notes = new List<bool>();
		for (int i=0;i<12;i++)
		{
			notes.Add(scaledat[i]=='1');
		}
	}
	
	public static List<Scale> LoadFromFile(TextAsset dat_scales)
	{
		List<Scale> scales = new List<Scale>();		
		string[] lines = dat_scales.text.Split('\n'); 		
		foreach (string l in lines)
		{
			scales.Add(new Scale(l));
		}	
		return scales;
	}
	
}
