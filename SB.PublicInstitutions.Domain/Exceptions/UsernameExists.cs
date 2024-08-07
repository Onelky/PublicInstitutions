namespace SB.PublicInstitutions.Domain.Exceptions
{
    public sealed class UsernameExists : BadRequestException
    {
        public UsernameExists(string name)
            : base($"Username {name} already exists")
        {
        }
    }
}

