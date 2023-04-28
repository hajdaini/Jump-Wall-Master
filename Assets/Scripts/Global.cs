using UnityEngine;

public class Global : MonoBehaviour
{
    public static Global instance;
    [HideInInspector] public bool isTestMode;

    [HideInInspector] public bool isPaused;

    public static string tagWall = "Wall", tagObstacle = "Obstacle";

    public Character[] characters;

    void Awake(){
        if(instance != null){ 
            Debug.LogWarning("There is other Global instance in the scene !");
            Destroy(this.gameObject);
            return;
        } else instance = this;

        #if UNITY_EDITOR
            this.isTestMode = true;
        #else
            this.isTestMode = false;
        #endif

        GameData gameData = SaveManager.LoadData();
        if(gameData.characterIds.Count == 0) {
            gameData.characterIds.Add("Normy");
            SaveManager.SaveData(gameData);
        }
    }

    public void EnablePause(){
        if(isPaused) return;
        isPaused = true;
        Time.timeScale = 0;
        AudioListener.pause = true;
    }

    public void DisablePause(){
        if(!isPaused) return;
        isPaused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    public void ScaleAnim(GameObject objectToScale, float scalePercent = 1.2f, float delay = 0.15f){
        objectToScale.transform.localScale = Vector2.one;
        LeanTween.scale(objectToScale, Vector2.one * scalePercent, delay).setEase(LeanTweenType.easeInOutBounce).setOnComplete(() => {
            LeanTween.scale(objectToScale, Vector2.one, delay).setEase(LeanTweenType.easeInOutBounce);
        });
    }
}
