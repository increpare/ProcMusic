using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class GenericMelody {
	public List<int> voices;
	public List<float> frequencies;
	public List<float> durations;
	public List<float> volumes;
	
	public void BinaryRead(BinaryReader br) {		
		int voices_count = br.ReadInt32();
		voices = new List<int>(voices_count);
		for(int i=0;i<voices_count;i++) {
			voices.Add(br.ReadInt32());
		}
		
		int frequencies_count = br.ReadInt32();
		frequencies = new List<float>(frequencies_count);
		for(int i=0;i<frequencies_count;i++) {
			frequencies.Add(br.ReadSingle());
		}
		
		int durations_count = br.ReadInt32();
		durations = new List<float>(durations_count);
		for(int i=0;i<durations_count;i++) {
			durations.Add(br.ReadSingle());
		}
		
		int volumes_count = br.ReadInt32();
		volumes = new List<float>(volumes_count);
		for(int i=0;i<volumes_count;i++) {
			volumes.Add(br.ReadSingle());
		}		
	}
	
	public string Serialize()
	{
		var voicesstr = System.String.Join(",",voices.Select(v=>v.ToString()).ToArray());
		var frequenciesstr = System.String.Join(",",frequencies.Select(v=>v.ToString()).ToArray());
		var durationsstr = System.String.Join(",",durations.Select(v=>v.ToString()).ToArray());
		var volumesstr = System.String.Join(",",voices.Select(v=>v.ToString()).ToArray());
		return voicesstr+"|"+frequenciesstr+"|"+durationsstr+"|"+volumesstr;
	}
	
	public static GenericMelody Deserialize(string dat)
	{
		GenericMelody result = new GenericMelody();
		string[][] dats = dat.Split('|').Select(l => l.Split(',')).ToArray();
		
		result.voices = dats[0].Select(v => int.Parse(v)).ToList();
		result.frequencies = dats[1].Select(v => float.Parse(v)).ToList();
		result.durations = dats[2].Select(v => float.Parse(v)).ToList();
		result.volumes = dats[3].Select(v => float.Parse(v)).ToList();
		return result;
	}
	
	public void StripToLength(float newlength)
	{
		float length=0;
		for(int i=0;i<voices.Count;i++)
		{
			length+=durations[i];
			if (length>=newlength)
			{
				int toremove= voices.Count-(i+1);
				voices.RemoveRange(i+1,toremove);
				frequencies.RemoveRange(i+1,toremove);
				durations.RemoveRange(i+1,toremove);
				volumes.RemoveRange(i+1,toremove);
			}
		}
	}	
}
