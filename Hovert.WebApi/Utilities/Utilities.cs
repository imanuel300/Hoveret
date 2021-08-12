using Aspose.Words;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using WEBAPIODATAV3.Models;

namespace WEBAPIODATAV3.Utilities
{
    internal enum SectionTypes
    {
        Father = 0,
        Son = 1,
        Grandson = 2,
        Grandgrandson = 3
    }

    public static class Routes
    {
        public static string SignUp() => "signup";
    }

    internal static class ColumnKeys
    {
        internal const string Date = "Date";
        internal const string Value = "Value";

    }
    public static class UtilityMethods
    {
        //private static log4net.ILog log;
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       

        public static DataSet ConvertForEitanDictionary(this DbSet<TenderTemplatesBookletSection> SectionsDBSet)
        {
            DataSet oDS = new DataSet();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(TenderTemplatesBookletSection));
            DataTable tbl = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                tbl.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (TenderTemplatesBookletSection section in SectionsDBSet)
            {
                DataRow row = tbl.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(section) ?? DBNull.Value;
                tbl.Rows.Add(row);
            }
            oDS.Tables.Add(tbl);
            return oDS;
        }

        internal static Dictionary<string, string> CreateBookmarksDictionary(int tenderId) => GetDictionaryByDataSet(
            GetBookMarks(tenderId, null)
            );
        //(new DBBMEntities().TenderTemplatesBookletSections.ConvertForEitanDictionary()));



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

        private static string ConvertTableToHtmlTable(DataTable dt, string bmStyle, bool hidden)
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


        private static SqlConnection OpenConnection()
        {

            Log.Info(_connectionString);
            try
            {
                SqlConnection conn = null;
                conn = new SqlConnection(_connectionString);
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                SqlConnection conn = null;
                return conn;
            }
        }


        public static DataTable GetDataTableByQuery(string strquery)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                try
                {
                    DataTable dt = new DataTable();
                    SqlCommand cmd = new SqlCommand(strquery, db);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                    adpt.Fill(dt);
                    return dt;
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
            Log.Info("spLett_TenderBooklet");
            DataSet dsBookMark = null;
            Dictionary<string, string> dicBookMarks = null;
            string userName = _UserName;//  System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring((System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length)-3,3);
            userId = userId ?? userName;
            SqlCommand cmd = new SqlCommand("[dbo].[spLett_TenderBooklet]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@TenderId", tenderId));

            dsBookMark = GetDataByCommand(cmd, tenderId);

            return dsBookMark;

        }


        private static DataSet GetDataByCommand(SqlCommand executionCommand, int tenderId)
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


            //    using (var db = new SqlConnection(_connectionString))
            //{
            //    try
            //    {
            //        //SqlConnection conn = OpenConnection();

            //        DataSet dsBookMarks = new DataSet();

            //       // if (db.State == ConnectionState.Open)
            //        //{
            //            executionCommand.Connection = db;
            //            SqlDataAdapter da = new SqlDataAdapter();
            //            da.SelectCommand = executionCommand;
            //            da.Fill(dsBookMarks);

            //            if (db != null)
            //                db.Close();

            //      //  }
            //        return dsBookMarks;
            //    }
            //    catch (Exception e)
            //    {
            //        Log.Error(e.ToString());
            //        DataSet dsBookMarks = new DataSet();
            //        return dsBookMarks;
            //    }


            //}
        }

        internal static string FileName;
        internal static Dictionary<string, string> oDict;
        internal static string GetBookmarkValue(this string text, int tenderId)
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
            // MmiMahozShortName / TENDERNUMBER / TENDERYEAR
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
        public static string GetHeaderBookmarks(int tenderId)
        {
            string sRet = String.Empty, mmi_mahoz_shortname = String.Empty, tendernumber = String.Empty, tenderyear = String.Empty;
            //  	מכרז מספר mmi_mahoz_shortname/tendernumber/tenderyear	
            oDict = CreateBookmarksDictionary(tenderId);
            //oDict.TryGetValue("MmiMahozShortName".ToLower(), out sShortName);
            //oDict.TryGetValue("TenderNumber".ToLower(), out  sTenderNumber);
            //oDict.TryGetValue("TenderYear".ToLower(), out sTenderYear );
            oDict.TryGetValue("mmi_mahoz_shortname".ToLower(), out mmi_mahoz_shortname);
            oDict.TryGetValue("tendernumber".ToLower(), out tendernumber);
            oDict.TryGetValue("tenderyear".ToLower(), out tenderyear);

            sRet = String.Format("מכרז מספר " + "{0}/{1}/{2}", mmi_mahoz_shortname, tendernumber, tenderyear);
            //sRet = String.Format("{0}_{1}_{2}_",sShortName,sTenderNumber,sTenderYear);
            return sRet;

        }
        public static Dictionary<string, string> GetAwaiter(Dictionary<string, string> dict) { return dict; }

        private static string ModifyHTML(string sOrigHTML)
        {

            string sRet = String.Empty;
            if (sOrigHTML != null && sOrigHTML.Length > 0)
            {
                sRet = sOrigHTML.Replace("font-size:11", "font-size:9px");
            }


            return sRet;
        }

        private static string _UserName { get { return System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring((System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length) - 3, 3); } }
        private static string _connectionString = ConfigurationManager.ConnectionStrings["DBBMEntitiesADO"].ConnectionString;

        //@"Data Source = svdsql17; Initial Catalog = DBBM; Integrated Security = SSPI; MultipleActiveResultSets=true";// System.Configuration.ConfigurationManager.ConnectionStrings["BamaConnectionString"].ConnectionString;


        public static void InsertText(this DocumentBuilder builder, StringBuilder text)
        {
            text.Replace("Times New Roman", "David"); //  text.Replace("dav26255", "David");
            builder.InsertHtml(text.ToString());
        }



        public static void SaveFileURLToDB(int id, string sLocalFile)
        {

            using (var conn = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spSaveTenderFileURL", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {

                command.Parameters.Add(new SqlParameter("@ID", id));
                command.Parameters.Add(new SqlParameter("@URL", sLocalFile));
                conn.Open();
                command.ExecuteNonQuery();
            }

        }


        public static void SaveLog(string msg)
        {
            string path = String.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("YYYY-MM-DD_HH:MM:SS"));
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteAsync(msg);
                sw.Flush();

            }




        }

    }

}
