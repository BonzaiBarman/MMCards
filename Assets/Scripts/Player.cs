using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
	public int[] hand = new int[7];
	public int nextHandIdx = 0;
	GameControl gControl;
	// Start is called before the first frame update
    void Start()
    {
	    for(int i = 0; i < hand.Length; i++)
	    {
	    	hand[i] = -1;
	    }
	    
	    gControl = FindObjectOfType<GameControl>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	public void CompactHand(int inIndex)
	{
		switch (inIndex)
		{
		case 0:
			hand[0] = hand[1];
			hand[1] = hand[3];
			hand[3] = hand[5];
			hand[5] = -1;
			if (hand[0] != -1)
			{
				gControl.GetTalentCardFromID(hand[0]).MoveCard(0);
			}
			if (hand[1] != -1)
			{
				gControl.GetTalentCardFromID(hand[1]).MoveCard(1);
			}
			if (hand[3] != -1)
			{
				gControl.GetTalentCardFromID(hand[3]).MoveCard(3);
			}
			break;
		case 1:
			break;
		case 2:
			break;
		case 3:
			break;
		case 4:
			break;
		case 5:
			break;
		case 6:
			break;
		}
	}
	
	public int GetHandIndexFromcardID(int inCardID)
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
		return -1;
	}
	
}
