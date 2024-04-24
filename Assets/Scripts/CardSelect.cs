using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSelect : MonoBehaviour, IPointerClickHandler
{
    public bool isSelected = false; 

    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = !isSelected;
        GetComponent<Image>().color = isSelected ? Color.red : Color.white; 
    }
}
