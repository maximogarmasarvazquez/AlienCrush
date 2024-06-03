using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using Mono.Data;
using System.Data;
using UnityEngine.UI;
public class InsertIntoDB : MonoBehaviour
{

    public string DatabaseNombre;
    public InputField NombreInput;

    public void InsertInto()
    {
        var _NombreInput = NombreInput.text.Trim();


        string conn = SetDataBaseClass.SetDataBase(DatabaseNombre + ".db");
        IDbConnection dbcon;
        IDbCommand dbcmd;
        IDataReader reader;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();
        string SQLQuery = "Insert Into Users(Nombre)" +
                          "Values('"+_NombreInput+"') ";
        dbcmd.CommandText = SQLQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {

        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
    }


    public InputField IDInput;
    public Text NombreText;

    public void FindItem() {

        var _IDInput =  IDInput.text.Trim();
       NombreText.text = "";


        string conn = SetDataBaseClass.SetDataBase(DatabaseNombre + ".db");
        IDbConnection dbcon;
        IDbCommand dbcmd;
        IDataReader reader;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();
        string SQLQuery = "Select Nombre FROM Users Where ID = '" + _IDInput + "'";
        dbcmd.CommandText = SQLQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            NombreText.text = reader.GetString(0);
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
    }
}
