using System.Collections;
using System.Collections.Generic;
using Assets.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WelcomeMenu : MonoBehaviour
{
    const string difficultyWriting = "Difficulty Level: ";
    const string highScoreWriting = "High Score: ";

    public Button newGameButton;
    public Slider difficultySlider;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI highScoreText;
    private int difficultyLevel;
    private int maxDifficulty = 5;
    private int minDifficulty = 1;
    public int highScore { get; private set; } = 0;

    // Start is called before the first frame update
    void Start()
	{
        highScore = GameOverData.highScore;
        highScoreText.text = highScoreWriting + highScore.ToString();
        difficultySlider.maxValue = maxDifficulty;
        difficultySlider.minValue = minDifficulty;
		difficultySlider.wholeNumbers = true;
		difficultySlider.onValueChanged.AddListener(delegate { SliderChangeCheck(); });
        newGameButton.onClick.AddListener(NewGameClick);
	}

    private void NewGameClick()
    {
        SceneController.SceneNavigate(SceneNames.Tetris);
    }

    private void SliderChangeCheck()
    {
        difficultyLevel = (int)difficultySlider.value;
        StartingData.difficultyLevel = difficultyLevel;
        GameOverData.difficultyLevel = difficultyLevel;

		difficultyText.text = difficultyWriting+difficultyLevel.ToString();
    }

	// Update is called once per frame
	void Update()
    {
        
    }
}
