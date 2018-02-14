using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager soundManagerScript;

    [SerializeField]
    private AudioSource cardSoundSource;
    [SerializeField]
    private AudioSource uiSoundSource;
    [SerializeField]
    private AudioSource bgMusicSoundSource;
    [SerializeField]
    private AudioClip[] coinSounds = new AudioClip[2];
    [SerializeField]
    private AudioClip[] cardSounds = new AudioClip[1];
    [SerializeField]
    private AudioClip[] backgroundSounds = new AudioClip[1];
    [SerializeField]
    private AudioClip[] backgroundMusic = new AudioClip[1];

    void Awake() {
        if(soundManagerScript == null) {
            soundManagerScript = this;
        } else {
            print("Sound Manager script duplicate destroyed!");
            Destroy(this.gameObject);
        }
    }
	
    public void PlaySound(string id) {
        switch(id) {
            case "coinBet":
                uiSoundSource.clip = coinSounds[0];
                uiSoundSource.Play();
                break;
            case "regularWin":
                uiSoundSource.clip = coinSounds[1];
                uiSoundSource.Play();
                break;
            case "cardDraw":
                cardSoundSource.clip = cardSounds[0];
                cardSoundSource.Play();
                break;
            default:
                print("Invalid main sound ID!");
                return;
        }
    }
}
