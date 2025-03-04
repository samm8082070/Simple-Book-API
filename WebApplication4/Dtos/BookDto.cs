namespace WebApplication4.Dtos
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int NumberOfPages { get; set; }
        public DateOnly PublishDate { get; set; }
        public List<GenreDto> Genres { get; set; } = new List<GenreDto>(); // Nested Genre DTO
    }
}
