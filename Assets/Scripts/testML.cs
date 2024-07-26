using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
public class starterAgent : Agent
{
    Rigidbody2D body;
    
    float horizontal;
    float vertical;

    public float runSpeed = 20.0f;
    public int type = 0;
    private float speed;
    private float orientation;
    private float raycastLength = (float)2;
    public int maxFriendly = 0;
    private float raycastOffsetLength = (float).7;
    private Vector2 raycastToThrow;
    private Vector3 raycastOffset;
    private float[] inputArr;
    private float previousDistance;
    public float[] outwardComms;
    private Vector3 target;
    int[] rayAngles = new int[] {90,60,30,0,-30,-60,-90 };

    string[] tagsAccepted = new string[] {"Drone","Enemy"};
    void Start ()
    {

        body = GetComponent<Rigidbody2D>();
    }
    public override void OnEpisodeBegin()
    {
        Debug.Log("Reset");
        transform.position = new Vector3(Random.Range(-20f, -15f), Random.Range(-8f, 8f),0);
        target= new Vector3(Random.Range(10f, 20f), Random.Range(-8f, 8f),0);
        Debug.Log(target);
        previousDistance = -Vector3.Distance(transform.position,target);
        orientation =Random.Range(0f, 360f);
        body.rotation = orientation;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector3)transform.position);
        sensor.AddObservation((Vector3)target);
        //sensor.AddObservation((Vector1)target);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log(actions.ContinuousActions.Length);
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];
        orientation += horizontal;
        speed = Mathf.Clamp(speed+moveY,-5,5);
        body.velocity = new Vector2(-Mathf.Sin(orientation/180*(Mathf.PI))*speed, Mathf.Cos(orientation/180*(Mathf.PI))*speed);
        float newDistance = -Vector3.Distance(transform.position,target);
        AddReward(newDistance-previousDistance);
        if (newDistance>0.5f){
            Debug.Log("Win");
            OnEpisodeBegin();
        }
        previousDistance= newDistance;
    }
    public void DroneStruck(){
        AddReward(-10f);
        Debug.Log("Death");
        OnEpisodeBegin();
    }
    void Update()
    {
        RequestDecision();
        //Gives a value between -1 and 1
        // horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        // orientation -= (float)horizontal;
        // vertical = Input.GetAxisRaw("Vertical"); // -1 is down
        // speed += vertical *1;
        // speed = Mathf.Clamp(speed,-5,5);
        // float newDistance = -Vector3.Distance(transform.position,target);
        // AddReward(newDistance-previousDistance);
        // previousDistance= newDistance;

    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        float moveX = -Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        orientation += moveX;
        speed = Mathf.Clamp(speed+moveY,-5,5);
        body.rotation = orientation;
        body.velocity = new Vector2(-Mathf.Sin(orientation/180*(Mathf.PI))*speed, Mathf.Cos(orientation/180*(Mathf.PI))*speed);
    }
    void FixedUpdate()
    {

    }   
    public float[] runRL(float[] friendlyInput){
        inputArr = new float[100];
        outwardComms = new float[3];
        body = GetComponent<Rigidbody2D>();
        inputArr[0] = body.position.x;
        inputArr[1] = body.position.y;
        inputArr[2] = 0f;
        for (int i = 0;i < 7; i++){
            raycastOffset = new Vector3(-Mathf.Sin((rayAngles[i]+orientation)/180*(Mathf.PI))*raycastOffsetLength,Mathf.Cos((rayAngles[i]+orientation)/180*(Mathf.PI))*raycastOffsetLength,0);
            raycastToThrow = new Vector2(-Mathf.Sin((rayAngles[i]+orientation)/180*(Mathf.PI)),Mathf.Cos((rayAngles[i]+orientation)/180*(Mathf.PI)));
            //raycastOffset = raycastToThrow * raycastOffsetLength;
            RaycastHit2D hit = Physics2D.Raycast(transform.position+raycastOffset, raycastToThrow,raycastLength);
            if (hit){
                inputArr[i*(tagsAccepted.Length+1)+System.Array.IndexOf(tagsAccepted,hit.collider.tag)+3]=1;
                inputArr[i*(tagsAccepted.Length+1)+tagsAccepted.Length+3]=hit.distance;
                //Debug.Log(hit.collider.tag=="Drone");
                Debug.Log(string.Format("Hit Position: {0} Hit Distance: {1} Index : {2}", hit.point, hit.distance,i*(tagsAccepted.Length+1)+System.Array.IndexOf(tagsAccepted,hit.collider.tag)+3));
            }else{
                // for (int j =0;j<(tagsAccepted.length+1);j++){
                //     inputArr[i*(tagsAccepted.length+1)+j]=0;
                // }
            }
            Debug.DrawRay(transform.position+raycastOffset,raycastToThrow*raycastLength,Color.green);
        }
        body.velocity = new Vector2(-Mathf.Sin(orientation/180*(Mathf.PI))*speed, Mathf.Cos(orientation/180*(Mathf.PI))*speed);
        body.rotation = orientation;
        
        Vector3 tmpPos = transform.position;
        tmpPos.x = Mathf.Clamp(tmpPos.x,-21.25f,21.25f);
        tmpPos.y = Mathf.Clamp(tmpPos.y,-9.25f,9.25f);
        transform.position = tmpPos;
        outwardComms[0] = 1;
        outwardComms[1] = Random.Range(0f,1f);
        outwardComms[2] = Random.Range(0f,1f);
        return outwardComms;
    }
}
