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
using System;
public class InsertIntoDB : MonoBehaviour
{
    public GameObject UserTextPrefab; // Referencia al Prefab
    public Transform UserListContainer; // Referencia al contenedor
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
        user = new User();
        string conn = SetDataBaseClass.SetDataBase(DatabaseNombre + ".db");
        IDbConnection dbcon = new SqliteConnection(conn);
        dbcon.Open();
        IDbCommand dbcmd = dbcon.CreateCommand();

        string SQLQuery = "SELECT * FROM Users WHERE Nombre = '" + nombre + "'";
        dbcmd.CommandText = SQLQuery;
        IDataReader reader = dbcmd.ExecuteReader();
   
        while (reader.Read())
        {


            user.Id = reader.GetInt32(0);
            user.nombre = reader.GetString(1);

            //ERROR
            //int nivel1Value = reader.GetInt32(2);  // Asumimos que Nivel1 se almacena como entero 0 (falso) o 1 (verdadero) 
            //user.nivel1 = nivel1Value != 0; //ERROR

            //user.puntos = reader.GetInt32(3); //ERROR
            //user.estrellas = reader.GetInt32(4); //ERROR

            Debug.Log("ID: " + user.Id);
            Debug.Log("Nombre: " + user.nombre);
            //Debug.Log("Nivel 1: " + user.nivel1);
            //Debug.Log("Puntos: " + user.puntos);
            //Debug.Log("Estrellas: " + user.estrellas);


        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
    }
}


