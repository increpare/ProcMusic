using UnityEngine;
using System.Collections.Generic;
using System.Linq;
 
[RequireComponent(typeof(AudioSource))]
public class SpeechSynth : MonoBehaviour
{              
        /* BEGIN PASTE FROM DUDE'S CODE*/
       
        private int SAMPLE_FREQUENCY            = 44100;
        private const int PLAY_TIME                             = 32;
        private const float MASTER_VOLUME               = 0.0003f;
        private const float PI                                  = 3.1415926535f;
        private const float PI_2                                = 2*PI;
 
        private float CutLevel ( float x, float lvl )
        {
                if ( x > lvl )
                        return lvl;
                if ( x < -lvl )
                        return -lvl;
                return x;
        }
       
        private float Sawtooth ( float x )
        {
                return ( 0.5f - ( x - Mathf.Floor ( x / PI_2 ) * PI_2 ) / PI_2 );
        }
       
        private class Shape {
                public byte len;
                public byte amp;
                public byte osc;
                public byte plosive;
                public Shape ( byte len, byte amp, byte osc, byte plosive)
                {
                        this.len=len;
                        this.amp=amp;
                        this.osc=osc;
                        this.plosive=plosive;
                }
        }
       
        private class Phoneme {
                public char p;
                public int[] f = new int[3];
                public int[] w = new int[3];
                public Shape shape;
               
                public Phoneme(char p, int[] f, int[] w, Shape shape)
                {
                        this.p=p;
                        this.f=f;
                        this.w=w;
                        this.shape=shape;
                }
        }
       
