var Common = function () {

    var siteRoot = '';
    var isReportsNotAvailable = false;

    var reportCenterUrl = '';

    return {
        Init: function () {
            var uiSettings = DmUiLibrary.GetUiSettings();
            siteRoot = uiSettings.SiteRoot;

            reportCenterUrl = $('.library-iframe').prop('src');

            this.InitMessageReceiver();

            this.InitNavTabButtons();

            this.InitReportViewerButtonsHandlers();
            this.InitIframeLoad();
            this.InitReportingKeyButton();
        },

        InitMessageReceiver: function () {
            var self = this;

            $(window).on('message onmessage', function (e) {
                var message = e.originalEvent.data;
                var url;

                switch (message.action) {
                    case 'RegenerateReport':
                        url = '/Report/UpdateOptions';
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
        },

        RunReport: function (callGetOptions) {
            $.ajax({
                type: 'POST',
                url: siteRoot + '/Report/GetReportViewerData',
                dataType: 'json',
                beforeSend: function () {
                    DmUiLibrary.ShowOverlaySpinner();
                },
                success: function (data) {
                    if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        $('.library-iframe').prop('src', '');

                        $('#last-name-search').data('visible', data.HasLastNameSearch);
                        $('#export-to-excel-button').data('visible', data.HasExportToExcel);
                        $('#actuatePostForm').attr('action', data.QueryString).submit();

                        if (callGetOptions)
                            Options.GetOptions(false);
                    }
                }
            });
        },

        GetExcelReportString: function () {
            $.ajax({
                type: 'POST',
                url: siteRoot + '/Report/GetExcelReportString',
                data: JSON.stringify({ queryString: $('#actuatePostForm').attr('action') }),
                dataType: 'json',
                success: function (data) {
                    $('#exportExcelForm').prop('action', data).submit();
                }
            });
        },

        ApplyLastNameSearch: function () {
            $.ajax({
                type: 'POST',
                url: siteRoot + '/Report/GetLastNameSearchString',
                dataType: 'json',
                data: JSON.stringify({ lastName: $('#last-name-search input').val() }),
                success: function (data) {
                    $('#actuatePostForm').attr('action', data.QueryString).submit();
                }
            });
        },

        CloseReportViewer: function () {
            $('.library-iframe').prop('src', reportCenterUrl);

            $('.report-viewer-modal-container').fadeOut();
            $('#last-name-search-text').val('');
        },

        InitReportViewerButtonsHandlers: function () {
            var self = this;

            $('#last-name-search-text').on('enterkeyup', function () {
                $('#last-name-search-button').trigger('click');
            });

            $('#last-name-search-button').on('click', function () {
                self.ApplyLastNameSearch();
            });

            $('#export-to-excel-button').on('click', function () {
                self.GetExcelReportString();
            });

            $('#edit-report-button').on('click', function () {
                DmUiLibrary.ShowOverlaySpinner();
                self.CloseReportViewer();
                window.location = siteRoot + '/Options';
            });
            $('#edit-report-button-data-export').on('click', function () {
                DmUiLibrary.ShowOverlaySpinner();
                self.CloseReportViewer();
                window.location = siteRoot + '/Options';
            });
            $("#AfterTimeout").on('click', function () {
                $("#AfterTimeout").hide();
                DashBoardReportCeneter.TimeoutClear();
                $('#gridCompleted').show();
                DashBoardReportCeneter.GetReportCenter();

            })
            $('#close-button').on('click', function () {
                self.CloseReportViewer();
                //the following line deals with an issue when an Excel generation request was sent to Actuate and the Report Viewer is closed before getting the Excel file back
                window.location = window.location;
            });
            $('#close-button-data-export').on('click', function () {
                self.CloseReportViewer();
                //the following line deals with an issue when an Excel generation request was sent to Actuate and the Report Viewer is closed before getting the Excel file back
                window.location = window.location;
            });
        },

        InitIframeLoad: function () {
            $('.report-iframe').on('load', function () {
                if ($('#actuatePostForm').attr('action') !== "") {
                    DmUiLibrary.HideOverlaySpinner();
                    $('.report-viewer-modal-container').fadeIn(400, function () {
                        $('#hdrReportViewer').focus();

                        if ($('#export-to-excel-button').data('visible') === false)
                            $('.export-to-excel-controls').hide();
                        else
                            $('.export-to-excel-controls').show();

                        if ($('#last-name-search').data('visible') === false)
                            $('#last-name-search').hide();
                        else
                            $('#last-name-search').show();
                    });
                }
            });
        },

        InitNavTabButtons: function () {
            $('.dm-ui-tab-nav li:not(.dm-ui-selected) a').on('click', function () {
                DmUiLibrary.ShowOverlaySpinner();
            });
        },

        InitReportingKeyButton: function () {
            var self = this;

            $('#display-reporting-key-modal-button').on('click', function () {
                $.ajax({
                    type: 'GET',
                    url: siteRoot + '/Utility/DisplayReportingKeyModal',
                    beforeSend: function () {
                        DmUiLibrary.ShowOverlaySpinner();
                    },
                    success: function (data) {
                        self.AssignReportingKeyModalHandlers(data);
                    },
                    complete: function () {
                        DmUiLibrary.HideOverlaySpinner();
                    }
                });
            });
        },

        AssignReportingKeyModalHandlers: function (html) {
            var self = this;

            $('#modal-placeholder').html(html);
            $('#modal-placeholder').attr('aria-label', 'Add a Reporting Key');
            $('#reporting-key-text').focus();


            $('#reporting-key-close-button').on('click', function () {
                $('#modal-placeholder').empty();
            });

            $('#reporting-key-text').on("keydown", function (e) {
                if (e.keyCode == 9 && e.shiftKey) {
                    $('#reporting-key-add-button').focus();
                    return false;
                }
            });

            $('#reporting-key-add-button').on("keydown", function (e) {
                if (e.keyCode == 9 && !e.shiftKey) {
                    $('#reporting-key-text').focus();
                    return false;
                }
            });

            $('#reporting-key-add-button').on('click', function () {
                self.CallReportingKeyService();
            });
        },

        CallReportingKeyService: function () {
            var self = this;

            $.ajax({
                type: 'POST',
                url: siteRoot + '/Utility/AddReportingKey',
                data: JSON.stringify({ reportingKey: $('#reporting-key-text').val() }),
                beforeSend: function () {
                    DmUiLibrary.ShowOverlaySpinner();
                    $('#modal-placeholder').empty();
                },
                success: function (data) {
                    if (data.includes('reporting-key-modal-body')) {
                        self.AssignReportingKeyModalHandlers(data);
                    }
                    else if (data.includes('dm-ui-alert-success')) {
                        DmUiLibrary.DisplayAlert(data, this.dataType);

                        if ($('.options-page-section').length > 0) {
                            $('#change-main-location-link').show();
                            Options.DisplayLocationChangeModal();
                            Options.GetOptions(true);
                        }
                    }
                    else {
                        DmUiLibrary.DisplayAlert(data, this.dataType);
                    }
                },
                complete: function () {
                    DmUiLibrary.HideOverlaySpinner();
                }
            });

            if (self.isReportsNotAvailable) {
                window.location.href = siteRoot + '/Utility/WebReports?reprocess=1';
            }
        }
    };
}();