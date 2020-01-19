using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerControl : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        animator.Play("BouncerNormal");
    }
}
