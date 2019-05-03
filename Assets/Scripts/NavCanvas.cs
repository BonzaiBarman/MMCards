using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class NavCanvas : MonoBehaviour
{
    
	[SerializeField] Canvas cGameHud;
	[SerializeField] Canvas cMovieHud;
	[SerializeField] Canvas cMenuHud;
	[SerializeField] Canvas cStartGameHud;
	[SerializeField] Canvas cEndGameHud;
	
	// Start is called before the first frame update
    void Start()
    {
	    var daCanvasList = FindObjectsOfType<Canvas>();
	    foreach(Canvas cnv in daCanvasList)
	    {
	    	switch(cnv.name)
	    	{
	    	case "GameHud":
	    		cGameHud = cnv;
		    	break;
	    	case "MovieHud":
		    	cMovieHud = cnv;
		    	break;
	    	case "MenuHud":
		    	cMenuHud = cnv;
		    	break;
	    	case "StartGameHud":
		    	cStartGameHud = cnv;
		    	break;
	    	case "EndGameHud":
		    	cEndGameHud = cnv;
		    	break;
	    	}
	    }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	public void InitHub()
	{
		cStartGameHud.gameObject.SetActive(true);
		cGameHud.gameObject.SetActive(false);
		cMovieHud.gameObject.SetActive(false);
		cMenuHud.gameObject.SetActive(false);
		cEndGameHud.gameObject.SetActive(false);
	}
	
	public void StartGame()
	{
		cStartGameHud.gameObject.SetActive(false);
		cGameHud.gameObject.SetActive(true);
		cMovieHud.enabled = true;
	}
	
	public void SetTicker(string inText)
	{
		cGameHud.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = inText;
	}
	
	public void SetMovieButton(bool inEnable)
	{
		if(inEnable)
		{
			cGameHud.transform.GetChild(1).gameObject.SetActive(true);
		}
		else
		{
			cGameHud.transform.GetChild(1).gameObject.SetActive(false);
		}
	}
	
	public void TurnOnActorLabel(int inActor, bool inOn)
	{
		switch(inActor)
		{
		case 1:
			cMovieHud.transform.GetChild(8).gameObject.SetActive(inOn);
			cMovieHud.transform.GetChild(9).gameObject.SetActive(inOn);
			break;
		case 2:
			cMovieHud.transform.GetChild(10).gameObject.SetActive(inOn);
			cMovieHud.transform.GetChild(11).gameObject.SetActive(inOn);
			break;
		case 3:
			cMovieHud.transform.GetChild(12).gameObject.SetActive(inOn);
			cMovieHud.transform.GetChild(13).gameObject.SetActive(inOn);
			break;
		}
	}
	
	public void SetMakeMovie(string inPlayerName, string inMovieTitle, int inMovieValue)
	{
		cMovieHud.gameObject.SetActive(true);
		cMovieHud.enabled = true;
		cGameHud.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inPlayerName + " makes a movie '" + inMovieTitle + "'";
		cMovieHud.transform.GetChild(15).GetComponent<TextMeshProUGUI>().text = inMovieTitle + " (" + inMovieValue + ")";
	}
	
	public void DisableMovieHud()
	{
		cMovieHud.enabled = false;
	}

	public void UpdateMovieScore(string inMovieTitle, int inMovieValue)
	{
		cMovieHud.transform.GetChild(15).GetComponent<TextMeshProUGUI>().text = inMovieTitle + " (" + inMovieValue + ")";
	}

}
