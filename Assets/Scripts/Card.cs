using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class Card : MonoBehaviour
{

	public CardData cardData;
	public int nextHandIdx = 0;
	GameControl gControl;

	Vector3[,] plyrHandLocs = new [,] {{new Vector3(-2.1f, 2f, -3.2f), new Vector3(-0.8f, 2f, -3.2f), new Vector3(0.5f, 2f, -3.2f), new Vector3(1.8f, 2f, -3.2f), new Vector3(3.1f, 2f, -3.2f), new Vector3(4.4f, 2f, -3.2f), new Vector3(5.7f, 2f, -3.2f)},
	{new Vector3(-3f, 0.1f, 2.9f), new Vector3(-3f, 0.1f, 2.2f), new Vector3(-3f, 0.1f, 1.5f), new Vector3(-3f, 0.1f, 0.8f), new Vector3(-3f, 0.1f, 0.1f), new Vector3(-3f, 0.1f, -0.6f), new Vector3(-3f, 0.1f, -1.3f)},
	{new Vector3(5.2f, 0.1f, 3.7f), new Vector3(4.5f, 0.1f, 3.7f), new Vector3(3.8f, 0.1f, 3.7f), new Vector3(3.1f, 0.1f, 3.7f), new Vector3(2.4f, 0.1f, 3.7f), new Vector3(1.7f, 0.1f, 3.7f), new Vector3(1f, 0.1f, 3.7f)},
	{new Vector3(7.3f, 0.1f, -1.3f), new Vector3(7.3f, 0.1f, -0.6f), new Vector3(7.3f, 0.1f, 0.1f), new Vector3(7.3f, 0.1f, 0.8f), new Vector3(7.3f, 0.1f, 1.5f), new Vector3(7.3f, 0.1f, 2.2f), new Vector3(7.3f, 0.1f, 2.9f)}};
	Vector3[] plyrDealLocs = new [] {new Vector3(1.8f, 2f, -3.2f), new Vector3(-3f, 0.1f, 0.8f), new Vector3(3.1f, 0.1f, 3.7f), new Vector3(7.3f, 0.1f, 0.8f)};
	Vector3[] plyrHandRots = new [] {new Vector3(350f, 0f, 0f), new Vector3(0f, 90f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0f, 270f, 0f)};

	Vector3 playerActionLoc = new Vector3(1.5f, 3f, -1.5f);

	// Start is called before the first frame update
    void Start()
    {
	    //cardData = Resources.Load<CardData>("CardSOs/Card1");
	    //transform.GetChild(0).GetComponent<Renderer>().material = cardData.frontMat;
	    //transform.GetChild(0).GetComponent<Renderer>().material = Resources.Load<Material>("TalentDeck/Materials/Talent4");
	    gControl = FindObjectOfType<GameControl>();
	}

    // Update is called once per frame
    void Update()
    {

	}

	void OnMouseDown()
    {

	    //Debug.Log("CardID: " + cardData.cardID + "Name: " + cardData.cardName);
	    if (cardData.status == CardData.Status.Deck)
	    {
		    //Debug.Log(cardData.cardName + ", " + cardData.deckIdx);
		    gControl.CardDraw(this);
		    if (cardData.type == CardData.CardType.Action)
		    {
			    StartCoroutine("DrawActionCardAnim");
			    GetComponent<Rigidbody>().isKinematic = true;
		    }	    
		    if (cardData.type == CardData.CardType.Talent)
		    {
			    StartCoroutine("DrawTalentCardAnim", gControl.thePlayerIndex);
			    GetComponent<Rigidbody>().isKinematic = true;
		    }	    	
	    }
	    else if (cardData.status == CardData.Status.Hand)
	    {
		    //Debug.Log(cardData.cardName);
		    gControl.CardDiscard(this);
		    if (cardData.type == CardData.CardType.Action)
		    {
			    GetComponent<Rigidbody>().isKinematic = false;
			    StartCoroutine("DiscardActionCardAnim");
			    //GetComponent<Rigidbody>().isKinematic = true;
		    }	    
		    if (cardData.type == CardData.CardType.Talent)
		    {
			    GetComponent<Rigidbody>().isKinematic = false;
			    StartCoroutine("DiscardTalentCardAnim");
			    //GetComponent<Rigidbody>().isKinematic = true;
		    }	    	
	    }

    }
	
	IEnumerator DrawActionCardAnim()
	{
		
		transform.DOMove(playerActionLoc, 1); //x was 1.8
		transform.DORotate(new Vector3(350f, 0f, 0f), 1);
		yield return new WaitForSeconds(1f);
		
	}
	
	IEnumerator DrawTalentCardAnim(int inPlayerIdx = 0)
	{
		
		//also based on cardData.Hand (not implemented) just doing player0
		//Debug.Log("carddata.cardname " + cardData.cardName);
		Vector3 orig = transform.rotation.eulerAngles;
		//transform.DOMove(plyrHandLocs[inPlayerIdx, cardData.handIdx], 0.7f);
		if (inPlayerIdx == gControl.thePlayerIndex)
		{
			if(cardData.handIdx == 99)
			{
				transform.DOMove(playerActionLoc, 0.5f);
				transform.DORotate(new Vector3(350f, 0f, 0f), 0.6f);				
			}
			else
			{
				transform.DOMove(plyrHandLocs[inPlayerIdx, cardData.handIdx], 0.5f);
				transform.DORotate(new Vector3(350f, 0f, 0f), 0.6f);				
			}

		}
		else
		{
			transform.DOMove(plyrDealLocs[inPlayerIdx], 0.5f);
			GetComponent<Rigidbody>().AddRelativeForce(0, Random.Range(-10,10), Random.Range(-100,100), ForceMode.Impulse);
			GetComponent<Rigidbody>().AddRelativeTorque(Random.Range(-10,10), Random.Range(-100,100), Random.Range(-10,10), ForceMode.Impulse);
			
		}



		yield return new WaitForSeconds(0.5f);

	}
	
	
	
	IEnumerator DiscardActionCardAnim()
	{
		
		//GetComponent<Rigidbody>().isKinematic = false;
		GetComponent<Rigidbody>().AddRelativeTorque(Random.Range(-5,5), Random.Range(-20,20), Random.Range(-5,5), ForceMode.Impulse);
		GetComponent<Rigidbody>().AddRelativeForce(0, Random.Range(-10,10), Random.Range(-30,30), ForceMode.Impulse);
		transform.DOMove(new Vector3(0.8f, 0.1f, 0f), 1);
		//transform.DORotate(new Vector3(0f, 0f, 0f), 1);

		yield return new WaitForSeconds(1f);
		//GetComponent<Rigidbody>().isKinematic = true;
		
	}
	IEnumerator DiscardTalentCardAnim()
	{
		
		//also based on cardData.Hand (not implemented) just doing player0
		transform.DORotate(new Vector3(0f, 0f, 0f), 0);		
		GetComponent<Rigidbody>().AddRelativeTorque(Random.Range(-5,5), Random.Range(-20,20), Random.Range(-5,5), ForceMode.Impulse);
		GetComponent<Rigidbody>().AddRelativeForce(0, Random.Range(-10,10), Random.Range(-30,30), ForceMode.Impulse);
		transform.DOMove(new Vector3(4.7f, 0.1f, 0f), 1);
		//transform.DORotate(new Vector3(0f, 0f, 0f), 1);

		yield return new WaitForSeconds(1f);
		
	}
	public  void DealCardAnim(int inPlayerIdx, int inHandIdx)
	{
		//transform.DOMove(plyrHandLocs[inPlayerIdx, inHandIdx], 0.1f);
		cardData.handIdx = inHandIdx;
		//Vector3 v = transform.position;
		//transform.DOMove(new Vector3 (v.x - 2, 2f, v.z), 0f);
		transform.DOMoveY(3f, 0.1f);
		StartCoroutine("DrawTalentCardAnim", inPlayerIdx);
	}
	
	
	
	public void MoveCard(int inPlayer, int inHandIdx)
	{
		Debug.Log(inPlayer + " " + inHandIdx);
		Vector3 v = plyrHandLocs[inPlayer, inHandIdx];
		transform.DOMove(new Vector3(v.x, (float)(v.y + (inHandIdx * 0.01)), v.z), 0.1f);
	}
	public void RotateCard(int inPlayer)
	{
		Vector3 v = transform.rotation.eulerAngles;
		transform.DORotate(new Vector3(v.x, plyrHandRots[inPlayer].y, v.z), 0.1f);
	}
}
