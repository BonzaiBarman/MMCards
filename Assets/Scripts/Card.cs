using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using Unity.IO;

public class Card : MonoBehaviour
{

	//Card Data Scriptable Object 
	public CardData cardData;
	
	//Ref to GameControl
	GameControl gControl;

	//Card Physical Location Vectors
	Vector3[,] plyrHandLocs = new [,] {{new Vector3(-2.1f, 2f, -3.2f), new Vector3(-0.8f, 2f, -3.2f), new Vector3(0.5f, 2f, -3.2f), new Vector3(1.8f, 2f, -3.2f), new Vector3(3.1f, 2f, -3.2f), new Vector3(4.4f, 2f, -3.2f), new Vector3(5.7f, 2f, -3.2f)},
	{new Vector3(-3f, 0.01f, 2.4f), new Vector3(-3f, 0.011f, 1.7f), new Vector3(-3f, 0.012f, 1f), new Vector3(-3f, 0.013f, 0.3f), new Vector3(-3f, 0.014f, -0.4f), new Vector3(-3f, 0.015f, -1.1f), new Vector3(-3f, 0.016f, -1.8f)},
	{new Vector3(4.7f, 0.01f, 3.7f), new Vector3(4f, 0.011f, 3.7f), new Vector3(3.3f, 0.012f, 3.7f), new Vector3(2.6f, 0.013f, 3.7f), new Vector3(1.9f, 0.014f, 3.7f), new Vector3(1.2f, 0.015f, 3.7f), new Vector3(0.5f, 0.016f, 3.7f)},
	{new Vector3(7.3f, 0.01f, -1.8f), new Vector3(7.3f, 0.011f, -1.1f), new Vector3(7.3f, 0.012f, -0.4f), new Vector3(7.3f, 0.013f, 0.3f), new Vector3(7.3f, 0.014f, 1f), new Vector3(7.3f, 0.015f, 1.7f), new Vector3(7.3f, 0.016f, 2.4f)}};
	Vector3[] plyrDealLocs = new [] {new Vector3(1.8f, 2f, -3.2f), new Vector3(-3f, 0.1f, 0.8f), new Vector3(3.1f, 0.1f, 3.7f), new Vector3(7.3f, 0.1f, 0.8f)};
	Vector3[] plyrHandRots = new [] {new Vector3(350f, 0f, 0f), new Vector3(0f, 90f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0f, 270f, 0f)};
	Vector3[] plyrDrawLocs = new [] {new Vector3(1.5f, 3f, -1.5f), new Vector3(-1.5f, 0.1f, 1.8f), new Vector3(3.1f, 0.1f, 2.2f), new Vector3(5.8f, 0.1f, 1.8f)};
	Vector3 playerActionLoc = new Vector3(1.5f, 3f, -1.5f);
	//-----------------------------
	
	//Misc Variables
	Rigidbody rBod;
	const int CardDrawnHandIDX = 99;

	// Start is called before the first frame update
    void Start()
    {
	    gControl = FindObjectOfType<GameControl>();
	    rBod = GetComponent<Rigidbody>();
	}

    // Update is called once per frame
    void Update()
    {

	}

