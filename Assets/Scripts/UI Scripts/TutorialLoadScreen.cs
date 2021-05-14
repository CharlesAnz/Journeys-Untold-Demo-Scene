using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLoadScreen : MonoBehaviour
{

    private float startTime;

    [SerializeField]
    private float duration;
    // Start is called before the first frame update
    void Start()
    {
        startTime = duration;   
    }

    // Update is called once per frame
    void Update()
    {
        startTime -= Time.deltaTime;
        if (startTime <= 0) gameObject.SetActive(false);
    }
}
