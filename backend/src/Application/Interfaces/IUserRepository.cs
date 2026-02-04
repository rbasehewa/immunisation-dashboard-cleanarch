namespace Application.Interfaces;

using Domain.Entities;
using Domain.Enums;
using Application.Models;

public interface IUserRepository
// application layer define the interface for user repository, 
// but infrastructure layer we implement it
// example : if we change database tomorrow MSMSQL to MongoDB this is the only place we need to change
{
    Task<IEnumerable<User>> GetAllUsersAsync(); // async method to get all users
    Task<User?> GetUserByIdAsync(int id); // user can be null
    Task<IEnumerable<User>> GetUsersByStatusAsync(ImmunisationStatus status);
    Task<DashboardStatistics> GetDashboardStatisticsAsync();
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
}