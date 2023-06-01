var Criteria = function () {

    var siteRoot = '';

    return {
        Init: function (callGetCriteria) {
            var uiSettings = DmUiLibrary.GetUiSettings();
            siteRoot = uiSettings.SiteRoot;

            if (callGetCriteria)
                this.GetCriteria();
        },

        InitDropdownsHandlers: function () {
            var self = this;
            $('#assessments-select, #report-type-select').on('change', function () {
                self.GetCriteria($('#assessments-select').val(), $('#report-type-select').val());
            });
        },

        InitCriteriaTableLinks: function () {
            var self = this;

            $('.load-criteria-link').on('enterkeyup click', function () {

                var data = JSON.stringify({
                    criteriaId: $(this).data('id'),
                    enableEditMode: true,
                    criteriaName: $(this).data('name'),
                    criteriaDescription: $(this).data('description'),
                    criteriaDate: $(this).data('date')
                });

                self.LoadCriteria(data);
            });
            $('.delete-criteria-link').on('enterkeyup click', function () {
                var criteriaId = $(this).data('id');
                $.ajax({
                    type: 'GET',
                    url: siteRoot + '/Criteria/DisplayDeleteCriteriaModal',
                    beforeSend: function () {
                        DmUiLibrary.ShowOverlaySpinner();
                    },
                    success: function (data) {
                        $('#modal-placeholder').html(data);
                        $('#modal-placeholder').attr('aria-label', 'Delete Criteria');
                        $('#cancel-delete-criteria-button').focus();

                        $('#cancel-delete-criteria-button').on('click', function () { $('#modal-placeholder').empty(); });

                        $('#cancel-delete-criteria-button').on('keydown', function (e) {
                            if (e.keyCode == 9 && e.shiftKey) {
                                $('#yes-delete-criteria-button').focus();
                                return false;
                            }
                        });

                        $('#yes-delete-criteria-button').on('keydown', function (e) {
                            if (e.keyCode == 9 && !e.shiftKey) {
                                $('#cancel-delete-criteria-button').focus();
                                return false;
                            }
                        });

                        $('#yes-delete-criteria-button').on('click', function () {
                            $('#modal-placeholder').empty();
                            self.DeleteCriteria(criteriaId);
                        });
                    },
                    complete: function () {
                        DmUiLibrary.HideOverlaySpinner();
                    }
                });
            });
        },

        DeleteCriteria: function (criteriaId) {
            var self = this;
            var params = JSON.stringify({ criteriaId: criteriaId });

            $.ajax({
                type: 'POST',
                url: siteRoot + '/Criteria/DeleteCriteria',
                data: params,
                beforeSend: function () {
                    DmUiLibrary.ShowOverlaySpinner();
                },
                success: function (data) {
                    DmUiLibrary.DisplayAlert(data, this.dataType);
                    self.GetCriteria($('#assessments-select').val(), $('#report-type-select').val());
                },
                complete: function () {
                    DmUiLibrary.HideOverlaySpinner();
                }
            });
        },

        LoadCriteria: function (data) {
            $.ajax({
                type: 'POST',
                url: siteRoot + '/Criteria/LoadCriteria',
                data: data,
                beforeSend: function () {
                    DmUiLibrary.ShowOverlaySpinner();
                },
                success: function (data) {
                    if (!DmUiLibrary.DisplayResponseAlert(data, this.dataType))
                        window.location = siteRoot + '/Options';
                }
            });
        },

        GetCriteria: function (assessmentCode, displayType) {
            var self = this;
            var data = JSON.stringify({ assessmentCode: assessmentCode, displayType: displayType });

            $.ajax({
                type: 'POST',
                url: siteRoot + '/Criteria/GetCriteria',
                data: data,
                beforeSend: function () {
                    DmUiLibrary.ShowOverlaySpinner();
                },
                success: function (data) {
                    if (DmUiLibrary.DisplayResponseAlert(data, this.dataType)) {
                        $('#criteria-place-holder').empty();
                    }
                    else {
                        $('#criteria-place-holder').html(data);

                        //for COVID reports
                        $('#report-type-select_dm_ui li[data-value=EGSR]').html($('#report-type-select_dm_ui li[data-value=EGSR]').text());

                        self.InitDropdownsHandlers();
                        self.InitCriteriaTableLinks();
                    }
                },
                complete: function () {
                    console.log('inside complete');
                    DmUiLibrary.HideOverlaySpinner();
                }
            });
        },

        SendReportToBackground: function (name) {
            $.ajax({
                type: 'POST',
                url: siteRoot + '/Report/SendReportToBackground',
                data: JSON.stringify({ name: name }),
                beforeSend: function () {
                    DmUiLibrary.ShowOverlaySpinner();
                },
                success: function (data) {
                    DmUiLibrary.DisplayAlert(data, this.dataType);
                },
                complete: function () {
                    DmUiLibrary.HideOverlaySpinner();
                }
            });
        },

        SaveCriteria: function (name, summary) {
            var data = JSON.stringify({
                name: name,
                summary: summary
            });

            $.ajax({
                type: 'POST',
                url: siteRoot + '/Criteria/SaveCriteria',
                data: data,
                beforeSend: function () {
                    DmUiLibrary.ShowOverlaySpinner();
                },
                success: function (data) {
                    DmUiLibrary.DisplayAlert(data, this.dataType);
                },
                complete: function () {
                    DmUiLibrary.HideOverlaySpinner();
                }
            });
        },

        UpdateCriteria: function (criteriaId, name, summary) {
            var params = JSON.stringify({
                criteriaId: criteriaId,
                name: name,
                summary: summary
            });

            $.ajax({
                type: 'POST',
                url: siteRoot + '/Criteria/UpdateCriteria',
                data: params,
                beforeSend: function () {
                    DmUiLibrary.ShowOverlaySpinner();
                },
                success: function (data) {
                    DmUiLibrary.DisplayAlert(data, this.dataType);
                },
                complete: function () {
                    DmUiLibrary.HideOverlaySpinner();
                }
            });
        },

        DisplayUnsavedChangesModal: function () {
            $.ajax({
                type: 'GET',
                url: siteRoot + '/Criteria/DisplayUnsavedChangesModal',
                beforeSend: function () {
                    DmUiLibrary.ShowOverlaySpinner();
                },
                success: function (data) {
                    $('#modal-placeholder').html(data);
                    $('#stay-edit-criteria-button').focus();
                    $('#stay-edit-criteria-button').on('click', function () {
                        $('#modal-placeholder').empty();
                    });

                    $('#leave-edit-criteria-button').on('keydown', function (e) {
                        if (e.keyCode == 9 && !e.shiftKey) {
                            $('#stay-edit-criteria-button').focus();
                            return false;
                        }
                    });

                    $('#stay-edit-criteria-button').on('keydown', function (e) {
                        if (e.keyCode == 9 && e.shiftKey) {
                            $('#leave-edit-criteria-button').focus();
                            return false;
                        }
                    });

                    $('#leave-edit-criteria-button').on('click', function () {
                        DmUiLibrary.ShowOverlaySpinner();
                        $('#modal-placeholder').empty();
                        Options.ClearEditMode(function () {
                            window.location = siteRoot + '/Options';
                        });
                    });
                },
                complete: function () {
                    DmUiLibrary.HideOverlaySpinner();
                }
            });
        },

        DisplayRunInBackgroundModal: function () {
            var self = this;

            $.ajax({
                type: 'GET',
                url: siteRoot + '/Criteria/DisplayRunInBackgroundModal',
                beforeSend: function () {
                    DmUiLibrary.ShowOverlaySpinner();
                },
                success: function (data) {
                    $('#modal-placeholder').html(data);
                    $('#modal-placeholder').attr('aria-label', 'Specify a Report Name');
                    $('#report-name').focus();

                    var $reportNameTextBox = $('#report-name');

                    $reportNameTextBox.on('keyup', function () { self.ValidateEmptyTextBox($reportNameTextBox); });
                    $('#cancel-run-in-background-button').on('click', function () { $('#modal-placeholder').empty(); });

                    $('#ok-run-in-background-button').on('keydown', function (e) {
                        if (e.keyCode == 9 && !e.shiftKey) {
                            $('#report-name').focus();
                            return false;
                        }
                    });

                    $('#report-name').on('keydown', function (e) {
                        if (e.keyCode == 9 && e.shiftKey) {
                            $('#ok-run-in-background-button').focus();
                            return false;
                        }
                    });

                    $('#ok-run-in-background-button').on('click', function () {
                        if (self.ValidateEmptyTextBox($reportNameTextBox)) {
                            self.SendReportToBackground($reportNameTextBox.val(), null, true);
                            $('#modal-placeholder').empty();
                        }
                    });
                },
                complete: function () {
                    DmUiLibrary.HideOverlaySpinner();
                }
            });
        },

        DisplaySaveCriteriaModal: function (saveAsCopyCallback) {
            var self = this;

            $.ajax({
                type: 'GET',
                url: siteRoot + '/Criteria/DisplaySaveCriteriaModal',
                beforeSend: function () {
                    DmUiLibrary.ShowOverlaySpinner();
                },
                success: function (data) {
                    $('#modal-placeholder').html(data);
                    $('#modal-placeholder').attr('aria-label', 'Save Criteria');
                    $('#criteria-name').focus();
                    
                    if (saveAsCopyCallback)
                        saveAsCopyCallback();

                    var $reportNameTextBox = $('#criteria-name');

                    $reportNameTextBox.on('keyup', function () { self.ValidateEmptyTextBox($reportNameTextBox); });
                    $('#cancel-save-criteria-button').on('click', function () { $('#modal-placeholder').empty(); });

                    $('#save-save-criteria-button').on('keydown', function (e) {
                        if (e.keyCode == 9 && !e.shiftKey) {
                            $('#criteria-name').focus();
                            return false;
                        }
                    });

                    $('#criteria-name').on('keydown', function (e) {
                        if (e.keyCode == 9 && e.shiftKey) {
                            $('#save-save-criteria-button').focus();
                            return false;
                        }
                    });

                    $('#save-save-criteria-button').on('click', function () {
                        if (self.ValidateEmptyTextBox($reportNameTextBox)) {
                            self.SaveCriteria($reportNameTextBox.val(), $('.save-criteria-modal-body #criteria-summary').val());
                            $('#modal-placeholder').empty();
                        }
                    });
                },
                complete: function () {
                    DmUiLibrary.HideOverlaySpinner();
                }
            });
        },

        ValidateEmptyTextBox: function (inputText) {
            var $textBox = $(inputText);

            if ($textBox.val() === '') {
                $textBox.addClass('dm-ui-error');
                $textBox.next('.dm-ui-error-text').show();
                return false;
            }
            else {
                $textBox.removeClass('dm-ui-error');
                $textBox.next('.dm-ui-error-text').hide();
                return true;
            }
        }
    };
}();