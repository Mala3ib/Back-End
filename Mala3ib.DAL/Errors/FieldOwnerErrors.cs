using Mala3ib.DAL.Enums;

namespace Mala3ib.DAL.Errors
{
    public static class FieldOwnerErrors
    {
        public static Error FieldOwnerNotFound
            = new Error("FieldOwner.NotFound", "Field owner not found", ErrorType.NotFound);
    }
}
