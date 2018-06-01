using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Goro.Check.Data
{
    public class Repository : IRepository
    {
        //获取解密后的数据库连接字符串
        private static readonly string connectionString = WebConfig.ConnectionString;

        private static readonly DbContextOptions<CheckDbContext> options = new DbContextOptionsBuilder<CheckDbContext>().UseSqlServer(connectionString).Options;

        public CheckDbContext Db { get; }

        /// <summary>
        /// 每次new会重新创建一个 DBContext 实例
        /// </summary>
        public Repository()
        {
            Db = new CheckDbContext(options);
        }

        public T Find<T>(int id) where T : class, new()
        {
            return Db.Set<T>().Find(id);
        }

        public T FirstOrDefault<T>(Expression<Func<T, bool>> func) where T : class, new()
        {
            return Db.Set<T>().FirstOrDefault(func);
        }

        public T SingleOrDefault<T>(Expression<Func<T, bool>> func) where T : class, new()
        {
            return Db.Set<T>().SingleOrDefault(func);
        }

        public IQueryable<T> Where<T>(Expression<Func<T, bool>> func) where T : class, new()
        {
            return Db.Set<T>().Where(func);
        }

        public void Add<T>(T model) where T : class, new()
        {
            Db.Set<T>().Add(model);
        }

        public void Update<T>(T model) where T : class, new()
        {
            Db.Update<T>(model);
        }

        public void Delete<T>(T model) where T : class, new()
        {
            Db.Set<T>().Remove(model);
        }

        public void Delete<T>(Expression<Func<T, bool>> func) where T : class, new()
        {
            var list = Db.Set<T>().Where(func).ToList();
            foreach (var item in list)
            {
                Db.Set<T>().Remove(item);
            }
        }

        public void Delete<T>(int id) where T : class, new()
        {
            var obj = Find<T>(id);
            if (obj != null)
            {
                Delete(obj);
            }
        }

        public int SaveChanges()
        {
            return Db.SaveChanges();
        }

        public IQueryable<T> Entities<T>() where T : class, new()
        {
            return Db.Set<T>();
        }


        public async Task<int> SaveChangesAsync()
        {
            return await Db.SaveChangesAsync();
        }


        public async Task AddRangeAsync<T>(List<T> model) where T : class, new()
        {
            await Db.Set<T>().AddRangeAsync(model);
        }

        public async Task AddAsync<T>(T model) where T : class, new()
        {
            await Db.Set<T>().AddAsync(model);
        }

        public void Reload<T>(T entity) where T : class, new()
        {
            Db.Entry(entity).Reload();
        }
    }
}
