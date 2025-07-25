package com.example.documentsender.dto;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;

public class DocumentRequest {

    @NotBlank(message = "Document name is required")
    private String name;

    @NotBlank(message = "File path is required")
    private String filePath;

    @NotBlank(message = "Recipient email is required")
    @Email(message = "Please provide a valid email address")
    private String recipientEmail;

    @Email(message = "Please provide a valid sender email address")
    private String senderEmail;

    private String subject;

    private String message;

    // Constructors
    public DocumentRequest() {}

    public DocumentRequest(String name, String filePath, String recipientEmail) {
        this.name = name;
        this.filePath = filePath;
        this.recipientEmail = recipientEmail;
    }

    // Getters and Setters
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
}