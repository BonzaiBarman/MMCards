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
		bool firstEmpty = false;
		for (int i = inIndex; i < (hand.Length - 1); i++)
		{
			hand[i] = hand[i + 1];
			if (hand[i] == -1 && !firstEmpty)
			{
				nextHandIdx = i;
				firstEmpty = true;
			}
			if (hand[i] != -1)
			{
				gControl.GetTalentCardFromID(hand[i]).MoveCard(i);				
			}

			
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
