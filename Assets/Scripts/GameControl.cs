using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour {
    public static GameControl gameControl;    
    public int gold;

    private string saveFilePath;

    private void Awake () {
        if(gameControl == null) {
            DontDestroyOnLoad(gameControl);
            gameControl = this;
        } else if(gameControl != this) {
            Destroy(gameObject);
        }

        saveFilePath = Application.persistentDataPath + "/playerInfo.dat";
    }

    private void Update () {
        if(Input.GetKeyDown(KeyCode.I)) {
            SaveData();
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadData();
        }
    }


    public void SaveData() {
        print(saveFilePath);
        BinaryFormatter binFormatter = new BinaryFormatter();
        FileStream file;
        if (!File.Exists(saveFilePath)) {
            file = File.Create(saveFilePath);
        } else {
            file = File.Open(saveFilePath, FileMode.Open);
        }        
        PlayerData data = new PlayerData();
        data.currentGold = gold;
        binFormatter.Serialize(file, data);
        file.Close();
    }

    public void LoadData() {
        if (File.Exists(saveFilePath)) {
            BinaryFormatter binFormatter = new BinaryFormatter();
            FileStream file = File.Open(saveFilePath, FileMode.Open);
            PlayerData loadedData = (PlayerData)binFormatter.Deserialize(file);
            file.Close();
            print("Gold -> " + loadedData.currentGold);
        } else {
            print("No save file exists.");
        }
    }
}

[Serializable]
class PlayerData {
    public int currentGold;
}