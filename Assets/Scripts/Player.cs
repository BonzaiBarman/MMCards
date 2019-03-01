using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
	public Card[] hand;
	public int nextHandIdx = 0;
	// Start is called before the first frame update
    void Start()
    {
	    hand = new Card[7];
	    foreach(Card crd in hand)
	    {
	    	crd.cardData.cardID = -1;
	    }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
