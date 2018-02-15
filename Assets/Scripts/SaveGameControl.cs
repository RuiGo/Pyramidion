using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveGameControl : MonoBehaviour {
    public static SaveGameControl saveGameControl;    
    public int gameGold = -1;

    private string saveFilePath;

    private void Awake () {
        if(saveGameControl == null) {
            DontDestroyOnLoad(saveGameControl);
            saveGameControl = this;
        } else if(saveGameControl != this) {
            Destroy(gameObject);
        }

        saveFilePath = Application.persistentDataPath + "/playerInfo.dat";
    }

    private void Update () {
        /*
        if(Input.GetKeyDown(KeyCode.I)) {
            SaveData();
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadData();
        }
        */
    }


    public void SaveData() {
        //print(saveFilePath);
        BinaryFormatter binFormatter = new BinaryFormatter();
        FileStream file;
        if (!File.Exists(saveFilePath)) {
            file = File.Create(saveFilePath);
        } else {
            file = File.Open(saveFilePath, FileMode.Open);
        }        
        PlayerData data = new PlayerData();
        data.currentGold = gameGold;
        binFormatter.Serialize(file, data);
        file.Close();
        print("Saved Gold -> " + gameGold);
    }

    public void LoadData() {
        if (File.Exists(saveFilePath)) {
            BinaryFormatter binFormatter = new BinaryFormatter();
            FileStream file = File.Open(saveFilePath, FileMode.Open);
            PlayerData loadedData = (PlayerData)binFormatter.Deserialize(file);
            file.Close();
            gameGold = loadedData.currentGold;
            print("Loaded Gold -> " + loadedData.currentGold);
        } else {
            print("No save file exists.");
            print("Adding 1000 gold by default.");
            gameGold = 1000;
        }
    }
}

[Serializable]
class PlayerData {
    public int currentGold;
}