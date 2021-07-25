using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using System.Web.Http.OData.Routing;
using WEBAPIODATAV3.Models;
using Microsoft.Data.OData;
using System.Web.Http.Cors;

namespace WEBAPIODATAV3.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using WEBAPIODATAV3.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Filename>("TenderFilename");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
  //  [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TenderFilenameController : ApiController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        // GET: odata/TenderFilename
        [HttpGet]
        public async Task<IHttpActionResult> GetTenderFilename(ODataQueryOptions<Filename> queryOptions)
        {
            // validate the query.
            try
            {
                queryOptions.Validate(_validationSettings);
            }
            catch (ODataException ex)
            {
                return BadRequest(ex.Message);
            }

            // return Ok<IEnumerable<Filename>>(filenames);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // GET: odata/TenderFilename(5)
        [HttpGet]
        public async Task<IHttpActionResult> GetFilename([FromODataUri] int key, ODataQueryOptions<Filename> queryOptions)
        {
            // validate the query.
            try
            {
                queryOptions.Validate(_validationSettings);
            }
            catch (ODataException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok<Filename>(new Models.Filename(key, Utilities.UtilityMethods.GetFilenameBookmarks(key)));
            //return StatusCode(HttpStatusCode.NotImplemented);
        }

     
    }
}
