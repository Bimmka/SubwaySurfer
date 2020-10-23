using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviour : MonoBehaviour
{
    [SerializeField] private ParticleSystem shine;
    public Vector3 rotateVector;
    public float rotateSpeed;
    private void Update()
    {
        transform.Rotate(rotateSpeed * rotateVector);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Destroy(this);
    }
}
