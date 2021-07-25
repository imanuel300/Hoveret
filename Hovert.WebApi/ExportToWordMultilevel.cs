using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.Words;
using WEBAPIODATAV3.Models;
using WEBAPIODATAV3.Utilities;
using System.Web;
using System.Drawing;
using System.Configuration;
using log4net;

namespace WEBAPIODATAV3
{
    public class ExportToWordMultilevel
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string OutputToWordFromPOST(TenderBookletSection ts)//, string webRoot)
        {
            string sTargetFolder = "", fileName = "";


            if (ts.SectionNumber == null || ts.SectionNumber == "")
            {
                ts.SectionNumber = "4/50/2";
            }

            string[] sParams = ts.SectionNumber.Split(new char[1] { '/' });
            try
            {
                StringBuilder sb = new StringBuilder();
                var _license = new Aspose.Words.License();
                try
                {

                    _license.SetLicense(ConfigurationManager.AppSettings["Asposelic"]);


                    //string name = @"TEMPLATE.docx";
                    // fileName = System.IO.Path.Combine(webRoot, name);
                    //if (!Directory.Exists(webRoot))
                    //{
                    //    Directory.CreateDirectory(webRoot);
                    //}
                    //fileName = Path.Combine(@"C:\Users\CarmelS\Downloads\", name);
                    //var estEncoding = Encoding.GetEncoding(1252);
                    //var est = System.IO.File.ReadAllText(fileName, estEncoding);
                    //var utf = System.Text.Encoding.UTF8;
                    //est = utf.GetString(System.Text.Encoding.Convert(estEncoding, utf, estEncoding.GetBytes(est)));
                    // string encodedTargetFolder = HttpUtility.UrlEncode(sTargetFolder);
                }
                catch (Exception ex)
                {
                    log.Error("ERROR " + ex.Message);
                    return ex.Message;
                    throw ex;

                }
                Aspose.Words.Document doc = new Aspose.Words.Document();// (fileName);                                                                       
                sb.Append(ts.SectionBody);
                string sHeaderText = "";

                //  	מכרז מספר mmi_mahoz_shortname/tendernumber/tenderyear	
                sHeaderText = UtilityMethods.GetHeaderBookmarks(ts.Id);








                if (true)
                {
                    sTargetFolder = @"/sites/externalsys/" + sParams[0] + "/" + sParams[1] + "/" + sParams[2] + "/שיווק/מכרזים/" + ts.Id.ToString() + "";
                    //sTargetFolder = @"/sites/externalsys/" + sParams[0] + "/" + sParams[1] + "/" + sParams[2] + "/XXX/YYY/" + ts.Id.ToString() + "";
                    string sTargetFolderASCII = sTargetFolder;//@"/sites/externalsys/2/14/7/9999" ;   
                                                              //  string documentTitle = "FILE"; 
                    string documentTitle = @"חוברת מכרז";
                    Dictionary<string, object> spProperties = null;
                    try
                    {
                        spProperties = SetDocumentProperties(doc, null, sTargetFolder, documentTitle); // sTargetFolder 
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
                    string httpfileName = HttpContext.Current.Request.Url.Authority + "/Doc/" + sSectionNumber.Replace(@"\", @"/") + @"/" + d + ".docx";
                    string fileNameLocal = sPathFileSystem + "\\" + sSectionNumber + @"\" + d + ".docx";
                    string folderNameLocal = sPathFileSystem + "\\" + sSectionNumber + @"\";
                    #endregion


                    doc.Save(fileNameLocal);
                    string sLocalFile = SaveFileToMOSS(fileNameLocal, sTargetFolder, d);
                    // System.Diagnostics.Process.Start(sLocalFile);

                    //UtilityMethods.SaveFileURLToDB(ts.Id, sLocalFile);
                    log.Info("new file " + sLocalFile);
                    return sLocalFile;
                }
                return "Couldn't write doc to folder";
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message);
                string e = ex.Message + " ------ TARGET = " + sTargetFolder + " ---- FILENAME = " + fileName;
                Exception newExc = new Exception(e);
                return ex.Message;
                throw newExc;
            }
        }



        private string SaveFileToMOSS(string fileNameFS, string folderNameURL, string fileName)
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
                            log.Error("Error: " + ex.Message);
                        }

                        //System.Diagnostics.Process.Start(returnValue.FolderPath);
                        //System.Diagnostics.Process.Start(returnValue.Result);


                    }
                    #endregion

                   

                }


            }
            catch (Exception e)
            {
                log.Error("ERROR EXPORTING TO WORD: " + e.Message);
                res = "ERROR EXPORTING TO WORD : " + e.Message;
            }


            return res;

        }

        private Dictionary<string, object> SetDocumentProperties(Document doc, Dictionary<string, string> dicDataSql, string sharePointUrl, string title)
        {
            var spProperties = new Dictionary<string, object>();
            spProperties.Add(sharePointUrl, "BamaProject");
            spProperties.Add("ContentType", "BamaDoc");

            doc.BuiltInDocumentProperties["Title"].Value = title;

            return spProperties;
        }

        private string GetPath(Document doc, string documentTitle, string sharePointUrl, Dictionary<string, object> spProperties)
        {


            var uncPath = sharePointUrl.Replace(@"/", @"\");
            var host = new Uri(@"http://svtmos10").Host;  

            uncPath = @"\\" + host + uncPath;

                     


            var fileName = Path.Combine(uncPath, documentTitle + ".docx");



            return fileName.ToString();

        }
             
                 
      


    }
}
