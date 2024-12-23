using UnityEngine;
using UnityEngine.EventSystems;

public class StartGameUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MainMenuUI mainMenuUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        mainMenuUI.StartGame();
        SoundManager.Instance.PlaySound(SoundManager.Sound.InteractableButton);
    }

    
}
