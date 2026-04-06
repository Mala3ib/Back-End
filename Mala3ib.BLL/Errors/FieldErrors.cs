namespace Mala3ib.BLL.Errors
{
    public static class FieldErrors
    {
        public static Error NotFound
            = new Error("Field.NotFound", "Field not found", ErrorType.NotFound);
        public static Error Unauthorized
            = new Error("Field.Unauthorized", "You dont have access to this field", ErrorType.Unauthorized);

    }
}