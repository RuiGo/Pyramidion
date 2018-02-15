using UnityEngine;
using UnityEngine.UI;


public class PlayerInformation : MonoBehaviour {

    public static PlayerInformation playerInformation;
    public int winnings = 0;
    public bool isAddingWinnings = false;
    public bool isSubtractingWinnings = false;
    public RectTransform goldPopupPrefab;
    public RectTransform goldPopupParent;

    [SerializeField]	
    private Text goldLabel;
    [SerializeField]
    private Text betLabel;
    [SerializeField]
    private Text winLabel;
    [SerializeField]
    private int gold = -1;
    [SerializeField]
    private int bet = 15;
    [SerializeField]
    private float addingSpeed = 0.001f;    
    private int tempGold;    

    void Awake() {
        if (playerInformation == null)
            playerInformation = this;
        else {
            print("PlayerInformation script duplicate destroyed!");
            Destroy(gameObject);
        }

        SaveGameControl.saveGameControl.LoadData();
        gold = SaveGameControl.saveGameControl.gameGold;
    }
	
	void Start () {
        winnings = 0;       
        UpdateLabels();			
	}

    void Update () { 
        if(isAddingWinnings && winnings > 0) {
            AddWinningsProgressively();
        } else if(isSubtractingWinnings) {
            SubtractBetProgressively();
        }

        if(Input.GetKeyDown(KeyCode.G)) {
            gold += 1000;
        }
    }
	
	public void PlaceBet () {
        if (!isAddingWinnings) {
            print("Bet Placed (" + bet + ")");
            tempGold = gold;
            SaveGameControl.saveGameControl.gameGold = gold - bet;
            SaveGameControl.saveGameControl.SaveData();
            isSubtractingWinnings = true;
            MakeGoldPopup(false, bet);
            SoundManager.soundManagerScript.PlaySound("coinBet");
        }		
    }

    public void SubtractBetProgressively () {
        //print("Subtracting | " + gold + " - " + bet);
        int finalValue = gold - bet;
        if (tempGold > finalValue) {
            float valueToSubtract = ((bet * Time.deltaTime) + addingSpeed);
            tempGold -= valueToSubtract < 1 ? 1 : (int)valueToSubtract;
            //print(tempGold);
            goldLabel.text = "Gold: " + tempGold;
        } else {
            //print("Subtracting stopped");
            gold = finalValue;
            isSubtractingWinnings = false;
            UpdateLabels();
        }
    }

    public void AddWinnings () {
        if (!isAddingWinnings) {
            print("You won (" + winnings + ")");
            SaveGameControl.saveGameControl.gameGold = gold + winnings;
            SaveGameControl.saveGameControl.SaveData();
            tempGold = gold;
            isAddingWinnings = true;
            MakeGoldPopup(true, winnings);
            SoundManager.soundManagerScript.PlaySound("regularWin");
        }
	}

    public void AddWinningsProgressively () {
        //print("Adding | " + gold + " + " + winnings);
        int finalValue = gold + winnings;
        if (tempGold < finalValue) {
            float valueToAdd = ((winnings * Time.deltaTime * winnings) + addingSpeed);
            tempGold += valueToAdd < 1 ? 1 : (int)valueToAdd;
            //print(tempGold);
            goldLabel.text = "Gold: " + tempGold;
        } else {
            //print("Adding stopped");
            gold = finalValue;
            winnings = 0;
            isAddingWinnings = false;
            UpdateLabels();            
        }
    }

    public void UpdateLabels() {
        goldLabel.text = "Gold: " + gold;
        betLabel.text = "Bet: " + bet;
        winLabel.text = "Win: " + winnings;
    }

    private void MakeGoldPopup(bool isAdding, int value) {
        Transform goldPopup = Instantiate(goldPopupPrefab, goldPopupPrefab.position, goldPopupPrefab.rotation, goldPopupParent) as RectTransform;        
        Text gold = goldPopup.GetComponent<Text>();
        gold.text = isAdding ? "+" + value : "-" + value;
        gold.color = isAdding ? Color.yellow : Color.red;
        goldPopup.localPosition = Vector3.zero;
        Destroy(goldPopup.gameObject, 1.5f);
    }
}
