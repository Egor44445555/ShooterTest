using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Player player;
    bool isButtonPressed = false;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isButtonPressed = true;
        StartCoroutine(WhileButtonHeld());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isButtonPressed = false;
    }

    IEnumerator WhileButtonHeld()
    {
        while (isButtonPressed)
        {
            if (player != null)
            {
                player.Action();
            }

            yield return null;
        }
    }
}
