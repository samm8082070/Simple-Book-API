namespace WebApplication4.Repositories
{
    public interface IBookImportExportRepository
    {
        Task ImportBooksFromCsv(Stream stream);
        Task<string> ExportBooksToCsvAsync();
    }
}
