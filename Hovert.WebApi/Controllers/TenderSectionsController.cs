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
    public class TenderSectionsController : ODataController
    {
        private DBBMEntities db = new DBBMEntities();

        // GET: odata/TenderSections
        [EnableQuery]
        public IQueryable<TenderSection> GetTenderSections()
        {
            return db.TenderSections;
        }

        // GET: odata/TenderSections(5)
        //       [EnableCors("http://localhost:55912", // Origin
        //            null,                     // Request headers
        //            "GET",                    // HTTP methods
        //            "bar",                    // Response headers
        //            SupportsCredentials = true  // Allow credentials
        //)]
        [EnableQuery]
        public SingleResult<TenderSection> GetTenderSection([FromODataUri] int key)
        {
            SingleResult<TenderSection> oTS = null;
            int iOffset = Convert.ToInt32(ConfigurationManager.AppSettings["SectionOffset"].ToString());
            IQueryable<TenderSection> oIq = db.TenderSections
                .Where(
                    tenderSection => tenderSection.SectionsOrder >= key
                    && tenderSection.SectionsOrder < key + iOffset
                 );// tenderSection => tenderSection.Id == key);
            TenderSection section = null;
            if (oIq.Count() > 0)
            {
                section = getConcatenatedSections(oIq);
            }

            section.Text = section.Text.Replace("<<marketingMethod>>", "  שיטת שיווק : מחיר למשתכן  ");

            var l = new List<TenderSection>();
            l.Add(section);
            IQueryable<TenderSection> ret = l.AsQueryable();
            oTS = SingleResult.Create(ret);
            return oTS;
        }

       
        // PUT: odata/TenderSections(5)
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

            return Updated(tenderSection);
        }

        // POST: odata/TenderSections
        public IHttpActionResult Post(TenderSection tenderSection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TenderSections.Add(tenderSection);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TenderSectionExists(tenderSection.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(tenderSection);
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

            return Updated(tenderSection);
        }

        // DELETE: odata/TenderSections(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            TenderSection tenderSection = db.TenderSections.Find(key);
            if (tenderSection == null)
            {
                return NotFound();
            }

            db.TenderSections.Remove(tenderSection);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
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
