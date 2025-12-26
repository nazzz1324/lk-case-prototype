
namespace Account.Domain.DTO.Discipline
{
    public class DisciplineDto
    {
        public long Id { get; set; }
        public string Index { get; set; }
        public string Name { get; set; }
        public List<long>? IndicatorIds { get; set; } = new();

    }
}
