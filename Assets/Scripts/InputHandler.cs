using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour {

    public static InputHandler inputHandlerScript;
    [SerializeField]
    private Button dealButton;
    [SerializeField]
    private Button bankButton;
    [SerializeField]
    private Button newRoundButton;
    [SerializeField]
    private Button mainMenuButton;


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
        dealButton.gameObject.SetActive(false);
        bankButton.gameObject.SetActive(false);
        newRoundButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
    }

    public void ToggleButton(string btn, bool visible) {
        switch(btn) {
            case "deal":
                dealButton.gameObject.SetActive(visible);
                break;
            case "bank":
                bankButton.gameObject.SetActive(visible);
                break;
            case "newRound":
                newRoundButton.gameObject.SetActive(visible);
                break;
            case "mainMenu":
                mainMenuButton.gameObject.SetActive(visible);
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
