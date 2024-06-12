using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;

public static class PlayerData
{
    public static int Id { get; set; }
    public static string Nombre { get; set; }
    public static int NivelTerminado { get; set; }
    public static int PuntosGanados { get; set; }
    public static int EstrellasGanadas { get; set; }
    public static string DatabaseNombre = "Database";

  

    public static void ActualizarData()
    {
        string conn = SetDataBaseClass.SetDataBase(DatabaseNombre + ".db");

        using (IDbConnection dbcon = new SqliteConnection(conn))
        {
            dbcon.Open();
            using (IDbCommand dbcmd = dbcon.CreateCommand())
            {
                string SQLQuery = @"
                    UPDATE Users 
                    SET 
                        NivelTerminado = @nivelTerminado, 
                        PuntosGanados = @puntosGanados, 
                        EstrellasGanadas = @estrellasGanadas
                    WHERE 
                        Nombre = @nombre";

                dbcmd.CommandText = SQLQuery;
                dbcmd.Parameters.Add(new SqliteParameter("@nombre", Nombre));
                dbcmd.Parameters.Add(new SqliteParameter("@nivelTerminado", NivelTerminado));
                dbcmd.Parameters.Add(new SqliteParameter("@puntosGanados", PuntosGanados));
                dbcmd.Parameters.Add(new SqliteParameter("@estrellasGanadas", EstrellasGanadas));

                dbcmd.ExecuteNonQuery();
            }
        }
    }
}
