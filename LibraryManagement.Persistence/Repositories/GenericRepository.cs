using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Domain.Common;
using LibraryManagement.Persistence.DatabaseContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly LmDatabaseContext _context;
        protected readonly IAppLogger<T> _appLogger;

        public GenericRepository(LmDatabaseContext context, Application.Contracts.Logging.IAppLogger<T> appLogger)
        {
            _context = context;
            _appLogger = appLogger;
        }
        protected async Task HandleDatabaseOperation(Func<Task> dbOperation)
        {
            try
            {
                await dbOperation();
            }
            catch (DbUpdateException ex)
            {
                _appLogger.LogWarning("Database update exception occurred.", ex);
                throw new ApplicationException("Database update failed. Please try again.", ex);
            }
            catch (SqlException ex)
            {
                _appLogger.LogWarning("SQL exception occurred.", ex);
                throw new ApplicationException("A database error occurred.", ex);
            }
            catch (Exception ex)
            {
                _appLogger.LogWarning("An unexpected Db/SQL error occurred.", ex);
                throw new ApplicationException("An unexpected error occurred. Please try again later");
            }
        }

        public async Task<T> CreateAsync(T entity)
        {
            await HandleDatabaseOperation(async () =>
            {
                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();
            });
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            await HandleDatabaseOperation(async () =>
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }); ;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().AsNoTracking()
                  .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            await HandleDatabaseOperation(async () =>
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            });
            return entity;
        }
    }
}
