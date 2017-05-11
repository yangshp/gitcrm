using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Site.Areas.workflow.Controllers
{
    using CRM.WebHelper;
    using Common;
    using IServices;
    using Model.ModelViews;
    using Model;
    using CRM.EntityMapping;
    [SkipCheckPermiss]
    public class wfworkController : BaseController
    {
        public wfworkController(IwfWorkServices wSer, IwfWorkNodesServices nSer, IsysKeyValueServices kSer, IwfWorkBranchServices bSer)
        {
            base.workSer = wSer;
            base.keyvalSer = kSer;
            base.worknodeSer = nSer;
            base.workbramchSer = bSer;
        }
        #region 1.0 工作流列表 
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult getlist()
        {  
            // 1.0 

            var list = workSer.QueryWhere(c => true).Select(c => new
            {
                c.wfID,
                c.wfTitle,
                c.wfRemark,
                c.wfStatus
            }).ToList();
            return Json(new { Rows = list });
        
        }

		 
	#endregion

    }
}
