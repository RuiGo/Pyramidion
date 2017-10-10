using UnityEngine;
using UnityEngine.UI;


public class PlayerInformation : MonoBehaviour {

    [SerializeField]	
    private Text m_goldLabel;
    [SerializeField]
    private Text m_betLabel;
    [SerializeField]
    private Text m_winLabel;
    [SerializeField]
    private int m_gold = 1000;
    [SerializeField]
    private int m_bet = 15;
    [SerializeField]
    private float m_addingSpeed = 0.001f;    
    private int m_tempGold;

    public static PlayerInformation playerInformation;
    public int winnings = 0;
    public bool isAddingWinnings = false;
    public RectTransform goldPopupPrefab;
    public RectTransform goldPopupParent;




    

    void Awake() {
        if (playerInformation == null)
            playerInformation = this;
        else {
            print("PlayerInformation script duplicate destroyed!");
            Destroy(gameObject);
        }
    }
	
	void Start () {
        winnings = 0;
        UpdateLabels();			
	}

    void Update () { 
        if(isAddingWinnings && winnings > 0) {
            AddWinningsProgressively();
        }
    }
	
	public void PlaceBet () {
		m_gold -= m_bet;
        print("Bet Placed (" + m_bet + ")");
        UpdateLabels();
        MakeGoldPopup(false, m_bet);
    }

    //TODO: usar uma especie de queue ou simplesmente so deixar uma coisa de cada vez acontecer?
	public void AddWinnings () {
        if (!isAddingWinnings) {
            print("You won (" + winnings + ")");
            m_tempGold = m_gold;
            isAddingWinnings = true;
            MakeGoldPopup(true, winnings);
        }
	}

    public void AddWinningsProgressively () {
        //print("Adding | " + m_gold + " + " + winnings);
        int finalValue = m_gold + winnings;
        if (m_tempGold < finalValue) {
            float valueToAdd = ((winnings * Time.deltaTime * winnings) + m_addingSpeed);
            m_tempGold += valueToAdd < 1 ? 1 : (int)valueToAdd;
            print(m_tempGold);
            m_goldLabel.text = "Gold: " + m_tempGold;
        } else {
            //print("Adding stopped");
            m_gold = finalValue;
            winnings = 0;
            isAddingWinnings = false;
            UpdateLabels();            
        }
    }

    public void UpdateLabels() {
        m_goldLabel.text = "Gold: " + m_gold;
        m_betLabel.text = "Bet: " + m_bet;
        m_winLabel.text = "Win: " + winnings;
    }

    private void MakeGoldPopup(bool isAdding, int value) {
        Transform goldPopup = Instantiate(goldPopupPrefab, goldPopupPrefab.position, goldPopupPrefab.rotation, goldPopupParent) as RectTransform;
        Text gold = goldPopup.GetChild(0).GetComponent<Text>();
        gold.text = isAdding ? "+" + value : "-" + value;
        gold.color = isAdding ? Color.yellow : Color.red;
    }
}
