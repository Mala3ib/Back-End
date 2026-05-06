using Mala3ib.BLL.Abstraction;

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

        public async Task<Result<FieldResponseDto>> AddAsync(AddFieldRequestDto request, string userId, CancellationToken cancellation = default)
        {
            var ownerId = await GetOwnerIdByUserIdAsync(userId, cancellation);
            var field = new Field
            {
                Name = request.Name,
                Location = request.Location,
                PricePerHour = request.PricePerHour,
                FieldOwnerId = ownerId.Value
            };

            await _fieldRepo.AddAsync(field);

            var fieldResponse = field.Adapt<FieldResponseDto>();

            return Result.Success(fieldResponse!);
        }

        

        public async Task<Result<PaginatedList<FieldResponseDto>>> GetAllAsync(RequestFilter filter,  CancellationToken cancellation = default)
        {
            var query = _fieldRepo.GetAll();

            if(!string.IsNullOrEmpty(filter.SearchValue))
            {
                query = query.Where(x => x.Location.Contains(filter.SearchValue));
            }

            var projected = query
                .Select(f => new
                {
                    f.Id,
                    f.Name,
                    f.Location,
                    f.PricePerHour,
                    Rating = f.Reviews
                        .Select(r => (float?)r.Rating)
                        .Average() ?? 0
                });

            var sortColumnKey = string.IsNullOrWhiteSpace(filter.SortColumn)
                ? FieldSortingConfig.DefaultColumn
                : filter.SortColumn.ToUpper();

            if (!FieldSortingConfig.ColumnMap.ContainsKey(sortColumnKey))
            {
                sortColumnKey = FieldSortingConfig.DefaultColumn;
            }

            var columnName = FieldSortingConfig.ColumnMap[sortColumnKey];

            var direction = string.IsNullOrWhiteSpace(filter.SortDirection)
                ? columnName == "Rating" ? "DESC" : "ASC"
                : filter.SortDirection.ToUpper();

            if (direction != "ASC" && direction != "DESC")
            {
                direction = "ASC";

                if (columnName == "Rating") direction = "DESC";
            }

            projected = projected.OrderBy($"{columnName} {direction}");

            var source = projected
                .Select(f => new FieldResponseDto(
                    f.Id,
                    f.Name,
                    f.Location,
                    f.PricePerHour,
                    f.Rating
                ));
            var fields = await PaginatedList<FieldResponseDto>.CreateAsync(source, filter.PageNumber, filter.PageSize);

            return Result.Success(fields);
        }

        public async Task<Result<FieldResponseDto>> GetByIdAsync(int id, CancellationToken cancellation = default)
        {
            var field = await _fieldRepo.GetById(id)
                .Select(f => new FieldResponseDto(
                    f.Id,
                    f.Name,
                    f.Location,
                    f.PricePerHour,
                    f.Reviews.Select(r => (float?)r.Rating).Average() ?? 0
                )).FirstOrDefaultAsync(cancellation);

            if (field is null)
                return Result.Failure<FieldResponseDto>(FieldErrors.NotFound);

            return Result.Success(field);
        }

        public async Task<Result<IEnumerable<FieldResponseDto>>> GetByOwnerIdAsync(string ownerId, CancellationToken cancellation = default)
        {
            var ownerIdResult = await GetOwnerIdByUserIdAsync(ownerId, cancellation);

            if (ownerIdResult.IsFailure)
                return Result.Failure<IEnumerable<FieldResponseDto>>(ownerIdResult.Error);

            var fields = await _fieldRepo.GetByOwnerId(ownerIdResult.Value)
                .Select(f => new FieldResponseDto(
                    f.Id,
                    f.Name,
                    f.Location,
                    f.PricePerHour,
                    f.Reviews.Select(r => (float?)r.Rating).Average() ?? 0
                )).ToListAsync(cancellation);


            return Result.Success<IEnumerable<FieldResponseDto>>(fields);
        }
      

        public async Task<Result> UpdateAsync(int id, UpdateFieldRequestDto request, string userId, CancellationToken cancellation = default)
        {
            var oldField = await _fieldRepo.GetById(id)
                .FirstOrDefaultAsync(cancellation);

            if (oldField is null)
                return Result.Failure(FieldErrors.NotFound);

            var ownerId = await GetOwnerIdByUserIdAsync(userId, cancellation);

            if (oldField.FieldOwnerId != ownerId.Value)
                return Result.Failure(FieldErrors.Unauthorized);

            var field = new Field
            {
                Name = request.Name,
                Location = request.Location,
                PricePerHour = request.PricePerHour
            };
            
            await _fieldRepo.UpdateAsync(id, field, cancellation);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, string userId, CancellationToken cancellation = default)
        {
            var field = await _fieldRepo.GetById(id)
                .FirstOrDefaultAsync(cancellation);

            if (field is null)
                return Result.Failure(FieldErrors.NotFound);

            var ownerId = await GetOwnerIdByUserIdAsync(userId, cancellation);

            if (field.FieldOwnerId != ownerId.Value)
                return Result.Failure(FieldErrors.Unauthorized);

            await _fieldRepo.DeleteAsync(id, cancellation);

            return Result.Success();
        }
        private async Task<Result<int>> GetOwnerIdByUserIdAsync(string userId, CancellationToken cancellation)
        {
            var owner = await _fieldOwnerRepo.GetOwnerByUserId(userId)
                .FirstOrDefaultAsync(cancellation);

            if (owner == null)
                return Result.Failure<int>(FieldOwnerErrors.NotFound);

            return Result.Success(owner.Id);
        }
    }
}