using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace ManejoPresupuesto.Services
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TiposCuentas tiposCuenta);
        Task Borrar(int idTipoCuenta);
        Task Crear(TiposCuentas tiposCuenta);
        Task<bool> Existe(string nombre, int idUsuario);
        Task<IEnumerable<TiposCuentas>> Obtener(int idUsuario);
        Task<TiposCuentas> ObtenerPorId(int idTipoCuenta, int idUsuario);
    }
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task Crear(TiposCuentas tiposCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var idTipoCuenta = await connection.QuerySingleAsync<int>(@"INSERT INTO TiposCuentas(Nombre, IdUsuario,Orden)
                                                            VALUES(@Nombre, @IdUsuario, 0);
                                                            SELECT SCOPE_IDENTITY();", tiposCuenta);
            tiposCuenta.IdTipoCuenta = idTipoCuenta;
        }
        //Validacion para retornar por pantalla
        //que el nombre ingresado ya existe
        public async Task<bool> Existe(string nombre, int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            //Trae el primer registro con ese nombre o un valor designado en este caso INT que equivale a 0
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1
                                                                        FROM TiposCuentas
                                                                        WHERE Nombre = @Nombre AND IdUsuario = @IdUsuario;",new {nombre, idUsuario});
            return existe == 1;
        }
        public async Task<IEnumerable<TiposCuentas>>Obtener(int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TiposCuentas>(@"SELECT IdTipoCuenta, Nombre,Orden
                                                             FROM TiposCuentas 
                                                           WHERE IdUsuario=@IdUsuario", new { idUsuario });
        }
        public async Task Actualizar(TiposCuentas tiposCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                          SET Nombre = @Nombre
                                          WHERE IdTipoCuenta = @IdTipoCuenta", tiposCuenta);
            //ExecuteAsync: ejecuta un query que no retorna nada como un UPDATE
        }
        public async Task<TiposCuentas>ObtenerPorId(int idTipoCuenta, int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TiposCuentas>(@"SELECT IdTipoCuenta,Nombre,Orden
                                                                            FROM TiposCuentas
                                                                            WHERE IdTipoCuenta= @IdTipoCuenta AND IdUsuario=@IdUsuario", new {idTipoCuenta,idUsuario});
        }

        public async Task Borrar(int idTipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE TiposCuentas WHERE IdTipoCuenta= @IdTipoCuenta", new { idTipoCuenta });
        }
    }
}
