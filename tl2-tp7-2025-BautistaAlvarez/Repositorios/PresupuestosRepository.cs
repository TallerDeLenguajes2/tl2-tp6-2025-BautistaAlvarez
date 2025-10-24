
using Microsoft.Data.Sqlite;
using tl2_tp7_2025_BautistaAlvarez.Models;

public class PresupuestosRepository
{
    string cadenaConexion = "Data Source=Tienda.db"; //conexion para todo el repositorio

    public void CrearPresupuesto(Presupuestos presupuesto)
    {
        using var conexion = new SqliteConnection(cadenaConexion);//usar using
        conexion.Open();//abro la conexion usando using para que se abra y cierre cuando sea deje de usarse

        string sql = "INSERT INTO Presupuestos (idPresupuesto, NombreDestinatario, FechaCreacion) VALUES (@idPresupuesto, @NombreDestinatario, @FechaCreacion)";//codigo sql

        using var comando = new SqliteCommand(sql, conexion);

        comando.Parameters.Add(new SqliteParameter("@idPresupuesto", presupuesto.IdPresupuesto));//cambio los valores por los valores que le doy por la funcion
        comando.Parameters.Add(new SqliteParameter("@nombreDestinatario", presupuesto.NombreDestinatario));
        comando.Parameters.Add(new SqliteParameter("@FechaCreacion", presupuesto.FechaCreacion.ToDateTime(TimeOnly.MinValue)));
        //como uso DateOnly debo pasar de dateOnly a DateTime con el comando.ToDateTime() y dentro del parentesis van los minutos que inicio por eso timeonly.minvalue

        comando.ExecuteNonQuery();//ejecuto sino tengo que mostrar nada
    }

    public List<Presupuestos> ListarPresupuesto()
    {
        var listado = new List<Presupuestos>();//inicio lista

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        string sql = "SELECT * FROM Presupuestos";
        using var comando = new SqliteCommand(sql, conexion);

        using var lector = comando.ExecuteReader();//inicio el reader

        while (lector.Read())//mientras el lector lea
        {
            var p = new Presupuestos
            {
                IdPresupuesto = Convert.ToInt32(lector["idPresupuesto"]),
                NombreDestinatario = lector["NombreDestinatario"].ToString(),
                FechaCreacion = DateOnly.FromDateTime(Convert.ToDateTime(lector["FechaCreacion"]))//paso de DateOnly a DateTime
                //Cuando leés de SQLite, lector["FechaCreacion"] te devuelve un objeto que puede ser: un string ("2025-10-24") o un DateTime (2025-10-24 00:00:00)
                //Convert.ToDateTime(...) convierte lo que sea (texto o fecha) a un DateTime de C#. Me aseguro en transformarlo en date time para luego aplicar dateonly
                //DateOnly.FromDateTime(..) extrae solo la parte de la fecha, descartando la hora. 
            };
            listado.Add(p);//agrego el presupuesto
        }
        return listado;
    }

    public Presupuestos PresupuestoPorId(int idPresupuesto)
    {
        var presupuesto = new Presupuestos();//creo una variable presupuesto para devolver

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        string sqlPresupuesto = "SELECT idPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuesto WHERE idPresupuesto = @idPresupuesto";
        var comandoPresupuesto = new SqliteCommand(sqlPresupuesto, conexion);

        comandoPresupuesto.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));

        using var lectorPresupuesto = comandoPresupuesto.ExecuteReader(); //ejecuto el lector
        if (lectorPresupuesto.Read())//si encontro algo
        {
            presupuesto.IdPresupuesto = Convert.ToInt32(lectorPresupuesto["idPresupuesto"]); //convierto y agrego los datos encontrados
            presupuesto.NombreDestinatario = lectorPresupuesto["NombreDestinatario"].ToString();
            presupuesto.FechaCreacion = DateOnly.FromDateTime(Convert.ToDateTime(lectorPresupuesto["FechaCreacion"]));//converto a datetime por las dudas y luego uso date only.fromdate time para sacar solo la fecha
        }
        //lectorPresupuesto.Close(); aqui tendria que cerrar el lector pero como uso using ya lo hace solo

        //el arroba es para poder escribir de esta manera, el join sirve para unir 2 o mas tablas a partir de una coincidencia como por ejemplo el id de los productos
        // de FROM seria la tabla principal, de JOIN esta la tabla a la cual se relacion, d y p son las alias de las tablas una abreviatura para luego hacer ON d.algo = p.algo
        string sqlDetalle = @"
        SELECT d.idProducto, d.Cantidad, p.Descripcion, p.Precio
        FROM PresupuestosDetalle d
        JOIN Productos p ON d.idProducto = p.idProducto
        WHERE d.idPresupuesto = @idPresupuesto";


        //Este query seria si quiero combinar las 3 tablas pero no es recomendable ya que para los datos del presupuesto necesito pasar una sola vez en cambio para su listado si necesito el while
        /*
        string query = @"SELECT pr.id_presupuesto, pr.nombre_destinatario, pr.fecha_creacion,
                        p.id_producto, p.descripcion, p.precio, d.cantidad
                 FROM Presupuestos pr
                 JOIN PresupuestosDetalle d ON pr.id_presupuesto = d.id_presupuesto
                 JOIN Productos p ON d.id_producto = p.id_producto
                 WHERE pr.id_presupuesto = @idPresupuesto;";
        */

        using var comandoDetalle = new SqliteCommand(sqlDetalle, conexion); //nuevo comando con otra orden sql
        comandoDetalle.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));//remplazo el valor con el que le doy al metodo

        using var lectorDetalle = comandoDetalle.ExecuteReader();//ejecuto el lector

        var listaDetalle = new List<PresupuestosDetalle>();//inicio lista de presupuesto detalle
        while (lectorDetalle.Read())//mientras el lector lea
        {
            var p = new Productos//creo el producto en base a la busqueda
            {
                IdProducto = Convert.ToInt32(lectorDetalle["idProducto"]),
                Descripcion = lectorDetalle["Descripcion"].ToString(),
                Precio = Convert.ToInt32(lectorDetalle["Precio"])
            };

            var pDetalle = new PresupuestosDetalle //creo el objeto PresupuestoDetalle
            {
                Producto = p, //agrego el producto formado anteriormente
                Cantidad = Convert.ToInt32(lectorDetalle["Cantidad"])
            };

            listaDetalle.Add(pDetalle);//agrego a la lista de presupuestoDetalle
        }

        //retomando el presupuesto
        presupuesto.Detalle = listaDetalle;//coloco la lista que forme

        return presupuesto;
    }
    public void AgregarProducto(int idPresupuesto, int idProducto, int cantidad)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        string sql = "INSERT INTO PresupuestosDetalle(idPresupuesto, idProducto, Cantidad) VALUES (@idPresupuesto, @idProducto, @Cantidad)";

        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));//manera mas precisa de agregar
        comando.Parameters.AddWithValue("@idProducto", idProducto);//una forma de agregar valores de manera rapida
        comando.Parameters.Add(new SqliteParameter("@Cantidad", cantidad));

        comando.ExecuteNonQuery();
    }
}