namespace LibraryManagement.Application.Features.Notifications.Dto
{
    public class NotificationDto
    {
        public int BookId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public bool IsNotified { get; set; }
        public int Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
