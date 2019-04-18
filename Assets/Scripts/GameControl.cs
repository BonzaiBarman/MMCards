using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;
using TMPro;

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
	Utilities utils = new Utilities();
	Card[] daCards;
	Card[] actionCards;
	public int curActionCardsIdx = 0;
	public int curActionDiscardIdx = 0;
	Card[] talentCards;
	public int curTalentCardsIdx = 0;
	public int curTalentDiscardIdx = 0;
	public int curPlayer = -1;
	int holdPlayer = -1;
	bool gameStarted = false;
	bool gameOver = false;
	int dealer = 3;
	public bool dealing = false;
	public bool shuffling = false;
	public int thePlayerIndex;
	public MovieTitles movieTitles;
	public int TalentCardDeckHiIdx = 69;
	
	
	Player[] player;
	public int playerCount;
	
	//public GameObject makeMovieButton;
	public Canvas gGameHud;
	public Canvas gMovieHud;
	public GameObject gMovieBackground;
	
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
	    
	    if(!gameOver)
	    {
		    if (gameStarted && shuffling == false)
		    {
			    if (holdPlayer != curPlayer)
			    {

				    ReshuffleCheck();

					if(shuffling == false)
				    {
					    holdPlayer = curPlayer;
						
						DoTickerStartTurnMessage();
						
						player[curPlayer].DoTurn();				    	
				    }

			    }
			}	    	
	    }
	    else
	    {
	    	
	    }

	}

	public void ReshuffleCheck()
	{
		if (curTalentCardsIdx >= TalentCardCount)
		{
			shuffling = true;
			
			StartCoroutine("ReshuffleTalentCards");
		}
		if (curActionCardsIdx >= ActionCardCount)
		{
			shuffling = true;
			StartCoroutine("ReshuffleActionCards");
		}
		//yield return new WaitForSeconds(1f);
	}
	
	IEnumerator InitGame()
	{
		
		//sets the player number on this machine
		
		thePlayerIndex = 0;
		//get all the cards
		daCards = FindObjectsOfType<Card>();
		
		InitHud();
		InitActionCards();
		InitTalentCards();
		InitPlayers();
		LoadMovieTitles();
		
		//makeMovieButton.SetActive(false);
		
		//wait for 2 seconds. cards should have fallen by then
		yield return new WaitForSeconds(2f);
		//stop gravity on cards
		foreach (Card item in daCards)
		{
			item.GetComponent<Rigidbody>().isKinematic = true;
		}
		//move cards in both decks physically to match order in deck
		actionCards = actionCards.OrderBy(go => go.cardData.deckIdx).ToArray();
		float nvalue = 0.02f;
		for (int i = 49; i >= 0; i--)
		{
			actionCards[i].transform.DOMoveY(nvalue,0);
			nvalue += .005f;
		}
		talentCards = talentCards.OrderBy(go => go.cardData.deckIdx).ToArray();
		nvalue = 0.02f;
		for (int i = 69; i >= 0; i--)
		{
			talentCards[i].transform.DOMoveY(nvalue,0);
			nvalue += .005f;
		}
		
		
		//Deal Cards
		StartCoroutine("DealCards", dealer);
		//Start Game
		yield return new WaitForSeconds(10f);
		gameStarted = true;
		//StartCoroutine("MainGameLoop");
		
	}

	//IEnumerator MainGameLoop()
	//{
	//	gameStarted = true;
	//	while (!gameOver)
	//	{
			
	//	}
	//}

	void InitHud()
	{
		//turn movie stuff off
		gGameHud.transform.GetChild(1).gameObject.SetActive(false);
		gMovieHud.enabled = false;
		//init ticker with dealing...
		gGameHud.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Dealling...";
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
		utils.ShuffleCards(actionCards);

		//Sort action cards by shuffle index
		actionCards = actionCards.OrderBy(go => go.cardData.deckIdx).ToArray();
		//move cards to the height to drop from
		float loc = 2f;
		for (int i = 49; i >= 0; i--)
		{
			loc = loc + 0.1f;
			actionCards[i].transform.DOMoveY(loc,0);
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
				talentCards[count].cardData.deck = 0;
				talentCards[count].cardData.deckIdx = count; //talentCards[count].cardData.cardID;
				talentCards[count].cardData.movie = -1;
				talentCards[count].cardData.movieIdx = -1;
				talentCards[count].cardData.discard = 0;
				talentCards[count].cardData.discardIdx = -1;
				count += 1;
			}
		}
		
		//shuffle
		utils.ShuffleCards(talentCards);
		
		//Sort talent cards by shuffle index
		talentCards = talentCards.OrderBy(go => go.cardData.deckIdx).ToArray();
		//move cards to the height to drop from
		float loc = 2f;
		for (int i = 69; i >= 0; i--)
		{
			//Debug.Log(talentCards[i].cardData.deckIdx);
			loc = loc + 0.1f;
			talentCards[i].transform.DOMoveY(loc,0);
		}
		//turn off Kinematic to activate gravity
		//talentCards = talentCards.OrderByDescending(go => go.cardData.deckIdx).ToArray();
		foreach (Card item in talentCards)
		{
			item.GetComponent<Rigidbody>().isKinematic = false;
		}
		//talentCards = talentCards.OrderBy(go => go.cardData.deckIdx).ToArray();
	}

	//IEnumerator DropTalentCards()
	//{
	//	for (int i = 69; i >= 0; i--)
	//	{
	//		talentCards[i].GetComponent<Rigidbody>().isKinematic = false;
	//		yield return new WaitForSeconds(0.01f);
	//	}
	//}

	void InitPlayers()
	{
		player = FindObjectsOfType<Player>();
		foreach(Player ply in player)
		{
			ply.hand = new int[] {-1, -1, -1, -1, -1, -1, -1};
			ply.nextHandIdx = 0;			
		}
		//Sort players by playerID
		player = player.OrderBy(go => go.playerID).ToArray();
		playerCount = player.Length;
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

	IEnumerator ReshuffleTalentCards()
	{
		if(shuffling)
		{
			Debug.Log("top reshuffle");
			//yield return new WaitForSeconds(0.5f);
			int cnt = 0;
			foreach(Card crd in talentCards)
			{
				if (crd.cardData.status	== CardData.Status.Discard)
				{
					crd.cardData.status = CardData.Status.Deck;
					cnt += 1;
				}
			}
			Debug.Log(cnt);
			int[] newDeck = new int[cnt];
			cnt = 0;
			foreach(Card crd in talentCards)
			{
				if (crd.cardData.status	== CardData.Status.Deck)
				{
					newDeck[cnt] = crd.cardData.cardID;
					cnt += 1;
				}
			}
			utils.ShuffleCards(newDeck);
			cnt = 0;
			foreach (int item in newDeck)
			{
				GetTalentCardFromID(item).cardData.deckIdx = cnt;
				cnt += 1;
			}
			//Sort talent cards before drop by deck/shuffle index
			talentCards = talentCards.OrderByDescending(go => go.cardData.deckIdx).ToArray();
	
			//move cards to the height to drop from
			float loc = 2f;
			//Debug.Log(talentCards.Length);
			foreach (Card item in talentCards)
			{
				if (item.cardData.status ==	CardData.Status.Deck)
				{
					item.GetComponent<Rigidbody>().isKinematic = true;
					loc = loc + 0.1f;
					item.transform.DOMove(new Vector3(3f,loc,0f), 0f);
					item.transform.DORotate(new Vector3(180f,180f,0f), 0f);
					//item.transform.DOMoveY(loc,0);
				}
			}
			
			//Drop Cards
			//curTalentCardsIdx = 0;
			yield return new WaitForSeconds(2f);

			foreach (Card item in talentCards) 
			{
				if (item.cardData.status ==	CardData.Status.Deck)
				{
					item.GetComponent<Rigidbody>().isKinematic = false;
					//item.transform.DOMoveY(TalentCardDeckHiIdx * 0.04f, 0.15f);
					TalentCardDeckHiIdx += 1;
					//yield return new WaitForSeconds(0.1f);
				}
			}

			yield return new WaitForSeconds(3f);
			
			//Lock Cards
			curTalentDiscardIdx = 0;
			cnt = 0;
			int deckCount = 0;

			foreach (Card item in talentCards)
			{
				if (item.cardData.status ==	CardData.Status.Deck)
				{
					item.GetComponent<Rigidbody>().isKinematic = true;
					deckCount += 1;
				}
				else
				{
					cnt += 1;
				}
			}
			
			
			//sort deck physically
			foreach (Card item in talentCards) 
			{
				if (item.cardData.status ==	CardData.Status.Deck)
				{
					item.transform.DOMoveY(((deckCount - item.cardData.deckIdx) * 0.01f) + 0.01f, 0f);
					//item.GetComponent<Rigidbody>().isKinematic = false;
				}
			}

			//Sort talent cards by reverse deck\shuffle index
			talentCards = talentCards.OrderBy(go => go.cardData.deckIdx).ToArray();

			Debug.Log(deckCount);
			curTalentCardsIdx = cnt;
			Debug.Log("bottom reshuffle");
			yield return new WaitForSeconds(5f);
			shuffling = false;
		}
	}
	
	IEnumerator ReshuffleActionCards()
	{
		int cnt = 0;
		foreach(Card crd in actionCards)
		{
			if (crd.cardData.status	== CardData.Status.Discard)
			{
				crd.cardData.status = CardData.Status.Deck;
				cnt += 1;
			}
		}
		int[] newDeck = new int[cnt + 1];
		cnt = 0;
		foreach(Card crd in actionCards)
		{
			if (crd.cardData.status	== CardData.Status.Deck)
			{
				newDeck[cnt] = crd.cardData.cardID;
				cnt += 1;
			}
		}
		utils.ShuffleCards(newDeck);
		cnt =0;
		foreach (int item in newDeck)
		{
			actionCards[item].cardData.deckIdx = cnt;
			cnt += 1;
		}
		//Sort talent cards by shuffle index
		actionCards = actionCards.OrderByDescending(go => go.cardData.deckIdx).ToArray();
		//move cards to the height to drop from
		float loc = 2f;
		//Debug.Log(actionCards.Length);
		foreach (Card item in actionCards)
		{
			if (item.cardData.status ==	CardData.Status.Deck)
			{
				item.GetComponent<Rigidbody>().isKinematic = true;
				loc = loc + 0.1f;
				item.transform.DOMove(new Vector3(-0.9f,loc,0f), 0f);
				item.transform.DORotate(new Vector3(180f,180f,0f), 0f);
				//item.transform.DOMoveY(loc,0f);
			}
		}
		//turn off Kinematic to activate gravity
		foreach (Card item in actionCards)
		{
			if (item.cardData.status ==	CardData.Status.Deck)
			{
				item.GetComponent<Rigidbody>().isKinematic = false;
			}
		}
		//wait for 2 seconds. cards should have fallen by then
		yield return new WaitForSeconds(2f);
		//stop gravity on cards
		foreach (Card item in actionCards)
		{
			item.GetComponent<Rigidbody>().isKinematic = true;
		}
		//move cards in both decks physically to match order in deck
		float nvalue = 0.02f;
		foreach (Card item in actionCards)
		{
			if (item.cardData.status ==	CardData.Status.Deck)
			{
				item.transform.DOMove(new Vector3(-0.9f, nvalue, 0f),0f);
				nvalue += .005f;				
			}
		}
		curActionCardsIdx = 0;
		curActionDiscardIdx = 0;
	}
	
	
	public void CardDraw(Card inCard)
	{
		
		if (inCard.cardData.type == CardData.CardType.Talent)
		{

			
			
			//if (inCard.cardData.deckIdx == 69)
			//{
			//	StartCoroutine("ReshuffleTalentCards");
			//}
			//Debug.Log(inCard.cardData.deckIdx + " : " + inCard.cardData.cardName);
			//Debug.Log("cur talent card idx: " + curTalentCardsIdx + " : deckidx " + inCard.cardData.deckIdx + " : cardid " + inCard.cardData.cardID + " : name " + inCard.cardData.cardName);

			if (player[thePlayerIndex].nextHandIdx < 7)
			{
				//Debug.Log(inCard.cardData.deckIdx);
				//Debug.Log(player.nextHandIdx);
				player[thePlayerIndex].hand[player[thePlayerIndex].nextHandIdx] = inCard.cardData.cardID;
				inCard.cardData.hand = 0;
				inCard.cardData.handIdx = player[thePlayerIndex].nextHandIdx;

				inCard.cardData.deckIdx = -1;
				inCard.cardData.status = CardData.Status.Hand;
				player[thePlayerIndex].nextHandIdx += 1;
				curTalentCardsIdx += 1;
				player[thePlayerIndex].playerAction = PlayerAction.DrawTalent;
			}
			else
			{
				inCard.cardData.hand = 0;
				inCard.cardData.handIdx = 99;
				
				inCard.cardData.deckIdx = -1;
				inCard.cardData.status = CardData.Status.Hand;
				
				curTalentCardsIdx += 1;
				player[thePlayerIndex].holdCardID = inCard.cardData.cardID;
				player[thePlayerIndex].playerAction = PlayerAction.DrawTalentDiscard;
				//Debug.Log("Hand Full");
			}

		}
		else if (inCard.cardData.type == CardData.CardType.Action)
		{
			Debug.Log(inCard.cardData.deckIdx);
			Debug.Log(curActionCardsIdx);
			//if (inCard.cardData.deckIdx == 49)
			//{
			//	StartCoroutine("ReshuffleActionCards");
			//}
			inCard.cardData.deckIdx = -1;
			curActionCardsIdx += 1;
			inCard.cardData.status = CardData.Status.Hand;
			switch(inCard.cardData.subType)
			{
			case	CardData.SubType.Collect:
				player[thePlayerIndex].playerAction =	PlayerAction.DrawActionCollect;
				break;
			case	CardData.SubType.Raid:
				player[thePlayerIndex].playerAction =	PlayerAction.DrawActionRaid;
				break;
			case	CardData.SubType.Sabotage:
				player[thePlayerIndex].playerAction =	PlayerAction.DrawActionSabotage;
				break;
			case	CardData.SubType.Trade:
				player[thePlayerIndex].playerAction =	PlayerAction.DrawActionTrade;
				break;
			case	CardData.SubType.Chaos:
				player[thePlayerIndex].playerAction =	PlayerAction.DrawActionChaos;
				break;
			case	CardData.SubType.RunOver:
				player[thePlayerIndex].playerAction =	PlayerAction.DrawActionRunOver;
				break;
			}
			
		}
		player[thePlayerIndex].playerActed = true;
	}
	
	public void CardDiscard(Card inCard)
	{
		if (inCard.cardData.type == CardData.CardType.Talent)
		{
			
			if (player[thePlayerIndex].playerAction == PlayerAction.DrawTalentDiscard)
			{
				inCard.cardData.hand = -1;
				inCard.cardData.deckIdx = -1;
				inCard.cardData.status = CardData.Status.Discard;
				inCard.cardData.discardIdx = curTalentDiscardIdx;
				curTalentDiscardIdx += 1;
				player[thePlayerIndex].discardedCardIdx = inCard.cardData.handIdx;
			}
			else
			{
				int playerHandIdx = player[thePlayerIndex].GetHandIndexFromCardID(inCard.cardData.cardID);

				//Debug.Log(playerHandIdx);
			
				inCard.cardData.hand = -1;
				inCard.cardData.deckIdx = -1;
				inCard.cardData.status = CardData.Status.Discard;
				inCard.cardData.discardIdx = curTalentDiscardIdx;
				curTalentDiscardIdx += 1;
				//Debug.Log("playerhandidx" + playerHandIdx);
				player[thePlayerIndex].CompactHand(playerHandIdx);
				inCard.cardData.handIdx = -1;				
			}
			player[thePlayerIndex].playerActed = true;	

		}
		else if (inCard.cardData.type == CardData.CardType.Action)
		{
			inCard.cardData.status = CardData.Status.Discard;
			inCard.cardData.discardIdx = curActionDiscardIdx;
			curActionDiscardIdx += 1;
			
		}
	}

	public Card GetTalentCardFromID(int inCardID)
	{
		foreach (Card crd in talentCards)
		{
			if(crd.cardData.cardID == inCardID)
			{
				return crd;
			}
		}
		//an error if here
		return talentCards[0];
		
	}
	
	public int GetNextTalentCardID()
	{
		//Debug.Log("cur talent card idx: " + curTalentCardsIdx);
		//Debug.Log("cur talent card idx: " + curTalentCardsIdx + " : deckidx " + talentCards[curTalentCardsIdx].cardData.deckIdx + " : cardid " + talentCards[curTalentCardsIdx].cardData.cardID + " : name " + talentCards[curTalentCardsIdx].cardData.cardName);
		return talentCards[curTalentCardsIdx].cardData.cardID;
	}
    
	IEnumerator DealCards(int inDealer)
	{
		dealing = true;
		
		int[,] dealOrder = new int[4,4] {{1,2,3,0}, {2,3,0,1}, {3,0,1,2}, {0,1,2,3}};

		
		for (int i = 0; i < (4); i++)
		{
			for (int plyr = 0; plyr < 4; plyr++)
			{
				//Debug.Log(talentCards[curTalentCardsIdx].cardData.cardName);
				player[dealOrder[inDealer, plyr]].hand[player[dealOrder[inDealer, plyr]].nextHandIdx] = talentCards[curTalentCardsIdx].cardData.cardID;
				if(dealOrder[inDealer, plyr] != 0){talentCards[curTalentCardsIdx].GetComponent<Rigidbody>().isKinematic = false;}
				talentCards[curTalentCardsIdx].DealCardAnim(dealOrder[inDealer, plyr], i);
				//Debug.Log(talentCards[curTalentCardsIdx].cardData.deckIdx);
				talentCards[curTalentCardsIdx].cardData.deckIdx = -1;
				talentCards[curTalentCardsIdx].cardData.status = CardData.Status.Hand;
				talentCards[curTalentCardsIdx].cardData.hand = plyr;
				talentCards[curTalentCardsIdx].cardData.handIdx = i;
				player[dealOrder[inDealer, plyr]].nextHandIdx = i + 1;
				curTalentCardsIdx += 1;
				yield return new WaitForSeconds(0.6f);
				//talentCards[curTalentCardsIdx].MoveCard(dealOrder[inDealer, plyr], i);

				//if(dealOrder[inDealer, plyr] != 0){talentCards[curTalentCardsIdx].GetComponent<Rigidbody>().isKinematic = true;}
			}
		}

		for (int i = 1; i < player.Length; i++)
		{
			player[i].AlignHand();
		}

		for (int i = 0; i < (4); i++)
		{
			for (int plyr = 0; plyr < 4; plyr++)
			{
				if (plyr != thePlayerIndex)
				{
					GetTalentCardFromID(player[plyr].hand[i]).GetComponent<Rigidbody>().isKinematic = true;					
				}
			}
		}
		dealing = false;
	}
	
	public PlayerAction GetCurPlayerAction()
	{
		return player[curPlayer].playerAction;
	}
	
	public int GetCurPlayerNextHandIdx()
	{
		return player[curPlayer].nextHandIdx;
	}
	
	public void SortTalentDiscard()
	{
		foreach(Card crd in talentCards)
		{
			if(crd.cardData.status == CardData.Status.Discard)
			{
				Vector3 loc = crd.transform.position;
				float yValue = 0.1f + (crd.cardData.discardIdx / 100f);
				if(loc.x < 4.5f || loc.x > 4.9f){loc.x = 4.7f;}
				if(loc.z < -0.2f || loc.z > 0.2f){loc.z = 0f;}
				crd.transform.DOMove(new Vector3(4.7f, yValue, 0f), 0f);
				Vector3 rot = crd.transform.rotation.eulerAngles;
				Debug.Log(rot.x + " " + rot.y + " " + rot.z);
				//crd.transform.DORotate(new Vector3(rot.x, rot.y, rot.z), 0f);
				crd.transform.DORotate(new Vector3(0, rot.y, 0), 0f);
			}
		}
	}
	
	private void DoTickerStartTurnMessage()
	{
		TextMeshProUGUI tmesh = gGameHud.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
		if(curPlayer == thePlayerIndex)
		{
			if(player[curPlayer].CanMakeMovie())
			{
				gGameHud.transform.GetChild(1).gameObject.SetActive(true);
				tmesh.text = "Your turn, You can Make a Movie, Draw an Action card, or Draw a Talent card.";
			}
			else
			{
				gGameHud.transform.GetChild(1).gameObject.SetActive(false);
				tmesh.text = "Your turn, Draw an Action or Talent card.";
			}
		}
		else
		{
			tmesh.text = player[curPlayer].GetName() + "'s turn";	
		}
		
	}
	
	public void MakeMovieClicked()
	{
		
		//Need to do player control of making movie
		//player[thePlayerIndex].MakeMovie();
		gGameHud.transform.GetChild(1).gameObject.SetActive(false);
		player[thePlayerIndex].playerAction =	PlayerAction.MakeMovie;
		player[thePlayerIndex].playerActed = true;
	}
	
	public string GetNewMovieTitle(string inType)
	{
		string retString = "";
		switch (inType)
		{
		case "Comedy":
			retString = movieTitles.comedy[Random.Range(0, movieTitles.comedy.Length - 1)];
			break;
		case "Drama":
			retString = movieTitles.drama[Random.Range(0, movieTitles.drama.Length - 1)];
			break;		
		case "Horror":
			retString = movieTitles.horror[Random.Range(0, movieTitles.horror.Length - 1)];
			break;		
		case "Musical":
			retString = movieTitles.musical[Random.Range(0, movieTitles.musical.Length - 1)];
			break;		
		case "Western":
			retString = movieTitles.western[Random.Range(0, movieTitles.western.Length - 1)];
			break;
		case "Action":
			retString = movieTitles.action[Random.Range(0, movieTitles.action.Length - 1)];
			break;
		}
		return retString;
	}
	
	public void MovieOKButton()
	{
		gMovieBackground.gameObject.SetActive(false);
		gMovieHud.enabled = false;
		player[curPlayer].playerActed = true;
	}

}
