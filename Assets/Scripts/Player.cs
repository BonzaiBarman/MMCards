using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;

public class Player : MonoBehaviour
{

	Vector3[,] plyrMovieLocs = new [,] {{new Vector3(-6.65f, 0.08f, -3.55f), new Vector3(-6.65f, 0.08f, -3.85f), new Vector3(-6.65f, 0.08f, -4.15f)},
	{new Vector3(-6.65f, 0.08f, -1.1f), new Vector3(-6.65f, 0.08f, -1.4f), new Vector3(-6.65f, 0.08f, -1.7f)},
	{new Vector3(-6.65f, 0.08f, 1.4f), new Vector3(-6.65f, 0.08f, 1.1f), new Vector3(-6.65f, 0.08f, 0.8f)},
	{new Vector3(-6.65f, 0.08f, 3.95f), new Vector3(-6.65f, 0.08f, 3.65f), new Vector3(-6.65f, 0.08f, 3.35f)}};
	
	Vector3[] movieLocs = {new Vector3(-2.1f, 2f, -0.2f), new Vector3(-0.8f, 2f, -0.2f), new Vector3(0.5f, 2f, -0.2f), new Vector3(1.8f, 2f, -0.2f), new Vector3(3.1f, 2f, -0.2f), new Vector3(4.4f, 2f, -0.2f), new Vector3(5.7f, 2f, -0.2f)};
	
	Vector3[] movieStackLocs = {new Vector3(-3.2f, 0.01f, -3.8f), new Vector3(-3.05f, 0.01f, 3.85f), new Vector3(7.45f, 0.01f, 3.85f), new Vector3(7.45f, 0.01f, -3.85f)};
	Vector3[] movieStackRots = new [] {new Vector3(0f, 0f, 180f), new Vector3(0f, 90f, 180f), new Vector3(0f, 180f, 180f), new Vector3(0f, 270f, 180f)};

	public enum PlayerType
	{
		Human,
		Computer,
	}

	public Movie newMovie;
	public Movie[] movies;
	public int playerID;
	public PlayerType playerType;
	public Material goldMaterial;
	public Material redMaterial;
	public Material greenMaterial;
	public Material blueMaterial;
	public Material yellowMaterial;
	public Material violetMaterial;
	Material origMaterial;
	public TextMeshPro playerName;
	public TextMeshPro scoreText;
	public int score;
	public bool playerActed = false;
	public int discardedCardIdx = 0;
	public int holdCardID = 0;
	public PlayerAction playerAction =	PlayerAction.DrawTalent;
	
	public int[] hand = new int[] {-1, -1, -1, -1, -1, -1, -1};
	public int nextHandIdx = 0;
	
	int nextMovieIDX = 0;
	
	int actorCnt = 0;
	int directorCnt = 0;
	int musicCnt = 0;
	int screenplayCnt = 0;
	int raidProtectionCnt = 0;
	int sabotageProtectionCnt = 0;
	int lowActor= -1;
	int lowActorValue = 99;
	int hiActor= -1;
	int hiActorValue = -1;
	int lowDirector = -1;
	int lowDirectorValue = 99;
	int hiDirector = -1;
	int hiDirectorValue = -1;
	int lowMusic = -1;
	int lowMusicValue = 99;
	int hiMusic = -1;
	int hiMusicValue = -1;
	int lastScreenplay = -1;
	int lastSabotageProtection = -1;
	int lastRaidProtection = -1;

	GameControl gControl;

