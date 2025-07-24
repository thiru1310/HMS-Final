using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HSM_PROJECT.Models;

namespace HSM_PROJECT.Services
{
    public class HL7ParserService
    {
        private readonly HL7ParserConfig _config;

        public HL7ParserService(HL7ParserConfig config = null)
        {
            _config = config ?? new HL7ParserConfig();
        }

        /// <summary>
        /// Parse HL7 message string into structured HL7Message object
        /// </summary>
        public HL7Message ParseMessage(string hl7MessageString)
        {
            if (string.IsNullOrWhiteSpace(hl7MessageString))
                throw new ArgumentException("HL7 message cannot be null or empty");

            var message = new HL7Message();
            var lines = SplitMessage(hl7MessageString);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var segmentType = GetSegmentType(line);
                var fields = ParseSegmentFields(line);

                // Store raw segment data
                if (!message.RawSegments.ContainsKey(segmentType))
                    message.RawSegments[segmentType] = new List<Dictionary<string, string>>();

                var rawSegment = new Dictionary<string, string>();
                for (int i = 0; i < fields.Length; i++)
                {
                    rawSegment[$"Field_{i}"] = fields[i];
                }
                message.RawSegments[segmentType].Add(rawSegment);

                // Parse specific segments
                switch (segmentType)
                {
                    case "MSH":
                        message.MSH = ParseMSHSegment(fields);
                        message.MessageType = message.MSH.MessageType;
                        message.MessageControlId = message.MSH.MessageControlID;
                        message.SendingApplication = message.MSH.SendingApplication;
                        message.SendingFacility = message.MSH.SendingFacility;
                        message.ReceivingApplication = message.MSH.ReceivingApplication;
                        message.ReceivingFacility = message.MSH.ReceivingFacility;
                        message.VersionId = message.MSH.VersionID;
                        message.ProcessingDateTime = message.MSH.DateTimeOfMessage;
                        break;
                    case "PID":
                        message.PID = ParsePIDSegment(fields);
                        break;
                    case "OBX":
                        message.OBX.Add(ParseOBXSegment(fields));
                        break;
                    case "OBR":
                        message.OBR.Add(ParseOBRSegment(fields));
                        break;
                    case "EVN":
                        message.EVN = ParseEVNSegment(fields);
                        break;
                    case "PV1":
                        message.PV1 = ParsePV1Segment(fields);
                        break;
                    case "NTE":
                        message.NTE.Add(ParseNTESegment(fields));
                        break;
                }
            }

            return message;
        }

