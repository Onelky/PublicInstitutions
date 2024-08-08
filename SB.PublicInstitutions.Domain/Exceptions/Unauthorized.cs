namespace SB.PublicInstitutions.Domain.Exceptions
{
    public sealed class Unauthorized: Exception
    {
        public Unauthorized(string username)
            : base("Unauthorized user " + username)
        {
        }
    }
}
