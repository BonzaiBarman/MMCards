using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;

public class Player : MonoBehaviour
{
    GameControl gControl;

    //location of movie names under player name on the scorecard
    Vector3[,] plyrMovieLocs = new [,] {{new Vector3(-6.65f, 0.08f, -3.55f), new Vector3(-6.65f, 0.08f, -3.85f), new Vector3(-6.65f, 0.08f, -4.15f)},
	{new Vector3(-6.65f, 0.08f, -1.1f), new Vector3(-6.65f, 0.08f, -1.4f), new Vector3(-6.65f, 0.08f, -1.7f)},
	{new Vector3(-6.65f, 0.08f, 1.4f), new Vector3(-6.65f, 0.08f, 1.1f), new Vector3(-6.65f, 0.08f, 0.8f)},
	{new Vector3(-6.65f, 0.08f, 3.95f), new Vector3(-6.65f, 0.08f, 3.65f), new Vector3(-6.65f, 0.08f, 3.35f)}};
	
	//location of cards in movie when hud is showing
	Vector3[] movieLocs = {new Vector3(-2.1f, 2f, -0.2f), new Vector3(-0.8f, 2f, -0.2f), new Vector3(0.5f, 2f, -0.2f), new Vector3(1.8f, 2f, -0.2f), new Vector3(3.1f, 2f, -0.2f), new Vector3(4.4f, 2f, -0.2f), new Vector3(5.7f, 2f, -0.2f)};
	
	//location and rotation of movie card stacks for for each player
	Vector3[] movieStackLocs = {new Vector3(-3.2f, 0.01f, -3.8f), new Vector3(-3.05f, 0.01f, 3.85f), new Vector3(7.45f, 0.01f, 3.85f), new Vector3(7.45f, 0.01f, -3.85f)};
	Vector3[] movieStackRots = new [] {new Vector3(0f, 0f, 180f), new Vector3(0f, 90f, 180f), new Vector3(0f, 180f, 180f), new Vector3(0f, 270f, 180f)};

	//Main Player Data
	public int playerID;
	public PlayerType playerType;
	public TextMeshPro playerName;
	public TextMeshPro scoreText;
	public int score;
    public PlayerAction playerAction = PlayerAction.DrawTalent;

    //Movie variables
    public Movie newMovie;
	public Movie[] movies;
    int nextMovieIDX = 0;

    //Hand variables
    public int[] hand = new int[] { -1, -1, -1, -1, -1, -1, -1 };
    public int nextHandIdx = 0;

    //Hand analysis variables
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

    //Material
    public Material playerSelectedMat;
    Material origMaterial;

    //state, misc varaiables
    public bool playerActed = false;
    public int discardedCardIdx = 0;
    public int holdCardID = 0;

    //constants
    const int CardDrawnHandIDX = 99;
    const int MaxCardsInHand = 7;

    // Start is called before the first frame update
    void Start()
    {
        //init some vars
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
        //inIndex is the card to compact from to the left
        for (int i = inIndex; i < (hand.Length - 1); i++)
		{
			if (hand[i] != -1)
			{
				hand[i] = hand[i + 1];
				if (hand[i] != -1)
				{
                    //move the card
                    gControl.GetTalentCardFromID(hand[i]).MoveCard(playerID,i);
					gControl.GetTalentCardFromID(hand[i]).cardData.handIdx = i;
				}
			}
		}
        //set last hand card to nothing
		hand[hand.Length - 1] = -1;
        //set the index of new nextcard
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
        //clear hand cards after new index
		for (int i = nextHandIdx; i < (hand.Length); i++)
		{
			hand[i] = -1;
		}
	}
	
