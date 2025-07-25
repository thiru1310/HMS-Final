package com.example.documentsender.service;

import com.example.documentsender.dto.DocumentRequest;
import com.example.documentsender.dto.DocumentResponse;
import com.example.documentsender.entity.Document;
import com.example.documentsender.entity.DocumentStatus;
import com.example.documentsender.repository.DocumentRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

@Service
@Transactional
public class DocumentService {

    private static final Logger logger = LoggerFactory.getLogger(DocumentService.class);

    @Autowired
    private DocumentRepository documentRepository;

    @Autowired
    private EmailService emailService;

    @Value("${app.max-send-retries:5}")
    private Integer maxSendRetries;

    /**
     * Create a new document
     * @param request DocumentRequest containing document details
     * @return DocumentResponse with created document information
     */
    public DocumentResponse createDocument(DocumentRequest request) {
        Document document = new Document();
        document.setName(request.getName());
        document.setFilePath(request.getFilePath());
        document.setRecipientEmail(request.getRecipientEmail());
        document.setSenderEmail(request.getSenderEmail());
        document.setSubject(request.getSubject());
        document.setMessage(request.getMessage());

        Document savedDocument = documentRepository.save(document);
        logger.info("Created new document with ID: {}", savedDocument.getId());

        return convertToResponse(savedDocument);
    }

    /**
     * Send a document via email
     * @param documentId ID of the document to send
     * @return DocumentResponse with updated status
     */
    public DocumentResponse sendDocument(Long documentId) {
        Optional<Document> optionalDocument = documentRepository.findById(documentId);
        
        if (optionalDocument.isEmpty()) {
            throw new RuntimeException("Document not found with ID: " + documentId);
        }

        Document document = optionalDocument.get();

        // Check if document has exceeded max retry attempts
        if (document.getSentCount() >= maxSendRetries) {
            logger.warn("Document {} has exceeded max send retries ({})", documentId, maxSendRetries);
            throw new RuntimeException("Document has exceeded maximum send attempts (" + maxSendRetries + ")");
        }

        boolean emailSent = emailService.sendDocument(document);

        if (emailSent) {
            document.incrementSentCount();
            document.setStatus(document.getSentCount() > 1 ? DocumentStatus.RESENT : DocumentStatus.SENT);
            logger.info("Document {} sent successfully (attempt {})", documentId, document.getSentCount());
        } else {
            document.setStatus(DocumentStatus.FAILED);
            logger.error("Failed to send document {}", documentId);
        }

        Document updatedDocument = documentRepository.save(document);
        return convertToResponse(updatedDocument);
    }

    /**
     * Send again functionality - resend a document
     * @param documentId ID of the document to resend
     * @return DocumentResponse with updated status
     */
    public DocumentResponse sendAgain(Long documentId) {
        logger.info("Attempting to send document {} again", documentId);
        return sendDocument(documentId); // Reuse the send logic
    }

    /**
     * Send again with updated details
     * @param documentId ID of the document to resend
     * @param request Updated document details
     * @return DocumentResponse with updated status
     */
    public DocumentResponse sendAgainWithUpdates(Long documentId, DocumentRequest request) {
        Optional<Document> optionalDocument = documentRepository.findById(documentId);
        
        if (optionalDocument.isEmpty()) {
            throw new RuntimeException("Document not found with ID: " + documentId);
        }

        Document document = optionalDocument.get();

        // Update document details if provided
        if (request.getRecipientEmail() != null) {
            document.setRecipientEmail(request.getRecipientEmail());
        }
        if (request.getSenderEmail() != null) {
            document.setSenderEmail(request.getSenderEmail());
        }
        if (request.getSubject() != null) {
            document.setSubject(request.getSubject());
        }
        if (request.getMessage() != null) {
            document.setMessage(request.getMessage());
        }

        // Save updates first
        document = documentRepository.save(document);
        
        // Then attempt to send
        logger.info("Attempting to send document {} again with updated details", documentId);
        return sendDocument(documentId);
    }

    /**
     * Bulk send again for multiple documents
     * @param documentIds List of document IDs to resend
     * @return List of DocumentResponse with updated statuses
     */
    public List<DocumentResponse> bulkSendAgain(List<Long> documentIds) {
        logger.info("Attempting bulk send again for {} documents", documentIds.size());
        
        return documentIds.stream()
                .map(this::sendAgain)
                .collect(Collectors.toList());
    }

    /**
     * Get all documents
     * @return List of all documents
     */
    public List<DocumentResponse> getAllDocuments() {
        List<Document> documents = documentRepository.findAll();
        return documents.stream()
                .map(this::convertToResponse)
                .collect(Collectors.toList());
    }

    /**
     * Get document by ID
     * @param documentId ID of the document
     * @return DocumentResponse
     */
    public DocumentResponse getDocumentById(Long documentId) {
        Optional<Document> optionalDocument = documentRepository.findById(documentId);
        
        if (optionalDocument.isEmpty()) {
            throw new RuntimeException("Document not found with ID: " + documentId);
        }

        return convertToResponse(optionalDocument.get());
    }

    /**
     * Get documents by status
     * @param status Document status
     * @return List of documents with the specified status
     */
    public List<DocumentResponse> getDocumentsByStatus(DocumentStatus status) {
        List<Document> documents = documentRepository.findByStatus(status);
        return documents.stream()
                .map(this::convertToResponse)
                .collect(Collectors.toList());
    }

    /**
     * Get failed documents that can be retried
     * @return List of failed documents that haven't exceeded max retries
     */
    public List<DocumentResponse> getRetryableFailedDocuments() {
        List<Document> documents = documentRepository.findAll().stream()
                .filter(doc -> doc.getStatus() == DocumentStatus.FAILED && doc.getSentCount() < maxSendRetries)
                .collect(Collectors.toList());
        
        return documents.stream()
                .map(this::convertToResponse)
                .collect(Collectors.toList());
    }

    /**
     * Retry all failed documents
     * @return List of DocumentResponse with updated statuses
     */
    public List<DocumentResponse> retryAllFailedDocuments() {
        List<Document> failedDocuments = documentRepository.findByStatus(DocumentStatus.FAILED).stream()
                .filter(doc -> doc.getSentCount() < maxSendRetries)
                .collect(Collectors.toList());

        logger.info("Retrying {} failed documents", failedDocuments.size());

        return failedDocuments.stream()
                .map(doc -> sendAgain(doc.getId()))
                .collect(Collectors.toList());
    }

    /**
     * Delete a document
     * @param documentId ID of the document to delete
     */
    public void deleteDocument(Long documentId) {
        if (!documentRepository.existsById(documentId)) {
            throw new RuntimeException("Document not found with ID: " + documentId);
        }
        
        documentRepository.deleteById(documentId);
        logger.info("Deleted document with ID: {}", documentId);
    }

    /**
     * Convert Document entity to DocumentResponse DTO
     * @param document Document entity
     * @return DocumentResponse DTO
     */
    private DocumentResponse convertToResponse(Document document) {
        DocumentResponse response = new DocumentResponse();
        response.setId(document.getId());
        response.setName(document.getName());
        response.setFilePath(document.getFilePath());
        response.setRecipientEmail(document.getRecipientEmail());
        response.setSenderEmail(document.getSenderEmail());
        response.setSubject(document.getSubject());
        response.setMessage(document.getMessage());
        response.setStatus(document.getStatus());
        response.setSentCount(document.getSentCount());
        response.setCreatedAt(document.getCreatedAt());
        response.setLastSentAt(document.getLastSentAt());
        response.setUpdatedAt(document.getUpdatedAt());
        return response;
    }
}