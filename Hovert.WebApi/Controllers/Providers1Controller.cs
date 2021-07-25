using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
    builder.EntitySet<Providers1>("Providers1");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class Providers1Controller : ODataController
    {
        private DBBMEntities db = new DBBMEntities();

        // GET: odata/Providers1
        [EnableQuery]
        public IQueryable<Provider> GetProviders1()
        {
            return db.Providers;
        }

        // GET: odata/Providers1(5)
        [EnableQuery]
        public SingleResult<Provider> GetProviders1([FromODataUri] int key)
        {
            return SingleResult.Create(db.Providers.Where(providers1 => providers1.Id == key));
        }

        // PUT: odata/Providers1(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Provider> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Provider providers1 = db.Providers.Find(key);
            if (providers1 == null)
            {
                return NotFound();
            }

            patch.Put(providers1);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Providers1Exists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(providers1);
        }

        // POST: odata/Providers1
        public IHttpActionResult Post(Provider providers1)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Providers.Add(providers1);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (Providers1Exists(providers1.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(providers1);
        }

        // PATCH: odata/Providers1(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Provider> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Provider providers1 = db.Providers.Find(key);
            if (providers1 == null)
            {
                return NotFound();
            }

            patch.Patch(providers1);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Providers1Exists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(providers1);
        }

        // DELETE: odata/Providers1(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Provider providers1 = db.Providers.Find(key);
            if (providers1 == null)
            {
                return NotFound();
            }

            db.Providers.Remove(providers1);
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

        private bool Providers1Exists(int key)
        {
            return db.Providers.Count(e => e.Id == key) > 0;
        }
    }
}
