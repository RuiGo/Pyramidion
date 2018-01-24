using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FortuneTower {
    public class Deck {
        public List<Card> deckCardsList = new List<Card>();

        private int differentCards = 10; //Number of different cards ex: 1-7
        private int numberOfEqualCards = 10; //Number of duplicates of each card

        public Deck() {
            AddStandardCards();
        }

        void AddStandardCards() {
            int cardNumber = 0;
            //Only cards from 1 to 7
            for (int i = 0; i < differentCards; i++) {
                cardNumber++;
                for (int j = 0; j < numberOfEqualCards; j++) {
                    Card c = new Card(cardNumber);
                    deckCardsList.Add(c);
                }
            }
        }

        public void ShuffleDeck() {
            //Fisher-Yates Shuffle
            Card temp;
            int randomNum;
            for (int i = deckCardsList.Count - 1; i > 0; i--) {
                randomNum = Mathf.RoundToInt(Random.Range(0, i));
                temp = deckCardsList[i];
                deckCardsList[i] = deckCardsList[randomNum];
                deckCardsList[randomNum] = temp;
            }
        }

        public void AddCardsToDeck(List<Card> list) {
            foreach (Card c in list) {
                deckCardsList.Add(c);
            }
        }
    }



    public class Card {
        public CardScript crdScript;
        public bool isTurned;
        public bool hasExploded;
        public int cardNumber;

        public Card(int num) {
            isTurned = false;
            hasExploded = false;
            cardNumber = num;
        }
    }
}
