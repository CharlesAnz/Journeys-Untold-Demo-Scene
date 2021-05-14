using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Detection : MonoBehaviour
{
    public Transform target;

    private void Start()
    {
        target = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Ally")
        {
            if(target == null) target = other.transform;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Ally")
        {
            target = null;
        }
    }
}
