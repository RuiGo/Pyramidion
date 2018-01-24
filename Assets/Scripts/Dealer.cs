using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using FortuneTower;

public class Dealer : MonoBehaviour {

    public static Dealer dealerScript;
    public Transform cardPrefab;
    public Transform cardSpawnPosition; // Fake deck position
    public Transform cardPositions;
    public Transform numberLabelsCanvas;
    public List<List<Transform>> availablePositionsList = new List<List<Transform>>();
    public delegate void CardDealerEventHandler();
    public static CardDealerEventHandler cardDealerDelegate;
    public static bool gameEnded;

    private Deck currentDeck = new Deck();
    private List<List<Card>> rows = new List<List<Card>>();
    [SerializeField]
    private List<GameObject> spawnedCards = new List<GameObject>();
    private int maxRows = 5;
    private int rowInPlay = 1;
    private bool isJackpot = true;
    private int jackpotValue = 0;
    private bool gateCardUsed = false;
    private float dealCardTimer = 0;
    private float dealCardTimerCooldown = 0.75f;
    private int dealRowCardIndex = 0;
    private Card firstCardExploded;
    
    void Awake() {
        if (dealerScript == null) {
            dealerScript = this;
        } else {
            print("Dealer script duplicate destroyed!");
            Destroy(gameObject);
        }
    }

	void Start () {
        InputHandler.inputHandlerScript.HideAllButtons();
        gameEnded = false;
        currentDeck.ShuffleDeck();
        ChooseRows();
        SetPositions();
        jackpotValue = CalculateJackpotValue();
        print("Jackpot Value: " + jackpotValue);
        PlayerInformation.playerInformation.PlaceBet();
        cardDealerDelegate = DealRowTimed;
    }
	
	void Update () {
        if (cardDealerDelegate != null) {
            cardDealerDelegate();
        }
    }

    //Fills the rows with cards from the deck
    private void ChooseRows() {
        int rowNumber = 0;
        for (int i = 0; i < maxRows; i++) {
            rowNumber++;
            List<Card> rowCards = new List<Card>();
            for (int j = 0; j < rowNumber; j++) {
                rowCards.Add(currentDeck.deckCardsList[0]);
                currentDeck.deckCardsList.Remove(currentDeck.deckCardsList[0]);
            }
            rows.Add(rowCards);
        }
        for (int x = 0; x < rows.Count; x++) {
            string str = "|";
            for (int y = 0; y < rows[x].Count; y++) {
                str += rows[x][y].cardNumber + "|";
            }
            //print(str);
        }
    }

    //Registers the cards' final positions in the rows
    private void SetPositions() {
        int rowNumber = 0;
        for (int i = 0; i < maxRows; i++) {
            rowNumber++;
            List<Transform> rowPositions = new List<Transform>();
            for (int j = 0; j < rowNumber; j++) {
                rowPositions.Add(cardPositions.GetChild(i).GetChild(j));
            }
            availablePositionsList.Add(rowPositions);
        }
    }

