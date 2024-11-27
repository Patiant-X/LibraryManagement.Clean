namespace LibraryManagement.Application.Features.Books.Queries.GetAllBooks
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ISBN { get; set; }
        public bool IsReserved { get; set; }
        public bool IsBorrowed { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
