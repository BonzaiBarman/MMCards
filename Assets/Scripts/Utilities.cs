using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
	public void ShuffleCards(Card[] inCards)
	{
		var cnt = inCards.Length;
		var last = cnt -1;
		for (var i = 0; i < last; i++)
		{
			var r = UnityEngine.Random.Range(i, cnt);
			var tmp = inCards[i];
			inCards[i] = inCards[r];
			inCards[i].cardData.deckIdx = i;
			inCards[r] = tmp;
			inCards[r].cardData.deckIdx = r;
		}
	}
	public void ShuffleCards(int[] inCards)
	{
		var cnt = inCards.Length;
		var last = cnt -1;
		for (var i = 0; i < last; i++)
		{
			var r = UnityEngine.Random.Range(i, cnt);
			var tmp = inCards[i];
			inCards[i] = inCards[r];
			inCards[r] = tmp;
		}
	}
}
