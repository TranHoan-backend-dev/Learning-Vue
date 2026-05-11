using System.ComponentModel.DataAnnotations;
using MISA.Common.Attributes;
using MISA.Common.Base;

namespace MISA.Common.Model;

[ConfigTable("pa_salary_composition_system")]
public class SalaryCompositionSystem : BaseModel
{
    [Key] [ConfigColumn("salary_component_system_id")] 
    public Guid SalaryComponentSystemId { get; set; } = Guid.NewGuid();

    [ConfigColumn("salary_component_system_name")] 
    [Required] 
    [MaxLength(255)] 
    public required string SalaryComponentSystemName { get; set; }

    [ConfigColumn("description")] 
    public string? Description { get; set; }
}
