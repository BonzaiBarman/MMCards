using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Linq;
//using DG.Tweening;

public enum PlayerAction
{
    DrawTalent,
    DrawTalentDiscard,
    DrawActionCollect,
    DrawActionRaid,
    DrawActionSabotage,
    DrawActionTrade,
    DrawActionChaos,
    DrawActionRunOver,
    TradingTalent,
    RaidingTalent,
    SabotagingMovie,
    MakeMovie
}

public enum Screenplays
{
    Comedy,
    Drama,
    Horror,
    Musical,
    Western,
    Action
}



public class Utilities
{

	public void ShuffleCards(Card[] inCards)
	{
		var cnt = inCards.Length;
		var last = cnt -1;
		for (var i = 0; i < last; i++)
		{
			var r = UnityEngine.Random.Range(i, cnt);
			var tmp = inCards[i];
			inCards[i] = inCards[r];
			inCards[i].cardData.deckIdx = i;
			inCards[r] = tmp;
			inCards[r].cardData.deckIdx = r;
		}
	}
	public void ShuffleCards(int[] inCards)
	{
		var cnt = inCards.Length;
		var last = cnt -1;
		for (var i = 0; i < last; i++)
		{
			var r = UnityEngine.Random.Range(i, cnt);
			var tmp = inCards[i];
			inCards[i] = inCards[r];
			inCards[r] = tmp;
		}
	}
	
	//public void DoReShuffleCards(Card[] inCards)
	//{
	//	StartCoroutine("ReShuffleCards", inCards);
	//}
	
	//IEnumerator ReShuffleCards(Card[] inCards)
	//{
	//	int cnt = 0;
	//	foreach(Card crd in inCards)
	//	{
	//		if (crd.cardData.status	== CardData.Status.Discard)
	//		{
	//			crd.cardData.status = CardData.Status.Deck;
	//			cnt += 1;
	//		}
	//	}
	//	int[] newDeck = new int[cnt + 1];
	//	cnt = 0;
	//	foreach(Card crd in inCards)
	//	{
	//		if (crd.cardData.status	== CardData.Status.Deck)
	//		{
	//			newDeck[cnt] = crd.cardData.cardID;
	//			cnt += 1;
	//		}
	//	}
	//	ShuffleCards(newDeck);
	//	cnt =0;
	//	foreach (int item in newDeck)
	//	{
	//		inCards[item].cardData.deckIdx = cnt;
	//		cnt += 1;
	//	}
	//	//Sort talent cards by shuffle index
	//	inCards = inCards.OrderByDescending(go => go.cardData.deckIdx).ToArray();
	//	//move cards to the height to drop from
	//	float loc = 2f;
	//	foreach (Card item in inCards)
	//	{
	//		if (item.cardData.status ==	CardData.Status.Deck)
	//		{
	//			item.GetComponent<Rigidbody>().isKinematic = true;
	//			loc = loc + 0.1f;
	//			item.transform.DOMove(new Vector3(-0.5f,loc,0f), 0f);
	//			item.transform.DORotate(new Vector3(180f,180f,0f), 0f);
	//			//item.transform.DOMoveY(loc,0f);
	//		}
	//	}
	//	//turn off Kinematic to activate gravity
	//	foreach (Card item in inCards)
	//	{
	//		if (item.cardData.status ==	CardData.Status.Deck)
	//		{
	//			item.GetComponent<Rigidbody>().isKinematic = false;
	//		}
	//	}
	//	//wait for 2 seconds. cards should have fallen by then
	//	yield return new WaitForSeconds(2f);
	//	//stop gravity on cards
	//	foreach (Card item in inCards)
	//	{
	//		item.GetComponent<Rigidbody>().isKinematic = true;
	//	}
	//	//move cards in both decks physically to match order in deck
	//	float nvalue = 0.02f;
	//	foreach (Card item in inCards)
	//	{
	//		if (item.cardData.status ==	CardData.Status.Deck)
	//		{
	//			item.transform.DOMoveY(nvalue,0);
	//			nvalue += .005f;				
	//		}
	//	}
	//}
}
