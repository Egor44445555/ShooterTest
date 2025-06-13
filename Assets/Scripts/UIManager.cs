using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject gameoverPopup;

    Player player;
    bool gameover = false;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        if (player == null && !gameover)
        {
            gameoverPopup.SetActive(true);
            gameover = true;
        }
    }

    public void ActionButton()
    {
        if (player != null)
        {
            player.Action();
        }
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
