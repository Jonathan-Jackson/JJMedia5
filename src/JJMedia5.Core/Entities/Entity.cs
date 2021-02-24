namespace JJMedia5.Core.Entities {

    public abstract class Entity {
        public int Id { get; set; }

        public string GetCacheKey()
            => $"{GetType().Name}_{Id}";
    }
}