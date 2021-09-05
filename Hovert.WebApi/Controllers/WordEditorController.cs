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
using Aspose.Words;
using System.IO;
using System.Web;
using Aspose.Words.Lists;
using System.Text.RegularExpressions;

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
        public IQueryable GetTemplatesHome()
        {
            var data = db.Templates.Where(d => d.Publish == true).OrderByDescending(d => d.UpdatedDate).Select(x => new
            {
                Id = x.Id,
                Title = x.Title,
                UpdatedById = x.UpdatedById,
                UpdatedDate = x.UpdatedDate
            });
            return data;
        }

        [HttpGet]
        public Template GetTemplate(int Id)
        {
            var data = db.Templates.FirstOrDefault(d => d.Id == Id && d.Publish == true);
            return data;
        }

        [HttpGet]
        public IHttpActionResult NewTemplate(string NameNewTemplate, int MarketingMethod)
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
                    myTemplate.MarketingMethod = MarketingMethod;
                    myTemplate.Publish = true;
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
                    if (db.Templates.Count(e => e.Id == Template.Id && e.Publish == true) > 0)
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
            var tenderId = (int)Util.GetTenderIdFromTenderYearAndNumber(TenderYear, TenderNumber)[0];
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
        public IHttpActionResult DeleteRow([FromUri] int Id)
        {
            try
            {
                using (var db = new DBBMEntities())
                {

                    Template result = db.Templates.SingleOrDefault(s3 => s3.Id == Id);
                    if (result != null)
                    {
                        result.Publish = false;
                        db.Entry(result).CurrentValues.SetValues(Id);
                        db.SaveChanges();
                        Util.updateTemplateUpdatedByDate(Id);
                        return Ok(true);
                    }
                }

            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return Ok(false);
            }
            return null;
        }

        [HttpGet]
        public IList<int?> GetTenderIdFromTenderYearAndNumber(int TenderYear, int TenderNumber)//,int year
        {
            var tmp = Util.GetTenderIdFromTenderYearAndNumber(TenderYear, TenderNumber);
            return tmp;
        }

        [HttpGet]
        public IHttpActionResult BindDataToTemplate(int TemplateId, int TenderYear, int TenderNumber)
        {
            Log.Info("BindDataToTemplate: " + TemplateId);
            var tenderId = (int)Util.GetTenderIdFromTenderYearAndNumber(TenderYear, TenderNumber)[0];
            Util.oDict = Util.CreateBookmarksDictionary(tenderId);
            var data = db.Templates.FirstOrDefault(d => d.Id == TemplateId).Value;
            Log.Info("GetData: " + tenderId);
            var final = Util.ReplaceBookmarks(data);
            Log.Info("ReplaceBookmarks: ");
            return Ok(final);
        }

        [HttpGet]
        public bool SaveInSPWordFile(int tenderId)
        {
            Document doc = Util.TemplateToWord(tenderId);
            string sTargetFolder = "", fileName = "";
            var ts = "4/50/2";// http:mochsps/sites/externalsys/4/50/2/%D7%A9%D7%99%D7%95%D7%95%D7%A7/%D7%9E%D7%9B%D7%A8%D7%96%D7%99%D7%9D

            string[] sParams = ts.Split(new char[1] { '/' });
            sTargetFolder = @"/sites/externalsys/" + sParams[0] + "/" + sParams[1] + "/" + sParams[2] + "/שיווק/מכרזים/" + tenderId.ToString() + "";// http:/mochsps/sites/externalsys/
            //sTargetFolder = @"/sites/externalsys/" + sParams[0] + "/" + sParams[1] + "/" + sParams[2] + "/XXX/YYY/" + ts.Id.ToString() + "";
            string sTargetFolderASCII = sTargetFolder;//@"/sites/externalsys/2/14/7/9999" ;   
                                                      //  string documentTitle = "FILE"; 
            string documentTitle = @"חוברת מכרז";
            Dictionary<string, object> spProperties = null;
            try
            {
                //spProperties = SetDocumentProperties(doc, null, sTargetFolder, documentTitle);
                //spProperties = SetDocumentProperties(srcDoc, null, sTargetFolder, documentTitle);

                spProperties = new Dictionary<string, object>();
                spProperties.Add(sTargetFolder, "BamaProject");
                spProperties.Add("ContentType", "BamaDoc");
                doc.BuiltInDocumentProperties["Title"].Value = documentTitle;
            }
            catch (Exception ex)
            {
                Log.Error("Error: " + ex.Message);
                return false;
                throw ex;
            }
            try
            {
                var uncPath = sTargetFolder.Replace(@"/", @"\");
                var host = new Uri(@"http://svtmos10").Host;// http:/svtmos10/sites/externalsys/4/50/2/%D7%A9%D7%99%D7%95%D7%95%D7%A7/%D7%9E%D7%9B%D7%A8%D7%96%D7%99%D7%9D
                uncPath = @"\\" + host + uncPath;
                fileName = Path.Combine(uncPath, documentTitle + ".docx").ToString();

                // Directory.CreateDirectory(sPathFileSystem + "\\" + sSectionNumber);
                string sPathFileSystem = ConfigurationManager.AppSettings["PathFileSystem"];
                string d = string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
                string sSectionNumber = ts.Replace(@"/", @"\");
                string fileNameLocal = sPathFileSystem + "\\" + sSectionNumber + @"\" + d + ".docx";

                string sLocalFile = Util.SaveFileToMOSS(fileNameLocal, sTargetFolder, d, TemplateId);
                System.Diagnostics.Process.Start(sLocalFile);

                UtilityMethods.SaveFileURLToDB(TemplateId, sLocalFile);


            }
            catch (Exception ex)
            {
                Log.Error("Error: " + ex.Message);
                return false;
                throw ex;
            }
            //throw new Exception("Test : ");


            return true;
        }

            [HttpGet]
        public IHttpActionResult DownloadWordFile(int TemplateId, int TenderYear, int TenderNumber)
        {
            Document doc = Util.TemplateToWord(TemplateId, TenderYear, TenderNumber);



            ///////////////////////////////////////////////////* CODE DES EXPORTTOWORD *////////////////////////////////////////////////////

            


            #region SAVE TO LOCAL
            // string sPathLocal = System.Web.HttpContext.Current.Server.MapPath(".");
            string sPathFileSystem = ConfigurationManager.AppSettings["PathFileSystem"];
            string d = string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
            //string sFolder = @"C:\Users\CarmelS\Downloads\_TEST\OUTPUT\";
            var ts = "4/50/2";
            string sSectionNumber = ts.Replace(@"/", @"\");
            string httpfileName = HttpContext.Current.Request.Url.Authority + @"/Doc/" + sSectionNumber.Replace(@"\", @"/") + @"/" + d + ".docx";
            string fileNameLocal = sPathFileSystem + "\\" + sSectionNumber + @"\" + d + ".docx";
            //string folderNameLocal = sPathFileSystem + "\\" + sSectionNumber + @"\";
            #endregion


            //Log.Info(HttpContext.Current.Request.Url.Host);
            //Log.Info(HttpContext.Current.Request.Url.Authority);
            //Log.Info(HttpContext.Current.Request.Url.Port);
            //Log.Info(HttpContext.Current.Request.Url.AbsolutePath);
            //Log.Info(HttpContext.Current.Request.ApplicationPath);
            //Log.Info(HttpContext.Current.Request.Url.AbsoluteUri);
            //Log.Info(HttpContext.Current.Request.Url.PathAndQuery);
            // localhost
            Log.Info("new file fileNameLocal:" + fileNameLocal);
            Log.Info("new file httpfileName:" + httpfileName);
            //Directory.CreateDirectory(sPathFileSystem + "\\" + sSectionNumber);
            doc.Save(fileNameLocal);
            //string sLocalFile = SaveFileToMOSS(fileNameLocal, sTargetFolder, d, ts.Id);
            // System.Diagnostics.Process.Start(sLocalFile);

            //UtilityMethods.SaveFileURLToDB(ts.Id, sLocalFile);


            ////////////////////////////////////////////////////////////////////////////////////////*/



            //builder.Document.Save(dataDir);

            //System.Diagnostics.Process.Start(sLocalFile);

            //return sLocalFile;
            return Ok(httpfileName);
        }

        [HttpGet]
        public IList<TenderTemplatesBookletSection> TemplateDisplay(int TenderYear, int TenderNumber)//,int year
        {
            var tenderId = (int)Util.GetTenderIdFromTenderYearAndNumber(TenderYear, TenderNumber)[0];
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
            //string sFilename = Util.GetFilenameBookmarks(tenderId);
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

        public static IList<int?> GetTenderIdFromTenderYearAndNumber(int TenderYear, int TenderNumber)//,int year
        {
            using (var db = new DBBMEntities())
            {
                try
                {
                    var tmp = db.spGetTenderIdFromTenderYearAndNumber(TenderNumber, TenderYear).ToList();
                    return tmp;
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    return null;
                }
            }
        }
        public static IList<int?> GetTenderYearAndNumberFromTenderId(int TenderId)
        {
            using (var db = new DBBMEntities())
            {
                try
                {
                    var tmp = db.GetTenderYearAndNumberFromTenderId(TenderId).ToList();
                    return tmp;
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    return null;
                }
            }
        }

        public static Document TemplateToWord(int TemplateId, int TenderYear, int TenderNumber)
        {
            using (var db = new DBBMEntities())
            {
                try
                {
                    Aspose.Words.License _license;
                    _license = new Aspose.Words.License();
                    _license.SetLicense(ConfigurationManager.AppSettings["Asposelic"]);

                    Document doc = new Document(ConfigurationManager.AppSettings["TemplatesFolder"] + @"\tpl.docx");
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    //builder.Font.Bidi = true;
                    //builder.ParagraphFormat.Bidi = true;
                    //Aspose.Words.Font font = builder.Font;

                    string sRightMargin = ConfigurationManager.AppSettings["RightMargin"];
                    string sLeftMargin = ConfigurationManager.AppSettings["LeftMargin"];
                    double dRightMargin = 1;
                    double dLeftMargin = 1;
                    Double.TryParse(sRightMargin, out dRightMargin);
                    Double.TryParse(sLeftMargin, out dLeftMargin);
                    builder.PageSetup.RightMargin = ConvertUtil.InchToPoint(dRightMargin);
                    builder.PageSetup.LeftMargin = ConvertUtil.InchToPoint(dLeftMargin);
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Justify;

                    var tenderId = (int)GetTenderIdFromTenderYearAndNumber(TenderYear, TenderNumber)[0];
                    Util.oDict = Util.CreateBookmarksDictionary(tenderId);
                    var data = db.Templates.FirstOrDefault(da => da.Id == TemplateId).Value;
                    Log.Info("GetData: " + tenderId);
                    var final = Util.ReplaceBookmarks(data);
                    Log.Info("ReplaceBookmarks: ");

                    ///////////// List Number start="x"  ///////////////////////
                    int index = 0;
                    int finishtofound = 0;
                    do
                    {
                        if (final.IndexOf("start=", finishtofound) > 0)
                            index = final.IndexOf("start=", index) + 7;
                        else break;
                        var indexline = final.IndexOf("<li>", index) + 4;
                        var Number = Int32.Parse(final.Substring(index, final.Substring(index).IndexOf("\"")));
                        final = final.Substring(0, indexline) + "##StartAt#" + Number + "#" + final.Substring(indexline);
                        finishtofound = finishtofound + index;
                    }
                    while (final.Length >= finishtofound);

                    /// ListFormat ///
                    List list = doc.Lists.Add(ListTemplate.NumberDefault);
                    list.ListLevels[1].NumberFormat = "\x0000.\x0001";
                    list.ListLevels[2].NumberFormat = "\x0000.\x0001.\x0002";
                    list.ListLevels[3].NumberFormat = "\x0000.\x0001.\x0002.\x0003";
                    list.ListLevels[4].NumberFormat = "\x0000.\x0001.\x0002.\x0003.\x0004";
                    list.ListLevels[5].NumberFormat = "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005";
                    foreach (ListLevel l in list.ListLevels)
                    {
                        l.NumberStyle = NumberStyle.Arabic;
                    }
                    /////////////// insert all Html  /////////////////////
                    builder.InsertHtml(String.Format(@"<div  style='direction: rtl;'><p dir='rtl' style = '' >{0}</p></div>", final));//הכנסת תוכן מרכזי
                                                                                                                                      ////////////////////////////////////


                    foreach (Paragraph para in doc.GetChildNodes(NodeType.Paragraph, true))
                    {
                        if (para.ListFormat.ListLevel != null)
                        {
                            if (para.ListFormat.ListLevel.NumberStyle.ToString() != "Bullet")
                            {
                                if (para.ListFormat.IsListItem)//&& para.ListFormat.ListLevelNumber > 0
                                {
                                    para.ListFormat.List = list;
                                    if (para.GetText().IndexOf("##StartAt#") >= 0)
                                    {
                                        var indexNumberStart = para.GetText().IndexOf("##StartAt#") + 10;
                                        var indexNumberEnd = para.GetText().IndexOf("#", indexNumberStart);
                                        var Number = Int32.Parse(para.GetText().Substring(indexNumberStart, indexNumberEnd - indexNumberStart));
                                        para.Range.Replace("##StartAt#" + Number + "#", "", false, false);
                                        para.ListFormat.ListLevel.StartAt = Number;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (para.ParentNode.NodeType != NodeType.Cell)
                            {
                                list = doc.Lists.Add(ListTemplate.NumberDefault);
                                list.ListLevels[1].NumberFormat = "\x0000.\x0001";
                                list.ListLevels[2].NumberFormat = "\x0000.\x0001.\x0002";
                                list.ListLevels[3].NumberFormat = "\x0000.\x0001.\x0002.\x0003";
                                list.ListLevels[4].NumberFormat = "\x0000.\x0001.\x0002.\x0003.\x0004";
                                list.ListLevels[5].NumberFormat = "\x0000.\x0001.\x0002.\x0003.\x0004.\x0005";
                                foreach (ListLevel l in list.ListLevels)
                                {
                                    l.NumberStyle = NumberStyle.Arabic;
                                }
                            }

                        }

                    }
                    //builder.InsertBreak(BreakType.SectionBreakNewPage);

                    ////////////// הכנסת תוכן כותרת עליונה //////////////////////
                    PageSetup ps = builder.PageSetup;
                    ps.DifferentFirstPageHeaderFooter = true;

                    builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
                    string mmi_mahoz_shortname = String.Empty, tendernumber = String.Empty, tenderyear = String.Empty;
                    Util.oDict.TryGetValue("mmi_mahoz_shortname".ToLower(), out mmi_mahoz_shortname);
                    Util.oDict.TryGetValue("tendernumber".ToLower(), out tendernumber);
                    Util.oDict.TryGetValue("tenderyear".ToLower(), out tenderyear);
                    var HeaderPrimary = String.Format("מכרז מספר " + "{0}/{1}/{2}", mmi_mahoz_shortname, tendernumber, tenderyear);
                    builder.InsertHtml(String.Format(@"<div  style=''><p style = '' >{0}</p></div>", HeaderPrimary));//הכנסת תוכן כותרת עליונה
                                                                                                                     ////////////////////////////////////
                    string dataDir = ConfigurationManager.AppSettings["Asposelic"];
                    dataDir = dataDir + string.Format("Lists_{0}.doc", DateTime.Now.ToString("dd-MM-yy_HH-mm"));
                    ///////NISPAHIM//////////////////////////////////////////////////////
                    //var fileTemplate = ConfigurationManager.AppSettings["TemplateNispachimFileMishtaken"];
                    //var dirTemplate = ConfigurationManager.AppSettings["TemplatesFolder"];
                    //Document srcDoc = new Document(dir + @"נספחים א עד ט לחוברת מכרז גנרית מחיר למשתכן.doc");
                    //Document srcDoc = new Document(dirTemplate + fileTemplate);
                    // ביטלתי את הוספת נספחים כי אני רוצה לתת אפשרות לערוך אותם בייחד
                    //doc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);

                    //////////////////////////////////////////////////////////

                   // DocumentBuilder build2 = new DocumentBuilder(srcDoc);
                   // string sHeaderText2 = "sHeaderText2";
                   // build2.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
                   // build2.InsertHtml(String.Format(@"<div dir='LTR' style='text-align:left'><p style = 'text-align:left;direction:RTL' >{0}</p></div>", sHeaderText2));
                    

                   
                    return doc;
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    return null;
                }
            }
        }




        public static string SaveFileToMOSS(string fileNameFS, string folderNameURL, string fileName, int TemplateId)
        {

            string res = "";
            string sEnvironment = ConfigurationManager.AppSettings["Environment"];

            try
            {
                if (sEnvironment == "Dev")
                {



                    #region CODE FOR TEST ENVIRONMENT
                    using (var service = new SPWebService.SPWebService())
                    {
                        //string tmpFolder = @"\\svpfil02\sps_temp$";

                        string tmpFileName = fileNameFS; //   tmpFolder + @"\" + "sp.txt";
                                                         //System.IO.File.Copy(mf.importFile.FileName, tmpFileName, true);
                                                         //string pathUrl = @"http://svtmos10/sites/externalsys/4/50/2" + @"/שיווק/מכרזים/" + @"2475";
                        var returnValue = service.SaveItem("http://svtmos10" + folderNameURL,
                                                           tmpFileName,
                                                           "{ 'ContentType' :'תיקיה' , 'Title' : '" + fileName + "' }",
                                                           "{'ContentType' :'BamaDoc' , 'Title' : 'חוברת מכרז' }",
                                                           "y01");

                        if (!returnValue.IsSuccess)
                            throw new Exception(returnValue.ErrorMsg + "(" + returnValue.ErrorCode + ")");

                        else
                            res = returnValue.Result;



                        // System.Diagnostics.Process.Start(returnValue.FolderPath);
                        // System.Diagnostics.Process.Start(returnValue.Result);
                    }
                    #endregion

                }
                else if (sEnvironment == "Prod")
                {



                    #region CODE FOR PROD ENVIRONMENT
                    using (var service = new svpsps10.SPWebService())
                    {
                        //string tmpFolder = @"\\svpfil02\sps_temp$";
                        string sIdentity = System.Security.Principal.WindowsIdentity.GetCurrent().Name; //  CurrentIdentity.Name
                                                                                                        //sIdentity = Environment.UserName;
                                                                                                        //sIdentity = System.Environment.GetEnvironmentVariable("UserName");
                                                                                                        // sIdentity = "y01";





                        string tmpFileName = fileNameFS; //   tmpFolder + @"\" + "sp.txt";
                                                         //System.IO.File.Copy(mf.importFile.FileName, tmpFileName, true);
                                                         //string pathUrl = @"http://svtmos10/sites/externalsys/4/50/2" + @"/שיווק/מכרזים/" + @"2475";

                        string sServerProd = "http://svpsps10";// "http://mochsps";
                        var returnValue = service.SaveItem(sServerProd + folderNameURL,
                                                                  tmpFileName,
                                                                  "{ 'ContentType' :'תיקיה' , 'Title' : '" + fileName + "' }",
                                                                  "{'ContentType' :'BamaDoc' , 'Title' : 'חוברת מכרז' }",
                                                                  sIdentity);//   "CARMELS"



                        if (!returnValue.IsSuccess)
                            throw new Exception(returnValue.ErrorMsg + "(" + returnValue.ErrorCode + " USER = " + sIdentity + ")");

                        else
                            res = returnValue.Result;

                        try
                        {
                            System.IO.File.Delete(tmpFileName);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Error: " + ex.Message);
                        }

                        //System.Diagnostics.Process.Start(returnValue.FolderPath);
                        //System.Diagnostics.Process.Start(returnValue.Result);


                    }
                    #endregion



                }


            }
            catch (Exception e)
            {
                Log.Error("ERROR EXPORTING TO WORD: " + e.Message);
                res = "ERROR EXPORTING TO WORD : " + e.Message;
            }


            return res;

        }

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
            //string sFilename = GetFilenameBookmarks(tenderId);
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



        //public enum ListTemplate
        //{
        //    BulletDefault = 0,
        //    BulletDisk = 0,
        //    BulletCircle = 1,
        //    BulletSquare = 2,
        //    BulletDiamonds = 3,
        //    BulletArrowHead = 4,
        //    BulletTick = 5,
        //    NumberDefault = 6,
        //    NumberArabicDot = 6,
        //    NumberArabicParenthesis = 7,
        //    NumberUppercaseRomanDot = 8,
        //    NumberUppercaseLetterDot = 9,
        //    NumberLowercaseLetterParenthesis = 10,
        //    NumberLowercaseLetterDot = 11,
        //    NumberLowercaseRomanDot = 12,
        //    OutlineNumbers = 13,
        //    OutlineLegal = 14,
        //    OutlineBullets = 15,
        //    OutlineHeadingsArticleSection = 16,
        //    OutlineHeadingsLegal = 17,
        //    OutlineHeadingsNumbers = 18,
        //    OutlineHeadingsChapter = 19
        //}

        public static string ReplaceBookmarks(string data)
        {
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
            return data;
        }











    }
}

