using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("AudioClip")]
    [SerializeField] AudioClip buttonClickAudio;
    [SerializeField] AudioClip buttonRewardAudio;
    [SerializeField] AudioClip jumpAudio;
    [SerializeField] AudioClip collisionAudio;
    [SerializeField] AudioClip scoreSound;
    [SerializeField] AudioClip deadAudio;


    [SerializeField] AudioSource audioSourceMusic, audioSourceSound, audioSourceCollision;

    [HideInInspector] public bool soundIsOn = true;
    [SerializeField] GameObject[] muteImages; 

    float collisionTimer;
    const float collisionDeltaTime = 0.25f;

    
    void Awake(){
        if(instance != null){
            Debug.LogWarning("Instance of AudioManager already exists in the scene !");
            Destroy(this.gameObject);
            return;
        } else instance = this;
    }

    void Start() {
        AudioCheck();    
    }

    void Update(){
        if(Player.instance.isDead) return;
        collisionTimer += Time.deltaTime; 
    }

    public void AudioCheck() {
        if (PlayerPrefs.GetInt("Audio", 0) == 0){
            foreach(GameObject muteImage in muteImages) muteImage.SetActive(false);
            AudioManager.instance.soundIsOn = true;
            AudioManager.instance.StopPlayMusic();
        }else{
            foreach(GameObject muteImage in muteImages) muteImage.SetActive(true);
            AudioManager.instance.soundIsOn = false;
            AudioManager.instance.StopPlayMusic();
        }
    }


    public void StopPlayMusic(){
        if(soundIsOn) audioSourceMusic.Play();
        else  audioSourceMusic.Stop();
    }

    
    public void PlaySoundButtonClick(){ if(soundIsOn) audioSourceMusic.PlayOneShot(buttonClickAudio); }
    public void PlaySoundButtonReward(){  if(soundIsOn) audioSourceMusic.PlayOneShot(buttonRewardAudio); }
    public void PlayJumpAudio(){ if(soundIsOn) audioSourceMusic.PlayOneShot(jumpAudio); }
    public void PlayScoreAudio(){ if(soundIsOn) audioSourceMusic.PlayOneShot(scoreSound); }
    public void PlayDeadAudio(){ if(soundIsOn) audioSourceMusic.PlayOneShot(deadAudio); }
    
    public void PlayCollisionAudio(){ 
        if(soundIsOn && collisionTimer >= collisionDeltaTime){
            collisionTimer = 0;
            audioSourceCollision.pitch = Random.Range(0.8f, 1f);
            audioSourceCollision.Play(); 
        }
    }
}
