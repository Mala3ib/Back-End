using Mala3ib.BLL.Contracts.FieldSlot;

namespace Mala3ib.BLL.Service.Implementation
{
    public class FieldSlotService : IFieldSlotService
    {
        private readonly IFieldSlotRepo _fieldSlotRepo;
        private readonly IFieldOwnerRepo _fieldOwnerRepo;
        private readonly IFieldRepo _fieldRepo;
        public FieldSlotService(IFieldSlotRepo fieldSlotRepo, IFieldOwnerRepo fieldOwnerRepo, IFieldRepo fieldRepo)
        {
            _fieldSlotRepo = fieldSlotRepo;
            _fieldOwnerRepo = fieldOwnerRepo;
            _fieldRepo = fieldRepo;
        }
        private async Task<Result> ValidateOwnerAsync(string userId, int fieldId, CancellationToken cancellation)
        {
            var owner = await _fieldOwnerRepo.GetOwnerByUserId(userId)
                .FirstOrDefaultAsync(cancellation);

            var field = await _fieldRepo.GetById(fieldId)
                .FirstOrDefaultAsync(cancellation);

            if (owner is null || field is null)
                return Result.Failure(FieldSlotErrors.NotFound);

            if (owner.Id != field.FieldOwnerId)
                return Result.Failure(FieldSlotErrors.Unauthorized);

            return Result.Success();
        }
        public async Task<Result> AddAsync(AddFieldSlotRequestDto request, string userId, int fieldId, CancellationToken cancellation = default)
        {
            var validation = await ValidateOwnerAsync(userId, fieldId, cancellation);
            if (!validation.IsSuccess)
                return validation;

            var isSlotAvailable = await _fieldSlotRepo.IsSlotAvailableAsync(fieldId, request.StartDate, request.EndDate, null, cancellation);
            if (!isSlotAvailable)
                return Result.Failure(FieldSlotErrors.NotAvialable);

            var fieldSlot = new FieldSlot
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                FieldId = fieldId
            };

            await _fieldSlotRepo.AddAsync(fieldSlot);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, int fieldId, string userId, CancellationToken cancellation = default)
        {
            var validation = await ValidateOwnerAsync(userId, fieldId, cancellation);
            if (!validation.IsSuccess)
                return validation;

            var fieldSlot = await _fieldSlotRepo.GetById(id)
                .FirstOrDefaultAsync(cancellation);

            if (fieldSlot is null)
                return Result.Failure(FieldSlotErrors.NotFound);

            if (fieldSlot.FieldId != fieldId)
                return Result.Failure(FieldSlotErrors.NotFound);

            await _fieldSlotRepo.DeleteAsync(id, cancellation);
            return Result.Success();
        }
        public async Task<Result> UpdateAsync(int id, UpdateFieldSlotRequestDto request, string userId, int fieldId, CancellationToken cancellation = default)
        {
            var validation = await ValidateOwnerAsync(userId, fieldId, cancellation);
            if (!validation.IsSuccess)
                return validation;

            var oldFieldSlot = await _fieldSlotRepo.GetById(id)
                .FirstOrDefaultAsync(cancellation);

            if (oldFieldSlot is null)
                return Result.Failure(FieldSlotErrors.NotFound);

            if (oldFieldSlot.FieldId != fieldId)
                return Result.Failure(FieldSlotErrors.NotFound);

            var isSlotAvailable = await _fieldSlotRepo.IsSlotAvailableAsync(fieldId, request.StartDate, request.EndDate, id, cancellation);
            if (!isSlotAvailable)
                return Result.Failure(FieldSlotErrors.NotAvialable);

            var fieldSlot = new FieldSlot
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsBooked = request.IsBooked
            };
            await _fieldSlotRepo.UpdateAsync(id, fieldSlot, cancellation);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<FieldSlotResponseDto>>> GetAvailableSlots(int fieldId, DateTime? date, CancellationToken cancellation = default)
        {
            var actualDate = date ?? DateTime.Today;

            var fieldSlots = await _fieldSlotRepo.GetAvailableSlots(fieldId, actualDate)
                .Select(f => new FieldSlotResponseDto(
                    f.StartDate,
                    f.EndDate,
                    f.IsBooked
                )).ToListAsync(cancellation);

            return Result.Success<IEnumerable<FieldSlotResponseDto>>(fieldSlots);
        }

        public async Task<Result<IEnumerable<FieldSlotResponseDto>>> GetByFieldIdAsync(int fieldId, CancellationToken cancellation = default)
        {
            var fieldSlots = await _fieldSlotRepo.GetByFieldId(fieldId)
                .Select(f => new FieldSlotResponseDto(
                    f.StartDate,
                    f.EndDate,
                    f.IsBooked
                )).ToListAsync(cancellation);

            return Result.Success<IEnumerable<FieldSlotResponseDto>>(fieldSlots);
        }

        public async Task<Result<FieldSlotResponseDto>> GetByIdAsync(int id, CancellationToken cancellation = default)
        {
            var fieldSlot = await _fieldSlotRepo.GetById(id)
                .Select(f => new FieldSlotResponseDto(
                    f.StartDate,
                    f.EndDate,
                    f.IsBooked
                )).FirstOrDefaultAsync(cancellation);

            if (fieldSlot is null)
                return Result.Failure<FieldSlotResponseDto>(FieldSlotErrors.NotFound);

            return Result.Success(fieldSlot);
        }

    }
}