    private void DealRowTimed() {
        if (dealCardTimer <= 0) {
            dealCardTimer = 0;
            // Check if row is within set limit of rounds
            if (rowInPlay >= 0 && rowInPlay <= maxRows) {
                if (dealRowCardIndex >= 0 && dealRowCardIndex < rows[rowInPlay - 1].Count) {
                    // Create card object
                    Transform c = Instantiate(cardPrefab, cardSpawnPosition.position, cardPrefab.rotation) as Transform;
                    CardScript script = c.GetComponent<CardScript>();
                    script.intendedPosition = availablePositionsList[rowInPlay - 1][dealRowCardIndex];
                    script.cardObj = rows[rowInPlay - 1][dealRowCardIndex];
                    script.cardObj.crdScript = script;
                    spawnedCards.Add(c.gameObject);
                    if (rowInPlay == 1) {
                        script.cardObj.isTurned = false;
                    } else {
                        script.cardObj.isTurned = true;
                        c.GetComponent<Animator>().SetTrigger("faceUp");
                    }
                    SortChildren(c, rowInPlay - 1);

                    dealRowCardIndex++;
                    dealCardTimer = dealCardTimerCooldown;
                    // check if reached the end of the deal index
                    if (dealRowCardIndex >= rows[rowInPlay - 1].Count) {
                        dealRowCardIndex = 0;
                        if (rowInPlay > 1) {
                            cardDealerDelegate = null;
                            dealCardTimer = 0;
                            if (rowInPlay == 2) {
                                numberLabelsCanvas.GetChild(rowInPlay - 2).gameObject.SetActive(true);
                                numberLabelsCanvas.GetChild(rowInPlay - 2).GetComponent<Text>().text = CalculateRowValue(rowInPlay - 1).ToString();
                                PlayerInformation.playerInformation.winnings = CalculateRowValue(rowInPlay - 1);
                                PlayerInformation.playerInformation.UpdateLabels();
                                //print("Row value: " + CalculateRowValue(rowInPlay - 1));
                            } else {
                                // Compare against upper row
                                CompareRows();
                                numberLabelsCanvas.GetChild(rowInPlay - 2).gameObject.SetActive(true);
                                numberLabelsCanvas.GetChild(rowInPlay - 2).GetComponent<Text>().text = CalculateRowValue(rowInPlay - 1).ToString();
                                PlayerInformation.playerInformation.winnings = CalculateRowValue(rowInPlay - 1);
                                //print("Row value: " + CalculateRowValue(rowInPlay - 1));                                
                            }
                        }
                        rowInPlay++;
                        // If reached last row
                        if (rowInPlay > maxRows && !gameEnded) {
                            if (isJackpot) {
                                PlayerInformation.playerInformation.winnings = jackpotValue;
                                PlayerInformation.playerInformation.UpdateLabels();
                                //start jackpot effects                                
                            }
                            PlayerInformation.playerInformation.AddWinnings();
                            gameEnded = true;
                            InputHandler.inputHandlerScript.ToggleButton("bank", false);
                            InputHandler.inputHandlerScript.ToggleButton("deal", false);

                            InputHandler.inputHandlerScript.ToggleButton("newRound", true);
                            InputHandler.inputHandlerScript.ToggleButton("mainMenu", true);
                        } else if(gameEnded) {
                            InputHandler.inputHandlerScript.ToggleButton("bank", false);
                            InputHandler.inputHandlerScript.ToggleButton("deal", false);

                            InputHandler.inputHandlerScript.ToggleButton("newRound", true);
                            InputHandler.inputHandlerScript.ToggleButton("mainMenu", true);
                        } else {
                            InputHandler.inputHandlerScript.ToggleButton("bank", true);
                            InputHandler.inputHandlerScript.ToggleButton("deal", true);
                        }
                    }
                } else {
                    print("deal index out of bounds");
                }
            } else {
                print("Rows beyond set limit!");
            }
        } else {
            dealCardTimer -= Time.deltaTime;
        }
    }

    private void SortChildren(Transform t, int order) {
        for (int i = 0; i < t.childCount; i++) {
            t.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = order;
        }
    }

    //Compares upper row cards with lower row
    private void CompareRows() {        
        List<Card> upperRow = rows[rowInPlay - 2];
        List<Card> lowerRow = rows[rowInPlay - 1];

        for (int i = 0; i < upperRow.Count; i++) {
            //Check card left to upper one
            if (upperRow[i].cardNumber == lowerRow[i].cardNumber) {
                if (!upperRow[i].hasExploded)
                    upperRow[i].crdScript.ExplodeCard();

                if (!lowerRow[i].hasExploded) {
                    lowerRow[i].crdScript.ExplodeCard();                   
                    if (!gateCardUsed) {
                        gateCardUsed = true;
                        isJackpot = false;
                        bool flag = UseGateCard(upperRow[i], lowerRow[i], i);
                        if (flag) {
                            upperRow[i].crdScript.CleanCard();
                        }
                    }
                }
            }
            //Check card right to upper one
            if (upperRow[i].cardNumber == lowerRow[i + 1].cardNumber) {
                if (!upperRow[i].hasExploded)
                    upperRow[i].crdScript.ExplodeCard();

                if (!lowerRow[i + 1].hasExploded) {
                    lowerRow[i + 1].crdScript.ExplodeCard();
                    if (!gateCardUsed) {
                        gateCardUsed = true;
                        isJackpot = false;
                        bool isDifferentNumberCard = UseGateCard(upperRow[i], lowerRow[i + 1], i + 1);
                        if (isDifferentNumberCard) {
                            upperRow[i].crdScript.CleanCard();
                        }
                    }
                }
            }
        }
        if (CheckForExplodedCardsInRow(rows[rowInPlay - 1])) {
            gameEnded = true;
        }
    }

    private bool CheckForExplodedCardsInRow(List<Card> row) {
        foreach(Card c in row) {
            if (c.hasExploded)
                return true;
        }
        return false;
    }

