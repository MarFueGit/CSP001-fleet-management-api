namespace FleetManagementAPI.Models
{
    public class PaginationMetadata
    {
        public int total { get; set; }
        public int totalPages { get; set; }
        public int currentPage { get; set; }
        public int? nextPage { get; set; }
        public int lastPage { get; set; }
        public IEnumerable<object> data { get; set; }
    }
}
