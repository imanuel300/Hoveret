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
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using WEBAPIODATAV3.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<TenderTemplatesBookletSection>("TenderTemplatesBookletSections");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
   // [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TenderTemplatesBookletSectionsController : ApiController
    {
        private DBBMEntities db = new DBBMEntities();

        // GET: odata/TenderTemplatesBookletSections
        [HttpGet]
        public IQueryable<TenderTemplatesBookletSection> GetTenderTemplatesBookletSections()
        {
            return db.TenderTemplatesBookletSections;
        }

        // GET: odata/TenderTemplatesBookletSections(5)
        [HttpGet]
        public IList<TenderTemplatesBookletSection> GetTenderTemplatesBookletSection(int key)//,int year
        {
            if (key == 0)
            {
                IQueryable<TenderTemplatesBookletSection> oIq = db.TenderTemplatesBookletSections;
                TenderTemplatesBookletSection section = null;
                if (oIq.Count() > 0)
                {
                    section = InsertStyle(GetConcatenatedSections(oIq));
                }

                List<TenderTemplatesBookletSection> template = new List<TenderTemplatesBookletSection>();
                template.Add(section);// section);
                                      //IQueryable<TenderTemplatesBookletSection> returnValue = template.AsQueryable();
                                      //SingleResult<TenderTemplatesBookletSection> oSingleResult = null;
                                      //oSingleResult = SingleResult.Create(returnValue);
                                      // return oSingleResult;
                return template;
            }

           
            SingleResult<TenderTemplatesBookletSection> oTS = null;
            int iOffset = Convert.ToInt32(ConfigurationManager.AppSettings["SectionOffset"].ToString());         


            var list = db.spTenderBooklettConditions(key).ToList<spTenderBooklettConditions_Result>();

            list = list.OrderBy<spTenderBooklettConditions_Result, int>(d => d.TenderSectionId ?? 0).ToList<spTenderBooklettConditions_Result>();
            TenderTemplatesBookletSection oRet = new TenderTemplatesBookletSection();
            if (list != null && list.Count() > 0)
            {
                
                int level = -1;
                foreach (var s in list)
                {
                    var ended = "";
                    if (s.MULTILEVEL == null) s.MULTILEVEL = 0;
                    if (s.MULTILEVEL > 0)
                    {
                        if (s.MULTILEVEL > 0 && level == -1)//פעם ראשונה
                        {
                            oRet.SectionBody += "<ol><li>";
                        }
                        if (level > s.MULTILEVEL && s.MULTILEVEL == 0)//התחלה חדשה לאחר הפסקה
                        {

                        }
                        if (level < s.MULTILEVEL && s.MULTILEVEL > 0)
                        {//עליה בשלב
                            oRet.SectionBody += "<ol><li>";
                            //ended = "</li>";
                        }
                        if (level == s.MULTILEVEL && s.MULTILEVEL > 0)
                        {//נשאר באותו שלב
                            oRet.SectionBody += "</li><li>";
                            //ended = "</li>";
                        }
                        if (level > s.MULTILEVEL)
                        {//ירידה בשלב
                            for (int i = 0; i < level - s.MULTILEVEL; i++)
                            {
                                oRet.SectionBody += "</li></ol>";
                            }
                            //ended = "</li>";
                        }
                    }
                    
                    level = (int)s.MULTILEVEL;
                    oRet.SectionBody += s.SectionBody;// InsertHTMLStyle(ts);
                    oRet.SectionBody += ended;

                    //if (list.IndexOf(s) == list.Count - 1)//אחרון
                    //{
                    //    oRet.SectionBody += "</li></ol>";
                    //}
                }
            }

            //section.SectionBody = section.SectionBody.Replace("<<marketingMethod>>", "  שיטת שיווק : מחיר למשתכן  ");
            oRet.SectionBody = oRet.SectionBody.GetBookmarkValue(key);
            //oRet.SectionBody = ReplaceBookmark(oRet.SectionBody);

            var l = new List<TenderTemplatesBookletSection>();
            l.Add(oRet);
            //IQueryable<TenderTemplatesBookletSection> ret = l.AsQueryable();
            // oTS = SingleResult.Create(ret);
            // return oTS;
            return l;
        }

        private static string ReplaceBookmark(string text)
        {
            text.Replace("<<marketingMethod>>", "  שיטת שיווק : מחיר למשתכן  ");
            return text;
        }

        // PUT: odata/TenderTemplatesBookletSections(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<TenderTemplatesBookletSection> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TenderTemplatesBookletSection tenderTemplatesBookletSection = db.TenderTemplatesBookletSections.Find(key);
            if (tenderTemplatesBookletSection == null)
            {
                return NotFound();
            }

            patch.Put(tenderTemplatesBookletSection);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TenderTemplatesBookletSectionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(tenderTemplatesBookletSection);
        }

        // POST: odata/TenderTemplatesBookletSections
        public IHttpActionResult Post(TenderTemplatesBookletSection tenderTemplatesBookletSection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TenderTemplatesBookletSections.Add(tenderTemplatesBookletSection);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TenderTemplatesBookletSectionExists(tenderTemplatesBookletSection.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(tenderTemplatesBookletSection);
        }

        // PATCH: odata/TenderTemplatesBookletSections(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<TenderTemplatesBookletSection> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TenderTemplatesBookletSection tenderTemplatesBookletSection = db.TenderTemplatesBookletSections.Find(key);
            if (tenderTemplatesBookletSection == null)
            {
                return NotFound();
            }

            patch.Patch(tenderTemplatesBookletSection);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TenderTemplatesBookletSectionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(tenderTemplatesBookletSection);
        }

        // DELETE: odata/TenderTemplatesBookletSections(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            TenderTemplatesBookletSection tenderTemplatesBookletSection = db.TenderTemplatesBookletSections.Find(key);
            if (tenderTemplatesBookletSection == null)
            {
                return NotFound();
            }

            db.TenderTemplatesBookletSections.Remove(tenderTemplatesBookletSection);
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


        private bool TenderBookletSectionExists(int key)
        {
            return db.TenderBookletSections.Count(e => e.Id == key) > 0;
        }


        private TenderTemplatesBookletSection GetConcatenatedSections(IQueryable<TenderTemplatesBookletSection> oIq)
        {
            TenderTemplatesBookletSection oTs = new TenderTemplatesBookletSection();
            oIq = oIq.OrderBy<TenderTemplatesBookletSection, int>(d => d.TenderSectionId ?? 0);
            foreach (TenderTemplatesBookletSection ts in oIq)
            {
                if (IsDisplayed(ts))
                {
                    oTs.SectionBody += ts.SectionBody;// InsertHTMLStyle(ts);
                }
            }

            return oTs;
        }


        string sNewSectionBody = String.Empty;
        private string InsertHTMLStyle(TenderTemplatesBookletSection section)
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
        private TenderTemplatesBookletSection InsertStyle(TenderTemplatesBookletSection tenderBookletSection)
        {
            return tenderBookletSection;
        }
        private SectionTypes GetSectionType(TenderTemplatesBookletSection section)
        {
            return SectionTypes.Father;
        }

        private bool IsDisplayed(TenderTemplatesBookletSection ts)
        {
            return true;
            //return ts.ConditionId ?? false;
        }

        private bool TenderTemplatesBookletSectionExists(int key)
        {
            return db.TenderTemplatesBookletSections.Count(e => e.Id == key) > 0;
        }
    }
}
