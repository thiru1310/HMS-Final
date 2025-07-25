import React, { useState } from 'react';
import './SendAgainDocumentButton.css';

const SendAgainDocumentButton = ({ 
  document, 
  onSendAgain, 
  disabled = false,
  loading = false,
  className = '',
  children
}) => {
  const [isLoading, setIsLoading] = useState(false);

  const handleSendAgain = async () => {
    if (disabled || isLoading || loading) return;

    setIsLoading(true);
    
    try {
      if (onSendAgain) {
        await onSendAgain(document);
      }
    } catch (error) {
      console.error('Error sending document again:', error);
      // Handle error appropriately
    } finally {
      setIsLoading(false);
    }
  };

  const isButtonDisabled = disabled || isLoading || loading;

  return (
    <button
      className={`send-again-btn ${className} ${isButtonDisabled ? 'disabled' : ''}`}
      onClick={handleSendAgain}
      disabled={isButtonDisabled}
      type="button"
      aria-label="Send document again"
    >
      {isLoading || loading ? (
        <span className="loading-content">
          <span className="spinner"></span>
          Sending...
        </span>
      ) : (
        children || (
          <span className="button-content">
            <svg className="send-icon" width="16" height="16" viewBox="0 0 24 24" fill="none">
              <path 
                d="M2 21L23 12L2 3V10L17 12L2 14V21Z" 
                fill="currentColor"
              />
            </svg>
            Send Again
          </span>
        )
      )}
    </button>
  );
};

export default SendAgainDocumentButton;