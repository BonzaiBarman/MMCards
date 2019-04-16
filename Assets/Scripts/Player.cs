using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;

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

public class Player : MonoBehaviour
{

	Vector3[,] plyrMovieLocs = new [,] {{new Vector3(-6.65f, 0.08f, -3.55f), new Vector3(-6.65f, 0.08f, -3.85f), new Vector3(-6.65f, 0.08f, -4.15f)},
	{new Vector3(-6.65f, 0.08f, -1.1f), new Vector3(-6.65f, 0.08f, -1.4f), new Vector3(-6.65f, 0.08f, -1.7f)},
	{new Vector3(-6.65f, 0.08f, 1.4f), new Vector3(-6.65f, 0.08f, 1.1f), new Vector3(-6.65f, 0.08f, 0.8f)},
	{new Vector3(-6.65f, 0.08f, 3.95f), new Vector3(-6.65f, 0.08f, 3.65f), new Vector3(-6.65f, 0.08f, 3.35f)}};
	
	Vector3[] movieLocs = {new Vector3(-2.1f, 2f, -0.2f), new Vector3(-0.8f, 2f, -0.2f), new Vector3(0.5f, 2f, -0.2f), new Vector3(1.8f, 2f, -0.2f), new Vector3(3.1f, 2f, -0.2f), new Vector3(4.4f, 2f, -0.2f), new Vector3(5.7f, 2f, -0.2f)};
	
	Vector3[] movieStackLocs = {new Vector3(-3.2f, 0.01f, -3.8f), new Vector3(-3.05f, 0.01f, 3.85f), new Vector3(7.45f, 0.01f, 3.85f), new Vector3(7.45f, 0.01f, -3.85f)};
	Vector3[] movieStackRots = new [] {new Vector3(350f, 0f, 0f), new Vector3(0f, 90f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0f, 270f, 0f)};

	public enum PlayerType
	{
		Human,
		Computer,
	}
	//public enum PlayerAction
	//{
	//	DrawTalent,
	//	DrawTalentDiscard,
	//	DrawActionCollect,
	//	DrawActionRaid,
	//	DrawActionSabotage,
	//	DrawActionTrade,
	//	DrawActionChaos,
	//	DrawActionRunOver,
	//	TradingTalent,
	//	RaidingTalent,
	//	SabotagingMovie,
	//	MakeMovie
	//}
	
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
	
	//public GameObject makeMovieButton;
	
	GameControl gControl;
	// Start is called before the first frame update
    void Start()
    {

    	//Debug.Log(hand.Length);
	    gControl = FindObjectOfType<GameControl>();
	    //playerName.text = "Waldorf";
	    score = 0;
	    scoreText.text = "Score: " + score;
	    //ChangeBackgroundMaterial("green");
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
		Debug.Log("problem in GetHandFromCardID");
		return cnt;
	}
	public void SetName(string inName)
	{
		TextMeshPro gName = transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
		gName.text = inName;
		//daText.DOText(inName, 2f, true, ScrambleMode.All);
	}
    
	public string GetName()
	{
		TextMeshPro gName = transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
		return gName.text;
		//daText.DOText(inName, 2f, true, ScrambleMode.All);
	}
	//public void RunAnimation(string inAnimName)
	//{
	//	if (inAnimName == "rotate")
	//	{
	//		DOTween.Play("NameRot");

	//	}
	//}
	
	public void ChangeBackgroundMaterial(string inMaterial)
	{
		GameObject back = transform.GetChild(1).gameObject;
		if (inMaterial == "gold")
		{
			back.GetComponent<Renderer>().material = goldMaterial;
			//back.SetActive(false);
		}
		else if (inMaterial == "red")
		{
			back.GetComponent<Renderer>().material = redMaterial;
		}
		else if (inMaterial == "green")
		{
			back.GetComponent<Renderer>().material = greenMaterial;
		}
		else if (inMaterial == "blue")
		{
			back.GetComponent<Renderer>().material = blueMaterial;
		}
		else
		{
			back.GetComponent<Renderer>().material = goldMaterial;
		}
		//iTween.MoveTo(gameObject, iTween.Hash("y", 0.05, "x", transform.position.x - 0.15));
		//transform.DOMoveY(0.03f, 0.8f, false);
	}
	
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
		
