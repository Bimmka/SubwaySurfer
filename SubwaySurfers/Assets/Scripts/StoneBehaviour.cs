using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBehaviour : MonoBehaviour
{
    [SerializeField] private Rigidbody rbStone;
    [SerializeField] private float rotateSpeed;

    private Vector3 rotateVector;
    void Start()
    {
        StartCoroutine(Delete());
        rbStone.velocity = new Vector3(0, 0, -10);
        rotateVector = new Vector3(Random.Range(-5,0), Random.Range(0, 5), 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateSpeed * rotateVector);
    }
    IEnumerator Delete()
    {
        yield return new WaitForSeconds(20f);
        Destroy(gameObject);
    }
}
