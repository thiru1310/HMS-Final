using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HSM_PROJECT.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace HSM_PROJECT.Services
{
    public class PdfGeneratorService
    {
        private readonly Font _headerFont;
        private readonly Font _subHeaderFont;
        private readonly Font _normalFont;
        private readonly Font _boldFont;
        private readonly Font _smallFont;

        public PdfGeneratorService()
        {
            // Initialize fonts
            _headerFont = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD, BaseColor.DARK_GRAY);
            _subHeaderFont = new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD, BaseColor.DARK_GRAY);
            _normalFont = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.BLACK);
            _boldFont = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.BLACK);
            _smallFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.GRAY);
        }

        /// <summary>
        /// Generate PDF from HL7 message
        /// </summary>
        public byte[] GeneratePdf(HL7Message message, PdfGenerationOptions options)
        {
            using (var memoryStream = new MemoryStream())
            {
                var document = CreateDocument(options);
                var writer = PdfWriter.GetInstance(document, memoryStream);

                // Set document properties
                document.AddTitle(options.Title);
                document.AddAuthor(options.Author);
                document.AddSubject(options.Subject);
                document.AddCreator("HSM HL7 Parser");
                document.AddCreationDate(DateTime.Now);

                document.Open();

                // Add watermark if specified
                if (!string.IsNullOrEmpty(options.WatermarkText))
                {
                    AddWatermark(writer, options.WatermarkText);
                }

                // Add header
                if (options.IncludeHeader)
                {
                    AddHeader(document, options);
                }

                // Add message content
                AddMessageContent(document, message, options);

                // Add footer
                if (options.IncludeFooter)
                {
                    AddFooter(document, options);
                }

                document.Close();
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Create document with specified options
        /// </summary>
        private Document CreateDocument(PdfGenerationOptions options)
        {
            Rectangle pageSize;
            switch (options.PageFormat)
            {
                case PdfPageFormat.Letter:
                    pageSize = PageSize.LETTER;
                    break;
                case PdfPageFormat.Legal:
                    pageSize = PageSize.LEGAL;
                    break;
                default:
                    pageSize = PageSize.A4;
                    break;
            }

            if (options.Orientation == PdfOrientation.Landscape)
            {
                pageSize = pageSize.Rotate();
            }

            return new Document(pageSize, 36, 36, 54, 54);
        }

        /// <summary>
        /// Add header to document
        /// </summary>
        private void AddHeader(Document document, PdfGenerationOptions options)
        {
            var headerTable = new PdfPTable(2) { WidthPercentage = 100 };
            headerTable.SetWidths(new float[] { 70, 30 });

            // Left side - Title
            var titleCell = new PdfPCell(new Phrase(options.Title, _headerFont))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingBottom = 10
            };
            headerTable.AddCell(titleCell);

            // Right side - Date
            var dateCell = new PdfPCell(new Phrase($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", _smallFont))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingBottom = 10
            };
            headerTable.AddCell(dateCell);

            document.Add(headerTable);
            document.Add(new Paragraph(" ", _normalFont)); // Spacer
        }

        /// <summary>
        /// Add message content to document
        /// </summary>
        private void AddMessageContent(Document document, HL7Message message, PdfGenerationOptions options)
        {
            // Message Summary Section
            AddMessageSummary(document, message);

            // Message Header (MSH) Section
            if (message.MSH != null)
            {
                AddMSHSection(document, message.MSH);
            }

            // Patient Information (PID) Section
            if (message.PID != null)
            {
                AddPIDSection(document, message.PID);
            }

            // Event Information (EVN) Section
            if (message.EVN != null)
            {
                AddEVNSection(document, message.EVN);
            }

            // Patient Visit (PV1) Section
            if (message.PV1 != null)
            {
                AddPV1Section(document, message.PV1);
            }

            // Observation Requests (OBR) Section
            if (message.OBR != null && message.OBR.Any())
            {
                AddOBRSection(document, message.OBR);
            }

            // Observations (OBX) Section
            if (message.OBX != null && message.OBX.Any())
            {
                AddOBXSection(document, message.OBX);
            }

            // Notes (NTE) Section
            if (message.NTE != null && message.NTE.Any())
            {
                AddNTESection(document, message.NTE);
            }

            // Raw Data Section (if requested)
            if (options.IncludeRawData)
            {
                AddRawDataSection(document, message);
            }
        }

        /// <summary>
        /// Add message summary section
        /// </summary>
        private void AddMessageSummary(Document document, HL7Message message)
        {
            document.Add(new Paragraph("Message Summary", _subHeaderFont));
            document.Add(new Paragraph(" ", _normalFont));

            var summaryTable = new PdfPTable(2) { WidthPercentage = 100 };
            summaryTable.SetWidths(new float[] { 30, 70 });

            AddTableRow(summaryTable, "Message Type:", message.MessageType ?? "N/A");
            AddTableRow(summaryTable, "Control ID:", message.MessageControlId ?? "N/A");
            AddTableRow(summaryTable, "Processing Date:", message.ProcessingDateTime != DateTime.MinValue ? 
                message.ProcessingDateTime.ToString("yyyy-MM-dd HH:mm:ss") : "N/A");
            AddTableRow(summaryTable, "Sending Application:", message.SendingApplication ?? "N/A");
            AddTableRow(summaryTable, "Sending Facility:", message.SendingFacility ?? "N/A");
            AddTableRow(summaryTable, "Receiving Application:", message.ReceivingApplication ?? "N/A");
            AddTableRow(summaryTable, "Receiving Facility:", message.ReceivingFacility ?? "N/A");
            AddTableRow(summaryTable, "Version:", message.VersionId ?? "N/A");

            document.Add(summaryTable);
            document.Add(new Paragraph(" ", _normalFont));
        }

        /// <summary>
        /// Add MSH section
        /// </summary>
        private void AddMSHSection(Document document, MSHSegment msh)
        {
            document.Add(new Paragraph("Message Header (MSH)", _subHeaderFont));
            document.Add(new Paragraph(" ", _normalFont));

            var mshTable = new PdfPTable(2) { WidthPercentage = 100 };
            mshTable.SetWidths(new float[] { 30, 70 });

            AddTableRow(mshTable, "Field Separator:", msh.FieldSeparator ?? "N/A");
            AddTableRow(mshTable, "Encoding Characters:", msh.EncodingCharacters ?? "N/A");
            AddTableRow(mshTable, "Sending Application:", msh.SendingApplication ?? "N/A");
            AddTableRow(mshTable, "Sending Facility:", msh.SendingFacility ?? "N/A");
            AddTableRow(mshTable, "Receiving Application:", msh.ReceivingApplication ?? "N/A");
            AddTableRow(mshTable, "Receiving Facility:", msh.ReceivingFacility ?? "N/A");
            AddTableRow(mshTable, "Date/Time of Message:", msh.DateTimeOfMessage != DateTime.MinValue ? 
                msh.DateTimeOfMessage.ToString("yyyy-MM-dd HH:mm:ss") : "N/A");
            AddTableRow(mshTable, "Security:", msh.Security ?? "N/A");
            AddTableRow(mshTable, "Message Type:", msh.MessageType ?? "N/A");
            AddTableRow(mshTable, "Message Control ID:", msh.MessageControlID ?? "N/A");
            AddTableRow(mshTable, "Processing ID:", msh.ProcessingID ?? "N/A");
            AddTableRow(mshTable, "Version ID:", msh.VersionID ?? "N/A");

            document.Add(mshTable);
            document.Add(new Paragraph(" ", _normalFont));
        }

        /// <summary>
        /// Add PID section
        /// </summary>
        private void AddPIDSection(Document document, PIDSegment pid)
        {
            document.Add(new Paragraph("Patient Identification (PID)", _subHeaderFont));
            document.Add(new Paragraph(" ", _normalFont));

            var pidTable = new PdfPTable(2) { WidthPercentage = 100 };
            pidTable.SetWidths(new float[] { 30, 70 });

            AddTableRow(pidTable, "Patient ID:", pid.PatientID ?? "N/A");
            AddTableRow(pidTable, "Patient Name:", pid.PatientName ?? "N/A");
            AddTableRow(pidTable, "Date of Birth:", pid.DateOfBirth?.ToString("yyyy-MM-dd") ?? "N/A");
            AddTableRow(pidTable, "Sex:", pid.Sex ?? "N/A");
            AddTableRow(pidTable, "Address:", pid.PatientAddress ?? "N/A");
            AddTableRow(pidTable, "Phone (Home):", pid.PhoneNumberHome ?? "N/A");
            AddTableRow(pidTable, "Phone (Business):", pid.PhoneNumberBusiness ?? "N/A");
            AddTableRow(pidTable, "Marital Status:", pid.MaritalStatus ?? "N/A");
            AddTableRow(pidTable, "Race:", pid.Race ?? "N/A");
            AddTableRow(pidTable, "Religion:", pid.Religion ?? "N/A");
            AddTableRow(pidTable, "Account Number:", pid.PatientAccountNumber ?? "N/A");
            AddTableRow(pidTable, "SSN:", pid.SSNNumber ?? "N/A");

            document.Add(pidTable);
            document.Add(new Paragraph(" ", _normalFont));
        }

        /// <summary>
        /// Add EVN section
        /// </summary>
        private void AddEVNSection(Document document, EVNSegment evn)
        {
            document.Add(new Paragraph("Event Type (EVN)", _subHeaderFont));
            document.Add(new Paragraph(" ", _normalFont));

            var evnTable = new PdfPTable(2) { WidthPercentage = 100 };
            evnTable.SetWidths(new float[] { 30, 70 });

            AddTableRow(evnTable, "Event Type Code:", evn.EventTypeCode ?? "N/A");
            AddTableRow(evnTable, "Recorded Date/Time:", evn.RecordedDateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");
            AddTableRow(evnTable, "Planned Event Date/Time:", evn.DateTimePlannedEvent?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");
            AddTableRow(evnTable, "Event Reason Code:", evn.EventReasonCode ?? "N/A");
            AddTableRow(evnTable, "Operator ID:", evn.OperatorID ?? "N/A");
            AddTableRow(evnTable, "Event Occurred:", evn.EventOccurred?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");

            document.Add(evnTable);
            document.Add(new Paragraph(" ", _normalFont));
        }

        /// <summary>
        /// Add PV1 section
        /// </summary>
        private void AddPV1Section(Document document, PV1Segment pv1)
        {
            document.Add(new Paragraph("Patient Visit (PV1)", _subHeaderFont));
            document.Add(new Paragraph(" ", _normalFont));

            var pv1Table = new PdfPTable(2) { WidthPercentage = 100 };
            pv1Table.SetWidths(new float[] { 30, 70 });

            AddTableRow(pv1Table, "Patient Class:", pv1.PatientClass ?? "N/A");
            AddTableRow(pv1Table, "Assigned Location:", pv1.AssignedPatientLocation ?? "N/A");
            AddTableRow(pv1Table, "Admission Type:", pv1.AdmissionType ?? "N/A");
            AddTableRow(pv1Table, "Attending Doctor:", pv1.AttendingDoctor ?? "N/A");
            AddTableRow(pv1Table, "Referring Doctor:", pv1.ReferringDoctor ?? "N/A");
            AddTableRow(pv1Table, "Hospital Service:", pv1.HospitalService ?? "N/A");
            AddTableRow(pv1Table, "Visit Number:", pv1.VisitNumber ?? "N/A");
            AddTableRow(pv1Table, "Admit Date/Time:", pv1.AdmitDateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");
            AddTableRow(pv1Table, "Discharge Date/Time:", pv1.DischargeDateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");

            document.Add(pv1Table);
            document.Add(new Paragraph(" ", _normalFont));
        }

        /// <summary>
        /// Add OBR section
        /// </summary>
        private void AddOBRSection(Document document, List<OBRSegment> obrList)
        {
            document.Add(new Paragraph("Observation Requests (OBR)", _subHeaderFont));
            document.Add(new Paragraph(" ", _normalFont));

            foreach (var obr in obrList)
            {
                var obrTable = new PdfPTable(2) { WidthPercentage = 100 };
                obrTable.SetWidths(new float[] { 30, 70 });

                AddTableRow(obrTable, "Set ID:", obr.SetID ?? "N/A");
                AddTableRow(obrTable, "Placer Order Number:", obr.PlacerOrderNumber ?? "N/A");
                AddTableRow(obrTable, "Filler Order Number:", obr.FillerOrderNumber ?? "N/A");
                AddTableRow(obrTable, "Universal Service ID:", obr.UniversalServiceIdentifier ?? "N/A");
                AddTableRow(obrTable, "Priority:", obr.Priority ?? "N/A");
                AddTableRow(obrTable, "Requested Date/Time:", obr.RequestedDateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");
                AddTableRow(obrTable, "Observation Date/Time:", obr.ObservationDateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");
                AddTableRow(obrTable, "Ordering Provider:", obr.OrderingProvider ?? "N/A");
                AddTableRow(obrTable, "Result Status:", obr.ResultStatus ?? "N/A");

                document.Add(obrTable);
                document.Add(new Paragraph(" ", _normalFont));
            }
        }

        /// <summary>
        /// Add OBX section
        /// </summary>
        private void AddOBXSection(Document document, List<OBXSegment> obxList)
        {
            document.Add(new Paragraph("Observations/Results (OBX)", _subHeaderFont));
            document.Add(new Paragraph(" ", _normalFont));

            foreach (var obx in obxList)
            {
                var obxTable = new PdfPTable(2) { WidthPercentage = 100 };
                obxTable.SetWidths(new float[] { 30, 70 });

                AddTableRow(obxTable, "Set ID:", obx.SetID ?? "N/A");
                AddTableRow(obxTable, "Value Type:", obx.ValueType ?? "N/A");
                AddTableRow(obxTable, "Observation Identifier:", obx.ObservationIdentifier ?? "N/A");
                AddTableRow(obxTable, "Observation Value:", obx.ObservationValue ?? "N/A");
                AddTableRow(obxTable, "Units:", obx.Units ?? "N/A");
                AddTableRow(obxTable, "Reference Range:", obx.ReferencesRange ?? "N/A");
                AddTableRow(obxTable, "Abnormal Flags:", obx.AbnormalFlags ?? "N/A");
                AddTableRow(obxTable, "Result Status:", obx.ObservationResultStatus ?? "N/A");
                AddTableRow(obxTable, "Date/Time of Observation:", obx.DateTimeOfObservation?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");

                document.Add(obxTable);
                document.Add(new Paragraph(" ", _normalFont));
            }
        }

        /// <summary>
        /// Add NTE section
        /// </summary>
        private void AddNTESection(Document document, List<NTESegment> nteList)
        {
            document.Add(new Paragraph("Notes and Comments (NTE)", _subHeaderFont));
            document.Add(new Paragraph(" ", _normalFont));

            foreach (var nte in nteList)
            {
                var nteTable = new PdfPTable(2) { WidthPercentage = 100 };
                nteTable.SetWidths(new float[] { 30, 70 });

                AddTableRow(nteTable, "Set ID:", nte.SetID ?? "N/A");
                AddTableRow(nteTable, "Source of Comment:", nte.SourceOfComment ?? "N/A");
                AddTableRow(nteTable, "Comment:", nte.Comment ?? "N/A");
                AddTableRow(nteTable, "Comment Type:", nte.CommentType ?? "N/A");

                document.Add(nteTable);
                document.Add(new Paragraph(" ", _normalFont));
            }
        }

        /// <summary>
        /// Add raw data section
        /// </summary>
        private void AddRawDataSection(Document document, HL7Message message)
        {
            document.Add(new Paragraph("Raw Segment Data", _subHeaderFont));
            document.Add(new Paragraph(" ", _normalFont));

            foreach (var segmentType in message.RawSegments.Keys)
            {
                document.Add(new Paragraph($"{segmentType} Segments:", _boldFont));
                
                foreach (var segment in message.RawSegments[segmentType])
                {
                    var rawTable = new PdfPTable(2) { WidthPercentage = 100 };
                    rawTable.SetWidths(new float[] { 20, 80 });

                    foreach (var field in segment)
                    {
                        AddTableRow(rawTable, field.Key, field.Value ?? "N/A", _smallFont);
                    }

                    document.Add(rawTable);
                    document.Add(new Paragraph(" ", _normalFont));
                }
            }
        }

        /// <summary>
        /// Add footer to document
        /// </summary>
        private void AddFooter(Document document, PdfGenerationOptions options)
        {
            var footerTable = new PdfPTable(3) { WidthPercentage = 100 };
            footerTable.SetWidths(new float[] { 33, 34, 33 });

            // Left - System info
            var leftCell = new PdfPCell(new Phrase("HSM HL7 Parser System", _smallFont))
            {
                Border = Rectangle.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                PaddingTop = 10
            };
            footerTable.AddCell(leftCell);

            // Center - Page number
            var centerCell = new PdfPCell(new Phrase("Page 1", _smallFont))
            {
                Border = Rectangle.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 10
            };
            footerTable.AddCell(centerCell);

            // Right - Date
            var rightCell = new PdfPCell(new Phrase(DateTime.Now.ToString("yyyy-MM-dd"), _smallFont))
            {
                Border = Rectangle.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                PaddingTop = 10
            };
            footerTable.AddCell(rightCell);

            document.Add(footerTable);
        }

        /// <summary>
        /// Add watermark to document
        /// </summary>
        private void AddWatermark(PdfWriter writer, string watermarkText)
        {
            var cb = writer.DirectContent;
            var bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            
            cb.SaveState();
            cb.SetGState(new PdfGState() { FillOpacity = 0.1f });
            cb.BeginText();
            cb.SetFontAndSize(bf, 60);
            cb.SetTextMatrix(200, 400);
            cb.ShowText(watermarkText);
            cb.EndText();
            cb.RestoreState();
        }

        /// <summary>
        /// Add table row helper method
        /// </summary>
        private void AddTableRow(PdfPTable table, string label, string value, Font font = null)
        {
            if (font == null) font = _normalFont;

            var labelCell = new PdfPCell(new Phrase(label, _boldFont))
            {
                Border = Rectangle.NO_BORDER,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            table.AddCell(labelCell);

            var valueCell = new PdfPCell(new Phrase(value, font))
            {
                Border = Rectangle.NO_BORDER,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            table.AddCell(valueCell);
        }
    }
}