using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class Card : MonoBehaviour
{

	public CardData cardData;
	public int nextHandIdx = 0;
	GameControl gControl;

	Vector3[,] plyrHandLocs = new [,] {{new Vector3(-2.1f, 2f, -3.2f), new Vector3(-0.8f, 2f, -3.2f), new Vector3(0.5f, 2f, -3.2f), new Vector3(1.8f, 2f, -3.2f), new Vector3(3.1f, 2f, -3.2f), new Vector3(4.4f, 2f, -3.2f), new Vector3(5.7f, 2f, -3.2f)},
	{new Vector3(-3f, 0.1f, 2.9f), new Vector3(-3f, 0.1f, 2.2f), new Vector3(-3f, 0.1f, 1.5f), new Vector3(-3f, 0.1f, 0.8f), new Vector3(-3f, 0.1f, 0.1f), new Vector3(-3f, 0.1f, -0.6f), new Vector3(-3f, 0.1f, -1.3f)},
	{new Vector3(5.2f, 0.1f, 3.7f), new Vector3(4.5f, 0.1f, 3.7f), new Vector3(3.8f, 0.1f, 3.7f), new Vector3(3.1f, 0.1f, 3.7f), new Vector3(2.4f, 0.1f, 3.7f), new Vector3(1.7f, 0.1f, 3.7f), new Vector3(-0.3f, 0.1f, 3.7f)},
	{new Vector3(7.3f, 0.1f, -1.3f), new Vector3(7.3f, 0.1f, -0.6f), new Vector3(7.3f, 0.1f, -0.1f), new Vector3(7.3f, 0.1f, 0.8f), new Vector3(7.3f, 0.1f, 1.5f), new Vector3(7.3f, 0.1f, 2.2f), new Vector3(7.3f, 0.1f, 2.9f)}};

	
	// Start is called before the first frame update
    void Start()
    {
	    //cardData = Resources.Load<CardData>("CardSOs/Card1");
	    //transform.GetChild(0).GetComponent<Renderer>().material = cardData.frontMat;
	    //transform.GetChild(0).GetComponent<Renderer>().material = Resources.Load<Material>("TalentDeck/Materials/Talent4");
	    gControl = FindObjectOfType<GameControl>();
	}

    // Update is called once per frame
    void Update()
    {
        //if (GetComponent<Rigidbody>().velocity.magnitude < 0.5 && runOnce)
        //{
        //    if (Mathf.Abs(transform.rotation.eulerAngles.x + 180) < 2)
        //    {
        //        runOnce = false;
	    //        //Debug.Log("in x");
	    //        GetComponent<Rigidbody>().AddForce(0, 200, 0, ForceMode.Impulse);
	    //        GetComponent<Rigidbody>().AddTorque(100, 0, 0, ForceMode.Impulse);
        //    }
        //    else if (Mathf.Abs(transform.rotation.eulerAngles.z - 180) < 2)
        //    {
        //        runOnce = false;
	    //        //Debug.Log("in z");
	    //        GetComponent<Rigidbody>().AddForce(0, 200, 0, ForceMode.Impulse);
	    //        GetComponent<Rigidbody>().AddTorque(0, 0, 100, ForceMode.Impulse);
        //    }
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
	    //    GetComponent<Rigidbody>().isKinematic = true;
        //    runOnce = false;
	    //    transform.position = new Vector3(2f, 2f, -3.2f);
	    //    transform.rotation = Quaternion.Euler(350f, 0f, 0f);
        //}
	}

	void OnMouseDown()
    {

	    Debug.Log("CardID: " + cardData.cardID);
	    if (cardData.status == CardData.Status.Deck)
	    {
		    Debug.Log(cardData.cardName + ", " + cardData.deckIdx);
		    gControl.CardDraw(this);
		    if (cardData.type == CardData.CardType.Action)
		    {
			    StartCoroutine("DrawActionCardAnim");
			    GetComponent<Rigidbody>().isKinematic = true;
		    }	    
		    if (cardData.type == CardData.CardType.Talent)
		    {
			    StartCoroutine("DrawTalentCardAnim", 0);
			    GetComponent<Rigidbody>().isKinematic = true;
		    }	    	
	    }
	    else if (cardData.status == CardData.Status.Hand)
	    {
		    //Debug.Log(cardData.cardName);
		    gControl.CardDiscard(this);
		    if (cardData.type == CardData.CardType.Action)
		    {
			    GetComponent<Rigidbody>().isKinematic = false;
			    StartCoroutine("DiscardActionCardAnim");
			    GetComponent<Rigidbody>().isKinematic = true;
		    }	    
		    if (cardData.type == CardData.CardType.Talent)
		    {
			    GetComponent<Rigidbody>().isKinematic = false;
			    StartCoroutine("DiscardTalentCardAnim");
			    GetComponent<Rigidbody>().isKinematic = true;
		    }	    	
	    }

	    
	    //Destroy(gameObject);
	    //transform.DORotate(new Vector3(0,0,0), 0.02f);
	    //transform.DOMoveY(2.5f, 0.01f);

	    //GetComponent<Rigidbody>().isKinematic = false;
        //runOnce = true;
	    //GetComponent<Rigidbody>().AddForce(Random.Range(-5, 5), 1325, 1600, ForceMode.Impulse);

		//switch (Random.Range((int)1, (int)4))
        //{
        //    case 1:
        //        //flip
	    //        //GetComponent<Rigidbody>().AddTorque(Random.Range(200, 300), Random.Range(-10, 10), Random.Range(-10, 10), ForceMode.Impulse);
	    //        GetComponent<Rigidbody>().AddTorque(Random.Range(900, 1200), Random.Range(-10, 10), Random.Range(-10, 10), ForceMode.Impulse);
	    //        break;
        //    case 2:
        //        //spin
        //        transform.rotation = Quaternion.Euler(-5f, 0f, 0f);
	    //        //GetComponent<Rigidbody>().AddTorque(Random.Range(-3, 3), Random.Range(-150, 150), Random.Range(-3, 3), ForceMode.Impulse);
	    //        GetComponent<Rigidbody>().AddTorque(Random.Range(-3, 3), Random.Range(-300, 300), Random.Range(-3, 3), ForceMode.Impulse);
	    //        break;
        //    case 3:
        //    	//flat spin
	    //        GetComponent<Rigidbody>().AddTorque(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5), ForceMode.Impulse);
	    //        break;
        //    default:
        //        //flat
	    //        //GetComponent<Rigidbody>().AddTorque(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10), ForceMode.Impulse);
	    //        GetComponent<Rigidbody>().AddTorque(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), ForceMode.Impulse);
		//		break;
        //}
    }
	
	IEnumerator DrawActionCardAnim()
	{
		
		transform.DOMove(new Vector3(1.8f, 3f, -1.5f), 1);
		transform.DORotate(new Vector3(350f, 0f, 0f), 1);
		yield return new WaitForSeconds(1f);
		
	}
	IEnumerator DrawTalentCardAnim(int inPlayerIdx = 0)
	{
		
		//also based on cardData.Hand (not implemented) just doing player0
		Debug.Log("carddata.cardname " + cardData.cardName);
		transform.DOMove(plyrHandLocs[inPlayerIdx, cardData.handIdx], 1);
		//transform.DORotate(new Vector3(350f, 0f, 0f), 1);
		switch (inPlayerIdx)
		{
		case 0:
			transform.DORotate(new Vector3(350f, 0f, 0f), 1);
			break;
		case 1:
			transform.DORotate(new Vector3(0f, 90f, 0f), 1);
			break;
		case 2:
			transform.DORotate(new Vector3(0f, 180f, 0f), 1);
			break;
		case 3:
			transform.DORotate(new Vector3(0f, 270f, 0f), 1);
			break;
		}

		yield return new WaitForSeconds(1f);
		
	}
	IEnumerator DiscardActionCardAnim()
	{
		
		transform.DOMove(new Vector3(0.8f, 0.1f, 0f), 1);
		transform.DORotate(new Vector3(0f, 0f, 0f), 1);
		yield return new WaitForSeconds(1f);
		
	}
	IEnumerator DiscardTalentCardAnim()
	{
		
		//also based on cardData.Hand (not implemented) just doing player0
		
		transform.DOMove(new Vector3(4.3f, 0.1f, 0f), 1);
		transform.DORotate(new Vector3(0f, 0f, 0f), 1);

		yield return new WaitForSeconds(1f);
		
	}
	public  void DealCardAnim(int inPlayerIdx, int inHandIdx)
	{
		//transform.DOMove(plyrHandLocs[inPlayerIdx, inHandIdx], 0.1f);
		cardData.handIdx = inHandIdx;
		StartCoroutine("DrawTalentCardAnim", inPlayerIdx);
	}
	
	
	
	public void MoveCard(int inPlayer, int inHandIdx)
	{
		transform.DOMove(plyrHandLocs[inPlayer, inHandIdx], 0.1f);
	}
}
