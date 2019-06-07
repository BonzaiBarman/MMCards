using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum PlayerAction
{
    DrawTalent,
    DrawTalentDiscard,
    DrawActionCollect,
    DrawActionRaid,
    DrawActionSabotage,
    DrawActionTrade,
    DrawActionChaos,
    DrawActionRunOver,
    TradingTalent,
    RaidingTalent,
    SabotagingMovie,
    MakeMovie
}

public enum Screenplays
{
    Comedy,
    Drama,
    Horror,
    Musical,
    Western,
    Action
}

public enum Huds
{
	Game,
	Movie,
	Menu,
	StartGame,
	EndGame
}

public enum PlayerType
{
	Human,
	Computer
}

public enum PlayerDisplay
{
    Name,
    Background,
	Score,
	Fire
}

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
	
	public void RunAnimation(string inAnimName)
	{
		if (inAnimName == "rotate")
		{
			DOTween.Play("NameRot");
		}
	}
	
}
