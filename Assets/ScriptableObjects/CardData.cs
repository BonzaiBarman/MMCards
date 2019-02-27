using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Custom/CardData")]
public class CardData : ScriptableObject
{
	public enum SubType
	{
		Collect,
		Raid,
		Sabotage,
		Trade,
		Chaos,
		RunOver,
		Actor,
		Actress,
		Director,
		Music,
		Screenplay,
		RaidProtection,
		SabotageProtection
	}
	
	public enum Status
	{
		Deck,
		Hand,
		Movie,
		Discard
	}
	
	public enum CardType
	{
		Action,
		Talent,
	}
	
	public int cardID;
	public string cardName;
	public CardType type;
	public SubType subType;
	public Status status;
	public Sprite frontSprite;
	public Sprite backSprite;
	public int[] value = new int[6];
	public int hand = -1;
	public int handIdx = -1;
	public CardType deck;
	public int deckIdx;
	public CardType discard;
	public int discardIdx = -1;
	public int movie = -1;
	public int movieIdx = -1;
	

}
