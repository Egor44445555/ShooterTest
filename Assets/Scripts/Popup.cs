using UnityEngine;

public class Popup : MonoBehaviour
{
    public GameObject popup;

    public void OpenPopup()
    {
        if (GameObject.FindGameObjectWithTag("Popup"))
        {
            GameObject.FindGameObjectWithTag("Popup").SetActive(false);
        }

        popup.SetActive(true);
    }

    public void ClosePopup()
    {
        GameObject.FindGameObjectWithTag("Popup").SetActive(false);
        Time.timeScale = 1;
    }

    public void CloseOnePopup(GameObject popup)
    {
        popup.SetActive(false);
        Time.timeScale = 1;
    }
}
