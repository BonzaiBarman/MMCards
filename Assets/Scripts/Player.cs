using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Player : MonoBehaviour
{
    
	public Material goldMaterial;
	public Material redMaterial;
	public Material greenMaterial;
	public Material blueMaterial;
	public TextMeshPro playerName;
	public TextMeshPro scoreText;
	public int score;
	
	public int[] hand = new int[] {-1, -1, -1, -1, -1, -1, -1};
	public int nextHandIdx = 0;
	GameControl gControl;
	// Start is called before the first frame update
    void Start()
    {

    	Debug.Log(hand.Length);
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
	
}
