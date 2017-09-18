using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour {

    public static InputHandler inputHandlerScript;
    [SerializeField]
    private Button m_dealButton;
    [SerializeField]
    private Button m_bankButton;
    [SerializeField]
    private Button m_newRoundButton;
    [SerializeField]
    private Button m_mainMenuButton;


    void Awake() {
        if (inputHandlerScript == null)
            inputHandlerScript = this;
        else {
            print("InputHandler script duplicate destroyed!");
            Destroy(gameObject);
        }
    }

	void Start () {}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
            RestartScene();

        if (Input.GetKeyDown(KeyCode.D))
            Dealer.dealerScript.DealNextRow();

        if(Input.GetKeyDown(KeyCode.B))
            Dealer.dealerScript.BankMoney();
    }

    public void DealRow() {
        Dealer.dealerScript.DealNextRow();
    }

    public void Bank() {
        Dealer.dealerScript.BankMoney();
    }

    public void NewRound() {
        Dealer.dealerScript.StartNewRound();
    }

    public void HideAllButtons() {
        m_dealButton.gameObject.SetActive(false);
        m_bankButton.gameObject.SetActive(false);
        m_newRoundButton.gameObject.SetActive(false);
        m_mainMenuButton.gameObject.SetActive(false);
    }

    public void ToggleButton(string btn, bool visible) {
        switch(btn) {
            case "deal":
                m_dealButton.gameObject.SetActive(visible);
                break;
            case "bank":
                m_bankButton.gameObject.SetActive(visible);
                break;
            case "newRound":
                m_newRoundButton.gameObject.SetActive(visible);
                break;
            case "mainMenu":
                m_mainMenuButton.gameObject.SetActive(visible);
                break;
            default:
                print("Not a valid button");
                break;
        }
    }

    public void RestartScene() {
        SceneManager.LoadScene("Rework Scene");
    }
}
