using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Name : MonoBehaviour
{

    public Material goldMaterial;
    public Material redMaterial;
    // Start is called before the first frame update
    void Start()
    {
        //iTween.RotateTo(gameObject, new Vector3(55, 0, 0), 5f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeBackgroundMaterial(string inMaterial)
    {
        GameObject back = transform.GetChild(1).gameObject;
        if (inMaterial == "gold")
        {
            //back.GetComponent<Renderer>().material = goldMaterial;
            back.SetActive(false);
        }
        else if (inMaterial == "red")
        {
            back.GetComponent<Renderer>().material = redMaterial;
        }
        else
        {
            back.GetComponent<Renderer>().material = redMaterial;
        }
	    //iTween.MoveTo(gameObject, iTween.Hash("y", 0.05, "x", transform.position.x - 0.15));
	    transform.DOMoveY(0.03f, 0.8f, false);
}

    public void ChangeName(string inName)
    {
        TextMeshPro daText = transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
	    daText.text = inName;
	    //daText.DOText(inName, 2f, true, ScrambleMode.All);
    }
    
	public void RunAnimation(string inAnimName)
	{
		if (inAnimName == "rotate")
		{
			DOTween.Play("NameRot");

		}
	}

}
