using Application.Common.Interfaces;
using Application.Common.Results;
using Application.Logs.Dtos;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Logs.Queries
{
    public class GetLogsHandler : IQueryHandler<LogRequest, Result<LogResponse>>
    {
        private readonly ApplicationDbContext _context;
        public GetLogsHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<LogResponse>> Handle(LogRequest request, CancellationToken cancellationToken)
        {
            var logsQuery = _context.Logs
                .Include(l => l.LogType)
                .Include(l => l.User)
                .AsQueryable();

            if (request.Success.HasValue)
                logsQuery = logsQuery.Where(l => l.ActionSuccess == request.Success.Value);

            int page = request.Page.GetValueOrDefault(1);
            if (page < 1) page = 1;

            var totalCount = await logsQuery.CountAsync(cancellationToken);

            var items = await logsQuery
                .OrderByDescending(l => l.EventDate)
                .Skip((page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(l => new LogResponseDto
                {
                    Id = l.Id,
                    LogTypeName = l.LogType.Name,
                    UserFullName = l.User != null ? $"{l.User.FirstName} {l.User.LastName}" : "System",
                    Action = l.Action,
                    Description = l.Description,
                    IpAddress = l.IpAddress,
                    EventDate = l.EventDate,
                    ActionSuccess = l.ActionSuccess
                })
                .ToListAsync(cancellationToken);

            var response = new LogResponse
            {
                Page = page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Items = items
            };

            return Result<LogResponse>.SuccessResult(response);
        }

    }
}