    private int CalculateRowValue(int rowNumber) {
        bool anyExploded = false;
        foreach (Card c in rows[rowNumber]) {
            if (c.hasExploded) {
                anyExploded = true;
                break;
            }
        }
        if (anyExploded) {
            return 0;
        } else {
            int value = 0; // for now it's the number, later it will be the value
            int count = 0; // number of equal cards
            int multiplierNumber = rows[rowNumber][0].cardNumber;
            foreach (Card c in rows[rowNumber]) {
                value += c.cardNumber;
                if (c.cardNumber == multiplierNumber)
                    count++;
            }
            //if number of equal card is the same as the cards in the row
            if (count > 0 && count == rows[rowNumber].Count) {
                return value * count;
                //anounce multiplier -> animations etc etc
            } else
                return value;
        }
    }

    private int CalculateJackpotValue () {
        int value = 0;
        foreach (List<Card> list in rows) {
            foreach (Card c in list) {
                value += c.cardNumber;
            }
        }
        return value;
    }

    private bool UseGateCard(Card upperCard, Card bottomCard, int bottomCardIndex) {
        //replace the old card for the new one in the list
        rows[rowInPlay - 1][bottomCardIndex] = rows[0][0];

        //pull the old card a little bit back
        Vector3 newPosition = bottomCard.crdScript.transform.position;
        newPosition += new Vector3(0, 0, 0.1f);
        bottomCard.crdScript.transform.position = newPosition;

        //set the old card's position and sorting order for the new one
        rows[0][0].crdScript.intendedPosition = availablePositionsList[rowInPlay - 1][bottomCardIndex];
        rows[0][0].crdScript.finalPositionDirection = rows[0][0].crdScript.intendedPosition.position - rows[0][0].crdScript.transform.position;
        rows[0][0].crdScript.isInPosition = false;
        SortChildren(rows[0][0].crdScript.transform, bottomCard.crdScript.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder);
        rows[0][0].crdScript.FlipCard();

        //TODO: change to use compare rows again
        //check if the cards still explode
        if (rows[0][0].cardNumber == upperCard.cardNumber) {
            return false;
        } else {
            return true;
        }            
    }

    private void PrintCardList(List<Card> list) {
        foreach(Card c in list) {
            print(c.cardNumber);
        }
    }

    private void PrintUnshuffledDeck() {
        int count = 0;
        for (int j = 0; j < 7; j++) {
            string str = "|";
            for (int i = 0; i < 10; i++) {
                str += currentDeck.deckCardsList[count].cardNumber.ToString() + "|";
                count++;
            }
            print(str);
        }
    }

    private void ClearSpawnedCards() {
        foreach (GameObject obj in spawnedCards) {
            Destroy(obj);
        }
        spawnedCards.Clear();
        for (int i=0; i<numberLabelsCanvas.childCount; i++) {
            numberLabelsCanvas.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void DealNextRow() {
        if (cardDealerDelegate == null && (rowInPlay >= 0 && rowInPlay <= maxRows) && !gameEnded) {
            InputHandler.inputHandlerScript.ToggleButton("bank", false);
            InputHandler.inputHandlerScript.ToggleButton("deal", false);
            cardDealerDelegate = DealRowTimed;
        }
    }

    public void BankMoney() {
        if (cardDealerDelegate == null && (rowInPlay >= 0 && rowInPlay <= maxRows) && !gameEnded) {
            // Hide Buttons
            InputHandler.inputHandlerScript.ToggleButton("bank", false);
            InputHandler.inputHandlerScript.ToggleButton("deal", false);

            PlayerInformation.playerInformation.AddWinnings();
            //show the rest of the rows

            // Show Buttons
            InputHandler.inputHandlerScript.ToggleButton("newRound", true);
            InputHandler.inputHandlerScript.ToggleButton("mainMenu", true);
        }
    }

    public void StartNewRound() {
        // Hide Buttons
        InputHandler.inputHandlerScript.HideAllButtons();

        ClearSpawnedCards();
        isJackpot = true;
        jackpotValue = 0;
        gateCardUsed = false;
        dealCardTimer = 0;
        dealRowCardIndex = 0;
        gameEnded = false;
        rows.Clear();
        currentDeck = new Deck();
        currentDeck.ShuffleDeck();
        ChooseRows();
        SetPositions();
        jackpotValue = CalculateJackpotValue();
        print("Jackpot Value: " + jackpotValue);
        PlayerInformation.playerInformation.PlaceBet();
        rowInPlay = 1;
        cardDealerDelegate = DealRowTimed;
    }
}
