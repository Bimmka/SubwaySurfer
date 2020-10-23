using System;
using System.Collections;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    [SerializeField] private Transform endPoint;                   //конец платформы,чтобы знать, где создавать следующую
    [SerializeField] private Transform[] spawnPoint;               //места для спавна ловушек/подарков
    [SerializeField] private Transform[] secondFloorSpawns;        //места для спавна 2 этажа
    [SerializeField] private GameObject[] traps;                   //массив с ловушками
    [SerializeField] private GameObject[] gifts;                   //массив с подарками
    [SerializeField] private GameObject secondFloor;               //2 этаж
    [SerializeField] private float[] trapsCost;                    //массив со стоимосью ловушек
    [SerializeField] private float[] giftsCost;                    //массив со стоимостью подарков

    public bool isStart;

    private  float midRangeAnchor;                            //реднее значение якоря
    private  float rangeAnchor;                                 //диапазон якоря
    private  float valueAnchor;                               //текущее значение якоря

    private void Start()
    {
        if (!isStart) GenerateSomething();
        StartCoroutine(Action());
    }
    void Update()
    {
        PlatformCreater.instance.SetEndPoint(endPoint.position);
    }
    /// <summary>
    /// Метод для генерации подарков/ловушек
    /// </summary>
    private void GenerateSomething()
    {
        valueAnchor = PlatformCreater.instance.GetValueAnchor();
        midRangeAnchor = PlatformCreater.instance.GetMidRangeAnchor();
        rangeAnchor = PlatformCreater.instance.GetRangeAnchor();
        for (int i = 0; i < spawnPoint.Length; i++)
        {
            if (valueAnchor <= midRangeAnchor + rangeAnchor && valueAnchor >= midRangeAnchor - rangeAnchor) RandomGenerate(i); //если значение якоря в допустимом диапазоне, то создаем рандомно что-то
            else SpecialGenerate(i);                                                                                           //если не в диапазоне, то нужно вернуть якорь в диапазон
            valueAnchor = PlatformCreater.instance.GetValueAnchor();                                                            //обновляем текущее значение якоря
        }
        GenerateSecondFloor();
    }
    /// <summary>
    /// Метод для рандомной генерации ловушки/подарка, если якорь в допустимом диапазоне
    /// </summary>
    /// <param name="spawnIndex">Индекс места спавна</param>
    private void RandomGenerate(int spawnIndex)
    {
        if (UnityEngine.Random.Range(0, 2) == 0) GenerateGifts(spawnIndex);
        else GenerateTraps(spawnIndex);
    }
    /// <summary>
    /// Метод для восстановления значения якоря и создания либо подарка, либо ловушки
    /// </summary>
    /// <param name="spawnIndex">Индекс места спавна</param>
    private void SpecialGenerate(int spawnIndex)
    {
        if (valueAnchor < midRangeAnchor - rangeAnchor)                                     //если текущее значение якоря меньше нижней границы, то необходимо создать подарки, чтобы уравновесить якорь
        {
            float closeValue = PlatformCreater.instance.SearchUpperCloseValue(midRangeAnchor - valueAnchor);
            GenerateGifts(spawnIndex, PlatformCreater.instance.GetGiftCostIndex(closeValue));
            Debug.Log(closeValue);
        }
        else
        {
            float closeValue = PlatformCreater.instance.SearchBottomCloseValue(midRangeAnchor - valueAnchor);       //если текущее значение якоря больше нижней границы, то необходимо создать ловушки, чтобы уравновесить якорь
            GenerateTraps(spawnIndex, PlatformCreater.instance.GetTrapCostIndex(closeValue));
            Debug.Log(closeValue);
        }
        
    }
    /// <summary>
    /// Метод для генерации рандомной ловушки, если значение якоря в допустимом диапазоне
    /// </summary>
    /// <param name="spawnIndex">Индекс места спавна</param>
    private void GenerateTraps(int spawnIndex)
    {
        int randomNumber = UnityEngine.Random.Range(0, traps.Length);
        PlatformCreater.instance.AddTraps(randomNumber);
        Debug.Log($"Generate traps {randomNumber} and valueAnchor {valueAnchor}");
        Instantiate(traps[randomNumber], spawnPoint[spawnIndex].position, Quaternion.identity, transform);      //создаем ловушку
    }
    /// <summary>
    /// Метод для генерации ловушки, которая наиболее подходит по стоимости, чтобы восстановить значение якоря
    /// </summary>
    /// <param name="spawnIndex">Индекс места спавна</param>
    /// <param name="trapIndex">Индекс ловушки</param>
    private void GenerateTraps(int spawnIndex, int trapIndex)                       
    {
        PlatformCreater.instance.AddTraps(trapIndex);
        Debug.Log($"Generate traps with {trapIndex} and valueAnchor {valueAnchor}");
        Instantiate(traps[trapIndex], spawnPoint[spawnIndex].position, Quaternion.identity, transform);
    }
    /// <summary>
    /// Метод для генерации рандомного подарка, если значение якоря в допустимом диапазоне
    /// </summary>
    /// <param name="spawnIndex">Индекс места спавна</param>
    private void GenerateGifts(int spawnIndex)
    {
        int randomNumber = UnityEngine.Random.Range(0, gifts.Length);
        PlatformCreater.instance.AddGift(randomNumber);
        Debug.Log($"Generate gift {randomNumber} valueAnchor {valueAnchor}");
        Instantiate(gifts[randomNumber], spawnPoint[spawnIndex].position, Quaternion.identity, transform);
    }
    /// <summary>
    /// Метод для генерации подарка, который наиболее подходит по стоимости, чтобы восстановить значение якоря
    /// </summary>
    /// <param name="spawnIndex">Индекс места спавна</param>
    /// <param name="giftIndex">Индекс подарка</param>
    private void GenerateGifts(int spawnIndex, int giftIndex)
    {
        PlatformCreater.instance.AddGift(giftIndex);
        Debug.Log($"Generate gifts with {giftIndex} valueAnchor {valueAnchor}");
        Instantiate(gifts[giftIndex], spawnPoint[spawnIndex].position, Quaternion.identity, transform);
    }
    /// <summary>
    /// Метод для спавна второго уровня
    /// </summary>
    private void GenerateSecondFloor()
    {
        int thing = UnityEngine.Random.Range(0, 2);
        int index = UnityEngine.Random.Range(0, 3);
        if (1 == thing) Instantiate(secondFloor, secondFloorSpawns[index].position, Quaternion.identity, transform);
    }
    /// <summary>
    /// Метод для удаления/добавления платформы
    /// </summary>
    IEnumerator Action()
    {        
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (transform.position.z < -55f)
            {
                PlatformCreater.instance.CreatePlatform();
                Destroy(gameObject);
            }
        }   
    }


}
