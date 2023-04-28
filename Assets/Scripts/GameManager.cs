using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [HideInInspector] public int score = 0;

    public Transform leftWall, rightWall;
    static int PlayCount;
    public int gamePlayedToShowAd = 1;


    void Awake(){
        if(instance != null){
			Debug.LogWarning("Instance of GameManager already exists in the scene !");
			Destroy(this.gameObject);
			return;
		} else instance = this;

        Time.timeScale = 1.0f;
    }

    public void AddScore(){
        score++;
        GlobalUI.instance.currentScoreTMP.text = score.ToString();
        Global.instance.ScaleAnim(GlobalUI.instance.currentScoreTMP.gameObject, 1.1f, 0.1f);
        AudioManager.instance.PlayScoreAudio();

        GameData gameData = SaveManager.LoadData();
		gameData.coins += 1;
		gameData.lastScore = score;
		if(score > gameData.bestScore) {
            gameData.bestScore = score;
            GlobalUI.instance.bestScoreTMP.text = "BEST\n" + gameData.bestScore.ToString();
        }
        SaveManager.SaveData(gameData);
    }


    public void Gameover(){
        PauseMenu.instance.pauseButton.SetActive(false);
        StartCoroutine(GameoverCoroutine());
    }

    IEnumerator GameoverCoroutine(){
        Color color = new Color(1, 0.9f, 0.7f);
        GlobalUI.instance.currentScoreTMP.color = color;
        GlobalUI.instance.bestScoreTMP.color = color;

        GlobalUI.instance.gameOverEffectPanel.SetActive(true);
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.5f);
        GlobalUI.instance.gameOverPanel.SetActive(true);
        yield break;
    }
}
