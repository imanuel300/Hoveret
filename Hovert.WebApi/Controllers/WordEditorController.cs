using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using WEBAPIODATAV3.Models;
using log4net;
using System.Numerics;
using WEBAPIODATAV3.Utilities;
using System.Data.SqlClient;
using System.Text;

namespace WEBAPIODATAV3.Controllers
{
    public class WordEditorController : ApiController
    {
        private DBBMEntities db = new DBBMEntities();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        public List<KeyValuePair<int, string>> GetLookup(string table)
        {
            Log.Info("GetLookup:" + table);
            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();
            using (var db = new DBBMEntities())
            {
                try
                {
                    var data = db.Lookup_Proc(table);
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            result.Add(new KeyValuePair<int, string>(item.Key, item.Value));
                        }
                    }
                    return result;
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    return null;
                }
            }
        }

        [HttpGet]
        public IQueryable<Template> GetTemplatesHome()
        {
            var data = db.Templates.OrderByDescending(d => d.UpdatedDate);
            return data;
        }

        [HttpGet]
        public Template GetTemplate(int Id)
        {
            var data = db.Templates.FirstOrDefault(d => d.Id == Id);
            return data;
        }

        [HttpGet]
        public IHttpActionResult NewTemplate(string NameNewTemplate)
        {
            Log.Info("NewTemplate");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var db = new DBBMEntities())
            {
                try
                {
                    Template resultwhile = new Template();
                    int TemplateId = -1;
                    while (resultwhile != null)
                    {
                        TemplateId = Util.MakeRandom();
                        resultwhile = db.Templates.FirstOrDefault(t => t.Id == TemplateId);
                    }

                    var Templates = db.Set<Template>();
                    Template myTemplate = new Template();
                    myTemplate.UpdatedById = 1;// this.getUserId();
                    myTemplate.UpdatedDate = DateTime.Now;
                    myTemplate.Id = TemplateId;
                    myTemplate.Title = NameNewTemplate;



                    Templates.Add(myTemplate);
                    db.SaveChanges();
                    return Ok(myTemplate);
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    throw;
                }
                return null;
            }
        }

        [HttpPost]
        public IHttpActionResult SaveTemplate(Template Template)
        {
            using (var db = new DBBMEntities())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    if (db.Templates.Count(e => e.Id == Template.Id) > 0)
                    {
                        Template result = db.Templates.SingleOrDefault(s3 => s3.Id == Template.Id);
                        db.Entry(result).CurrentValues.SetValues(Template);
                        db.SaveChanges();
                        Util.updateTemplateUpdatedByDate(Template.Id);
                        return Ok(Template);
                    }

                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    throw;
                }
                return null;
            }
        }


        [HttpGet]
        public IQueryable<TenderTemplatesBookletSection> GetTenderTemplateEditor()
        {
            return db.TenderTemplatesBookletSections.OrderBy(d => d.TenderSectionId ?? 0).ThenBy(d => d.Id);
        }



        [HttpGet]
        public IQueryable<TenderTemplatesBookletSection> GetTenderTemplateEditor(int key)
        {
            return db.TenderTemplatesBookletSections
                 .Where(TenderTemplatesBookletSection => TenderTemplatesBookletSection.Id == key);
        }

        [HttpGet]
        public Dictionary<string, string> Getbookmarks(int TenderNumber, int TenderYear)
        {
            var tenderId = (int)GetTenderIdFromTenderYearAndNumber(TenderYear, TenderNumber)[0];
            var Data = Util.CreateBookmarksDictionary(tenderId);
            return Data;
        }
        [HttpGet]
        public Dictionary<string, string> Getbookmarks(int tenderId)
        {
            var Data = Util.CreateBookmarksDictionary(tenderId);
            return Data;
        }

        [HttpPost]
        public IHttpActionResult SaveTenderTemplateEditor(TenderTemplatesBookletSection tenderSection)
        {
            using (var db = new DBBMEntities())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    if (db.TenderTemplatesBookletSections.Count(e => e.Id == tenderSection.Id) > 0)
                    {
                        TenderTemplatesBookletSection result = db.TenderTemplatesBookletSections.SingleOrDefault(s3 => s3.Id == tenderSection.Id);
                        db.Entry(result).CurrentValues.SetValues(tenderSection);//result.SectionBody = tenderSection.SectionBody;
                        db.SaveChanges();
                        return Ok(tenderSection);
                    }
                    else
                    {
                        db.TenderTemplatesBookletSections.Add(tenderSection);
                        return Ok(tenderSection);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    throw;
                }
                return null;
            }
        }

        [HttpPost]
        public int AddNewRow(int TenderSectionId, int id)
        {
            Log.Info("AddNewRow:" + TenderSectionId);
            using (var dbEntitie = new DBBMEntities())
            using (var db = new SqlConnection(ConfigurationManager.ConnectionStrings["DBBMEntitiesADO"].ConnectionString))
            {
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand("UPDATE [dbo].[TenderTemplatesBookletSections] SET TenderSectionId = TenderSectionId + 1 where TenderSectionId > " + TenderSectionId, db);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    SqlCommand cmd1 = new SqlCommand("insert into [dbo].[TenderTemplatesBookletSections](Id, TenderSectionId, TenderId) values((select max(id) + 1 from [dbo].[TenderTemplatesBookletSections])," + (TenderSectionId + 1) + ",0)", db);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                    da1.Fill(ds);

                    db.Close();

                    var db_TenderTemplatesBookletSections = dbEntitie.Set<TenderTemplatesBookletSection>();
                    var ID = db_TenderTemplatesBookletSections.Max(c => c.Id);
                    return ID;
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    return 0;
                }
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteRow([FromODataUri] int id)
        {
            TenderTemplatesBookletSection TenderTemplatesBookletSection = db.TenderTemplatesBookletSections.Find(id);
            if (TenderTemplatesBookletSection == null)
            {
                return NotFound();
            }

            db.TenderTemplatesBookletSections.Remove(TenderTemplatesBookletSection);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public IList<int?> GetTenderIdFromTenderYearAndNumber(int TenderYear, int TenderNumber)//,int year
        {
            var tmp = db.spGetTenderIdFromTenderYearAndNumber(TenderNumber, TenderYear).ToList();
            return tmp;
        }

        [HttpGet]
        public IHttpActionResult BindDataToTemplate(int TemplateId, int TenderYear, int TenderNumber)
        {
            var tenderId = (int)GetTenderIdFromTenderYearAndNumber(TenderYear, TenderNumber)[0];
            Util.oDict = Util.CreateBookmarksDictionary(tenderId);
            string sFilename = Util.GetFilenameBookmarks(tenderId);
            var data = db.Templates.FirstOrDefault(d => d.Id == TemplateId).Value;
            string sOrigHTML = String.Empty;
            foreach (string key in Util.oDict.Keys)
            {
                if (data.Contains(key))
                {
                    sOrigHTML = Util.oDict[key];

                    if (key != "id")
                    {
                        data = data.Replace(key, Util.ModifyHTML(sOrigHTML));
                    }
                }
            }

            return Ok(data);
        }

        [HttpGet]
        public IList<TenderTemplatesBookletSection> TemplateDisplay(int TenderYear, int TenderNumber)//,int year
        {
            var tenderId = (int)GetTenderIdFromTenderYearAndNumber(TenderYear, TenderNumber)[0];
            IQueryable<TenderTemplatesBookletSection> oIq = db.TenderTemplatesBookletSections
                 .Where(TenderTemplatesBookletSection => TenderTemplatesBookletSection.Id == tenderId).OrderBy(d => d.Id);
            TenderTemplatesBookletSection section = null;
            if (oIq.Count() > 0)
            {
                foreach (TenderTemplatesBookletSection ts in oIq)
                {
                    section.SectionBody += ts.SectionBody;// InsertHTMLStyle(ts);
                }
            }

            Util.oDict = Util.CreateBookmarksDictionary(tenderId);
            string sFilename = Util.GetFilenameBookmarks(tenderId);
            string sText = null;
            string sOrigHTML = String.Empty;
            foreach (string key in Util.oDict.Keys)
            {
                if (sText.Contains(key))
                {
                    sOrigHTML = Util.oDict[key];

                    if (key != "id")
                    {
                        sText = sText.Replace(key, Util.ModifyHTML(sOrigHTML));
                    }
                }
            }


            section.SectionBody = section.SectionBody.GetBookmarkValue(tenderId);
            List<TenderTemplatesBookletSection> template = new List<TenderTemplatesBookletSection>();
            template.Add(section);// section);
            return template;
        }


    }

    //--------------------------------------------------//--------------------------------------------------
    //--------------------------------------------------//--------------------------------------------------
    public static class Util
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string _UserName { get { return System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring((System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length) - 3, 3); } }
        public static string _connectionString = ConfigurationManager.ConnectionStrings["DBBMEntitiesADO"].ConnectionString;
        internal static Dictionary<string, string> oDict;






        //internal static Dictionary<string, string> CreateBookmarksDictionary_TenderBooklet(int tenderId) => GetDictionaryByDataSet(
        //    GetBookMarks_TenderBooklet(tenderId, null)
        //    );
        public static Dictionary<string, string> CreateBookmarksDictionary(int tenderId) => GetDictionaryByDataSet(
           GetBookMarks(tenderId, null)
           );

        public static Dictionary<string, string> GetDictionaryByDataSet(DataSet dsBookMark)
        {
            Dictionary<string, string> dicBookMarks = new Dictionary<string, string>();  // declare and init dictionary .
            bool hidden = false;
            for (int i = 0; i < dsBookMark.Tables.Count; i++)
            {
                DataTable dtBm = dsBookMark.Tables[i];

                if (dtBm.Columns[0].Caption.ToLower().IndexOf("DinamicTable".ToLower()) > -1)
                {
                    if (dtBm.Columns[0].Caption.ToLower().IndexOf("_0".ToLower()) > -1)
                        hidden = true;
                    string strKey = dtBm.Columns[0].Caption.ToLower();
                    string strKeyStyle = dtBm.Rows[0][0].ToString().ToLower();
                    i++;
                    dicBookMarks.Add(strKey, ConvertTableToHtmlTable(dsBookMark.Tables[i], strKeyStyle, hidden));
                    continue;
                }
                foreach (DataColumn dc in dtBm.Columns)// loop on all column tables. 
                {
                    if (dicBookMarks.ContainsKey(dc.ColumnName.ToLower()))// if have this key on dictionary.
                    {
                        dicBookMarks.Remove(dc.ColumnName.ToLower());// delete key from dictionary.
                    }
                    if (dtBm.Rows.Count > 0)// condition if have row table.
                    {
                        string strVal = dtBm.Rows[0][dc.ColumnName].ToString();
                        //if(!string.IsNullOrEmpty(strVal.Trim()))
                        //if (dc.DataType == System.Type.GetType("System.DateTime")) strVal = strVal.Substring(0, 10);
                        dicBookMarks.Add(dc.ColumnName.ToLower(), strVal);
                    }// insert bookmark and value to dictionary .
                }
            }

            return dicBookMarks;
        }

        public static DataSet GetDataByCommand(SqlCommand executionCommand, int tenderId, int TenderYear)
        {
            Log.Info("OpenConnection");
            using (var db = new SqlConnection(_connectionString))
            {
                //DataTable dt = null;
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand("EXEC[dbo].[spLett_TenderBookMarks] @TenderID = " + tenderId + ", @TenderYear=" + TenderYear, db);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    //dt = ds.Tables[0];
                    db.Close();
                    return ds;
                    //

                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    return null;
                }

            }
        }
        public static DataSet GetDataByCommand_TenderBooklet(SqlCommand executionCommand, int tenderId)
        {
            Log.Info("OpenConnection");
            using (var db = new SqlConnection(_connectionString))
            {
                //DataTable dt = null;
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand("EXEC[dbo].[spLett_TenderBooklet] @TenderID = " + tenderId, db);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    //dt = ds.Tables[0];
                    db.Close();
                    return ds;
                    //

                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    return null;
                }

            }
        }
        public static DataSet GetBookMarks(int tenderId, string userId)
        {
            Log.Info("spLett_TenderBookMarks");
            DataSet dsBookMark = null;
            Dictionary<string, string> dicBookMarks = null;
            string userName = _UserName;//  System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring((System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length)-3,3);
            userId = userId ?? userName;
            SqlCommand cmd = new SqlCommand("[dbo].[spLett_TenderBooklet]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@TenderId", tenderId));


            dsBookMark = GetDataByCommand_TenderBooklet(cmd, tenderId);

            return dsBookMark;

        }

        //public static DataSet GetBookMarks(int tenderId, int TenderYear, string userId)
        //{
        //    Log.Info("spLett_TenderBookMarks");
        //    DataSet dsBookMark = null;
        //    Dictionary<string, string> dicBookMarks = null;
        //    string userName = _UserName;//  System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring((System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length)-3,3);
        //    userId = userId ?? userName;
        //    SqlCommand cmd = new SqlCommand("[dbo].[spLett_TenderBookMarks]");
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.Add(new SqlParameter("@TenderId", tenderId));
        //    cmd.Parameters.Add(new SqlParameter("@TenderYear", TenderYear));

        //    dsBookMark = GetDataByCommand(cmd, tenderId, TenderYear);

        //    return dsBookMark;

        //}
        public static string ConvertTableToHtmlTable(DataTable dt, string bmStyle, bool hidden)
        {
            if (dt.Rows.Count == 0) return "";
            DataTable dtRight = new DataTable();
            StringBuilder html = new StringBuilder();
            html.Append(bmStyle);
            html.Append("<tr  height='100%' style='font-weight:bold;'>");
            foreach (DataColumn dc in dt.Columns)
            {
                if (dt.Rows[0][dc.Caption].ToString() != "-1234")
                {
                    html.Append("<th>" + dc.Caption + "</th>");
                }
            }
            html.Append("</tr>");
            foreach (DataRow dr in dt.Rows)
            {
                html.Append("<tr>");
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dr[dc].ToString() != "-1234")
                    {
                        html.Append("<td>" + dr[dc].ToString() + "</td>");
                    }
                }
                html.Append("</tr>");
            }
            html.Append("</table>");
            return html.ToString();
        }



        //public TenderTemplatesBookletSection GetConcatenatedSections(IQueryable<TenderTemplatesBookletSection> oIq)
        //{
        //    TenderTemplatesBookletSection oTs = new TenderTemplatesBookletSection();
        //    oIq = oIq.OrderBy<TenderTemplatesBookletSection, int>(d => d.TenderSectionId ?? 0);
        //    foreach (TenderTemplatesBookletSection ts in oIq)
        //    {
        //        oTs.SectionBody += ts.SectionBody;// InsertHTMLStyle(ts);
        //    }

        //    return oTs;
        //}

        public static string GetBookmarkValue(this string text, int tenderId)
        {

            Log.Info(tenderId);
            string sRet = String.Empty;
            text = text == null ? "" : text;
            string sText = text.ToLower();
            string sOrigHTML = String.Empty;
            oDict = CreateBookmarksDictionary(tenderId);
            string sFilename = GetFilenameBookmarks(tenderId);
            foreach (string key in oDict.Keys)
            {
                if (sText.Contains(key))
                {
                    sOrigHTML = oDict[key];

                    if (key != "id")
                    {
                        sText = sText.Replace(key, ModifyHTML(sOrigHTML));
                    }
                }
            }


            //text.Replace("<<marketingMethod>>", "  שיטת שיווק : מחיר למשתכן  ");
            return sText;// CreateBookmarksDictionary( tenderId).First(k=> k.Key == key).;
        }

        public static string GetFilenameBookmarks(int tenderId)
        {
            string sRet = String.Empty, mahozcode = String.Empty, mashbashcode = String.Empty, atarcode = String.Empty;
            oDict = CreateBookmarksDictionary(tenderId);
            //oDict.TryGetValue("MmiMahozShortName".ToLower(), out sShortName);
            //oDict.TryGetValue("TenderNumber".ToLower(), out  sTenderNumber);
            //oDict.TryGetValue("TenderYear".ToLower(), out sTenderYear );
            oDict.TryGetValue("mahozcode".ToLower(), out mahozcode);
            oDict.TryGetValue("mashbashcode".ToLower(), out mashbashcode);
            oDict.TryGetValue("atarcode".ToLower(), out atarcode);

            sRet = String.Format("{0}/{1}/{2}", mahozcode, mashbashcode, atarcode);
            //sRet = String.Format("{0}_{1}_{2}_",sShortName,sTenderNumber,sTenderYear);
            return sRet;

        }

        public static string ModifyHTML(string sOrigHTML)
        {

            string sRet = String.Empty;
            if (sOrigHTML != null && sOrigHTML.Length > 0)
            {
                sRet = sOrigHTML.Replace("font-size:11", "font-size:9px");
            }


            return sRet;
        }

        public static int MakeRandom()
        {
            Guid gguid = Guid.NewGuid();
            byte[] guidAsBytes = gguid.ToByteArray();
            BigInteger guidAsInt = new BigInteger(guidAsBytes);
            string guidAsString = guidAsInt.ToString();
            return Int32.Parse(guidAsString.Substring(guidAsString.Length - 9, 9));
        }

        public static void updateTemplateUpdatedByDate(int TemplateID)
        {
            // update
            // if (getPermissionRole((tenderID)) > 4) return;
            using (var db = new DBBMEntities())
            {
                var result = db.Templates.SingleOrDefault(t => t.Id == TemplateID);
                if (result != null)
                {
                    result.UpdatedById = 1;//this.getUserId();
                    result.UpdatedDate = DateTime.Now;
                    db.SaveChanges();
                }
            }
        }















    }
}

