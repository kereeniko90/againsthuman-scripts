using UnityEngine;
using UnityEngine.UI;

public class ControlsUI : MonoBehaviour
{
    [SerializeField] private GameObject controlInfoObject;
    [SerializeField] private GameObject controlInfoButtonObject;
    [SerializeField] private GameObject closeButtonObject;
    

    private Button controlInfoButton;
    private Button closeButton;
    private Animator animator;
    private Animator controlButtonAnimator;

    private void Start() {
        animator = controlInfoObject.GetComponent<Animator>();
        controlButtonAnimator = controlInfoButtonObject.GetComponent<Animator>();
        controlInfoButton = controlInfoButtonObject.GetComponent<Button>();
        closeButton = closeButtonObject.GetComponent<Button>();
        
        controlInfoButton.onClick.AddListener(() => {
            ShowControlUI();
            controlButtonAnimator.SetBool("show", false);
            SoundManager.Instance.PlaySound(SoundManager.Sound.Tik);
        });
        closeButton.onClick.AddListener(() => {
            HideControlUI();
            SoundManager.Instance.PlaySound(SoundManager.Sound.Tik);
            
        });
        
    }

    

    public void ShowControlUI() {
        controlButtonAnimator.SetBool("show", false);
        
        
        animator.SetBool("show", true);
    }

    public void HideControlUI() {

        controlButtonAnimator.SetBool("show",true);
        animator.SetBool("show", false);
    }



    
    
    

}
