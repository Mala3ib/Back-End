namespace Mala3ib.DAL.Abstraction
{
    public record Error(string Code, string Description, int? StatusCode)
    {
        public static readonly Error None = new Error(string.Empty,string.Empty, null);
    }
}
