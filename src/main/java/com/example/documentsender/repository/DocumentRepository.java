package com.example.documentsender.repository;

import com.example.documentsender.entity.Document;
import com.example.documentsender.entity.DocumentStatus;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;

@Repository
public interface DocumentRepository extends JpaRepository<Document, Long> {

    List<Document> findByStatus(DocumentStatus status);

    List<Document> findByRecipientEmail(String recipientEmail);

    List<Document> findBySenderEmail(String senderEmail);

    @Query("SELECT d FROM Document d WHERE d.lastSentAt BETWEEN :startDate AND :endDate")
    List<Document> findDocumentsSentBetween(@Param("startDate") LocalDateTime startDate, 
                                          @Param("endDate") LocalDateTime endDate);

    @Query("SELECT d FROM Document d WHERE d.sentCount > :count")
    List<Document> findDocumentsWithSentCountGreaterThan(@Param("count") Integer count);

    @Query("SELECT d FROM Document d WHERE d.status = :status AND d.sentCount >= :maxRetries")
    List<Document> findFailedDocumentsWithMaxRetries(@Param("status") DocumentStatus status, 
                                                    @Param("maxRetries") Integer maxRetries);
}