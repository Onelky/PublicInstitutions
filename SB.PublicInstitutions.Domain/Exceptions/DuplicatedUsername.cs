namespace SB.PublicInstitutions.Domain.Exceptions
{
    public sealed class DuplicatedUsername : BadRequestException
    {
        public DuplicatedUsername(string name)
            : base($"Username {name} already exists")
        {
        }
    }
}

