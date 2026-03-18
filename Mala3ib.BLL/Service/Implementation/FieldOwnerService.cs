namespace Mala3ib.BLL.Service.Implementation
{
    public class FieldOwnerService : IFieldOwnerService
    {
        private readonly IFieldOwnerRepo _fieldOwnerRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public FieldOwnerService(IFieldOwnerRepo fieldOwnerRepo, UserManager<ApplicationUser> userManager)
        {
            _fieldOwnerRepo = fieldOwnerRepo;
            _userManager = userManager;
        }

        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequestDto request)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
            {
                return Result.Success();
            }

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, ErrorType.BadRequest));
        }

        public async Task<Result> DeleteAsync(string userId, CancellationToken cancellation = default)
        {
            return await _fieldOwnerRepo.DeleteAsync(userId, cancellation);
        }

        public async Task<Result<FieldOwnerProfileDto>> GetAsync(string userId, CancellationToken cancellation = default)
        {
            var fieldOwner = await _fieldOwnerRepo.Get(userId)
                .Select(p => new FieldOwnerProfileDto(
                    p.User.Email!,
                    p.User.FirstName,
                    p.User.LastName,
                    p.User.PhoneNumber!,
                    p.DateOfBirth,
                    p.IsApproved
                ))
                .FirstOrDefaultAsync(cancellation);

            if (fieldOwner is null)
                return Result.Failure<FieldOwnerProfileDto>(FieldOwnerErrors.NotFound);

            return Result.Success(fieldOwner);
        }

        public async Task<Result> UpdateAsync(string userId, UpdateFieldOwnerRequestDto request, CancellationToken cancellation = default)
        {
            var fieldOwner = new FieldOwner
            {
                IsApproved = request.IsApproved,
                DateOfBirth = request.DateOfBirth,
                User = new ApplicationUser
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber
                }
            };

            return await _fieldOwnerRepo.UpdateAsync(userId, fieldOwner, cancellation);
        }
    }
}