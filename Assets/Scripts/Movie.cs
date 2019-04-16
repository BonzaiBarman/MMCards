using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Movie : MonoBehaviour
{
	GameControl gControl;
	
	public enum ScreenplayIndex
	{
		Comedy,
		Drama,
		Horror,
		Musical,
		Western,
		Action
	}
    
	public int screenplayID = -1;
	public int directorID = -1;
	public int musicID = -1;
	public int[] actorID = new[] {-1, -1, -1, -1};
    public string title = "";
	int spIndex = -1;
	
	// Start is called before the first frame update
    void Start()
    {
    	gControl = FindObjectOfType<GameControl>();
    	//InitMovie();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	public void InitMovie()
	{
		
		screenplayID = -1;
		directorID = -1;
		musicID = -1;
		actorID[0] = -1;
		actorID[1] = -1;
		actorID[2] = -1;
		actorID[3] = -1;
		title = "";
		gControl = FindObjectOfType<GameControl>();
	}
	
	public int value()
	{
		
		//gControl = FindObjectOfType<GameControl>();
		int daValue = 0;
		spIndex = GetScreenplayIndex();
		daValue += gControl.GetTalentCardFromID(directorID).cardData.value[0];
		daValue += gControl.GetTalentCardFromID(musicID).cardData.value[0];
		for(int i = 0; i < actorID.Length; i++)
		{
			if(actorID[i] != -1){daValue += gControl.GetTalentCardFromID(actorID[i]).cardData.value[spIndex];}
		}
		return daValue;
	}
	
	private int GetScreenplayIndex()
	{
		//gControl = FindObjectOfType<GameControl>();
		int retIdx = -1;
		switch(gControl.GetTalentCardFromID(screenplayID).cardData.cardName)
		{
		case "Comedy":
			retIdx = 0;
			break;
		case "Drama":
			retIdx = 1;
			break;
		case "Horror":
			retIdx = 2;
			break;
		case "Musical":
			retIdx = 3;
			break;
		case "Western":
			retIdx = 4;
			break;
		case "Action":
			retIdx = 5;
			break;
		}
		return retIdx;
	}
	
	public void SetTitle(string inTitle)
	{
		title = inTitle;
		transform.GetChild(0).GetComponent<TextMeshPro>().text = title + " (" + value() + ")";
	}
	
}
