using Microsoft.Data.Sqlite;
string connectionString = "Data Source=Tienda.db;";//coloco la base dato archivo
//string connectionString = "Data Source=base_test.db;";

// Crear conexión a la base de datos
using (SqliteConnection connection = new SqliteConnection(connectionString))//creo un objeto para la conexion
{
    connection.Open();
    // Crear tabla si no existe
    // por lo general este tipo de consultas no se implementa en un porgrama real
    // la aplicamos para poder crear nuestra base de datos desde cero
    string createTableQuery = "CREATE TABLE IF NOT EXISTS productos (id INTEGER PRIMARY KEY, nombre TEXT, precio REAL)";
    using (SqliteCommand createTableCmd = new SqliteCommand(createTableQuery, connection))//comando, query y la conexion
    {
        createTableCmd.ExecuteNonQuery();//executeNonQuery es para cuando no espero un resultado, como en INSERT, UPDATE o DELETE.
        Console.WriteLine("Tabla 'productos' creada o ya existe.");
    }
    
    // Insertar datos
    string insertQuery = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (4, 8, 40)";
            using (SqliteCommand insertCmd = new SqliteCommand(insertQuery, connection))//comando, query y la conexion
            {
                insertCmd.ExecuteNonQuery();//executeNonQuery es para cuando no espero un resultado, como en INSERT, UPDATE o DELETE.
                Console.WriteLine("Datos insertados en la tabla 'Presupuesto'."); //Si quisieras leer resultados (como con SELECT), usarías ExecuteReader().
            }
    // Leer datos
            string selectQuery = "SELECT * FROM PresupuestosDetalle";
            using (SqliteCommand selectCmd = new SqliteCommand(selectQuery, connection))//comando, query y la conexion
            using (SqliteDataReader reader = selectCmd.ExecuteReader())//ExecuteReader() devuelve un objeto lector (SqliteDataReader) que permite recorrer las filas.
            {
                Console.WriteLine("Datos en la tabla 'PresupuestosDetalle':");
                while (reader.Read())//while (reader.Read()) recorre cada fila hasta que no haya más.
                {
                    Console.WriteLine($"ID: {reader["idPresupuesto"]}, idProducto: {reader["idProducto"]}, Cantidad: {reader["Cantidad"]}");
                }
            }

            connection.Close();
}