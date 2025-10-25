using System.Collections;
using UnityEngine;

public class GoForward : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GoForwardObject());
    }

    private IEnumerator GoForwardObject()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            transform.position += transform.forward;
        }
    }
}
