using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;

public enum GameStates { Start, LeftWay, CenterWay, RightWay, Death }
public class PlayerController : MonoBehaviour
{
    public float jumpForce;
    public float distanceMove;
    public float distanceUp;
    public float climbingDistance;

    [SerializeField] private Animator chAnimator;
    [SerializeField] private CharacterController chController;
    [SerializeField] private Transform[] spawns;


    private float gravityForce;
    private int indexOfState = 2;
    private float distance;
    private float dir;
    private float speedMulti=1f;
    private UnityEngine.Vector3 direction;

    private bool movePlayer = false;
    private bool isAnimated = false;
    private GameStates states;

    public static PlayerController instance;

    private void Awake()
    {
        if (PlayerController.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        PlayerController.instance = this;

    }
    private void OnDestroy()
    {
        PlayerController.instance = null;
    }
    void Start()
    {        
        states = GameStates.Start;                                                                  //ставим состояние в Start
        StartCoroutine(StartGame());                                                                //через 2с начинаем игру
        StartCoroutine(SpeedUp());
    }
    /// <summary>
    /// корутин, чтобы остановить движение, после того, как закончился клип
    /// </summary>
    /// <param name="clipLength"></param>
    /// <returns></returns>
    IEnumerator StopMoving(float clipLength)                                                        
    {                                                                                               
        yield return new WaitForSeconds(clipLength);
        movePlayer = false;
    }
    /// <summary>
    /// корутин, чтобы отслеживать конец анимации
    /// </summary>
    /// <param name="clipLength"></param>
    /// <returns></returns>
    IEnumerator EndOfAnimation(float clipLength)                                                    
    {
        yield return new WaitForSeconds(clipLength);
        isAnimated = false;
    }
    /// <summary>
    /// Корутин для начала игры
    /// </summary>
    /// <returns></returns>
    IEnumerator StartGame()                                                                         //начинаем игру
    {
        yield return new WaitForSeconds(2f);
        chAnimator.SetTrigger("Start");
        states = GameStates.CenterWay;
    }
    IEnumerator SpeedUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            speedMulti += 0.1f;
            chAnimator.SetFloat("SpeedMulti", speedMulti);
        }
        

    }
    void Update()                                                               
    {
        if (states != GameStates.Start && states != GameStates.Death)                               //позволяем игроку что-то делать, если он не мертв или не в состоянии Start    
        {
            Raycasting();
            Gravity();                                                                              //метод для гравитации персонажа
            if (!movePlayer) PlayerAction();                                                        //если персонаж не в движении, то позволяем выбрать действие
            else
                if (chAnimator.GetCurrentAnimatorStateInfo(0).IsName("Climbing")) PlayerMoveUp(distanceUp / chAnimator.GetCurrentAnimatorStateInfo(0).length);
            else
            {
                direction = new UnityEngine.Vector3(spawns[indexOfState - 1].position.x -  transform.position.x, 0, 0); // вектор,направленный в точку, в которую должен прийти игрок
                PlayerMove(direction.x*2 / chAnimator.GetCurrentAnimatorStateInfo(0).length); //если стоит флаг на движение, то передвигаем персонажа
            }
        }
        if (states == GameStates.Death)
        {
            chAnimator.SetTrigger("Dead");
            StopAllCoroutines();
            
        }

    }
    private void Raycasting()
    {
        RaycastHit hit;
        UnityEngine.Vector3 point = transform.position;
        point.y += 0.5f*point.y;
        Ray climbingRay = new Ray(point, UnityEngine.Vector3.forward);
        if (Physics.Raycast(climbingRay, out hit, climbingDistance))
        {
            if (hit.collider.CompareTag("Second Flor"))
            {
                Climbing();
                Debug.Log("Get second floor");
            }
        }
    }
    /// <summary>
    /// Метод для выбора действий
    /// </summary>
    private void PlayerAction()                                                                     //выбор действий
    {
        dir = Input.GetAxisRaw("Horizontal");
        if (chAnimator.GetCurrentAnimatorStateInfo(0).IsName("Run"))                                //позволяем отпрыгнуть влево/вправо, если персонаж бежит
        {
            if (Input.GetKeyDown(KeyCode.A) && states != GameStates.LeftWay && !isAnimated) StepAway("StepLeft", --indexOfState);
            if (Input.GetKeyDown(KeyCode.D) && states != GameStates.RightWay && !isAnimated) StepAway("StepRight", ++indexOfState);
        }
        if (Input.GetKeyDown(KeyCode.S) && !isAnimated)                                             //проигрываем анимацию переката и выставляем тригер для анимации
        {
            chAnimator.SetTrigger("Roll");
            isAnimated = true;
            StartCoroutine(EndOfAnimation(chAnimator.GetCurrentAnimatorStateInfo(0).length));
        }
        chController.Move(new UnityEngine.Vector3(0, gravityForce, 0) * Time.deltaTime);            //постоянное воздействие гравитации на персонажа
        
    }
    /// <summary>
    /// Метод для передвижения персонажа в сторону
    /// </summary>
    /// <param name="animationaName">Название анимации</param>
    /// <param name="Stateindex">Индекс состояния</param>
    private void StepAway(string animationaName, int Stateindex)
    {
        chAnimator.SetTrigger(animationaName);                                                      //возвпроизводим анимациюю с названием animationName
        states = (GameStates)Stateindex;                                                            //выставляем индекс состояния
        movePlayer = true;                                                                          //ставим флаги на передвижение и отлавливания конца анимации
        isAnimated = true;
        StartCoroutine(EndOfAnimation(chAnimator.GetCurrentAnimatorStateInfo(0).length));
        StartCoroutine(StopMoving(chAnimator.GetCurrentAnimatorStateInfo(0).length));
    }
    /// <summary>
    /// Метод для передвижения персонажа в сторону
    /// </summary>
    /// <param name="speed">Скорость</param>
    private void PlayerMove(float speed )
    {
        chController.Move(UnityEngine.Vector3.right * dir * Math.Sign(direction.x) * speed * Time.deltaTime);          //передвигаем персонажа на растояниее direction со скоростью speed
    }
    private void PlayerMoveUp(float speed)
    {
        chController.Move(UnityEngine.Vector3.up * speed * Time.deltaTime);          //передвигаем персонажа на растояниее direction со скоростью speed
    }
    /// <summary>
    /// Метод гравитации
    /// </summary>
    private void Gravity()
    {
        if (!chController.isGrounded)                                                               //если персонаж не на земле, то выставляем флаг = false у аниматора
        {                                                                                           //и задаем вектор движения вниз
            gravityForce -= 20f * Time.deltaTime;
            chAnimator.SetBool("Grounded", false);
            
        }
        else                                                                                        //если на земле
        {
            gravityForce = -1f;
            chAnimator.SetBool("Grounded", true);
        }
        if (Input.GetKeyDown(KeyCode.Space) && chController.isGrounded && !isAnimated)              //если можно прыгнуть и нажат пробел
        {                                                                                           //выставляем тригер и флаг
            gravityForce = jumpForce;
            chAnimator.SetTrigger("Jump");
            isAnimated = true;
            StartCoroutine(EndOfAnimation(chAnimator.GetCurrentAnimatorStateInfo(0).length));
        }
    }
    private void Climbing()
    {
        chAnimator.SetTrigger("Climbing");
        isAnimated = true;
        movePlayer = true;
        StartCoroutine(StopMoving(chAnimator.GetCurrentAnimatorStateInfo(0).length));
        StartCoroutine(EndOfAnimation(chAnimator.GetCurrentAnimatorStateInfo(0).length));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dead") && states!=GameStates.Death)
        {
            chAnimator.SetTrigger("Hit");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Back")) chController.Move(5*transform.forward * Time.deltaTime);    //если персонаж застрял
    }
    public void SetStatus(int index)
    {
        states = (GameStates)index;
    }
    public bool isDead()
    {
        if (states == GameStates.Death) return true;
        else return false;
    }    
    public float GetFactor()
    {
        return speedMulti;
    }
}