	// Start is called before the first frame update
    void Start()
    {
	    gControl = FindObjectOfType<GameControl>();
	    score = 0;
	    scoreText.text = "Score: " + score;
	    movies = new Movie[3];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	public void CompactHand(int inIndex)
	{
		for (int i = inIndex; i < (hand.Length - 1); i++)
		{
			if (hand[i] != -1)
			{
				hand[i] = hand[i + 1];
				if (hand[i] != -1)
				{
					gControl.GetTalentCardFromID(hand[i]).MoveCard(playerID,i);
					gControl.GetTalentCardFromID(hand[i]).cardData.handIdx = i;
				}
			}
		}
		hand[hand.Length - 1] = -1;
		int cnt = 0;
		foreach(int idx in hand)
		{
			if (idx == -1)
			{
				nextHandIdx = cnt;
				break;
			}
			cnt += 1;
		}
		for (int i = nextHandIdx; i < (hand.Length); i++)
		{
			hand[i] = -1;
		}
	}
	
	public int GetHandIndexFromCardID(int inCardID)
	{
		int cnt = 0;
		foreach(int crd in hand)
		{
			if (inCardID == crd)
			{
				return cnt;
			}
			cnt += 1;
		}
		Debug.Log("problem in GetHandIndexFromCardID");
		return cnt;
	}
	public void SetName(string inName)
	{
		TextMeshPro gName = transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
		gName.text = inName;
	}
    
	public string GetName()
	{
		TextMeshPro gName = transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
		return gName.text;
	}
	//public void RunAnimation(string inAnimName)
	//{
	//	if (inAnimName == "rotate")
	//	{
	//		DOTween.Play("NameRot");

	//	}
	//}
	
	//public void ChangeBackgroundMaterial(string inMaterial)
	//{
	//	GameObject back = transform.GetChild(1).gameObject;
	//	if (inMaterial == "gold")
	//	{
	//		back.GetComponent<Renderer>().material = goldMaterial;
	//		//back.SetActive(false);
	//	}
	//	else if (inMaterial == "red")
	//	{
	//		back.GetComponent<Renderer>().material = redMaterial;
	//	}
	//	else if (inMaterial == "green")
	//	{
	//		back.GetComponent<Renderer>().material = greenMaterial;
	//	}
	//	else if (inMaterial == "blue")
	//	{
	//		back.GetComponent<Renderer>().material = blueMaterial;
	//	}
	//	else
	//	{
	//		back.GetComponent<Renderer>().material = goldMaterial;
	//	}
	//}
	
	public void AlignHand()
	{
		StartCoroutine("ProcessAlignHand");
	}
	
	IEnumerator ProcessAlignHand()
	{
		for(int i = 0; i < hand.Length; i++)
		{
			if (hand[i] != -1)
			{
				Card daCard = gControl.GetTalentCardFromID(hand[i]);
				
				daCard.MoveCard(playerID, i);
				daCard.RotateCard(playerID);
				yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
			}
		}		
	}
	
	public void DoTurn()
	{
		TextMeshPro tmesh = transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
		DOTweenAnimation[] tanim = tmesh.GetComponents<DOTweenAnimation>();
		tanim[0].DORestart();
		tanim[0].DOPlay();

		origMaterial = transform.GetChild(1).gameObject.GetComponent<Renderer>().material;
		transform.GetChild(1).gameObject.GetComponent<Renderer>().material = greenMaterial;
	

		if (playerType == PlayerType.Human)
		{
			StartCoroutine("HumanTurn");
		}
		else if (playerType == PlayerType.Computer)
		{
			StartCoroutine("ComputerTurn");
		}
	}
	
	IEnumerator HumanTurn()
	{

		yield return new WaitUntil(() => playerActed == true);
		switch (playerAction)
		{
		case	PlayerAction.DrawActionCollect:
			StartCoroutine("DrawActionCollect");
			break;
		case	PlayerAction.DrawActionRaid:
			StartCoroutine("DrawActionRaid");
			break;
		case	PlayerAction.DrawActionSabotage:
			StartCoroutine("DrawActionSabotage");
			break;
		case	PlayerAction.DrawActionTrade:
			StartCoroutine("DrawActionTrade");
			break;
		case	PlayerAction.DrawActionChaos:
			StartCoroutine("DrawActionChaos");
			break;
		case	PlayerAction.DrawActionRunOver:
			StartCoroutine("DrawActionRunOver");
			break;
		case	PlayerAction.DrawTalent:
			StartCoroutine("DrawTalent");
			yield return new WaitForSeconds(2.5f);
			break;
		case	PlayerAction.DrawTalentDiscard:
			playerActed = false;
			StartCoroutine("DrawTalentDiscard");
			yield return new WaitUntil(() => playerActed == true);
			yield return new WaitForSeconds(2.5f);
			break;
		case PlayerAction.MakeMovie:
			playerActed = false;
			//movies[nextMovieIDX] = Instantiate(newMovie, plyrMovieLocs[playerID, nextMovieIDX], Quaternion.identity);

			MakeMovie();

			gControl.SetTickerText("Select Cards to add to the movie.");
			yield return new WaitUntil(() => playerActed == true);
			playerActed = false;
				
			StackMovieCards();
			yield return new WaitUntil(() => playerActed == true);
			nextMovieIDX += 1;
			//playerActed = true;
			break;
		default:
			Debug.Log("player action error: " + playerAction);
			break;
		}

		playerActed = false;
		gControl.curPlayer += 1;
		if (gControl.curPlayer >= gControl.playerCount){gControl.curPlayer = 0;}
		//change player name color to black
		transform.GetChild(1).gameObject.GetComponent<Renderer>().material = origMaterial;
		//turn off make movie button
		gControl.gGameHud.transform.GetChild(1).gameObject.SetActive(false);
	}

	IEnumerator ComputerTurn()
	{
		yield return new WaitForSeconds(1f);
		while (!playerActed)
		{
			//pause before turn
			yield return new WaitForSeconds(0.3f);
			
			//*
			//Determine What Computer Player Does
			//*
			
			if(CanMakeMovie())
			{
				playerAction =	PlayerAction.MakeMovie;
			}
			else
			{
				playerAction =	PlayerAction.DrawTalent;
			}
			
			switch (playerAction)
			{
			case PlayerAction.DrawTalent:
				//draw a talent card

				Card drawCard;
				Card discardCard;
				int cardToFillIdx;
				drawCard = gControl.GetTalentCardFromID(gControl.GetNextTalentCardID());

				if (nextHandIdx >= 7)
				{
					if(playerType == PlayerType.Computer){drawCard.GetComponent<Rigidbody>().isKinematic = false;}
					drawCard.cardData.handIdx = 99;
					drawCard.DrawCardAnim(playerID, nextHandIdx);
					cardToFillIdx = ComputerDetermineDiscard();
					nextHandIdx -= 1;
					//throw away card
					//Needs tobe done
					//***********
					discardCard = gControl.GetTalentCardFromID(hand[cardToFillIdx]);
					if(playerType == PlayerType.Computer){discardCard.GetComponent<Rigidbody>().isKinematic = false;}
					
					
					discardCard.cardData.hand = -1;
					discardCard.cardData.deckIdx = -1;
					discardCard.cardData.status = CardData.Status.Discard;
					discardCard.cardData.discardIdx = gControl.curTalentDiscardIdx;
					
					hand[cardToFillIdx] = drawCard.cardData.cardID;
					drawCard.cardData.deckIdx = -1;
					drawCard.cardData.status = CardData.Status.Hand;
					drawCard.cardData.hand = playerID;
					drawCard.cardData.handIdx = cardToFillIdx;
					nextHandIdx += 1;
					gControl.curTalentCardsIdx += 1;
				
					//pause after card drawn but before align
					yield return new WaitForSeconds(0.05f);
					if(playerType == PlayerType.Computer){drawCard.GetComponent<Rigidbody>().isKinematic = true;}
				
					yield return new WaitForSeconds(1f);
				
					AlignHand();
					
					yield return new WaitForSeconds(1.5f);
					
					discardCard.DiscardTalentCard();
					gControl.curTalentDiscardIdx += 1;
					//pause to wait for next player turn
					yield return new WaitForSeconds(2.5f);
					playerActed = true;
				}
				else
				{
					drawCard.DrawCardAnim(playerID, nextHandIdx);
					cardToFillIdx = nextHandIdx;

					hand[cardToFillIdx] = drawCard.cardData.cardID;
					drawCard.cardData.deckIdx = -1;
					drawCard.cardData.status = CardData.Status.Hand;
					drawCard.cardData.hand = playerID;
					drawCard.cardData.handIdx = cardToFillIdx;
					nextHandIdx += 1;
					gControl.curTalentCardsIdx += 1;
				
					//pause after card drawn but before align
					yield return new WaitForSeconds(0.5f);
					if(playerType == PlayerType.Computer){drawCard.GetComponent<Rigidbody>().isKinematic = true;}
				
					AlignHand();
					//pause to wait for next player turn
					yield return new WaitForSeconds(2.5f);
					playerActed = true;

				}

				break;
			case PlayerAction.MakeMovie:
				//movies[nextMovieIDX] = Instantiate(newMovie, plyrMovieLocs[playerID, nextMovieIDX], Quaternion.identity);

                MakeMovie();

				yield return new WaitUntil(() => playerActed == true);
				playerActed = false;
				
				StackMovieCards();
				yield return new WaitUntil(() => playerActed == true);
				nextMovieIDX += 1;

				break;
		}

		}
		playerActed = false;
		gControl.curPlayer += 1;
		if (gControl.curPlayer >= gControl.playerCount){gControl.curPlayer = 0;}
		transform.GetChild(1).gameObject.GetComponent<Renderer>().material = origMaterial;
	}

	IEnumerator DrawTalent()
	{
		yield return null;
		//yield return new WaitForSeconds(2.5f);
	}
	
	IEnumerator DrawTalentDiscard()
	{
		if (playerType ==	PlayerType.Human)
		{
			playerActed = false;
		
			yield return new WaitUntil(() => playerActed == true);

			if (discardedCardIdx == 99)
			{
				//so be it
				
				discardedCardIdx = 0;
				holdCardID = 0;
				gControl.GetTalentCardFromID(holdCardID).cardData.handIdx = -1;
				//playerAction =	PlayerAction.DrawTalent;
			}
			else
			{
				CompactHand(discardedCardIdx);

				hand[nextHandIdx] = holdCardID;
				gControl.GetTalentCardFromID(holdCardID).MoveCard(playerID, nextHandIdx);
				gControl.GetTalentCardFromID(holdCardID).cardData.handIdx = nextHandIdx;
				nextHandIdx += 1;
				discardedCardIdx = 0;
				holdCardID = 0;
			}
						
		}
	}
	
	int ComputerDetermineDiscard()
	{
		int handIdxToReturn = -1;
		PopHandInfo();

		if (raidProtectionCnt > 1)
		{
			handIdxToReturn = lastRaidProtection;
		}
		else if (sabotageProtectionCnt > 1)
		{
			handIdxToReturn = lastSabotageProtection;
		}
		else if (directorCnt > 2)
		{
			handIdxToReturn = lowDirector;
		}
		else if (musicCnt > 2)
		{
			handIdxToReturn = lowMusic;			
		}
		else if (screenplayCnt > 2)
		{
			handIdxToReturn = lastScreenplay;
		}
		else  if (actorCnt > 2)
		{
			handIdxToReturn = lowActor;
		}
		else
		{
			if (screenplayCnt > 1)
			{
				handIdxToReturn = lastScreenplay;
			}
			else if (musicCnt > 1)
			{
				handIdxToReturn = lowMusic;
			}
			else if (directorCnt > 1)
			{
				handIdxToReturn = lowDirector;
			}
			else
			{
				handIdxToReturn = lowActor;
			}
		}
		return handIdxToReturn;
	}
	
	public void PopHandInfo()
	{
		//reset vars
		actorCnt = 0;
		directorCnt = 0;
		musicCnt = 0;
		screenplayCnt = 0;
		raidProtectionCnt = 0;
		sabotageProtectionCnt = 0;
		lowActor = -1;
		lowActorValue = 99;
		hiActor= -1;
		hiActorValue = -1;
		lowDirector = -1;
		lowDirectorValue = 99;
		hiDirector = -1;
		hiDirectorValue = -1;
		lowMusic = -1;
		lowMusicValue = 99;
		hiMusic = -1;
		hiMusicValue = -1;
		lastScreenplay = -1;
		lastSabotageProtection = -1;
		lastRaidProtection = -1;
			
		Card tmpCard;

		for (int i = 0; i < hand.Length; i++)
		{
			if (hand[i] != -1)
			{
				tmpCard = gControl.GetTalentCardFromID(hand[i]);

				switch (tmpCard.cardData.subType)
				{
				case CardData.SubType.Actor: 
					actorCnt += 1;
					if (tmpCard.cardData.value.Sum() > hiActorValue)
					{
						hiActor = i;
						hiActorValue = tmpCard.cardData.value.Sum(); 
					}
					if (tmpCard.cardData.value.Sum() < lowActorValue)
					{
						lowActor = i;
						lowActorValue = tmpCard.cardData.value.Sum(); 
					}
					break;
				case CardData.SubType.Actress:
					actorCnt += 1;
					if (tmpCard.cardData.value.Sum() > hiActorValue)
					{
						hiActor = i;
						hiActorValue = tmpCard.cardData.value.Sum(); 
					}
					if (tmpCard.cardData.value.Sum() < lowActorValue)
					{
						lowActor = i;
						lowActorValue = tmpCard.cardData.value.Sum(); 
					}
					break;
				case CardData.SubType.Director:
					directorCnt += 1;
					if (tmpCard.cardData.value.Sum() > hiDirectorValue)
					{
						hiDirector = i;
						hiDirectorValue = tmpCard.cardData.value.Sum(); 
					}
					if (tmpCard.cardData.value.Sum() < lowDirectorValue)
					{
						lowDirector = i;
						lowDirectorValue = tmpCard.cardData.value.Sum(); 
					}
					break;
				case CardData.SubType.Music:
					musicCnt += 1;
					if (tmpCard.cardData.value.Sum() > hiMusicValue)
					{
						hiMusic = i;
						hiMusicValue = tmpCard.cardData.value.Sum(); 
					}
					if (tmpCard.cardData.value.Sum() < lowMusicValue)
					{
						lowMusic = i;
						lowMusicValue = tmpCard.cardData.value.Sum(); 
					}					
					break;
				case CardData.SubType.Screenplay:
					screenplayCnt += 1;
					lastScreenplay = i;
					break;
				case CardData.SubType.RaidProtection:
					raidProtectionCnt += 1;
					lastRaidProtection = i;
					break;
				case CardData.SubType.SabotageProtection:
					sabotageProtectionCnt += 1;
					lastSabotageProtection = i;
					break;
				}					
			}
		}
	}
	
	public bool CanMakeMovie()
	{
		PopHandInfo();
		if(actorCnt > 0 && directorCnt > 0 && musicCnt > 0 && screenplayCnt > 0)
		{
			return true;
		}
		return false;
	}
	
	public void MakeMovie()
	{
		movies[nextMovieIDX] = Instantiate(newMovie, plyrMovieLocs[playerID, nextMovieIDX], Quaternion.identity);
		PopHandInfo();
		movies[nextMovieIDX].transform.DORotate(new Vector3(90,0,0), 1);
		movies[nextMovieIDX].InitMovie();
		
		if(playerType == PlayerType.Human)
		{
			//player make movie
			//movies[nextMovieIDX].transform.DORotate(new Vector3(90,0,0), 1);
			//movies[nextMovieIDX].InitMovie();
			
			movies[nextMovieIDX].screenplayID = hand[lastScreenplay];
			movies[nextMovieIDX].directorID = hand[hiDirector];		
			movies[nextMovieIDX].musicID = hand[hiMusic];
			movies[nextMovieIDX].actorID[0] = hand[hiActor];
            hand[hiActor] = -1;

			//movies[nextMovieIDX].SetTitle(gControl.GetNewMovieTitle(gControl.GetTalentCardFromID(movies[nextMovieIDX].screenplayID).cardData.cardName));
			//score += movies[nextMovieIDX].value();
			//scoreText.text = "Score: " + score;
			//MoveMovieCards();
		}
		else
		{
			//Computer make a movie
			int curActor = 0;
			
			//create movie instance
			
			//movies[nextMovieIDX] = Instantiate(newMovie, plyrMovieLocs[playerID, nextMovieIDX], Quaternion.identity);
			//movies[nextMovieIDX].transform.DORotate(new Vector3(90,0,0), 1);
			//movies[nextMovieIDX].InitMovie();
			
			//determine movie cards
			//will need alot more logic here in the future
			movies[nextMovieIDX].screenplayID = hand[lastScreenplay];
			movies[nextMovieIDX].directorID = hand[hiDirector];		
			movies[nextMovieIDX].musicID = hand[hiMusic];
			int idx = 0;
			foreach(int crd in hand)
			{
				if((gControl.GetTalentCardFromID(crd).cardData.subType == CardData.SubType.Actor) || (gControl.GetTalentCardFromID(crd).cardData.subType == CardData.SubType.Actress))
				{
					//Debug.Log(nextMovieIDX + " " + curActor);
					movies[nextMovieIDX].actorID[curActor] = crd;
					hand[idx] = -1;
					curActor += 1;
				}
				idx += 1;
			}
			//movies[nextMovieIDX].SetTitle(gControl.GetNewMovieTitle(gControl.GetTalentCardFromID(movies[nextMovieIDX].screenplayID).cardData.cardName));
			//score += movies[nextMovieIDX].value();
			//scoreText.text = "Score: " + score;
			//MoveMovieCards();
			
		}

		movies[nextMovieIDX].SetTitle(gControl.GetNewMovieTitle(gControl.GetTalentCardFromID(movies[nextMovieIDX].screenplayID).cardData.cardName));
		score += movies[nextMovieIDX].value();
		scoreText.text = "Score: " + score;
		MoveMovieCards();
		
	}
	
	void MoveMovieCards()
	{		
		Card moveCard;

		//set the movies screenplay card data and move to movie hud
		moveCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].screenplayID);
		
