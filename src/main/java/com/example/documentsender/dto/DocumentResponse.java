package com.example.documentsender.dto;

import com.example.documentsender.entity.DocumentStatus;
import java.time.LocalDateTime;

public class DocumentResponse {

    private Long id;
    private String name;
    private String filePath;
    private String recipientEmail;
    private String senderEmail;
    private String subject;
    private String message;
    private DocumentStatus status;
    private Integer sentCount;
    private LocalDateTime createdAt;
    private LocalDateTime lastSentAt;
    private LocalDateTime updatedAt;

    // Constructors
    public DocumentResponse() {}

    public DocumentResponse(Long id, String name, String filePath, String recipientEmail, 
                          DocumentStatus status, Integer sentCount, LocalDateTime createdAt) {
        this.id = id;
        this.name = name;
        this.filePath = filePath;
        this.recipientEmail = recipientEmail;
        this.status = status;
        this.sentCount = sentCount;
        this.createdAt = createdAt;
    }

    // Getters and Setters
    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getFilePath() {
        return filePath;
    }

    public void setFilePath(String filePath) {
        this.filePath = filePath;
    }

    public String getRecipientEmail() {
        return recipientEmail;
    }

    public void setRecipientEmail(String recipientEmail) {
        this.recipientEmail = recipientEmail;
    }

    public String getSenderEmail() {
        return senderEmail;
    }

    public void setSenderEmail(String senderEmail) {
        this.senderEmail = senderEmail;
    }

    public String getSubject() {
        return subject;
    }

    public void setSubject(String subject) {
        this.subject = subject;
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    public DocumentStatus getStatus() {
        return status;
    }

    public void setStatus(DocumentStatus status) {
        this.status = status;
    }

    public Integer getSentCount() {
        return sentCount;
    }

    public void setSentCount(Integer sentCount) {
        this.sentCount = sentCount;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }

    public LocalDateTime getLastSentAt() {
        return lastSentAt;
    }

    public void setLastSentAt(LocalDateTime lastSentAt) {
        this.lastSentAt = lastSentAt;
    }

    public LocalDateTime getUpdatedAt() {
        return updatedAt;
    }

    public void setUpdatedAt(LocalDateTime updatedAt) {
        this.updatedAt = updatedAt;
    }
}