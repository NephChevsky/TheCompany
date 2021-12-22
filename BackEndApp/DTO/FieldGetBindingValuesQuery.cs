using ModelsApp.Models;

namespace BackEndApp.DTO
{
    public class FieldGetBindingValuesQuery
    {
        public string DataSource { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
