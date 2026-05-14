namespace Application.UseCases.Events.Queries
{
    public class GetEventsQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
