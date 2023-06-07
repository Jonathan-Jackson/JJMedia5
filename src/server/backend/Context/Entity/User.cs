using System;

namespace JJMedia5.Server.backend.Context.Entity {

    public class User {

        public User() {
        }

        public User(string name) {
            Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}