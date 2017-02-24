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

    public static void LerpTransparency(this UnityEngine.UI.Image p_image,float rate)
    {
        if (p_image != null)
        {
            UnityEngine.Color __alpha = p_image.color;
            __alpha.a = lerp(rate, 1, 0); 
            p_image.color = __alpha;
        }
    }

    public static void SetTransparency(this UnityEngine.UI.Image p_image, float val)
    {
        if (p_image != null)
        {
            UnityEngine.Color __alpha = p_image.color;
            __alpha.a = val;
            p_image.color = __alpha;
        }
    }

    public static void LerpTextTransparency(this UnityEngine.UI.Text p_text, float rate)
    {
        if (p_text != null)
        {
            UnityEngine.Color __alpha = p_text.color;
            __alpha.a = lerp(rate, 1, 0);
            p_text.color = __alpha;
        }
    }

    public static void SetTextTransparency(this UnityEngine.UI.Text p_text, float val)
    {
        if (p_text != null)
        {
            UnityEngine.Color __alpha = p_text.color;
            __alpha.a = val;
            p_text.color = __alpha;
        }
    }

}

public class MenuController : MonoBehaviour
{
    [SerializeField] private List<GameObject> MenuObjects;

    [SerializeField] private List<GameObject> startButtons;


    private float startOffset = 100f; //how far to the left to go
    private float endOffset = 300f; //how far over to the right go
    private float moveSpeed = 1.8f;

    private Vector2[] startPositions;
    private Vector2[] rightPositions;
    private Vector2[] leftPositions;

    private float time = 0;
    public float factor;

    #region ButtonBools

    private bool playPressed = false;
    private bool pausePressed = false;

    #endregion

    void Start()
    {
        MenuObjects[1].gameObject.SetActive(false); //settings button
        MenuObjects[2].gameObject.SetActive(false); //exit/pause button
    }
  
    public void PlayButton()
    {
        startPositions = new Vector2[startButtons.Count];
        leftPositions = new Vector2[startButtons.Count];
        rightPositions = new Vector2[startButtons.Count];
        for (int i = 0; i < startButtons.Count; i++)
        {
            startButtons[i].GetComponent<Button>().interactable = false; //so that player can't spam the button
            startPositions[i] = new Vector2(startButtons[i].transform.position.x, startButtons[i].transform.position.y);
            leftPositions[i] = new Vector2(startButtons[i].transform.position.x - startOffset, startButtons[i].transform.position.y);
            rightPositions[i] = new Vector2(startButtons[i].transform.position.x + endOffset, startButtons[i].transform.position.y);
        }
        playPressed = true;
    }

    public void PauseButton()
    {
        startPositions = new Vector2[startButtons.Count];
        leftPositions = new Vector2[startButtons.Count];
        rightPositions = new Vector2[startButtons.Count];
        for (int i = 0; i < startButtons.Count; i++)
        {
            startButtons[i].GetComponent<Button>().interactable = true; //so that player can't spam the button
            startPositions[i] = new Vector2(startButtons[i].transform.position.x, startButtons[i].transform.position.y);
            leftPositions[i] = new Vector2(startButtons[i].transform.position.x - startOffset, startButtons[i].transform.position.y);
            rightPositions[i] = new Vector2(startButtons[i].transform.position.x + endOffset, startButtons[i].transform.position.y);
        }
        MenuObjects[0].gameObject.SetActive(true);
        MenuObjects[0].GetComponent<Text>().SetTextTransparency(1);
        MenuObjects[0].GetComponent<Text>().text = "Paused";
        pausePressed = true;
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
                startButtons[i].GetComponent<Image>().LerpTransparency(time); //button color
                startButtons[i].GetComponentInChildren<Text>().LerpTextTransparency(time); //button text color
                MenuObjects[0].GetComponent<Text>().LerpTextTransparency(time*4); //Title transparency

                if (Vector2.Distance(startButtons[startButtons.Count - 1].transform.position, rightPositions[i]) == 400) 
                    //hard coded, but this is the stop position where you sill start the game
                {
                    playPressed = false;
                    time = 0f;
                    for (int j = 0; j < startButtons.Count; j++)
                    {
                        startButtons[j].gameObject.SetActive(false);
                    }
                    MenuObjects[0].gameObject.SetActive(false); //disabling the title
                    MenuObjects[1].gameObject.SetActive(true); //enabling the settings button
                    MenuObjects[2].gameObject.SetActive(true); //enabling the game exit/pause button
                }
            }
        }
        #endregion

        #region OptionsButton
        #endregion

        #region ExtrasButton
        #endregion

        #region MiscButton
        #endregion

        #region PauseButton
        if (pausePressed)
        {
            for (int j = 0; j < startButtons.Count; j++)
            {
                startButtons[j].gameObject.SetActive(true);
            }
            startButtons[0].gameObject.SetActive(false);
            time += Time.deltaTime * moveSpeed;
            for (int i = 0; i < startButtons.Count; i++)
            {
                float localTime = Mathf.Clamp01(time - factor * i);
                startButtons[i].transform.position = Vector2.LerpUnclamped(localTime < 0.1f ? startPositions[i] : rightPositions[i],
                    localTime < 0.1f ? rightPositions[i] : leftPositions[i], localTime * 2); //moving the object to the left, and then to the right
                startButtons[i].GetComponent<Image>().LerpTransparency(-time); //button color
                startButtons[i].GetComponentInChildren<Text>().LerpTextTransparency(-time); //button text color
                //MenuObjects[0].GetComponent<Text>().LerpTextTransparency(-localTime); //Title transparency
          

                if (Vector2.Distance(startButtons[startButtons.Count - 1].transform.position, leftPositions[i]) == 400)
                //hard coded, but this is the stop position where you sill start the game
                {
                    pausePressed = false;
                    time = 0f;
                }
            }
        }
        #endregion

        #region SettingsButton



        #endregion
    }
}


