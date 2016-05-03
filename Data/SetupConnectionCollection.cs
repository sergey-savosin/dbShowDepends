using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace dbShowDepends.Data
{
    public class SetupConnectionCollection
    {
        List<SetupConnection> connections = new List<SetupConnection>();

        public List<SetupConnection> Connections
        {
            get { return connections; }
            set { connections = value; }
        }
    }
}
