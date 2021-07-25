using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using WEBAPIODATAV3.Models;

namespace WEBAPIODATAV3.Controllers
{
    public class FileController : ApiController
    {
        public IQueryable<string > GetFile()
        {
            return new List<string>() {  "TEST"}.AsQueryable();
        }


        [Route("api/Filename/{key}")]
        public async Task<IHttpActionResult> GetFile([FromODataUri]  int key)
        {
            Task<string> task = new Task<string>(() => Utilities.UtilityMethods.GetFilenameBookmarks(key));
            task.Start();
            string sFile = await task;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent("{'filename':'" + sFile + "'}", Encoding.UTF8, "application/json");
            return ResponseMessage(response);
        }






        [Route("api/File/{key}")]
        public SingleResult<TenderBookletSection> GetFile1([FromODataUri]  int key)
        {
            string sFile = Utilities.UtilityMethods.GetFilenameBookmarks(key);

            List<TenderBookletSection> template = new List<TenderBookletSection>();
            TenderBookletSection e = new Models.TenderBookletSection();
            e.SectionNumber = sFile;
            template.Add(e);// section);
            IQueryable<TenderBookletSection> returnValue = template.AsQueryable();
            SingleResult<TenderBookletSection> oSingleResult = null;
            oSingleResult = SingleResult.Create(returnValue);
            return oSingleResult;
        }

    }
}
