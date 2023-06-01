var Wcag = function () {

    var private = {
        RosterGridNavigationEvent: false,
        RosterGridKeyEvent: false,
        RosterGridOrderingEvent: false,
        isTabNavigationOn: false,
        LastFocusedElement: {},
        LastRosterTableKeyPressedCode: 0
    };

    return {
        Init: function () {
            //Fix for svg href clicks: Set a split prototype on the SVGAnimatedString object which will return the split of the baseVal on this
            SVGAnimatedString.prototype.split = function () { return String.prototype.split.apply(this.baseVal, arguments); };
            this.AssignAllEventHandlers();
        },

        SetRosterGridNavigationEvent: function (value) {
            private.RosterGridNavigationEvent = value;
        },

        FocusLastFocusedElement: function () {
            if (!$.isEmptyObject(private.LastFocusedElement)) {
                private.LastFocusedElement.focus();
                $("body").addClass("wcag_focuses_on");
            }
        },

        MakeContentTabbableWithDelay: function (element, delay) {
            function makeElementContentTabbableWithDelay() {
                element.rootMakeContentTabbable();
            }
            setTimeout(makeElementContentTabbableWithDelay, delay);
        },

        FocusElementWithDelay: function (element, delay) {
            function focusElement() {
                $("body").addClass("wcag_focuses_on");
                element.focus();
            }
            setTimeout(focusElement, delay);
        },

        AddCssClassToElementWithDelay: function (element, cssClass, delay) {
            function addCssClassToElement() {
                $(element).addClass(cssClass);
            }
            setTimeout(addCssClassToElement, delay);
        },

        AssignAllEventHandlers: function () {
            //***** WCAG tab key navigation behavior *****
            if (Dashboard.IsTabNavigationOn()) {
                $('header.main-header-container [tabindex="0"], header a, .main-menu-container [tabindex="0"], .main-menu-container a').each(function () {
                    $(this).attr("tabindex", "1");
                });
                $('.filters [tabindex="0"], .filters a, .filters button').each(function () {
                    $(this).attr("tabindex", "-1");
                });
                $('footer [tabindex="0"], footer a').each(function () {
                    $(this).attr("tabindex", "5");
                });
            }
            $(document).on("click touchstart", ".root-tab-element", function (e) {
                if (Dashboard.IsTabNavigationOn()) {
                    if ($.inArray("root-tab-element", e.target.classList) === -1) {
                        if (e.originalEvent !== undefined) {
                            if (e.originalEvent.screenX !== 0 && e.originalEvent.screenY !== 0) {
                                if (e.originalEvent.mozInputSource === undefined || e.originalEvent.mozInputSource === 1) {
                                    $(this).rootMakeContentTabbable();
                                } else {
                                    $(this).rootMakeContentTabbable(true);
                                }
                            }
                        }
                    } else {
                        e.stopPropagation();
                        $(this).rootMakeContentTabbable(true);
                    }

                    var clickedGridElement;
                    if (($(e.target).attr("id") === "roster-table-wrapper_active_cell") || ($(e.target).attr("id") === "stanine-table-wrapper_active_cell") || ($(e.target).attr("id") === "right-card-table-wrapper_active_cell") || ($(e.target).attr("id") === "right-card-table2-wrapper_active_cell")) {
                        clickedGridElement = $(e.target);
                    } else {
                        //clickedGridElement = $(e.target).parents("#roster-table-wrapper td, #roster-table-wrapper th, .dashboard-table td, .dashboard-table th");
                        clickedGridElement = $(e.target).parents("#roster-table-wrapper td, #roster-table-wrapper th, #stanine-table-wrapper td, #stanine-table-wrapper th, #right-card-table-wrapper td, #right-card-table-wrapper th, #right-card-table2-wrapper td, #right-card-table2-wrapper th");
                    }
                    if (clickedGridElement.length) {
                        if (typeof e.originalEvent === "object" || typeof e.originalEvent === "undefined") {
                            //checking is click event from Mouse or from JS method? and focusing only if this is Mouse event
                            clickedGridElement.focus();
                        }
                    }
                }
            });

            $(document).on("keydown", "#modal-dashboard-print-students .students-wrapper .dm-ui-checkbox", function (e) {
                if (e.keyCode >= 37 && e.keyCode <= 40) {
                    var nextFocusedCheckbox, currentColumnNumber;
                    if (e.keyCode === 37) {
                        nextFocusedCheckbox = $(this).parents("td").prev().find(".dm-ui-checkbox");
                    }
                    if (e.keyCode === 39) {
                        nextFocusedCheckbox = $(this).parents("td").next().find(".dm-ui-checkbox");
                    }
                    if (e.keyCode === 38) {
                        currentColumnNumber = $(this).parents("td").data("col");
                        nextFocusedCheckbox = $(this).parents("tr").prev().find("td:nth-child(" + Number(currentColumnNumber + 1) + ") .dm-ui-checkbox");
                    }
                    if (e.keyCode === 40) {
                        currentColumnNumber = $(this).parents("td").data("col");
                        nextFocusedCheckbox = $(this).parents("tr").next().find("td:nth-child(" + Number(currentColumnNumber + 1) + ") .dm-ui-checkbox");
                    }
                    if (nextFocusedCheckbox !== undefined && nextFocusedCheckbox !== null && nextFocusedCheckbox.length) {
                        nextFocusedCheckbox.focus();
                    }
                }
            });
            $(document).on("change", "#roster-table-wrapper .k-pager-sizes select", function (e) {
                $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
            });
            $(document).on("click keyup", "#roster-table-wrapper .k-pager-nav", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32 || e.which === 1) {
                    $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                }
            });
            $(document).on("click keyup", ".master-of-roots", function (e) {
                if ((e.keyCode === 13 || e.keyCode === 32 || e.which === 1) && ($.inArray("master-of-roots", e.target.classList) !== -1)) {
                    $(this).masterMakeInnerRootsTabbable(true);
                }
                if (e.keyCode === 27) {
                    $(this).masterMakeInnerRootsNotTabbable();
                }
            });
            $(document).on("keyup", ".root-tab-element", function (e) {
                if ((e.keyCode === 13 || e.keyCode === 32) && ($.inArray("root-tab-element", e.target.classList) !== -1)) {
                    e.stopPropagation();
                    $(this).rootMakeContentTabbable(true);
                }
                if (e.keyCode === 27) {
                    if (!$(this).hasClass("filters") && !$(this).find(":focus").parents(".dm-ui-dropdown-widget").length) {
                        if ($(this).attr("tabindex") === "-1") {
                            e.stopPropagation();
                        }
                        $(this).rootMakeContentNotTabbable(true);
                        DmUiLibrary.StartBrowserScrolling();
                    }
                }
            });
            $(document).on("keyup", ".filters.root-tab-element .dm-ui-dropdown-button, .dashboard-filter.button .dm-ui-button-primary, .performance-band-card.root-tab-element .dm-ui-dropdown-button, .performance-summary-card.root-tab-element .dm-ui-dropdown-button", function (e) {
                if (e.keyCode === 27) {
                    $(this).rootMakeContentNotTabbable(true);
                }
            });
            $(document).on("keyup", ".filters.root-tab-element .dm-ui-dropdown-content, .performance-band-card.root-tab-element .dm-ui-dropdown-content, .performance-summary-card.root-tab-element .dm-ui-dropdown-content", function (e) {
                if (e.keyCode === 27) {
                    $(this).prev().focus();
                }
            });
            $(document).on("keyup", ".root-tab-element .dm-ui-dropdown-widget .dm-ui-dropdown-buttons-container button", function (e) {
                if (e.keyCode === 27) {
                    $(this).closest(".dm-ui-dropdown-button").focus();
                }
            });
            $(document).on("keydown", ".first-root-tab-element", function (e) {
                if (e.keyCode === 9 && e.shiftKey && ($.inArray("root-tab-element", e.target.classList) !== -1)) {
                    $(this).parents(".master-of-roots").masterMakeInnerRootsNotTabbable();
                }
            });
            $(document).on("keydown", ".last-root-tab-element", function (e) {
                if (e.keyCode === 9 && !e.shiftKey && ($.inArray("root-tab-element", e.target.classList) !== -1)) {
                    $(this).parents(".master-of-roots").masterMakeInnerRootsNotTabbable();
                }
            });
            $(document).on("keydown", ".last-tab-element", function (e) {
                if ((e.keyCode === 9 && !e.shiftKey) || e.keyCode === 40) {
                    if ($(this).hasClass("wcag-modal-last-element")) {
                        e.preventDefault();
                        $(this).parents(".dm-ui-modal-container").find(".wcag-modal-first-element").focus();
                    } else {
                        if ($(this).parents(".root-tab-element").hasClass("filters")) {
                            if (($(this).hasClass("dm-ui-expanded") && e.keyCode === 9) || e.keyCode === 40) {
                            } else {
                                $(this).rootMakeContentNotTabbable(true);
                                DmUiLibrary.StartBrowserScrolling();
                            }
                        } else {
                            $(this).rootMakeContentNotTabbable(true);
                            DmUiLibrary.StartBrowserScrolling();
                        }
                    }
                }
            });
            $(document).on("keydown", ".first-tab-element", function (e) {
                if ((e.keyCode === 9 && e.shiftKey) || e.keyCode === 38) {
                    if ($(this).hasClass("wcag-modal-first-element")) {
                        e.preventDefault();
                        $(this).parents(".dm-ui-modal-container").find(".wcag-modal-last-element").focus();
                    } else {
                        $(this).rootMakeContentNotTabbable(true);
                        DmUiLibrary.StartBrowserScrolling();
                    }
                }
            });
            $(document).on("keydown", ".tab:not(.disabled-element):not([aria-autocomplete=list]):not(.k-pager-sizes .k-dropdown):not(.k-state-disabled)", function (e) {
                if (e.keyCode === 13 && e.target.className !== undefined) {
                    if (e.target.className === "k-textbox tab") {
                        $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                        //$(this).focus();
                        Dashboard.FocusElementWithDelay($(".roster-table-card.root-tab-element .k-pager-input input"), 1);
                    }
                }
                if (e.keyCode === 38 || e.keyCode === 40) {
                    var allElementsInBlock = $(this).parents(".root-tab-element").find(".tab:not(.disabled-element)");
                    var index = allElementsInBlock.index(this);

                    if (e.keyCode === 38) {
                        if (index > 0) {
                            allElementsInBlock[index - 1].focus();
                            DmUiLibrary.StopBrowserScrolling();
                            e.preventDefault();
                            e.stopPropagation();
                        }
                    }
                    if (e.keyCode === 40) {
                        if (index < allElementsInBlock.length - 1) {
                            allElementsInBlock[index + 1].focus();
                            DmUiLibrary.StopBrowserScrolling();
                            e.preventDefault();
                            e.stopPropagation();
                        }
                    }
                }
            });
            $(document).on("keydown", ".root-tab-element", function (e) {
                if (e.keyCode === 38 || e.keyCode === 40) {
                    var allElementsInBlock = $(this).parents(".master-of-roots").find(".root-tab-element");
                    var index = allElementsInBlock.index(this);

                    if (e.keyCode === 38) {
                        if (index > 0) {
                            allElementsInBlock[index - 1].focus();
                            DmUiLibrary.StopBrowserScrolling();
                        }
                    }
                    if (e.keyCode === 40) {
                        if (index < allElementsInBlock.length - 1) {
                            allElementsInBlock[index + 1].focus();
                            DmUiLibrary.StopBrowserScrolling();
                        }
                    }
                }
            });

            $(document).on("keydown", "#roster-top-buttons-wrapper .k-input", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    $("body").removeClass("wcag_focuses_on");
                    $(".roster-table-card.root-tab-element").rootMakeContentNotTabbable(false);
                    $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                    var self = $(this);
                    function setFocusBack() {
                        self.focus();
                        $("body").addClass("wcag_focuses_on");
                    }
                    setTimeout(setFocusBack, 100);
                }
            });

            $(document).on("keydown", "#roster-top-buttons-wrapper .k-button.k-button-icon, #apply-dashboard-report-button, #cancel-dashboard-report-button", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    $("body").removeClass("wcag_focuses_on");
                    $(".roster-table-card.root-tab-element").rootMakeContentNotTabbable(false);
                    $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                    function setFocusBack() {
                        $("#roster-filters-popup-button").focus();
                        $("body").addClass("wcag_focuses_on");
                        $(".roster-table-card.root-tab-element").rootMakeContentTabbable();
                    }
                    setTimeout(setFocusBack, 100);
                }
            });

            $(document).on("blur", "#roster-table-wrapper td, #roster-table-wrapper th", function (e) {
                //if (!private.RosterGridNavigationEvent && !private.RosterGridKeyEvent) {
                if (true) {
                    if (e.relatedTarget !== null && e.relatedTarget.nodeName === "TABLE") {
                        $(e.target).focus();

                        function setFocusBack(element) {
                            if (!private.RosterGridNavigationEvent && !private.RosterGridOrderingEvent) {
                                element.focus();
                            }
                            private.RosterGridOrderingEvent = false;
                        }

                        if ((private.LastRosterTableKeyPressedCode >= 37 && private.LastRosterTableKeyPressedCode <= 40) || private.LastRosterTableKeyPressedCode === 9 || private.LastRosterTableKeyPressedCode === 16 || private.LastRosterTableKeyPressedCode === 13) {
                            setTimeout(setFocusBack, 1, $(e.target)); //for Firefox additional focus event
                        }
                        private.LastRosterTableKeyPressedCode = 0;

                    }
                }
                private.RosterGridNavigationEvent = false;
                private.RosterGridKeyEvent = false;
            });

            $(document).on("keyup", "#roster-table-wrapper td, #roster-table-wrapper th, .dashboard-table td, .dashboard-table th", function (e) {
                private.LastRosterTableKeyPressedCode = e.keyCode;
                private.RosterGridKeyEvent = true;
                if (e.keyCode === 9) {
                    Dashboard.setGridCellCurrentState($(this));
                }
            });

            $(document).on("keydown", "#roster-table-wrapper th.k-header, .dashboard-table th.k-header", function (e) {
                if (e.keyCode === 9) {
                    Dashboard.setGridCellCurrentState($(this));
                }
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    var orderingLink = $(this).find(".k-link");
                    if (orderingLink.length) {
                        $(this).find(".k-link").click();
                        Dashboard.setGridCellCurrentState(e.element);
                        private.RosterGridOrderingEvent = true;
                    }
                }
            });

            $(document).on("keydown", "#roster-table-wrapper td#roster-table-wrapper_active_cell, #roster-table-wrapper tbody th#roster-table-wrapper_active_cell", function (e) {
                if (e.keyCode === 13 || e.keyCode === 32) {
                    e.preventDefault();
                    var link = $(this).find(".location-drill, .student-link");
                    if (link.length) {
                        link.click();
                    }
                }
            });

