public var maxsamples:int=10;
public var maxintervals:int=5;

public var mininterval:float=0.1;
public var maxinterval:float=5;

public var maxhistorylength:int=500;

public var stagelengthbase=10;//takes 10 sounds for level 1
public var stagelengthmodifier=4;//each level is 4 times lenger than the prior

private var clip:Array;
private var stage:int;

private var history:int;
/*
function GenerateMarkovChain(history:Array,level:int):Array
{
	var MC:Object={};
	var intervalcount:float=history.length-(level-1);
	var cur:Array=[0];//temp value, will be discarded
	//populate cur 
	for (var i:int=0;i<level-1;i++)
	{
		cur.Add(history[i]);
	}
	for (i=0;i<history.length;i++)
	{
		cur.Shift();
		cur.Push(history[i]);
		if (MC[cur]==null)
		{
			MC[cur]=0;
		}
		
		MC[cur]+=1.0f/intervalcount;
		
	}
	
}*/
function Start()
{
	clip = new Array();

   var obs : Object[] = Resources.LoadAll("Samples");
    for (var i:int=0; i < obs.length; i++)
    {
    	var ac:AudioClip = obs[i] as AudioClip;
    	if (ac!=null)
    	{
        	clip.Add(obs[i]);
        }
    }

	PlayMusic();
}

public var min:float=0.5;
public var max:float=5;
// This JavaScript function can be called by an Animation Event
function PlayMusic() {
	while(true)
	{
		yield WaitForSeconds(Random.Range(mininterval,maxinterval));
		var ac:AudioClip = clip[Random.Range(0,clip.length)];
    	Debug.Log("playing " + ac.name);
		GetComponent(AudioSource).PlayOneShot(ac);
	}
}
