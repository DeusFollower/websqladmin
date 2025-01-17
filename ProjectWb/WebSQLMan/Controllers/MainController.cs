﻿using Ext.Net;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;
using WebSQLMan.Models;
using WebSQLMan.SQL;
using Ext.Net.MVC;
using System;
using System.Web.UI;

namespace WebSQLMan.Controllers
{

    public class MainController : Controller
    {
        // GET: Home

        

        public ActionResult Index(ConnectionParams cnParams)
        {


            HttpContext.Cache["CnInfo"] = cnParams;

            return View();

        }
        public RedirectToRouteResult Dissconnect()
        {
            return RedirectToAction("Index", "Login");
        }


        public ActionResult Run(string query, string containerId)
        {
            //DataTable dt = new DataTable();
            ConnectionParams cnP = (ConnectionParams)HttpContext.Cache["CnInfo"];
            string server = cnP.ServerName;
            string db = (string)HttpContext.Cache["CurDB"];
            try
            {
                DataSet ds = SQL.Func.Input(query, server, db, cnP.Login, cnP.Password);
                MessageBus.Default.Publish("ResponseServer", "Запрос выполнен успешно");
                var result = new Ext.Net.MVC.PartialViewResult
                {

                    ViewName = "Run",

                    ContainerId = containerId,
                    Model = ds, //passing the DataTable as my Model
                    RenderMode = RenderMode.AddTo
                    

                };
            
            return result;
            }
            catch (SqlException ex)
            {
                string Errors = "";
                foreach (SqlError sqlError in ex.Errors)
                    Errors += sqlError.Message + "\n";
                
                MessageBus.Default.Publish("ResponseServer", Errors);

                return this.Direct();
                
            }
            catch (Exception ex)
            {
                string Error;
                Error = ex.Message;

                MessageBus.Default.Publish("ResponseServer", Error);

                return this.Direct();

            }
        }

        public ActionResult ResponseEvent (string message)
		{
            string succes="Запрос выполнен успешно";
            if (succes != message)
            {
                this.GetCmp<Panel>("MessagePan").Body.CreateChild(new DomObject
                {
                    Html =String.Format("{0} : ({1})", message, DateTime.Now.ToLongTimeString()),
                    Tag = HtmlTextWriterTag.P,
                    CustomConfig =
				{
					new ConfigItem("style", "color:red;", ParameterMode.Value)
				}
                });
            }
            else
            {
                this.GetCmp<Panel>("MessagePan").Body.CreateChild(new DomObject
                {
                    Html = String.Format("{0} : ({1})", message, DateTime.Now.ToLongTimeString()),
                    Tag = HtmlTextWriterTag.P,
                    CustomConfig =
				{
					new ConfigItem("style", "color:green;", ParameterMode.Value)
				}
                });

            }

			return this.Direct();
		}

        

        public ActionResult AddTab(int index)
        {

            index++;
            this.GetCmp<Hidden>("HiddenNumber").Text = index.ToString();
            HttpContext.Cache["CurTab"] = index.ToString();

            Panel pan = new Panel
            {
                ID = "pan" + index,
                Title = "NewQuery" + index,
                Closable = true,
                Height = 250,
                MaxHeight = 400,
                Border = false,
                Items =
                {
                    new Container
                    {
                        ID="query"+ index,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("Query", "Main"),
                        Mode = LoadMode.Script

                    }
                    }
                }

            };

            pan.AddTo(this.GetCmp<TabPanel>("SQLcommandTab"));


            this.GetCmp<TabPanel>("SQLcommandTab").SetActiveTab("pan" + index);


            return this.Direct();
        }

        public Ext.Net.MVC.PartialViewResult Query()
        {

            var ind = HttpContext.Cache["CurTab"].ToString();

            return new Ext.Net.MVC.PartialViewResult
             {
                 ViewName = "Query",
                 Model = "textAreaId" + ind,
                 ContainerId = "query" + ind,
                 WrapByScriptTag = false,
                 RenderMode = RenderMode.Replace
             };
        }

