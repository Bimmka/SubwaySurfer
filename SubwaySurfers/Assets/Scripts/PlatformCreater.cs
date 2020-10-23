using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCreater : MonoBehaviour
{
    public float speed;

    [SerializeField] private GameObject startPlatform;
    [SerializeField] private GameObject easyPlatform;
    [SerializeField] private float[] trapsCost;                    //массив со стоимосью ловушек
    [SerializeField] private float[] giftsCost;                    //массив со стоимостью подарков

    public float midRangeAnchor = 200;                            //среднее значение якоря
    public float rangeAnchor = 20;                                 //диапазон якоря
    public float valueAnchor = 200;                               //текущее значение якоря

    private Vector3 endPoint;
    
    private float time;
    private bool flag = false;
    private bool dead = false;

    public static PlatformCreater instance;

    private void Awake()
    {
        if (PlatformCreater.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        PlatformCreater.instance = this;
    }
    private void OnDestroy()
    {
        PlatformCreater.instance = null;
    }
    void Start()
    {        
        Instantiate(startPlatform, transform.position, Quaternion.identity, transform);                                             //создаем стартовую платформу
        StartCoroutine(StartMove());                                                                                                //через 3с начинаем движение
        StartCoroutine(SpeedUp());                                                                                                  //корутина для увеличения скорости движения
        StartCoroutine(CreateSecondPlatform());                                                                                     //создаем вторую платформу
    }
    IEnumerator CreateSecondPlatform()
    {
        yield return new WaitForSeconds(4f);
        Instantiate(easyPlatform, endPoint, Quaternion.identity, transform);
    }
    IEnumerator StartMove()
    {
        yield return new WaitForSeconds(3f);
        flag = true;
    }
    IEnumerator SpeedUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(8f);
            speed += 0.1f;
        }
    }
    private void Update()
    {
        if (flag && !dead) transform.position -= Vector3.forward * speed * Time.deltaTime;
    }
    public void SetEndPoint(Vector3 coordinate)
    {
        endPoint = coordinate;
    }
    /// <summary>
    /// Метод,  сздающий начальную платформу
    /// </summary>
    public void CreatePlatform()
    {
        Instantiate(easyPlatform, endPoint, Quaternion.identity, transform);
    }
    /// <summary>
    /// Метод, возвращающий среднее значение диапазона для якоря
    /// </summary>
    /// <returns></returns>
    public float GetMidRangeAnchor()
    {
        return midRangeAnchor;
    }
    /// <summary>
    /// Метод, возращающий текущее значение якоря
    /// </summary>
    /// <returns></returns>
    public float GetValueAnchor()
    {
        return valueAnchor;
    }
    /// <summary>
    /// Метод, возвращающий диапазон для якоря
    /// </summary>
    /// <returns></returns>
    public float GetRangeAnchor()
    {
        return rangeAnchor;
    }
    /// <summary>
    /// Метод, возвращающий индекс ловушки с такой ценой
    /// </summary>
    /// <param name="closeValue">Цена ловушки</param>
    /// <returns></returns>
    public int GetTrapCostIndex(float closeValue)
    {
        return Array.IndexOf(trapsCost, closeValue);
    }
    /// <summary>
    /// Метод, возвращающий индекс подарка с такой ценой
    /// </summary>
    /// <param name="closeValue">Цена подарка</param>
    /// <returns></returns>
    public int GetGiftCostIndex(float closeValue)
    {
        return Array.IndexOf(giftsCost, closeValue);
    }
    /// <summary>
    /// Метод для поиска ближайшей стоимости ловушки, если текущнее значение якоря больше верхней допустимой границы
    /// </summary>
    /// <param name="value">Значение, на которое текущее значение якоря больше среднего значения якоря</param>
    /// <returns></returns>
    public float SearchBottomCloseValue(float value)
    {
        float closeValue = value - trapsCost[0];
        for (int i = 1; i < trapsCost.Length; i++)
        {
            if (value - trapsCost[i] > closeValue) closeValue = value - trapsCost[i];    //находим наибольшую разность
        }
        return value - closeValue; //получаем стоимость ловушки
    }
    /// <summary>
    /// Метод для поиска ближайшей стоимости подарка, если текущее значение якоря меньше нижней допустимой границы
    /// </summary>
    /// <param name="value">Значение, на которое текущее значение якоря меньше среднего значения якоря</param>
    /// <returns></returns>
    public float SearchUpperCloseValue(float value)
    {
        float closeValue = value - giftsCost[0];
        for (int i = 0; i < giftsCost.Length; i++)
        {
            if (value - giftsCost[i] < closeValue) closeValue = value - giftsCost[i]; //находим наименьшуюю разность
        }
        return value - closeValue;  //получаем стоимость подарка
    }
    /// <summary>
    /// Метод для изменения значения якоря при добавлении ловушки
    /// </summary>
    /// <param name="trapIndex"></param>
    public void AddTraps(int trapIndex)
    {
        switch (trapIndex)
        {
            case 0: valueAnchor += trapsCost[trapIndex]; break;                                              //изменяем значение якоря на стоимость ловушки
            case 1: valueAnchor += trapsCost[trapIndex]; break;
            case 2: valueAnchor += trapsCost[trapIndex]; break;
            case 3: valueAnchor += trapsCost[trapIndex]; break;
        }
    }
    /// <summary>
    /// Метод для изменения значения якоря при добавлении подарка
    /// </summary>
    /// <param name="giftIndex"></param>
    public void AddGift(int giftIndex)
    {
        switch (giftIndex)
        {
            case 0: valueAnchor += giftsCost[giftIndex]; break;
            case 1: valueAnchor += giftsCost[giftIndex]; break;
        }
    }
    public void SetDead()
    {
        dead = true;
    }


}
