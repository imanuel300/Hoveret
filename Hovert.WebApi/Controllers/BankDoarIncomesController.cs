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
    builder.EntitySet<BankDoarIncome>("BankDoarIncomes");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class BankDoarIncomesController : ODataController
    {
        private DBBMEntities db = new DBBMEntities();

        // GET: odata/BankDoarIncomes
        [EnableQuery]
        public IQueryable<BankDoarIncome> GetBankDoarIncomes()
        {
            return db.BankDoarIncomes;
        }

        // GET: odata/BankDoarIncomes(5)
        [EnableQuery]
        public SingleResult<BankDoarIncome> GetBankDoarIncome([FromODataUri] int key)
        {
            return SingleResult.Create(db.BankDoarIncomes.Where(bankDoarIncome => bankDoarIncome.id == key));
        }

        // PUT: odata/BankDoarIncomes(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<BankDoarIncome> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BankDoarIncome bankDoarIncome = db.BankDoarIncomes.Find(key);
            if (bankDoarIncome == null)
            {
                return NotFound();
            }

            patch.Put(bankDoarIncome);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankDoarIncomeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(bankDoarIncome);
        }

        // POST: odata/BankDoarIncomes
        public IHttpActionResult Post(BankDoarIncome bankDoarIncome)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BankDoarIncomes.Add(bankDoarIncome);
            db.SaveChanges();

            return Created(bankDoarIncome);
        }

        // PATCH: odata/BankDoarIncomes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<BankDoarIncome> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BankDoarIncome bankDoarIncome = db.BankDoarIncomes.Find(key);
            if (bankDoarIncome == null)
            {
                return NotFound();
            }

            patch.Patch(bankDoarIncome);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankDoarIncomeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(bankDoarIncome);
        }

        // DELETE: odata/BankDoarIncomes(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            BankDoarIncome bankDoarIncome = db.BankDoarIncomes.Find(key);
            if (bankDoarIncome == null)
            {
                return NotFound();
            }

            db.BankDoarIncomes.Remove(bankDoarIncome);
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

        private bool BankDoarIncomeExists(int key)
        {
            return db.BankDoarIncomes.Count(e => e.id == key) > 0;
        }
    }
}
