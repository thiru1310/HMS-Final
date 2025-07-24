# HL7 Message to PDF Parser

A comprehensive .NET solution for dynamically parsing HL7 (Health Level 7) messages and converting them to formatted PDF documents. This system provides both a web interface and programmatic API for healthcare data processing.

## Features

### Core Functionality
- **Dynamic HL7 Parsing**: Parse any HL7 message format with configurable separators
- **PDF Generation**: Convert parsed HL7 data to professional PDF reports
- **Web Interface**: User-friendly web application for interactive HL7 processing
- **REST API**: Programmatic access for integration with other systems
- **Batch Processing**: Handle multiple HL7 messages simultaneously
- **Validation**: Built-in HL7 message structure validation

### Supported HL7 Segments
- **MSH** - Message Header
- **PID** - Patient Identification  
- **OBX** - Observation/Result
- **OBR** - Observation Request
- **EVN** - Event Type
- **PV1** - Patient Visit
- **NTE** - Notes and Comments

### PDF Customization Options
- Multiple page formats (A4, Letter, Legal)
- Portrait/Landscape orientation
- Custom headers and footers
- Watermark support
- Raw segment data inclusion
- Professional formatting with tables and sections

## Getting Started

### Prerequisites
- .NET Framework 4.7.2 or higher
- Visual Studio 2017 or later
- IIS or IIS Express for web hosting

### Required NuGet Packages
```xml
<package id="iTextSharp" version="5.5.13.3" targetFramework="net472" />
<package id="NHapi" version="3.0.7" targetFramework="net472" />
<package id="Newtonsoft.Json" version="12.0.2" targetFramework="net472" />
```

### Installation
1. Clone or download the project
2. Open `HSM-PROJECT.sln` in Visual Studio
3. Restore NuGet packages
4. Build the solution
5. Run the web application

## Usage

### Web Interface
Navigate to `/HL7Parser` in your web application to access the interactive interface:

1. **Input HL7 Message**: Paste your HL7 message or load a sample
2. **Configure Options**: Set PDF formatting and parser options
3. **Generate PDF**: Click "Generate PDF" to create and download the report
4. **Parse & Analyze**: Extract specific information like patient data or observations

### Programmatic Usage

#### Basic Conversion
```csharp
using HSM_PROJECT.Services;
using HSM_PROJECT.Models;

// Create service instance
var hl7Service = new HL7ToPdfService();

// Your HL7 message
string hl7Message = "MSH|^~\\&|SENDING_APP|FACILITY|...";

// Create conversion request
var request = new HL7ToPdfRequest
{
    HL7Message = hl7Message,
    PdfOptions = new PdfGenerationOptions
    {
        Title = "Patient Lab Results",
        Author = "Hospital System",
        IncludeHeader = true,
        IncludeFooter = true,
        PageFormat = PdfPageFormat.A4,
        Orientation = PdfOrientation.Portrait
    }
};

// Convert to PDF
var response = hl7Service.ConvertToPdf(request);

if (response.Success)
{
    // Save PDF file
    File.WriteAllBytes(response.FileName, response.PdfData);
    Console.WriteLine($"PDF generated: {response.FileName}");
}
else
{
    Console.WriteLine($"Error: {response.Message}");
}
```

#### Parse Only
```csharp
// Parse HL7 message without generating PDF
var parsedMessage = hl7Service.ParseHL7Message(hl7Message);

// Extract patient information
var patientInfo = hl7Service.ExtractPatientInfo(parsedMessage);
Console.WriteLine($"Patient: {patientInfo["PatientName"]}");

// Extract observation results
var observations = hl7Service.ExtractObservationResults(parsedMessage);
foreach (var obs in observations)
{
    Console.WriteLine($"Test: {obs["ObservationIdentifier"]} = {obs["ObservationValue"]}");
}
```

#### Custom Parser Configuration
```csharp
var parserConfig = new HL7ParserConfig
{
    FieldSeparator = "|",
    ComponentSeparator = "^",
    RepetitionSeparator = "~",
    EscapeCharacter = "\\",
    ValidateMessage = true,
    IncludeUnknownSegments = true
};

var parsedMessage = hl7Service.ParseHL7Message(hl7Message, parserConfig);
```

### REST API Endpoints

#### Convert HL7 to PDF
```http
POST /HL7Parser/ConvertToPdf
Content-Type: application/json

{
    "HL7Message": "MSH|^~\\&|SENDING_APP|...",
    "PdfOptions": {
        "Title": "Lab Results",
        "Author": "Hospital System",
        "IncludeHeader": true,
        "PageFormat": 0,
        "Orientation": 0
    }
}
```

#### Parse HL7 Message
```http
POST /HL7Parser/ParseMessage
Content-Type: application/x-www-form-urlencoded

hl7Message=MSH|^~\&|SENDING_APP|...
```

#### Validate HL7 Message
```http
POST /HL7Parser/ValidateMessage
Content-Type: application/x-www-form-urlencoded

hl7Message=MSH|^~\&|SENDING_APP|...
```

#### Extract Patient Information
```http
POST /HL7Parser/ExtractPatientInfo
Content-Type: application/x-www-form-urlencoded

hl7Message=MSH|^~\&|SENDING_APP|...
```

