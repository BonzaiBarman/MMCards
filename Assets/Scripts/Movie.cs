using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Movie : MonoBehaviour
{
	GameControl gControl;
    
	public int screenplayID = -1;
	public int directorID = -1;
	public int musicID = -1;
	public int[] actorID = new[] {-1, -1, -1, -1};
    public string title = "";

	
	// Start is called before the first frame update
    void Start()
    {
    	gControl = FindObjectOfType<GameControl>();
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
		int daValue = 0;
		int spIndex = gControl.GetTalentCardFromID(screenplayID).cardData.value[0];
		daValue += gControl.GetTalentCardFromID(directorID).cardData.value[spIndex];
		daValue += gControl.GetTalentCardFromID(musicID).cardData.value[spIndex];
		for(int i = 0; i < actorID.Length; i++)
		{
			if(actorID[i] != -1){daValue += gControl.GetTalentCardFromID(actorID[i]).cardData.value[spIndex];}
		}
		return daValue;
	}
	
	public void SetTitle(string inTitle)
	{
		title = inTitle;
		transform.GetChild(0).GetComponent<TextMeshPro>().text = title + " (" + value() + ")";
	}
	
}