	public int GetHandIndexFromCardID(int inCardID)
	{
        //find card in hand by cardid
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
	
	public IEnumerator DoTurn()
	{
        //Animate name & change background material when turn starts
        TextMeshPro tmeshName = transform.GetChild((int)PlayerDisplay.Name).gameObject.GetComponent<TextMeshPro>();
		DOTweenAnimation[] tanimName = tmeshName.GetComponents<DOTweenAnimation>();
		tanimName[0].DORestart();
		tanimName[0].DOPlay();

		origMaterial = transform.GetChild((int)PlayerDisplay.Background).gameObject.GetComponent<Renderer>().material;
		transform.GetChild((int)PlayerDisplay.Background).gameObject.GetComponent<Renderer>().material = playerSelectedMat;

        //Run Turn Routine based on type of player
        if (playerType == PlayerType.Human)
		{
			yield return StartCoroutine("HumanTurn");
		}
		else if (playerType == PlayerType.Computer)
		{
			yield return StartCoroutine("ComputerTurn");
		}
		yield return null;
	}
	
	IEnumerator HumanTurn()
	{
		Debug.Log("before switch");
        //Wait for human player action
		yield return new WaitUntil(() => playerActed == true);
        //Act on action
		switch (playerAction)
		{
		case PlayerAction.DrawActionCollect:
            yield return StartCoroutine("DrawActionCollect");
			break;
		case PlayerAction.DrawActionRaid:
            yield return StartCoroutine("DrawActionRaid");
			break;
		case PlayerAction.DrawActionSabotage:
            yield return StartCoroutine("DrawActionSabotage");
			break;
		case PlayerAction.DrawActionTrade:
            yield return StartCoroutine("DrawActionTrade");
			break;
		case PlayerAction.DrawActionChaos:
            yield return StartCoroutine("DrawActionChaos");
			break;
		case PlayerAction.DrawActionRunOver:
            yield return StartCoroutine("DrawActionRunOver");
			break;
		case PlayerAction.DrawTalent:
            yield return StartCoroutine("DrawTalent");
			break;
		case PlayerAction.DrawTalentDiscard:
			playerActed = false;
            yield return StartCoroutine("DrawTalentDiscard");
			yield return new WaitUntil(() => playerActed == true);
			break;
		case PlayerAction.MakeMovie:
             yield return StartCoroutine("ProcessMovie");
             break;
        default:
			Debug.Log("player action error: " + playerAction);
			break;
		}
		Debug.Log("after switch");
		yield return new WaitForSeconds(1.5f);
		playerActed = false;
		gControl.curPlayer += 1;
		if (gControl.curPlayer >= gControl.playerCount){gControl.curPlayer = 0;}
		//change player name color to black
		transform.GetChild(1).gameObject.GetComponent<Renderer>().material = origMaterial;
		//turn off make movie button
		gControl.gNavCanvas.SetMovieButton(false);
	}

	IEnumerator DrawTalent()
	{
		yield return null;
	}
	
	IEnumerator DrawTalentDiscard()
	{
		if (playerType ==	PlayerType.Human)
		{
			playerActed = false;

            //Wait for player to select card to discard
            yield return new WaitUntil(() => playerActed == true);

			if (discardedCardIdx == CardDrawnHandIDX)
			{
				//Drawn card is selected to be discarded
				discardedCardIdx = -1;
				holdCardID = -1;
				gControl.GetTalentCardFromID(holdCardID).cardData.handIdx = -1;
			}
			else
			{
                //Hand card selected to be discarded
                CompactHand(discardedCardIdx);

                //Add drawn card to hand (at end)
				hand[nextHandIdx] = holdCardID;
				gControl.GetTalentCardFromID(holdCardID).MoveCard(playerID, nextHandIdx);
				gControl.GetTalentCardFromID(holdCardID).cardData.handIdx = nextHandIdx;
				nextHandIdx += 1;
				discardedCardIdx = -1;
				holdCardID = -1;
			}
		}
	}

    IEnumerator ProcessMovie()
    {
        playerActed = false;
        Debug.Log("before make movie");
        //init movie
        MakeMovie();
        Debug.Log("after make movie");
        gControl.gNavCanvas.SetTicker("Select Cards to add to the movie.");
        //Wait for player to select cards for the movie and hit OK
        yield return new WaitUntil(() => playerActed == true);
        Debug.Log("after ok");
        movies[nextMovieIDX].SetTitle(movies[nextMovieIDX].title);
        score = score + movies[nextMovieIDX].value();
        scoreText.text = "Score: " + score;
        playerActed = false;

        //move movie cards to pile and fix hand
        StackMovieCards();
        //wait until stack is done
        yield return new WaitUntil(() => playerActed == true);
        nextMovieIDX += 1;
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
			
			movies[nextMovieIDX].screenplayID = hand[lastScreenplay];
			movies[nextMovieIDX].directorID = hand[hiDirector];		
			movies[nextMovieIDX].musicID = hand[hiMusic];
			movies[nextMovieIDX].actorID[0] = hand[hiActor];
            hand[hiActor] = -1;

			movies[nextMovieIDX].SetTitle(gControl.GetNewMovieTitle(gControl.GetTalentCardFromID(movies[nextMovieIDX].screenplayID).cardData.cardName));
			MoveMovieCards();
		}
		else
		{
			//Computer make a movie
			int curActor = 0;
			
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
				gControl.gNavCanvas.TurnOnActorLabel(idx, true);
			}
			else
			{
				gControl.gNavCanvas.TurnOnActorLabel(idx, false);
			}
			idx += 1;
		}
		gControl.gNavCanvas.SetMakeMovie(GetName(), movies[nextMovieIDX].title, movies[nextMovieIDX].value());
		gControl.gMovieBackground.gameObject.SetActive(true);
	}

    public void MovieCardClicked(Card inCard)
    {
        //set settings of clicked card to movie
        Card movCard = inCard;
        bool exchange = false;
        int movieLoc = 0;


        if (inCard.cardData.status == CardData.Status.Movie)
        {
            //if a card in a movie is clicked
            if (inCard.cardData.subType == CardData.SubType.Actor || inCard.cardData.subType == CardData.SubType.Actress)
            {
                if (GetMovieActorCount() > 1)
                {
                    int mIdx = -1;
                    int hndIdx = -1;

                    //put the movie card back in the hand
                    inCard.cardData.hand = playerID;
                    inCard.cardData.status = CardData.Status.Hand;
                    inCard.cardData.movie = -1;
                    inCard.cardData.movieIdx = -1;
                    //find last hand index
                    for (int idx = 0; idx < hand.Length; idx++)
                    {
                        if (hand[idx] == -1)
                        {
                            hndIdx = idx;
                            break;
                        }
                    }
                    //put card in hands last index
                    for (int act = 0; act < movies[nextMovieIDX].actorID.Length; act++)
                    {
                        if (movies[nextMovieIDX].actorID[act] == inCard.cardData.cardID)
                        {
                            mIdx = act;
                            break;
                        }
                    }
                    //remove the cards info from the movie
                    movies[nextMovieIDX].actorID[mIdx] = -1;
                    hand[hndIdx] = inCard.cardData.cardID;
                    inCard.MoveCard(playerID, hndIdx);
                    int hole = -1;
                    //determine movie index of selected card
                    for (int idx = 0; idx < movies[nextMovieIDX].actorID.Length; idx++)
                    {
                        if (movies[nextMovieIDX].actorID[idx] == -1)
                        {
                            hole = idx;
                            break;
                        }
                    }
                    //compact the movie cards
                    for (int idx = hole; idx < movies[nextMovieIDX].actorID.Length - 1; idx++)
                    {
                        movies[nextMovieIDX].actorID[idx] = movies[nextMovieIDX].actorID[idx + 1];
                        if (movies[nextMovieIDX].actorID[idx + 1] != -1)
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
                    //something other than actor clicked
                    gControl.gNavCanvas.SetTicker("Can't remove only actor in a movie.");
                }
            }
        }
        else
        {
            //if a card in the player hand is clicked
            int hIdx = GetHandIndexFromCardID(inCard.cardData.cardID);

            //set selected card data to movie
            inCard.cardData.hand = -1;
            inCard.cardData.status = CardData.Status.Movie;
            inCard.cardData.movie = playerID;
            inCard.cardData.movieIdx = nextMovieIDX;

            switch (inCard.cardData.subType)
            {
                case CardData.SubType.Screenplay:
                    //start swap screenplays
                    movCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].screenplayID);
                    movies[nextMovieIDX].screenplayID = inCard.cardData.cardID;
                    movieLoc = 0;
                    exchange = true;

                    break;
                case CardData.SubType.Director:
                    //start swap director
                    movCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].directorID);
                    movies[nextMovieIDX].directorID = inCard.cardData.cardID;
                    movieLoc = 1;
                    exchange = true;

                    break;
                case CardData.SubType.Music:
                    //start swap music cards
                    movCard = gControl.GetTalentCardFromID(movies[nextMovieIDX].musicID);
                    movies[nextMovieIDX].musicID = inCard.cardData.cardID;
                    movieLoc = 2;
                    exchange = true;

                    break;
                case CardData.SubType.Actor:
                case CardData.SubType.Actress:
                    //add actor to movie
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

            if (exchange)
            {
                //if card exchange put movie card back in hand
                movCard.cardData.hand = playerID;
                movCard.cardData.status = CardData.Status.Hand;
                movCard.cardData.movie = -1;
                movCard.cardData.movieIdx = -1;

                hand[hIdx] = movCard.cardData.cardID;

                //phys put hand card in movie
                inCard.transform.DOMove(movieLocs[movieLoc], 0.5f);
                inCard.transform.DORotate(new Vector3(0, 0, 0), 0.5f);
                movCard.MoveCard(playerID, hIdx);
            }
            else
            {
                //phys put hand card (actor) in movie
                inCard.transform.DOMove(movieLocs[movieLoc], 0.5f);
                inCard.transform.DORotate(new Vector3(0, 0, 0), 0.5f);
            }
        }
        //update movie score in canvas
        gControl.gNavCanvas.UpdateMovieScore(movies[nextMovieIDX].title, movies[nextMovieIDX].value());

        FixMovieHudLabels();
    }

    void FixMovieHudLabels()
    {
        int inIdx = 8;
        for (int idx = 1; idx < movies[nextMovieIDX].actorID.Length; idx++)
        {
            if (movies[nextMovieIDX].actorID[idx] == -1)
            {
                gControl.gNavCanvas.TurnOnActorLabel(idx, false);
            }
            else
            {
                gControl.gNavCanvas.TurnOnActorLabel(idx, true);
            }
            inIdx += 2;
        }
    }

    int GetNextMovieActorIndex()
    {
        for (int act = 0; act < movies[nextMovieIDX].actorID.Length; act++)
        {
            if (movies[nextMovieIDX].actorID[act] == -1)
            {
                return act;
            }
        }
        return -1;
    }

    int GetMovieActorCount()
    {
        int cnt = 0;
        for (int act = 0; act < movies[nextMovieIDX].actorID.Length; act++)
        {
            if (movies[nextMovieIDX].actorID[act] != -1)
            {
                cnt++;
            }
        }
        return cnt;
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
        //align hand after movie cards removed
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

    //--------------------------------------------------------------------
    //--------------------------------------------------------------------
    //Set card var values after a card has been clicked to draw or discard
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------

    public void PlayerDrawCard(Card inCard)
	{
		if (inCard.cardData.type == CardData.CardType.Talent)
		{
            //Drew a talent card
			if (nextHandIdx < MaxCardsInHand)
			{
                //hand is full
                hand[nextHandIdx] = inCard.cardData.cardID;
				inCard.cardData.hand = playerID;
				inCard.cardData.handIdx = nextHandIdx;

				inCard.cardData.deckIdx = -1;
				inCard.cardData.status = CardData.Status.Hand;
				nextHandIdx += 1;
				gControl.curTalentCardsIdx += 1;
				playerAction = PlayerAction.DrawTalent;
			}
			else
			{
                //hand not full
                inCard.cardData.hand = playerID;
				inCard.cardData.handIdx = CardDrawnHandIDX;
				
				inCard.cardData.deckIdx = -1;
				inCard.cardData.status = CardData.Status.Hand;
				
				gControl.curTalentCardsIdx += 1;
				holdCardID = inCard.cardData.cardID;
				playerAction = PlayerAction.DrawTalentDiscard;
			}

		}
		else if (inCard.cardData.type == CardData.CardType.Action)
		{
            //Drew an action card
            inCard.cardData.deckIdx = -1;
			gControl.curActionCardsIdx += 1;
			inCard.cardData.status = CardData.Status.Hand;
			switch(inCard.cardData.subType)
			{
			case	CardData.SubType.Collect:
				playerAction = PlayerAction.DrawActionCollect;
				break;
			case	CardData.SubType.Raid:
				playerAction = PlayerAction.DrawActionRaid;
				break;
			case	CardData.SubType.Sabotage:
				playerAction = PlayerAction.DrawActionSabotage;
				break;
			case	CardData.SubType.Trade:
				playerAction = PlayerAction.DrawActionTrade;
				break;
			case	CardData.SubType.Chaos:
				playerAction = PlayerAction.DrawActionChaos;
				break;
			case	CardData.SubType.RunOver:
				playerAction = PlayerAction.DrawActionRunOver;
				break;
			}
		}
		playerActed = true;
	}
	
	public void PlayerDiscardCard(Card inCard)
	{
		if (inCard.cardData.type == CardData.CardType.Talent)
		{
			//Discarded a talent card
			if (playerAction == PlayerAction.DrawTalentDiscard)
			{
                //discarded a card when hand was full
                inCard.cardData.hand = -1;
				inCard.cardData.deckIdx = -1;
				inCard.cardData.status = CardData.Status.Discard;
				inCard.cardData.discardIdx = gControl.curTalentDiscardIdx;
				gControl.curTalentDiscardIdx += 1;
				discardedCardIdx = inCard.cardData.handIdx;
			}
			else
			{
                //?? NOT SURE THIS HAPPENS ??
                int playerHandIdx = GetHandIndexFromCardID(inCard.cardData.cardID);
			
				inCard.cardData.hand = -1;
				inCard.cardData.deckIdx = -1;
				inCard.cardData.status = CardData.Status.Discard;
				inCard.cardData.discardIdx = gControl.curTalentDiscardIdx;
				gControl.curTalentDiscardIdx += 1;
				CompactHand(playerHandIdx);
				inCard.cardData.handIdx = -1;
                Debug.Log("?? Discarded Card when hand was not full ??");
			}
			playerActed = true;	

		}
		else if (inCard.cardData.type == CardData.CardType.Action)
		{
            //Discarded an action card from in front of hand
            inCard.cardData.status = CardData.Status.Discard;
			inCard.cardData.discardIdx = gControl.curActionDiscardIdx;
			gControl.curActionDiscardIdx += 1;
		}
	}

    //--------------------------------------------------------------------
    //--------------------------------------------------------------------
    //Utilities
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------

    public bool CanMakeMovie()
    {
        PopHandInfo();
        if (actorCnt > 0 && directorCnt > 0 && musicCnt > 0 && screenplayCnt > 0)
        {
            return true;
        }
        return false;
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
        hiActor = -1;
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

    //--------------------------------------------------------------------
    //--------------------------------------------------------------------
    //Computer Player Control and Logic
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------

    IEnumerator ComputerTurn()
    {
        yield return new WaitForSeconds(0.5f);
        while (!playerActed)
        {
            //pause before turn
            yield return new WaitForSeconds(0.3f);

            //*
            //Determine What Computer Player Does
            //*

            if (CanMakeMovie())
            {
                playerAction = PlayerAction.MakeMovie;
            }
            else
            {
                playerAction = PlayerAction.DrawTalent;
            }

            switch (playerAction)
            {
                case PlayerAction.DrawTalent:
                    //draw a talent card
                    gControl.gNavCanvas.SetTicker(GetName() + " draws a talent card");
                    Card drawCard;
                    Card discardCard;
                    int cardToFillIdx;
                    drawCard = gControl.GetTalentCardFromID(gControl.GetNextTalentCardID());

                    if (nextHandIdx >= 7)
                    {
                        if (playerType == PlayerType.Computer) { drawCard.GetComponent<Rigidbody>().isKinematic = false; }
                        drawCard.cardData.handIdx = 99;

                        //drawCard.DrawTalentCard(playerID, nextHandIdx);
                        yield return StartCoroutine(drawCard.DrawTalentCardAnim(playerID));

                        cardToFillIdx = ComputerDetermineDiscard();
                        nextHandIdx -= 1;
                        //throw away card
                        //Needs tobe done
                        //***********
                        discardCard = gControl.GetTalentCardFromID(hand[cardToFillIdx]);
                        if (playerType == PlayerType.Computer) { discardCard.GetComponent<Rigidbody>().isKinematic = false; }


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
                        //yield return new WaitForSeconds(0.05f);
                        if (playerType == PlayerType.Computer) { drawCard.GetComponent<Rigidbody>().isKinematic = true; }

                        //yield return new WaitForSeconds(1f);

                        yield return StartCoroutine("ProcessAlignHand");

                        //yield return new WaitForSeconds(1.5f);

                        //discardCard.DiscardTalentCard();
                        yield return StartCoroutine(discardCard.DiscardTalentCardAnim());

                        gControl.curTalentDiscardIdx += 1;
                        //pause to wait for next player turn
                        playerActed = true;
                    }
                    else
                    {

                        //drawCard.DrawTalentCard(playerID, nextHandIdx);
                        yield return StartCoroutine(drawCard.DrawTalentCardAnim(playerID));

                        cardToFillIdx = nextHandIdx;

                        hand[cardToFillIdx] = drawCard.cardData.cardID;
                        drawCard.cardData.deckIdx = -1;
                        drawCard.cardData.status = CardData.Status.Hand;
                        drawCard.cardData.hand = playerID;
                        drawCard.cardData.handIdx = cardToFillIdx;
                        nextHandIdx += 1;
                        gControl.curTalentCardsIdx += 1;

                        //pause after card drawn but before align
                        //yield return new WaitForSeconds(0.5f);
                        if (playerType == PlayerType.Computer) { drawCard.GetComponent<Rigidbody>().isKinematic = true; }

                        yield return StartCoroutine("ProcessAlignHand");

                        //pause to wait for next player turn
                        playerActed = true;

                    }

                    break;
                case PlayerAction.MakeMovie:

                    MakeMovie();

                    yield return new WaitUntil(() => playerActed == true);
                    playerActed = false;

                    StackMovieCards();
                    yield return new WaitUntil(() => playerActed == true);
                    nextMovieIDX += 1;

                    break;
            }

        }
        yield return new WaitForSeconds(0.5f);
        playerActed = false;
        gControl.curPlayer += 1;
        if (gControl.curPlayer >= gControl.playerCount) { gControl.curPlayer = 0; }
        transform.GetChild(1).gameObject.GetComponent<Renderer>().material = origMaterial;
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
        else if (actorCnt > 2)
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

}