/*
            $(document).on("keydown", "body", function (e) {
                var focusedElement = $(":focus");
                console.log("FOCUS:");
                console.log(focusedElement);
            });
*/
        }
    };
}();


//jQuery Plugin
(function ($) {
    //WCAG functions
    $.fn.makeElementNotTabbable = function () {
        this.attr("tabindex", "-1");
        this.blur();
    };
    $.fn.makeElementTabbable = function () {
        if (Dashboard.IsTabNavigationOn()) {
            var rootElement = this.parents(".root-tab-element");
            var rootTabIndex = rootElement.data("tabindex");
            if (rootTabIndex !== undefined) {
                this.attr("tabindex", rootTabIndex);
            }
        } else {
            this.attr("tabindex", "0");
            this.blur();
        }
    };
    $.fn.masterMakeInnerRootsTabbable = function (isFocusAtTheEnd) {
        if (Dashboard.IsTabNavigationOn()) {
            this.find("> .root-tab-hidden-content").removeAttr("aria-hidden");
            this.find(".root-tab-element").last().addClass("last-root-tab-element");
            this.find(".root-tab-element").first().addClass("first-root-tab-element");
            var elements = this.find(".root-tab-element");
            if (this.data("tabindex") !== undefined && elements.length) {
                elements.each(function () {
                    $(this).attr("tabindex", $(this).data("tabindex"));
                });
                this.attr("tabindex", "-1");

                if (typeof isFocusAtTheEnd !== "undefined" && isFocusAtTheEnd === true) {
                    $("body").addClass("wcag_focuses_on");
                    Dashboard.AddCssClassToElementWithDelay("body", "wcag_focuses_on", 1); //for NVDA screenreader
                    this.find(".root-tab-element").first().focus();
                }
            }
        }
    };
    $.fn.masterMakeInnerRootsNotTabbable = function () {
        if (Dashboard.IsTabNavigationOn()) {
            this.find("> .root-tab-hidden-content").attr("aria-hidden", "true");
            var elements = this.find(".root-tab-element");
            if (this.data("tabindex") !== undefined && elements.length) {
                this.attr("tabindex", this.data("tabindex"));
                elements.each(function () {
                    $(this).attr("tabindex", "-1");
                });
                this.focus();
            }
        }
    };
    $.fn.rootMakeContentTabbable = function (isFocusAtTheEnd) {
        if (Dashboard.IsTabNavigationOn()) {
            //console.log("rootMakeContentTabbable");
            if (this.data("tabindex") !== undefined) {
                this.find(".root-tab-hidden-content").removeAttr("aria-hidden");
                var elements, firstElement;
                var isFilters = false;
                var rootTabIndex = this.data("tabindex");
                //this.removeAttr("tabindex");
                this.attr("tabindex", "-1");
                //this.find(".aria-hidden").removeAttr("aria-hidden");
                elements = this.find("a, .tab");
                if (this.hasClass("filters")) {
                    isFilters = true;
                    elements = this.find("a, .tab, button:enabled, .dm-ui-dropdown-widget:not(.dm-ui-disabled) [tabindex]:not([role=alert])");
                    this.find(".dm-ui-dropdown-button:enabled").first().addClass("first-tab-element");
                    if (this.find(".dashboard-filter.button .dm-ui-button-primary").hasClass("disabled-element")) {
                        this.find(".dm-ui-dropdown-button").last().addClass("last-tab-element");
                    } else {
                        this.find(".dashboard-filter.button .dm-ui-button-primary").addClass("last-tab-element");
                    }
                }
                if (this.hasClass("performance-band-card")) {
                    isFilters = true;
                    elements = this.find(".tab, .dropdown-wrapper button, .dm-ui-dropdown-widget:not(.dm-ui-disabled) [tabindex]:not([role=alert])");
                    this.find(".performance-band-blocks-wrapper .block:last-child").addClass("last-tab-element");
                }
                if (this.hasClass("performance-summary-card")) {
                    isFilters = true;
                    elements = this.find(".tab, .dropdown-wrapper button, .dm-ui-dropdown-widget:not(.dm-ui-disabled) [tabindex]:not([role=alert])");
                    this.find(".performance-band-summary-wrapper .block:last-child .percent").addClass("last-tab-element");
                }
                if (this.hasClass("roster-table-card")) {
                    elements = this.find(".tab, #roster-table-wrapper th.k-header:not(:last-child), #roster-table-wrapper td, #roster-table-wrapper tbody th, #roster-table-wrapper input.k-textbox, #roster-table-wrapper .k-widget.k-dropdown, #roster-table-wrapper .k-pager-nav, #roster-legend-wrapper, .scale-minus, .scale-plus, .scale-reset");
                    this.find("#roster-legend-wrapper").addClass("last-tab-element");
                }
                elements.each(function () {
                    if ($(this).attr("tabindex-important") !== "true" && !$(this).hasClass("disabled-element") && !$(this).hasClass("k-state-disabled")) {
                        $(this).attr("tabindex", rootTabIndex);
                        if (typeof firstElement === "undefined") {
                            firstElement = $(this);
                        }
                        if (!isFilters) {
                            $(this).addClass("tab");
                        }
                    }
                });
                if (typeof isFocusAtTheEnd !== "undefined" && isFocusAtTheEnd === true) {
                    if ($("body").hasClass("wcag_focuses_on")) {
                        if (typeof firstElement !== "undefined" && firstElement !== null) {
                            if (firstElement.text() !== "") {
                                Dashboard.FocusElementWithDelay(firstElement, 100);
                            }
                        }
                    }
                }
            }
        }
    };
    $.fn.rootMakeContentNotTabbable = function (isFocusAtTheEnd) {
        if (Dashboard.IsTabNavigationOn()) {
            //console.log("rootMakeContentNotTabbable");
            var self = this;
            if (!this.hasClass("root-tab-element")) {
                var rootElement = this.parents(".root-tab-element");
                if (rootElement.length) {
                    self = rootElement;
                }
            }
            var rootTabIndex = self.data("tabindex");
            if (rootTabIndex !== undefined) {
                self.find(".root-tab-hidden-content").attr("aria-hidden", "true");
                self.attr("tabindex", rootTabIndex);
                var elements = self.find("a, .tab");
                if (self.hasClass("filters")) {
                    elements = self.find("a, .tab, button, [tabindex]:not([role=alert])");
                }
                if (self.hasClass("performance-band-card")) {
                    elements = self.find(".tab, .dropdown-wrapper button, .dm-ui-dropdown-widget:not(.dm-ui-disabled) [tabindex]:not([role=alert])");
                }
                if (self.hasClass("performance-summary-card")) {
                    elements = self.find(".tab, .dropdown-wrapper button, .dm-ui-dropdown-widget:not(.dm-ui-disabled) [tabindex]:not([role=alert])");
                }
                if (self.hasClass("roster-table-card")) {
                    elements = self.find(".tab, #roster-table-wrapper th.k-header:not(:last-child), #roster-table-wrapper td, #roster-table-wrapper tbody th, #roster-table-wrapper input.k-textbox, #roster-table-wrapper .k-widget.k-dropdown, #roster-table-wrapper .k-pager-nav, #roster-legend-wrapper, .scale-minus, .scale-plus, .scale-reset");
                    self.find("#roster-legend-wrapper").addClass("last-tab-element");
                }
                elements.each(function () {
                    if ($(this).attr("tabindex-important") !== "true") {
                        $(this).attr("tabindex", "-1");
                    }
                });
                if (typeof isFocusAtTheEnd !== "undefined" && isFocusAtTheEnd === true) {
                    $("body").addClass("wcag_focuses_on");
                    self.focus();
                }
            }
        }
    };
})(jQuery);
