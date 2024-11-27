using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Models
{
    public class CustomProblemDetails : ProblemDetails
    {
        public IDictionary<string, string[]> ValidationErrors { get; set; } = new Dictionary<string, string[]>();
    }
}
