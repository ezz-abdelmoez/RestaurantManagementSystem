using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;

namespace SalesManagement.files.classes
{
    public class Printer1 : MyPrinter
    {
        
        public Printer1(int x, int y, int z, bool activex, int orderTypeNo1)
            :base(x, y, z, activex)
        {

        }

        public override void Export(LocalReport report)
        {
            CommonMethods.query = (itemCount * 0.25 + 3.5).ToString();
            if (repNum == 1)
                CommonMethods.query = (itemCount * .25 + 2.5).ToString();
            else if (repNum == 2)
                CommonMethods.query = (itemCount * .25 + 3.5).ToString();
            else if (repNum == 3)
                CommonMethods.query = (itemCount * .25 + 2.5).ToString();
            string devInfo = "<DeviceInfo> <OutputFormat>EMF</OutputFormat>  <PageWidth>2.84467in</PageWidth> " +
                "<PageHeight>" + CommonMethods.query + "in</PageHeight> <MarginTop>0in</MarginTop> " +
                "<MarginLeft>.01in</MarginLeft>  <MarginRight>.01in</MarginRight>  " +
                "<MarginBottom>.01in</MarginBottom>  </DeviceInfo>";
            mStreams = (IList<Stream>)new List<Stream>();
            report.Render("Image", devInfo, new CreateStreamCallback(CreateStream), out Warning[] _);
            foreach (Stream s in mStreams)
                s.Position = 0L;
        }

        public override void PrintPage(object sender, PrintPageEventArgs ev)
        {
            CommonMethods.billNo = 450;
            if (repNum == 1)
                CommonMethods.billNo = 350;
            else if (repNum == 2)
                CommonMethods.billNo = 450;
            else if (repNum == 3)
                CommonMethods.billNo = 350;
            Metafile metafile1 = new Metafile(mStreams[currentPageIndex]);
            Graphics graphics = ev.Graphics;
            Metafile metafile2 = metafile1;
            CommonMethods.x = ev.PageBounds.X;
            Rectangle pageBound = ev.PageBounds;
            CommonMethods.y = ev.PageBounds.Y;
            pageBound = ev.PageBounds;
            CommonMethods.width = ev.PageBounds.Width;
            CommonMethods.height = itemCount * 30 + CommonMethods.billNo;
            graphics.DrawImage((Image)metafile2, CommonMethods.x, CommonMethods.y, CommonMethods.width,
                CommonMethods.height);

            ++currentPageIndex;
            ev.HasMorePages = currentPageIndex < mStreams.Count;
        }

        public override void Run()
        {
            LocalReport report = new LocalReport();
            report.ReportPath = "Report01.rdlc";
            if (repNum == 1)
                report.ReportPath = "Report11.rdlc";
            else if (repNum == 2)
                report.ReportPath = "Report21.rdlc";
            else if (repNum == 3)
                report.ReportPath = "Report31.rdlc";
            report.DataSources.Add(new ReportDataSource("DataSet1", loadData()));
            Export(report);
            currentPageIndex = 0;
            Print();
        }
    }
}
