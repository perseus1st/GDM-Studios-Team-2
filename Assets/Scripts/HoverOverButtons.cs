using UnityEngine;
using UnityEngine.EventSystems;

public class HoverOverButtons : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public GameObject LeftArrow;
    public GameObject RightArrow;


    public void OnPointerEnter(PointerEventData eventData)
    {
        LeftArrow.SetActive(true);
        RightArrow.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeftArrow.SetActive(false);
        RightArrow.SetActive(false);
    }

}
