using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;

public class GameControl : MonoBehaviour
{
    
	public enum Screenplays
	{
		Comedy,
		Drama,
		Horror,
		Musical,
		Western,
		Action
	}
	
	Name daname;
	Card[] daCards;
	Card[] actionCards;
	int curActionCardsIdx = 0;
	int curActionDiscardIdx = 0;
	Card[] talentCards;
	int curTalenCardsIdx = 0;
	int curTalentDiscardIdx = 0;
	MovieTitles movieTitles;
	
	Player player;
	const int ActionCardCount = 50;
	const int TalentCardCount = 70;
	
	// Start is called before the first frame update
	void Start()
    {
	    //daname = FindObjectOfType<Name>();
	    //float r = daname.transform.position.x;
	    //daname.ChangeName("Walter");
	    //daname.RunAnimation("rotate");
	    
	    StartCoroutine("InitGame");
	}

    // Update is called once per frame
	void Update()
    {
        
    }

	IEnumerator InitGame()
	{
		//get all the cards
		daCards = FindObjectsOfType<Card>();
		
		InitActionCards();
		InitTalentCards();
		
		InitPlayer();
		
		LoadMovieTitles();
		
		//wait for 2 seconds. cards should have fallen by then
		yield return new WaitForSeconds(2f);
		//stop gravity on cards
		foreach (Card item in daCards)
		{
			item.GetComponent<Rigidbody>().isKinematic = true;
		}
		//move cards in both decks physically to match order in deck
		float nvalue = 0.02f;
		foreach (Card item in actionCards)
		{
			item.transform.DOMoveY(nvalue,0);
			nvalue += .005f;
		}
		nvalue = 0.02f;
		foreach (Card item in talentCards)
		{
			item.transform.DOMoveY(nvalue,0);
			nvalue += .005f;
		}
	}

	void LoadMovieTitles()
	{
		string filePath = Path.Combine(Application.streamingAssetsPath, "MovieTitles.json");
		if (File.Exists(filePath))
		{
			string dataJson = File.ReadAllText(filePath);
			movieTitles = JsonUtility.FromJson<MovieTitles>(dataJson);
		}
	}

	void InitActionCards()
	{
		//Get Action Cards
		int count = 0;
		actionCards = new Card[ActionCardCount];
		foreach (Card item in daCards)
		{
			if (item.cardData.type == CardData.CardType.Action)
			{
				actionCards[count] = item;
				actionCards[count].cardData.status = CardData.Status.Deck;
				actionCards[count].cardData.hand = 0;
				actionCards[count].cardData.handIdx = -1;
				actionCards[count].cardData.deckIdx = actionCards[count].cardData.cardID;
				actionCards[count].cardData.movie = -1;
				actionCards[count].cardData.movieIdx = -1;
				count += 1;
			}
		}

		//shuffle
		ShuffleListCards(actionCards);

		//Sort action cards by shuffle index
		actionCards = actionCards.OrderByDescending(go => go.cardData.deckIdx).ToArray();
		//move cards to the height to drop from
		float loc = 2f;
		foreach (Card item in actionCards)
		{
			loc = loc + 0.1f;
			item.transform.DOMoveY(loc,0);
		}
		//turn off Kinematic to activate gravity
		foreach (Card item in actionCards)
		{
			item.GetComponent<Rigidbody>().isKinematic = false;
		}

	}
	
	void InitTalentCards()
	{
		//Get Talent Cards
		int count = 0;
		talentCards = new Card[TalentCardCount];
		foreach (Card item in daCards)
		{
			if (item.cardData.type == CardData.CardType.Talent)
			{
				talentCards[count] = item;
				talentCards[count].cardData.status = CardData.Status.Deck;
				talentCards[count].cardData.hand = 0;
				talentCards[count].cardData.handIdx = -1;
				talentCards[count].cardData.deckIdx = talentCards[count].cardData.cardID;
				talentCards[count].cardData.movie = -1;
				talentCards[count].cardData.movieIdx = -1;
				count += 1;
			}
		}
		
		//shuffle
		ShuffleListCards(talentCards);
		
		//Sort talent cards by shuffle index
		talentCards = talentCards.OrderByDescending(go => go.cardData.deckIdx).ToArray();
		//move cards to the height to drop from
		float loc = 2f;
		foreach (Card item in talentCards)
		{
			//if (item.cardData.type == CardData.CardType.Action)
			//{
			loc = loc + 0.1f;
			item.transform.DOMoveY(loc,0);
			//}
		}
		//turn off Kinematic to activate gravity
		foreach (Card item in talentCards)
		{
			item.GetComponent<Rigidbody>().isKinematic = false;
		}
		
	}
	
	void ShuffleListCards(Card[] inCards)
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
	
	void InitPlayer()
	{
		player = FindObjectOfType<Player>();
		player.hand = new Card[7];
		player.nextHandIdx = 0;
	}
	
	public void CardDraw(Card inCard)
	{
		if (inCard.cardData.type == CardData.CardType.Talent)
		{
			if (player.nextHandIdx < 7)
			{
				//Debug.Log(inCard.cardData.deckIdx);
				//Debug.Log(player.nextHandIdx);
				player.hand[player.nextHandIdx] = inCard;
				inCard.cardData.hand = 0;
				inCard.cardData.handIdx = player.nextHandIdx;

				inCard.cardData.deckIdx = -1;
				inCard.cardData.status = CardData.Status.Hand;
				player.nextHandIdx += 1;
				curTalenCardsIdx += 1;
				
			}
			else
			{
				Debug.Log("Hand Full");
			}
			
		}
		else if (inCard.cardData.type == CardData.CardType.Action)
		{
			Debug.Log(inCard.cardData.deckIdx);
			inCard.cardData.deckIdx = -1;
			curActionCardsIdx += 1;
			inCard.cardData.status = CardData.Status.Hand;
		}
	}
	public void CardDiscard(Card inCard)
	{
		if (inCard.cardData.type == CardData.CardType.Talent)
		{

			//Debug.Log(player.nextHandIdx);
			player.hand[player.nextHandIdx].cardData.cardID = -1;
			inCard.cardData.hand = -1;
			inCard.cardData.deckIdx = -1;
			inCard.cardData.status = CardData.Status.Discard;
			inCard.cardData.discardIdx = curTalentDiscardIdx;
			curTalentDiscardIdx += 1;
			CompactPlayerHand(inCard.cardData.handIdx);
			inCard.cardData.handIdx = -1;

		}
		else if (inCard.cardData.type == CardData.CardType.Action)
		{
			inCard.cardData.status = CardData.Status.Discard;
			inCard.cardData.discardIdx = curActionDiscardIdx;
			curActionDiscardIdx += 1;
			
		}
	}
	void CompactPlayerHand(int inHandIDX)
	{
		
	}
	
    
    
}
