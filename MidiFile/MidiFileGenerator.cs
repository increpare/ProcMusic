using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

public class MidiFileGenerator : MelodyGenerator {
		
	TextAsset ta;
	private GenericMelody gm;
	private float timescale=1000;

	public MidiFileGenerator(TextAsset dat)
	{
		ta=dat;
		var s = new System.IO.MemoryStream(ta.bytes);
		var br = new System.IO.BinaryReader(s);
		
		var mfr = new Multimedia.Midi.MidiFileReader (br);
	
		
		GenericMelody melody = new GenericMelody();
		melody.frequencies= new List<float>();
		melody.voices = new List<int>();
		melody.volumes = new List<float>();
		melody.durations = new List<float>();

		int accum = 0;
		foreach (var track in mfr.tracks) {
	
		for (int i=0;i<track.Count;i++){
			var e = track[i];
			 
			var cm = e.Message is Multimedia.Midi.ChannelMessage ? (Multimedia.Midi.ChannelMessage)e.Message : null;

			accum+=e.Ticks;
			if (cm!=null && cm.Command == Multimedia.Midi.ChannelCommand.NoteOn){
				melody.frequencies.Add(Utils.Utility.NoteToFreq(cm.Data1-64));
				melody.voices.Add(0);
				melody.volumes.Add(1.0f);
				if (melody.durations.Count>0){				
					melody.durations[melody.durations.Count-1]=accum/timescale;
				}
				melody.durations.Add(0);
				accum=0;
			}				
		}
		gm = melody;
		}
	}

	public override GenericMelody GenerateMelody()
	{				
		return gm;
		/*
		float lasttime=0;
		for (int i=0;i<flatnotes.Count;i++)
		{
			float duration = flatnotes[i].onset-lasttime;
			
			melody.frequencies.Add(flatnotes[i].pitch);
			melody.voices.Add(sounds[flatnotes[i].voice]);
			melody.volumes.Add(flatnotes[i].volume);
			melody.durations.Add(duration);
			
			lasttime=flatnotes[i].onset;
		}	
		 */
		
	}
}