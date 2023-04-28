using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GlobalUI : MonoBehaviour
{
    public static GlobalUI instance;

    [Header("Text")]
    public TextMeshProUGUI currentScoreTMP;
    public TextMeshProUGUI bestScoreTMP;
    public TextMeshProUGUI [] coinAmounts;
    
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject inGamePanel;
    public GameObject gameOverPanel;
    public GameObject gameOverEffectPanel;


    [SerializeField] GameObject[] objectsToScale;

    [Header("Notify")]
    [SerializeField] CanvasGroup notifyCanvas;
    Vector3 defaultNotifyLocalPosition;


    void Awake() {
        if(instance != null){ 
            Debug.LogWarning("Instance of GlobalUI already exists in the scene !");
            Destroy(this.gameObject);
            return;
        } else instance = this;
    }

    void Start(){
        GameData gameData = SaveManager.LoadData();
        inGamePanel.SetActive(false);
        currentScoreTMP.text = "0";
        bestScoreTMP.text = "BEST\n" + gameData.bestScore.ToString();
        foreach (var objectToScale in objectsToScale) Global.instance.ScaleAnim(objectToScale);
        UpdateAllCoinsAmount();
        if(notifyCanvas != null) {
            notifyCanvas.alpha = 0;
            defaultNotifyLocalPosition = notifyCanvas.transform.localPosition;
        }

        bestScoreTMP.color = new Color(bestScoreTMP.color.r, bestScoreTMP.color.g, bestScoreTMP.color.b, 1f);
    }


    public void UpdateAllCoinsAmount(){ 
        foreach (TextMeshProUGUI coinAmount in coinAmounts) coinAmount.text = SaveManager.LoadData().coins.ToString();
    }

    public void AudioButton() {
        if (PlayerPrefs.GetInt("Audio", 0) == 0) PlayerPrefs.SetInt("Audio", 1);
        else PlayerPrefs.SetInt("Audio", 0);
        AudioManager.instance.AudioCheck();
        AudioManager.instance.PlaySoundButtonClick();
    }

    public void StartGame(){
        AudioManager.instance.PlaySoundButtonClick();
        PlayerCustomize.instance.ChangePlayerSkin();
        CanvasGroup canvasGroupTmp = startPanel.transform.GetComponent<CanvasGroup>();
        LeanTween.alphaCanvas(canvasGroupTmp, 0, 0.25f).setOnComplete( ()=> { 
            startPanel.SetActive(false);
            inGamePanel.SetActive(true);
            canvasGroupTmp.interactable = false;
            Player.instance.canJump = true;
            Player.instance.StartPlayer();
        } );
        bestScoreTMP.color = new Color(bestScoreTMP.color.r, bestScoreTMP.color.g, bestScoreTMP.color.b, 0.25f);
    }

    public void Restart(){
        AudioManager.instance.PlaySoundButtonClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /* ***** NOTFIY *****/
    public void Notify(string text, float delay = 1f){ 
        NotifyHandle(notifyCanvas, text, defaultNotifyLocalPosition, delay); 
    }

    void NotifyHandle(CanvasGroup notify, string text, Vector3 localPos, float delay = 2f){
        notify.GetComponentInChildren<TextMeshProUGUI>().text = text;
        AnimUiUp(notify, localPos, 75, delay);
    }

    void AnimUiUp(CanvasGroup canvasGroup, Vector3 localPos, float moveYValue, float delay = 1.2f, bool disappear = true){
        canvasGroup.gameObject.SetActive(true);
        LeanTween.cancel(canvasGroup.gameObject, true);
        canvasGroup.transform.localPosition = localPos;
        canvasGroup.alpha = 0;
        if(disappear) LeanTween.alphaCanvas(canvasGroup, 1, delay < 3 ? delay/3 : delay/5);
        else LeanTween.alphaCanvas(canvasGroup, 1, delay);
        LeanTween.moveLocalY(canvasGroup.gameObject, localPos.y + moveYValue, delay).setOnComplete( ()=> { 
            if(disappear) LeanTween.alphaCanvas(canvasGroup, 0, delay < 3 ? delay/3 : delay/5);
        } );
    }
}
