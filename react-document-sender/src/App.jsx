import React, { useState } from 'react';
import SendAgainDocumentButton from './components/SendAgainDocumentButton';
import './App.css';

function App() {
  const [documents] = useState([
    {
      id: 1,
      name: 'Contract.pdf',
      status: 'sent',
      recipient: 'john@example.com'
    },
    {
      id: 2,
      name: 'Invoice_2024.pdf',
      status: 'failed',
      recipient: 'client@company.com'
    },
    {
      id: 3,
      name: 'Report.docx',
      status: 'delivered',
      recipient: 'manager@office.com'
    }
  ]);

  const [sendingStates, setSendingStates] = useState({});

  const handleSendAgain = async (document) => {
    console.log('Sending document again:', document);
    
    setSendingStates(prev => ({ ...prev, [document.id]: true }));

    // Simulate API call
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        // Simulate success/failure
        const success = Math.random() > 0.3; // 70% success rate
        
        setSendingStates(prev => ({ ...prev, [document.id]: false }));
        
        if (success) {
          alert(`Document "${document.name}" sent successfully to ${document.recipient}!`);
          resolve();
        } else {
          alert(`Failed to send document "${document.name}". Please try again.`);
          reject(new Error('Send failed'));
        }
      }, 2000);
    });
  };

  return (
    <div className="app">
      <header className="app-header">
        <h1>Document Management System</h1>
        <p>Manage and resend your documents</p>
      </header>

      <main className="app-main">
        <div className="documents-grid">
          {documents.map(document => (
            <div key={document.id} className="document-card">
              <div className="document-info">
                <h3>{document.name}</h3>
                <p className="recipient">To: {document.recipient}</p>
                <span className={`status status-${document.status}`}>
                  {document.status.toUpperCase()}
                </span>
              </div>
              
              <div className="document-actions">
                <SendAgainDocumentButton
                  document={document}
                  onSendAgain={handleSendAgain}
                  loading={sendingStates[document.id]}
                  className={document.status === 'failed' ? 'danger' : ''}
                />
              </div>
            </div>
          ))}
        </div>

        <div className="examples-section">
          <h2>Button Variations</h2>
          <div className="button-examples">
            <SendAgainDocumentButton
              document={{ id: 'demo1', name: 'demo.pdf' }}
              onSendAgain={() => console.log('Default button clicked')}
            />
            
            <SendAgainDocumentButton
              document={{ id: 'demo2', name: 'demo.pdf' }}
              onSendAgain={() => console.log('Secondary button clicked')}
              className="secondary"
            />
            
            <SendAgainDocumentButton
              document={{ id: 'demo3', name: 'demo.pdf' }}
              onSendAgain={() => console.log('Small button clicked')}
              className="small"
            />
            
            <SendAgainDocumentButton
              document={{ id: 'demo4', name: 'demo.pdf' }}
              onSendAgain={() => console.log('Large button clicked')}
              className="large"
            />
            
            <SendAgainDocumentButton
              document={{ id: 'demo5', name: 'demo.pdf' }}
              disabled={true}
            >
              Disabled Button
            </SendAgainDocumentButton>
          </div>
        </div>
      </main>
    </div>
  );
}

export default App;