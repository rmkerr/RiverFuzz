namespace Database
{
    public interface IDatabaseHelper
    {
        void CreateDatabase();
        void DeleteDatabase();
        void CreateIfNotExists();
    }
}