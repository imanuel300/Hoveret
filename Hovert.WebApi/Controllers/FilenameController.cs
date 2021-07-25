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

namespace WEBAPIODATAV3.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using WEBAPIODATAV3.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<TenderSectionType>("Filename");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class FilenameController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        // GET: odata/Filename
        public async Task<IHttpActionResult> GetFilename(ODataQueryOptions<string > queryOptions)
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

            var oDictionary = Utilities.UtilityMethods.oDict;
            Task<string> task = new Task<string>(() => Utilities.UtilityMethods.GetFilenameBookmarks(2475));
            task.Start();
            string sFilename = await task;
           
           // return Ok<string>(sFilename);  
            
            return Ok<IEnumerable<string>>(new List<string>() { sFilename }); // StatusCode(HttpStatusCode.NotImplemented);
        }

        // GET: odata/Filename(5)
        public async Task<IHttpActionResult> GetFilenameByKey([FromODataUri] int key, ODataQueryOptions<string > queryOptions)
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

            var oDictionary = Utilities.UtilityMethods.oDict;
            Task<string> task = new Task<string>(() => Utilities.UtilityMethods.GetFilenameBookmarks(2475));
            task.Start();
            string sFilename = await task;
            // return Ok<TenderSectionType>(tenderSectionType);
            return Ok<string>(sFilename); //StatusCode(HttpStatusCode.NotImplemented);
        }



    }
}
