using UnityEngine;
using System.Collections;

public class SpriteStorage : MonoBehaviour {

    public static SpriteStorage spriteStorage;

    public Sprite[] cardFaces = new Sprite[11];

    void Awake() {
        if (spriteStorage == null)
            spriteStorage = this;
        else {
            print("SpriteStorage script duplicate destroyed!");
            Destroy(gameObject);
        }
    }
}
