using Assets.Data;
using TMPro;
using UnityEngine;

public class WelcomeMenuUiController : MonoBehaviour
{
	private string currentHighScoreText
	{
		set { SetTextValue(highScoreObject, value); }
	}
	private string currentDifficultyLevelText
	{
		set { SetTextValue(difficultyLevelObject, value); }
	}
	public TextMeshProUGUI difficultyLevelObject;
	public TextMeshProUGUI highScoreObject;

	private void SetTextValue(TextMeshProUGUI textObject, string textValue)
	{
		textObject.text = textValue;
	}

	public void SetDifficultySliderText(int difficultyLevel)
	{
		currentDifficultyLevelText = UiStrings.difficultyLevelTitle + difficultyLevel.ToString();
	}
	public void SetHighScoreText(int highScore)
	{
		currentHighScoreText = UiStrings.scoreTitle + highScore.ToString();
	}
}
