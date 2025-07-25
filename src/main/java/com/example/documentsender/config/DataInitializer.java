package com.example.documentsender.config;

import com.example.documentsender.entity.Document;
import com.example.documentsender.entity.DocumentStatus;
import com.example.documentsender.repository.DocumentRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.CommandLineRunner;
import org.springframework.context.annotation.Profile;
import org.springframework.stereotype.Component;

@Component
@Profile("!test") // Don't run during tests
public class DataInitializer implements CommandLineRunner {

    private static final Logger logger = LoggerFactory.getLogger(DataInitializer.class);

    @Autowired
    private DocumentRepository documentRepository;

    @Override
    public void run(String... args) throws Exception {
        if (documentRepository.count() == 0) {
            logger.info("Initializing sample data...");

            // Create sample documents
            Document doc1 = new Document("Invoice_001.pdf", "/documents/invoice_001.pdf", "client1@example.com");
            doc1.setSenderEmail("sender@example.com");
            doc1.setSubject("Invoice #001");
            doc1.setMessage("Please find attached invoice for your recent purchase.");

            Document doc2 = new Document("Contract_ABC.pdf", "/documents/contract_abc.pdf", "client2@example.com");
            doc2.setSenderEmail("sender@example.com");
            doc2.setSubject("Contract Agreement");
            doc2.setMessage("Please review and sign the attached contract.");

            Document doc3 = new Document("Report_Q1.pdf", "/documents/report_q1.pdf", "manager@example.com");
            doc3.setSenderEmail("sender@example.com");
            doc3.setSubject("Q1 Report");
            doc3.setStatus(DocumentStatus.FAILED); // Simulate a failed document
            doc3.incrementSentCount(); // Simulate one failed attempt

            Document doc4 = new Document("Proposal_XYZ.pdf", "/documents/proposal_xyz.pdf", "prospect@example.com");
            doc4.setSenderEmail("sender@example.com");
            doc4.setSubject("Business Proposal");
            doc4.setStatus(DocumentStatus.SENT);
            doc4.incrementSentCount(); // Simulate successful send

            documentRepository.save(doc1);
            documentRepository.save(doc2);
            documentRepository.save(doc3);
            documentRepository.save(doc4);

            logger.info("Sample data initialized successfully. Created {} documents.", documentRepository.count());
        } else {
            logger.info("Database already contains data. Skipping initialization.");
        }
    }
}