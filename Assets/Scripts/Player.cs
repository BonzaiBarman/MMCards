using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
	public List<Card> hand = new List<Card>();
	public int nextHandIdx = 0;
	// Start is called before the first frame update
    void Start()
    {
	    //hand = new Card[7];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
