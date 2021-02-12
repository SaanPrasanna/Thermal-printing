using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomJson
{
    public class CustomerInfo
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Phone { get; set; }
    }

    public class BillSummary
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class Header
    {
        public string MainName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string BillNo { get; set; }
        public string DateOfBill { get; set; }
        public object Table { get; set; }
        public string FssaiNo { get; set; }
        public string GSTNo { get; set; }
        public string CustomerRemarksLine1 { get; set; }
        public string CustomerRemarksLine2 { get; set; }
        public string CustomerRemarksLine3 { get; set; }
        public string OrderNote { get; set; }
        public string TimeOfBill { get; set; }
    }

    public class Item
    {
        public int No { get; set; }
        public string ItemAmt { get; set; }
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public string Rate { get; set; }
        public string CustRmks { get; set; }
    }

    public class Settings
    {
        public Settings()
        {
            PageSize = "Size5";
        }
        public string PrinterName { get; set; }
        public string PrinterType { get; set; }
        public int ItemLength { get; set; }
        public bool PrintLogo { get; set; }
        public string ThankYouNote { get; set; }
        public string ThankYouNote2 { get; set; }
        public string EIDRMK { get; set; }
        public string PrintType { get; set; }
        public string PageSize { get; set; }
    }

    public class RootObject
    {
        public string Total { get; set; }
        public string GrandTotal { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public List<BillSummary> BillSummary { get; set; }
        public Header Header { get; set; }
        public List<Item> Items { get; set; }
        public Settings Settings { get; set; }
    }
}
