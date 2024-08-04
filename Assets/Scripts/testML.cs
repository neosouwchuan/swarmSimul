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
    private GameObject targetObj;
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
    private float newDirection;
    private int maxStep = 300;
    private float reward = 0;
    private int currStep = 0;
    private float maxTimePenalty = 1f;
    private Vector3 target;
    int[] rayAngles = new int[] {90,60,30,0,-30,-60,-90 };
    private int Mod(int a, int n) => (a % n + n) % n;
    string[] tagsAccepted = new string[] {"Drone","Enemy"};
    void Start ()
    {
        targetObj = GameObject.Find("target");
        body = GetComponent<Rigidbody2D>();
    }
    public override void OnEpisodeBegin()
    {
        Debug.Log("Reset");
        transform.position = new Vector3(Random.Range(-20f, 20f), Random.Range(-8f, 8f),0);
        target= new Vector3(Random.Range(-20f, 20f), Random.Range(-8f, 8f),0);//Vector3(Random.Range(-20f, 20f), Random.Range(-8f, 8f),0);//new Vector3(Random.Range(10f, 20f), Random.Range(-8f, 8f),0);
        targetObj.transform.position=target;
        //Debug.Log(target);
        speed = 0;
        previousDistance = -Vector3.Distance(transform.position,target);
        orientation =Random.Range(0,360);//Random.Range(0f, 360f);
        body.rotation = orientation;
        body.velocity = new Vector2(0, 0);
        currStep = 0;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // sensor.AddObservation(transform.position);
        // sensor.AddObservation(target);//s

        sensor.AddObservation(transform.InverseTransformDirection(target-transform.position));//s
        sensor.AddObservation(speed);

        //sensor.AddObservation(new Vector2(-Mathf.Sin(orientation/180*(Mathf.PI))*speed, Mathf.Cos(orientation/180*(Mathf.PI))*speed));
        //sensor.AddObservation((Vector1)target);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        reward = 0;
        //Debug.Log(actions.ContinuousActions.Length);
        float moveX = actions.DiscreteActions[0]-1;
        float moveY = actions.DiscreteActions[1]-1;
        if (true){
            //orientation += Mathf.Clamp(moveX*10f,-10,10);   
            orientation += moveX * 5;
            
        }else{
            newDirection = moveX*180f;
            orientation = Mathf.Clamp(newDirection,orientation-5f,orientation+5f);
        }
        orientation=Mod((int)orientation,360);
        speed = Mathf.Clamp(speed+moveY*2,-5,5);
        body.velocity = new Vector2(-Mathf.Sin(orientation/180*(Mathf.PI))*speed, Mathf.Cos(orientation/180*(Mathf.PI))*speed);
        float newDistance = -Vector3.Distance(transform.position,target);
        body.rotation = orientation;
        reward += newDistance-previousDistance;
        // if (newDistance-previousDistance<0){
        //     reward += (newDistance-previousDistance) * 2f;
        // }else{
        //     reward += newDistance-previousDistance;
        // }
        reward -= (float)(currStep/maxStep) * maxTimePenalty;
        currStep +=1;
        // if (newDistance>previousDistance){
        //     AddReward(5f);
        // }else{
        //     AddReward(-5f);
        // }
        //Debug.Log(40+newDistance);
        if (newDistance>-1.50f){
            AddReward(20f);
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

        float moveX = Mathf.Clamp(-Input.GetAxisRaw("Horizontal"),-1f,1f);
        float moveY = Mathf.Clamp(Input.GetAxisRaw("Vertical"),-1f,1f);
        //Debug.Log(actions.ContinuousActions.Length);
        if (true){
            //orientation += Mathf.Clamp(moveX*10f,-10,10);   
            orientation += Mathf.Clamp(Mathf.RoundToInt(moveX)*5,-10,10);
        }else{
            newDirection = moveX*180f;
            orientation = Mathf.Clamp(newDirection,orientation-5f,orientation+5f);
        }
        orientation = Mod((int)orientation,360);
        speed = Mathf.Clamp(speed+moveY,-5,5);
        body.rotation = orientation;
        body.velocity = new Vector2(-Mathf.Sin(orientation/180*(Mathf.PI))*speed, Mathf.Cos(orientation/180*(Mathf.PI))*speed);
        float newDistance = -Vector3.Distance(transform.position,target);
        AddReward(newDistance-previousDistance);
        Debug.Log(string.Format("TargetDir: {0} VelocityDir: {1}", transform.InverseTransformDirection(target-transform.position), transform.InverseTransformDirection(body.velocity)));
        // if (newDistance>previousDistance){
        //     AddReward(5f);
        // }else{
        //     AddReward(-5f);
        // }
        //Debug.Log(40+newDistance);
        if (newDistance>-1.50f){
            AddReward(20f);
            Debug.Log("Win");
            OnEpisodeBegin();
        }
        previousDistance= newDistance;
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
