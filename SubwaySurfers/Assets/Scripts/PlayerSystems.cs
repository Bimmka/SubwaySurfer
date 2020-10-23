using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSystems : MonoBehaviour
{
    [SerializeField] private ParticleSystem stoneExplosion;
    [SerializeField] private ParticleSystem giftExplosion;
    [SerializeField] private ParticleSystem heartExplosion;
    [SerializeField] private int MyHealth;           //количество моих жизней
    [SerializeField] private int NowNumberOfLifes;  //максимальное количество жизней на данный момент
    [SerializeField] private Image[] MaxLives;      //максимальное возможное количество жизней
    [SerializeField] private Sprite EmptyHealth;    //пустая жизнь (значок(
    [SerializeField] private Sprite FullHealth;     //полная жизнь (значок)
    [SerializeField] private Text NumberOfGold;     //подсказка
    [SerializeField] private Text NumberOfGift;
    [SerializeField] private Text DeadPrompt;
    [SerializeField] private Text BestScorePrompt;
    [SerializeField] private GameObject RagDoll;
    private float Gold = 0;                               //количество золота
    private float Gifts = 0;
    private int bestScore;

    private void Start()
    {
        BestScorePrompt.enabled = false;
        bestScore = PlayerPrefs.GetInt("HighScore", 0);
        DeadPrompt.enabled = false;
    }
    void Update()
    {
        NumberOfGold.text = Gold.ToString();
        NumberOfGift.text = Gifts.ToString();
        HealthSystem();                                             //проверяем жизни
        if (MyHealth < 1)
        {
            PlayerController.instance.SetStatus(4);
            DeadPrompt.enabled = true;
            CalculateScore();
            PlatformCreater.instance.SetDead();
            PauseMenu.instance.SetDead();
            PlatformCreater.instance.StopAllCoroutines();
            Instantiate(RagDoll, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gold"))
        {
            stoneExplosion.Play();
            Destroy(other.gameObject);            
            Gold++;
        }
        if (other.CompareTag("Gift"))
        {
            Destroy(other.gameObject);
            giftExplosion.Play();
            Gifts++;
        }
        if (other.CompareTag("Health"))
        {
            MyHealth++;
            heartExplosion.Play();
            Destroy(other.gameObject);
        }
         if (other.CompareTag("Dead")) MyHealth--;

    }
    private void HealthSystem()
    {
        if (MyHealth > NowNumberOfLifes) MyHealth = NowNumberOfLifes;    //если подобрали жизни, то не увеличиваем сверх максимума
        for (int i = 0; i < MaxLives.Length; i++)
        {
            if (i < MyHealth) MaxLives[i].sprite = FullHealth;          //если счетчик меньше количества жизней, то этот элемент массива = картинке с полным сердцем
            else MaxLives[i].sprite = EmptyHealth;
            if (i < NowNumberOfLifes) MaxLives[i].enabled = true;
            else MaxLives[i].enabled = false;
        }
    }
    /// <summary>
    /// Метод для подсчета результата
    /// </summary>
    private void CalculateScore()
    {
        float factor = PlayerController.instance.GetFactor();
        double score = Gifts * 2 * factor + Gold * 1.2f * factor;
        score = Math.Round(score);
        int intscore = Convert.ToInt16(score);
        if (score > bestScore)
        {
            PlayerPrefs.SetInt("HighScore", intscore);
            BestScorePrompt.text = "Рекорд составляет:" + intscore.ToString();
            BestScorePrompt.enabled = true;
        }
        else
        {
            BestScorePrompt.text = "Рекорд составляет:" + bestScore.ToString();
            BestScorePrompt.enabled = true;
        }


    }
}
