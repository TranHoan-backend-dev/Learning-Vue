using System.ComponentModel.DataAnnotations;
using MISA.Common.Attributes;
using MISA.Common.Base;
using MISA.Common.Resources;

namespace MISA.Common.Model;

[ConfigTable("Candidate")]
public class Candidate : BaseModel
{
    [Key] [ConfigColumn("candidate_id")] public Guid CandidateId { get; set; }

    [ConfigColumn("name")]
    [ConfigSearchableColumn]
    public required string Name { get; set; }

    [CheckDuplicate(nameof(ResourcesVN.DuplicatedPhone))]
    [ConfigColumn("phone")]
    [ConfigSearchableColumn]
    public required string Phone { get; set; }

    [CheckDuplicate(nameof(ResourcesVN.DuplicatedEmail))]
    [ConfigColumn("email")]
    [ConfigSearchableColumn]
    public required string Email { get; set; }

    [ConfigColumn("hiring_campaign")]
    [ConfigSearchableColumn]
    public required string HiringCampaign { get; set; }

    [ConfigColumn("hiring_position")]
    [ConfigSearchableColumn]
    public required string HiringPosition { get; set; }

    [ConfigColumn("hiring_round")] public required string HiringRound { get; set; }

    [ConfigColumn("review")] public required string Review { get; set; }

    [ConfigColumn("hiring_at")] public DateTime HiringAt { get; set; }

    [ConfigColumn("status")]
    [ConfigSearchableColumn]
    public bool Status { get; set; }

    public Candidate()
    {
    }
}