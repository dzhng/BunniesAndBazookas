using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {


    public Transform p1WinGUI;
    public Transform p2WinGUI;
    public enum GameState
    {
        Playing,
        GameOver,
    }

    public GameState state;
    
    // Use this for initialization
	void Start () {
        state = GameState.Playing;

        DisableGUI(p1WinGUI);
        DisableGUI(p2WinGUI);
    }

    public void EndGame(int playerWonId)
    {
        if (state == GameState.GameOver)
        {
            return;
        }
        state = GameState.GameOver;
        switch (playerWonId)
        {
            case 1:
                EnableGUI(p1WinGUI);
                break;
            case 2:
                EnableGUI(p2WinGUI);
                break;
        }
    }

    public void RestartGame()
    {
        if (state == GameState.GameOver)
        {
    		Application.LoadLevel(Application.loadedLevel);
        }
    }


    void DisableGUI(Transform GUI)
    {
        GUI.GetComponent<SpriteRenderer>().enabled = false;
    }

    void EnableGUI(Transform GUI)
    {
        GUI.GetComponent<SpriteRenderer>().enabled = true;
    }
}
