import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';

import AppHeader from './components/AppHeader/AppHeader';
import { StaffInactivityTimer } from '@riversideinsights/elevate-react-lib';

import './assets/css/App.scss';
import './assets/css/variables.scss';
import './assets/css/base.scss';
import './assets/css/forms.scss';
import '@riversideinsights/elevate-react-lib/dist/elevate-react-lib.css';

const customLogout = () => {
  window.location.href = 'https://www.dev.elevate.riverside-insights.com/administration/auth/logout';
};

const App: React.FunctionComponent = () => {
  return (
    <Router>
      <StaffInactivityTimer customLogout={customLogout} />
      {/* <StaffInactivityTimer inactivityLogoutMinutes={2} inactivityModalMinutes={1.9} customLogout={customLogout} /> */}
      <AppHeader />
    </Router>
  );
};

export default App;
