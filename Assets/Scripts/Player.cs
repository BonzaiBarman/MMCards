using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;

public class Player : MonoBehaviour
{
	public enum PlayerType
	{
		Human,
		Computer,
	}
	public enum PlayerAction
	{
		DrawTalent,
		DrawTalentDiscard,
		DrawAction,
		MakeMovie
	}
	
	public int playerID;
	public PlayerType playerType;
	public Material goldMaterial;
	public Material redMaterial;
	public Material greenMaterial;
	public Material blueMaterial;
	public TextMeshPro playerName;
	public TextMeshPro scoreText;
	public int score;
	public bool playerActed = false;
	public int discardedCardIdx = 0;
	public int holdCardID = 0;
	public PlayerAction playerAction =	PlayerAction.DrawTalent;
	
	public int[] hand = new int[] {-1, -1, -1, -1, -1, -1, -1};
	public int nextHandIdx = 0;
	
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

    	//Debug.Log(hand.Length);
	    gControl = FindObjectOfType<GameControl>();
	    //playerName.text = "Waldorf";
	    score = 0;
	    scoreText.text = "Score: " + score;
	    //ChangeBackgroundMaterial("green");

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
					gControl.GetTalentCardFromID(hand[i]).MoveCard(0,i);				
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
		Debug.Log("Player Turn: " + playerName.text);

		yield return new WaitUntil(() => playerActed == true);
		switch (playerAction)
		{
		case	PlayerAction.DrawAction:
			StartCoroutine("DrawAction");
			break;
		case	PlayerAction.DrawTalent:
			StartCoroutine("DrawTalent");
			break;
		case	PlayerAction.DrawTalentDiscard:
			playerActed = false;
			StartCoroutine("DrawTalentDiscard");
			yield return new WaitUntil(() => playerActed == true);
			break;
		case PlayerAction.MakeMovie:
			break;
		}
		
		//yield return new WaitUntil(() => playerActed == true);
		
		playerActed = false;
		gControl.curPlayer += 1; //testing, not good for real
		if (gControl.curPlayer >= gControl.playerCount){gControl.curPlayer = 0;}
	}

	IEnumerator ComputerTurn()
	{
		Debug.Log("Player Turn: " + playerName.text);
		
		while (!playerActed)
		{
			//pause before turn
			yield return new WaitForSeconds(0.5f);
			
			//Determine What Computer Player Does
			int rnd = Random.Range(0,0); //2
			switch (rnd)
			{
			case 0:
				//draw a talent card
				Card drawCard;
				Card discardCard;
				int cardToFillIdx;
				drawCard = gControl.GetTalentCardFromID(gControl.GetNextTalentCardID());
				Debug.Log(drawCard.cardData.cardName);


				if (nextHandIdx >= 7)
				{
					if(playerType == PlayerType.Computer){drawCard.GetComponent<Rigidbody>().isKinematic = false;}
					drawCard.cardData.handIdx = 99;
					drawCard.DealCardAnim(playerID, nextHandIdx);
					cardToFillIdx = ComputerDetermineDiscard();
					//Debug.Log(cardToFillIdx);
					//Debug.Log(gControl.GetTalentCardFromID(hand[cardToFillIdx]).cardData.cardName);
					nextHandIdx -= 1;
					//throw away card
					//Needs tobe done
					//***********
					discardCard = gControl.GetTalentCardFromID(hand[cardToFillIdx]);
					Debug.Log(discardCard.cardData.cardName);
					if(playerType == PlayerType.Computer){discardCard.GetComponent<Rigidbody>().isKinematic = false;}
					
					discardCard.DiscardTalentCard();
					
				}
				else
				{
					drawCard.DealCardAnim(playerID, nextHandIdx);
					cardToFillIdx = nextHandIdx;
				}

				//Debug.Log("card to fill: " + cardToFillIdx);

				hand[cardToFillIdx] = drawCard.cardData.cardID; //gControl.GetNextTalentCardID();
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
				yield return new WaitForSeconds(1f);
				playerActed = true;
				break;
			case 1: 
				break;
			case 2:
				break;
			}
			
			


		}
		playerActed = false;
		gControl.curPlayer += 1;
		if (gControl.curPlayer >= gControl.playerCount){gControl.curPlayer = 0;}
	}
	
	IEnumerator DrawTalent()
	{

		//playerActed = false;
		yield return new WaitForSeconds(0.1f);
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
				discardedCardIdx = -1;
				holdCardID = -1;
				//playerAction =	PlayerAction.DrawTalent;
			}
			else
			{
				//Debug.Log(discardedCardIdx);
				//so be it
				//Debug.Log("nexthandidx: " + nextHandIdx);
				//nextHandIdx = 6;
				CompactHand(discardedCardIdx);
				Debug.Log("nexthandidx: " + nextHandIdx);
				hand[nextHandIdx] = holdCardID;
				gControl.GetTalentCardFromID(holdCardID).MoveCard(playerID, nextHandIdx);
				nextHandIdx += 1;
				discardedCardIdx = -1;
				holdCardID = -1;
			}
			playerActed = true;
			yield return new WaitForSeconds(0.8f);			
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
	
	
	
	
}


