using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.IRepository
{
    using System.Data;
    using System.Data.Entity;
    using System.Linq.Expressions;
   
    public  interface IBaseRepository <TEntity > where  TEntity :class 
    {
        

    
       
        #region 2.0 查询相关方法
          /// <summary>
          ///  根据 labmba 表达式进行查询  
          /// </summary>
          /// <param name="where"></param>
          /// <returns></returns>
        List<TEntity> QueryWhere(Expression<Func<TEntity, bool>> where);
         
          /// <summary>
          /// 连表查询
          /// </summary>
          /// <param name="where"></param>
          /// <param name="tableNames"></param>
          /// <returns></returns>
          List<TEntity> QueryJoin( Expression <Func<TEntity , bool >>  where , string[] tableNames ) ;
      
          /// <summary>
          /// 按照条件查询出数据以后， 根据 外部制定的字段进行升序 排列 
          /// </summary>
          /// <typeparam name="TKey"> 表示从TEntity中获取的属性类型</typeparam>
          /// <param name="where"> 条件</param>
          /// <param name="order">排序lanmba 表达式</param>
          /// <returns></returns>
           List<TEntity> QueryOrderBy<TKey>(Expression<Func<TEntity, bool>> where , Expression<Func<TEntity , TKey >>order) ;
        
          /// <summary>
          /// 按照条件查询出数据以后， 根据 外部制定的字段进行倒序 排列 
          /// </summary>
          /// <typeparam name="TKey"> 表示从TEntity中获取的属性类型</typeparam>
          /// <param name="where"> 条件</param>
          /// <param name="order">排序lanmba 表达式</param>
          /// <returns></returns>
         List<TEntity> QueryOrderByDescending<TKey>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> order) ;
         

          /// <summary>
          /// 分页分法
          /// </summary>
          /// <typeparam name="TKey"> 要 指定排序属性名称 Tentity.property</typeparam>
          /// <param name="pageIndex">分页页码</param>
          /// <param name="pageSize">页容量</param>
          /// <param name="rowCount"> 总行数</param>
          /// <param name="order">排序lambda表达式</param>
          /// <param name="where"> 查询条件</param>
          /// <returns></returns>
         List<TEntity> QueryByPage<TKey>( int pageIndex, int pageSize, out int rowCount ,  Expression<Func<TEntity , TKey >> order  ,Expression<Func<TEntity , bool >> where  ) ;
          



        #endregion
        #region 3.0 编辑相关方法
         void Edit(TEntity model, string[] propertys) ;

          
        #endregion
        #region 4.0 删除相关方法

          void Delete( TEntity model  , bool isadded) ;
         

        #endregion
        #region 5.0 新增相关方法
          void Add(TEntity model) ;
        


        #endregion

          List<TResult> RunProc<TResult>(string sql, params object[] pamrs);
        #region 6.0 统一提交相关方法
           int SaveChanges() ;
         

        #endregion

  

    }
}