        Phoneme[] g_phonemes = new Phoneme[]{
                new Phoneme('o',        new int[]{12,15,0},      new int[]{10,10,0},    new Shape(3,6,0,0)),
                new Phoneme('i',        new int[]{5,56,0},       new int[]{10,10,0},    new Shape(3,3,0,0)),   
                new Phoneme('j',        new int[]{5,56,0},       new int[]{10,10,0},    new Shape(1,3,0,0)),   
                new Phoneme('u',        new int[]{5,14,0},       new int[]{10,10,0},    new Shape(3,3,0,0)),   
                new Phoneme('a',        new int[]{18,30,0},      new int[]{10,10,0},    new Shape(3,15,0,0)),  
                new Phoneme('e',        new int[]{14,50,0},      new int[]{10,10,0},    new Shape(3,15,0,0)),  
                new Phoneme('E',        new int[]{20,40,0},      new int[]{10,10,0},    new Shape(3,12,0,0)),  
                new Phoneme('w',        new int[]{3,14,0},       new int[]{10,10,0},    new Shape(3,1,0,0)),   
                new Phoneme('v',        new int[]{2,20,0},       new int[]{20,10,0},    new Shape(3,3,0,0)),   
               
                new Phoneme('T',        new int[]{2,20,0},       new int[]{40,1,0},     new Shape(3,5,0,0)),   
                new Phoneme('z',        new int[]{5,28,80},      new int[]{10,5,10},    new Shape(3,3,0,0)),   
                new Phoneme('Z',        new int[]{4,30,60},      new int[]{50,1,5},     new Shape(3,5,0,0)),   
                new Phoneme('b',        new int[]{4,0,0},        new int[]{10,0,0},     new Shape(1,2,0,0)),   
                new Phoneme('d',        new int[]{4,40,80},      new int[]{10,10,10},   new Shape(1,2,0,0)),   
                new Phoneme('m',        new int[]{4,20,0},       new int[]{10,10,0},    new Shape(3,2,0,0)),   
                new Phoneme('n',        new int[]{4,40,0},       new int[]{10,10,0},    new Shape(3,2,0,0)),   
                new Phoneme('r',        new int[]{3,10,20},      new int[]{30,8,1},     new Shape(3,3,0,0)),   
                new Phoneme('l',        new int[]{8,20,0},       new int[]{10,10,0},    new Shape(3,5,0,0)),   
                new Phoneme('g',        new int[]{2,10,26},      new int[]{15,5,2},     new Shape(2,1,0,0)),
                       
                new Phoneme('f',        new int[]{8,20,34},      new int[]{10,10,10},   new Shape(3,4,1,0)),   
                new Phoneme('h',        new int[]{22,26,32},     new int[]{30,10,30},   new Shape(1,10,1,0)),  
                new Phoneme('s',        new int[]{80,110,0},     new int[]{80,40,0},    new Shape(3,5,1,0)),   
                new Phoneme('S',        new int[]{20,30,0},      new int[]{100,100,0},  new Shape(3,10,1,0)),  
               
                new Phoneme('p',        new int[]{4,10,0},       new int[]{5,10,10},    new Shape(1,2,1,1)),   
                new Phoneme('t',        new int[]{4,20,0},       new int[]{10,20,5},    new Shape(1,3,1,1)),   
                new Phoneme('k',        new int[]{20,80,0},      new int[]{10,10,0},    new Shape(1,3,1,1))
        };
       
       
        // Synthesizes speech and adds it to specified buffer
        private void SynthSpeech ( float[] buffer, int buf, string text ) {
                // Loop through all phonemes
 
                for (int i=0;i<text.Length;i++)
                {
                        char l = text[i];                      
                        // Find phoneme description
                        Phoneme p = g_phonemes[0];
                        float v=0;
                        if (l!=' ')
                        {
                                p = g_phonemes.First(phon => phon.p == l);
                                v = p.shape.amp;
                        }
                       
                        // Generate sound
                        int sl = p.shape.len * (SAMPLE_FREQUENCY / 15);
                        for ( int f = 0; f < 3; f++ ) {
                                int ff = p.f[f];
                                float freq = (float)ff*(50.0f/SAMPLE_FREQUENCY);
                                if ( ff==0 )
                                        continue;
                                float buf1Res = 0;
                                float buf2Res = 0;
                                int b = buf;
                                float q = 1.0f - p.w[f] * (PI * 10.0f / SAMPLE_FREQUENCY);
                                float xp = 0;
                                for ( int s = 0; s < sl; s++ ) {
                                        float n = Random.Range(-0.5f,0.5f);
                                        float x = n;
                                        if ( p.shape.osc == 0) {
                                                x = Sawtooth ( s * (120.0f * PI_2 / SAMPLE_FREQUENCY) );
                                                xp = 0;
                                        }
                                        // Apply formant filter
                                        x = x + 2.0f * Mathf.Cos ( PI_2 * freq ) * buf1Res * q - buf2Res * q * q;
                                        buf2Res = buf1Res;
                                        buf1Res = x;
                                        x = 0.75f * xp + x * v;
                                        xp = x;
                                        // Anticlick function
                                        x *= CutLevel ( Mathf.Sin ( PI * s / sl ) * 5, 1 );
                                        buffer[b++] += x;
                                        buffer[b++] += x;
                                }
                        }
                        // Overlap neighbour phonemes
                        buf += ((3*sl/4)<<1);
                        if ( p.shape.plosive != 0)
                                buf += (sl&0xfffffe);
                }
        }
       
