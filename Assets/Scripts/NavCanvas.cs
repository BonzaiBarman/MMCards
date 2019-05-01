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

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	//public void SetKin()
	//{
	//	Card[] daCards = FindObjectsOfType<Card>();
	//	foreach (Card item in daCards)
	//	{
	//		if (item.cardData.type == CardData.CardType.Action)
	//		{
	//			//count = count + 0.03f;
	//			item.GetComponent<Rigidbody>().isKinematic = true;
	//		}
	//	}
	//}
    
	public void Navigate(string inCommand)
	{

		//switch (inCommand)
		//{
		//	case "Close1_Open2":

		//		cHud.enabled = false;
		//		cHud.transform.DOScale(new Vector3(0,0,0),0);
		//		cInfo.enabled = true;
		//		cInfo.transform.DOScale(new Vector3(1,1, 1), 1.5f);

		//		break;
		//	case "Open1_Close2":
		//		cInfo.enabled = false;
		//		cInfo.transform.DOScale(new Vector3(0,0,0),0);
		//		cHud.enabled = true;
		//		cHud.transform.DOScale(new Vector3(1,1, 1), 1.5f);
		//		break;
		//}
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
