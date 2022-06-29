using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSystem
{
    class Inventory
    {
        public string ScanCode { get; set; }
        public string Descripation { get; set; }
        public string Department { get; set; }
        public string Closing { get; set; }
        public string Value { get; set; }
    }
    class UserWSaleReport
    {
        public string CreateBy { get; set; }
        public string GrossAmount { get; set; }
        public string TaxAmount { get; set; }
        public string Receive { get; set; }
        public string Cash { get; set; }
        public string Chec { get; set; }
        public string Card { get; set; }
        public string Loan { get; set; }
        public string Exp { get; set; }
    }
    class clDayClose
    {
        public string Description { get; set; }
        public string Amount { get; set; }
        public string Type { get; set; }
    }
    class TransDetails
    {
        public string Tran_id { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandAmount { get; set; }
        public string CreateBy { get; set; }
        public string ScanCode { get; set; }
        public string descripation { get; set; }
        public decimal quantity { get; set; }
        public string price { get; set; }
        public decimal amount { get; set; }
        public string TenderCode { get; set; }
        public decimal TenderAmount { get; set; }
    }
}
