using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int forceFactor = 300;

    void Start()
    {

    }

    void Update()
    {

    }

    public void PushLeft()
    {
        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.AddForce(Vector2.left * forceFactor);
    }
    public void PushRight()
    {
        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.AddForce(Vector2.right * forceFactor);
    }
    public void PushUp()
    {
        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.AddForce(Vector2.up * forceFactor);
    }
    public void PushDown()
    {
        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.AddForce(Vector2.down * forceFactor);
    }
}

