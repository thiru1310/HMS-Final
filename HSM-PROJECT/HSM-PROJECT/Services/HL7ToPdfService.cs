using System;
using System.Collections.Generic;
using System.Linq;
using HSM_PROJECT.Models;

namespace HSM_PROJECT.Services
{
    public class HL7ToPdfService
    {
        private readonly HL7ParserService _parserService;
        private readonly PdfGeneratorService _pdfService;

        public HL7ToPdfService()
        {
            _parserService = new HL7ParserService();
            _pdfService = new PdfGeneratorService();
        }

        /// <summary>
        /// Convert HL7 message to PDF
        /// </summary>
        public HL7ToPdfResponse ConvertToPdf(HL7ToPdfRequest request)
        {
            var response = new HL7ToPdfResponse();

            try
            {
                // Validate input
                if (request == null)
                {
                    response.Success = false;
                    response.Message = "Request cannot be null";
                    response.Errors.Add("Invalid request");
                    return response;
                }

                if (string.IsNullOrWhiteSpace(request.HL7Message))
                {
                    response.Success = false;
                    response.Message = "HL7 message cannot be empty";
                    response.Errors.Add("Missing HL7 message content");
                    return response;
                }

                // Initialize parser service with custom config if provided
                var parserService = request.ParserConfig != null 
                    ? new HL7ParserService(request.ParserConfig) 
                    : _parserService;

                // Parse HL7 message
                HL7Message parsedMessage;
                try
                {
                    parsedMessage = parserService.ParseMessage(request.HL7Message);
                    response.ParsedMessage = parsedMessage;
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = $"Failed to parse HL7 message: {ex.Message}";
                    response.Errors.Add($"Parse error: {ex.Message}");
                    return response;
                }

                // Validate parsed message
                if (request.ParserConfig?.ValidateMessage == true)
                {
                    var validationErrors = parserService.ValidateMessage(parsedMessage);
                    if (validationErrors.Any())
                    {
                        response.Warnings.AddRange(validationErrors);
                    }
                }

                // Generate PDF
                try
                {
                    var pdfOptions = request.PdfOptions ?? new PdfGenerationOptions();
                    response.PdfData = _pdfService.GeneratePdf(parsedMessage, pdfOptions);
                    
                    // Generate filename
                    response.FileName = GenerateFileName(parsedMessage, pdfOptions);
                    
                    response.Success = true;
                    response.Message = "PDF generated successfully";
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = $"Failed to generate PDF: {ex.Message}";
                    response.Errors.Add($"PDF generation error: {ex.Message}");
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Unexpected error: {ex.Message}";
                response.Errors.Add($"System error: {ex.Message}");
            }

            return response;
        }

        /// <summary>
        /// Parse HL7 message only (without PDF generation)
        /// </summary>
        public HL7Message ParseHL7Message(string hl7MessageString, HL7ParserConfig config = null)
        {
            var parserService = config != null ? new HL7ParserService(config) : _parserService;
            return parserService.ParseMessage(hl7MessageString);
        }

        /// <summary>
        /// Generate PDF from already parsed HL7 message
        /// </summary>
        public byte[] GeneratePdfFromParsedMessage(HL7Message message, PdfGenerationOptions options = null)
        {
            var pdfOptions = options ?? new PdfGenerationOptions();
            return _pdfService.GeneratePdf(message, pdfOptions);
        }

        /// <summary>
        /// Validate HL7 message structure
        /// </summary>
        public List<string> ValidateHL7Message(HL7Message message)
        {
            return _parserService.ValidateMessage(message);
        }

        /// <summary>
        /// Get message summary
        /// </summary>
        public string GetMessageSummary(HL7Message message)
        {
            return _parserService.GetMessageSummary(message);
        }

        /// <summary>
        /// Check if HL7 message string is valid format
        /// </summary>
        public bool IsValidHL7Format(string hl7MessageString)
        {
            if (string.IsNullOrWhiteSpace(hl7MessageString))
                return false;

            try
            {
                // Basic format check - should start with MSH
                var lines = hl7MessageString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (!lines.Any())
                    return false;

                var firstLine = lines.First().Trim();
                if (!firstLine.StartsWith("MSH"))
                    return false;

                // Try to parse - if it throws, it's invalid
                _parserService.ParseMessage(hl7MessageString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get supported HL7 segment types
        /// </summary>
        public List<string> GetSupportedSegmentTypes()
        {
            return new List<string>
            {
                "MSH", // Message Header
                "PID", // Patient Identification
                "OBX", // Observation/Result
                "OBR", // Observation Request
                "EVN", // Event Type
                "PV1", // Patient Visit
                "NTE"  // Notes and Comments
            };
        }

        /// <summary>
        /// Get HL7 message statistics
        /// </summary>
        public Dictionary<string, int> GetMessageStatistics(HL7Message message)
        {
            var stats = new Dictionary<string, int>();

            if (message?.RawSegments != null)
            {
                foreach (var segmentType in message.RawSegments.Keys)
                {
                    stats[segmentType] = message.RawSegments[segmentType].Count;
                }
            }

            return stats;
        }

        /// <summary>
        /// Extract patient information from HL7 message
        /// </summary>
        public Dictionary<string, string> ExtractPatientInfo(HL7Message message)
        {
            var patientInfo = new Dictionary<string, string>();

            if (message?.PID != null)
            {
                patientInfo["PatientID"] = message.PID.PatientID ?? "";
                patientInfo["PatientName"] = message.PID.PatientName ?? "";
                patientInfo["DateOfBirth"] = message.PID.DateOfBirth?.ToString("yyyy-MM-dd") ?? "";
                patientInfo["Sex"] = message.PID.Sex ?? "";
                patientInfo["Address"] = message.PID.PatientAddress ?? "";
                patientInfo["Phone"] = message.PID.PhoneNumberHome ?? "";
                patientInfo["AccountNumber"] = message.PID.PatientAccountNumber ?? "";
            }

            return patientInfo;
        }

        /// <summary>
        /// Extract observation results from HL7 message
        /// </summary>
        public List<Dictionary<string, string>> ExtractObservationResults(HL7Message message)
        {
            var results = new List<Dictionary<string, string>>();

            if (message?.OBX != null)
            {
                foreach (var obx in message.OBX)
                {
                    var result = new Dictionary<string, string>
                    {
                        ["SetID"] = obx.SetID ?? "",
                        ["ValueType"] = obx.ValueType ?? "",
                        ["ObservationIdentifier"] = obx.ObservationIdentifier ?? "",
                        ["ObservationValue"] = obx.ObservationValue ?? "",
                        ["Units"] = obx.Units ?? "",
                        ["ReferenceRange"] = obx.ReferencesRange ?? "",
                        ["AbnormalFlags"] = obx.AbnormalFlags ?? "",
                        ["ResultStatus"] = obx.ObservationResultStatus ?? "",
                        ["ObservationDateTime"] = obx.DateTimeOfObservation?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
                    };
                    results.Add(result);
                }
            }

            return results;
        }

        /// <summary>
        /// Generate filename for PDF
        /// </summary>
        private string GenerateFileName(HL7Message message, PdfGenerationOptions options)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var messageType = message?.MessageType?.Replace("^", "_") ?? "HL7";
            var controlId = message?.MessageControlId ?? timestamp;
            
            return $"HL7_{messageType}_{controlId}_{timestamp}.pdf";
        }

        /// <summary>
        /// Create sample HL7 message for testing
        /// </summary>
        public string CreateSampleHL7Message()
        {
            var sample = @"MSH|^~\&|SENDING_APP|SENDING_FACILITY|RECEIVING_APP|RECEIVING_FACILITY|20231201120000||ADT^A01|MSG001|P|2.5
EVN|A01|20231201120000|||USER1|20231201120000
PID|1||12345^^^MR||DOE^JOHN^MIDDLE||19800115|M||C|123 MAIN ST^^ANYTOWN^ST^12345||555-1234|555-5678|EN|M|CHR|11111111|123-45-6789
PV1|1|I|ICU^101^1|||12345^SMITH^JOHN^A|||SUR||||19|||12345^SMITH^JOHN^A|INP|A|||||||||||||||||||SF|||20231201120000
OBR|1|ORD123|FIL456|CBC^COMPLETE BLOOD COUNT||20231201120000|20231201120000|||||||20231201120000||12345^SMITH^JOHN^A||||||20231201130000|||F
OBX|1|NM|WBC^White Blood Cell Count|1|7.5|10*3/uL|4.0-11.0|N|||F|||20231201130000
OBX|2|NM|RBC^Red Blood Cell Count|1|4.2|10*6/uL|4.2-5.4|N|||F|||20231201130000
OBX|3|NM|HGB^Hemoglobin|1|13.5|g/dL|12.0-16.0|N|||F|||20231201130000
NTE|1||Patient fasting for 12 hours prior to blood draw
NTE|2||Results reviewed and approved by attending physician";

            return sample;
        }

        /// <summary>
        /// Get available PDF generation options
        /// </summary>
        public PdfGenerationOptions GetDefaultPdfOptions()
        {
            return new PdfGenerationOptions
            {
                Title = "HL7 Message Report",
                Author = "HSM System",
                Subject = "HL7 Message Analysis",
                IncludeHeader = true,
                IncludeFooter = true,
                IncludeRawData = false,
                PageFormat = PdfPageFormat.A4,
                Orientation = PdfOrientation.Portrait
            };
        }
    }
}