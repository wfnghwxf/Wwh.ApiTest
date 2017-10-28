using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Wwh.MVC.Web.DAL
{
    public class Repository : IRepository
    {
        private readonly DbContext DB = null;

        public Repository(string constr)
        {
            DB = new DbContext(constr);
        }
        /// <summary>
        /// 添加一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public bool Add<T>(T Entity) where T : class
        {
            DB.Set<T>().Add(Entity);
            int Count = DB.SaveChanges();
            return Count > 0;
        }

        /// <summary>
        /// 批量的插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public bool AddRange<T>(List<T> Entity) where T : class
        {
            bool result = false;
            using (var tran = DB.Database.BeginTransaction())
            {
                try
                {
                    DB.Set<T>().AddRange(Entity);
                    int Count = DB.SaveChanges();
                    tran.Commit();
                    result = Count > 0;
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
            }
            return result;
        }

        /// <summary>
        /// 根据查询条件进行删除对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereLambda">查询条件</param>
        /// <returns></returns>
        public bool Delete<T>(Expression<Func<T, bool>> whereLambda) where T : class
        {
            using (var ts = DB.Database.BeginTransaction())
            {
                try
                {
                    var EntityModel = DB.Set<T>().Where(whereLambda).FirstOrDefault();
                    if (EntityModel != null)
                    {
                        DB.Set<T>().Remove(EntityModel);
                        int Count = DB.SaveChanges();
                        ts.Commit();
                        return Count > 0;
                    }
                }
                catch (Exception)
                {
                    ts.Rollback();
                }
                return false;
            }
        }

        /// <summary>
        /// 删除单个对象的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity">实体对象</param>
        /// <returns></returns>
        public bool Delete<T>(T Entity) where T : class
        {
            using (var ts = DB.Database.BeginTransaction())
            {
                try
                {
                    var temps = DB.Set<T>();
                    temps.Attach(Entity);
                    temps.Remove(Entity);
                    int Count = DB.SaveChanges();
                    ts.Commit();
                    return Count > 0;
                }
                catch (Exception)
                {
                    ts.Rollback();
                }
            }
            return false;
        }

        /// <summary>
        /// 批量的进行更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public bool Update<T>(List<T> Entity) where T : class
        {
            using (var ts = DB.Database.BeginTransaction())
            {
                try
                {
                    if (Entity != null)
                    {
                        foreach (var items in Entity)
                        {
                            var EntityModel = DB.Entry(Entity);
                            DB.Set<T>().Attach(items);
                            EntityModel.State = EntityState.Modified;
                        }
                    }
                    int Count = DB.SaveChanges();
                    ts.Commit();
                    return Count > 0;
                }
                catch (Exception)
                {
                    ts.Rollback();
                }
            }
            return false;
        }


        /// <summary>
        /// 进行修改单个实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity">实体对象</param>
        /// <returns></returns>
        public bool Update<T>(T Entity) where T : class
        {
            using (var ts = DB.Database.BeginTransaction())
            {
                try
                {
                    var EntityModel = DB.Entry<T>(Entity);
                    DB.Set<T>().Attach(Entity);
                    EntityModel.State = EntityState.Modified;
                    int Count = DB.SaveChanges();
                    ts.Commit();
                    return Count > 0;
                }
                catch (Exception)
                {
                    ts.Rollback();
                }
            }
            return false;
        }

        /// <summary>
        /// 批量的修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="WhereLambda"></param>
        /// <param name="UpdateLambda"></param>
        /// <returns></returns>
        public bool Update<T>(Expression<Func<T, bool>> WhereLambda, Expression<Func<T, T>> UpdateLambda) where T : class
        {
            int count = DB.Set<T>().Where(WhereLambda).Update(UpdateLambda);
            return count > 0;
        }

        /// <summary>
        /// 查询条件进行修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="WhereLambda"></param>
        /// <param name="ModifiedProNames"></param>
        /// <returns></returns>
        public bool Update<T>(T model, Expression<Func<T, bool>> WhereLambda, params string[] ModifiedProNames) where T : class
        {
            //查询要修改的数据
            List<T> ListModifing = DB.Set<T>().Where(WhereLambda).ToList();
            Type t = typeof(T);
            List<PropertyInfo> ProInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            Dictionary<string, PropertyInfo> DitProList = new Dictionary<string, PropertyInfo>();
            ProInfos.ForEach(p =>
            {
                if (ModifiedProNames.Contains(p.Name))
                {
                    DitProList.Add(p.Name, p);
                }
            });

            if (DitProList.Count <= 0)
            {
                throw new Exception("指定修改的字段名称有误或为空");
            }
            foreach (var item in DitProList)
            {
                PropertyInfo proInfo = item.Value;
                object newValue = proInfo.GetValue(model, null);
                //批量进行修改相互对应的属性
                foreach (T oModel in ListModifing)
                {
                    proInfo.SetValue(oModel, newValue, null);//设置其中新的值
                }
            }
            return DB.SaveChanges() > 0;
        }
        /// <summary>
        /// 释放缓存
        /// </summary>
        public void Dispose()
        {
            DB.Dispose();
        }

        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        public T FindByID<T>(dynamic ID) where T : class
        {
            return DB.Set<T>().Find(ID) ?? null;
        }


        /// <summary>
        /// 获取全部数据的列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Order">排序</param>
        /// <returns></returns>
        public List<T> GetAll<T>(Expression<Func<T, T>> Order) where T : class
        {
            return Order != null ? DB.Set<T>().OrderBy(Order).ToList() ?? null : DB.Set<T>().ToList() ?? null;
        }

        /// <summary>
        ///根据查询条件进行查询列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="WhereLambda"></param>
        /// <returns></returns>
        public List<T> GetAllQuery<T>(Expression<Func<T, bool>> WhereLambda = null) where T : class
        {
            return WhereLambda != null ? DB.Set<T>().Where(WhereLambda).ToList() ?? null : DB.Set<T>().ToList() ?? null;
        }

        /// <summary>
        ///判断对象是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="WhereLambda"></param>
        /// <returns></returns>
        public bool GetAny<T>(Expression<Func<T, bool>> WhereLambda = null) where T : class
        {
            return WhereLambda != null ? DB.Set<T>().Where(WhereLambda).Any() : DB.Set<T>().Any();
        }

        /// <summary>
        /// 获取查询条件的记录数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="WhereLambda"></param>
        /// <returns></returns>
        public int GetCount<T>(Expression<Func<T, bool>> WhereLambda = null) where T : class
        {
            return WhereLambda != null ? DB.Set<T>().Where(WhereLambda).Count() : DB.Set<T>().Count();
        }

        /// <summary>
        /// 获取单条的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="WhereLambda"></param>
        /// <returns></returns>
        public T GetFristDefault<T>(Expression<Func<T, bool>> WhereLambda = null) where T : class
        {
            return WhereLambda != null ? DB.Set<T>().Where(WhereLambda).FirstOrDefault() ?? null : DB.Set<T>().FirstOrDefault() ?? null;
        }

        /// <summary>
        /// 查询对象的转化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="WhereLambda"></param>
        /// <returns></returns>
        public List<T> GetSelect<T>(Expression<Func<T, bool>> WhereLambda) where T : class
        {
            return DB.Set<T>().Where(WhereLambda).ToList() ?? null;
        }

        /// <summary>
        ///根据查询条件进行分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">每页的大小</param>
        /// <param name="TotalCount">总记录数</param>
        /// <param name="ordering">排序条件</param>
        /// <param name="WhereLambda">查询条件</param>
        /// <returns></returns>
        public List<T> Pagination<T>(int PageIndex, int PageSize, out int TotalCount, Expression<Func<T, T>> Ordering, Expression<Func<T, bool>> WhereLambda = null) where T : class
        {
            //分页的时候一定要注意 Order 一定在Skip 之前
            var QueryList = DB.Set<T>().OrderBy(Ordering).AsQueryable();
            if (WhereLambda != null)
            {
                QueryList = QueryList.Where(WhereLambda);
            }
            TotalCount = QueryList.Count();
            return QueryList.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList() ?? null;
        }

        /// <summary>
        ///根据查询条件进行分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">每页的大小</param>
        /// <param name="TotalCount">总记录数</param>
        /// <param name="OrderBy">排序条件</param>
        /// <param name="WhereLambda">查询的条件</param>
        /// <param name="IsOrder"></param>
        /// <returns></returns>
        public List<T> Pagination<T, TKey>(int PageIndex, int PageSize, out int TotalCount, Expression<Func<T, TKey>> OrderBy, Expression<Func<T, bool>> WhereLambda = null, bool IsOrder = true) where T : class
        {
            //分页的时候一定要注意 Order一定在Skip 之前
            IQueryable<T> QueryList = IsOrder == true ? DB.Set<T>().OrderBy(OrderBy) : DB.Set<T>().OrderByDescending(OrderBy);

            if (WhereLambda != null)
            {
                QueryList = QueryList.Where(WhereLambda);
            }

            TotalCount = QueryList.Count();
            return QueryList.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList() ?? null;
        }

        /// <summary>
        /// 执行存储过程的SQL 语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Sql">执行的SQL语句</param>
        /// <param name="Parms">SQL 语句的参数</param>
        /// <param name="CmdType"></param>
        /// <returns></returns>
        public List<T> QueryPro<T>(string Sql, List<SqlParameter> Parms, CommandType CmdType = CommandType.Text) where T : class
        {
            //进行执行存储过程
            if (CmdType == CommandType.StoredProcedure)
            {
                StringBuilder paraNames = new StringBuilder();
                foreach (var item in Parms)
                {
                    paraNames.Append($" @{item},");
                }
                Sql = paraNames.Length > 0 ? $"exec {Sql} {paraNames.ToString().Trim(',')}" : $"exec {Sql} ";
            }
            return DB.Set<T>().SqlQuery(Sql, Parms.ToArray()).ToList();
        }

        /// <summary>
        /// 进行回滚
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RollBackChanges<T>() where T : class
        {
            var Query = DB.ChangeTracker.Entries().ToList();

            Query.ForEach(p => p.State = EntityState.Unchanged);
        }
        
    }
}