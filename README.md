# DataManager - Web Reporting

Web Reporting module of DataManager.

## .NET Configurations.
Targeted .NET Framework version 4.7.2.

## Deployment / Server Set Up.

### Install .NET 4.7.2.
### Managed Oracle Data Access - set up TNS_ADMIN system variable and point it to the folder where tnsnames.ora resides.
### Run EventLog script.
### Set up MIME types in IIS.
1. Open IIS on the server.
1. Click on the top level (such as ODAWEBRPT01).
1. In the icons click on MIME Types.
1. In the top right corner click "Add...".
1. File name extension: **svg**, MIME type: **image/svg+xml**
1. File name extension: **woff**, MIME type: **application/font-woff**
1. File name extension: **woff2**, MIME type: **application/font-woff**

### Create 'Downloads' virtual directory in IIS.


If you see an error of pdf converting on Windows Server 2008 R2, when you print Dashboard reports, than try to copy such 2 DLL's to folder **Rotativa** where file **wkhtmltopdf.exe** is placed.
**msvcp140.dll**, **vcruntime140.dll**
