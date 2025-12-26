
namespace Account.Domain.DTO.Discipline
{
    public class CreateDisciplineDto
    {
        public string Name { get; set; }
        public List<long>? IndicatorIds { get; set; } = new();
    }
}
