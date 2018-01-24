using UnityEngine;
using FortuneTower;

public class CardScript : MonoBehaviour {
    
    private Animator cardAnimator;
    private float movementSpeed = 20f;
    private float snapPositionDistance = 0.2f;

    public Vector3 finalPositionDirection;
    public Card cardObj;
    public Transform intendedPosition;
    public bool isInPosition;

    

    void Start () {
        cardAnimator = GetComponent<Animator>();
        isInPosition = false;
        //print("card position: " + intendedPosition.gameObject.name);
        finalPositionDirection = intendedPosition.position - transform.position;

        if (cardObj != null)
            AssignSprite();
        else
            print("Cardinfo not loaded!!!");
	}
	
	void Update () {
	    if (!isInPosition && intendedPosition != null) {
            GoToIntendedPosition();
        }
	}

    public void ExplodeCard() {
        if (!cardObj.hasExploded) {
            cardObj.hasExploded = true;
            //Exploding Animation
            transform.GetChild(0).gameObject.SetActive(true);
        } else {
            print("Card already exploded!");
        }
    }

    public void CleanCard() {
        if (cardObj.hasExploded) {
            cardObj.hasExploded = false;
            //Clean Card Animation
            transform.GetChild(0).gameObject.SetActive(false);
        } else {
            print("Card was already clean!");
        }
    }

    public void FlipCard() {
        if (cardObj.isTurned) {
            cardObj.isTurned = false;
            cardAnimator.SetTrigger("faceDown");
        } else {
            cardObj.isTurned = true;
            cardAnimator.SetTrigger("faceUp");
        }
    }

    public void GoToIntendedPosition() {
        float distanceToPosition = Vector3.Distance(transform.position, intendedPosition.position);
        if (distanceToPosition > snapPositionDistance) {
            Vector3 finalPosDir = intendedPosition.position - transform.position;
            Vector3 newPosition = transform.position;
            newPosition += finalPosDir.normalized * movementSpeed * Time.deltaTime;
            transform.position = newPosition;
        } else {
            transform.position = intendedPosition.position;
            isInPosition = true;
        }
    }

    void AssignSprite() {
        if(cardObj.cardNumber>=1 && cardObj.cardNumber <= 10) 
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = SpriteStorage.spriteStorage.cardFaces[cardObj.cardNumber-1];
        else
            print("Card number not correct");
    }
}
