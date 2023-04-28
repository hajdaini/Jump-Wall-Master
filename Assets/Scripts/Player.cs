using UnityEngine;
using System.Collections;
using UnityEngine.Pool;

public class Player : MonoBehaviour
{
    public static Player instance;

    [Header("Appearance")]


    [HideInInspector] public bool isDead = false;
    float hueValue;

    [Header("Prefabs")]
    [SerializeField] GameObject JumpEffectObj;
    public GameObject WallBounceEffectObj;
    public GameObject DeadEffectObj;

    [Header("Physics")]
    [SerializeField] float Gravity;
    
    Rigidbody2D[] rbs;
    [SerializeField] Transform centerBone;
    [HideInInspector] public CameraShake cameraShake;
    [HideInInspector] public bool isRight;

    public IObjectPool<GameObject> pool;

    [HideInInspector] public bool canJump;

    float fxTimer;
    const float fxDeltaTime = 0.05f;


    [HideInInspector] public float jumpSpeedX, jumpSpeedY;

    void Awake(){
        if(instance != null){
			Debug.LogWarning("Instance of Player already exists in the scene !");
			Destroy(this.gameObject);
			return;
		} else instance = this;

        pool = new ObjectPool<GameObject>( 
            () => { return CreateProjectile(); },
            fxTmp => { fxTmp.SetActive(true); },
            fxTmp => { fxTmp.SetActive(false); },
            fxTmp => { Destroy(fxTmp); },
            false,
            50,
            50
        );
    }

    void Start(){
        rbs = GetComponentsInChildren<Rigidbody2D>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
        foreach(Rigidbody2D rb in rbs) rb.gravityScale = Gravity;
        hueValue = Random.Range(0, 10) / 10.0f;
        StopPlayer();
    }

    
    void Update(){
        if (isDead || !canJump) return;
        if (Input.GetMouseButtonDown(0)){
            AudioManager.instance.PlayJumpAudio();
            StartCoroutine(SpawnJumpParticle(centerBone.position));
            foreach(Rigidbody2D rb in rbs){
                if (isRight) rb.velocity = new Vector2(jumpSpeedX, jumpSpeedY);
                else rb.velocity = new Vector2(-jumpSpeedX, jumpSpeedY);
            }
        }
        fxTimer += Time.deltaTime; 
    }



    private GameObject CreateProjectile(){
        GameObject gameObjectTmp = Instantiate(JumpEffectObj);
        gameObjectTmp.SetActive(false);
        return gameObjectTmp;
    }

    public IEnumerator SpawnJumpParticle(Vector3 spawnPosition){
        if(fxTimer < fxDeltaTime) yield break;
        fxTimer = 0;
        GameObject effectObj = pool.Get();
        effectObj.transform.position = spawnPosition;
        yield return new WaitForSeconds(0.5f);
        pool.Release(effectObj);
    }

    public void StartPlayer(){
        foreach(Rigidbody2D rb in rbs){
            rb.velocity = new Vector2(-1, 0);
            rb.isKinematic = false;
        }
    }

    public void StopPlayer(){
        foreach(Rigidbody2D rb in rbs){
            rb.velocity = new Vector2(0, 0);
            rb.isKinematic = true;
        }
    }

    public float GetJumpX(float sizeX){ return (1 / (sizeX / 0.75f)); }
    public float GetJumpY(float sizeY){ return (1 / (sizeY / 2.1f)); }
}
