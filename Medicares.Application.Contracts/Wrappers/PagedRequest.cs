namespace Medicares.Application.Contracts.Wrappers
{
    public abstract class PagedRequest
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string StringSearch { get; set; } = string.Empty;
        public string Orderby { get; set; } = string.Empty;
    }
}
