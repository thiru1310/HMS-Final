package com.example.documentsender.service;

import com.example.documentsender.entity.Document;
import jakarta.mail.MessagingException;
import jakarta.mail.internet.MimeMessage;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.io.FileSystemResource;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.mail.javamail.MimeMessageHelper;
import org.springframework.stereotype.Service;

import java.io.File;

@Service
public class EmailService {

    private static final Logger logger = LoggerFactory.getLogger(EmailService.class);

    @Autowired
    private JavaMailSender mailSender;

    @Value("${spring.mail.username:noreply@example.com}")
    private String defaultFromEmail;

    /**
     * Send document via email
     * @param document The document to send
     * @return true if email was sent successfully, false otherwise
     */
    public boolean sendDocument(Document document) {
        try {
            MimeMessage message = mailSender.createMimeMessage();
            MimeMessageHelper helper = new MimeMessageHelper(message, true);

            // Set email properties
            helper.setFrom(document.getSenderEmail() != null ? document.getSenderEmail() : defaultFromEmail);
            helper.setTo(document.getRecipientEmail());
            helper.setSubject(document.getSubject() != null ? document.getSubject() : 
                            "Document: " + document.getName());

            // Set email body
            String emailBody = buildEmailBody(document);
            helper.setText(emailBody, true);

            // Attach document if file exists
            File file = new File(document.getFilePath());
            if (file.exists() && file.isFile()) {
                FileSystemResource fileResource = new FileSystemResource(file);
                helper.addAttachment(document.getName(), fileResource);
                logger.info("Attached file: {} to email", document.getFilePath());
            } else {
                logger.warn("File not found: {}", document.getFilePath());
                // Still send email but without attachment
            }

            // Send email
            mailSender.send(message);
            logger.info("Email sent successfully to: {}", document.getRecipientEmail());
            return true;

        } catch (MessagingException e) {
            logger.error("Failed to send email to: {}. Error: {}", 
                        document.getRecipientEmail(), e.getMessage());
            return false;
        } catch (Exception e) {
            logger.error("Unexpected error while sending email to: {}. Error: {}", 
                        document.getRecipientEmail(), e.getMessage());
            return false;
        }
    }

    /**
     * Build email body with document information
     * @param document The document
     * @return HTML email body
     */
    private String buildEmailBody(Document document) {
        StringBuilder body = new StringBuilder();
        
        body.append("<html><body>");
        body.append("<h3>Document Delivery</h3>");
        
        if (document.getMessage() != null && !document.getMessage().trim().isEmpty()) {
            body.append("<p>").append(document.getMessage()).append("</p>");
        } else {
            body.append("<p>Please find the attached document: <strong>")
                .append(document.getName()).append("</strong></p>");
        }
        
        body.append("<p><strong>Document Details:</strong></p>");
        body.append("<ul>");
        body.append("<li><strong>Name:</strong> ").append(document.getName()).append("</li>");
        body.append("<li><strong>Sent Count:</strong> ").append(document.getSentCount() + 1).append("</li>");
        if (document.getLastSentAt() != null) {
            body.append("<li><strong>Previous Sent:</strong> ").append(document.getLastSentAt()).append("</li>");
        }
        body.append("</ul>");
        
        body.append("<p>Best regards,<br/>Document Sender System</p>");
        body.append("</body></html>");
        
        return body.toString();
    }

    /**
     * Test email configuration
     * @param testEmail Email address to send test email to
     * @return true if test email was sent successfully
     */
    public boolean sendTestEmail(String testEmail) {
        try {
            MimeMessage message = mailSender.createMimeMessage();
            MimeMessageHelper helper = new MimeMessageHelper(message, false);

            helper.setFrom(defaultFromEmail);
            helper.setTo(testEmail);
            helper.setSubject("Document Sender - Test Email");
            helper.setText("This is a test email from Document Sender application. Email configuration is working correctly.");

            mailSender.send(message);
            logger.info("Test email sent successfully to: {}", testEmail);
            return true;

        } catch (Exception e) {
            logger.error("Failed to send test email to: {}. Error: {}", testEmail, e.getMessage());
            return false;
        }
    }
}