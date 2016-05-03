using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

namespace dbShowDepends
{
    class DbParams
    {
        public string ServerName { get; set; }
        public string ServerLogin { get; set; }
        public string ServerPassword { get; set; }
        public bool ServerWinAuth { get; set; }
        public string DbName { get; set; }

        public DbParams(string serverName, bool serverWinAuth, string serverLogin,
            string serverPassword, string dbName)
        {
            ServerName = serverName;
            ServerWinAuth = serverWinAuth;
            ServerLogin = serverLogin;
            ServerPassword = serverPassword;
            DbName = dbName;
        }
    }

    class DbLayer
    {
        private SqlConnection _sc1;
        private SqlConnection sc
        {
            get
            {
                PrepareConnection();
                return _sc1;
            }
        }

        readonly DbParams _dbParams;

        public DbLayer(DbParams dbParams)
        {
            _dbParams = dbParams;
        }

        public void Connect()
        {
            var b = new SqlConnectionStringBuilder();
            if (_dbParams.ServerWinAuth)
            {
                b.IntegratedSecurity = true;
                b.DataSource = _dbParams.ServerName;
                b.InitialCatalog = _dbParams.DbName;
            }
            else
            {
                b.IntegratedSecurity = false;
                b.DataSource = _dbParams.ServerName;
                b.UserID = _dbParams.ServerLogin;
                b.Password = _dbParams.ServerPassword;
                b.InitialCatalog = _dbParams.DbName;
            }
            _sc1 = new SqlConnection(b.ToString());

        }

        public void Disconnect()
        {
            if (_sc1 == null)
                return;

            if (_sc1.State == ConnectionState.Open)
                _sc1.Close();
            _sc1 = null;
        }

        // Подготовка соединения с базой данных
        private void PrepareConnection()
        {
            if (_sc1 == null)
                Connect();

            Debug.Assert(_sc1 != null, "var '_sc1' should be not null");
            if (_sc1.State != ConnectionState.Open)
                _sc1.Open();
        }

        // Имя текущей БД
        public string GetCurrentDbName()
        {
            string sQuery = dbQueries.getDbName;
            var scom = new SqlCommand(sQuery, sc) { CommandType = CommandType.Text };
            var res = scom.ExecuteScalar();
            return res.ToString();
        }

        // Изменить текущую БД
        public void ChangeDb(string databaseName)
        {
            string sQuery = dbQueries.changeDB.Replace("<DB>", databaseName);
            var scom = new SqlCommand(sQuery, sc);
            scom.ExecuteNonQuery();
        }

        // Получить список доступных баз данных
        public List<DbDataRecord> GetDatabaseList()
        {
            var recs = new List<DbDataRecord>();

            string sQuery = dbQueries.databaseList;
            var scom = new SqlCommand(sQuery, sc) {CommandType = CommandType.Text};

            using (var reader = scom.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    /*
                    foreach (DbDataRecord dr in reader)
                    {
                        recs.Add(dr);
                    }
                     */
                    recs.AddRange(reader.Cast<DbDataRecord>());
                }
                reader.Close();
            }
            return recs;
        }

        // Поиск объектов по переданной строке
        public List<DbDataRecord> GetObjectList(string searchName, List<DbObjectType> objTypes)
        {
            string sqlTypes = string.Join(" ,", objTypes.Select(c => "'" + c + "'"));

            var recs = new List<DbDataRecord>();
            string sQuery = dbQueries.objectList;
            sQuery = sQuery.Replace("<objTypes>", sqlTypes);

            var scom = new SqlCommand(sQuery, sc) { CommandType = CommandType.Text };
            scom.Parameters.Add("@SearchName", SqlDbType.VarChar, 250);
            scom.Parameters["@SearchName"].Value = searchName;


            using (var reader = scom.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    //foreach (DbDataRecord dr in reader)
                    //{
                    //    recs.Add(dr);
                    //}
                    recs.AddRange(reader.Cast<DbDataRecord>());
                }
                reader.Close();
            }
            return recs;
        }

        // Вернуть Id рабочей таблицы
        public string GetTempTableId()
        {
            string sQuery = dbQueries.tempTableId;
            var scom = new SqlCommand(sQuery, sc) {CommandType = CommandType.Text};
            var o = scom.ExecuteScalar();

            return o is DBNull ? "NULL" : o.ToString();
        }

        // Подготовить рабочую таблицу
        public void PrepareTempTable()
        {
            string sQuery = dbQueries.tempTableId;
            var scom = new SqlCommand(sQuery, sc) {CommandType = CommandType.Text};
            var res = scom.ExecuteScalar();
            if (!(res is DBNull))
                return;

            sQuery = dbQueries.prepareWeakObjects;
            scom = new SqlCommand(sQuery, sc) {CommandType = CommandType.Text};
            scom.ExecuteNonQuery();

            sQuery = dbQueries.tempTableId;
            scom = new SqlCommand(sQuery, sc) {CommandType = CommandType.Text};
            res = scom.ExecuteScalar();

            Debug.Assert(!(res is DBNull), "!(res is DBNull)");

        }

        // Получить список объектов, зависимых от указанного
        public List<DbDataRecord> GetReferencedObjects(string objName)
        {
            var recs = new List<DbDataRecord>();
            string sQuery = dbQueries.findReferencedObjs;

            var scom = new SqlCommand(sQuery, sc) {CommandType = CommandType.Text};
            scom.Parameters.Add(new SqlParameter("@ObjectName", SqlDbType.VarChar, 250));
            scom.Parameters["@ObjectName"].Value = objName;

            using (var reader = scom.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    //foreach (DbDataRecord dr in reader)
                    //{
                    //    recs.Add(dr);
                    //}
                    recs.AddRange(reader.Cast<DbDataRecord>());
                }
                reader.Close();
            }

            return recs;
        }

        /// <summary>
        /// Получить исходный текст скриптового объекта
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="objType">Возвращается тип объекта</param>
        /// <returns>Исходный текст объекта</returns>
        public string GetObjectSource(string objName, ref string objType)
        {
            string srcCode;
            string sQuery = dbQueries.showSource;

            var scom = new SqlCommand(sQuery, sc) {CommandType = CommandType.Text};
            scom.Parameters.Add(new SqlParameter("@ObjectName", SqlDbType.VarChar, 250));
            scom.Parameters["@ObjectName"].Value = objName;

            using (var reader = scom.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    //read only first record
                    reader.Read();
                    srcCode = ((IDataRecord)reader)["definition"].ToString();
                    objType = ((IDataRecord)reader)["type"].ToString();
                }
                else
                {
                    srcCode = "<not avaible>";
                    objType = null;
                }
                reader.Close();
            }
            return srcCode;
        }
    }
}
