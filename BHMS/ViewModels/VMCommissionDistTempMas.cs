using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BHMS.Models;

namespace BHMS.ViewModels
{
    public class VMCommissionDistTempMas
    {
        public int Id { get; set; }

        public int BuyerInfoId { get; set; }
        public string BuyerName { get; set; }

        public int ProdDepartmentId { get; set; }
        public string ProdDepartmentName { get; set; }

        public enumCalType CalcType { get; set; }

    }

}