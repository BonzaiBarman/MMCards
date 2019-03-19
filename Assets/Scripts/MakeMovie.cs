using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MakeMovie : MonoBehaviour
{
	// OnMouseOver is called every frame while the mouse is over the GUIElement or Collider.
	void OnMouseOver()
	{
		Material mat;
		mat = transform.GetComponent<Material>();
		mat.color = Color.green;

	}		//TextMeshPro erg;
		//erg = transform.GetChild[0];
		//erg.GetComponent<TextMeshPro>().color = Color.green;
	// OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.
	void OnMouseDown()
	{
		Debug.Log("clicked");
	}
}
