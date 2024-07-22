using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyControl : MonoBehaviour
{
    Rigidbody2D body;
    
    float horizontal;
    float vertical;
    public float type = 1;
    public float runSpeed = 20.0f;
    private float speed;
    private float orientation;
    private float raycastLength = (float)2.5;
    private float raycastOffsetLength = (float)0.25;
    private Vector2 raycastToThrow;
    private Vector3 raycastOffset;
    private float[] inputArr;
    int[] rayAngles =  new int[] {90,60,30,0,-30,-60,-90 };
    
    void Start ()
    {

        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        orientation -= (float)horizontal*.3f;
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down
        speed += vertical *1;
        speed = Mathf.Clamp(speed,-5,5);
    }

    void FixedUpdate()
    {   
        
        // for (int i = 0;i < 7; i++){
        //     raycastOffset = new Vector3(-Mathf.Sin((rayAngles[i]+orientation)/180*(Mathf.PI))*raycastOffsetLength,Mathf.Cos((rayAngles[i]+orientation)/180*(Mathf.PI))*raycastOffsetLength,0);
        //     raycastToThrow = new Vector2(-Mathf.Sin((rayAngles[i]+orientation)/180*(Mathf.PI))*raycastLength,Mathf.Cos((rayAngles[i]+orientation)/180*(Mathf.PI))*raycastLength);
        //     raycastOffset = raycastToThrow * raycastOffsetLength;
        //     RaycastHit2D hit = Physics2D.Raycast(transform.position+raycastOffset, raycastToThrow);
        //     Debug.DrawRay(transform.position+raycastOffset,raycastToThrow,Color.green);
        // }
        body.velocity = new Vector2(-Mathf.Sin(orientation/180*(Mathf.PI))*speed, Mathf.Cos(orientation/180*(Mathf.PI))*speed);
        body.rotation = orientation;

        Vector3 tmpPos = transform.position;
        tmpPos.x = Mathf.Clamp(tmpPos.x,-21.25f,21.25f);
        tmpPos.y = Mathf.Clamp(tmpPos.y,-9.25f,9.25f);
        transform.position = tmpPos;
    }   
}
