using System;
using System.IO;
using HSM_PROJECT.Models;
using HSM_PROJECT.Services;

namespace HSM_PROJECT
{
    /// <summary>
    /// Test class to demonstrate HL7 to PDF conversion
    /// This can be used for testing and development purposes
    /// </summary>
    public class TestHL7Parser
    {
        /// <summary>
        /// Example method showing how to use the HL7 to PDF parser
        /// </summary>
        public static void TestConversion()
        {
            try
            {
                // Create service instance
                var hl7Service = new HL7ToPdfService();

                // Sample HL7 message
                var sampleHL7 = hl7Service.CreateSampleHL7Message();

                Console.WriteLine("=== HL7 to PDF Parser Test ===");
                Console.WriteLine();
                Console.WriteLine("Sample HL7 Message:");
                Console.WriteLine(sampleHL7);
                Console.WriteLine();

                // Test 1: Parse message only
                Console.WriteLine("Test 1: Parsing HL7 Message...");
                var parsedMessage = hl7Service.ParseHL7Message(sampleHL7);
                Console.WriteLine("✓ Message parsed successfully");
                Console.WriteLine($"Message Type: {parsedMessage.MessageType}");
                Console.WriteLine($"Control ID: {parsedMessage.MessageControlId}");
                Console.WriteLine();

                // Test 2: Validate message
                Console.WriteLine("Test 2: Validating message...");
                var validationErrors = hl7Service.ValidateHL7Message(parsedMessage);
                if (validationErrors.Count == 0)
                {
                    Console.WriteLine("✓ Message validation passed");
                }
                else
                {
                    Console.WriteLine("⚠ Validation warnings:");
                    validationErrors.ForEach(error => Console.WriteLine($"  - {error}"));
                }
                Console.WriteLine();

                // Test 3: Extract patient information
                Console.WriteLine("Test 3: Extracting patient information...");
                var patientInfo = hl7Service.ExtractPatientInfo(parsedMessage);
                Console.WriteLine("Patient Information:");
                foreach (var info in patientInfo)
                {
                    if (!string.IsNullOrEmpty(info.Value))
                    {
                        Console.WriteLine($"  {info.Key}: {info.Value}");
                    }
                }
                Console.WriteLine();

                // Test 4: Extract observation results
                Console.WriteLine("Test 4: Extracting observation results...");
                var observations = hl7Service.ExtractObservationResults(parsedMessage);
                Console.WriteLine($"Found {observations.Count} observation(s):");
                for (int i = 0; i < observations.Count; i++)
                {
                    Console.WriteLine($"  Observation {i + 1}:");
                    foreach (var obs in observations[i])
                    {
                        if (!string.IsNullOrEmpty(obs.Value))
                        {
                            Console.WriteLine($"    {obs.Key}: {obs.Value}");
                        }
                    }
                }
                Console.WriteLine();

                // Test 5: Get message statistics
                Console.WriteLine("Test 5: Getting message statistics...");
                var statistics = hl7Service.GetMessageStatistics(parsedMessage);
                Console.WriteLine("Segment Statistics:");
                foreach (var stat in statistics)
                {
                    Console.WriteLine($"  {stat.Key}: {stat.Value} segment(s)");
                }
                Console.WriteLine();

                // Test 6: Generate PDF
                Console.WriteLine("Test 6: Generating PDF...");
                var request = new HL7ToPdfRequest
                {
                    HL7Message = sampleHL7,
                    PdfOptions = new PdfGenerationOptions
                    {
                        Title = "Test HL7 Report",
                        Author = "HL7 Parser Test",
                        IncludeHeader = true,
                        IncludeFooter = true,
                        IncludeRawData = true,
                        PageFormat = PdfPageFormat.A4,
                        Orientation = PdfOrientation.Portrait
                    }
                };

                var response = hl7Service.ConvertToPdf(request);

                if (response.Success)
                {
                    Console.WriteLine("✓ PDF generated successfully");
                    Console.WriteLine($"File name: {response.FileName}");
                    Console.WriteLine($"PDF size: {response.PdfData.Length} bytes");

                    // Optionally save to file for testing
                    var testFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, response.FileName);
                    File.WriteAllBytes(testFilePath, response.PdfData);
                    Console.WriteLine($"PDF saved to: {testFilePath}");
                }
                else
                {
                    Console.WriteLine("✗ PDF generation failed");
                    Console.WriteLine($"Error: {response.Message}");
                    if (response.Errors.Count > 0)
                    {
                        Console.WriteLine("Errors:");
                        response.Errors.ForEach(error => Console.WriteLine($"  - {error}"));
                    }
                }

                Console.WriteLine();
                Console.WriteLine("=== Test Completed ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed with exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Test method for custom HL7 message
        /// </summary>
        public static void TestCustomMessage(string hl7Message)
        {
            try
            {
                var hl7Service = new HL7ToPdfService();

                Console.WriteLine("=== Custom HL7 Message Test ===");
                Console.WriteLine();

                // Validate format first
                if (!hl7Service.IsValidHL7Format(hl7Message))
                {
                    Console.WriteLine("✗ Invalid HL7 message format");
                    return;
                }

                Console.WriteLine("✓ Valid HL7 format detected");

                // Parse and convert
                var request = new HL7ToPdfRequest
                {
                    HL7Message = hl7Message,
                    PdfOptions = hl7Service.GetDefaultPdfOptions()
                };

                var response = hl7Service.ConvertToPdf(request);

                if (response.Success)
                {
                    Console.WriteLine("✓ Conversion successful");
                    Console.WriteLine($"Generated: {response.FileName}");
                    
                    // Save to file
                    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, response.FileName);
                    File.WriteAllBytes(filePath, response.PdfData);
                    Console.WriteLine($"Saved to: {filePath}");
                }
                else
                {
                    Console.WriteLine("✗ Conversion failed");
                    Console.WriteLine($"Error: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Batch processing test
        /// </summary>
        public static void TestBatchProcessing()
        {
            try
            {
                var hl7Service = new HL7ToPdfService();
                
                Console.WriteLine("=== Batch Processing Test ===");
                Console.WriteLine();

                // Create multiple test messages
                var messages = new[]
                {
                    hl7Service.CreateSampleHL7Message(),
                    // You can add more test messages here
                };

                Console.WriteLine($"Processing {messages.Length} messages...");

                var successCount = 0;
                var errorCount = 0;

                for (int i = 0; i < messages.Length; i++)
                {
                    try
                    {
                        var request = new HL7ToPdfRequest
                        {
                            HL7Message = messages[i],
                            PdfOptions = new PdfGenerationOptions
                            {
                                Title = $"Batch Test Report {i + 1}",
                                Author = "Batch Processor"
                            }
                        };

                        var response = hl7Service.ConvertToPdf(request);

                        if (response.Success)
                        {
                            successCount++;
                            Console.WriteLine($"✓ Message {i + 1}: {response.FileName}");
                            
                            // Save file
                            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, response.FileName);
                            File.WriteAllBytes(filePath, response.PdfData);
                        }
                        else
                        {
                            errorCount++;
                            Console.WriteLine($"✗ Message {i + 1}: {response.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        Console.WriteLine($"✗ Message {i + 1}: Exception - {ex.Message}");
                    }
                }

                Console.WriteLine();
                Console.WriteLine($"Batch processing completed: {successCount} successful, {errorCount} failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Batch processing failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Performance test
        /// </summary>
        public static void TestPerformance(int iterations = 10)
        {
            try
            {
                var hl7Service = new HL7ToPdfService();
                var sampleMessage = hl7Service.CreateSampleHL7Message();

                Console.WriteLine($"=== Performance Test ({iterations} iterations) ===");
                Console.WriteLine();

                var startTime = DateTime.Now;

                for (int i = 0; i < iterations; i++)
                {
                    var request = new HL7ToPdfRequest
                    {
                        HL7Message = sampleMessage,
                        PdfOptions = new PdfGenerationOptions
                        {
                            Title = $"Performance Test {i + 1}"
                        }
                    };

                    var response = hl7Service.ConvertToPdf(request);
                    
                    if (i % 10 == 0 || i == iterations - 1)
                    {
                        Console.WriteLine($"Processed {i + 1}/{iterations} messages...");
                    }
                }

                var endTime = DateTime.Now;
                var totalTime = endTime - startTime;
                var averageTime = totalTime.TotalMilliseconds / iterations;

                Console.WriteLine();
                Console.WriteLine($"Performance Results:");
                Console.WriteLine($"Total time: {totalTime.TotalSeconds:F2} seconds");
                Console.WriteLine($"Average time per conversion: {averageTime:F2} ms");
                Console.WriteLine($"Throughput: {iterations / totalTime.TotalSeconds:F2} conversions/second");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Performance test failed: {ex.Message}");
            }
        }
    }
}