using System;

namespace JJMedia5.Core.Database {

    public class JJMediaDbManager {

        public JJMediaDbManager(string connString) {
            ConnString = connString ?? throw new ArgumentNullException(nameof(connString));
        }

        public string ConnString { get; set; }
    }
}