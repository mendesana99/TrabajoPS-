namespace Application.UseCases.Events.Queries
{
    public class GetSeatsByEventQuery
    {
        public int EventId { get; set; }

        public GetSeatsByEventQuery(int eventId)
        {
            EventId = eventId;
        }
    }
}
