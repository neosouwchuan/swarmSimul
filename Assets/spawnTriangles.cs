using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnTriangles : MonoBehaviour
{
    public GameObject baseDrone;
    public GameObject[] triangleArr;
    // Start is called before the first frame update
    public int numOfTriangles = 5;
    public int numOfFriendlies = 3;
    public int numOfComms = 3;
    private float[] returnValueOthers;
    private int chosenIndex;
    private float[,] oldFriendlyArr;// = new float[numOfTriangles, 3];
    private float[,] newFriendlyArr;// = new float[numOfTriangles, 3]; 
    private float[] inputArr;// = new float[numOfFriendlies*3];  
    //thisFireball = (GameObject)Instantiate(FireballPrefab, transform.position, transform.rotation);
    void Start()
    {
        oldFriendlyArr = new float[numOfTriangles, numOfComms]; 
        // 
        newFriendlyArr = new float[numOfTriangles, numOfComms]; 
        inputArr = new float[numOfFriendlies*numOfComms]; 
        triangleArr = new GameObject[numOfTriangles];
        for(int i = 0;i<numOfTriangles;i++){
            triangleArr[i] = (GameObject)Instantiate(baseDrone,new Vector3(-20.5f+6f*i,-8.82f,0f),Quaternion.identity);
            oldFriendlyArr[i,0] = 1;
            oldFriendlyArr[i,1] = 0;
            oldFriendlyArr[i,2] = 0;
        }

    }

    // Update is called once per frame
    void Update()
    {   
        
    }
    
    void FixedUpdate()
    {
        for(int k = 0;k<numOfComms;k++){

            for(int i = 0;i<numOfFriendlies;i++){
                for(int j = 0;j<numOfComms;j++){
                    chosenIndex = Random.Range(0,numOfTriangles);
                    // Debug.Log(i*numOfComms+j);
                    // Debug.Log(inputArr.Length);
                    //Debug.Log(string.Format("inputArr: {0} oldFriendlyArr: {1}j: {2}",i*numOfComms+j,chosenIndex,j));
                    inputArr[i*numOfComms+j] = oldFriendlyArr[chosenIndex,j];
                    
                }
                //System.Array.Copy(oldFriendlyArr[Random.Range(0,numOfTriangles),],0,inputArr,i*numOfFriendlies,numOfFriendlies);
            }

            returnValueOthers = triangleArr[k].GetComponent<playerControlTest>().runRL(inputArr);
            for(int j = 0;j<numOfComms;j++){
                newFriendlyArr[k,j] = returnValueOthers[j];     
            }
        }   
    }
}
