using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Site.Areas.admin.Controllers
{
    using System.Web.WebPages;
    using CRM.WebHelper;
    using Common;
    using IServices;
    using Model.ModelViews;
    using Model;
    using CRM.EntityMapping;
  [SkipCheckPermiss]
    public class keyvalueController : BaseController
    {

        public keyvalueController( IsysKeyValueServices kser)
        {
            base.keyvalSer = kser;
        }
        //
        // GET: /admin/keyvalue/
        #region 1.0  列表 
        /// <summary>
        ///  返回视图
        /// </summary>
        /// <returns></returns>
    [HttpGet]
        public ActionResult Index()
        {
            return View();
        } 
    
        [HttpPost]
    public ActionResult getlist()
    { 
            // 1.0  获取异步对象传入的参数 kname 的值  
        string kname = Request.Form["kname"]; 
            // 2.0 判断kname 如果为null 则执行全部查询工作 ，否则根据kname 的条件来查询 
        object list;
        if (kname.IsEmpty())
        {
            list = keyvalSer.QueryOrderByDescending(c => true, c => c.KID).Select(
                c => new {

                    c.KID,
                    c.KName,
                    c.KType,
                    c.Kvalue 
                }
                );
        }
        else
        {
            list = keyvalSer.QueryOrderByDescending(c => c.KName.Contains(kname), c => c.KID).Select(c => new {
            c.KID,c.KName,c.KType,c.Kvalue 
            
            }
                );
        }
        return Json(new { Rows=list,Total=0 });

    }
        #endregion 


        #region 2.0 新增
        #region 2.0 新增

        /// <summary>
        /// 负责返回视图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult add()
        {
            return View();
        }

        /// <summary>
        /// 负责接收参数，存入表syskeyvalue中
        /// 以json格式响应回异步对象
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult add(sysKeyValueView model)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return base.WriteError("实体验证失败");
                }

                //1.0 转换实体
                sysKeyValue entity = model.EntityMap();

                keyvalSer.Add(entity);

                keyvalSer.SaveChanges();

                return base.WriteSucess("新增成功");
            }
            catch (Exception ex)
            {
                return base.WriteError(ex);
            }
        }

        #endregion
        #endregion

        #region 3.0 编辑
        [HttpGet]
        public ActionResult edit(int id)
        {
            // 1.0 根据id 做查询
            var model = keyvalSer.QueryWhere(c => c.KID == id).FirstOrDefault();
            // 2.0 将老数据传入视图 

            return View(model.EntityMap());
      
             
        }
        [HttpPost]
        public  ActionResult edit (int id ,sysKeyValueView  model)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return WriteError("实体验证失败");
                }
                //1.0 将model 的数据补全 
                model.KID = id;
                //2.0 进行数据编辑 
                sysKeyValue entity = model.EntityMap();
                keyvalSer.Edit(entity, new string[] { 
                
                 "KType", "KName", "Kvalue", "KRemark" 
                });
                keyvalSer.SaveChanges();
                return WriteSucess("数据编辑成功");
            }
            catch (Exception ex)
            {
                return  WriteError( ex);
            
            }

        }


        #endregion

        #region 4.0 实现批量删除

        /// <summary>
        /// 实现批量删除
        /// </summary>
        /// <param name="id">格式 KID,KID,</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult del(string id)
        {
            try
            {
                //1.0 将id打断成一个数组
                string[] ids = id.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                //2.0 遍历ids进行数据的物理删除
                sysKeyValue model;
                foreach (var kid in ids)
                {
                    //实例化要删除的数据实体，此时没有追加到EF中
                    model = new sysKeyValue() { KID = int.Parse(kid) };

                    //2.0 追加到EF容器
                    keyvalSer.Delete(model, false);
                }

                //批量删除操作
                keyvalSer.SaveChanges();

                return WriteSucess ("数据删除成功");
            }
            catch (Exception ex)
            {
                return WriteError(ex);
            }
        }

        #endregion
    }
}
