namespace Infrastructure.Repositories;

using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository // handle database queries
{
    private readonly ApplicationDbContext _context;

    // Constructor injection - DbContext is provided by DI
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<User>> GetUsersByStatusAsync(ImmunisationStatus status)
    {
        return await _context.Users
            .Where(u => u.Status == status)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync();
    }

    // ---- using LINQ to use a stored procedure later ----

    // public async Task<DashboardStatistics> GetDashboardStatisticsAsync()
    // {
    //     var stats = new DashboardStatistics
    //     {
    //         TotalUsers = await _context.Users.CountAsync(),
    //         FullyImmunised = await _context.Users
    //             .CountAsync(u => u.Status == ImmunisationStatus.FullyImmunised),
    //         PartiallyImmunised = await _context.Users
    //             .CountAsync(u => u.Status == ImmunisationStatus.PartiallyImmunised),
    //         NonImmunised = await _context.Users
    //             .CountAsync(u => u.Status == ImmunisationStatus.NonImmunised),
    //         Overdue = await _context.Users
    //             .CountAsync(u => u.Status == ImmunisationStatus.Overdue)
    //     };

    //     return stats;
    // }

    // Now uses stored procedure - 1 query instead of 5
    // this point you are talking with the database directly    
    public async Task<DashboardStatistics> GetDashboardStatisticsAsync()
    {
        var stats = await _context.Database
            .SqlQuery<DashboardStatistics>($"EXEC [dbo].[sp_GetDashboardStatistics]")
            .ToListAsync();

        return stats.First();
    }

    public async Task<User> CreateUserAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = null;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        
        return user;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        return true;
    }
}