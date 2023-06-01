import React, { 
  useMemo, 
  // useRef,
} from 'react';

import { 
  ElevateHeader, 
  useStaffUser,
} from '@riversideinsights/elevate-react-lib';

import MenuItem from '@material-ui/core/MenuItem';

const AppHeader: React.FunctionComponent = () => {

  const user = useStaffUser(true);

  const goToLogout = () => {
    window.location.href = 'https://www.dev.elevate.riverside-insights.com/administration/auth/logout';
  };

  const goToAvatar = () => {
    window.location.href = 'https://www.dev.elevate.riverside-insights.com/administration/user/dashboard/avatar';
  };

//const rightMenuItems = useMemo<React.ReactNode | React.ReactNode[] | React.ReactElement | React.ReactElement[] | Element | Element[]>(() => {
  const rightMenuItems = useMemo<React.ReactNode | React.ReactElement | Element>(() => {
    const items: React.ReactNode | React.ReactNode[] | React.ReactElement | React.ReactElement[] | Element | Element[] = [];
    if (user.loggedIn) {
      Array.prototype.push.call(
        items,
        <MenuItem className="right-menu-entry autohide-on-click" key="changeAvatar" title="Change Avatar" onClick={() => goToAvatar()}>
          <span className="right-menu-entry-text">Change Avatar</span>
          <span className="right-menu-entry-icon">
            <svg xmlns="http://www.w3.org/2000/svg">
              <path d="M9,.562a9,9,0,1,0,9,9A9,9,0,0,0,9,.562ZM9,4.046A3.194,3.194,0,1,1,5.806,7.24,3.194,3.194,0,0,1,9,4.046ZM9,16.53a6.954,6.954,0,0,1-5.317-2.475,4.046,4.046,0,0,1,3.575-2.17.888.888,0,0,1,.258.04A4.8,4.8,0,0,0,9,12.175a4.787,4.787,0,0,0,1.484-.25.888.888,0,0,1,.258-.04,4.046,4.046,0,0,1,3.575,2.17A6.954,6.954,0,0,1,9,16.53Z" />
            </svg>
          </span>
        </MenuItem>,
      );
      Array.prototype.push.call(
        items,
        <MenuItem
          className="right-menu-entry autohide-on-click"
          key="logout"
          title="Logout"
          onClick={() => {
            setTimeout(() => {
              goToLogout();
            }, 10);
          }}
        >
          <span className="right-menu-entry-text">Logout</span>
          <span className="right-menu-entry-icon">
            <svg xmlns="http://www.w3.org/2000/svg">
              <path d="M7.125,0V2.375h9.5V14.25h-9.5v2.375H19V0ZM4.75,4.75,0,8.313l4.75,3.563V9.5h9.5V7.125H4.75Z" />
            </svg>
          </span>
        </MenuItem>,
      );
    }
    return items;
  }, [
    user.role,
    user.loggedIn,
    user.loadComplete,
  ]);
  
  return (
    <ElevateHeader
      user={user}
      appBusy={false}
      headerVisible={true}
      noHeaderPage={false}
      toolbarVisible={false}
      rightMenuItems={rightMenuItems}
      elevateEnv="dev"
      appMenuProps={{
        prependDomain: 'https://www.dev.elevate.riverside-insights.com',
      }}
    >
      <div key="headerTitle" className="app-header-section app-header-title">
        <span className="app-header-title-section">
          Elevate
        </span>
      </div>
    </ElevateHeader>
  );
};

export default AppHeader;