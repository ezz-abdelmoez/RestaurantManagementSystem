using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace SalesManagement.files.classes
{
    public class MyPrinter
    {
        public string printerName;
        public int currentPageIndex;
        public IList<Stream> mStreams;
        public int itemCount = 0;
        public int repNum;
        public int tableNum;
        public int chareNum;
        public bool activ = false;
        //public DataSet1.presa preSalesTableAdapter;
        public PrintDocument printDocument;

        public MyPrinter(int x, int y, int z, bool activex)
        {
            repNum = x;
            tableNum = y;
            chareNum = z;
            activ = activex;
        }

        public DataTable loadData()
        {
            //preSalesTableAdapter = new PreSalesTableAdapter();
            //try
            //{
            //    preSalesTableAdapter.Connection.Close();
            //}
            //catch { }
            //preSalesTableAdapter.Connection = DB.con;
            //DataSet1.PreSalesDataTable preDataTable = new DataSet1.PreSalesDataTable();

            //preSalesTableAdapter.Fill(preDataTable, new int?(MainWindow.userId), activ, 
            //    new int?(chareNum), new int?(tableNum), new int?(repNum));

            //preSalesTableAdapter.Dispose();
            //itemCount = preDataTable.Rows.Count;
            //return (DataTable) preDataTable;
            return null;
        }

        public Stream CreateStream(
            string name, 
            string fileNameExtension, 
            Encoding encoding, 
            string mimeType, 
            bool willSeek
            )
        {
            Stream stream = new MemoryStream();
            mStreams.Add(stream);
            return stream;
        }

        public virtual void Export(LocalReport report)
        {
            CommonMethods.query = (itemCount * .25 + 3.5).ToString();
            if(repNum == 1)
                CommonMethods.query = (itemCount * .25 + 3.6).ToString();
            else if (repNum == 2)
                CommonMethods.query = (itemCount * .25 + 5.7).ToString();
            else if (repNum == 3)
                CommonMethods.query = (itemCount * .25 + 3).ToString();
            string devInfo = "<DeviceInfo> <OutputFormat>EMF</OutputFormat>  <PageWidth>8cm</PageWidth> " +
                "<PageHeight>" + CommonMethods.query + "in</PageHeight> <MarginTop>0in</MarginTop> " +
                "<MarginLeft>.01in</MarginLeft>  <MarginRight>.01in</MarginRight>  " +
                "<MarginBottom>.01in</MarginBottom>  </DeviceInfo>";
            mStreams = (IList<Stream>)new List<Stream>();
            report.Render("Image", devInfo, new CreateStreamCallback(CreateStream), out Warning[] _);
            foreach (Stream s in mStreams)
                s.Position = 0L;
        }


        public virtual void PrintPage(object sender, PrintPageEventArgs ev)
        {
            CommonMethods.billNo = 350;
            if(repNum == 1)
                CommonMethods.billNo = 300;
            else if (repNum == 2)
                CommonMethods.billNo = 450;
            else if (repNum == 3)
                CommonMethods.billNo = 300;
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


        public void Print()
        {
            if(printerName.Length ==0)
                printerName = new PrinterSettings().PrinterName;
            if (mStreams == null || mStreams.Count == 0)
                return;
            printDocument = new PrintDocument();
            printDocument.PrinterSettings.PrinterName = printerName;
            if (!printDocument.PrinterSettings.IsValid)
                MessageBox.Show("Can\'t find printer \""+ printerName + "\".", "Printer Error");
            else
            {
                printDocument.PrintPage += new PrintPageEventHandler(PrintPage);
                printDocument.Print();
            }
        }

        public virtual void Run()
        {
            LocalReport report = new LocalReport();
            report.ReportPath = "Report0.rdlc";
            if (repNum == 1)
                report.ReportPath = "Report1.rdlc";
            else if (repNum == 2)
                report.ReportPath = "Report2.rdlc";
            else if (repNum == 3)
                report.ReportPath = "Report3.rdlc";
            report.DataSources.Add(new ReportDataSource("DataSet1", loadData()));
            Export(report);
            currentPageIndex = 0;
            Print();
        }

        public void Dispose()
        {
            if (mStreams == null)
                return;
            foreach (Stream s in mStreams)
                s.Close();
            mStreams = null;
        }
    }
}