        public Ext.Net.MVC.PartialViewResult Refresh()
        {

            return new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "_BasesTree",
                ContainerId = "TreePanel",
                WrapByScriptTag = false,
                ClearContainer = true,
                RenderMode = RenderMode.RenderTo
            };
        }

        [HttpPost]
        public ActionResult ContextMenuTop100(string NodeText, string NodeData, string containerId)
        {
            containerId = "ResultTabPanel";


            DataTable dt = new DataTable();
            
            string querystring = string.Format("Select TOP 100 * From {0}", NodeText);
            ConnectionParams cnP = (ConnectionParams)HttpContext.Cache["CnInfo"];
            string server = cnP.ServerName;
            string db = ParseDB(NodeData);

                       try
            {
                DataSet ds = SQL.Func.Input(querystring, server, db);
                MessageBus.Default.Publish("ResponseServer", "Запрос выполнен успешно");
                var result = new Ext.Net.MVC.PartialViewResult
                {

                    ViewName = "Run",

                    ContainerId = containerId,
                    Model = ds, //passing the DataTable as my Model
                    RenderMode = RenderMode.AddTo


                };

                return result;
            }
            catch (SqlException ex)
            {
                string Errors = "";
                foreach (SqlError sqlError in ex.Errors)
                    Errors += sqlError.Message + "\n";

                MessageBus.Default.Publish("ResponseServer", Errors);

                return this.Direct();

            }

           
        }


        [HttpPost]
        public ActionResult ContextMenuTop1000(string NodeText, string NodeData, string containerId)
        {
            containerId = "ResultTabPanel";


            DataTable dt = new DataTable();

            string querystring = string.Format("Select TOP 1000 * From {0}", NodeText);
            ConnectionParams cnP = (ConnectionParams)HttpContext.Cache["CnInfo"];
            string server = cnP.ServerName;
            string db = ParseDB(NodeData);

            try
            {
                DataSet ds = SQL.Func.Input(querystring, server, db);
                MessageBus.Default.Publish("ResponseServer", "Запрос выполнен успешно");
                var result = new Ext.Net.MVC.PartialViewResult
                {

                    ViewName = "Run",

                    ContainerId = containerId,
                    Model = ds, //passing the DataTable as my Model
                    RenderMode = RenderMode.AddTo


                };

                return result;
            }
            catch (SqlException ex)
            {
                string Errors = "";
                foreach (SqlError sqlError in ex.Errors)
                    Errors += sqlError.Message + "\n";

                MessageBus.Default.Publish("ResponseServer", Errors);

                return this.Direct();

            }


        }

        [HttpPost]
        public ActionResult DropTable(string NodeText, string NodeData, string containerId)
        {
            containerId = "ResultTabPanel";


            DataTable dt = new DataTable();

            string querystring = string.Format("DROP TABLE {0}", NodeText);
            ConnectionParams cnP = (ConnectionParams)HttpContext.Cache["CnInfo"];
            string server = cnP.ServerName;
            string db = ParseDB(NodeData);

            try
            {
                DataSet ds = SQL.Func.Input(querystring, server, db);
                MessageBus.Default.Publish("ResponseServer", "Запрос выполнен успешно");
                var result = new Ext.Net.MVC.PartialViewResult
                {

                    ViewName = "Run",

                    ContainerId = containerId,
                    Model = ds, //passing the DataTable as my Model
                    RenderMode = RenderMode.AddTo


                };

                return RedirectToAction("Index", "Main", HttpContext.Cache["CnInfo"]);
            }
            catch (SqlException ex)
            {
                string Errors = "";
                foreach (SqlError sqlError in ex.Errors)
                    Errors += sqlError.Message + "\n";

                MessageBus.Default.Publish("ResponseServer", Errors);

                return this.Direct();

            }


        }

        public Ext.Net.MVC.PartialViewResult BasesTree(string containerId)
        {

            return new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "_BasesTree",
                ContainerId = containerId,
                WrapByScriptTag = false,
                ClearContainer = true,
                RenderMode = RenderMode.RenderTo
            };
        }

        [HttpPost]
        public JsonResult CashDB(string NodeData)
        {
            HttpContext.Cache["CurDB"] = NodeData;
            return new JsonResult();
        }

        public string GetRootNodes()
        {

            return "<ul> <li class=\"jstree-closed\">System Databases</li>" +
                        "<li class=\"jstree-closed\">User Databases</li>" +
                        "<li class=\"jstree-closed\">Database Snapshots</li> " +
                   "</ul>";
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult GetChildren(string id, string NodeData, string NodeText)
        {
            ConnectionParams CnP = (ConnectionParams)HttpContext.Cache["CnInfo"];

            string Server = CnP.ServerName;

            {
                //CurrConn.Open();
                List<G_JSTree> Nodes = new List<G_JSTree>();

                G_JSTree Node = new G_JSTree();

                switch (NodeText)
                {
                    case "System Databases":
                        List<string> SystemDBs = Func.GetSystemDatabases(Server);
                        foreach (string SysDB in SystemDBs)
                        {
                            Node = new G_JSTree();
                            Node.children = true;
                            Node.data = "DB";
                            Node.text = SysDB;
                            Node.state = "closed";
                            Nodes.Add(Node);
                        }
                        return Json(Nodes, JsonRequestBehavior.AllowGet);
                    case "User Databases":
                        List<string> UserDBs = Func.GetUserDatabases(Server);
                        foreach (string UserDB in UserDBs)
                        {
                            Node = new G_JSTree();
                            Node.children = true;
                            Node.data = "DB";  //  Все узлы баз получают аттрибут DB для дальнейшей идентификации
                            Node.text = UserDB;
                            Node.state = "closed";
                            Nodes.Add(Node);
                        }
                        return Json(Nodes, JsonRequestBehavior.AllowGet);
                    case "Database Snapshots":
                        List<string> DBsnapshots = Func.GetDBsnapshots(Server);
                        foreach (string DBsnapshot in DBsnapshots)
                        {
                            Node = new G_JSTree();
                            Node.children = true;
                            Node.data = "DB";
                            Node.text = DBsnapshot;
                            Node.state = "closed";
                            Nodes.Add(Node);
                        }
                        return Json(Nodes, JsonRequestBehavior.AllowGet);
                };

                switch (GetFirstWord(NodeData))
                {
                    case "DB":
                        
                        #region DBnodeCONTENT
                        Node = new G_JSTree();
                        Node.children = true;
                        Node.data = "Tables " + "(DB)" + NodeText + "(-DB)";
                        Node.text = "Tables";
                        Node.state = "closed";
                        Nodes.Add(Node);
                        Node = new G_JSTree();
                        Node.children = true;
                        Node.data = "Views " + "(DB)" + NodeText + "(-DB)";
                        Node.text = "Views";
                        Node.state = "closed";

                        Nodes.Add(Node);
                        Node = new G_JSTree();
                        Node.children = true;
                        Node.data = "Programmability " + "(DB)" + NodeText + "(-DB)";
                        Node.text = "Programmability";
                        Node.state = "closed";
                        //Node.attr = new G_JsTreeAttribute(){ id = NodeText, selected=false };
                        Nodes.Add(Node);
                        
                        #endregion
                        return Json(Nodes, JsonRequestBehavior.AllowGet);
                    case "Tables":
                        string DB = ParseDB(NodeData);
                        List<string> tables = Func.GetDBtables(Server, DB);
                        foreach (string table in tables)
                        {
                            Node = new G_JSTree();
                            Node.children = true;
                            Node.data = "Table " + NodeData;

                            Node.text = table;
                            Node.state = "closed";
                            Nodes.Add(Node);
                        }
                        return Json(Nodes, JsonRequestBehavior.AllowGet);
                    case "Views":
                        DB = ParseDB(NodeData);
                        List<string> views = Func.GetViews(Server, DB);
                        foreach (string view in views)
                        {
                            Node = new G_JSTree();
                            Node.children = false;
                            Node.data = "View " + NodeData;

                            Node.text = view;
                            Node.state = "closed";
                            Nodes.Add(Node);
                        }
                        return Json(Nodes, JsonRequestBehavior.AllowGet);
                    case "Programmability":
                        string db = ParseDB(NodeData);
                        #region ProgrammabilityNodeCONTENT
                        Node = new G_JSTree();
                        Node.children = true;
                        Node.data = "StoredProcs " + "(DB)" + db + "(-DB)";
                        Node.text = "Stored Procedures";
                        Node.state = "closed";
                        Nodes.Add(Node);
                        Node = new G_JSTree();
                        Node.children = true;
                        Node.data = "Functions " + "(DB)" + db + "(-DB)";
                        Node.text = "Functions";
                        Node.state = "closed";

                        Nodes.Add(Node);
                        Node = new G_JSTree();
                        Node.children = true;
                        Node.data = "Triggers " + "(DB)" + db + "(-DB)";
                        Node.text = "Database Triggers";
                        Node.state = "closed";
                        //Node.attr = new G_JsTreeAttribute(){ id = NodeText, selected=false };
                        Nodes.Add(Node);
                        Node = new G_JSTree();
                        Node.children = true;
                        Node.data = "Rules " + "(DB)" + db + "(-DB)";
                        Node.text = "Rules";
                        Node.state = "closed";
                        //Node.attr = new G_JsTreeAttribute(){ id = NodeText, selected=false };
                        Nodes.Add(Node);

                        #endregion
                        return Json(Nodes, JsonRequestBehavior.AllowGet);
                    case "StoredProcs":
                        db = ParseDB(NodeData);
                        List<string> procs = Func.GetStoredProcs(Server, db);
                        foreach (string proc in procs)
                        {
                            Node = new G_JSTree();
                            Node.children = false;
                            Node.data = "Proc " + NodeData;

                            Node.text = proc;
                            Node.state = "closed";
                            Nodes.Add(Node);
                        }
                        return Json(Nodes, JsonRequestBehavior.AllowGet);
                    case "Functions":
                        db = ParseDB(NodeData);
                        List<string> funcs = Func.GetFuncs(Server, db);
                        foreach (string func in funcs)
                        {
                            Node = new G_JSTree();
                            Node.children = false;
                            Node.data = "Func " + NodeData;

                            Node.text = func;
                            Node.state = "closed";
                            Nodes.Add(Node);
                        }
                        return Json(Nodes, JsonRequestBehavior.AllowGet);
                    case "Triggers":
                        db = ParseDB(NodeData);
                        List<string> trigs = Func.GetTrigs(Server, db);
                        foreach (string trig in trigs)
                        {
                            Node = new G_JSTree();
                            Node.children = false;
                            Node.data = "Func " + NodeData;

                            Node.text = trig;
                            Node.state = "closed";
                            Nodes.Add(Node);
                        }
                        return Json(Nodes, JsonRequestBehavior.AllowGet);
                    /* Раскрытие конкретной табл*/
                    case "Table":
                        Node = new G_JSTree();
                        Node.children = true;
                        Node.data = "Columns " + "(Tbl)" + NodeText + "(-Tbl)" + NodeData;
                        Node.text = "Columns";
                        Node.state = "closed";
                        Nodes.Add(Node);
                        Node = new G_JSTree();
                        Node.children = true;
                        Node.data = "Constraints " + "(Tbl)" + NodeText + "(-Tbl)" + NodeData;
                        Node.text = "Constraints";
                        Node.state = "closed";

                        Nodes.Add(Node);
                        
                        
                        return Json(Nodes, JsonRequestBehavior.AllowGet);
                    case "Constraints":
                        string Tb = ParseTbl(NodeData);
                        string Db = ParseDB(NodeData);
                        List<string> constraints = Func.GetConstraints(Server, Db, Tb);
                        foreach (string constraint in constraints)
                        {
                            Node = new G_JSTree();
                            Node.children = true;
                            Node.data = "Constraints " + NodeData;

                            Node.text = constraint;
                            Node.state = "closed";
                            Nodes.Add(Node);
                        }
                        return Json(Nodes, JsonRequestBehavior.AllowGet);
                    case "Columns":
                        Tb = ParseTbl(NodeData);
                        Db = ParseDB(NodeData);
                        List<string> columns = Func.GetColumns(Server, Db, Tb);
                        foreach (string column in columns)
                        {
                            Node = new G_JSTree();
                            Node.children = false;
                            Node.data = "Column " + NodeData;

                            Node.text = column;
                            Node.state = "closed";
                            Nodes.Add(Node);
                        }
                        return Json(Nodes, JsonRequestBehavior.AllowGet);
                }

                return new JsonResult();
            }

        }

        string GetFirstWord(string str)
        {
            if (str == null)
                return "";
            if (str.IndexOf(" ") < 0)
                return str;
            return str.Substring(0, str.IndexOf(" "));
        }

        string ParseDB(string str)
        {
            if (str == null)
                return "";
            int i = str.IndexOf("(DB)");
            int j = str.IndexOf("(-DB)");

            if (i == -1 || j == -1)
                return "";
            return str.Substring(i + 4, j - i - 4);
        }

        string ParseTbl(string str)
        {
            if (str == null)
                return "";
            int i = str.IndexOf("(Tbl)");
            int j = str.IndexOf("(-Tbl)");

            if (i == -1 || j == -1)
                return "";

            return str.Substring(i + 5, j - i - 5);
        }

    }
}
