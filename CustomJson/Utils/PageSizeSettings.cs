using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomJson
{
    public class PageSizeSettings
    {
        public PageSizeSettings()
        {
            PageSize = "Size5";
            PageSetting = new PageSettings();
        }
        public string PageSize { get; set; }
        public PageSettings PageSetting { get; set; }
    }
    public class PageSettings
    {
        public PageSettings()
        {
            CashItemFontName = "Segoe UI";
            HeaderCashFontName = "Segoe UI";
            HeaderKitchenFontName = "Segoe UI";
            KitchenItemFontName = "Segoe UI";
            SubHeaderFontName = "Segoe UI";
            CustomerRemarksText = "Remarks: ";
            KitchenCustomerRmkLength = 40;
            ItemPoint = 0;
            QuantityPoint = 160;
            PricePoint = 175;
            AmountPoint = 200;
            ItemWidth = 275;
            RecWidth = 315;
            SummaryPoint = 250;
            TotalPoint = 200;
            WidthSummary = 90;
            KitchenRecWidth = 160;
            KitchenQuantityPoint = 100;
            KitchenItemLength = 26;
            ItemLength = 26;
            HeaderCashFontSize = (float)12;
            CashItemFontSize = (float)8.5;
            KitchenItemFontSize = (float)8.5;
            HeaderKitchenFontSize = 12;
            BillNoRowFontSize = 10;
            SubHeaderFontSize = 12;
            XPoint = 5;
            Offset = 5;
            YPoint = 0;
            LogoWidth = 315;
            LogoHeight = 100;
            LogoXPoint = 0;
        }

        public string CashItemFontName { get; set; }
        public string HeaderCashFontName { get; set; }
        public string HeaderKitchenFontName { get; set; }
        public string KitchenItemFontName { get; set; }
        public string SubHeaderFontName { get; set; }
        public string CustomerRemarksText { get; set; }
        public int KitchenCustomerRmkLength { get; set; }
        public int ItemPoint { get; set; }
        public int QuantityPoint { get; set; }
        public int PricePoint { get; set; }
        public int AmountPoint { get; set; }
        public int ItemWidth { get; set; }
        public int RecWidth { get; set; }
        public int SummaryPoint { get; set; }
        public int TotalPoint { get; set; }
        public int WidthSummary { get; set; }
        public int KitchenRecWidth { get; set; }
        public int KitchenQuantityPoint { get; set; }
        public int KitchenItemLength { get; set; }
        public int ItemLength { get; set; }
        public float HeaderCashFontSize { get; set; }
        public float CashItemFontSize { get; set; }
        public float KitchenItemFontSize { get; set; }
        public float HeaderKitchenFontSize { get; set; }
        public float BillNoRowFontSize { get; set; }
        public float SubHeaderFontSize { get; set; }
        public int XPoint { get; set; }
        public int Offset { get; set; }
        public int YPoint { get; set; }
        public int LogoWidth { get; set; }
        public int LogoHeight { get; set; }
        public float LogoXPoint { get; set; }
    }
}
