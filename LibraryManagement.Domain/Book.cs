using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain
{
    public class Book : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public bool IsReserved { get; set; }
        public bool IsBorrowed { get; set; }
        public int ISBN { get; set; }
        public DateTime? ReturnDate { get; set; } = null;
    }


}
