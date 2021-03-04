using System;

namespace JJMedia5.Core.Attributes {

    [AttributeUsage(AttributeTargets.All)]
    public class RepositoryAttribute : Attribute {

        public RepositoryAttribute() {
        }

        public string TableName { get; set; }
    }
}