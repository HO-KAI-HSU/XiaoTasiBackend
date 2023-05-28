using System;
using System.Linq;
using System.Linq.Expressions;

namespace XiaoTasiBackend.Repository
{
    /// <summary>
    /// 代表一個Repository的interface。
    /// </summary>
    /// <typeparam name="T">任意model的class</typeparam>
    public interface Repository<T>
    {
        /// <summary>
        /// 新增一筆資料。
        /// </summary>
        /// <param name="entity">要新增到的Entity</param>
        void Create(T entity);

        /// <summary>
        /// 取得第一筆符合條件的內容。如果符合條件有多筆，也只取得第一筆。
        /// </summary>
        /// <param name="predicate">要取得的Where條件。</param>
        /// <returns>取得第一筆符合條件的內容。</returns>
        T Read(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 取得Entity全部筆數的IQueryable。
        /// </summary>
        /// <returns>Entity全部筆數的IQueryable。</returns>
        IQueryable<T> Reads();

        /// <summary>
        /// 更新一筆資料的內容。
        /// </summary>
        /// <param name="entity">要更新的內容</param>
        void Update(T entity);

        /// <summary>
        /// 刪除一筆資料內容。
        /// </summary>
        /// <param name="entity">要被刪除的Entity。</param>
        void Delete(T entity);

        /// <summary>
        /// 儲存異動。
        /// </summary>
        void SaveChanges();
    }

    //public interface IGenericRepository<TEntity> where TEntity : EntityIdentify
    //{
    //    TEntity Get(int id);
    //    TEntity Get(TEntity entity);
    //    IEnumerable<TEntity> GetAll();
    //    void Add(TEntity entity);
    //    void AddRange(IEnumerable<TEntity> entities);
    //    void Update(TEntity entity);
    //    void Remove(int id);
    //    void Remove(TEntity entity);
    //    void RemoveRange(IEnumerable<TEntity> entities);
    //}
}
