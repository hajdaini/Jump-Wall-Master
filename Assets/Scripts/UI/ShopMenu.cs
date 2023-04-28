using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopMenu : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] GameObject shopMenu;
    [SerializeField] GameObject cardcharacterImage;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI characterNameTMP;  
    [SerializeField] TextMeshProUGUI characterPriceTMP;  

    [Header("Image")]
    [SerializeField] Image characterImage;
    [SerializeField] Image lockImage;
    
    [Header("Button")]
    [SerializeField] Button buyButton;
    [SerializeField] Button chooseButton;


    [Header("Sliders")]
    [SerializeField] Slider speedXSlider;
    [SerializeField] Slider speedYSlider;
    [SerializeField] Slider sizeXSlider;
    [SerializeField] Slider sizeYSlider;
    

    int currentCharacterIndex;

    void Start(){
        for (int i = 0; i < Global.instance.characters.Length; i++){
            if(Global.instance.characters[i].label == SaveManager.LoadData().currentCharacter) currentCharacterIndex = i;
        }
        shopMenu.GetComponent<CanvasGroup>().alpha = 0;
        speedXSlider.maxValue = 5f;
        speedYSlider.maxValue = 13f;
        sizeXSlider.maxValue = 0.35f;
        sizeYSlider.maxValue = 0.35f;
        
        ChangeCharacterUI();
    }

    public void ShopMenuShow(){
        AudioManager.instance.PlaySoundButtonClick();
        CanvasGroup shopCanvasGroup = shopMenu.GetComponent<CanvasGroup>();
        RectTransform childCardRect = shopMenu.transform.GetChild(0).GetComponent<RectTransform>();
        shopMenu.SetActive(true);
        shopCanvasGroup.interactable = false;
        shopMenu.GetComponent<CanvasGroup>().LeanAlpha(1f, 0.3f).setOnComplete(() => {shopCanvasGroup.interactable = true; });
        childCardRect.LeanScale(Vector2.one * 0.9f, 0.1f).setEaseInOutBounce().setOnComplete(() => { childCardRect.LeanScale(Vector2.one, 0.1f).setEaseInOutBounce(); });
    }

    public void ShopMenuHide(){
        CanvasGroup shopCanvasGroup = shopMenu.GetComponent<CanvasGroup>();
        AudioManager.instance.PlaySoundButtonClick();
        shopCanvasGroup.LeanAlpha(0f, 0.25f).setOnComplete(() => {shopMenu.SetActive(false); });
    }

    public void NextCharacter(){
        AudioManager.instance.PlaySoundButtonClick();
        currentCharacterIndex += 1;
        if(currentCharacterIndex > Global.instance.characters.Length - 1) currentCharacterIndex = 0;
        ChangeCharacterUI();
    }

    public void PreviousCharacter(){
        AudioManager.instance.PlaySoundButtonClick();
        currentCharacterIndex -= 1;
        if(currentCharacterIndex < 0) currentCharacterIndex = Global.instance.characters.Length - 1;
        ChangeCharacterUI();
    }

    void ChangeCharacterUI(){
        AnimChangeCharacterUI();
        GameData gameData = SaveManager.LoadData();
        Character characterTmp = Global.instance.characters[currentCharacterIndex];
        characterImage.sprite = characterTmp.icon;
        characterNameTMP.text = characterTmp.label;
        characterPriceTMP.text = characterTmp.price.ToString().ToUpper() + " BUY";

        speedXSlider.value =  Player.instance.GetJumpX(characterTmp.size.y);
        speedYSlider.value = Player.instance.GetJumpY(characterTmp.size.x);
        sizeXSlider.value = characterTmp.size.x;
        sizeYSlider.value = characterTmp.size.y;

        chooseButton.GetComponent<Button>().onClick.RemoveAllListeners();
        buyButton.GetComponent<Button>().onClick.RemoveAllListeners();
        
        if(gameData.characterIds.Contains(characterTmp.label)) {
            buyButton.gameObject.SetActive(false);
            if(!(characterTmp.label == gameData.currentCharacter)){
                chooseButton.gameObject.SetActive(true);
                chooseButton.GetComponent<Button>().onClick.AddListener(delegate { ChooseCharacter(characterTmp); });
            }else{
                chooseButton.gameObject.SetActive(false);
            }
        }else{
            buyButton.gameObject.SetActive(true);
            chooseButton.gameObject.SetActive(false);
            if(gameData.coins >= characterTmp.price){
                buyButton.interactable = true;
                
                buyButton.GetComponent<Button>().onClick.AddListener(delegate { BuyCharacter(characterTmp); });
                lockImage.gameObject.SetActive(false);
            }else{
                buyButton.interactable = false;
                lockImage.gameObject.SetActive(true);
            }
        }
    }

    void AnimChangeCharacterUI(){
        RectTransform cardcharacterImageRect = cardcharacterImage.transform.GetComponent<RectTransform>();
        RectTransform buyButtonRect = buyButton.transform.GetComponent<RectTransform>();
        RectTransform chooseButtonRect = chooseButton.transform.GetComponent<RectTransform>();
        cardcharacterImageRect.localScale = Vector2.one * 0.9f;
        buyButtonRect.localScale = Vector2.one * 0.9f;
        chooseButtonRect.localScale = Vector2.one * 0.9f;
        cardcharacterImageRect.LeanScale(Vector2.one, 0.1f).setEaseInOutBounce();
        buyButtonRect.LeanScale(Vector2.one, 0.1f).setEaseInOutBounce();
        chooseButtonRect.LeanScale(Vector2.one, 0.1f).setEaseInOutBounce();
    }

    void BuyCharacter(Character character){
        GameData gameData = SaveManager.LoadData();
		if(!gameData.characterIds.Contains(character.label)) {
            gameData.coins -= character.price;
            gameData.characterIds.Add(character.label);
            gameData.currentCharacter = character.label;
            GlobalUI.instance.Notify($"New player \"{character.label}\"");
        }else Debug.LogWarning(character.label + $" Can't buy {character.label} because already exists inventory !");
		SaveManager.SaveData(gameData);
        AudioManager.instance.PlaySoundButtonReward();
        GlobalUI.instance.UpdateAllCoinsAmount();
        buyButton.gameObject.SetActive(false);
	}

    void ChooseCharacter(Character character){
        GameData gameData = SaveManager.LoadData();
		if(gameData.characterIds.Contains(character.label)) { 
            gameData.currentCharacter = character.label;
            GlobalUI.instance.Notify($"Current player \"{character.label}\"");
        }
        else Debug.LogWarning($"{character.label} doesn't exists in inventory !");
		SaveManager.SaveData(gameData);
        AudioManager.instance.PlaySoundButtonClick();
        chooseButton.gameObject.SetActive(false);
	}
}
