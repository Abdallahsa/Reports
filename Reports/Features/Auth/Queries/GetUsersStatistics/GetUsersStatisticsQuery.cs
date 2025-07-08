using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Common.Abstractions.Mediator;
using Reports.Features.Auth.Models;

namespace Reports.Features.Auth.Queries.GetUsersStatistics
{
    public class GetUsersStatisticsQuery : ICommand<GetUsersStatisticsModel>
    {
    }

    // Handler for the GetUsersStatisticsQuery

    public class GetUsersStatisticsQueryHandler(AppDbContext _context) : ICommandHandler<GetUsersStatisticsQuery, GetUsersStatisticsModel>
    {

        public async Task<GetUsersStatisticsModel> Handle(GetUsersStatisticsQuery request, CancellationToken cancellationToken)
        {
            var totalUsers = await _context.Users.CountAsync(cancellationToken);
            var activeUsers = await _context.Users.CountAsync(u => u.EmailConfirmed, cancellationToken);
            var inactiveUsers = totalUsers - activeUsers;
            var totalAdmins = await _context.Users.CountAsync(u => u.Level == Level.Admin, cancellationToken);

            var totalLevelZeroUsers = await _context.Users.CountAsync(u => u.Level == Level.LevelZero, cancellationToken);
            var totalLevelOneUsers = await _context.Users.CountAsync(u => u.Level == Level.LevelOne, cancellationToken);
            var totalLevelTwoUsers = await _context.Users.CountAsync(u => u.Level == Level.LevelTwo, cancellationToken);
            var totalLevelThreeUsers = await _context.Users.CountAsync(u => u.Level == Level.LevelThree, cancellationToken);
            var totalLevelFourUsers = await _context.Users.CountAsync(u => u.Level == Level.LevelFour, cancellationToken);

            return new GetUsersStatisticsModel
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                InactiveUsers = inactiveUsers,
                TotalAdmins = totalAdmins,
                TotalLevelZeroUsers = totalLevelZeroUsers,
                TotalLevelOneUsers = totalLevelOneUsers,
                TotalLevelTwoUsers = totalLevelTwoUsers,
                TotalLevelThreeUsers = totalLevelThreeUsers,
                TotalLevelFourUsers = totalLevelFourUsers
            };
        }
    }

}
