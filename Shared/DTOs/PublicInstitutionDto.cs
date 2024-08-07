using SB.PublicInstitutions.Domain.Enums;

namespace Shared;

public sealed class PublicInstitutionDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public string Director { get; set; }
    public string? Website { get; set; }
    public GovermentBranch GovermentBranch { get; set; }
    public InstitutionType InstitutionType { get; set; }
}