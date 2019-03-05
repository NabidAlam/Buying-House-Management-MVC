namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CommissionDistTempMas
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public CommissionDistTempMas()
        //{
        //    CommissionDistTempDet = new HashSet<CommissionDistTempDet>();
        //}

        public int Id { get; set; }

        public int BuyerInfoId { get; set; }

        public int ProdDepartmentId { get; set; }

        public enumCalType CalcType { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        public virtual BuyerInfo BuyerInfo { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CommissionDistTempDet> CommissionDistTempDet { get; set; }

        public virtual ProdDepartment ProdDepartment { get; set; }
    }

    public enum enumCalType
    {
        Total_Commission, Total_Export_Value
    }
}
