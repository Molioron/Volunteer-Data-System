import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App.jsx';


/**
 * `index.js` is the entry point of the React application, rendering the `App` component into the root element.
 * 
 * The `App` component is wrapped in `React.StrictMode` to enable checks for potential issues.
 */
const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);