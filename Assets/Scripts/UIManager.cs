using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager main;

    [SerializeField] GameObject gameoverPopup;
    [SerializeField] TextMeshProUGUI ammoCount;
    [SerializeField] Image imageCurrentWeapon;

    Player player;
    bool gameover = false;

    void Awake()
    {
        main = this;
    }

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

    public void AmmoCountCheck()
    {
        ammoCount.text = FindObjectOfType<Weapon>().bulletCount.ToString() + " / " + FindObjectOfType<Weapon>().maxBulletCount.ToString();
        imageCurrentWeapon.sprite = FindObjectOfType<Weapon>().GetComponent<SpriteRenderer>().sprite;
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
