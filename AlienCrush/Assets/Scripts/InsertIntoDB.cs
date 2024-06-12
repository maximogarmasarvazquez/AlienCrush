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
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
public class InsertIntoDB : MonoBehaviour
{
    public GameObject UserTextPrefab; // Referencia al Prefab
    public Transform UserListContainer; // Referencia al contenedor
    public string DatabaseNombre;
    public InputField NombreInput;
    public User user;
    public Text textElement;
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
        string SQLQuery = "Insert Into Users(Nombre) Values(@nombre)";
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = SQLQuery;
        dbcmd.Parameters.Add(new SqliteParameter("@nombre", _NombreInput));
        reader = dbcmd.ExecuteReader();

        SelectUser(_NombreInput);

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


           user.Id = int.Parse(reader["ID"].ToString());
        
            user.nombre = reader["Nombre"].ToString();


            try
            {
                int NivelTerminado = int.Parse(reader["NivelTerminado"].ToString());  // Asumimos que Nivel1 se almacena como entero 0 (falso) o 1 (verdadero) 

                user.nivel1 = NivelTerminado != 0;
            }
            catch (Exception )
            {
                user.nivel1 = false;
            }

            try
            {
                user.puntosGanados = int.Parse(reader["PuntosGanados"].ToString());
            } catch (Exception )
            {
                user.puntosGanados = 0;
            }


            try
            {
                user.estrellasGanadas = int.Parse(reader["EstrellasGanadas"].ToString());
            }
            catch (Exception)
            {
                user.estrellasGanadas = 0;
            }

            Debug.Log("ID: " + user.Id);
            Debug.Log("Nombre: " + user.nombre);
            Debug.Log("Nivel 1: " + user.nivel1);
            Debug.Log("Puntos: " + user.puntosGanados);
            Debug.Log("Estrellas: " + user.estrellasGanadas);

            
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
       
        // Guardar la información del jugador seleccionado
        PlayerData.Id = user.Id;
        PlayerData.Nombre = user.nombre;
        PlayerData.NivelTerminado = user.nivel1 ? 1 : 0; // Asume que solo hay un nivel por ahora
        PlayerData.PuntosGanados = user.puntosGanados;
        PlayerData.EstrellasGanadas = user.estrellasGanadas;
        
        SceneManager.LoadScene(2);
        
    }


}


