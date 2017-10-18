namespace Exchange.DAL.Managers
{
    public interface IDbManager
    {
        void CreateDb(string connectionString);
        void DeleteDb(string connectionString);
    }
}
