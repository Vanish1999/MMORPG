using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    public GameObject plane1;
    public GameObject plane2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BeChoose()
    {
        plane1.SetActive(false);
        plane2.SetActive(true);
    }
    public void DonotChoose()
    {
        plane1.SetActive(true);
        plane2.SetActive(false);
    }
}
