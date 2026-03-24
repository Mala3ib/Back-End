using Mala3ib.BLL.Contracts.Field;

namespace Mala3ib.BLL.Service.Implementation
{
    public class FieldService : IFieldService
    {
        private readonly IFieldRepo _fieldRepo;
        private readonly IFieldOwnerRepo _fieldOwnerRepo;
        public FieldService(IFieldRepo fieldRepo, IFieldOwnerRepo fieldOwnerRepo)
        {
            _fieldRepo = fieldRepo;
            _fieldOwnerRepo = fieldOwnerRepo;
        }

        public async Task<Result<int>> AddAsync(AddFieldRequestDto request, string userId, CancellationToken cancellation = default)
        {
            var ownerId = await GetOwnerIdByUserIdAsync(userId);
            var field = new Field
            {
                Name = request.Name,
                Location = request.Location,
                PricePerHour = request.PricePerHour,
                FieldOwnerId = ownerId.Value
            };
            await _fieldRepo.AddAsync(field);

            return Result.Success(field.Id);
        }

        public async Task<Result> DeleteAsync(int id, string userId, CancellationToken cancellation = default)
        {
            var field = await _fieldRepo.GetById(id)
                .FirstOrDefaultAsync(cancellation);

            if (field is null)
                return Result.Failure(FieldErrors.NotFound);

            var ownerId = await GetOwnerIdByUserIdAsync(userId);

            if (field.FieldOwnerId != ownerId.Value)
                return Result.Failure(FieldErrors.Unauthorized);

            await _fieldRepo.DeleteAsync(id, cancellation);
            return Result.Success();
        }

        public async Task<Result<IEnumerable<FieldResponseDto>>> GetAllAsync(CancellationToken cancellation = default)
        {
            var fields = await _fieldRepo.GetAll()
                .Select(f => new FieldResponseDto(
                    f.Id,
                    f.Name,
                    f.Location,
                    f.PricePerHour
                )).ToListAsync(cancellation);

            return Result.Success<IEnumerable<FieldResponseDto>>(fields);
        }

        public async Task<Result<FieldResponseDto>> GetByIdAsync(int id, CancellationToken cancellation = default)
        {
            var field = await _fieldRepo.GetById(id)
                .Select(f => new FieldResponseDto(
                    f.Id,
                    f.Name,
                    f.Location,
                    f.PricePerHour
                )).FirstOrDefaultAsync(cancellation);

            if (field is null)
                return Result.Failure<FieldResponseDto>(FieldErrors.NotFound);

            return Result.Success(field);
        }

        public async Task<Result<IEnumerable<FieldResponseDto>>> GetByOwnerIdAsync(int ownerId, CancellationToken cancellation = default)
        {
            var fields = await _fieldRepo.GetByOwnerId(ownerId)
                .Select(f => new FieldResponseDto(
                    f.Id,
                    f.Name,
                    f.Location,
                    f.PricePerHour
                )).ToListAsync(cancellation);


            return Result.Success<IEnumerable<FieldResponseDto>>(fields);
        }

        public async Task<Result<int>> GetOwnerIdByUserIdAsync(string userId, CancellationToken cancellation = default)
        {
            var owner = await _fieldOwnerRepo.GetOwnerByUserId(userId)
                .FirstOrDefaultAsync(cancellation);

            if (owner == null)
                return Result.Failure<int>(FieldOwnerErrors.NotFound);

            return Result.Success(owner.Id);
        }

        public async Task<Result> UpdateAsync(int id, UpdateFieldRequestDto request, string userId, CancellationToken cancellation = default)
        {
            var oldField = await _fieldRepo.GetById(id)
                .FirstOrDefaultAsync(cancellation);

            if (oldField is null)
                return Result.Failure(FieldErrors.NotFound);

            var ownerId = await GetOwnerIdByUserIdAsync(userId);

            if (oldField.FieldOwnerId != ownerId.Value)
                return Result.Failure(FieldErrors.Unauthorized);

            var field = new Field
            {
                Name = request.Name,
                Location = request.Location,
                PricePerHour = request.PricePerHour
            };

            return await _fieldRepo.UpdateAsync(id, field, cancellation);
        }
    }
}