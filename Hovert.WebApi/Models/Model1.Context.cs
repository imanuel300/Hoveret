﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WEBAPIODATAV3.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DBBMEntities : DbContext
    {
        public DBBMEntities()
            : base("name=DBBMEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BankDoarIncome> BankDoarIncomes { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<Letter> Letters { get; set; }
        public virtual DbSet<TenderSection> TenderSections { get; set; }
        public virtual DbSet<TenderBookletSection> TenderBookletSections { get; set; }
        public virtual DbSet<TenderSectionType> TenderSectionTypes { get; set; }
        public virtual DbSet<TenderTemplatesBookletSection> TenderTemplatesBookletSections { get; set; }
        public virtual DbSet<MarketingMethodLookup> MarketingMethodLookups { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
    
        public virtual ObjectResult<spTenderBooklettConditions_Result> spTenderBooklettConditions(Nullable<int> tenderId)
        {
            var tenderIdParameter = tenderId.HasValue ?
                new ObjectParameter("TenderId", tenderId) :
                new ObjectParameter("TenderId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spTenderBooklettConditions_Result>("spTenderBooklettConditions", tenderIdParameter);
        }
    
        public virtual ObjectResult<spLett_TenderBooklet_Result> spLett_TenderBooklet(Nullable<int> tenderId)
        {
            var tenderIdParameter = tenderId.HasValue ?
                new ObjectParameter("TenderId", tenderId) :
                new ObjectParameter("TenderId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spLett_TenderBooklet_Result>("spLett_TenderBooklet", tenderIdParameter);
        }
    
        public virtual ObjectResult<Lookup_Proc_Result> Lookup_Proc(string lookup_Table)
        {
            var lookup_TableParameter = lookup_Table != null ?
                new ObjectParameter("Lookup_Table", lookup_Table) :
                new ObjectParameter("Lookup_Table", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Lookup_Proc_Result>("Lookup_Proc", lookup_TableParameter);
        }
    
        public virtual ObjectResult<spLett_TenderBookMarks_Result> spLett_TenderBookMarks(Nullable<int> tenderId, Nullable<int> tenderYear)
        {
            var tenderIdParameter = tenderId.HasValue ?
                new ObjectParameter("TenderId", tenderId) :
                new ObjectParameter("TenderId", typeof(int));
    
            var tenderYearParameter = tenderYear.HasValue ?
                new ObjectParameter("TenderYear", tenderYear) :
                new ObjectParameter("TenderYear", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spLett_TenderBookMarks_Result>("spLett_TenderBookMarks", tenderIdParameter, tenderYearParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> spGetTenderIdFromTenderYearAndNumber(Nullable<int> tenderId, Nullable<int> tenderYear)
        {
            var tenderIdParameter = tenderId.HasValue ?
                new ObjectParameter("TenderId", tenderId) :
                new ObjectParameter("TenderId", typeof(int));
    
            var tenderYearParameter = tenderYear.HasValue ?
                new ObjectParameter("TenderYear", tenderYear) :
                new ObjectParameter("TenderYear", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("spGetTenderIdFromTenderYearAndNumber", tenderIdParameter, tenderYearParameter);
        }
    
        public virtual ObjectResult<spGetTenderYearAndNumberFromTenderId_Result1> spGetTenderYearAndNumberFromTenderId(Nullable<int> tenderId)
        {
            var tenderIdParameter = tenderId.HasValue ?
                new ObjectParameter("TenderId", tenderId) :
                new ObjectParameter("TenderId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spGetTenderYearAndNumberFromTenderId_Result1>("spGetTenderYearAndNumberFromTenderId", tenderIdParameter);
        }
    }
}
