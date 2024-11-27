namespace LibraryManagement.Application.Features.Books.Commands.CreateBook
{
    public class CreateBookDto
    {
        public int Id { get; set; }
        public int ISBN { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsReserved { get; set; }
        public bool IsBorrowed { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
