namespace LibraryManagement.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsReserved { get; set; }
        public bool IsBorrowed { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
