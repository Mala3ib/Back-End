using Mala3ib.BLL.Contracts.Field;

namespace Mala3ib.BLL.Service.Implementation
{
    public class FieldService : IFieldService
    {
        private readonly IFieldRepo _fieldRepo;

        public FieldService(IFieldRepo fieldRepo)
        {
            _fieldRepo = fieldRepo;
        }

        public async Task<Result<int>> AddAsync(AddFieldRequestDto request, int ownerId, CancellationToken cancellation = default)
        {
            var field = new Field
            {
                Name = request.Name,
                Location = request.Location,
                PricePerHour = request.PricePerHour,
                FieldOwnerId = ownerId
            };
            await _fieldRepo.AddAsync(field);

            return Result.Success(field.Id);
        }

        public async Task<Result> DeleteAsync(int id, int ownerId, CancellationToken cancellation = default)
        {
            var field = await _fieldRepo.GetById(id)
                .FirstOrDefaultAsync(cancellation);

            if (field is null)
                return Result.Failure(FieldErrors.NotFound);

            if (field.FieldOwnerId != ownerId)
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

        public async Task<Result> UpdateAsync(int id, UpdateFieldRequestDto request, int ownerId, CancellationToken cancellation = default)
        {
            var oldField = await _fieldRepo.GetById(id)
                .FirstOrDefaultAsync(cancellation);

            if (oldField is null)
                return Result.Failure(FieldErrors.NotFound);

            if (oldField.FieldOwnerId != ownerId)
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