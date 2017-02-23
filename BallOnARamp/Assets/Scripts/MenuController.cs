using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private static int numElements = 5;
    [SerializeField] private GameObject[] MenuObjects = new GameObject[numElements];
	// Use this for initialization

  
    public void StartPressed()
    {
        for (int i = 0; i < numElements; i++)
        {
            MenuObjects[i].GetComponent<CanvasScaler>().scaleFactor = 2;
        }
    }
}
