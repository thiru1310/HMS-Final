#!/bin/bash

# Document Sender Demo Script
# This script demonstrates the "Send Again" functionality

BASE_URL="http://localhost:8080/api/documents"

echo "=== Document Sender Demo ==="
echo "Make sure the application is running on http://localhost:8080"
echo ""

# Check if server is running
echo "1. Checking server status..."
if curl -s $BASE_URL/stats > /dev/null; then
    echo "✓ Server is running"
else
    echo "✗ Server is not running. Please start the application first."
    exit 1
fi
echo ""

# Get all documents
echo "2. Getting all documents..."
curl -s $BASE_URL | jq '.'
echo ""

# Get statistics
echo "3. Getting application statistics..."
curl -s $BASE_URL/stats | jq '.'
echo ""

# Create a new document
echo "4. Creating a new document..."
NEW_DOC=$(curl -s -X POST $BASE_URL \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Demo_Document.pdf",
    "filePath": "/tmp/demo_document.pdf",
    "recipientEmail": "demo@example.com",
    "senderEmail": "sender@example.com",
    "subject": "Demo Document",
    "message": "This is a demo document for testing send again functionality."
  }')

DOC_ID=$(echo $NEW_DOC | jq -r '.id')
echo "Created document with ID: $DOC_ID"
echo $NEW_DOC | jq '.'
echo ""

# Send the document (first time)
echo "5. Sending document for the first time..."
SENT_RESULT=$(curl -s -X POST $BASE_URL/$DOC_ID/send)
echo $SENT_RESULT | jq '.'
echo ""

# Send again (demonstrating the main feature)
echo "6. *** SEND AGAIN - Main Feature ***"
echo "Sending the document again..."
RESENT_RESULT=$(curl -s -X POST $BASE_URL/$DOC_ID/send-again)
echo $RESENT_RESULT | jq '.'
echo ""

# Send again with updated details
echo "7. Send again with updated recipient..."
UPDATED_RESULT=$(curl -s -X PUT $BASE_URL/$DOC_ID/send-again \
  -H "Content-Type: application/json" \
  -d '{
    "recipientEmail": "updated-recipient@example.com",
    "subject": "Updated Demo Document",
    "message": "This document has been resent with updated details."
  }')
echo $UPDATED_RESULT | jq '.'
echo ""

# Get failed documents (for retry demonstration)
echo "8. Getting failed documents that can be retried..."
curl -s $BASE_URL/retryable-failed | jq '.'
echo ""

# Bulk send again example
echo "9. Demonstrating bulk send again..."
curl -s -X POST $BASE_URL/bulk-send-again \
  -H "Content-Type: application/json" \
  -d "[$DOC_ID]" | jq '.'
echo ""

# Final statistics
echo "10. Final statistics..."
curl -s $BASE_URL/stats | jq '.'
echo ""

echo "=== Demo Complete ==="
echo "Key Send Again Features Demonstrated:"
echo "- ✓ Basic send again: POST /api/documents/{id}/send-again"
echo "- ✓ Send again with updates: PUT /api/documents/{id}/send-again"
echo "- ✓ Bulk send again: POST /api/documents/bulk-send-again"
echo "- ✓ Retry failed documents: GET /api/documents/retryable-failed"
echo "- ✓ Status tracking: PENDING → SENT → RESENT"
echo ""