using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private void Awake()
    {
        Instantiate(player, transform.position, Quaternion.identity);
    }
}
