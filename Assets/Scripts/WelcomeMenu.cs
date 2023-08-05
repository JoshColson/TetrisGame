using Assets.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Data;

public class WelcomeMenu : MonoBehaviour
{


    public Button newGameButton;
    public Slider difficultySlider;

    private int difficultyLevel;
    private int maxDifficulty = 5;
    private int minDifficulty = 1;
    public int highScore { get; private set; } = 0;
    private WelcomeMenuUiController uiController { get; set; }

	// Start is called before the first frame update
	void Start()
	{
        uiController = GameObject.FindWithTag(Tags.SceneController.ToString()).GetComponent<WelcomeMenuUiController>();

        difficultySlider.maxValue = maxDifficulty;
        difficultySlider.minValue = minDifficulty;
		difficultySlider.wholeNumbers = true;
		difficultySlider.onValueChanged.AddListener(delegate { SliderChangeCheck(); });
        newGameButton.onClick.AddListener(NewGameClick);
        SetHighScore();
	}

    private void NewGameClick()
    {
		NavigationController.SceneNavigate(SceneNames.Tetris);
    }

    private void SetHighScore()
    {
		highScore = GameOverData.highScore;
		uiController.SetHighScoreText(highScore);
    }

    private void SliderChangeCheck()
    {
        difficultyLevel = (int)difficultySlider.value;
        StartingData.difficultyLevel = difficultyLevel;
        GameOverData.difficultyLevel = difficultyLevel;
		uiController.SetDifficultySliderText(difficultyLevel);
    }

	// Update is called once per frame
	void Update()
    {
        
    }
}