        /// <summary>
        /// Split HL7 message into individual lines/segments
        /// </summary>
        private string[] SplitMessage(string message)
        {
            // HL7 messages can use \r, \n, or \r\n as line separators
            return message.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Get segment type from line
        /// </summary>
        private string GetSegmentType(string line)
        {
            if (string.IsNullOrEmpty(line) || line.Length < 3)
                return "UNKNOWN";

            return line.Substring(0, 3);
        }

        /// <summary>
        /// Parse segment fields using field separator
        /// </summary>
        private string[] ParseSegmentFields(string segment)
        {
            return segment.Split(new[] { _config.FieldSeparator }, StringSplitOptions.None);
        }

        /// <summary>
        /// Parse components within a field using component separator
        /// </summary>
        private string[] ParseComponents(string field)
        {
            if (string.IsNullOrEmpty(field))
                return new string[0];

            return field.Split(new[] { _config.ComponentSeparator }, StringSplitOptions.None);
        }

        /// <summary>
        /// Parse MSH (Message Header) segment
        /// </summary>
        private MSHSegment ParseMSHSegment(string[] fields)
        {
            var msh = new MSHSegment();

            if (fields.Length > 1) msh.FieldSeparator = fields[1];
            if (fields.Length > 2) msh.EncodingCharacters = fields[2];
            if (fields.Length > 3) msh.SendingApplication = fields[3];
            if (fields.Length > 4) msh.SendingFacility = fields[4];
            if (fields.Length > 5) msh.ReceivingApplication = fields[5];
            if (fields.Length > 6) msh.ReceivingFacility = fields[6];
            if (fields.Length > 7) msh.DateTimeOfMessage = ParseHL7DateTime(fields[7]);
            if (fields.Length > 8) msh.Security = fields[8];
            if (fields.Length > 9) msh.MessageType = fields[9];
            if (fields.Length > 10) msh.MessageControlID = fields[10];
            if (fields.Length > 11) msh.ProcessingID = fields[11];
            if (fields.Length > 12) msh.VersionID = fields[12];

            return msh;
        }

        /// <summary>
        /// Parse PID (Patient Identification) segment
        /// </summary>
        private PIDSegment ParsePIDSegment(string[] fields)
        {
            var pid = new PIDSegment();

            if (fields.Length > 1) pid.PatientID = fields[1];
            if (fields.Length > 2) pid.PatientIdentifierList = fields[2];
            if (fields.Length > 3) pid.AlternatePatientID = fields[3];
            if (fields.Length > 4) pid.PatientName = fields[4];
            if (fields.Length > 5) pid.MothersMaidenName = fields[5];
            if (fields.Length > 6) pid.DateOfBirth = ParseHL7Date(fields[6]);
            if (fields.Length > 7) pid.Sex = fields[7];
            if (fields.Length > 8) pid.PatientAlias = fields[8];
            if (fields.Length > 9) pid.Race = fields[9];
            if (fields.Length > 10) pid.PatientAddress = fields[10];
            if (fields.Length > 11) pid.CountyCode = fields[11];
            if (fields.Length > 12) pid.PhoneNumberHome = fields[12];
            if (fields.Length > 13) pid.PhoneNumberBusiness = fields[13];
            if (fields.Length > 14) pid.PrimaryLanguage = fields[14];
            if (fields.Length > 15) pid.MaritalStatus = fields[15];
            if (fields.Length > 16) pid.Religion = fields[16];
            if (fields.Length > 17) pid.PatientAccountNumber = fields[17];
            if (fields.Length > 18) pid.SSNNumber = fields[18];

            return pid;
        }

        /// <summary>
        /// Parse OBX (Observation/Result) segment
        /// </summary>
        private OBXSegment ParseOBXSegment(string[] fields)
        {
            var obx = new OBXSegment();

            if (fields.Length > 1) obx.SetID = fields[1];
            if (fields.Length > 2) obx.ValueType = fields[2];
            if (fields.Length > 3) obx.ObservationIdentifier = fields[3];
            if (fields.Length > 4) obx.ObservationSubID = fields[4];
            if (fields.Length > 5) obx.ObservationValue = fields[5];
            if (fields.Length > 6) obx.Units = fields[6];
            if (fields.Length > 7) obx.ReferencesRange = fields[7];
            if (fields.Length > 8) obx.AbnormalFlags = fields[8];
            if (fields.Length > 9) obx.Probability = fields[9];
            if (fields.Length > 10) obx.NatureOfAbnormalTest = fields[10];
            if (fields.Length > 11) obx.ObservationResultStatus = fields[11];
            if (fields.Length > 12) obx.DateLastObsNormalValues = ParseHL7Date(fields[12]);
            if (fields.Length > 13) obx.UserDefinedAccessChecks = fields[13];
            if (fields.Length > 14) obx.DateTimeOfObservation = ParseHL7Date(fields[14]);
            if (fields.Length > 15) obx.ProducersID = fields[15];
            if (fields.Length > 16) obx.ResponsibleObserver = fields[16];
            if (fields.Length > 17) obx.ObservationMethod = fields[17];

            return obx;
        }

        /// <summary>
        /// Parse OBR (Observation Request) segment
        /// </summary>
        private OBRSegment ParseOBRSegment(string[] fields)
        {
            var obr = new OBRSegment();

            if (fields.Length > 1) obr.SetID = fields[1];
            if (fields.Length > 2) obr.PlacerOrderNumber = fields[2];
            if (fields.Length > 3) obr.FillerOrderNumber = fields[3];
            if (fields.Length > 4) obr.UniversalServiceIdentifier = fields[4];
            if (fields.Length > 5) obr.Priority = fields[5];
            if (fields.Length > 6) obr.RequestedDateTime = ParseHL7Date(fields[6]);
            if (fields.Length > 7) obr.ObservationDateTime = ParseHL7Date(fields[7]);
            if (fields.Length > 8) obr.ObservationEndDateTime = ParseHL7Date(fields[8]);
            if (fields.Length > 9) obr.CollectionVolume = fields[9];
            if (fields.Length > 10) obr.CollectorIdentifier = fields[10];
            if (fields.Length > 11) obr.SpecimenActionCode = fields[11];
            if (fields.Length > 12) obr.DangerCode = fields[12];
            if (fields.Length > 13) obr.RelevantClinicalInfo = fields[13];
            if (fields.Length > 14) obr.SpecimenReceivedDateTime = ParseHL7Date(fields[14]);
            if (fields.Length > 15) obr.SpecimenSource = fields[15];
            if (fields.Length > 16) obr.OrderingProvider = fields[16];
            if (fields.Length > 17) obr.OrderCallbackPhoneNumber = fields[17];
            if (fields.Length > 18) obr.PlacerField1 = fields[18];
            if (fields.Length > 19) obr.PlacerField2 = fields[19];
            if (fields.Length > 20) obr.FillerField1 = fields[20];
            if (fields.Length > 21) obr.FillerField2 = fields[21];
            if (fields.Length > 22) obr.ResultsRptStatusChngDateTime = ParseHL7Date(fields[22]);
            if (fields.Length > 23) obr.ChargeToPractice = fields[23];
            if (fields.Length > 24) obr.DiagnosticServSectID = fields[24];
            if (fields.Length > 25) obr.ResultStatus = fields[25];

            return obr;
        }

        /// <summary>
        /// Parse EVN (Event Type) segment
        /// </summary>
        private EVNSegment ParseEVNSegment(string[] fields)
        {
            var evn = new EVNSegment();

            if (fields.Length > 1) evn.EventTypeCode = fields[1];
            if (fields.Length > 2) evn.RecordedDateTime = ParseHL7Date(fields[2]);
            if (fields.Length > 3) evn.DateTimePlannedEvent = ParseHL7Date(fields[3]);
            if (fields.Length > 4) evn.EventReasonCode = fields[4];
            if (fields.Length > 5) evn.OperatorID = fields[5];
            if (fields.Length > 6) evn.EventOccurred = ParseHL7Date(fields[6]);

            return evn;
        }

        /// <summary>
        /// Parse PV1 (Patient Visit) segment
        /// </summary>
        private PV1Segment ParsePV1Segment(string[] fields)
        {
            var pv1 = new PV1Segment();

            if (fields.Length > 1) pv1.SetID = fields[1];
            if (fields.Length > 2) pv1.PatientClass = fields[2];
            if (fields.Length > 3) pv1.AssignedPatientLocation = fields[3];
            if (fields.Length > 4) pv1.AdmissionType = fields[4];
            if (fields.Length > 5) pv1.PreadmitNumber = fields[5];
            if (fields.Length > 6) pv1.PriorPatientLocation = fields[6];
            if (fields.Length > 7) pv1.AttendingDoctor = fields[7];
            if (fields.Length > 8) pv1.ReferringDoctor = fields[8];
            if (fields.Length > 9) pv1.ConsultingDoctor = fields[9];
            if (fields.Length > 10) pv1.HospitalService = fields[10];
            if (fields.Length > 44) pv1.AdmitDateTime = ParseHL7Date(fields[44]);
            if (fields.Length > 45) pv1.DischargeDateTime = ParseHL7Date(fields[45]);

            return pv1;
        }

        /// <summary>
        /// Parse NTE (Notes and Comments) segment
        /// </summary>
        private NTESegment ParseNTESegment(string[] fields)
        {
            var nte = new NTESegment();

            if (fields.Length > 1) nte.SetID = fields[1];
            if (fields.Length > 2) nte.SourceOfComment = fields[2];
            if (fields.Length > 3) nte.Comment = fields[3];
            if (fields.Length > 4) nte.CommentType = fields[4];

            return nte;
        }

        /// <summary>
        /// Parse HL7 date/time format
        /// </summary>
        private DateTime ParseHL7DateTime(string dateTimeString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString))
                return DateTime.MinValue;

