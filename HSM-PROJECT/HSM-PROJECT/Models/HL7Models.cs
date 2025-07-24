using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HSM_PROJECT.Models
{
    /// <summary>
    /// Represents a parsed HL7 message with all segments
    /// </summary>
    public class HL7Message
    {
        public string MessageType { get; set; }
        public string MessageControlId { get; set; }
        public DateTime ProcessingDateTime { get; set; }
        public string SendingApplication { get; set; }
        public string SendingFacility { get; set; }
        public string ReceivingApplication { get; set; }
        public string ReceivingFacility { get; set; }
        public string VersionId { get; set; }
        
        public MSHSegment MSH { get; set; }
        public PIDSegment PID { get; set; }
        public List<OBXSegment> OBX { get; set; }
        public List<OBRSegment> OBR { get; set; }
        public EVNSegment EVN { get; set; }
        public PV1Segment PV1 { get; set; }
        public List<NTESegment> NTE { get; set; }
        
        // Raw segments for dynamic parsing
        public Dictionary<string, List<Dictionary<string, string>>> RawSegments { get; set; }
        
        public HL7Message()
        {
            OBX = new List<OBXSegment>();
            OBR = new List<OBRSegment>();
            NTE = new List<NTESegment>();
            RawSegments = new Dictionary<string, List<Dictionary<string, string>>>();
        }
    }

    /// <summary>
    /// Message Header Segment
    /// </summary>
    public class MSHSegment
    {
        public string FieldSeparator { get; set; }
        public string EncodingCharacters { get; set; }
        public string SendingApplication { get; set; }
        public string SendingFacility { get; set; }
        public string ReceivingApplication { get; set; }
        public string ReceivingFacility { get; set; }
        public DateTime DateTimeOfMessage { get; set; }
        public string Security { get; set; }
        public string MessageType { get; set; }
        public string MessageControlID { get; set; }
        public string ProcessingID { get; set; }
        public string VersionID { get; set; }
    }

    /// <summary>
    /// Patient Identification Segment
    /// </summary>
    public class PIDSegment
    {
        public string PatientID { get; set; }
        public string PatientIdentifierList { get; set; }
        public string AlternatePatientID { get; set; }
        public string PatientName { get; set; }
        public string MothersMaidenName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Sex { get; set; }
        public string PatientAlias { get; set; }
        public string Race { get; set; }
        public string PatientAddress { get; set; }
        public string CountyCode { get; set; }
        public string PhoneNumberHome { get; set; }
        public string PhoneNumberBusiness { get; set; }
        public string PrimaryLanguage { get; set; }
        public string MaritalStatus { get; set; }
        public string Religion { get; set; }
        public string PatientAccountNumber { get; set; }
        public string SSNNumber { get; set; }
    }

    /// <summary>
    /// Observation/Result Segment
    /// </summary>
    public class OBXSegment
    {
        public string SetID { get; set; }
        public string ValueType { get; set; }
        public string ObservationIdentifier { get; set; }
        public string ObservationSubID { get; set; }
        public string ObservationValue { get; set; }
        public string Units { get; set; }
        public string ReferencesRange { get; set; }
        public string AbnormalFlags { get; set; }
        public string Probability { get; set; }
        public string NatureOfAbnormalTest { get; set; }
        public string ObservationResultStatus { get; set; }
        public DateTime? DateLastObsNormalValues { get; set; }
        public string UserDefinedAccessChecks { get; set; }
        public DateTime? DateTimeOfObservation { get; set; }
        public string ProducersID { get; set; }
        public string ResponsibleObserver { get; set; }
        public string ObservationMethod { get; set; }
    }

    /// <summary>
    /// Observation Request Segment
    /// </summary>
    public class OBRSegment
    {
        public string SetID { get; set; }
        public string PlacerOrderNumber { get; set; }
        public string FillerOrderNumber { get; set; }
        public string UniversalServiceIdentifier { get; set; }
        public string Priority { get; set; }
        public DateTime? RequestedDateTime { get; set; }
        public DateTime? ObservationDateTime { get; set; }
        public DateTime? ObservationEndDateTime { get; set; }
        public string CollectionVolume { get; set; }
        public string CollectorIdentifier { get; set; }
        public string SpecimenActionCode { get; set; }
        public string DangerCode { get; set; }
        public string RelevantClinicalInfo { get; set; }
        public DateTime? SpecimenReceivedDateTime { get; set; }
        public string SpecimenSource { get; set; }
        public string OrderingProvider { get; set; }
        public string OrderCallbackPhoneNumber { get; set; }
        public string PlacerField1 { get; set; }
        public string PlacerField2 { get; set; }
        public string FillerField1 { get; set; }
        public string FillerField2 { get; set; }
        public DateTime? ResultsRptStatusChngDateTime { get; set; }
        public string ChargeToPractice { get; set; }
        public string DiagnosticServSectID { get; set; }
        public string ResultStatus { get; set; }
    }

    /// <summary>
    /// Event Type Segment
    /// </summary>
    public class EVNSegment
    {
        public string EventTypeCode { get; set; }
        public DateTime? RecordedDateTime { get; set; }
        public DateTime? DateTimePlannedEvent { get; set; }
        public string EventReasonCode { get; set; }
        public string OperatorID { get; set; }
        public DateTime? EventOccurred { get; set; }
    }

    /// <summary>
    /// Patient Visit Segment
    /// </summary>
    public class PV1Segment
    {
        public string SetID { get; set; }
        public string PatientClass { get; set; }
        public string AssignedPatientLocation { get; set; }
        public string AdmissionType { get; set; }
        public string PreadmitNumber { get; set; }
        public string PriorPatientLocation { get; set; }
        public string AttendingDoctor { get; set; }
        public string ReferringDoctor { get; set; }
        public string ConsultingDoctor { get; set; }
        public string HospitalService { get; set; }
        public string TemporaryLocation { get; set; }
        public string PreadmitTestIndicator { get; set; }
        public string ReadmissionIndicator { get; set; }
        public string AdmitSource { get; set; }
        public string AmbulatoryStatus { get; set; }
        public string VIPIndicator { get; set; }
        public string AdmittingDoctor { get; set; }
        public string PatientType { get; set; }
        public string VisitNumber { get; set; }
        public string FinancialClass { get; set; }
        public string ChargePriceIndicator { get; set; }
        public string CourtesyCode { get; set; }
        public string CreditRating { get; set; }
        public string ContractCode { get; set; }
        public string ContractEffectiveDate { get; set; }
        public string ContractAmount { get; set; }
        public string ContractPeriod { get; set; }
        public string InterestCode { get; set; }
        public string TransferToBadDebtCode { get; set; }
        public DateTime? TransferToBadDebtDate { get; set; }
        public string BadDebtAgencyCode { get; set; }
        public string BadDebtTransferAmount { get; set; }
        public string BadDebtRecoveryAmount { get; set; }
        public string DeleteAccountIndicator { get; set; }
        public DateTime? DeleteAccountDate { get; set; }
        public string DischargeDisposition { get; set; }
        public string DischargedToLocation { get; set; }
        public string DietType { get; set; }
        public string ServicingFacility { get; set; }
        public string BedStatus { get; set; }
        public string AccountStatus { get; set; }
        public string PendingLocation { get; set; }
        public string PriorTemporaryLocation { get; set; }
        public DateTime? AdmitDateTime { get; set; }
        public DateTime? DischargeDateTime { get; set; }
        public string CurrentPatientBalance { get; set; }
        public string TotalCharges { get; set; }
        public string TotalAdjustments { get; set; }
        public string TotalPayments { get; set; }
        public string AlternateVisitID { get; set; }
    }

    /// <summary>
    /// Notes and Comments Segment
    /// </summary>
    public class NTESegment
    {
        public string SetID { get; set; }
        public string SourceOfComment { get; set; }
        public string Comment { get; set; }
        public string CommentType { get; set; }
    }

    /// <summary>
    /// HL7 Parser Configuration
    /// </summary>
    public class HL7ParserConfig
    {
        public string FieldSeparator { get; set; } = "|";
        public string ComponentSeparator { get; set; } = "^";
        public string RepetitionSeparator { get; set; } = "~";
        public string EscapeCharacter { get; set; } = "\\";
        public string SubComponentSeparator { get; set; } = "&";
        public bool IncludeUnknownSegments { get; set; } = true;
        public bool ValidateMessage { get; set; } = true;
    }

    /// <summary>
    /// PDF Generation Options
    /// </summary>
    public class PdfGenerationOptions
    {
        public string Title { get; set; } = "HL7 Message Report";
        public string Author { get; set; } = "HSM System";
        public string Subject { get; set; } = "HL7 Message Analysis";
        public bool IncludeHeader { get; set; } = true;
        public bool IncludeFooter { get; set; } = true;
        public bool IncludeRawData { get; set; } = false;
        public string LogoPath { get; set; }
        public string WatermarkText { get; set; }
        public PdfPageFormat PageFormat { get; set; } = PdfPageFormat.A4;
        public PdfOrientation Orientation { get; set; } = PdfOrientation.Portrait;
    }

    public enum PdfPageFormat
    {
        A4,
        Letter,
        Legal
    }

    public enum PdfOrientation
    {
        Portrait,
        Landscape
    }

    /// <summary>
    /// HL7 to PDF conversion request model
    /// </summary>
    public class HL7ToPdfRequest
    {
        [Required]
        public string HL7Message { get; set; }
        
        public HL7ParserConfig ParserConfig { get; set; }
        public PdfGenerationOptions PdfOptions { get; set; }
        
        public HL7ToPdfRequest()
        {
            ParserConfig = new HL7ParserConfig();
            PdfOptions = new PdfGenerationOptions();
        }
    }

    /// <summary>
    /// HL7 to PDF conversion response model
    /// </summary>
    public class HL7ToPdfResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public byte[] PdfData { get; set; }
        public string FileName { get; set; }
        public HL7Message ParsedMessage { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> Errors { get; set; }
        
        public HL7ToPdfResponse()
        {
            Warnings = new List<string>();
            Errors = new List<string>();
        }
    }
}