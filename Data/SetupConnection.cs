using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbShowDepends.Data
{
    public class SetupConnection
    {
        string name = "new";
        string serverName = "server";
        string dbName = "db";
        bool isAnalyzeBadObjects = false;
        int sortOrder = 0;

        public string ConnectionName
        {
            get { return name; }
            set { name = value; }
        }

        public string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }

        public string DbName
        {
            get { return dbName; }
            set { dbName = value; }
        }

        public bool IsAnalyzeBadObjects
        {
            get { return isAnalyzeBadObjects; }
            set { isAnalyzeBadObjects = value; }
        }

        public int SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }
    }
}
