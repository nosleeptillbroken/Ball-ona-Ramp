using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Boundary : MonoBehaviour {

    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Debug.Log("You Lose.");
            SceneManager.LoadScene("_GameOver");
        }
    }

}