	public void OnMouseDown()
    {

	    if (cardData.status == CardData.Status.Deck)
	    {
		    if(gControl.curPlayer == gControl.thePlayerIndex && gControl.dealing == false)
		    {
			    gControl.CardDraw(this);
			    if (cardData.type == CardData.CardType.Action)
			    {
				    StartCoroutine("DrawActionCardAnim");
				    //rBod.isKinematic = true;
			    }	    
			    if (cardData.type == CardData.CardType.Talent)
			    {
				    StartCoroutine("DrawTalentCardAnim", gControl.thePlayerIndex);
				    //rBod.isKinematic = true;
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
					    	//rBod.isKinematic = false;
					    	StartCoroutine("DiscardActionCardAnim");
				    	}	    
				    	if (cardData.type == CardData.CardType.Talent)
				    	{
					    	//rBod.isKinematic = false;
					    	StartCoroutine("DiscardTalentCardAnim");
				    	}		    		
			    	}
			    }
		    }

		}
	    else if (cardData.status == CardData.Status.Movie)
	    {
		    gControl.MovieCardSelected(this);
	    }
    }
	
	IEnumerator DrawActionCardAnim()
	{
		transform.DOMove(playerActionLoc, 1); //x was 1.8
		transform.DORotate(new Vector3(350f, 0f, 0f), 1);
		yield return new WaitForSeconds(1f);
		rBod.isKinematic = true;
	}
	
	public IEnumerator DrawTalentCardAnim(int inPlayerIdx = 0)
	{
		Vector3 orig = transform.rotation.eulerAngles;
		if (inPlayerIdx == gControl.thePlayerIndex)
		{
			if(cardData.handIdx == CardDrawnHandIDX)
			{
				//Put in front of hand (when hand is full)
				transform.DOMove(playerActionLoc, 0.5f);
				transform.DORotate(new Vector3(350f, 0f, 0f), 0.6f);				
			}
			else
			{
				//Put in hand
				transform.DOMove(plyrHandLocs[inPlayerIdx, cardData.handIdx], 0.5f);
				transform.DORotate(new Vector3(350f, 0f, 0f), 0.6f);				
			}
		}
		else
		{
			//temp while opp cards are face up
			//transform.DOMoveY(8f, 1f);
			Vector3 v = transform.position;
			transform.DOMove(new Vector3(v.x, v.y + 7, v.z), 0.4f);
			transform.DORotate(new Vector3(0f, 0f, 0f), 0.5f);
			//--
			
			//Send Drawn card to card draw location for computer player
			transform.DOMove(plyrDrawLocs[inPlayerIdx], 0.5f);
			rBod.AddRelativeForce(0, Random.Range(-10,10), Random.Range(-100,100), ForceMode.Impulse);
			rBod.AddRelativeTorque(Random.Range(-10,10), Random.Range(-100,100), Random.Range(-10,10), ForceMode.Impulse);
		}
		yield return new WaitForSeconds(0.5f);
		rBod.isKinematic = true;
		yield return null;
	}
	
	IEnumerator DealTalentCardAnim(int inPlayerIdx = 0)
	{
		Vector3 orig = transform.rotation.eulerAngles;
		if (inPlayerIdx == gControl.thePlayerIndex)
		{
			//deal card to player hand
			transform.DOMove(plyrHandLocs[inPlayerIdx, cardData.handIdx], 0.5f);
			transform.DORotate(new Vector3(350f, 0f, 0f), 0.6f);				
		}
		else
		{
			//temp
			transform.DORotate(new Vector3(0f, 0f, 0f), 0f);
			//--
			
			//deal card to player deal loc
			transform.DOMove(plyrDealLocs[inPlayerIdx], 0.5f);				
			rBod.AddRelativeForce(0, Random.Range(-10,10), Random.Range(-100,100), ForceMode.Impulse);
			rBod.AddRelativeTorque(Random.Range(-10,10), Random.Range(-100,100), Random.Range(-10,10), ForceMode.Impulse);
		}
		yield return new WaitForSeconds(0.5f);
	}
	
	public IEnumerator DiscardActionCardAnim()
	{
		//for human only
		//need to do for computer
		
		transform.DOMoveY(3f, 0.1f);
		rBod.isKinematic = false;
		rBod.AddRelativeTorque(Random.Range(-5,5), Random.Range(-20,20), Random.Range(-5,5), ForceMode.Impulse);
		rBod.AddRelativeForce(0, Random.Range(-10,10), Random.Range(-30,30), ForceMode.Impulse);
		transform.DOMove(new Vector3(0.8f, 0.1f, 0f), 1);

		yield return new WaitForSeconds(1f);
	}
	
	public IEnumerator DiscardTalentCardAnim()
	{
		transform.DOMoveY(3f, 0.1f);
		rBod.isKinematic = false;
		transform.DORotate(new Vector3(0f, 0f, 0f), 0);
		Vector3 origPos = transform.position;
		if(gControl.curPlayer == gControl.thePlayerIndex)
		{
			transform.DOMove(new Vector3(origPos.x, origPos.y + 1f, origPos.z), 0.3f);	
		}
		else
		{
			transform.DOMove(new Vector3(origPos.x, origPos.y + 3f, origPos.z), 0.3f);	
		}
		rBod.AddRelativeTorque(Random.Range(-5,5), Random.Range(-20,20), Random.Range(-5,5), ForceMode.Impulse);
		rBod.AddRelativeForce(0, Random.Range(-10,10), Random.Range(-30,30), ForceMode.Impulse);
		yield return new WaitForSeconds(0.3f);
		float yValue;
		yValue = 0.1f + (cardData.discardIdx / 100f);
		transform.DOMove(new Vector3(4.7f, yValue, 0f), 0.5f);
		yield return new WaitForSeconds(0.8f);
		rBod.isKinematic = true;

		yield return new WaitForSeconds(0.6f);
		gControl.SortTalentDiscard();
		
	}
	
	public  void DealCardAnim(int inPlayerIdx, int inHandIdx)
	{
		cardData.handIdx = inHandIdx;
		transform.DOMoveY(3f, 0.1f);
		StartCoroutine("DealTalentCardAnim", inPlayerIdx);
	}

	public void MoveCard(int inPlayer, int inHandIdx)
	{
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
