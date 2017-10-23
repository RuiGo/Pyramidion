using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour {
    public static GameControl gameControl;
    private string saveFilePath = Application.persistentDataPath + "/playerInfo.dat";
    public int gold;
    public int level;

    private void Awake () {
        if(gameControl == null) {
            DontDestroyOnLoad(gameControl);
            gameControl = this;
        } else if(gameControl != this) {
            Destroy(gameObject);
        }
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
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveFilePath); // se não houver ficheiro
        print(saveFilePath);
        PlayerData data = new PlayerData();
        data.gold = gold;
        data.level = level;
        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadData() {
        if (File.Exists(saveFilePath)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();
            print("Gold -> " + data.gold);
            print("Level -> " + data.level);
        }
    }
}

[Serializable]
class PlayerData {
    public int gold;
    public int level;
}