import React, { useState } from "react";
import { BrowserRouter as Router } from "react-router-dom";
import { ElevateHeader, useCachedUser } from '@riversideinsights/elevate-react-lib';
//import "@riversideinsigts/elevate-react-lib/dist/assets/css/components/AppMenu/AppMenu.scss";
import "../assets/css/global-styles.scss";
import "../assets/css/HeaderProctor.scss";
//import '@riversideinsights/elevate-react-lib/dist/elevate-react-lib.css';

function ReportingHeader(props) {
    const user = useCachedUser();
    //const appStatus = useSelector((state) => {
    //    return state.appStatus;
    //});



    return (
        <Router>
        <ElevateHeader
            loggedIn={true}
            loadComplete={true}
            loadingUser={false}
            appBusy={true}
            headerVisible={true}
            noHeaderPage={false}
            userRole={user?.role || ''}
            districtName={user?.districtName || ''}
            toolbarVisible={false}
        >
            <div key="headerTitle" className="app-header-section app-header-title">
                <span className="app-header-title-section">Elevate</span>
            </div>
            </ElevateHeader>
            </Router>
    );
};


export default ReportingHeader;
