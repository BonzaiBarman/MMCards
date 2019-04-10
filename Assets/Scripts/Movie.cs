using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movie : MonoBehaviour
{
    
	public int screenplayID;
	public int directorID;
	public int musicID;
	public int[] actorID = new int[4];
	public string title;
	// Start is called before the first frame update
    void Start()
    {
    	InitMovie();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	private void InitMovie()
	{
		screenplayID = -1;
		directorID = -1;
		musicID = -1;
		actorID[0] = -1;
		actorID[1] = -1;
		actorID[2] = -1;
		actorID[3] = -1;
		title = "";
	}
	
	//public int value()
	//{
		
	//}
	
}
