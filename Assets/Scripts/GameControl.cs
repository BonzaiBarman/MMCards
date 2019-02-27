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
	Card[] talentCards;
	MovieTitles movieTitles;
	
	Player player;
	const int ActionCardCount = 50;
	const int TalentCardCount = 70;
	
	// Start is called before the first frame update
	IEnumerator Start()
    {
	    daname = FindObjectOfType<Name>();
	    float r = daname.transform.position.x;
	    daname.ChangeName("Walter");
	    daname.RunAnimation("rotate");
	    daCards = FindObjectsOfType<Card>();
		InitActionCards();
	    InitTalentCards();
	    //player = new Player();
	    InitPlayer();
	    LoadMovieTitles();
	    yield return new WaitForSeconds(2f);
	    foreach (Card item in daCards)
	    {
			item.GetComponent<Rigidbody>().isKinematic = true;
		}



    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void LoadMovieTitles()
	{
		string filePath = Path.Combine(Application.streamingAssetsPath, "MovieTitles.json");
		if (File.Exists(filePath))
		{
			string dataJson = File.ReadAllText(filePath);
			movieTitles = JsonUtility.FromJson<MovieTitles>(dataJson);
			Debug.Log(movieTitles.comedy[4]);
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

		//move cards to the height to drop from
		float loc = 2f;
		foreach (Card item in actionCards)
		{
			//if (item.cardData.type == CardData.CardType.Action)
			//{
			loc = loc + 0.03f;
			item.transform.DOMoveY(loc,0);
			//}
		}
		//turn off Kinematic to activate gravity
		foreach (Card item in actionCards)
		{
			//if (item.cardData.type == CardData.CardType.Action)
			//{
				item.GetComponent<Rigidbody>().isKinematic = false;
			//}
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
		
		//move cards to the height to drop from
		float loc = 2f;
		foreach (Card item in talentCards)
		{
			//if (item.cardData.type == CardData.CardType.Action)
			//{
			loc = loc + 0.03f;
			item.transform.DOMoveY(loc,0);
			//}
		}
		//turn off Kinematic to activate gravity
		foreach (Card item in talentCards)
		{
			//if (item.cardData.type == CardData.CardType.Action)
			//{
			item.GetComponent<Rigidbody>().isKinematic = false;
			//}
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
			inCards[r] = tmp;
		}
	}
	
	void InitPlayer()
	{
		player = FindObjectOfType<Player>();
	}
	
	public void CardDraw(Card inCard)
	{
		if (inCard.cardData.type == CardData.CardType.Talent)
		{
			if (player.nextHandIdx < 7)
			{
				Debug.Log(player.nextHandIdx);
				player.hand[player.nextHandIdx] = inCard;
				inCard.cardData.hand = 0;
				inCard.cardData.handIdx = player.nextHandIdx;
				inCard.cardData.deckIdx = -1;
				inCard.cardData.status = CardData.Status.Hand;
				player.nextHandIdx += 1;				
			}
			else
			{
				Debug.Log("Hand Full");
			}
			
		}
		else if (inCard.cardData.type == CardData.CardType.Action)
		{
			inCard.cardData.status = CardData.Status.Hand;
		}
	}
	public void CardDiscard(Card inCard)
	{
		if (inCard.cardData.type == CardData.CardType.Talent)
		{
			if (player.nextHandIdx < 7)
			{
				Debug.Log(player.nextHandIdx);
				player.hand[player.nextHandIdx] = inCard;
				inCard.cardData.hand = -1;
				inCard.cardData.handIdx = -1;
				inCard.cardData.deckIdx = -1;
				inCard.cardData.status = CardData.Status.Discard;
				player.nextHandIdx += 1;				
			}
			else
			{
				Debug.Log("Hand Full");
			}
			
		}
		else if (inCard.cardData.type == CardData.CardType.Action)
		{
			inCard.cardData.status = CardData.Status.Discard;
			
		}
	}
	
	
    
    
}