            // HL7 datetime formats: YYYYMMDDHHMMSS, YYYYMMDDHHMM, YYYYMMDDHH, YYYYMMDD
            var formats = new[]
            {
                "yyyyMMddHHmmss",
                "yyyyMMddHHmm",
                "yyyyMMddHH",
                "yyyyMMdd",
                "yyyyMM",
                "yyyy"
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(dateTimeString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
            }

            // Try standard parsing as fallback
            if (DateTime.TryParse(dateTimeString, out DateTime fallbackResult))
            {
                return fallbackResult;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Parse HL7 date format (nullable)
        /// </summary>
        private DateTime? ParseHL7Date(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            var result = ParseHL7DateTime(dateString);
            return result == DateTime.MinValue ? (DateTime?)null : result;
        }

        /// <summary>
        /// Validate HL7 message structure
        /// </summary>
        public List<string> ValidateMessage(HL7Message message)
        {
            var errors = new List<string>();

            if (message == null)
            {
                errors.Add("Message is null");
                return errors;
            }

            if (message.MSH == null)
            {
                errors.Add("MSH segment is required");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(message.MSH.MessageType))
                    errors.Add("Message type is required in MSH segment");
                
                if (string.IsNullOrWhiteSpace(message.MSH.MessageControlID))
                    errors.Add("Message control ID is required in MSH segment");
            }

            return errors;
        }

        /// <summary>
        /// Get message summary for display
        /// </summary>
        public string GetMessageSummary(HL7Message message)
        {
            if (message?.MSH == null)
                return "Invalid HL7 Message";

            var summary = new StringBuilder();
            summary.AppendLine($"Message Type: {message.MessageType}");
            summary.AppendLine($"Control ID: {message.MessageControlId}");
            summary.AppendLine($"Sending Application: {message.SendingApplication}");
            summary.AppendLine($"Sending Facility: {message.SendingFacility}");
            summary.AppendLine($"Processing Date: {message.ProcessingDateTime:yyyy-MM-dd HH:mm:ss}");
            
            if (message.PID != null)
            {
                summary.AppendLine($"Patient: {message.PID.PatientName}");
                summary.AppendLine($"Patient ID: {message.PID.PatientID}");
            }

            summary.AppendLine($"Segments Found: {string.Join(", ", message.RawSegments.Keys)}");

            return summary.ToString();
        }
    }
}