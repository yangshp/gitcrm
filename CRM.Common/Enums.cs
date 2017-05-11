using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Common
{
  
    /// <summary>
    ///  枚举管理类
    /// </summary>
    public class Enums
    {
        #region 负责标记ajax  请求以后的Json 状态
        /// <summary>
        /// 负责标记ajax  请求以后的Json 状态 
        /// 
        /// </summary>
        public enum EAjaxState
        {
            /// <summary>
            /// 成功
            /// </summary>
            sucess = 0,
            /// <summary>
            /// 错误异常
            /// </summary>
            error = 1,
            /// <summary>
            /// 未登录
            /// </summary>
            nologin = 2,
        }
    #endregion
        #region  负责 标记 菜单栏目 是否停用
        /// <summary>
        /// 负责 标记 菜单栏目 是否停用
        /// </summary>
        public enum Estate
        { 
            /// <summary>
            /// 正常
            /// </summary>
         Nomal  =0 ,
            /// <summary>
            /// 停用 （删除）
            /// </summary>
            Stop =1 
        
        
        
        
        
        }

        #endregion

        #region  节点类型定义
        public enum ENodeType
        {
            /// <summary>
            /// 开始节点
            /// </summary>
             StartNode = 34,
             /// <summary>
             /// 执行节点
             /// </summary>
             ProcessNode  = 35 ,
             /// <summary>
             /// 结束节点
             /// </summary>
             EndNode = 36


        }

        #endregion


        public enum EKeyvalueType
        {
            /// <summary>
            /// 组织结构类型
            /// </summary>
             OrganType=1,
             /// <summary>
             /// 角色类型
             /// </summary>
             RoleType=2 , 
             /// <summary>
             /// 节点类型
             /// </summary>
             NodeType=3,

        }

        /// <summary>
        /// 处理状态
        /// </summary>
        public enum ERequestFormStatus
        {

            /// <summary>
            /// 处理中
            /// </summary>
            Processing = 40 , 
            /// <summary>
            /// 驳回上级
            /// </summary>
            Back = 41 ,
            /// <summary>
            /// 拒绝
            /// </summary>
            Reject = 42 , 
            /// <summary>
            /// 通过
            /// </summary>
            Pass = 43 
        }








    } 
  
}
