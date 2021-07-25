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
using System.Data.SqlClient;

namespace WEBAPIODATAV3.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using WEBAPIODATAV3.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<TenderSection>("TenderSections");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    // [EnableCors(origins: "http://localhost:52253", headers: "*", methods: "*")]
    //[RoutePrefix("api/TenderTemplateEditor")]
    public class TenderTemplateEditorController : ApiController//:ODataController
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       

        private DBBMEntities db = new DBBMEntities();

        // GET: odata/TenderTemplateEditor
       // [EnableQuery]
        [HttpGet]
        public IQueryable<TenderTemplatesBookletSection> GetTenderTemplateEditor()
        {
            return db.TenderTemplatesBookletSections.OrderBy(d => d.TenderSectionId ?? 0);
        }

        //http://localhost:52253/odata/TenderTemplateEditor(10)
        // GET: odata/TenderTemplateEditor(5)
        //       [EnableCors("http://localhost:55912", // Origin
        //            null,                     // Request headers
        //            "GET",                    // HTTP methods
        //            "bar",                    // Response headers
        //            SupportsCredentials = true  // Allow credentials
        //)]
        //[EnableQuery]
        [HttpGet]
        public IQueryable<TenderTemplatesBookletSection> GetTenderTemplateEditor(int key)
        {
            //IQueryable<TenderTemplatesBookletSection> oIq = db.TenderTemplatesBookletSections
            //    .Where(TenderTemplatesBookletSection => TenderTemplatesBookletSection.Id == key);

            //SingleResult<TenderTemplatesBookletSection> oTS = SingleResult.Create(oIq);
            // return oTS;
            return db.TenderTemplatesBookletSections
                 .Where(TenderTemplatesBookletSection => TenderTemplatesBookletSection.Id == key);
        }

        // POST: http://localhost:52253/TenderTemplateEditor/AddNewRow?id=10
        // [EnableQuery]
        [HttpPost]
        public int AddNewRow(int id)
        {
            Log.Info("AddNewRow:"+id);
            using (var dbEntitie = new DBBMEntities())
            using (var db = new SqlConnection(ConfigurationManager.ConnectionStrings["DBBMEntitiesADO"].ConnectionString))
            {
                try
                {
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand("UPDATE [dbo].[TenderTemplatesBookletSections] SET TenderSectionId = TenderSectionId + 1 where TenderSectionId > " + id, db);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    SqlCommand cmd1 = new SqlCommand("insert into [dbo].[TenderTemplatesBookletSections](Id, TenderSectionId, TenderId) values((select max(id) + 1 from [dbo].[TenderTemplatesBookletSections]),"+ (id+1) +",0)" , db);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                    da1.Fill(ds);
                    
                    db.Close();
                    //var response = new HttpResponseMessage();
                    //response.Content = new StringContent(ds.ToString());
                    //return response;

                    var db_TenderTemplatesBookletSections  = dbEntitie.Set<TenderTemplatesBookletSection>();
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
       
        // PUT: odata/TenderTemplateEditor(5)
        [HttpPut]
        public IHttpActionResult Put([FromODataUri] int key, Delta<TenderSection> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TenderSection tenderSection = db.TenderSections.Find(key);
            if (tenderSection == null)
            {
                return NotFound();
            }

            patch.Put(tenderSection);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TenderSectionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(tenderSection);
        }


        // POST: odata/TenderTemplateEditor
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
                    }
                    else
                    {
                        db.TenderTemplatesBookletSections.Add(tenderSection);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
                return Ok(tenderSection);
            }
        }

        // DELETE: odata/TenderSections(5)
        [HttpPost]
        public IHttpActionResult Delete([FromODataUri] int id)
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

        // PATCH: odata/TenderSections(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<TenderSection> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TenderSection tenderSection = db.TenderSections.Find(key);
            if (tenderSection == null)
            {
                return NotFound();
            }

            patch.Patch(tenderSection);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TenderSectionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(tenderSection);
        }

       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TenderSectionExists(int key)
        {
            return db.TenderSections.Count(e => e.Id == key) > 0;
        }

        private TenderSection getConcatenatedSections(IQueryable<TenderSection> oIq)
        {
            TenderSection oTs = new TenderSection();
            foreach (TenderSection ts in oIq)
            {
                if (IsDisplayed(ts))
                {
                    oTs.Text += ts.Text;
                }
            }

            return oTs;
        }

        private bool IsDisplayed(TenderSection ts)
        {
            return true;
            //return ts.ConditionId ?? false;
        }
    }
}
