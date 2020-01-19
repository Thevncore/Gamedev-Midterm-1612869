using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(Handler);
    }

    private void Handler()
    {
        //Debug.Log("Clicked!");

        var ball = GameObject.FindGameObjectWithTag("Ball");
        var bc = ball.GetComponent<BallController>();
        bc.CreateSputnik();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
