using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    [SerializeField] private Text BestScorePrompt;

    private void Start()
    {
        int bestScore = PlayerPrefs.GetInt("HighScore", 0);
        if (bestScore == 0) BestScorePrompt.text = "Рекорд пока не установлен";
        else BestScorePrompt.text = "Рекорд: " + bestScore.ToString();
    }
}
