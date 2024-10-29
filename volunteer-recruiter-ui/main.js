import { app, BrowserWindow } from 'electron';
import path from 'path';
import isDev from 'electron-is-dev';


/**
 * Creates the main application window and sets development/production settings.
 * 
 * The window loads a URL during development or the `index.html` file for production.
 */
function createWindow() {
  const win = new BrowserWindow({
    width: 1024,
    height: 768,
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false,
    },
  });

  // Load URL for development or production
  win.loadURL(
    isDev
      ? 'http://localhost:3000'  // Development URL
      : `file://${path.join(__dirname, 'build', 'index.html')}` // Production URL
  );

  // Open Developer Tools for debugging
  win.webContents.openDevTools();

  // Handle loading errors
  win.webContents.on('did-fail-load', (event, errorCode, errorDescription) => {
    console.error(`Failed to load: ${errorDescription} (Error code: ${errorCode})`);
  });
}

app.whenReady().then(() => {
  createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      createWindow();
    }
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});
