using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using Unity.IO;

public class Card : MonoBehaviour
{

	public CardData cardData;
	public int nextHandIdx = 0;
	GameControl gControl;

	Vector3[,] plyrHandLocs = new [,] {{new Vector3(-2.1f, 2f, -3.2f), new Vector3(-0.8f, 2f, -3.2f), new Vector3(0.5f, 2f, -3.2f), new Vector3(1.8f, 2f, -3.2f), new Vector3(3.1f, 2f, -3.2f), new Vector3(4.4f, 2f, -3.2f), new Vector3(5.7f, 2f, -3.2f)},
	{new Vector3(-3f, 0.01f, 2.4f), new Vector3(-3f, 0.011f, 1.7f), new Vector3(-3f, 0.012f, 1f), new Vector3(-3f, 0.013f, 0.3f), new Vector3(-3f, 0.014f, -0.4f), new Vector3(-3f, 0.015f, -1.1f), new Vector3(-3f, 0.016f, -1.8f)},
	{new Vector3(4.7f, 0.01f, 3.7f), new Vector3(4f, 0.011f, 3.7f), new Vector3(3.3f, 0.012f, 3.7f), new Vector3(2.6f, 0.013f, 3.7f), new Vector3(1.9f, 0.014f, 3.7f), new Vector3(1.2f, 0.015f, 3.7f), new Vector3(0.5f, 0.016f, 3.7f)},
	{new Vector3(7.3f, 0.01f, -1.8f), new Vector3(7.3f, 0.011f, -1.1f), new Vector3(7.3f, 0.012f, -0.4f), new Vector3(7.3f, 0.013f, 0.3f), new Vector3(7.3f, 0.014f, 1f), new Vector3(7.3f, 0.015f, 1.7f), new Vector3(7.3f, 0.016f, 2.4f)}};
	Vector3[] plyrDealLocs = new [] {new Vector3(1.8f, 2f, -3.2f), new Vector3(-3f, 0.1f, 0.8f), new Vector3(3.1f, 0.1f, 3.7f), new Vector3(7.3f, 0.1f, 0.8f)};
	Vector3[] plyrHandRots = new [] {new Vector3(350f, 0f, 0f), new Vector3(0f, 90f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0f, 270f, 0f)};
	Vector3[] plyrDrawLocs = new [] {new Vector3(1.5f, 3f, -1.5f), new Vector3(-1.5f, 0.1f, 1.8f), new Vector3(3.1f, 0.1f, 2.2f), new Vector3(5.8f, 0.1f, 1.8f)};
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

