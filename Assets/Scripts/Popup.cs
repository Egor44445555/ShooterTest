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
        DestroyAllObjectsOfType<Item>(); 
        GameObject.FindGameObjectWithTag("Popup").SetActive(false);
        Time.timeScale = 1;
    }
    
    void DestroyAllObjectsOfType<T>() where T : Component
    {
        T[] objects = FindObjectsOfType<T>();
        
        foreach (T obj in objects)
        {
            Destroy(obj.gameObject);
        }
    }
}
