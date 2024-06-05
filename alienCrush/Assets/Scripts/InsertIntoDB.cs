using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using Mono.Data;
using System.Data;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.SocialPlatforms;
using UnityEditor;
public class InsertIntoDB : MonoBehaviour
{
    public GameObject UserTextPrefab; // Referencia al Prefab
    public Transform UserListContainer; // Referencia al contenedor
    public InputField IDInput;
    public Text NombreText;
    public string DatabaseNombre;
    public InputField NombreInput;
    public User user;

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
                          "Values('" + _NombreInput + "') ";
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


    public void FindItem()
    {

        var _IDInput = IDInput.text.Trim();
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

    public void listaUser()
    {
     
    
        string conn = SetDataBaseClass.SetDataBase(DatabaseNombre + ".db");
        IDbConnection dbcon;
        IDbCommand dbcmd;
        IDataReader reader;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();

       string SQLQuery = "SELECT Nombre FROM Users ";

        dbcmd.CommandText = SQLQuery;
       
        reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            // Instanciamos un nuevo elemento de usuario a partir del Prefab
            GameObject newUserButton = Instantiate(UserTextPrefab, UserListContainer);
            // Configuramos el texto del elemento
            newUserButton.GetComponentInChildren<Text>().text = reader.GetString(0);
            newUserButton.GetComponent<Button>().onClick.AddListener(() => SelectUser(newUserButton.GetComponentInChildren<Text>().text));
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;


    }

    public void SelectUser(string nombre)
    {
        Debug.Log("Botón presionado: " + nombre);
        // Aquí puedes añadir cualquier funcionalidad adicional que necesites al seleccionar un usuario
    }
}
