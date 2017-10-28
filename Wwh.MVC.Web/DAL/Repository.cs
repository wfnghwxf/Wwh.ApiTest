using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Wwh.MVC.Web.DAL
{
    public class Repository : IRepository
    {
        private readonly static DbContext _DbContextHandle = new DbContext("");
        public bool Add<T>(T Entity) where T : class
        {
            throw new NotImplementedException();
        }

        public bool AddRange<T>(List<T> Entity) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Delete<T>(T Entity) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Delete<T>(Expression<Func<T, bool>> whereLambda) where T : class
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public T FindByID<T>(dynamic ID) where T : class
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll<T>(string Order = null) where T : class
        {
            throw new NotImplementedException();
        }

        public List<T> GetAllQuery<T>(Expression<Func<T, bool>> WhereLambda = null) where T : class
        {
            throw new NotImplementedException();
        }

        public bool GetAny<T>(Expression<Func<T, bool>> WhereLambda = null) where T : class
        {
            throw new NotImplementedException();
        }

        public int GetCount<T>(Expression<Func<T, bool>> WhereLambda = null) where T : class
        {
            throw new NotImplementedException();
        }

        public T GetFristDefault<T>(Expression<Func<T, bool>> WhereLambda = null) where T : class
        {
            throw new NotImplementedException();
        }

        public List<T> GetSelect<T>(Expression<Func<T, bool>> WhereLambda) where T : class
        {
            throw new NotImplementedException();
        }

        public List<T> Pagination<T, TKey>(int PageIndex, int PageSize, out int TotalCount, Expression<Func<T, TKey>> OrderBy, Expression<Func<T, bool>> WhereLambda = null, bool IsOrder = true) where T : class
        {
            throw new NotImplementedException();
        }

        public List<T> Pagination<T>(int PageIndex, int PageSize, out int TotalCount, string ordering, Expression<Func<T, bool>> WhereLambda = null) where T : class
        {
            throw new NotImplementedException();
        }

        public List<T> QueryPro<T>(string Sql, List<SqlParameter> Parms, CommandType CmdType = CommandType.Text) where T : class
        {
            throw new NotImplementedException();
        }

        public void RollBackChanges<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public bool Update<T>(T Entity) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Update<T>(Expression<Func<T, bool>> WhereLambda, Expression<Func<T, T>> UpdateLambda) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Update<T>(List<T> Entity) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Update<T>(T model, Expression<Func<T, bool>> WhereLambda, params string[] ModifiedProNames) where T : class
        {
            throw new NotImplementedException();
        }
    }
}