using System;
using UnityEngine;

[Serializable] public struct PlayerSkin {
    public Character character;
    public GameObject[] skinsToActivate;
}

public class PlayerCustomize : MonoBehaviour
{
    public static PlayerCustomize instance;
    
    [SerializeField] PlayerSkin[] playerSkins;
    [SerializeField] Material particleMaterial;
    [SerializeField] TrailRenderer trailRenderer;
    
    [HideInInspector] public Sprite eyesClosedSprite;
	[HideInInspector] public Sprite eyesBorderSprite;
	[HideInInspector] public Sprite eyesWhiteSprite;
    
    Eyes eyes;

    void Awake(){
        if(instance != null){
            Debug.LogWarning("Instance of PlayerCustomize already exists in the scene !");
            Destroy(this.gameObject);
            return;
        } else instance = this;
    }

    void Start(){
        eyes = GetComponentInChildren<Eyes>();
    }

    [ContextMenu("Change Player Skin")]
    public void ChangePlayerSkin(){
        GameData gameData = SaveManager.LoadData();
        foreach (PlayerSkin playerSkin in playerSkins){
            if(gameData.currentCharacter == playerSkin.character.label){
                foreach (GameObject skinToActivate in playerSkin.skinsToActivate) skinToActivate.SetActive(true);
                Player.instance.transform.localScale = playerSkin.character.size;
                Player.instance.jumpSpeedX =  Player.instance.GetJumpX(playerSkin.character.size.x);
                Player.instance.jumpSpeedY =  Player.instance.GetJumpY(playerSkin.character.size.y);
                eyesClosedSprite = playerSkin.character.eyesClosed;
                eyesBorderSprite = playerSkin.character.eyesBorder;
                eyesWhiteSprite = playerSkin.character.eyesWhite;
                ChangePlayerColor(playerSkin.character);
        		eyes.whiteEyes.transform.GetComponent<SpriteRenderer>().sprite = PlayerCustomize.instance.eyesWhiteSprite;
            }else{
                foreach (GameObject skinToActivate in playerSkin.skinsToActivate) skinToActivate.SetActive(false);
            }
        }
    }

    void ChangePlayerColor(Character character){
        Color color = character.color;
        GetComponent<SpriteRenderer>().color = color;
        eyes.eyesSpriteRenderer.color = color;
        particleMaterial.color = color;
        trailRenderer.startColor = new Color(color.r * 0.75f, color.g * 0.75f, color.b * 0.75f, 1f);
        trailRenderer.endColor = new Color(color.r, color.g, color.b, 0.25f);
        
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, (transform.localScale.x + transform.localScale.y)  * 2f);
        curve.AddKey(0.6f, 0.0f);
        trailRenderer.time = 0.6f;
        trailRenderer.widthCurve = curve;
    }
}
