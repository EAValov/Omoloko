using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace OrderPaymentApp.Model.Repository
{
    /// <summary>
    /// Базовый репозиторий.
    /// </summary>
    public abstract class RepositoryBase
    {
        /// <summary>
        /// Строка соединения с сервером
        /// </summary>
        protected string ConnectionString { get; set; }

        /// <summary>
        /// Наименование БД по умолчанию.
        /// </summary>
        protected string DefaultDatabase { get; set; }

        /// <summary>
        /// Базовый конструктор репозитория.
        /// </summary>
        /// <param name="connection_string">Строка соединения с базой.</param>
        /// <param name="default_database">Наименование БД по умолчанию.</param>
        protected RepositoryBase(string connection_string, string default_database)
        {
            if (string.IsNullOrWhiteSpace(connection_string))
                throw new Exception("Cтрока соединения не заполнена или пустая.");

            this.ConnectionString = connection_string;
            this.DefaultDatabase = default_database;
        }

        /// <summary>
        /// Создание нового коннекшна.
        /// </summary>
        /// <returns>Экземпляр IDbConnection</returns>
        protected IDbConnection GetConnection()
        {
            var conn = new SqlConnection(ConnectionString);

            if (conn.State != ConnectionState.Open)
                conn.Open();

            conn.ChangeDatabase(DefaultDatabase);

            return conn;
        }
    }
}
