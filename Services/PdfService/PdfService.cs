using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Transaction = CBA.Models.Transaction;
using Path = System.IO.Path;
using CBA.Models;
using CBA.Context;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using iText.Signatures;


namespace CBA.Services;
public class PdfServiceFactory : IPdfService
{
    private readonly ILogger<PdfServiceFactory> _logger;
    private readonly UserDataContext _context;
    public PdfServiceFactory(ILogger<PdfServiceFactory> logger, UserDataContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<string>CreateAccountStatementPdfAsync(List<Transaction> transactions, string customerId, string filePath, string startDate, string endDate)
    {
        try
        {
            _logger.LogInformation("Creating PDF filessss");

           /* string fileFolder = "C:/Users/Adesoji/Desktop/.NET Projects/CBA/Assets/";
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
            }
            string fileName = "AccountStatement" + DateTime.Now.ToString("ddMMMMyyyyHHmmssfffff") + ".pdf";
            string filePath = Path.Combine(fileFolder, fileName);
            _logger.LogInformation("File path: {FilePath}", filePath);
            _logger.LogInformation("File name: {FileName}", fileName);
            _logger.LogInformation("File folder: {FileFolder}", fileFolder);*/

            var customer = await _context.CustomerEntity.SingleAsync(x => x.Id.ToString() == customerId);
            _logger.LogInformation($"Customer found {customer.FullName}");  
            
            var pdfWriter = new PdfWriter(filePath);
            _logger.LogInformation("PDF writer created");
            var pdf = new PdfDocument(pdfWriter);
            _logger.LogInformation("PDF document created");
            var document = new Document(pdf, PageSize.A4);
            _logger.LogInformation("PDF document created");
            DeviceRgb offPurpleColour = new(230, 230, 250);
            _logger.LogInformation("PDF document created");
            DeviceRgb PurpleColour = new(128, 0, 128);
            _logger.LogInformation("PDF document created");
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA, PdfEncodings.CP1252, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            _logger.LogInformation("PDF document created");
            SetFontAndMargins(document, font);
            SetHeadersFootersUsingPDFEventHandlers(pdf);

            var addressTable = CreateAddressTable(font, customer);
            var summaryTable = CreateSummaryTable(PurpleColour, font, customer, startDate, endDate, transactions);
            MergeAddressAndSummaryTables(document, font, addressTable, summaryTable);

            var headerTable = CreateHeaderTable(PurpleColour, font);
            var noticeBoard = CreateNoticeBoard(PurpleColour, font);
            MergeHeaderAndNoticeBoardTables(document, font, headerTable, noticeBoard);
            TransactionTable(document, PurpleColour, offPurpleColour, font, transactions);
            Miscelleanous(document, PurpleColour, font);
            _logger.LogInformation("PDF file created successfully");
            var currentEvent = new PDFFooterEventHandler();
            _logger.LogInformation("Writing page numbers to PDF");
            
            currentEvent.WritePageNumbers(pdf, document);
            document.Close();
            _logger.LogInformation("PDF file created successfully");

            return "PDF file created successfully";

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating PDF file");
            throw new Exception(ex.Message);
        }
    }
    private void SetHeadersFootersUsingPDFEventHandlers(PdfDocument pdf)
    {
        pdf.AddEventHandler(PdfDocumentEvent.START_PAGE, new PDFHeaderEventHandler());
        pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new PDFFooterEventHandler());
        _logger.LogInformation("Header and Footer added to PDF");

    }
    private void SetFontAndMargins(Document document, PdfFont font)
    {
        document.SetMargins(50, 50, 50, 50);
        document.SetFont(font);
        _logger.LogInformation("Font and Margins set");
    }
    private Table CreateAddressTable(PdfFont font, CustomerEntity customer)
    {
        Table addressTable = new Table(new float[] { 345F }).SetFontSize(10F).SetFontColor(ColorConstants.BLACK).SetFont(font).SetBorder(Border.NO_BORDER)
            .SetMarginTop(150F).SetMarginLeft(10f);

        addressTable.AddCell(new Cell()
            .Add(new Paragraph(customer.FullName))
            .Add(new Paragraph(customer.Address))
            .Add(new Paragraph(customer.State))
            .Add(new Paragraph(customer.PhoneNumber)).SetBorder(Border.NO_BORDER));
        _logger.LogInformation("Address Table created");
        return addressTable;
    }
    private Table CreateSummaryTable(DeviceRgb purpleColour, PdfFont font, CustomerEntity customer,
    string startDate, string endDate, List<Transaction> transactions)
    {
        _logger.LogInformation("Creating Summary Table");
        Table summaryTable = new Table(new float[] { 130F }).SetFontSize(8F).SetFontColor(ColorConstants.BLACK).SetFont(font).SetBorder(Border.NO_BORDER);
        summaryTable.AddCell(new Cell().Add(new Paragraph(customer.Branch)).SetBorder(Border.NO_BORDER).SetFontColor(purpleColour).SetFontSize(14F));
        summaryTable.AddCell(new Cell().Add(new Paragraph($"{startDate}   {endDate}")).SetBorder(Border.NO_BORDER));
        summaryTable.AddCell(new Cell().Add(new Paragraph(customer.FullName)).SetBorder(Border.NO_BORDER));

        List summaryBullets = new();
        summaryBullets.Add(new ListItem("Sort Code 00-01-02"));
        summaryBullets.Add(new ListItem(customer.AccountNumber));
        summaryBullets.Add(new ListItem("SWIFTBIC CNGDVV11"));
        summaryBullets.Add(new ListItem("IBAN CFDSA 111 2344 2233"));
        summaryTable.AddCell(new Cell().Add(summaryBullets).SetBorder(Border.NO_BORDER).SetPaddingBottom(10f));

        summaryTable.AddCell(new Cell().Add(new Table(new float[] { 65f, 65f }).AddCell(new Cell().Add(new Paragraph("Start balance")).SetTextAlignment(TextAlignment.LEFT).SetBorder(Border.NO_BORDER)).
            AddCell(new Cell().Add(new Paragraph(transactions.First().Amount.ToString())).SetTextAlignment(TextAlignment.RIGHT).SetBorder(Border.NO_BORDER))).SetBorder(Border.NO_BORDER)
            .SetBorderBottom(new SolidBorder(purpleColour, 1F)));

        summaryTable.AddCell(new Cell().Add(new Table(new float[] { 65f, 65f }).AddCell(new Cell().Add(new Paragraph("End Balance")).SetTextAlignment(TextAlignment.LEFT).SetBorder(Border.NO_BORDER)).
            AddCell(new Cell().Add(new Paragraph(transactions.Last().Amount.ToString())).SetTextAlignment(TextAlignment.RIGHT).SetBorder(Border.NO_BORDER))).SetBorder(Border.NO_BORDER)
            .SetBorderBottom(new SolidBorder(purpleColour, 1F)));

        _logger.LogInformation("Summary Table created");
        return summaryTable;

    }
    private void MergeAddressAndSummaryTables(Document document, PdfFont font, Table addressTable, Table summaryTable)
    {

        _logger.LogInformation("Address and Summary Tables merged");
        Table addressSummaryMergeTable = new Table(new float[] { 385F, 30F, 130F }).SetFont(font);
        addressSummaryMergeTable.AddCell(new Cell().Add(addressTable).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT));
        addressSummaryMergeTable.AddCell(new Cell().Add(new Paragraph("")).SetBorder(Border.NO_BORDER));
        addressSummaryMergeTable.AddCell(new Cell().Add(summaryTable).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.RIGHT));

        document.Add(addressSummaryMergeTable);
    }
    private Table CreateHeaderTable(DeviceRgb purpleColour, PdfFont font)
    {

        Table headerTable = new Table(new float[] { 300F }).SetFontColor(purpleColour).SetFont(font).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT);
        headerTable.AddCell(new Cell().Add(new Paragraph("Your Goodman Bank Account Statement")).SetBorder(Border.NO_BORDER).SetFontSize(15F));
        headerTable.AddCell(new Cell().Add(new Paragraph("Current account statement")).SetBorder(Border.NO_BORDER).SetFontSize(11F).SetPaddingTop(7F));
        _logger.LogInformation("Header Table created");
        return headerTable;
    }
    private Table CreateNoticeBoard(DeviceRgb purpleColour, PdfFont font)
    {
        Table noticeBoard = new Table(new float[] { 130F }).SetFont(font).SetBorder(Border.NO_BORDER).SetFontSize(8F).SetHorizontalAlignment(HorizontalAlignment.RIGHT);
        noticeBoard.AddCell(new Cell().Add(new Paragraph("NoticeBoard")).SetBorder(Border.NO_BORDER).SetFontSize(11F).SetBackgroundColor(purpleColour).SetFontColor(ColorConstants.WHITE)
            .SetBorderTopLeftRadius(new BorderRadius(10F)).SetBorderTopRightRadius(new BorderRadius(10F)));
        noticeBoard.AddCell(new Cell().Add(new Paragraph("Your deposit is eligible for protection by the Financial Services Compensation Scheme")).SetBorder(Border.NO_BORDER));
        _logger.LogInformation("Notice Board created");
        return noticeBoard;
    }
    private void MergeHeaderAndNoticeBoardTables(Document document, PdfFont font, Table headerTable, Table noticeBoard)
    {
        Table summNoticeBoardMerge = new Table(new float[] { 385F, 30F, 130F }).SetFont(font).SetMarginTop(10F);
        summNoticeBoardMerge.AddCell(new Cell().Add(headerTable).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT));
        summNoticeBoardMerge.AddCell(new Cell().Add(new Paragraph("")).SetBorder(Border.NO_BORDER));
        summNoticeBoardMerge.AddCell(new Cell().Add(noticeBoard).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.RIGHT));

        document.Add(summNoticeBoardMerge);
        _logger.LogInformation("Header and Notice Board Tables merged");

    }

    private void TransactionTable(Document document, DeviceRgb purpleColour , DeviceRgb offPurpleColour, PdfFont font, List<Transaction> transactions)
    {
        Table transactionsTable = new Table(new float[] { 80, 110, 70, 70, 70 }).SetFont(font).SetFontSize(10F).SetFontColor(ColorConstants.BLACK).SetBorder(Border.NO_BORDER).SetMarginTop(10);
      

        transactionsTable.AddCell(new Cell(1, 2).Add(new Paragraph("Your transactions").SetPadding(3)).SetBorder(Border.NO_BORDER).SetBackgroundColor(purpleColour)
        .SetFontColor(ColorConstants.WHITE).SetPaddingBottom(3F).SetFontSize(11F).SetBorderTopRightRadius(new BorderRadius(10F)).SetBorderTopLeftRadius(new BorderRadius(10F)).SetBorderBottom(new SolidBorder(purpleColour, 1F)));
        transactionsTable.AddCell(new Cell(1, 3).Add(new Paragraph("")).SetBorder(Border.NO_BORDER).SetFontSize(11F).SetBorderBottom(new SolidBorder(purpleColour, 1F)));
        transactionsTable.AddCell(new Cell().Add(new Paragraph("Date")).SetBorder(Border.NO_BORDER).SetFontSize(11F).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour));
        transactionsTable.AddCell(new Cell().Add(new Paragraph("Description")).SetBorder(Border.NO_BORDER).SetFontSize(11F).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour));
        transactionsTable.AddCell(new Cell().Add(new Paragraph("Money out")).SetBorder(Border.NO_BORDER).SetFontSize(11F).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetTextAlignment(TextAlignment.RIGHT));
        transactionsTable.AddCell(new Cell().Add(new Paragraph("Money In")).SetBorder(Border.NO_BORDER).SetFontSize(11F).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetTextAlignment(TextAlignment.RIGHT));
        transactionsTable.AddCell(new Cell().Add(new Paragraph("Amount")).SetBorder(Border.NO_BORDER).SetFontSize(11F).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetTextAlignment(TextAlignment.RIGHT));
        transactionsTable.AddCell(new Cell().Add(new Paragraph("Balance")).SetBorder(Border.NO_BORDER).SetFontSize(11F).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetTextAlignment(TextAlignment.RIGHT));
       
        transactionsTable.AddCell(new Cell().Add(new Paragraph(transactions.First().TransactionDate.ToString())).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetBackgroundColor(offPurpleColour));
        transactionsTable.AddCell(new Cell().Add(new Paragraph("Start balance")).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetBackgroundColor(offPurpleColour));
        transactionsTable.AddCell(new Cell().Add(new Paragraph(" ")).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetBackgroundColor(offPurpleColour));
        transactionsTable.AddCell(new Cell().Add(new Paragraph(" ")).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetBackgroundColor(offPurpleColour));
        transactionsTable.AddCell(new Cell().Add(new Paragraph(transactions.First().Amount.ToString())).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetBackgroundColor(offPurpleColour).SetTextAlignment(TextAlignment.RIGHT));

        int backgroundCounter = 0;
        for (int i = 0; i < transactions.Count; i++)
        {

            transactionsTable.AddCell(new Cell().Add(new Paragraph(transactions[i].TransactionDate.ToString())).SetBackgroundColor((backgroundCounter % 2 == 0) ? ColorConstants.WHITE : offPurpleColour)
                .SetBorder(new SolidBorder(ColorConstants.WHITE, 1F)).SetFontColor(ColorConstants.BLACK));
            transactionsTable.AddCell(new Cell().Add(new Paragraph(transactions[i].TransactionDescription)).SetBackgroundColor((backgroundCounter % 2 == 0) ? ColorConstants.WHITE : offPurpleColour)
                .SetBorder(new SolidBorder(ColorConstants.WHITE, 1F)).SetFontColor(ColorConstants.BLACK).SetKeepTogether(true));
            // transactionsTable.AddCell(new Cell().Add(new Paragraph(transactions[i].MoneyOut.ToString())).SetBackgroundColor((backgroundCounter % 2 == 0) ? ColorConstants.WHITE : offPurpleColour)
            //     .SetBorder(new SolidBorder(ColorConstants.WHITE, 1F)).SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.RIGHT));
            // transactionsTable.AddCell(new Cell().Add(new Paragraph(transactions[i].MoneyIn.ToString())).SetBackgroundColor((backgroundCounter % 2 == 0) ? ColorConstants.WHITE : offPurpleColour)
            //     .SetBorder(new SolidBorder(ColorConstants.WHITE, 1F)).SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.RIGHT));
            // transactionsTable.AddCell(new Cell().Add(new Paragraph(transactions[i].Balance.ToString())).SetBackgroundColor((backgroundCounter % 2 == 0) ? ColorConstants.WHITE : offPurpleColour)
            //     .SetBorder(new SolidBorder(ColorConstants.WHITE, 1F)).SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.RIGHT));

            transactionsTable.AddCell(new Cell().Add(new Paragraph(transactions[i].Amount.ToString())).SetBackgroundColor((backgroundCounter % 2 == 0) ? ColorConstants.WHITE : offPurpleColour)
                .SetBorder(new SolidBorder(ColorConstants.WHITE, 1F)).SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.RIGHT));
            backgroundCounter++;
        }

        transactionsTable.AddCell(new Cell().Add(new Paragraph(transactions.Last().TransactionDate.ToString())).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetBackgroundColor(offPurpleColour));
        transactionsTable.AddCell(new Cell().Add(new Paragraph("End balance")).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetBackgroundColor(offPurpleColour));
        transactionsTable.AddCell(new Cell().Add(new Paragraph(" ")).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetBackgroundColor(offPurpleColour));
        transactionsTable.AddCell(new Cell().Add(new Paragraph(" ")).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetBackgroundColor(offPurpleColour));
        transactionsTable.AddCell(new Cell().Add(new Paragraph(transactions.Last().Amount.ToString())).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(purpleColour, 0.5F)).SetBorderRight(new SolidBorder(ColorConstants.WHITE, 0.5F)).SetFontColor(purpleColour).SetBackgroundColor(offPurpleColour).SetTextAlignment(TextAlignment.RIGHT));

        document.Add(transactionsTable);
        _logger.LogInformation("Transaction Table created");

    }
    private void Miscelleanous(Document document, DeviceRgb purpleColour, PdfFont font)
    {
        Table miscTable = new Table(new float[] { 95F, 305F }).SetFont(font).SetFontSize(10F).SetFontColor(ColorConstants.BLACK).SetBorder(Border.NO_BORDER).SetMarginTop(20).SetKeepTogether(true);
        miscTable.AddCell(new Cell().Add(new Paragraph("Anything Wrong?").SetFontColor(purpleColour)).SetBorder(Border.NO_BORDER));
        miscTable.AddCell(new Cell().Add(new Paragraph("If you've spotted any incorrect or unusual transactions, kindly contact us immediately")).SetBorder(Border.NO_BORDER));

        document.Add(miscTable);
        _logger.LogInformation("Miscelleanous Table created");
    }
}
public class PDFHeaderEventHandler : IEventHandler
{
    public void HandleEvent(Event currentEvent)
    {
        try
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;
            string logoPath = "C:/Assets/Logo.png";
            var logo = ImageDataFactory.Create(logoPath);
            PdfPage page = docEvent.GetPage();
            PdfDocument pdf = docEvent.GetDocument();
            Rectangle pageSize = page.GetPageSize();
            PdfCanvas pdfCanvas = new(page.GetLastContentStream(), page.GetResources(), pdf);
            if (pdf.GetPageNumber(page) == 1)
            {
                //i want the logo just on page 1
                pdfCanvas.AddImageAt(logo, pageSize.GetWidth() - logo.GetWidth() - 480, pageSize.GetHeight() - logo.GetHeight() - 15, true);
                _ = new Canvas(pdfCanvas, pageSize);
            }
            else
            {
                _ = new Canvas(pdfCanvas, pageSize);
            }


        }
        catch (Exception)
        {
            throw;
        }
    }
}
public class PDFFooterEventHandler : IEventHandler
{
    public void HandleEvent(Event currentEvent)
    {
        try
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;

            PdfPage page = docEvent.GetPage();
            PdfDocument pdf = docEvent.GetDocument();
            Rectangle pageSize = page.GetPageSize();
            PdfCanvas pdfCanvas = new(page.GetLastContentStream(), page.GetResources(), pdf);
            int pageNumber = pdf.GetPageNumber(page);
            int numberOfPages = pdf.GetNumberOfPages();
            

            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA, PdfEncodings.CP1252, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            DeviceRgb offPurpleColour = new(230, 230, 250);

            float[] tableWidth = { 445, 50F };
            Table footerTable = new Table(tableWidth).SetFixedPosition(0F, 15F, pageSize.GetWidth()).SetBorder(Border.NO_BORDER);

            var botom = pageSize.GetBottom() + 15F;
            var getwidth = pageSize.GetWidth();

            footerTable.AddCell(new Cell().Add(new Paragraph("Goodman Bank PLC is authorised by the Prudential Regulation Authority and regulated by the Financial Conduct Authority and the Prudential Regulation Authority(FRN: 54113411) Registered in Brussels and Vienna (Company Number: 23336654) Registered Office: 15 Downing Street, London XY11 6TF"))
                                .SetFont(font).SetFontSize(7F).SetBackgroundColor(offPurpleColour).SetBorder(Border.NO_BORDER).SetPaddingLeft(25F).SetPaddingRight(10F));



            Canvas canvas = new(pdfCanvas, pageSize);
            canvas.Add(footerTable).SetBorder(Border.NO_BORDER);

        }
        catch (Exception)
        {
            //_logger.LogError(ex, "An error occurred while in HandleEvent method in PDFFooterEventHandler class : {RequestId}");

            throw;
        }

    }

    public void WritePageNumbers(PdfDocument pdf, Document document)
    {
        PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA, PdfEncodings.CP1252, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
        DeviceRgb offPurpleColour = new(230, 230, 250);
        int numberOfPages = pdf.GetNumberOfPages();

        for (int i = 1; i <= numberOfPages; i++)
        {
            // Write aligned text to the specified by parameters point
            document.ShowTextAligned(new Paragraph("Page " + i + " of " + numberOfPages).SetFont(font).SetFontSize(7F).SetBackgroundColor(offPurpleColour).SetBorder(Border.NO_BORDER).SetWidth(50F).SetPaddings(8F, 28F, 9F, 7F).SetTextAlignment(TextAlignment.RIGHT),
                    555, 15.5f, i, TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);
        }
    }
}




