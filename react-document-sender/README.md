# React Send Again Document Button

A modern, reusable React component for sending documents again with loading states, error handling, and beautiful styling.

## Features

- ✨ Modern, responsive design
- 🔄 Loading states with spinner animation
- 🎨 Multiple style variations (primary, secondary, danger, small, large)
- ♿ Accessibility friendly
- 🚀 TypeScript ready
- 📱 Mobile responsive

## Installation

```bash
npm install
npm start
```

## Usage

### Basic Usage

```jsx
import SendAgainDocumentButton from './components/SendAgainDocumentButton';

function MyComponent() {
  const handleSendAgain = async (document) => {
    // Your send logic here
    console.log('Sending document:', document);
  };

  return (
    <SendAgainDocumentButton
      document={{ id: 1, name: 'contract.pdf' }}
      onSendAgain={handleSendAgain}
    />
  );
}
```

### With Loading State

```jsx
const [isLoading, setIsLoading] = useState(false);

<SendAgainDocumentButton
  document={document}
  onSendAgain={handleSendAgain}
  loading={isLoading}
/>
```

### Different Styles

```jsx
{/* Primary (default) */}
<SendAgainDocumentButton document={doc} onSendAgain={handler} />

{/* Secondary */}
<SendAgainDocumentButton 
  document={doc} 
  onSendAgain={handler}
  className="secondary" 
/>

{/* Danger */}
<SendAgainDocumentButton 
  document={doc} 
  onSendAgain={handler}
  className="danger" 
/>

{/* Small */}
<SendAgainDocumentButton 
  document={doc} 
  onSendAgain={handler}
  className="small" 
/>

{/* Large */}
<SendAgainDocumentButton 
  document={doc} 
  onSendAgain={handler}
  className="large" 
/>
```

### Custom Content

```jsx
<SendAgainDocumentButton
  document={document}
  onSendAgain={handleSendAgain}
>
  Resend Document
</SendAgainDocumentButton>
```

## Props

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `document` | `object` | Required | Document object to be sent |
| `onSendAgain` | `function` | Required | Callback function when button is clicked |
| `disabled` | `boolean` | `false` | Whether the button is disabled |
| `loading` | `boolean` | `false` | External loading state |
| `className` | `string` | `''` | Additional CSS classes |
| `children` | `ReactNode` | Default content | Custom button content |

## CSS Classes

- `.send-again-btn` - Base button class
- `.send-again-btn.secondary` - Secondary style
- `.send-again-btn.danger` - Danger/error style
- `.send-again-btn.small` - Small size variant
- `.send-again-btn.large` - Large size variant
- `.send-again-btn.disabled` - Disabled state

## Example Implementation

The component handles:
- Loading states (both internal and external)
- Error handling
- Accessibility (ARIA labels, focus states)
- Responsive design
- Smooth animations

## Development

To run the development server:

```bash
npm start
```

To build for production:

```bash
npm run build
```