		moveCard.cardData.hand = -1;
		moveCard.cardData.status = CardData.Status.Movie;
		moveCard.cardData.movie = playerID;
		moveCard.cardData.movieIdx = nextMovieIDX;
		moveCard.transform.DOMove(movieLocs[0], 0.5f);
		moveCard.transform.DORotate(new Vector3(0,0,0), 0.5f);
		hand[lastScreenplay] = -1;
		
		//director
		moveCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].directorID);
		
		moveCard.cardData.hand = -1;
		moveCard.cardData.status = CardData.Status.Movie;
		moveCard.cardData.movie = playerID;
		moveCard.cardData.movieIdx = nextMovieIDX;
		moveCard.transform.DOMove(movieLocs[1], 0.5f);
		moveCard.transform.DORotate(new Vector3(0,0,0), 0.5f);
		hand[hiDirector] = -1;
		
		//music
		moveCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].musicID);
		
		moveCard.cardData.hand = -1;
		moveCard.cardData.status = CardData.Status.Movie;
		moveCard.cardData.movie = playerID;
		moveCard.cardData.movieIdx = nextMovieIDX;
		moveCard.transform.DOMove(movieLocs[2], 0.5f);
		moveCard.transform.DORotate(new Vector3(0,0,0), 0.5f);
		hand[hiMusic] = -1;
		
		//actors
		int idx = 0;

		foreach(int crd in movies[nextMovieIDX].actorID)
		{
			if(movies[nextMovieIDX].actorID[idx] != -1)
			{
				moveCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].actorID[idx]);
		
				moveCard.cardData.hand = -1;
				moveCard.cardData.status = CardData.Status.Movie;
				moveCard.cardData.movie = playerID;
				moveCard.cardData.movieIdx = nextMovieIDX;
				moveCard.transform.DOMove(movieLocs[idx + 3], 0.5f);
				moveCard.transform.DORotate(new Vector3(0,0,0), 0.5f);
				switch(idx)
				{
				case 1:
					gControl.gMovieHud.transform.GetChild(8).gameObject.SetActive(true);
					gControl.gMovieHud.transform.GetChild(9).gameObject.SetActive(true);
					break;
				case 2:
					gControl.gMovieHud.transform.GetChild(10).gameObject.SetActive(true);
					gControl.gMovieHud.transform.GetChild(11).gameObject.SetActive(true);
					break;
				case 3:
					gControl.gMovieHud.transform.GetChild(12).gameObject.SetActive(true);
					gControl.gMovieHud.transform.GetChild(13).gameObject.SetActive(true);
					break;
				}
			}
			else
			{
				switch(idx)
				{
				case 1:
					gControl.gMovieHud.transform.GetChild(8).gameObject.SetActive(false);
					gControl.gMovieHud.transform.GetChild(9).gameObject.SetActive(false);
					break;
				case 2:
					gControl.gMovieHud.transform.GetChild(10).gameObject.SetActive(false);
					gControl.gMovieHud.transform.GetChild(11).gameObject.SetActive(false);
					break;
				case 3:
					gControl.gMovieHud.transform.GetChild(12).gameObject.SetActive(false);
					gControl.gMovieHud.transform.GetChild(13).gameObject.SetActive(false);
					break;
				}
			}
			idx += 1;
		}

		gControl.gMovieHud.enabled = true;
		gControl.gGameHud.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetName() + " makes a movie '" + movies[nextMovieIDX].title + "'";
		gControl.gMovieHud.transform.GetChild(15).GetComponent<TextMeshProUGUI>().text = movies[nextMovieIDX].title + " (" + movies[nextMovieIDX].value() + ")";
		gControl.gMovieBackground.gameObject.SetActive(true);
	}

	void StackMovieCards()
	{		
		Card discardCard;
		
		//discard screenplay
		discardCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].screenplayID);
		discardCard.cardData.hand = -1;
		discardCard.cardData.discardIdx = gControl.curTalentDiscardIdx;
		discardCard.transform.DOMove(movieStackLocs[playerID], 0.5f);
		discardCard.transform.DORotate(movieStackRots[playerID], 0.5f);
		gControl.curTalentDiscardIdx += 1;
		
		
		//discard director
		discardCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].directorID);
		
		discardCard.cardData.hand = -1;
		discardCard.cardData.discardIdx = gControl.curTalentDiscardIdx;
		discardCard.transform.DOMove(movieStackLocs[playerID], 0.5f);
		discardCard.transform.DORotate(movieStackRots[playerID], 0.5f);
		gControl.curTalentDiscardIdx += 1;
		
		//discard music
		discardCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].musicID);
		
		discardCard.cardData.hand = -1;
		discardCard.cardData.discardIdx = gControl.curTalentDiscardIdx;
		discardCard.transform.DOMove(movieStackLocs[playerID], 0.5f);
		discardCard.transform.DORotate(movieStackRots[playerID], 0.5f);
		gControl.curTalentDiscardIdx += 1;
		
		//discard actors
		int idx = 0;
		foreach(int crd in movies[nextMovieIDX].actorID)
		{
			if(movies[nextMovieIDX].actorID[idx] != -1)
			{
				discardCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].actorID[idx]);
		
				discardCard.cardData.hand = -1;
				discardCard.cardData.discardIdx = gControl.curTalentDiscardIdx;
				discardCard.transform.DOMove(movieStackLocs[playerID], 0.5f);
				discardCard.transform.DORotate(movieStackRots[playerID], 0.5f);
				gControl.curTalentDiscardIdx += 1;
			}
			idx += 1;
		}
		FixHandAfterMovie();
		playerActed = true;
	}
	
	void FixHandAfterMovie()
	{
		int cnt = 0;
		int[] hold = new int[7] {-1,-1,-1,-1,-1,-1,-1};
		for(int idx = 0; idx < hand.Length; idx++)
		{
			if(hand[idx] != -1)
			{
				hold[cnt] = hand[idx];
				cnt += 1;
			}
		}
		
		hand = hold;
		
		if( cnt == 0)
		{
			nextHandIdx = 0;
		}
		else
		{
			

			for(int idx = 0; idx < hand.Length; idx++)
			{
				if(hand[idx] != -1)
				{
					gControl.GetTalentCardFromID(hand[idx]).MoveCard(playerID, idx);
				}
			}
			nextHandIdx = cnt;					
		}
	}
	
	public void MovieCardClicked(Card inCard)
	{
		//set settings of clicked card to movie
		Card movCard = inCard;
		bool exchange = false;
		int movieLoc = 0;
		
		
		if(inCard.cardData.status == CardData.Status.Movie)
		{
			if(inCard.cardData.subType == CardData.SubType.Actor || inCard.cardData.subType == CardData.SubType.Actress)
			{
				if(GetMovieActorCount() > 1)
				{
					int mIdx = -1;
					int hndIdx = -1;

					inCard.cardData.hand = playerID;
					inCard.cardData.status = CardData.Status.Hand;
					inCard.cardData.movie = -1;
					inCard.cardData.movieIdx = -1;
					for(int idx = 0; idx < hand.Length; idx++)
					{
						if(hand[idx] == -1)
						{
							hndIdx = idx;
							break;
						}
					}
				
					for(int act = 0; act <  movies[nextMovieIDX].actorID.Length; act++)
					{
						if(movies[nextMovieIDX].actorID[act] == inCard.cardData.cardID)
						{
							mIdx = act;						
							break;
						}
					}
					movies[nextMovieIDX].actorID[mIdx] = -1;
					hand[hndIdx] = inCard.cardData.cardID;
					inCard.MoveCard(playerID, hndIdx);
					int hole = -1;
					for(int idx = 0; idx < movies[nextMovieIDX].actorID.Length; idx++)
					{
						if(movies[nextMovieIDX].actorID[idx] == -1)
						{
							hole = idx;
							break;
						}
					}
					for(int idx = hole; idx < movies[nextMovieIDX].actorID.Length - 1; idx++)
					{
						movies[nextMovieIDX].actorID[idx] = movies[nextMovieIDX].actorID[idx + 1];
						if(movies[nextMovieIDX].actorID[idx + 1] != -1)
						{
							movCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].actorID[idx + 1]);
							movCard.transform.DOMove(movieLocs[idx + 3], 0.2f);
						}
					}
					//clear last actor slot
					movies[nextMovieIDX].actorID[3] = -1;
				}
				else
				{
					gControl.SetTickerText("Can't remove only actor in a movie.");
				}
			}
		}
		else
		{
			int hIdx = GetHandIndexFromCardID(inCard.cardData.cardID);
			
			inCard.cardData.hand = -1;
			inCard.cardData.status = CardData.Status.Movie;
			inCard.cardData.movie = playerID;
			inCard.cardData.movieIdx = nextMovieIDX;
			
			switch(inCard.cardData.subType)
			{
			case CardData.SubType.Screenplay:
	
				movCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].screenplayID);
				movies[nextMovieIDX].screenplayID = inCard.cardData.cardID;
				movieLoc = 0;
				exchange = true;
							
				break;
			case CardData.SubType.Director:
			
				movCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].directorID);
				movies[nextMovieIDX].directorID = inCard.cardData.cardID;
				movieLoc = 1;
				exchange = true;
				
				break;
			case CardData.SubType.Music:
			
				movCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].musicID);
				movies[nextMovieIDX].musicID = inCard.cardData.cardID;
				movieLoc = 2;
				exchange = true;
				
				break;
			case CardData.SubType.Actor: case CardData.SubType.Actress:
				int naIDX = GetNextMovieActorIndex();
				movies[nextMovieIDX].actorID[naIDX] = inCard.cardData.cardID;
				movieLoc = naIDX + 3;
				hand[hIdx] = -1;
				exchange = false;
				break;
			case CardData.SubType.RaidProtection:
				return;
			case CardData.SubType.SabotageProtection:
				return;
			default:
				return;
			}
			
			if(exchange) 
			{
				movCard.cardData.hand = playerID;
				movCard.cardData.status = CardData.Status.Hand;
				movCard.cardData.movie = -1;
				movCard.cardData.movieIdx = -1;
				
				hand[hIdx] = movCard.cardData.cardID;
				
				inCard.transform.DOMove(movieLocs[movieLoc], 0.5f);		
				inCard.transform.DORotate(new Vector3(0,0,0), 0.5f);
				movCard.MoveCard(playerID, hIdx);				
			}
			else
			{
				inCard.transform.DOMove(movieLocs[movieLoc], 0.5f);		
				inCard.transform.DORotate(new Vector3(0,0,0), 0.5f);
			}
		}
		//update movie score in canvas
		gControl.gMovieHud.transform.GetChild(15).GetComponent<TextMeshProUGUI>().text = movies[nextMovieIDX].title + " (" + movies[nextMovieIDX].value() + ")";
		FixMovieHudLabels();
	}
	
	void FixMovieHudLabels()
	{
		int inIdx = 8;
		for(int idx = 1; idx < movies[nextMovieIDX].actorID.Length; idx++)
		{
			if(movies[nextMovieIDX].actorID[idx] == -1)
			{
				gControl.gMovieHud.transform.GetChild(inIdx).gameObject.SetActive(false);
				gControl.gMovieHud.transform.GetChild(inIdx + 1).gameObject.SetActive(false);
			}
			else
			{
				gControl.gMovieHud.transform.GetChild(inIdx).gameObject.SetActive(true);
				gControl.gMovieHud.transform.GetChild(inIdx + 1).gameObject.SetActive(true);
			}
			inIdx += 2;
		}
	}
	
	int GetNextMovieActorIndex()
	{
		for(int act = 0; act <  movies[nextMovieIDX].actorID.Length; act++)
		{
			if(movies[nextMovieIDX].actorID[act] == -1)
			{
				return act;
			}
		}
		return -1;
	}
	
	int GetMovieActorCount()
	{
		int cnt = 0;
		for(int act = 0; act <  movies[nextMovieIDX].actorID.Length; act++)
		{
			if(movies[nextMovieIDX].actorID[act] != -1)
			{
				cnt++;
			}
		}
		return cnt;
	}

}


