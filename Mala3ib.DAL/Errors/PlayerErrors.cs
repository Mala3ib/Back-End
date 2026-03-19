using Mala3ib.DAL.Enums;

namespace Mala3ib.DAL.Errors
{
    public static class PlayerErrors
    {
        public static Error NotFound
            = new Error("Player.NotFound", "Player not found", ErrorType.NotFound);
              
    }
}
