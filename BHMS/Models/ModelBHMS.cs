namespace BHMS.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelBHMS : DbContext
    {
        public ModelBHMS()
            : base("name=ModelBHMS")
        {
        }

        public virtual DbSet<BankBranch> BankBranch { get; set; }
        public virtual DbSet<BuyerInfo> BuyerInfo { get; set; }
        public virtual DbSet<BuyerOrderDet> BuyerOrderDet { get; set; }
        public virtual DbSet<BuyerOrderMas> BuyerOrderMas { get; set; }
        public virtual DbSet<CommissionDistTempDet> CommissionDistTempDet { get; set; }
        public virtual DbSet<CommissionDistTempMas> CommissionDistTempMas { get; set; }
        public virtual DbSet<CommissionDistDet> CommissionDistDet { get; set; }
        public virtual DbSet<CommissionDistMas> CommissionDistMas { get; set; }
        public virtual DbSet<CompanyInformation> CompanyInformations { get; set; }
        public virtual DbSet<CompanyResource> CompanyResources { get; set; }
        public virtual DbSet<CountryInfo> CountryInfo { get; set; }

        public virtual DbSet<CourierInfo> CourierInfo { get; set; }

        public virtual DbSet<DestinationPort> DestinationPort { get; set; }

        public virtual DbSet<DocSubmissionDet> DocSubmissionDet { get; set; }
        public virtual DbSet<DocSubmissionFactDet> DocSubmissionFactDet { get; set; }
        public virtual DbSet<DocSubmissionMas> DocSubmissionMas { get; set; }
        public virtual DbSet<ExFactoryDet> ExFactoryDet { get; set; }
        public virtual DbSet<ExFactoryMas> ExFactoryMas { get; set; }
        public virtual DbSet<ExFactoryShipDet> ExFactoryShipDet { get; set; }

        public virtual DbSet<FabricItem> FabricItem { get; set; }
        public virtual DbSet<FactoryOrderDet> FactoryOrderDet { get; set; }
        public virtual DbSet<FactoryOrderMas> FactoryOrderMas { get; set; }

        public virtual DbSet<InvoiceCommDet> InvoiceCommDet { get; set; }
        public virtual DbSet<InvoiceCommFactDet> InvoiceCommFactDet { get; set; }
        public virtual DbSet<InvoiceCommFactMas> InvoiceCommFactMas { get; set; }
        public virtual DbSet<InvoiceCommMas> InvoiceCommMas { get; set; }

        public virtual DbSet<LCAmendInfo> LCAmendInfo { get; set; }
        public virtual DbSet<LCTransferDet> LCTransferDet { get; set; }
        public virtual DbSet<LCTransferMas> LCTransferMas { get; set; }
        public virtual DbSet<MasterLCInfoDet> MasterLCInfoDet { get; set; }
        public virtual DbSet<MasterLCInfoMas> MasterLCInfoMas { get; set; }
        public virtual DbSet<MasterLCInfoOrderDet> MasterLCInfoOrderDet { get; set; }
        public virtual DbSet<MiddleParty> MiddleParty { get; set; }
        public virtual DbSet<ProdCategory> ProdCategory { get; set; }
        public virtual DbSet<ProdCatType> ProdCatType { get; set; }
        public virtual DbSet<ProdColor> ProdColor { get; set; }
        public virtual DbSet<ProdDepartment> ProdDepartment { get; set; }
        public virtual DbSet<ProdSize> ProdSize { get; set; }
        public virtual DbSet<SeasonInfo> SeasonInfo { get; set; }
        public virtual DbSet<ShipmentSummDet> ShipmentSummDet { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<ProceedRealizationMas> ProceedRealizationMas { get; set; }
        public virtual DbSet<ProceedRealizationDet> ProceedRealizationDet { get; set; }

        public virtual DbSet<AIT> AIT { get; set; }
        public virtual DbSet<BankCharge> BankCharge { get; set; }
        public virtual DbSet<CommissionRealization> CommissionRealization { get; set; }



        public virtual DbSet<FDDPayment> FDDPayment { get; set; }
        public virtual DbSet<TTPayment> TTPayment { get; set; }

        public virtual DbSet<Brand> Brand { get; set; }

        public virtual DbSet<FabricSupplier> FabricSupplier { get; set; }
        public virtual DbSet<FabricType> FabricType { get; set; }

        //Added by Tazbirul(8/8/2018)
        public virtual DbSet<FactoryOrderDelivDet> FactoryOrderDelivDet { get; set; }

        //Added by Nabid 16.8.18
        public virtual DbSet<DiscountMas> DiscountMas { get; set; }
        public virtual DbSet<DiscountDet> DiscountDet { get; set; }
        public virtual DbSet<DiscountAdjustmentFactoryMas> DiscountAdjustmentFactoryMas { get; set; }
        public virtual DbSet<DiscountAdjustmentFactoryDet> DiscountAdjustmentFactoryDet { get; set; }
        public virtual DbSet<DiscountAdjustmentFactoryPrev> DiscountAdjustmentFactoryPrev { get; set; }
        public virtual DbSet<DiscountAdjustmentFactoryAdj> DiscountAdjustmentFactoryAdj { get; set; }


        // Added by anis 27/08/2018

        public virtual DbSet<DiscountAdjustmentBuyerMas> DiscountAdjustmentBuyerMas { get; set; }
        public virtual DbSet<DiscountAdjustmentBuyerPrev> DiscountAdjustmentBuyerPrev { get; set; }
        public virtual DbSet<DiscountAdjustmentBuyerAdj> DiscountAdjustmentBuyerAdj { get; set; }
        public virtual DbSet<DiscountAdjustmentBuyerDet> DiscountAdjustmentBuyerDet { get; set; }


        //Added by Anis(14/8/2018)
        public virtual DbSet<FactoryCashAdjustment> FactoryCashAdjustment { get; set; }
        public virtual DbSet<BuyerCashAdjustment> BuyerCashAdjustment { get; set; }


        //Added by Tazbirul(26/9/2018)
        public virtual DbSet<TimeActionMas> TimeActionMas { get; set; }
        public virtual DbSet<TimeActionDet> TimeActionDet { get; set; }
        public virtual DbSet<ActionActivityMas> ActionActivityMas { get; set; }
        public virtual DbSet<ActionActivityDet> ActionActivityDet { get; set; }
        public virtual DbSet<FDDPaymentDet> FDDPaymentDet { get; set; }

        //Added by Tazbirul(16/10/2018)
        public virtual DbSet<InvoiceCommDetDet> InvoiceCommDetDet { get; set; }

        public virtual DbSet<FabricItemDet> FabricItemDet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<BuyerInfo>()
            //    .HasMany(e => e.ProdDepartment)
            //    .WithRequired(e => e.BuyerInfo)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<BuyerOrderDet>()
                .Property(e => e.UnitPrice)
                .HasPrecision(12, 2);

            //modelBuilder.Entity<BuyerOrderDet>()
            //    .HasMany(e => e.FactoryOrderDet)
            //    .WithRequired(e => e.BuyerOrderDet)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<BuyerOrderDet>()
            //    .HasMany(e => e.ShipmentSummDet)
            //    .WithRequired(e => e.BuyerOrderDet)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<BuyerOrderMas>()
            //    .HasMany(e => e.BuyerOrderDet)
            //    .WithRequired(e => e.BuyerOrderMas)
            //    .HasForeignKey(e => e.BuyerOrderMasId)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<BuyerOrderMas>()
            //    .HasMany(e => e.CommissionDistMas)
            //    .WithRequired(e => e.BuyerOrderMas)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<BuyerOrderMas>()
            //    .HasMany(e => e.FactoryOrderMas)
            //    .WithRequired(e => e.BuyerOrderMas)
            //    .HasForeignKey(e => e.BuyerOrderMasId)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<CommissionDistTempMas>()
            //    .HasMany(e => e.CommissionDistTempDet)
            //    .WithRequired(e => e.CommissionDistTempMas)
            //    .HasForeignKey(e => e.CommissionDistTempMasId)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<CommissionDistDet>()
                .Property(e => e.OverseasCommValue)
                .HasPrecision(12, 2);

            modelBuilder.Entity<CommissionDistDet>()
                .Property(e => e.OthersCommValue)
                .HasPrecision(12, 2);

            modelBuilder.Entity<CommissionDistDet>()
                .Property(e => e.CompCommValue)
                .HasPrecision(12, 2);

            //modelBuilder.Entity<CommissionDistMas>()
            //    .HasMany(e => e.CommissionDistDet)
            //    .WithRequired(e => e.CommissionDistMas)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<CompanyResource>()
            //    .HasMany(e => e.BuyerInfo)
            //    .WithOptional(e => e.CompanyResource)
            //    .HasForeignKey(e => e.MerchandiserId);

            modelBuilder.Entity<DocSubmissionMas>()
                .Property(e => e.FDBCValue)
                .HasPrecision(12, 2);

            //modelBuilder.Entity<FactoryOrderMas>()
            //    .HasMany(e => e.FactoryOrderDet)
            //    .WithRequired(e => e.FactoryOrderMas)
            //    .HasForeignKey(e => e.FactoryOrderMasId)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactoryOrderDet>()
                .Property(e => e.FOBUnitPrice)
                .HasPrecision(12, 2);

            //modelBuilder.Entity<FactoryOrderDet>()
            //    .HasMany(e => e.CommissionDistDet)
            //    .WithRequired(e => e.FactoryOrderDet)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<LCAmendInfo>()
            //    .Property(e => e.AmendLCNo)
            //    .IsUnicode(false);

            modelBuilder.Entity<LCAmendInfo>()
                .Property(e => e.AmendTotalValue)
                .HasPrecision(12, 2);

            modelBuilder.Entity<MasterLCInfoMas>()
                .Property(e => e.TotalValue)
                .HasPrecision(12, 2);

            //modelBuilder.Entity<ProdCategory>()
            //    .HasMany(e => e.ProdCatType)
            //    .WithRequired(e => e.ProdCategory)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<ProdCatType>()
            //    .HasMany(e => e.ProdSize)
            //    .WithRequired(e => e.ProdCatType)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Supplier>()
            //    .HasMany(e => e.BuyerOrderMas)
            //    .WithOptional(e => e.Supplier)
            //    .HasForeignKey(e => e.FabSupplierId);

            //modelBuilder.Entity<Supplier>()
            //    .HasMany(e => e.FactoryOrderMas)
            //    .WithRequired(e => e.Supplier)
            //    .WillCascadeOnDelete(false);
        }

        public System.Data.Entity.DbSet<BHMS.Models.UserDept> UserDepts { get; set; }
    }
}
