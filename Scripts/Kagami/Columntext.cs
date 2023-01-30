using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Columntext : MonoBehaviour
{
    public Text OwnerObject;
    private List<string> Charactors = new List<string>();

    private void Awake()
    {
        try
        {
            var fileName = "ainu_DB_ALL.db";
            var db = new SqliteDatabase(fileName);
            var query = db.ExecuteQuery("SELECT * FROM charactors");

            foreach (var row in query.Rows)
            {
                var id = row["ColumnID"];
                var ainu = row["Ainu"];
                var columntext = row["Columntext"];
                var imageurl = row["Imageurl"];

                var text = $"ID:{id}, ainu:{id}, text:{id}, url:{id}";

                Charactors.Add(text);
            }

        }
        catch (Exception ex)
        {
            Charactors.Add(ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (OwnerObject != null && Charactors.Count > 0)
        {
            string text = "";

            if (Charactors.Count > 0)
            {
                text = string.Join("\r\n", Charactors);
            }
            else
            {
                text = "キャラクターが存在しません";
            }

            OwnerObject.text = text;
        }
    }
}
