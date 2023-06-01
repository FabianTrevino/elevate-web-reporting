var DashBoardReportCeneter = function (appSettingValue, actuateUrl, actuteGenerateUrl, userId, password) {

    var record = 0;
    var siteRoot = '';
    var setTime = '';
    //var pageSize = 10;
    var pageSize = 5;
    var time = 0;
    var groupNumber = 0;
    /*  
     *  This library is used for report center  
     */
    return {


        Init: function (appSettingValue, actuateUrl, actuteGenerateUrl, userId, password) {

            sessionStorage.setItem("actuateurl", actuateUrl);
            sessionStorage.setItem("actuteGenerateUrl", actuteGenerateUrl);
            sessionStorage.setItem("userId", userId);
            sessionStorage.setItem("password", password);
            if (appSettingValue) {

                DmUiLibrary.HideOverlaySpinner();
                var uiSettings = DmUiLibrary.GetUiSettings();
                siteRoot = uiSettings.SiteRoot;
                this.newCompletedgrid();
                this.newtab();
                this.newgridPending();
                this.newgridRunning();
                this.GetReportCenter();
                this.SetTimeout();
                this.printDataExportter();
            }
            else {
                $('#actuateIframe').removeAttr("style");
                $('#actuateIframe').removeAttr('hidden');
                $('#tabstrip').attr('style', 'display:none');
                $('.library-iframe').on('load', function () {
                    DmUiLibrary.HideOverlaySpinner();
                });
            }
        },
        printDataExportter: function () {
            $("#printDataExporter").on('click', function () {
                $("#form-input-dataexport #input").val($("#dataExportContentValue").html().replace(/href=/g, "data-href="));
                var newPopupTarget = "print_preview_popup" + DmUiLibrary.RandomStringGenerator(5);
                $("#form-input-dataexport").attr("target", newPopupTarget);
                window.open($("#form-input-dataexport").attr("action"), newPopupTarget, "width=1340,height=700,left=0,top=5");
                $("#form-input-dataexport").submit();
            })
        },
        newCompletedgrid: function () {
            $("#gridCompleted").kendoGrid({
                height: 200,
                sortable: true,
            });
        },
        newgridPending: function () {
            $("#gridPending").kendoGrid({
                height: 750,
                sortable: true,
            });
        },
        newgridRunning: function () {
            $("#gridRunning").kendoGrid({
                height: 750,
                sortable: true,
            });
        },
        newtab: function () {
            $("#tabstrip").kendoTabStrip({
                animation: {
                    open: {
                        effects: "fadeIn"
                    }
                }
            });

        },
        TimeoutClear: function () {
            time = 0;
        },
        SetTimeout: function () {
            setTimeout(function () {
                DashBoardReportCeneter.GetReportCenter()
            }, 60000)
        },
        GetReportCenter: function () {
            var spinner = '<div class="spinner"><div class="rect1"></div><div class="rect2"></div><div class="rect3"></div><div class="rect4"></div><div class="rect5"></div></div>';
            $("#tabstrip .spinner").remove();
            $("#tabstrip").append(spinner);
            $("#tabstrip").show();

            $.ajax({
                url: siteRoot + '/DashboardCogat/GetFileByID',
                method: 'GET',
                data: {
                },
                success: function (response) {
                    $("#tabstrip .spinner").remove();
                    var setTime;
                    time += 1;
                    if (time < 5) {
                        setTime = DashBoardReportCeneter.SetTimeout();
                    }
                    else {
                        $('#gridCompleted').hide();
                        $("#AfterTimeout").show();
                    }
                    var Jobj = JSON.parse(response);
                    if (Jobj.length === 1 && Object.keys(Jobj[0]).length === 0) {
                        $("#tabstrip").hide();
                    } else {
                        $("#tabstrip").show();
                        WriteGrid(Jobj);
                    }
                },
                error: function (error) {
                    $("#tabstrip .spinner").remove();
                }
            });

            function DaysAvailable(val) {
                var date1 = new Date(val.CreatedDate);
                var date2 = new Date();
                var Difference_In_Time = date2.getTime() - date1.getTime();
                var Difference_In_Days = Difference_In_Time / (1000 * 3600 * 24);
                Difference_In_Days = 5 - Math.round(Difference_In_Days);
                if (Difference_In_Days < 1) Difference_In_Days = 1;
                return Difference_In_Days;
            }

            function WriteGrid(data) {
                var ClearCompGrid = $('#gridCompleted').html("");
                var completedJObj = data["0"].Reports;
                var objIconsCssClass = {
                    INPROGRESS: "fa-clock",
                    EVALUATING: "fa-clock",
                    RUNNING: "fa-clock",
                    COMPLETED: "fa-check",
                    FAILED: "fa-times"
                };
                var objIconsTitle = {
                    INPROGRESS: "InProgress",
                    EVALUATING: "Evaluating",
                    RUNNING: "Running",
                    COMPLETED: "Completed",
                    FAILED: "Failed"
                };
                var objIconsCssColor = {
                    INPROGRESS: "black",
                    EVALUATING: "black",
                    RUNNING: "black",
                    COMPLETED: "green",
                    FAILED: "red"
                };

                if (completedJObj !== undefined) {
                    //console.log(completedJObj);
                    var gridcomp = $("#gridCompleted").kendoGrid({
                        dataSource: {
                            data: completedJObj,
                            pageSize: pageSize
                        },
                        pageable: {
                            pageSizes: [5, 10, 25, 50],
                            numeric: false
                        },
                        resizable: true,
                        columns: [
                            {
                                field: "FileName",
                                title: "Report Name",
                                width: 145,
                                template: function (completedJObj) {
                                    var fileStatus = completedJObj.Status;
                                    var returnLinks = "";
                                    if (completedJObj.ActuateFileID == null) {
                                        var telFileName = encodeURIComponent(completedJObj.FileName);
                                        var telFilePath = encodeURIComponent(completedJObj.FilePath);
                                        returnLinks = "<!--img src='../Reskin/Content/img/pdfLogo.png' /-->" + " " + "<a href='" + siteRoot + "/DashboardCogat/GetfileforDownload?UserID=" + completedJObj.USERID + "&Filename=" + telFileName + "&Filepath=" + telFilePath + "&FileID=" + completedJObj.FileID + "&reportType=" + completedJObj.ReportType + "' target='_blank' >" + completedJObj.FileName + "</a>";
                                        if (fileStatus == "INPROGRESS") {
                                            returnLinks = "<!--img src='../Reskin/Content/img/Pending.png' width = '20' heigth = '23' /-->" + " " + "<span>" + completedJObj.FileName + "</span>";
                                        }
                                        if (fileStatus == "RUNNING") {
                                            returnLinks = "<!--img src='../Reskin/Content/img/Pending.png' width = '20' heigth = '23' /-->" + " " + "<span>" + completedJObj.FileName + "</span>";
                                        }
                                        if (fileStatus == "EVALUATING") {
                                            returnLinks = "<!--img src='../Reskin/Content/img/Pending.png' width = '20' heigth = '23' /-->" + " " + "<span>" + completedJObj.FileName + "</span>";
                                        }
                                        if (fileStatus == "FAILED") {
                                            returnLinks = "<!--img src='../Reskin/Content/img/pdffailed.png' width = '20' heigth = '23' /-->" + " " + "<span style='color: red;'>" + completedJObj.FileName + "</span>" + " " + "<span id=failedQuerySpn" + completedJObj.FileID + " hidden ='hidden'>" + completedJObj.QueryString + "</span> ";
                                        }
                                        if (completedJObj.ReportType == "CatalogExporter" && fileStatus == "COMPLETED") {
                                            //returnLinks = "<!--img src='../Reskin/Content/img/pdfLogo.png' /-->" + " " + "<a href='./DashboardCogat/GetfileforDownload?UserID=" + completedJObj.USERID + "&Filename=" + telFileName + "&FileID=" + completedJObj.FileID + "&reportType=" + completedJObj.ReportType + "' target='_blank' >" + completedJObj.FileName + "</a>";
                                            returnLinks = "<!--img src='../Reskin/Content/img/pdfLogo.png' /-->" + " " + "<a class='k-action-link-1'>" + completedJObj.FileName + "</a>";
                                        }
                                    } else {
                                        var fileName = completedJObj.ActuateFileName;
                                        var fileExt = fileName.split("/");
                                        if (fileExt[3].includes(".PDF")) {

                                            returnLinks = "<img src='../Reskin/Content/img/pdfLogo.png'  />" + " " + "<a href='" + sessionStorage.actuateurl + "servlet/DownloadFile/" + completedJObj.ActuateFileName + "?fileid=" + completedJObj.ActuateFileID + "&userid=" + sessionStorage.userId + "&password=" + sessionStorage.password + "&fileExtension=PDF&fromDashboard=true&showBanner=false&locale=en_US' target='_blank' >" + completedJObj.FileName + "</a>";
                                        }
                                        if (fileExt[3].includes(".ROI")) {

                                            returnLinks = "<img src='../Reskin/Content/img/Roi.png' width = '20' heigth = '23' />" + " " + "<a class='k-action-link' id='roiSubmit" + completedJObj.FileID + "' >" + completedJObj.FileName + "</a>";
                                        }
                                        if (fileStatus == "FAILED") {
                                            returnLinks = "<img src='../Reskin/Content/img/pdffailed.png' />" + " " + "<span style='color: red;'>" + completedJObj.FileName + "</span>";
                                        }
                                    }
                                    return returnLinks;
                                }
                            },
                            {
                                field: "CreatedDate",
                                title: "Date Created",
                                //template: "#= kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-ddTHH:mm:ss'), 'MM/dd/yyyy hh:mm:ss tt') #",
                                //template: "#= kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-ddTHH:mm:ss'), 'MM/dd/yyyy') #",
                                template: "#= kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-ddTHH:mm:ss'), 'MM/dd/yyyy hh:mm') #",
                                width: 65
                            },
                            {
                                field: "CreatedDate",
                                title: "Days Available",
                                template: DaysAvailable,
                                width: 55
                            },
                            {
                                field: "Status",
                                title: "Status",
                                //template: completedJObj.Status.replace("COMPLETED", "C"),
                                //template: "<#=(" + completedJObj.Status + " == "COMPLETED") ? 'C' : 'V' '#=" + completedJObj.Status + "#</#=(" + completedJObj.Status + " == 0) ? 'span' : 'span'#>"
                                //template: "#=(" + completedJObj.Status + " == 'COMPLETED') ? 'C' : <i class='fas fa-check'></i>#",
                                //template: "<#=(" + completedJObj.Status + " != 'COMPLETED') ? 'i' : 'i'# class='fas fa-check'></i>",
                                template: function (completedJObj) {
                                    //return completedJObj.Status;
                                    return "<i title='" + objIconsTitle[completedJObj.Status] + "' class='fas " + objIconsCssClass[completedJObj.Status] + "' style='color: " + objIconsCssColor[completedJObj.Status] + "'></i>";
                                },
                                width: 40
                            }
/*
                            ,
                            {
                                field: "FileID",
                                title: "Actions",
                                width: 35,
                                template: function (completedJObj) {
                                    var returnLinks = "";
                                    var fileStatus = completedJObj.Status;
                                    returnLinks = "<input type='image' class='ob-delete' id='Deletebutton'  src='../Reskin/Content/img/Deletelogo.png'name='Delete'/>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + " ";
                                    //if (fileStatus == "FAILED") {
                                    //    returnLinks += "<input type='image' class= 'ob-resend' id='ResendButton'  src='../Reskin/Content/img/Rerun.png'name='Resend' width = '15' heigth = '15'/>"
                                    //}
                                    //if (fileStatus == "RUNNING")
                                    //{
                                    //    returnLinks += "<p style='font-style:italic;'>"+completedJObj.EvaluationString+"</p>";
                                    //}
                                    //completedJObj.EvaluationString

                                    return returnLinks;
                                }
                            }
*/
                        ],
                        //height: 200,
                        height: 280,
                        sortable: true,
                    }).data("kendoGrid");
                    $(document).on("click", "#gridCompleted tbody tr .ob-delete", function (e) {
                        var item = gridcomp.dataItem($(this).closest("tr"));
                        softDeletefile(item.FileID);
                        gridcomp.removeRow($(this).closest("tr"));
                        //$('#divConfirmDelete').removeAttr("style");
                        //$('#btnConfirmDeleteOk').click(function ()
                        //{

                        //    $('#divConfirmDelete').attr('style', 'display:none');
                        //});
                        //$('#btnConfirmDeleteCancel').click(function ()
                        //{
                        //    $('#divConfirmDelete').attr('style', 'display:none');
                        //});


                    });
                    $(document).on("click", "#gridCompleted tbody tr .k-action-link-1", function showDataExport(e) { //work here
                      
                        $("#dataExporterReportModal").attr('style', 'display none');
                        var item = gridcomp.dataItem($(this).closest("tr"));
                        $.ajax({
                            url: siteRoot + '/DashboardCogat/GetDataExportData',
                            method: 'GET',
                            data:
                            {
                                fileID: item.FileID
                            },
                            success: function (dataResult) {
                                var dataResult = JSON.parse(dataResult);
                                getDataExportGrid(dataResult, item);
                            },
                            error: function () {

                            },
                        });
                       
                        
                    });
                    $(document).on("click", "#gridCompleted tbody tr .k-action-link", function (e) {
                        var item = gridcomp.dataItem($(this).closest("tr"));
                        var queryString = item.Parameters;
                        if (item.HasLastNameSearch == 'False') {
                            item.HasLastNameSearch = false;
                        }
                        if (item.HasLastNameSearch == 'True') {
                            item.HasLastNameSearch = true;
                        }
                        if (item.HasExportToExcel == 'False') {
                            item.HasExportToExcel = false;
                        }
                        if (item.HasExportToExcel == 'True') {
                            item.HasExportToExcel = true;
                        }
                        
                        var submitlink = "" + sessionStorage.actuteGenerateUrl + "?userid=" + sessionStorage.userId + "&password=" + sessionStorage.password + "&?criteriaID=" + item.CriteriaID + "&id=" + item.ActuateFileID + "&fileType=ROI&__report=" + item.ActuateFileName + "";
                        var updateOptionsID = "criteriaID=" + item.CriteriaID + "&id=" + item.ActuateFileID + "&closex=true&fromDashboard=true&showBanner=false&locale=en_US&fileType=ROI&__report=" + item.ActuateFileName + "";
                        DmUiLibrary.ShowOverlaySpinner();
                        $('.library-iframe').prop('src', '');
                        $('#last-name-search').data('visible', JSON.parse(item.HasLastNameSearch));
                        $('#export-to-excel-button').data('visible', JSON.parse(item.HasExportToExcel));
                        updateLibraryPrams(updateOptionsID);
                        deleteDataExporterSessionkey();
                        //updateOptions();
                        $('#actuatePostForm').attr('action', submitlink).submit();
                    });
                    $(document).on("click", ".k-list-scroller li", function (e) {
                        pageSize = $(this).text();
                    });
                    //k-widget k-dropdown
                }
                else {

                }

            }
            function softDeletefile(responseFileId) {
                $.ajax({
                    url: siteRoot + '/DashboardCogat/DeletePDF',
                    method: 'GET',
                    data:
                    {
                        FileID: responseFileId
                    },
                    success: function () {
                    },
                    error: function () {
                        alert("Error deleting file");
                    },
                });
            }
            function updateLibraryPrams(Parameters) {
                $.ajax({
                    url: siteRoot + '/Library/UpdateOptions',
                    method: 'POST',
                    data: JSON.stringify({ parameters: Parameters }),
                    success: function () {

                    },
                    error: function (e) {

                    },
                });
            }
            function updateOptions() {

                $(window).on('message onmessage', function (e) {
                    debugger;
                    var message = e.originalEvent.data;
                    var url;

                    switch (message.action) {
                        case 'RegenerateReport':
                            url = '/DashboardCogat/UpdateOptions';
                            break;
                        case 'LoadReport':
                            url = '/Library/UpdateOptions';
                            break;
                        default:
                            return;
                    }

                    $.ajax({
                        type: 'POST',
                        url: siteRoot + url,
                        data: JSON.stringify({ parameters: message.params }),
                        beforeSend: function () {
                            self.CloseReportViewer();
                            DmUiLibrary.ShowOverlaySpinner();
                        },
                        success: function () {
                            self.RunReport(true);
                        }
                    });
                });
            }
            function deleteDataExporterSessionkey() {
                //DeleteDataExporterSession
                $.ajax({
                    url: siteRoot + '/Library/DeleteDataExporterSession',
                    method: 'POST',
                    //data: JSON.stringify({ parameters: Parameters }),
                    success: function () {

                    },
                    error: function (e) {

                    },
                });

            }
            function getDataExportGrid(jsonData, item) {
               
                var ClearCompGrid = $('#fieldsAndLengths').html("");
                var dataObj = jsonData.NameAndLengths;
                var date = new Date(jsonData.TestDate);
                //var formatDate = date.getMonth().toString()+"/" + date.getDate().toString() +"/" +date.getFullYear().toString();
                $("#TestFamily").text(jsonData.TestFamily);
                $("#TestDateData").text(jsonData.TestDate);
                $("#NormsData").text(jsonData.Norms);
                var buildingLabel = jsonData.buildingLabel;
                var classLable = jsonData.classLabel;
                var disaggLabel = jsonData.disaggLabel;
                var districtLabel = jsonData.districtLabel;
                var regionLabel = jsonData.regionLabel;
                var stateLabel = jsonData.stateLabel;
                var systemLabel = jsonData.systemLabel;
                var reportGrouping = jsonData.reportGrouping;
                var groupingNumber = GetGroupNumericValue(reportGrouping);
                //if (jsonData.Building == null)
                if ((groupNumber > 2) || (buildingLabel.toUpperCase() == "SUPPRESS"))
                {
                    $("#albl").html('asdasdasdasdasdasd');
                    $("#albl").css('visibility', 'hidden');
                }
                else {
                    $("#albl").text(buildingLabel+ ":" + " " + jsonData.Building);
                }
                if ((groupNumber > 1) || (classLable.toUpperCase() == "SUPPRESS"))
                {
                    $("#blbl").html('asdasdasdasdasdasd');
                    $("#blbl").css('visibility', 'hidden');
                }
                else {
                    $("#blbl").text(classLable + ":" + " " + jsonData.Class);
                }
                if ((groupNumber > 4) || (systemLabel.toUpperCase() == "SUPPRESS"))
                {
                    $("#clbl").html('asdasdasdasdasdasd');
                    $("#clbl").css('visibility', 'hidden');
                }
                else
                {
                    $("#clbl").text(systemLabel + ":" + " " + jsonData.System);
                }
                if ((groupNumber > 5) || (regionLabel.toUpperCase() == "SUPPRESS"))
                {
                    $("#dlbl").html('asdasdasdasdasdasd');
                    $("#dlbl").css('visibility', 'hidden');
                } 
                else
                {
                    $("#dlbl").text(regionLabel + ":" + " " + jsonData.Region);
                }
                if (stateLabel.toUpperCase() == "SUPPRESS")
                {
                    $("#elbl").html('asdasdasdasdasdasd');
                    $("#elbl").css('visibility', 'hidden');
                }
                else
                {
                    $("#elbl").text(stateLabel + ":" + " " + jsonData.State);
                }
                if ((groupNumber > 3) || (districtLabel.toUpperCase() == "SUPPRESS"))
                {
                    $("#flbl").html('asdasdasdasdasdasd');
                    $("#flbl").css('visibility', 'hidden');
                }
                else
                {
                    $("#flbl").text(districtLabel + ":" + " " + jsonData.District);
                }
                var telFileName = encodeURIComponent(item.FileName);
                var telFilePath = encodeURIComponent(item.FilePath);
                $("#FileDownloadLink").replaceWith(function () {
                    var url = "<a href='" + siteRoot + "/DashboardCogat/GetfileforDownload?UserID=" + item.USERID + "&Filename=" + telFileName + "&Filepath=" + telFilePath + "&FileID=" + item.FileID + "&reportType=" + item.ReportType + "' target='_blank'  align='center' >" + item.FileName + "</a>"
                    return url;
                });
                $("#FileName").text(item.FileName);
                $("#ExportFormat").text(jsonData.ExportInfo);
                $("#Grade").text(jsonData.Grade);
                var updateOptionsID = "criteriaID=" + item.CriteriaID + "&id=" + item.ActuateFileID + "&closex=true&fromDashboard=true&showBanner=false&locale=en_US&fileType=ROI&__report=" + telFileName + "";
                updateLibraryPrams(updateOptionsID);
                deleteDataExporterSessionkey();
                var gridcomp = $("#fieldsAndLengths").kendoGrid({
                    dataSource: {
                        data: dataObj,
                        pageSize: 200
                    },
                    resizable: true,
                   
                    columns: [
                        {
                            title: "Field",
                            template: "<span class='row-number'></span>",
                            width: 35
                        },

                        {
                            field: "FieldName",
                            title: "Field Name",
                            width: 35,

                        },
                        {
                            field: "Length",
                            title: "Length",
                            width: 100,
                        },
                    ],
                    dataBound: function () {
                        var rows = this.items();
                        $(rows).each(function () {
                            var index = $(this).index() + 1
                                + ($("#fieldsAndLengths").data("kendoGrid").dataSource.pageSize() * ($("#fieldsAndLengths").data("kendoGrid").dataSource.page() - 1));;
                            var rowLabel = $(this).find(".row-number");
                            $(rowLabel).html(index);
                        });
                    }
                   
                });
               
                
               
            }
            function GetGroupNumericValue(reportGrouping) {
                
                switch (reportGrouping) {
                    case "CLASS":
                        groupNumber = 1;
                        break;
                    case "BUILDING":
                        groupNumber = 2;
                        break;
                    case "DISTRICT":
                        groupNumber = 3;
                        break;
                    case "SYSTEM":
                        groupNumber = 4;
                        break;
                    case "REGION":
                        groupNumber = 5;
                        break;
                    case "STATE":
                        groupNumber = 6;
                        break;
                    default:
                        groupNumber = 1;
                        break;
                }
            }
         


        },
      

    }
}();





