using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Goro.Check.Data
{
    public interface IRepository 
    {
        /// <summary>
        /// 通过Reload方法刷新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void Reload<T>(T entity) where T : class, new();


        T Find<T>(int id) where T : class, new();

        T FirstOrDefault<T>(Expression<Func<T, bool>> func) where T : class, new();


        T SingleOrDefault<T>(Expression<Func<T, bool>> func) where T : class, new();


        IQueryable<T> Where<T>(Expression<Func<T, bool>> func) where T : class, new();


        void Add<T>(T model) where T : class, new();


        void Update<T>(T model) where T : class, new();


        void Delete<T>(T model) where T : class, new();


        void Delete<T>(Expression<Func<T, bool>> func) where T : class, new();


        void Delete<T>(int id) where T : class, new();


        int SaveChanges();

        IQueryable<T> Entities<T>() where T : class, new();


        Task<int> SaveChangesAsync();

        Task AddAsync<T>(T model) where T : class, new();

        Task AddRangeAsync<T>(List<T> model) where T : class, new();
    }
}
