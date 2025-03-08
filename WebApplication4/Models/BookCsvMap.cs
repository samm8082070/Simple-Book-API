using CsvHelper.Configuration;

namespace WebApplication4.Models
{
    public class BookCsvMap : ClassMap<Book>
    {


        public BookCsvMap()
        {
            Map(m => m.Title).Name("Title");
            Map(m => m.Author).Name("Author");
            Map(m => m.NumberOfPages).Name("NumberOfPages");
            Map(m => m.PublishDate).Name("Publish Date").TypeConverterOption.Format("yyyy-MM-dd");

            
        }

       
        
    }
}
