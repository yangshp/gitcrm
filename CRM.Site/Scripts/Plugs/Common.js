var helper = {
    //1.0 给ligerGrid获取完服务器响应回来的数据后回调时使用,data:服务器响应回来的json数据被转换成了js的对象
    gridsu: function (data) {
        //data:格式：1、正常格式 {"Rows":[{KID:1,KType:1,KName:"",Kvalue:""}],"Total":10}
        //2.异常或者未登录格式:{status:1/2,msg:""}
        if (data.status == "1") {
            helper.errorTip(data.msg);
        } else if (data.status == "2") {
            //提醒用户未登录,用户点击确定按钮跳转到登录页
            helper.warnTip(data.msg, "tip", function () { window.top.location = "/admin/login/login"; });
            //2秒钟以后自动跳转到登录页
            setTimeout(function () { window.top.location = "/admin/login/login"; }, 2000);
        }
    }
    ,//2.0 错误提醒
    errorTip: function (msg) {
        $.ligerDialog.error(msg);
    }
      ,//3.0 警告提醒
    warnTip: function (msg) {
        $.ligerDialog.warn(msg);
    }
      ,//4.0 成功提醒
    successTip: function (msg) {
        $.ligerDialog.success(msg);
    }
    ,//5.0
    win: null
    //6.0 封装打开编辑和新增的面板方法
    , openPanel: function (title, url, height, width) {
        var h = 450;
        var w = 450;
        if (height) {
            h = height;
        }
        if (width) {
            w = width;
        }
        this.win = $.ligerDialog.open({ title: title, height: h, width: w, url: url });
    }
    ,//7.0 封装统一判断服务器响应回来的具体状态值，进行不同的操作
    checkStatus: function (ajaxobj,callbackFun) {
        //ajaxobj格式:{status=0/1/2,msg=""}
        if (ajaxobj.status == "1")//error
        {
          helper.errorTip(ajaxobj.msg);
        } else if (ajaxobj.status == "2")//未登录
        {
            //提醒用户未登录,用户点击确定按钮跳转到登录页
            helper.warnTip(ajaxobj.msg, "tip", function () { window.top.location = "/admin/login/login"; });
            //2秒钟以后自动跳转到登录页
            setTimeout(function () { window.top.location = "/admin/login/login"; }, 2000);
        } else if (ajaxobj.status == "0") {
            callbackFun();//成功以后，回调特定的逻辑
        } else {
            helper.errorTip("未知错误,请确认js属性名称是否存在");
        }
    }
    ,//8.0新增和编辑的回调函数封装
    ajaxsuccess: function (ajaxobj) {
        //ajaxobj格式:{status=0/1/2,msg=""}
        helper.checkStatus(ajaxobj, function () {
            //1.0 刷新父页中的列表
            window.parent.grid.reload();

            //2.0 关闭当前的新增面板
            window.parent.helper.win.close();
        });
    }
    ,//9.0 封装ajax请求之前的提示
    ajaxbegin: function () {
        //打开正在提交中。。。。的提示
        $("#loading").show();
    } ,
    // 10. 0  封装统一获取菜单权限按钮的方法
    //getfunctions : function (murl,callbackFun){
       
    //    $.post("/admin/PermissOpt/GetFunctions", "murl="+murl, function (toobarItems) {
    //        // toobaritems格式
    //        //items: [
    //        //{ text: '增加', click: add, icon: 'add' },
    //        //{ line: true },
    //        //{ text: '修改', click: edit, icon: 'modify' },
    //        //{ line: true },
    //        //{ text: '删除', click: del, icon: 'delete' },
    //        //{ line: true },
    //        //{ text: '刷新', click: getlist, icon: 'refresh' }
    //        //]
    //        for (var i = 0; i < toobarItems.length; i++) {
    //            if (toobarItems[i].click) {
    //                //动态执行一个字符串，将结果覆盖原来的click的值
    //                toobarItems[i].click = eval(toobarItems[i].click);
    //            }
    //        }
    //        callbackFun(toobarItems);
    //    }, "json");
    //    }    getfunctions: function (murl,callbackFun) {
//    getfunctions: function (murl,callbackFun)
//    {
//    $.post("/admin/PermissOpt/getFunctions", "murl=" + murl, function (toobaritems) {
//        //toobaritems格式： [
//        //{ text: '增加', click: add, icon: 'add' },
//        //{ line: true },
//        //{ text: '修改', click: edit, icon: 'modify' },
//        //{ line: true },
//        //{ text: '删除', click: del, icon: 'delete' },
//        //{ line: true },
//        //{ text: '刷新', click: getlist, icon: 'refresh' }
//        //]
//        //由于toobaritems中的click存放的是一个字符串，此时不会当做函数调用，应该利用eval()
//        for(var i = 0; i < toobaritems.length; i++){
//           if (toobaritems[i].click) {
//               //动态执行一个字符串，将结果覆盖原来的click的值
//               toobaritems[i].click = eval(toobaritems[i].click);
//           }
//        }
//        callbackFun(toobaritems);
//    }, "json");
//}
    //10.0封装统一获取菜单权限按钮的方法
        getfunctions: function (murl,callbackFun) {
            $.post("/admin/PermissOpt/getFunctions", "murl=" + murl, function (toobaritems) {
                //toobaritems格式： [
                //{ text: '增加', click: add, icon: 'add' },
                //{ line: true },
                //{ text: '修改', click: edit, icon: 'modify' },
                //{ line: true },
                //{ text: '删除', click: del, icon: 'delete' },
                //{ line: true },
                //{ text: '刷新', click: getlist, icon: 'refresh' }
                //]
                //由于toobaritems中的click存放的是一个字符串，此时不会当做函数调用，应该利用window
                for (var i = 0; i < toobaritems.length; i++) {
                    if (toobaritems[i].click) {
                        //动态执行一个字符串，将结果覆盖原来的click的值
                        toobaritems[i].click = window[toobaritems[i].click];
                    }
                }
                callbackFun(toobaritems);
            }, "json");
        }
 }
