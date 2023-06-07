namespace JJMedia5.Server.backend.Context.Entity {

    public class Import {
        public int Id { get; set; }

        public string Folder { get; set; }

        public bool IsPolled { get; set; }

        public bool IsActive { get; set; }
    }
}