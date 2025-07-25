package com.example.documentsender.controller;

import com.example.documentsender.dto.DocumentRequest;
import com.example.documentsender.dto.DocumentResponse;
import com.example.documentsender.entity.DocumentStatus;
import com.example.documentsender.service.DocumentService;
import com.example.documentsender.service.EmailService;
import jakarta.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Map;

@RestController
@RequestMapping("/api/documents")
@CrossOrigin(origins = "*")
public class DocumentController {

    @Autowired
    private DocumentService documentService;

    @Autowired
    private EmailService emailService;

    /**
     * Create a new document
     */
    @PostMapping
    public ResponseEntity<DocumentResponse> createDocument(@Valid @RequestBody DocumentRequest request) {
        try {
            DocumentResponse response = documentService.createDocument(request);
            return ResponseEntity.status(HttpStatus.CREATED).body(response);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.BAD_REQUEST).build();
        }
    }

    /**
     * Send a document
     */
    @PostMapping("/{id}/send")
    public ResponseEntity<DocumentResponse> sendDocument(@PathVariable Long id) {
        try {
            DocumentResponse response = documentService.sendDocument(id);
            return ResponseEntity.ok(response);
        } catch (RuntimeException e) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).build();
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    /**
     * Send again functionality - Main endpoint for resending documents
     */
    @PostMapping("/{id}/send-again")
    public ResponseEntity<DocumentResponse> sendAgain(@PathVariable Long id) {
        try {
            DocumentResponse response = documentService.sendAgain(id);
            return ResponseEntity.ok(response);
        } catch (RuntimeException e) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).build();
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    /**
     * Send again with updated details
     */
    @PutMapping("/{id}/send-again")
    public ResponseEntity<DocumentResponse> sendAgainWithUpdates(
            @PathVariable Long id,
            @Valid @RequestBody DocumentRequest request) {
        try {
            DocumentResponse response = documentService.sendAgainWithUpdates(id, request);
            return ResponseEntity.ok(response);
        } catch (RuntimeException e) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).build();
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    /**
     * Bulk send again for multiple documents
     */
    @PostMapping("/bulk-send-again")
    public ResponseEntity<List<DocumentResponse>> bulkSendAgain(@RequestBody List<Long> documentIds) {
        try {
            List<DocumentResponse> responses = documentService.bulkSendAgain(documentIds);
            return ResponseEntity.ok(responses);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    /**
     * Retry all failed documents
     */
    @PostMapping("/retry-failed")
    public ResponseEntity<List<DocumentResponse>> retryFailedDocuments() {
        try {
            List<DocumentResponse> responses = documentService.retryAllFailedDocuments();
            return ResponseEntity.ok(responses);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    /**
     * Get all documents
     */
    @GetMapping
    public ResponseEntity<List<DocumentResponse>> getAllDocuments() {
        try {
            List<DocumentResponse> responses = documentService.getAllDocuments();
            return ResponseEntity.ok(responses);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    /**
     * Get document by ID
     */
    @GetMapping("/{id}")
    public ResponseEntity<DocumentResponse> getDocumentById(@PathVariable Long id) {
        try {
            DocumentResponse response = documentService.getDocumentById(id);
            return ResponseEntity.ok(response);
        } catch (RuntimeException e) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).build();
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    /**
     * Get documents by status
     */
    @GetMapping("/status/{status}")
    public ResponseEntity<List<DocumentResponse>> getDocumentsByStatus(@PathVariable DocumentStatus status) {
        try {
            List<DocumentResponse> responses = documentService.getDocumentsByStatus(status);
            return ResponseEntity.ok(responses);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    /**
     * Get failed documents that can be retried
     */
    @GetMapping("/retryable-failed")
    public ResponseEntity<List<DocumentResponse>> getRetryableFailedDocuments() {
        try {
            List<DocumentResponse> responses = documentService.getRetryableFailedDocuments();
            return ResponseEntity.ok(responses);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    /**
     * Delete a document
     */
    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteDocument(@PathVariable Long id) {
        try {
            documentService.deleteDocument(id);
            return ResponseEntity.noContent().build();
        } catch (RuntimeException e) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).build();
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    /**
     * Test email configuration
     */
    @PostMapping("/test-email")
    public ResponseEntity<Map<String, String>> testEmail(@RequestBody Map<String, String> request) {
        try {
            String testEmail = request.get("email");
            if (testEmail == null || testEmail.trim().isEmpty()) {
                return ResponseEntity.badRequest()
                    .body(Map.of("error", "Email address is required"));
            }

            boolean success = emailService.sendTestEmail(testEmail);
            if (success) {
                return ResponseEntity.ok(Map.of("message", "Test email sent successfully"));
            } else {
                return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                    .body(Map.of("error", "Failed to send test email"));
            }
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body(Map.of("error", "Unexpected error: " + e.getMessage()));
        }
    }

    /**
     * Get application health and statistics
     */
    @GetMapping("/stats")
    public ResponseEntity<Map<String, Object>> getStats() {
        try {
            List<DocumentResponse> allDocuments = documentService.getAllDocuments();
            
            long totalDocuments = allDocuments.size();
            long sentDocuments = allDocuments.stream()
                .filter(doc -> doc.getStatus() == DocumentStatus.SENT || doc.getStatus() == DocumentStatus.RESENT)
                .count();
            long failedDocuments = allDocuments.stream()
                .filter(doc -> doc.getStatus() == DocumentStatus.FAILED)
                .count();
            long pendingDocuments = allDocuments.stream()
                .filter(doc -> doc.getStatus() == DocumentStatus.PENDING)
                .count();

            Map<String, Object> stats = Map.of(
                "totalDocuments", totalDocuments,
                "sentDocuments", sentDocuments,
                "failedDocuments", failedDocuments,
                "pendingDocuments", pendingDocuments,
                "retryableFailedDocuments", documentService.getRetryableFailedDocuments().size()
            );

            return ResponseEntity.ok(stats);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }
}