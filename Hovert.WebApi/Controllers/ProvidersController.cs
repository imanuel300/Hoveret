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
    builder.EntitySet<Provider>("Providers");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ProvidersController : ODataController
    {
        private DBBMEntities db = new DBBMEntities();

        // GET: odata/Providers
        [EnableQuery]
        public IQueryable<Provider> GetProviders()
        {
            return db.Providers;
        }

        // GET: odata/Providers(5)
        [EnableQuery]
        public SingleResult<Provider> GetProvider([FromODataUri] int key)
        {
            return SingleResult.Create(db.Providers.Where(provider => provider.Id == key));
        }

        // PUT: odata/Providers(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Provider> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Provider provider = db.Providers.Find(key);
            if (provider == null)
            {
                return NotFound();
            }

            patch.Put(provider);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProviderExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(provider);
        }

        // POST: odata/Providers
        public IHttpActionResult Post(Provider provider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Providers.Add(provider);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ProviderExists(provider.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(provider);
        }

        // PATCH: odata/Providers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Provider> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Provider provider = db.Providers.Find(key);
            if (provider == null)
            {
                return NotFound();
            }

            patch.Patch(provider);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProviderExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(provider);
        }

        // DELETE: odata/Providers(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Provider provider = db.Providers.Find(key);
            if (provider == null)
            {
                return NotFound();
            }

            db.Providers.Remove(provider);
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

        private bool ProviderExists(int key)
        {
            return db.Providers.Count(e => e.Id == key) > 0;
        }
    }
}