## Sample HL7 Message

```
MSH|^~\&|SENDING_APP|SENDING_FACILITY|RECEIVING_APP|RECEIVING_FACILITY|20231201120000||ADT^A01|MSG001|P|2.5
EVN|A01|20231201120000|||USER1|20231201120000
PID|1||12345^^^MR||DOE^JOHN^MIDDLE||19800115|M||C|123 MAIN ST^^ANYTOWN^ST^12345||555-1234|555-5678|EN|M|CHR|11111111|123-45-6789
PV1|1|I|ICU^101^1|||12345^SMITH^JOHN^A|||SUR||||19|||12345^SMITH^JOHN^A|INP|A|||||||||||||||||||SF|||20231201120000
OBR|1|ORD123|FIL456|CBC^COMPLETE BLOOD COUNT||20231201120000|20231201120000|||||||20231201120000||12345^SMITH^JOHN^A||||||20231201130000|||F
OBX|1|NM|WBC^White Blood Cell Count|1|7.5|10*3/uL|4.0-11.0|N|||F|||20231201130000
OBX|2|NM|RBC^Red Blood Cell Count|1|4.2|10*6/uL|4.2-5.4|N|||F|||20231201130000
OBX|3|NM|HGB^Hemoglobin|1|13.5|g/dL|12.0-16.0|N|||F|||20231201130000
NTE|1||Patient fasting for 12 hours prior to blood draw
NTE|2||Results reviewed and approved by attending physician
```

## Testing

### Unit Testing
The project includes a `TestHL7Parser` class with various test methods:

```csharp
// Run basic functionality test
TestHL7Parser.TestConversion();

// Test custom HL7 message
TestHL7Parser.TestCustomMessage(yourHL7Message);

// Test batch processing
TestHL7Parser.TestBatchProcessing();

// Performance testing
TestHL7Parser.TestPerformance(100);
```

### Web Interface Testing
1. Navigate to `/HL7Parser`
2. Click "Load Sample" to load a test message
3. Click "Validate" to check message format
4. Click "Generate PDF" to test PDF creation
5. Use other buttons to test specific extraction features

## Configuration

### PDF Generation Options
```csharp
var pdfOptions = new PdfGenerationOptions
{
    Title = "Custom Report Title",
    Author = "Your Organization",
    Subject = "HL7 Message Analysis",
    IncludeHeader = true,
    IncludeFooter = true,
    IncludeRawData = false,
    PageFormat = PdfPageFormat.A4,
    Orientation = PdfOrientation.Portrait,
    WatermarkText = "CONFIDENTIAL"
};
```

### Parser Configuration
```csharp
var parserConfig = new HL7ParserConfig
{
    FieldSeparator = "|",
    ComponentSeparator = "^",
    RepetitionSeparator = "~",
    EscapeCharacter = "\\",
    SubComponentSeparator = "&",
    IncludeUnknownSegments = true,
    ValidateMessage = true
};
```

## Architecture

### Project Structure
```
HSM-PROJECT/
├── Controllers/
│   └── HL7ParserController.cs    # Web API endpoints
├── Models/
│   └── HL7Models.cs              # Data models
├── Services/
│   ├── HL7ParserService.cs       # HL7 parsing logic
│   ├── PdfGeneratorService.cs    # PDF generation
│   └── HL7ToPdfService.cs        # Main service orchestrator
├── Views/
│   └── HL7Parser/
│       └── Index.cshtml           # Web interface
└── TestHL7Parser.cs              # Test utilities
```

### Key Classes

#### HL7ToPdfService
Main service class that orchestrates HL7 parsing and PDF generation.

#### HL7ParserService  
Handles parsing of HL7 messages into structured objects.

#### PdfGeneratorService
Generates formatted PDF documents from parsed HL7 data.

#### HL7Message
Primary data model representing a parsed HL7 message with all segments.

## Error Handling

The system includes comprehensive error handling:

- **Validation Errors**: Invalid HL7 format or structure
- **Parse Errors**: Malformed segments or fields
- **PDF Generation Errors**: Issues creating PDF output
- **System Errors**: Unexpected exceptions

All errors are captured in the response objects with detailed messages.

## Performance Considerations

- **Memory Usage**: Large HL7 messages are processed efficiently
- **Throughput**: Typical processing time is 100-500ms per message
- **Scalability**: Stateless design supports concurrent processing
- **Caching**: Consider implementing caching for frequently processed messages

## Security

- **Input Validation**: All HL7 input is validated before processing
- **XSS Protection**: Web interface includes proper encoding
- **File Access**: PDF generation uses secure temporary files
- **Error Information**: Sensitive data is not exposed in error messages

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is provided as-is for educational and development purposes.

## Support

For issues or questions:
1. Check the test examples in `TestHL7Parser.cs`
2. Review the web interface at `/HL7Parser`
3. Examine the sample HL7 messages provided
4. Check the API documentation above

## Version History

- **v1.0**: Initial release with core HL7 parsing and PDF generation
- Dynamic segment parsing support
- Web interface and REST API
- Comprehensive test suite
- Professional PDF formatting