		//tmesh.color = new Color32(0, 130, 0, 255);
		origMaterial = transform.GetChild(1).gameObject.GetComponent<Renderer>().material;
		transform.GetChild(1).gameObject.GetComponent<Renderer>().material = greenMaterial;
		
		//tanim[1].DORestart();
		//tanim[1].DOPlay();
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

		//resetting player action before player takes an action
		//Using DrawTalent as a default
		playerAction =	PlayerAction.DrawTalent;
		
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
			break;
		}
		
		//yield return new WaitUntil(() => playerActed == true);
		
		playerActed = false;
		gControl.curPlayer += 1;
		if (gControl.curPlayer >= gControl.playerCount){gControl.curPlayer = 0;}
		//change player name color to black
		transform.GetChild(1).gameObject.GetComponent<Renderer>().material = origMaterial;
	}

	IEnumerator ComputerTurn()
	{
		//Debug.Log("Player Turn: " + playerName.text);
		yield return new WaitForSeconds(1f);
		while (!playerActed)
		{
			//pause before turn
			yield return new WaitForSeconds(0.3f);
			
			
			//Determine What Computer Player Does
			//int rnd = Random.Range(0,0); //2
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
				//Debug.Log(drawCard.cardData.deckIdx + " : " + drawCard.cardData.cardName);
				//Debug.Log(drawCard.cardData.cardName);


				if (nextHandIdx >= 7)
				{
					if(playerType == PlayerType.Computer){drawCard.GetComponent<Rigidbody>().isKinematic = false;}
					drawCard.cardData.handIdx = 99;
					drawCard.DrawCardAnim(playerID, nextHandIdx);
					cardToFillIdx = ComputerDetermineDiscard();
					//Debug.Log(cardToFillIdx);
					//Debug.Log(gControl.GetTalentCardFromID(hand[cardToFillIdx]).cardData.cardName);
					nextHandIdx -= 1;
					//throw away card
					//Needs tobe done
					//***********
					discardCard = gControl.GetTalentCardFromID(hand[cardToFillIdx]);
					//Debug.Log(discardCard.cardData.cardName);
					if(playerType == PlayerType.Computer){discardCard.GetComponent<Rigidbody>().isKinematic = false;}
					
					
					discardCard.cardData.hand = -1;
					discardCard.cardData.deckIdx = -1;
					discardCard.cardData.status = CardData.Status.Discard;
					discardCard.cardData.discardIdx = gControl.curTalentDiscardIdx;
					//Debug.Log("discard idx: " + discardCard.cardData.discardIdx + "  curdiscard: " + gControl.curTalentDiscardIdx);
					
					//discardCard.DiscardTalentCard();
					//gControl.curTalentDiscardIdx += 1;
					
					//if (drawCard.cardData.deckIdx == 69)
					//{
					//	yield return new WaitForSeconds(1f);
					//	gControl.StartCoroutine("ReshuffleTalentCards");
					//	yield return new WaitForSeconds(3f);
					//}
					
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
					
					//if (drawCard.cardData.deckIdx == 69)
					//{
					//	yield return new WaitForSeconds(1f);
					//	gControl.StartCoroutine("ReshuffleTalentCards");
					//	yield return new WaitForSeconds(3f);
					//}
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
				
					AlignHand();
					//pause to wait for next player turn
					yield return new WaitForSeconds(2.5f);
					playerActed = true;

				}

				break;
			case PlayerAction.MakeMovie:
				movies[nextMovieIDX] = Instantiate(newMovie, plyrMovieLocs[playerID, nextMovieIDX], Quaternion.identity);
                int xr = 3;
                MakeMovie();
				xr = 4;
				yield return new WaitUntil(() => playerActed == true);
				playerActed = false;
				
				StackMovieCards();
				yield return new WaitUntil(() => playerActed == true);
				nextMovieIDX += 1;
				//playerActed = true;
				break;
		}
			
			


		}
		playerActed = false;
		gControl.curPlayer += 1;
		if (gControl.curPlayer >= gControl.playerCount){gControl.curPlayer = 0;}
		//change player name color to black
		//transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().color = Color.black;
		transform.GetChild(1).gameObject.GetComponent<Renderer>().material = origMaterial;
	}
	
	//IEnumerator CompDrawTalent()
	//{
		
	//	Card drawCard;
	//	drawCard = gControl.GetTalentCardFromID(gControl.GetNextTalentCardID());
	//	drawCard.DrawCardAnim(playerID, nextHandIdx);
	//	hand[nextHandIdx] = drawCard.cardData.cardID;
		
	//	//if (drawCard.cardData.deckIdx == 69)
	//	//{
	//	//	gControl.StartCoroutine("ReshuffleTalentCards");
	//	//}
		
	//	drawCard.cardData.deckIdx = -1;
	//	drawCard.cardData.status = CardData.Status.Hand;
	//	drawCard.cardData.hand = playerID;
	//	drawCard.cardData.handIdx = nextHandIdx;
	//	nextHandIdx += 1;
	//	gControl.curTalentCardsIdx += 1;
				
	//	//pause after card drawn but before align
	//	yield return new WaitForSeconds(0.05f);
	//	drawCard.GetComponent<Rigidbody>().isKinematic = true;
		
	//	yield return new WaitForSeconds(1f);
	//	playerActed = true;
		
	//}
	
	//IEnumerator CompDrawTalentDiscard()
	//{
	//	Card drawCard;
	//	Card discardCard;
	//	int cardToFillIdx;
	//	drawCard = gControl.GetTalentCardFromID(gControl.GetNextTalentCardID());
		
	//	drawCard.GetComponent<Rigidbody>().isKinematic = false;
	//	drawCard.cardData.handIdx = 99;
	//	drawCard.DrawCardAnim(playerID, nextHandIdx);
	//	cardToFillIdx = ComputerDetermineDiscard();
		
	//	nextHandIdx -= 1;
		
	//	discardCard = gControl.GetTalentCardFromID(hand[cardToFillIdx]);
	//	discardCard.GetComponent<Rigidbody>().isKinematic = false;
					
	//	//if (drawCard.cardData.deckIdx == 69)
	//	//{
	//	//	gControl.StartCoroutine("ReshuffleTalentCards");
	//	//}
					
	//	discardCard.cardData.hand = -1;
	//	discardCard.cardData.deckIdx = -1;
	//	discardCard.cardData.status = CardData.Status.Discard;
	//	discardCard.cardData.discardIdx = gControl.curTalentDiscardIdx;
	//	Debug.Log("discard idx: " + discardCard.cardData.discardIdx + "  curdiscard: " + gControl.curTalentDiscardIdx);

	//	hand[cardToFillIdx] = drawCard.cardData.cardID; //gControl.GetNextTalentCardID();
	//	drawCard.cardData.deckIdx = -1;
	//	drawCard.cardData.status = CardData.Status.Hand;
	//	drawCard.cardData.hand = playerID;
	//	drawCard.cardData.handIdx = cardToFillIdx;
	//	nextHandIdx += 1;
	//	gControl.curTalentCardsIdx += 1;
				
	//	//pause after card drawn but before align
	//	yield return new WaitForSeconds(0.05f);
	//	drawCard.GetComponent<Rigidbody>().isKinematic = true;
				
	//	discardCard.DiscardTalentCard();
	//	gControl.curTalentDiscardIdx += 1;
		
	//	AlignHand();
	//	//pause to wait for next player turn
	//	yield return new WaitForSeconds(1f);
	//	playerActed = true;
		
	//}
	
	IEnumerator DrawTalent()
	{

		yield return null;
		//playerActed = false;
		//yield return new WaitForSeconds(2.5f);
		//playerActed = true;
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
				//Debug.Log(discardedCardIdx);
				//so be it
				//Debug.Log("nexthandidx: " + nextHandIdx);
				//nextHandIdx = 6;
				CompactHand(discardedCardIdx);
				//Debug.Log("nexthandidx: " + nextHandIdx);
				hand[nextHandIdx] = holdCardID;
				gControl.GetTalentCardFromID(holdCardID).MoveCard(playerID, nextHandIdx);
				gControl.GetTalentCardFromID(holdCardID).cardData.handIdx = nextHandIdx;
				nextHandIdx += 1;
				discardedCardIdx = 0;
				holdCardID = 0;
			}
			//playerActed = true;
			
						
		}
	}
	
	int ComputerDetermineDiscard()
	{
		int handIdxToReturn = -1;
		PopHandInfo();
		//Debug.Log("actor:" + actorCnt + " director:" + directorCnt + " music:" + musicCnt + " scrn:" + screenplayCnt + " raid:" + raidProtectionCnt + " sabotage:" + sabotageProtectionCnt);

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
		//Debug.Log(hand[0] + ", " + hand[1] + ", " + hand[2] + ", " + hand[3] + ", " + hand[4] + ", " + hand[5] + ", " + hand[6]);
		//Debug.Log("hand length: " + hand.Length);
		for (int i = 0; i < hand.Length; i++)
		{
			if (hand[i] != -1)
			{
				
				tmpCard = gControl.GetTalentCardFromID(hand[i]);
				//Debug.Log(tmpCard.cardData.cardName);
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

		if(playerType == PlayerType.Human)
		{
			Debug.Log("make movie clicked");			
		}
		else
		{
			//do the computer make a movie
			int curActor = 0;

			PopHandInfo();			
			
			//create movie instance
			//movies[nextMovieIDX] = Instantiate(newMovie, plyrMovieLocs[playerID, nextMovieIDX], Quaternion.identity);
			movies[nextMovieIDX].transform.DORotate(new Vector3(90,0,0), 1);
			
			//determine movie cards
			//will need alot more logic here in the future
			movies[nextMovieIDX].InitMovie();
			movies[nextMovieIDX].screenplayID = hand[lastScreenplay];
			movies[nextMovieIDX].directorID = hand[hiDirector];		
			movies[nextMovieIDX].musicID = hand[hiMusic];
			int idx = 0;
			foreach(int crd in hand)
			{
				if((gControl.GetTalentCardFromID(crd).cardData.subType == CardData.SubType.Actor) || (gControl.GetTalentCardFromID(crd).cardData.subType == CardData.SubType.Actress))
				{
					Debug.Log(nextMovieIDX + " " + curActor);
					movies[nextMovieIDX].actorID[curActor] = crd;
					hand[idx] = -1;
					curActor += 1;
				}
				idx += 1;
			}
			movies[nextMovieIDX].SetTitle(gControl.GetNewMovieTitle(gControl.GetTalentCardFromID(movies[nextMovieIDX].screenplayID).cardData.cardName));
			
			score += movies[nextMovieIDX].value();
			scoreText.text = "Score: " + score;
			MoveMovieCards();
			
		}
	}
	
	void MoveMovieCards()
	{		
		Card moveCard;
		
		//gControl = FindObjectOfType<GameControl>();
		
		//discard screenplay
		moveCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].screenplayID);
		
		moveCard.cardData.hand = -1;
		moveCard.cardData.status = CardData.Status.Movie;
		moveCard.cardData.movie = playerID;
		moveCard.cardData.movieIdx = nextMovieIDX;
		moveCard.transform.DOMove(movieLocs[0], 0.5f);
		moveCard.transform.DORotate(new Vector3(0,0,0), 0.5f);
		hand[lastScreenplay] = -1;
		
		//discard director
		moveCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].directorID);
		
		moveCard.cardData.hand = -1;
		moveCard.cardData.status = CardData.Status.Movie;
		moveCard.cardData.movie = playerID;
		moveCard.cardData.movieIdx = nextMovieIDX;
		moveCard.transform.DOMove(movieLocs[1], 0.5f);
		moveCard.transform.DORotate(new Vector3(0,0,0), 0.5f);
		hand[hiDirector] = -1;
		
		//discard music
		moveCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].musicID);
		
		moveCard.cardData.hand = -1;
		moveCard.cardData.status = CardData.Status.Movie;
		moveCard.cardData.movie = playerID;
		moveCard.cardData.movieIdx = nextMovieIDX;
		moveCard.transform.DOMove(movieLocs[2], 0.5f);
		moveCard.transform.DORotate(new Vector3(0,0,0), 0.5f);
		hand[hiMusic] = -1;
		
		//discard actors
		int idx = 0;
		//int idxActor = 0;
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

		//gControl.gMovieHud.gameObject.SetActive(true);
		gControl.gMovieHud.enabled = true;
		gControl.gGameHud.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetName() + " makes a movie '" + movies[nextMovieIDX].title + "'";
		gControl.gMovieHud.transform.GetChild(15).GetComponent<TextMeshProUGUI>().text = movies[nextMovieIDX].title + " (" + movies[nextMovieIDX].value() + ")";
		gControl.gMovieBackground.gameObject.SetActive(true);
	}

	void StackMovieCards()
	{		
		Card discardCard;
		
		//discard screenplay
		Debug.Log("player: " + playerID + " nextMovieIDX: " + nextMovieIDX + " sp: " +  movies[nextMovieIDX].screenplayID + " spCardName: " + gControl.GetTalentCardFromID(movies[nextMovieIDX].screenplayID).cardData.cardName);
		discardCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].screenplayID);
		discardCard.cardData.hand = -1;
		discardCard.cardData.status = CardData.Status.Discard;
		discardCard.cardData.discard =	CardData.CardType.Talent;
		discardCard.cardData.discardIdx = gControl.curTalentDiscardIdx;
		discardCard.transform.DOMove(movieStackLocs[playerID], 0.5f);
		discardCard.transform.DORotate(movieStackRots[playerID], 0.5f);
		gControl.curTalentDiscardIdx += 1;
		
		
		//discard director
		Debug.Log("nextMovieIDX: " + nextMovieIDX + " dir: " +  movies[nextMovieIDX].directorID + " dirCardName: " + gControl.GetTalentCardFromID(movies[nextMovieIDX].directorID).cardData.cardName);
		discardCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].directorID);
		
		discardCard.cardData.hand = -1;
		discardCard.cardData.status = CardData.Status.Discard;
		discardCard.cardData.discard = CardData.CardType.Talent;
		discardCard.cardData.discardIdx = gControl.curTalentDiscardIdx;
		discardCard.transform.DOMove(movieStackLocs[playerID], 0.5f);
		discardCard.transform.DORotate(movieStackRots[playerID], 0.5f);
		gControl.curTalentDiscardIdx += 1;
		
		//discard music
		Debug.Log("mus: " +gControl.GetTalentCardFromID(movies[nextMovieIDX].musicID).cardData.cardName);
		discardCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].musicID);
		
		discardCard.cardData.hand = -1;
		discardCard.cardData.status = CardData.Status.Discard;
		discardCard.cardData.discard = CardData.CardType.Talent;
		discardCard.cardData.discardIdx = gControl.curTalentDiscardIdx;
		discardCard.transform.DOMove(movieStackLocs[playerID], 0.5f);
		discardCard.transform.DORotate(movieStackRots[playerID], 0.5f);
		gControl.curTalentDiscardIdx += 1;
		
		//discard actors
		int idx = 0;
		//int idxActor = 0;
		foreach(int crd in movies[nextMovieIDX].actorID)
		{
			if(movies[nextMovieIDX].actorID[idx] != -1)
			{
				discardCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].actorID[idx]);
		
				discardCard.cardData.hand = -1;
				discardCard.cardData.status = CardData.Status.Discard;
				discardCard.cardData.discard = CardData.CardType.Talent;
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
		
	}

}


