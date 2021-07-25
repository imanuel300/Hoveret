using System;
using Aspose.Words.Lists;
using Aspose.Words.Saving;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using Aspose.Words;
using WEBAPIODATAV3.Models;
using System.Collections.Generic;
using WEBAPIODATAV3.Utilities;
using System.Text;
using System.Configuration;
using System.IO;
using System.Web;
using log4net;

namespace WEBAPIODATAV3
{
    public static class ExportAsposeMultilevel
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       
        public static string OutputToWordFromPOST(TenderBookletSection ts)
        {
            string dataDir = ConfigurationManager.AppSettings["Asposelic"];
            log.Info("Asposelic" + dataDir);
            Aspose.Words.License _license;
            _license = new Aspose.Words.License();
            _license.SetLicense(ConfigurationManager.AppSettings["Asposelic"]);
            Document doc = new Document(ConfigurationManager.AppSettings["TemplatesFolder"] + @"\tpl.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);
            eMarketingMethod marketingMethod = eMarketingMethod.MechirLeMishtaken;

            builder.Font.Bidi = true;
            builder.ParagraphFormat.Bidi = true;

            Aspose.Words.Font font = builder.Font;
            font.Size = 12;
            font.Name = "David";

            if (ts.SectionNumber == null || ts.SectionNumber == "")
            {
                ts.SectionNumber = "4/50/2";
            }

            string[] sParams = ts.SectionNumber.Split(new char[1] { '/' });

            List list = doc.Lists.Add(ListTemplate.OutlineLegal);


            //  https://apireference.aspose.com/net/words/aspose.words.lists/listlevel/properties/font 
            foreach (ListLevel l in list.ListLevels)
            {
                l.Font.Color = Color.Black;
                l.Font.Size = 12;
                l.Font.Name = "David";
            }

            ///////////  2
            list.ListLevels[1].NumberPosition = 18;  //   39.6;    
            list.ListLevels[1].TabPosition = 50;   //  60;                //  LINEA 1
            list.ListLevels[1].TextPosition = 50; //   72;       //  LINEA 2


            ///////////  3
            list.ListLevels[2].NumberPosition = 50;  //   39.6;    
            list.ListLevels[2].TabPosition = 90;   //  60;                //  LINEA 1
            list.ListLevels[2].TextPosition = 90; //   72;       //  LINEA 2


            //////////  4
            list.ListLevels[3].NumberPosition = 90;
            list.ListLevels[3].TabPosition = 140; //  100;
            list.ListLevels[3].TextPosition = 140;

            list.ListLevels[4].NumberPosition = 140;
            list.ListLevels[4].TabPosition = 180;
            list.ListLevels[4].TextPosition = 180;



            List list1 = doc.Lists.Add(ListTemplate.OutlineLegal);


            foreach (ListLevel l in list1.ListLevels)
            {
                // ListLevel level1 = list.ListLevels[0];
                l.Font.Color = Color.White;
                l.Font.Size = 12;
                l.Font.Name = "David";
            }
            list1.ListLevels[1].NumberPosition = 18;  //   39.6;    
            list1.ListLevels[1].TabPosition = 50;   //  60;                //  LINEA 1
            list1.ListLevels[1].TextPosition = 50; //   72;       //  LINEA 2


            list1.ListLevels[2].NumberPosition = 50;  //  39.6;
            list1.ListLevels[2].TabPosition = 90;  // 60;
            list1.ListLevels[2].TextPosition = 90;  //  72;

            list1.ListLevels[3].NumberPosition = 90;
            list1.ListLevels[3].TabPosition = 140;
            list1.ListLevels[3].TextPosition = 140;

            list1.ListLevels[4].NumberPosition = 140;
            list1.ListLevels[4].TabPosition = 180;
            list1.ListLevels[4].TextPosition = 180;

            string sRightMargin = ConfigurationManager.AppSettings["RightMargin"];
            string sLeftMargin = ConfigurationManager.AppSettings["LeftMargin"];
            double dRightMargin = 1;
            double dLeftMargin = 1;
            Double.TryParse(sRightMargin, out dRightMargin);
            Double.TryParse(sLeftMargin, out dLeftMargin);
            builder.PageSetup.RightMargin = ConvertUtil.InchToPoint(dRightMargin);
            builder.PageSetup.LeftMargin = ConvertUtil.InchToPoint(dLeftMargin);
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Justify;

            builder.ListFormat.List = list;  //  doc.Lists.Add(ListTemplate.OutlineLegal);

            int tenderId = ts.Id;
            DataSet ds = GetData(tenderId);
            //log = InitializeLog4Net();
            log.Info("GetData: " + tenderId);
            marketingMethod = ReplaceBookmarks(ds, tenderId);
            log.Info("ReplaceBookmarks: ");
            BuildBuilder(builder, ds, list, list1);
            log.Info("BuildBuilder: ");
            builder.ListFormat.List = null;

            ////////////////////////////////////

            PageSetup ps = builder.PageSetup;
            ps.DifferentFirstPageHeaderFooter = true;

            string sHeaderText = UtilityMethods.GetHeaderBookmarks(ts.Id);
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            // builder.InsertHtml(@"<div dir='LTR' style='text-align:left'><p style = 'text-align:left;direction:RTL' > עיברית ימין לשמאול.</p></div>");
            //builder.InsertHtml(String.Format(@"<div dir='LTR' style='text-align:left'><p style = 'text-align:left;direction:RTL' >{0}</p></div>", sHeaderText));
            builder.InsertHtml(String.Format(@"<div  style=''><p style = '' >{0}</p></div>", sHeaderText));

            ////////////////////////////////////

            dataDir = dataDir + string.Format("Lists_{0}.doc", DateTime.Now.ToString("dd-MM-yy_HH-mm"));



            ///////NISPAHIM//////////////////////////////////////////////////////

            string dirTemplate, fileTemplate;
            GetNispachimFile(out dirTemplate, out fileTemplate, marketingMethod);
            //Document srcDoc = new Document(dir + @"נספחים א עד ט לחוברת מכרז גנרית מחיר למשתכן.doc");
            Document srcDoc = new Document(dirTemplate + fileTemplate);
            doc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);

            //////////////////////////////////////////////////////////

            DocumentBuilder build2 = new DocumentBuilder(srcDoc);
            string sHeaderText2 = UtilityMethods.GetHeaderBookmarks(ts.Id);
            build2.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            build2.InsertHtml(String.Format(@"<div dir='LTR' style='text-align:left'><p style = 'text-align:left;direction:RTL' >{0}</p></div>", sHeaderText2));
            ///////////////////////////////////////////////////* CODE DES EXPORTTOWORD *////////////////////////////////////////////////////

            string sTargetFolder = "", fileName = "";
            sTargetFolder = @"/sites/externalsys/" + sParams[0] + "/" + sParams[1] + "/" + sParams[2] + "/שיווק/מכרזים/" + ts.Id.ToString() + "";
            //sTargetFolder = @"/sites/externalsys/" + sParams[0] + "/" + sParams[1] + "/" + sParams[2] + "/XXX/YYY/" + ts.Id.ToString() + "";
            string sTargetFolderASCII = sTargetFolder;//@"/sites/externalsys/2/14/7/9999" ;   
                                                      //  string documentTitle = "FILE"; 
            string documentTitle = @"חוברת מכרז";
            Dictionary<string, object> spProperties = null;
            try
            {
                spProperties = SetDocumentProperties(doc, null, sTargetFolder, documentTitle);
                spProperties = SetDocumentProperties(srcDoc, null, sTargetFolder, documentTitle);
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message);
                return ex.Message;
                throw ex;
            }
            try
            {
                fileName = GetPath(doc, documentTitle, sTargetFolder, spProperties);  //   sTargetFolder
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message);
                return ex.Message;
                throw ex;
            }
            //throw new Exception("Test : ");

            #region SAVE TO LOCAL
            string sPathLocal = System.Web.HttpContext.Current.Server.MapPath(".");
            string sPathFileSystem = ConfigurationManager.AppSettings["PathFileSystem"];
            string d = string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
            //string sFolder = @"C:\Users\CarmelS\Downloads\_TEST\OUTPUT\";
            string sSectionNumber = ts.SectionNumber.Replace(@"/", @"\");
            string httpfileName = HttpContext.Current.Request.Url.Authority + @"/Doc/" + sSectionNumber.Replace(@"\", @"/") + @"/" + d + ".docx";
            string fileNameLocal = sPathFileSystem + "\\" + sSectionNumber + @"\" + d + ".docx";
            string folderNameLocal = sPathFileSystem + "\\" + sSectionNumber + @"\";
            #endregion


            log.Info(HttpContext.Current.Request.Url.Host);
            log.Info(HttpContext.Current.Request.Url.Authority);
            log.Info(HttpContext.Current.Request.Url.Port);
            log.Info(HttpContext.Current.Request.Url.AbsolutePath);
            log.Info(HttpContext.Current.Request.ApplicationPath);
            log.Info(HttpContext.Current.Request.Url.AbsoluteUri);
            log.Info(HttpContext.Current.Request.Url.PathAndQuery);
            // localhost
            log.Info("new file fileNameLocal:" + fileNameLocal);
            log.Info("new file httpfileName:" + httpfileName);
            //Directory.CreateDirectory(sPathFileSystem + "\\" + sSectionNumber);
            doc.Save(fileNameLocal);
            return httpfileName;
            //string sLocalFile = SaveFileToMOSS(fileNameLocal, sTargetFolder, d, ts.Id);
            // System.Diagnostics.Process.Start(sLocalFile);

            //UtilityMethods.SaveFileURLToDB(ts.Id, sLocalFile);


            ////////////////////////////////////////////////////////////////////////////////////////*/



            //builder.Document.Save(dataDir);

            //System.Diagnostics.Process.Start(sLocalFile);

            //return sLocalFile;


        }

        private static void GetNispachimFile(out string dirTemplate, out string fileTemplate, eMarketingMethod marketingMethod)
        {
            dirTemplate = ConfigurationManager.AppSettings["TemplatesFolder"];

            switch (marketingMethod)
            {
                case eMarketingMethod.MechirLeMishtaken:
                    fileTemplate = ConfigurationManager.AppSettings["TemplateNispachimFileMishtaken"];
                    break;
                case eMarketingMethod.RemiKarkha:
                    fileTemplate = ConfigurationManager.AppSettings["TemplateNispachimFileRemi"];
                    break;
                case eMarketingMethod.DiurMugan:
                    fileTemplate = ConfigurationManager.AppSettings["TemplateNispachimFileDiur"];
                    break;
                default:
                    fileTemplate = ConfigurationManager.AppSettings["TemplateNispachimFileMishtaken"];
                    break;
            }

            //fileTemplate = ConfigurationManager.AppSettings["TemplateNispachimFileMishtaken"];
        }

        private static eMarketingMethod ReplaceBookmarks(DataSet ds, int tenderId)
        {
            Dictionary<string, string> oDict = UtilityMethods.CreateBookmarksDictionary(tenderId);


            string sText;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                sText = (string)row["SECTIONBODY"];
                foreach (string bm in oDict.Keys)
                {

                    if (string.Compare(bm, "id") != 0)
                    {
                        sText = sText.ToLower().Replace(bm.ToLower(), oDict[bm]);
                    }

                }
                row["SECTIONBODY"] = sText;
            }


            if (oDict.ContainsKey("marketingmethod"))
                return MarketingMethod.Convert(oDict["marketingmethod"]);
            else
                //throw new Exception("המפתח הנתון לא נמצא במילון.");
                return 0;
        }

        private static void BuildBuilder(DocumentBuilder builder, DataSet ds, List defaultList, List paraList)
        {
            builder.ListFormat.ListLevelNumber = 0;
            DataTable dt = ds.Tables["Clauses"];
            foreach (DataRow record in dt.Rows)
            {
                AddClause(builder, record, defaultList, paraList);
            }

            //foreach (Paragraph para in builder.Document.GetChildNodes(NodeType.Paragraph, true))
            //{

            //    if (para.IsListItem)
            //    {


            //    }
            //}

        }

        private static void AddClause(DocumentBuilder builder, DataRow record, List defaultList, List paraList)
        {

            string sText = String.Empty;
            int iLevel = 0;

            if (iLevel >= 0)
            {
                string sPrefix = "<span  dir=RTL style='margin-top:12.0pt;font-size:12.0pt;font-family:David,sans-serif;text-align:justify;text-align-last: right;'>",
                    sSuffix = "</span>";
                iLevel = (int)record["MULTILEVEL"] - 1;
                sText = String.Format("{0}{1}{2}", sPrefix, (string)record["SECTIONBODY"], sSuffix);

                if ((bool)record["PARAGRAPH"])
                {
                    builder.ListFormat.List = paraList;   //  list1
                }
                else
                {
                    builder.ListFormat.List = defaultList;   //  list
                }


                builder.ListFormat.ListLevelNumber = iLevel;
            }
            builder.InsertHtml(sText);
            builder.Write("\n");
            builder.ListFormat.List = null;
            builder.Write("\n");
            builder.ListFormat.List = defaultList;

        }

        private static DataSet GetData(int tenderId)
        {
            try
            {
                //string sEnvironment = ConfigurationManager.AppSettings["Environment"];
                //string sServer = "svdsql17";
                //if (sEnvironment == "Prod")
                //{
                //    sServer = "svpsql17";
                //}

                SqlConnection oConn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBBMEntitiesADO"].ConnectionString);
                //todo - entety  using (var dbEntitie = new DBBMEntities())
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                        da.SelectCommand = new SqlCommand();
                        da.SelectCommand.Connection = oConn;
                        da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        da.SelectCommand.CommandText = "SP_TENDER_ASPOSE";
                        da.SelectCommand.Parameters.Add(new SqlParameter("@TENDER", tenderId));  // 2475

                        DataSet ds = new DataSet();
                        da.Fill(ds, "Clauses");

                        return ds;
                }

            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
                log.Error("SQL Error: " + ex.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                log.Error("Error: " + e.Message);
                return null;
            }
        }


        private static string SaveFileToMOSS(string fileNameFS, string folderNameURL, string fileName, int id)
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
                        {
                            res = returnValue.Result;
                            UtilityMethods.SaveFileURLToDB(id, res);
                        }



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
                        catch (Exception e)
                        {
                            log.Error("Error: " + e.Message);
                        }

                        //System.Diagnostics.Process.Start(returnValue.FolderPath);
                        //System.Diagnostics.Process.Start(returnValue.Result);


                    }
                    #endregion



                }


            }
            catch (Exception e)
            {
                res = "ERROR EXPORTING TO WORD : " + e.Message;
                log.Error("Error: " + e.Message);
            }


            return res;

        }

        private static Dictionary<string, object> SetDocumentProperties(Document doc, Dictionary<string, string> dicDataSql, string sharePointUrl, string title)
        {
            var spProperties = new Dictionary<string, object>();
            spProperties.Add(sharePointUrl, "BamaProject");
            spProperties.Add("ContentType", "BamaDoc");

            doc.BuiltInDocumentProperties["Title"].Value = title;

            return spProperties;
        }

        private static string GetPath(Document doc, string documentTitle, string sharePointUrl, Dictionary<string, object> spProperties)
        {


            var uncPath = sharePointUrl.Replace(@"/", @"\");
            var host = new Uri(@"http://svtmos10").Host;

            uncPath = @"\\" + host + uncPath;




            var fileName = Path.Combine(uncPath, documentTitle + ".docx");



            return fileName.ToString();

        }







    }


}
