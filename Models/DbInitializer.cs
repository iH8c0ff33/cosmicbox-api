namespace CosmicBox.Models {
    public static class DbInitializer {
        public static void Initialize(ApiContext context) {
            context.Database.EnsureCreated();
            context.SaveChanges();
        }
    }
}