using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{

    static float lerp(float t, float a, float b)
    {
        return (1 - t) * a - t * b;
    }

    public static void SetTransparency(this UnityEngine.UI.Image p_image,float rate)
    {
        if (p_image != null)
        {
            UnityEngine.Color __alpha = p_image.color;
            __alpha.a = lerp(rate, 1, 0); 
            p_image.color = __alpha;
        }
    }

    public static void SetTextTransparency(this UnityEngine.UI.Text p_text, float rate)
    {
        if (p_text != null)
        {
            UnityEngine.Color __alpha = p_text.color;
            __alpha.a = lerp(rate, 1, 0);
            p_text.color = __alpha;
        }
    }

}

public class MenuController : MonoBehaviour
{
    [SerializeField] private List<GameObject> MenuObjects;

    #region PlayButton Variables
    [SerializeField] private List<GameObject> startButtons;
    private float startOffset = 100f;
    private float endOffset = 300f;
    private float moveSpeed = 1.8f;
    private bool playPressed = false;
    private Vector2[] startPositions;
    private Vector2[] rightPositions;
    private Vector2[] leftPositions;
    private float time = 0;
    public float factor;
#endregion

  
    public void PlayButton()
    {
        startPositions = new Vector2[startButtons.Count];
        leftPositions = new Vector2[startButtons.Count];
        rightPositions = new Vector2[startButtons.Count];
        for (int i = 0; i < startButtons.Count; i++)
        {
            //startButtons[i].GetComponent<Button>().interactable = false;
            startPositions[i] = new Vector2(startButtons[i].transform.position.x, startButtons[i].transform.position.y);
            leftPositions[i] = new Vector2(startButtons[i].transform.position.x - startOffset, startButtons[i].transform.position.y);
            rightPositions[i] = new Vector2(startButtons[i].transform.position.x + endOffset, startButtons[i].transform.position.y);
        }
        playPressed = true;
    }

    void Update()
    {
        #region PlayButton
        if (playPressed)
        {
            time += Time.deltaTime * moveSpeed;
            for (int i = 0; i < startButtons.Count; i++)
            {
                float localTime = Mathf.Clamp01(time - factor * i);
                
                startButtons[i].transform.position = Vector2.LerpUnclamped(localTime < 0.1f ? startPositions[i] : leftPositions[i],
                    localTime < 0.1f ? leftPositions[i] : rightPositions[i], localTime * 2); //moving the object to the left, and then to the right
                startButtons[i].GetComponent<Image>().SetTransparency(time); //button color
                startButtons[i].GetComponentInChildren<Text>().SetTextTransparency(time); //button text color

                if (Vector2.Distance(startButtons[startButtons.Count - 1].transform.position, rightPositions[i]) == 400) //hard coded, but this is the stop position
                {
                    playPressed = false;
                    time = 0f;
                }
            }
        }
#endregion
    }
}


