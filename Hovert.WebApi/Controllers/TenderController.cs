using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WEBAPIODATAV3.Models;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using WEBAPIODATAV3.Utilities;

namespace WEBAPIODATAV3.Controllers
{
    public class TenderController : ApiController
    {
        private DBBMEntities db = new DBBMEntities();
        // GET: api/Tender
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }



        // GET: api/Tender/5
        public string Get(int id)
        {
            string ret = "";
            ret = Utilities.UtilityMethods.GetFilenameBookmarks(id);
            if (id == 0)
            {

                return "לא נשלח מספר מכרז";
            }

            TenderBookletSection tenderBookletSection = new TenderBookletSection();

            //int iOffset = Convert.ToInt32(ConfigurationManager.AppSettings["SectionOffset"].ToString());


            //var list = db.spTenderBooklettConditions(id).ToList<spTenderBooklettConditions_Result>();

            //list = list.OrderBy<spTenderBooklettConditions_Result, int>(d => d.TenderSectionId ?? 0).ToList<spTenderBooklettConditions_Result>();

            //if (list != null && list.Count() > 0)
            //{
            //    foreach (var s in list)
            //    {
            //        tenderBookletSection.SectionBody += s.SectionBody;// InsertHTMLStyle(ts);
            //    }
            //}



            //tenderBookletSection.SectionBody = tenderBookletSection.SectionBody.GetBookmarkValue(id);

            tenderBookletSection.SectionNumber = ret;
            tenderBookletSection.Id = id;

            try
            {
                //ExportToWord etw = new ExportToWord();
                //ret = etw.OutputToWordFromPOST(tenderBookletSection);

                ret = ExportAsposeMultilevel.OutputToWordFromPOST(tenderBookletSection);
            }
            catch (Exception ex)
            {
                throw ex;
            }



            return ret;
        }

        // GET: api/Tender/5
        public string ancientfonctionavantdeavoirutilisemultilevel(int id)
        {
            string ret = "";
            ret = Utilities.UtilityMethods.GetFilenameBookmarks(id);
            if (id == 0)
            {
                IQueryable<TenderTemplatesBookletSection> oIq = db.TenderTemplatesBookletSections;
                TenderTemplatesBookletSection section = null;
                if (oIq.Count() > 0)
                {
                    section = InsertStyle(GetConcatenatedSections(oIq));
                }

                List<TenderTemplatesBookletSection> template = new List<TenderTemplatesBookletSection>();
                template.Add(section);// section);
                IQueryable<TenderTemplatesBookletSection> returnValue = template.AsQueryable();
                SingleResult<TenderTemplatesBookletSection> oSingleResult = null;
                oSingleResult = SingleResult.Create(returnValue);
                return "";
            }


            //SingleResult<TenderTemplatesBookletSection> oTS = null;
            int iOffset = Convert.ToInt32(ConfigurationManager.AppSettings["SectionOffset"].ToString());


            var list = db.spTenderBooklettConditions(id).ToList<spTenderBooklettConditions_Result>();

            list = list.OrderBy<spTenderBooklettConditions_Result, int>(d => d.TenderSectionId ?? 0).ToList<spTenderBooklettConditions_Result>();
            TenderBookletSection tenderBookletSection = new TenderBookletSection();
            if (list != null && list.Count() > 0)
            {
                foreach (var s in list)
                {
                    tenderBookletSection.SectionBody += s.SectionBody;// InsertHTMLStyle(ts);
                }
            }

            //section.SectionBody = section.SectionBody.Replace("<<marketingMethod>>", "  שיטת שיווק : מחיר למשתכן  ");
            tenderBookletSection.SectionBody = tenderBookletSection.SectionBody.GetBookmarkValue(id);
            //oRet.SectionBody = ReplaceBookmark(oRet.SectionBody);
            tenderBookletSection.SectionNumber = ret;
            tenderBookletSection.Id = id;

            try
            {
                ExportToWord etw = new ExportToWord();
                ret = etw.OutputToWordFromPOST(tenderBookletSection);
            }
            catch (Exception ex)
            {
                throw ex;
            }



            return ret;
        }


        private TenderTemplatesBookletSection InsertStyle(TenderTemplatesBookletSection tenderBookletSection)
        {
            return tenderBookletSection;
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

        private bool IsDisplayed(TenderTemplatesBookletSection ts)
        {
            return true;
            //return ts.ConditionId ?? false;
        }

        // POST: api/Tender
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Tender/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Tender/5
        public void Delete(int id)
        {
        }


    }
}