	public void OnMouseDown()
    {

	    //Debug.Log("CardID: " + cardData.cardID + "Name: " + cardData.cardName);
	    if (cardData.status == CardData.Status.Deck)
	    {
		    if(gControl.curPlayer == gControl.thePlayerIndex && gControl.dealing == false)
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
	    }
	    else if (cardData.status == CardData.Status.Hand)
	    {
		    if(gControl.curPlayer == gControl.thePlayerIndex && gControl.GetCurPlayerAction() == PlayerAction.MakeMovie)
		    {
		    	gControl.MovieCardSelected(this);
		    }
		    else
		    {
			    if(gControl.curPlayer == gControl.thePlayerIndex && gControl.dealing == false)
			    {
			    	if((cardData.hand == gControl.thePlayerIndex && gControl.GetCurPlayerAction() == PlayerAction.DrawTalentDiscard) || gControl.GetCurPlayerAction() == PlayerAction.RaidingTalent || gControl.GetCurPlayerAction() == PlayerAction.TradingTalent)
			    	{				    	
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
			//temp while opp cards are face up
			Vector3 v = transform.position;
			transform.DOMove(new Vector3(v.x, v.y + 1, v.z), 0.2f);
			transform.DORotate(new Vector3(0f, 0f, 0f), 0.5f);
			//--
			
			transform.DOMove(plyrDrawLocs[inPlayerIdx], 0.5f);
			GetComponent<Rigidbody>().AddRelativeForce(0, Random.Range(-10,10), Random.Range(-100,100), ForceMode.Impulse);
			GetComponent<Rigidbody>().AddRelativeTorque(Random.Range(-10,10), Random.Range(-100,100), Random.Range(-10,10), ForceMode.Impulse);
		}
		yield return new WaitForSeconds(0.5f);
	}
	
	IEnumerator DealTalentCardAnim(int inPlayerIdx = 0)
	{
		Vector3 orig = transform.rotation.eulerAngles;
		if (inPlayerIdx == gControl.thePlayerIndex)
		{
			transform.DOMove(plyrHandLocs[inPlayerIdx, cardData.handIdx], 0.5f);
			transform.DORotate(new Vector3(350f, 0f, 0f), 0.6f);				
		}
		else
		{
			//temp
			transform.DORotate(new Vector3(0f, 0f, 0f), 0f);
			//--
			
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
		
		transform.DORotate(new Vector3(0f, 0f, 0f), 0);
		if(gControl.curPlayer == gControl.thePlayerIndex)
		{
			Vector3 origPos = transform.position;
			transform.DOMove(new Vector3(origPos.x, origPos.y + 1f, origPos.z), 0.3f);	
		}
		else
		{
			Vector3 origPos = transform.position;
			transform.DOMove(new Vector3(origPos.x, origPos.y + 3f, origPos.z), 0.3f);	
		}
		GetComponent<Rigidbody>().AddRelativeTorque(Random.Range(-5,5), Random.Range(-20,20), Random.Range(-5,5), ForceMode.Impulse);
		GetComponent<Rigidbody>().AddRelativeForce(0, Random.Range(-10,10), Random.Range(-30,30), ForceMode.Impulse);
		yield return new WaitForSeconds(0.2f);
		float yValue;
		yValue = 0.1f + (cardData.discardIdx / 100f);
		//Debug.Log("yValue: " + cardData.discardIdx);
		transform.DOMove(new Vector3(4.7f, yValue, 0f), 0.5f);
		yield return new WaitForSeconds(1f);
		GetComponent<Rigidbody>().isKinematic = true;
		//transform.DORotate(new Vector3(0f, 0f, 0f), 1);

		yield return new WaitForSeconds(0.8f);
		gControl.SortTalentDiscard();
		
	}
	
	public  void DealCardAnim(int inPlayerIdx, int inHandIdx)
	{
		//transform.DOMove(plyrHandLocs[inPlayerIdx, inHandIdx], 0.1f);
		cardData.handIdx = inHandIdx;
		//Vector3 v = transform.position;
		//transform.DOMove(new Vector3 (v.x - 2, 2f, v.z), 0f);
		transform.DOMoveY(3f, 0.1f);
		StartCoroutine("DealTalentCardAnim", inPlayerIdx);
	}
	
	public  void DrawCardAnim(int inPlayerIdx, int inHandIdx)
	{
		cardData.handIdx = inHandIdx;
		transform.DOMoveY(3f, 0.1f);
		StartCoroutine("DrawTalentCardAnim", inPlayerIdx);
	}
	
	
	public void DiscardTalentCard()
	{
		transform.DOMoveY(3f, 0.1f);
		StartCoroutine("DiscardTalentCardAnim");
	}
	
	public void DiscardMovieTalentCard()
	{
		transform.DOMoveY(3f, 0.1f);
		float yValue;
		yValue = 0.1f + (cardData.discardIdx / 100f);
		transform.DOMove(new Vector3(4.7f, yValue, 0f), 0.5f);

	}
	
	public void MoveCard(int inPlayer, int inHandIdx)
	{
		//Debug.Log(inPlayer + " " + inHandIdx);
		Vector3 v = plyrHandLocs[inPlayer, inHandIdx];
		transform.DOMove(new Vector3(v.x, (float)(v.y + (inHandIdx * 0.01)), v.z), 0.1f);
		RotateCard(inPlayer);
	}
	
	public void RotateCard(int inPlayer)
	{
		if(inPlayer == gControl.thePlayerIndex)
		{
			Vector3 v = transform.rotation.eulerAngles;
			transform.DORotate(new Vector3(v.x, plyrHandRots[inPlayer].y, v.z), 0.1f);
		}
		else
		{
			transform.DORotate(new Vector3(0, plyrHandRots[inPlayer].y, 0), 0.1f);			
		}

	}
}