        // Synthesizes speech and adds it to specified buffer (a little bit more complex synth)
        private void SynthSpeechComplex (
                float[] buffer,
                int buf,
                string text,
                string lengths_,
                string amps_,
                bool whisper,
                float f0 )
        {
                int[] lengths = lengths_.Select( c => (int)c).ToArray();
                int[] amps = amps_.Select( c => (int)c).ToArray();
                // DEBUG ONLY!!! Used to catch string length mismatch.
                /*if ( strlen ( text ) != strlen ( len ) || strlen ( text ) != strlen ( amp ) ) {
                        int tl = strlen ( text );
                        int ll = strlen ( len );
                        int al = strlen ( amp );
                        f0 = f0;
                }*/
                // Loop through all phonemes
 
//                      for ( char* l = text, *ln = len, *am = amp; *l; l++, ln++, am++ ) {
 
                for ( int i=0;i<text.Length;i++)
                {
                        char l = text[i];
                        int ln = lengths[i];
                        int am = amps[i];
               
                        // Find phoneme description
                        Phoneme p = g_phonemes[0];
                        float v=0;
                        if (l!=' ')
                        {
                                p = g_phonemes.First(phon => phon.p == l);
                                v = p.shape.amp;
                        }
                       
                        // Generate sound
                        int sl = (int) ( p.shape.len * (SAMPLE_FREQUENCY / 15) * (ln/32.0f) );
                        for ( int f = 0; f < 3; f++ ) {
                                int ff = p.f[f];
                                float freq = (float)ff*(50.0f/SAMPLE_FREQUENCY);
                                if ( ff == 0 )
                                        continue;
                                float buf1Res = 0, buf2Res = 0;
                                float q = 1.0f - p.w[f] * (PI * 10.0f / SAMPLE_FREQUENCY);
                                int b = buf;
                                float xp = 0;
                                for ( int s = 0; s < sl; s++ ) {
                                        float n = Random.Range(-0.5f,0.5f);
                                        float x = n;
                                        if ( p.shape.osc==0 && !whisper ) {
                                                x = Sawtooth ( s * (f0 * PI_2 / SAMPLE_FREQUENCY) );
                                                xp = 0;
                                        }
                                        x *= am/32.0f;
                                        // Apply formant filter
                                        x = x + 2.0f * Mathf.Cos ( PI_2 * freq ) * buf1Res * q - buf2Res * q * q;
                                        buf2Res = buf1Res;
                                        buf1Res = x;
                                        x = 0.75f * xp + x * v;
                                        xp = x;
                                        // Anticlick function
                                        x *= CutLevel ( Mathf.Sin ( PI * s / sl ) * 5, 1 );
                                        buffer[b++] += x;
                                        buffer[b++] += x;
                                }
                        }
                               
                        // Overlap neighbour phonemes
                        buf += ((3*sl/4)<<1);
                        if ( p.shape.plosive != 0 )
                                buf += (sl&0xfffffe);
                }
        }
       
        // Creates large buffer and fills it with samples. Starts playing generated sound.
        // Performs no error checking and does not dispose resources.
        void CreateAndPlaySynth()
        {
                float[] data = new float[PLAY_TIME*SAMPLE_FREQUENCY*2];
               
               
                SynthSpeech ( data, 0,  "stap  put daun jo wEpn ju hEv twenti sekondz tu komplaj     start distrakSn sikuens" );
                SynthSpeechComplex ( data, 14*SAMPLE_FREQUENCY*2,      
                                                                "welkam  tu saund test",
                                                                " \x25        \x10          ",
                                                                " \x30          \x30        ",
                                                                false,
                                                                220 );
                SynthSpeechComplex ( data, 20*SAMPLE_FREQUENCY*2,      
                                                                "ridej ij omu ir jviioumeZ",
                                                                "                         ",
                                                                "                         ",
                                                                false,
                                                                20 );
                SynthSpeechComplex ( data, 26*SAMPLE_FREQUENCY*2,      
                                                                "tis iz E wispE",
                                                                "22222222222222",
                                                                "              ",
                                                                true,
                                                                0 );
                for(int i=0;i<data.Length;i++)
                {
                        data[i]*=MASTER_VOLUME;
                }
                AudioClip ac = AudioClip.Create("clip",data.Length,2,SAMPLE_FREQUENCY,false);
                ac.SetData(data,0);
                AudioSource asource = GetComponent<AudioSource>();
                asource.clip = ac;
                asource.Play();        
        }
               
               
        /* END PASTE */
       
       
        void Awake()
        {
                SAMPLE_FREQUENCY=AudioSettings.outputSampleRate;       
                CreateAndPlaySynth();
        }
       
        void OnAudioFilterRead (float[] data, int channels)
        {
                for (int i=0;i<data.Length;i = i+channels)
                {
                        float val =0;
                       
                        //adding to overlay on top of existing sounds if there are any                 
                        data[i]+=val;                  
                       
                        if (channels == 2)
                                data [i+1]+=val;
                }
               
        }
       
 
}