using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Soccer_IQ.Data;
using Soccer_IQ.Repository.IRepository;

public class BaseRepository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext context;
    private readonly DbSet<T> dbSet;

    public BaseRepository(AppDbContext context)
    {
        this.context = context;
        this.dbSet = context.Set<T>();
    }

    public IEnumerable<T> GetAll(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true)
    {
        IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();

        if (expression != null)
            query = query.Where(expression);

        if (includeProps != null)
            foreach (var includeProp in includeProps)
                query = query.Include(includeProp);

        return query.ToList();
    }

    public T? GetOne(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true)
    {
        IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();

        if (expression != null)
            query = query.Where(expression);

        if (includeProps != null)
            foreach (var includeProp in includeProps)
                query = query.Include(includeProp);

        return query.FirstOrDefault();
    }

    public void Create(T entity)
    {
        dbSet.Add(entity);
    }

    public void Edit(T entity)
    {
        dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        dbSet.Remove(entity);
    }

    public void Commit()
    {
        context.SaveChanges();
    }
}
