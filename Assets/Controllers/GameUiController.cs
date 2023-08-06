using Assets.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUiController : MonoBehaviour
{
	private string currentScoreText
	{
		set { SetTextValue(scoreObject, value); }
	}
	private string currentLinesClearedText
	{
		set { SetTextValue(linesClearedObject, value); }
	}
	public Text linesClearedObject;
	public Text scoreObject;

	private void SetTextValue(Text textObject, string textValue)
	{
		textObject.text = textValue;
	}

	public void SetLinesClearedText(int linesCleared)
	{
		currentLinesClearedText = UiStrings.linesClearedTitle + linesCleared.ToString();
	}
	public void SetScoreText(int currentScore)
	{
		currentScoreText = UiStrings.scoreTitle + currentScore.ToString();
	}
}
