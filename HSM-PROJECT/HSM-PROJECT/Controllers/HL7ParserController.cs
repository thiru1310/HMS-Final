using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HSM_PROJECT.Models;
using HSM_PROJECT.Services;
using Newtonsoft.Json;

namespace HSM_PROJECT.Controllers
{
    public class HL7ParserController : Controller
    {
        private readonly HL7ToPdfService _hl7ToPdfService;

        public HL7ParserController()
        {
            _hl7ToPdfService = new HL7ToPdfService();
        }

        // GET: HL7Parser
        public ActionResult Index()
        {
            ViewBag.SupportedSegments = _hl7ToPdfService.GetSupportedSegmentTypes();
            ViewBag.SampleMessage = _hl7ToPdfService.CreateSampleHL7Message();
            ViewBag.DefaultOptions = _hl7ToPdfService.GetDefaultPdfOptions();
            return View();
        }

        /// <summary>
        /// Convert HL7 message to PDF
        /// </summary>
        [HttpPost]
        public ActionResult ConvertToPdf(HL7ToPdfRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Json(new { success = false, message = "Invalid request" }, JsonRequestBehavior.AllowGet);
                }

                var response = _hl7ToPdfService.ConvertToPdf(request);

                if (response.Success)
                {
                    // Return PDF as file download
                    return File(response.PdfData, "application/pdf", response.FileName);
                }
                else
                {
                    return Json(new 
                    { 
                        success = false, 
                        message = response.Message, 
                        errors = response.Errors,
                        warnings = response.Warnings
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Parse HL7 message and return JSON
        /// </summary>
        [HttpPost]
        public ActionResult ParseMessage(string hl7Message, HL7ParserConfig config = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hl7Message))
                {
                    return Json(new { success = false, message = "HL7 message is required" }, JsonRequestBehavior.AllowGet);
                }

                var parsedMessage = _hl7ToPdfService.ParseHL7Message(hl7Message, config);
                var summary = _hl7ToPdfService.GetMessageSummary(parsedMessage);
                var statistics = _hl7ToPdfService.GetMessageStatistics(parsedMessage);
                var patientInfo = _hl7ToPdfService.ExtractPatientInfo(parsedMessage);
                var observationResults = _hl7ToPdfService.ExtractObservationResults(parsedMessage);

                return Json(new 
                { 
                    success = true,
                    message = "Message parsed successfully",
                    parsedMessage = parsedMessage,
                    summary = summary,
                    statistics = statistics,
                    patientInfo = patientInfo,
                    observationResults = observationResults
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Parse error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Validate HL7 message format
        /// </summary>
        [HttpPost]
        public ActionResult ValidateMessage(string hl7Message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hl7Message))
                {
                    return Json(new { success = false, message = "HL7 message is required" }, JsonRequestBehavior.AllowGet);
                }

                var isValid = _hl7ToPdfService.IsValidHL7Format(hl7Message);
                
                if (isValid)
                {
                    var parsedMessage = _hl7ToPdfService.ParseHL7Message(hl7Message);
                    var validationErrors = _hl7ToPdfService.ValidateHL7Message(parsedMessage);
                    
                    return Json(new 
                    { 
                        success = true,
                        isValid = true,
                        message = "Valid HL7 message format",
                        validationErrors = validationErrors
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new 
                    { 
                        success = true,
                        isValid = false,
                        message = "Invalid HL7 message format"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Validation error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get sample HL7 message
        /// </summary>
        [HttpGet]
        public ActionResult GetSampleMessage()
        {
            try
            {
                var sample = _hl7ToPdfService.CreateSampleHL7Message();
                return Json(new 
                { 
                    success = true,
                    sampleMessage = sample
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get supported segment types
        /// </summary>
        [HttpGet]
        public ActionResult GetSupportedSegments()
        {
            try
            {
                var segments = _hl7ToPdfService.GetSupportedSegmentTypes();
                return Json(new 
                { 
                    success = true,
                    supportedSegments = segments
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get default PDF options
        /// </summary>
        [HttpGet]
        public ActionResult GetDefaultPdfOptions()
        {
            try
            {
                var options = _hl7ToPdfService.GetDefaultPdfOptions();
                return Json(new 
                { 
                    success = true,
                    defaultOptions = options
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Extract patient information from HL7 message
        /// </summary>
        [HttpPost]
        public ActionResult ExtractPatientInfo(string hl7Message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hl7Message))
                {
                    return Json(new { success = false, message = "HL7 message is required" }, JsonRequestBehavior.AllowGet);
                }

                var parsedMessage = _hl7ToPdfService.ParseHL7Message(hl7Message);
                var patientInfo = _hl7ToPdfService.ExtractPatientInfo(parsedMessage);

                return Json(new 
                { 
                    success = true,
                    patientInfo = patientInfo
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Extract observation results from HL7 message
        /// </summary>
        [HttpPost]
        public ActionResult ExtractObservationResults(string hl7Message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hl7Message))
                {
                    return Json(new { success = false, message = "HL7 message is required" }, JsonRequestBehavior.AllowGet);
                }

                var parsedMessage = _hl7ToPdfService.ParseHL7Message(hl7Message);
                var observationResults = _hl7ToPdfService.ExtractObservationResults(parsedMessage);

                return Json(new 
                { 
                    success = true,
                    observationResults = observationResults
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get message statistics
        /// </summary>
        [HttpPost]
        public ActionResult GetMessageStatistics(string hl7Message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hl7Message))
                {
                    return Json(new { success = false, message = "HL7 message is required" }, JsonRequestBehavior.AllowGet);
                }

                var parsedMessage = _hl7ToPdfService.ParseHL7Message(hl7Message);
                var statistics = _hl7ToPdfService.GetMessageStatistics(parsedMessage);

                return Json(new 
                { 
                    success = true,
                    statistics = statistics
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Advanced conversion with custom options
        /// </summary>
        [HttpPost]
        public ActionResult AdvancedConvert(string hl7Message, string parserConfigJson = null, string pdfOptionsJson = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hl7Message))
                {
                    return Json(new { success = false, message = "HL7 message is required" }, JsonRequestBehavior.AllowGet);
                }

                // Parse configuration options
                HL7ParserConfig parserConfig = null;
                if (!string.IsNullOrWhiteSpace(parserConfigJson))
                {
                    parserConfig = JsonConvert.DeserializeObject<HL7ParserConfig>(parserConfigJson);
                }

                PdfGenerationOptions pdfOptions = null;
                if (!string.IsNullOrWhiteSpace(pdfOptionsJson))
                {
                    pdfOptions = JsonConvert.DeserializeObject<PdfGenerationOptions>(pdfOptionsJson);
                }

                var request = new HL7ToPdfRequest
                {
                    HL7Message = hl7Message,
                    ParserConfig = parserConfig ?? new HL7ParserConfig(),
                    PdfOptions = pdfOptions ?? new PdfGenerationOptions()
                };

                var response = _hl7ToPdfService.ConvertToPdf(request);

                if (response.Success)
                {
                    return File(response.PdfData, "application/pdf", response.FileName);
                }
                else
                {
                    return Json(new 
                    { 
                        success = false, 
                        message = response.Message, 
                        errors = response.Errors,
                        warnings = response.Warnings
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Batch process multiple HL7 messages
        /// </summary>
        [HttpPost]
        public ActionResult BatchProcess(string[] hl7Messages, string pdfOptionsJson = null)
        {
            try
            {
                if (hl7Messages == null || !hl7Messages.Any())
                {
                    return Json(new { success = false, message = "At least one HL7 message is required" }, JsonRequestBehavior.AllowGet);
                }

                PdfGenerationOptions pdfOptions = null;
                if (!string.IsNullOrWhiteSpace(pdfOptionsJson))
                {
                    pdfOptions = JsonConvert.DeserializeObject<PdfGenerationOptions>(pdfOptionsJson);
                }

                var results = new List<object>();

                foreach (var hl7Message in hl7Messages)
                {
                    try
                    {
                        var request = new HL7ToPdfRequest
                        {
                            HL7Message = hl7Message,
                            PdfOptions = pdfOptions ?? new PdfGenerationOptions()
                        };

                        var response = _hl7ToPdfService.ConvertToPdf(request);
                        
                        results.Add(new 
                        {
                            success = response.Success,
                            message = response.Message,
                            fileName = response.FileName,
                            hasErrors = response.Errors.Any(),
                            errorCount = response.Errors.Count,
                            warningCount = response.Warnings.Count
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new 
                        {
                            success = false,
                            message = $"Error processing message: {ex.Message}",
                            fileName = "",
                            hasErrors = true,
                            errorCount = 1,
                            warningCount = 0
                        });
                    }
                }

                return Json(new 
                { 
                    success = true,
                    message = $"Processed {hl7Messages.Length} messages",
                    results = results,
                    totalProcessed = hl7Messages.Length,
                    successCount = results.Count(r => (bool)((dynamic)r).success),
                    errorCount = results.Count(r => !(bool)((dynamic)r).success)
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Batch processing error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}