using log4net;
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
using WEBAPIODATAV3.Utilities;

namespace WEBAPIODATAV3.Controllers
{
    /*
       builder.EntitySet<TenderBookletSection>("TenderBookletSections");
    */

    //  [EnableCors(origins: "http://localhost:52253", headers: "*", methods: "*")]
    public class TenderBookletSectionsController : ApiController
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        

        private DBBMEntities db = new DBBMEntities();

        // GET: odata/TenderBookletSections
        [HttpGet]
        public IQueryable<TenderBookletSection> GetTenderBookletSections()
        {
            //log = InitializeLog4Net();
            log.Info("TESTING SAVE TO SHARE POINT");
            TenderBookletSection tbs = null;
            try
            {
                tbs = new TenderBookletSection();
                tbs.Id = 2475;
                tbs.SectionBody = "TESTING SAVE TO SHARE POINT";
                tbs.SectionNumber = "4/50/2";
                tbs.TenderId = 0;
                tbs.TenderSectionId = 0;
                ExportToWord etw = new ExportToWord();
                etw.OutputToWordFromPOST(tbs);
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
                if (TenderBookletSectionExists(tbs.Id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }


            return db.TenderBookletSections;
        }

        // GET: odata/TenderBookletSections(5)
        [HttpGet]
        public SingleResult<TenderBookletSection> GetTenderBookletSection( int key)
        {
            // return SingleResult.Create(db.TenderBookletSections.Where(tenderBookletSection => tenderBookletSection.Id == key));


            SingleResult<TenderBookletSection> oTS = null;
            int iOffset = Convert.ToInt32(ConfigurationManager.AppSettings["SectionOffset"].ToString());
            IQueryable<TenderBookletSection> oIq = db.TenderBookletSections
                .Where(
                    tenderSection => tenderSection.TenderSectionId >= key
                    && tenderSection.TenderSectionId < key + iOffset
                 );// tenderSection => tenderSection.Id == key);
            TenderBookletSection section = null;
            if (oIq.Count() > 0)
            {
                section = InsertStyle(GetConcatenatedSections(oIq));
            }

            //section.SectionBody = section.SectionBody.Replace("<<marketingMethod>>", "  שיטת שיווק : מחיר למשתכן  ");
            section.SectionBody = section.SectionBody.GetBookmarkValue(key);


            var l = new List<TenderBookletSection>();
            l.Add(section);
            IQueryable<TenderBookletSection> ret = l.AsQueryable();
            oTS = SingleResult.Create(ret);
            return oTS;
        }

        private TenderBookletSection InsertStyle(TenderBookletSection tenderBookletSection)
        {




            return tenderBookletSection;
        }

        // PUT: odata/TenderBookletSections(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<TenderBookletSection> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TenderBookletSection tenderBookletSection = db.TenderBookletSections.Find(key);
            if (tenderBookletSection == null)
            {
                return NotFound();
            }

            patch.Put(tenderBookletSection);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TenderBookletSectionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(tenderBookletSection);
        }

        // POST: odata/TenderBookletSections
        [HttpPost]
        public IHttpActionResult Post(TenderBookletSection tenderBookletSection)
        {
            //log = InitializeLog4Net();
            log.Info("TenderBookletSection");
            string ret = "";
            if (!ModelState.IsValid)
            {
                log.Error("TenderBookletSection - BadRequest");
                return BadRequest(ModelState);
            }

            // db.TenderBookletSections.Add(tenderBookletSection);





            try
            {
                log.Info("pre OutputToWordFromPOST");
                //                ExportToWord etw = new ExportToWord();
                //                 etw.OutputToWordFromPOST(tenderBookletSection);
                ret = ExportAsposeMultilevel.OutputToWordFromPOST(tenderBookletSection);

               // ret = System.Security.Principal.WindowsIdentity.GetCurrent().Name ;
            }
            catch (Exception e)
            {
                if (TenderBookletSectionExists(tenderBookletSection.Id))
                {
                    log.Error("TenderBookletSection - Conflict" + e.Message);
                    return Conflict();
                }
                else
                {
                    log.Error("TenderBookletSection - throw" + e.Message);//The invoked member is not supported in a dynamic assembly
                    throw;
                }
            }
            tenderBookletSection.SectionBody = ret;
            return Ok(tenderBookletSection);  //   
        }

        // PATCH: odata/TenderBookletSections(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<TenderBookletSection> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TenderBookletSection tenderBookletSection = db.TenderBookletSections.Find(key);
            if (tenderBookletSection == null)
            {
                return NotFound();
            }

            patch.Patch(tenderBookletSection);

            try
            {
                // db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TenderBookletSectionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(tenderBookletSection);
        }

        // DELETE: odata/TenderBookletSections(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            TenderBookletSection tenderBookletSection = db.TenderBookletSections.Find(key);
            if (tenderBookletSection == null)
            {
                return NotFound();
            }

            //db.TenderBookletSections.Remove(tenderBookletSection);
            //db.SaveChanges();

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

        private bool TenderBookletSectionExists(int key)
        {
            return db.TenderBookletSections.Count(e => e.Id == key) > 0;
        }


        private TenderBookletSection GetConcatenatedSections(IQueryable<TenderBookletSection> oIq)
        {
            TenderBookletSection oTs = new TenderBookletSection();
            oIq = oIq.OrderBy<TenderBookletSection, int>(d => d.TenderSectionId ?? 0);
            foreach (TenderBookletSection ts in oIq)
            {
                if (IsDisplayed(ts))
                {
                    oTs.SectionBody += InsertHTMLStyle(ts);
                }
            }

            return oTs;
        }


        string sNewSectionBody = String.Empty;
        private string InsertHTMLStyle(TenderBookletSection section)
        {

            TenderSectionType[] oST = db.TenderSectionTypes.ToArray();
            switch (GetSectionType(section))
            {
                case SectionTypes.Father: sNewSectionBody = oST[0].SectionHTML.Replace("defaulttext", section.SectionBody); break;
                case SectionTypes.Son: sNewSectionBody = oST[1].SectionHTML.Replace("defaulttext", section.SectionBody); break;
                case SectionTypes.Grandson: sNewSectionBody = oST[2].SectionHTML.Replace("defaulttext", section.SectionBody); break;
                case SectionTypes.Grandgrandson: sNewSectionBody = oST[3].SectionHTML.Replace("defaulttext", section.SectionBody); break;
                default:
                    break;
            }


            return sNewSectionBody;
        }

        private SectionTypes GetSectionType(TenderBookletSection section)
        {
            return SectionTypes.Father;
        }

        private bool IsDisplayed(TenderBookletSection ts)
        {
            return true;
            //return ts.ConditionId ?? false;
        }



    }
}
