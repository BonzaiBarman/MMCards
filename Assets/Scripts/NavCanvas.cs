using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NavCanvas : MonoBehaviour
{
    
	[SerializeField] Canvas canvas1;
	[SerializeField] Canvas canvas2;
	// Start is called before the first frame update
    void Start()
    {
	    canvas1.enabled = true;
	    canvas2.enabled = false;
	    canvas2.transform.DOScale(new Vector3(0,0,0),0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	public void SetKin()
	{
		Card[] daCards = FindObjectsOfType<Card>();
		foreach (Card item in daCards)
		{
			if (item.cardData.type == CardData.CardType.Action)
			{
				//count = count + 0.03f;
				//item.transform.DOMoveY(count,0);
				item.GetComponent<Rigidbody>().isKinematic = true;
			}
		}
	}
    
	public void Navigate(string inCommand)
	{

		switch (inCommand)
		{
			case "Close1_Open2":

				canvas1.enabled = false;
				canvas1.transform.DOScale(new Vector3(0,0,0),0);
				canvas2.enabled = true;
				canvas2.transform.DOScale(new Vector3(1,1, 1), 1.5f);

				break;
			case "Open1_Close2":
				canvas2.enabled = false;
				canvas2.transform.DOScale(new Vector3(0,0,0),0);
				canvas1.enabled = true;
				canvas1.transform.DOScale(new Vector3(1,1, 1), 1.5f);
				break;
		}
	}
}
