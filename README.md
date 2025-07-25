# Document Sender - Spring Boot Application

A Spring Boot application that provides "Send Again" functionality for documents via email.

## Features

- **Document Management**: Create, read, update, and delete documents
- **Send Again Functionality**: Resend documents with tracking
- **Email Integration**: Send documents as email attachments
- **Status Tracking**: Track document sending status (PENDING, SENT, FAILED, RESENT)
- **Retry Logic**: Automatic retry capability with configurable max attempts
- **Bulk Operations**: Send multiple documents at once
- **Statistics**: View application statistics and document counts

## Technology Stack

- **Framework**: Spring Boot 3.2.0
- **Database**: H2 (development), MySQL (production)
- **Email**: Spring Mail with SMTP
- **Build Tool**: Maven
- **Java Version**: 17+

## Setup Instructions

### 1. Clone and Build

```bash
git clone <repository-url>
cd document-sender
mvn clean install
```

### 2. Configure Email Settings

Update `src/main/resources/application.properties`:

```properties
# Email Configuration
spring.mail.host=smtp.gmail.com
spring.mail.port=587
spring.mail.username=your-email@gmail.com
spring.mail.password=your-app-password
```

For Gmail, you'll need to:
1. Enable 2-factor authentication
2. Generate an App Password
3. Use the App Password in the configuration

### 3. Run the Application

```bash
mvn spring-boot:run
```

The application will start on `http://localhost:8080`

### 4. Access H2 Console (Development)

- URL: `http://localhost:8080/h2-console`
- JDBC URL: `jdbc:h2:mem:testdb`
- Username: `sa`
- Password: (leave blank)

## API Endpoints

### Document Management

#### Create Document
```
POST /api/documents
Content-Type: application/json

{
  "name": "Invoice.pdf",
  "filePath": "/path/to/invoice.pdf",
  "recipientEmail": "recipient@example.com",
  "senderEmail": "sender@example.com",
  "subject": "Your Invoice",
  "message": "Please find attached invoice."
}
```

#### Get All Documents
```
GET /api/documents
```

#### Get Document by ID
```
GET /api/documents/{id}
```

#### Get Documents by Status
```
GET /api/documents/status/{status}
```
Status values: `PENDING`, `SENT`, `FAILED`, `RESENT`

### Send Again Functionality

#### Send Again (Primary Feature)
```
POST /api/documents/{id}/send-again
```

#### Send Again with Updated Details
```
PUT /api/documents/{id}/send-again
Content-Type: application/json

{
  "recipientEmail": "new-recipient@example.com",
  "subject": "Updated Subject",
  "message": "Updated message"
}
```

#### Bulk Send Again
```
POST /api/documents/bulk-send-again
Content-Type: application/json

[1, 2, 3, 4, 5]
```

#### Retry All Failed Documents
```
POST /api/documents/retry-failed
```

#### Get Retryable Failed Documents
```
GET /api/documents/retryable-failed
```

### Utility Endpoints

#### Send Document
```
POST /api/documents/{id}/send
```

#### Test Email Configuration
```
POST /api/documents/test-email
Content-Type: application/json

{
  "email": "test@example.com"
}
```

#### Get Statistics
```
GET /api/documents/stats
```

Response:
```json
{
  "totalDocuments": 10,
  "sentDocuments": 7,
  "failedDocuments": 2,
  "pendingDocuments": 1,
  "retryableFailedDocuments": 2
}
```

#### Delete Document
```
DELETE /api/documents/{id}
```

## Usage Examples

### Example 1: Basic Send Again

1. Create a document:
```bash
curl -X POST http://localhost:8080/api/documents \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Contract.pdf",
    "filePath": "/documents/contract.pdf",
    "recipientEmail": "client@example.com",
    "subject": "Contract Document"
  }'
```

2. Send the document:
```bash
curl -X POST http://localhost:8080/api/documents/1/send
```

3. Send again if needed:
```bash
curl -X POST http://localhost:8080/api/documents/1/send-again
```

### Example 2: Send Again with Updates

```bash
curl -X PUT http://localhost:8080/api/documents/1/send-again \
  -H "Content-Type: application/json" \
  -d '{
    "recipientEmail": "new-client@example.com",
    "subject": "Updated Contract Document",
    "message": "Please find the updated contract attached."
  }'
```

### Example 3: Bulk Retry

```bash
curl -X POST http://localhost:8080/api/documents/bulk-send-again \
  -H "Content-Type: application/json" \
  -d '[1, 2, 3]'
```

## Configuration

### Application Properties

Key configuration options:

```properties
# Maximum retry attempts
app.max-send-retries=5

# Email settings
spring.mail.host=smtp.gmail.com
spring.mail.port=587
spring.mail.username=your-email@gmail.com
spring.mail.password=your-app-password

# Database settings
spring.datasource.url=jdbc:h2:mem:testdb
spring.jpa.hibernate.ddl-auto=create-drop
```

### Production Deployment

For production, update these settings:

1. **Database**: Switch to MySQL or PostgreSQL
2. **Email**: Configure production SMTP server
3. **Security**: Add Spring Security for authentication
4. **File Storage**: Configure persistent file storage

## Document Status Flow

```
PENDING → SENT → RESENT (on subsequent sends)
PENDING → FAILED (on send failure)
FAILED → SENT/RESENT (on successful retry)
```

## Error Handling

The application handles various error scenarios:

- **File not found**: Email sent without attachment, warning logged
- **Email failure**: Status marked as FAILED, can be retried
- **Max retries exceeded**: Prevents infinite retry loops
- **Invalid document ID**: Returns 404 Not Found

## Logging

The application provides comprehensive logging:

- **INFO**: Document operations and email sending
- **WARN**: File not found warnings
- **ERROR**: Email sending failures
- **DEBUG**: SMTP communication details

## Testing

Run tests with:
```bash
mvn test
```

For manual testing, use the `/api/documents/test-email` endpoint to verify email configuration.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## License

This project is licensed under the MIT License.