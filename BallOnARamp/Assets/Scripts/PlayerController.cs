using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    TileSpawner TS;
    Rigidbody rb;
    Dictionary<string, System.Action> actions = new Dictionary<string, System.Action>();
    bool onGround = true;

    [SerializeField]
    private float jumpForce = 3f;
    [SerializeField]
    private float dashSpeed = 1.5f;
    [SerializeField]
    private float dashTime = 0.5f;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        TS = FindObjectOfType<TileSpawner>();

        actions.Add("jump", jump);
        actions.Add("dash", dash);
    }

     void Update()
    {
        // Get the info of the current tile
        TileInfo currentInfo = TS.ActiveTiles.First.Value.GetComponent<TileInfo>();

        // If the player hit space, do the thing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            actions[currentInfo.associatedAction]();
        }
    }

    void jump()
    {
        Debug.Log("Jumping!");
        if (onGround)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    void dash()
    {
        Debug.Log("Dashing!");
        StartCoroutine(dashRoutine());
    }

    IEnumerator dashRoutine()
    {
        TS.MovementSpeed += dashSpeed;
        yield return new WaitForSeconds(dashTime);
        TS.MovementSpeed -= dashSpeed;
    }

    void OnTriggerEnter()
    {
        onGround = true;
    }

    void OnTriggerExit()
    {
        onGround = false;
    }
}
