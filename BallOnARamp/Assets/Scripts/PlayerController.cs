using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    TileSpawner ts;
    Dictionary<string, System.Action> actions = new Dictionary<string, System.Action>();

    void Start()
    {
        ts = FindObjectOfType<TileSpawner>();
        actions.Add("jump", jump);
        actions.Add("dash", dash);

    }

     void Update()
    {
        // Get the info of the current tile
        TileInfo currentInfo = ts.ActiveTiles.First.Value.GetComponent<TileInfo>();

        // If the player hit space, do the thing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            actions[currentInfo.associatedAction]();
        }
    }

    void jump()
    {
        Debug.Log("Jumping!");

    }

    void dash()
    {
        Debug.Log("Dashing!");

    }
}
