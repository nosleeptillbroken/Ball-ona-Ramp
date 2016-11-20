using UnityEngine;
using System.Collections;

public class Boundary : MonoBehaviour {

    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Debug.Log("You Lose.");
            Destroy(other.gameObject);
            Debug.Break();
        }
    }

}
