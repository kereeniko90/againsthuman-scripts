using UnityEngine;

using UnityEngine.EventSystems;


public class BuildingButtonHighlight : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData) {
        SoundManager.Instance.PlaySound(SoundManager.Sound.Tik);
    }
    
}
