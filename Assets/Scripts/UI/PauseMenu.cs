using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    
    [SerializeField] GameObject pausePanel;
    public GameObject pauseButton;

    void Awake(){ 
        if(instance != null){ 
            Debug.LogWarning("Instance of PauseMenu already exists in the scene !");
            Destroy(this.gameObject);
            return;
        } else instance = this;
    }

    void Start(){
        pausePanel.SetActive(false);
    }

    public void PauseMenuShow(){
        Global.instance.EnablePause();
        AudioManager.instance.PlaySoundButtonClick();
        pausePanel.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void PauseMenuHide(){
        Global.instance.DisablePause();
        AudioManager.instance.PlaySoundButtonClick();
        pausePanel.SetActive(false);
        pauseButton.SetActive(true);
        Player.instance.canJump = true;
    }

    public void OnPauseHover(){ Player.instance.canJump = false; }
    public void OnPauseExit(){ Player.instance.canJump = true; }
}
