using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool isPressed = false;
    private float delay = 0.2f;
    private float minDelay = 0.01f;
    private float subtractDelay = 0.025f;
    private bool minDelayAchieved;
    public void OnPointerClick(PointerEventData eventData)
    {
        
        bool leftClick = Input.GetMouseButtonUp(0);
        if (leftClick && eventData.pointerClick.TryGetComponent(out ItemSlot itemSlot))
        {
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                itemSlot.ToggleFavorite();
                return;
            }

            PlayerCursor.Instance.Click(itemSlot, leftClick);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        bool leftClick = Input.GetMouseButton(0);
        isPressed = true;
        if (!leftClick)
        {
            if (eventData.pointerEnter.TryGetComponent(out ItemSlot itemSlot))
            {
                StopAllCoroutines();
                minDelayAchieved = false;
                StartCoroutine(WhileHolding(itemSlot, leftClick));
            }
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
    IEnumerator WhileHolding(ItemSlot itemSlot, bool leftClick)
    {
        if (!isPressed)
        {
            delay = 0.2f;
            yield break;
        }
        PlayerCursor.Instance.Click(itemSlot, leftClick);
        yield return new WaitForSeconds(delay);
        if (!minDelayAchieved && delay > minDelay)
        {
            delay -= subtractDelay;
        }
        else
        {
            delay = minDelay;
            minDelayAchieved = true;
        }

        StartCoroutine(WhileHolding(itemSlot, leftClick));
    }
}
