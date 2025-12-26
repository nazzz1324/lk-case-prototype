
namespace Account.Domain.DTO.Discipline
{
    public class DisciplinesDto
    {
        public long Id { get; set; }
        public string Index { get; set; }
        public string Name { get; set; }
        public int IndicatorCount { get; set; }
        public List<long> IndicatorIds { get; set; } = new List<long>();

    }
}
