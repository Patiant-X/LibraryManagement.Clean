using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain
{
    public class Notification : BaseEntity
    {
        public int BookId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public bool IsNotified { get; set; }
    }
}

