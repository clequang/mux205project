using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }
}
