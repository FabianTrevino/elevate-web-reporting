import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';

import App from './App';
import * as serviceWorker from './serviceWorker';
import { createTheme, responsiveFontSizes } from '@material-ui/core/styles';

import { ThemeProvider } from 'styled-components';

let elevateTheme = createTheme({
  palette: {
    primary: {
      main: '#096DA9',
      dark: '#147CBD',
      light: '#1E7EB8',
      contrastText: '#FFFFFF',
    },
  },
});

elevateTheme = responsiveFontSizes(elevateTheme);

ReactDOM.render(
  <BrowserRouter>
    <ThemeProvider theme={elevateTheme}>
      <App />
    </ThemeProvider>
  </BrowserRouter>,
  document.getElementById('root'),
);

serviceWorker.unregister();
