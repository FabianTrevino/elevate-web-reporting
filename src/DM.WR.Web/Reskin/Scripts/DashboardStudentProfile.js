function valueInPercents(ssValue, start, diff) {
    return ((ssValue - start) / diff * 100).toFixed(3);
}

function initStudentProfile(profileIndex, ssValue) {
    var wrapperElementSelector = ".pdf_wrapper_num_" + profileIndex;
    //debugger;
    var start = 100, end = 400;
    var diff = end - start;
    var ssValuePercent = ((ssValue - start) / diff * 100).toFixed(3);

    //var beginRed = 134, endRed = 187;
    var beginGreen = 187; // endGreen = 225;
    var beginBlue = 225; // endBlue = 300;
    var scoreLineColor = "red";
    if (ssValue >= beginGreen) {
        scoreLineColor = "green";
    }
    if (ssValue >= beginBlue) {
        scoreLineColor = "blue";
    }
    //$(wrapperElementSelector + " .standard_score_graph.current .score_line").addClass(scoreLineColor);
    $(wrapperElementSelector + " .standard_score_graph.current .score_line").attr("data-transited-width", ssValuePercent + "%");

    //fill bottom table with random values (except first row)
    var rndBeginLevel1, rndEndLevel1;
    var rndBeginLevel2, rndEndLevel2;
    var rndBeginLevel3, rndEndLevel3;
    var rndBeginLevel4, rndEndLevel4;
    var rndBeginLevel5, rndEndLevel5;
    var rndBeginLevel1OfFirstRandomLine;
    $(wrapperElementSelector + " .standard-score-graph-table .standard_score_graph .chart").each(function (index) {
    //$($(wrapperElementSelector + " .standard-score-graph-table .standard_score_graph .chart").get().reverse()).each(function (index) {
        if (index >= 0) {
            //debugger;
            if (index === 0) {
                rndBeginLevel1 = Math.floor(Math.random() * 40) + 130;
                rndBeginLevel1OfFirstRandomLine = rndBeginLevel1;
            } else {
                rndBeginLevel1 = rndBeginLevel1OfFirstRandomLine - index*3;
            }
            rndEndLevel1 = rndBeginLevel1 + 20;
            rndBeginLevel2 = rndEndLevel1;
            rndEndLevel2 = rndBeginLevel2 + 20;
            rndBeginLevel3 = rndEndLevel2;
            rndEndLevel3 = rndBeginLevel3 + 20;
            rndBeginLevel4 = rndEndLevel3;
            rndEndLevel4 = rndBeginLevel4 + 20;
            rndBeginLevel5 = rndEndLevel4;
            rndEndLevel5 = rndBeginLevel5 + 20;

            var element;
            element = $(this).find(".bg.level1");
            element.css("left", valueInPercents(rndBeginLevel1, start, diff) + "0%");
            element.css("width", (valueInPercents(rndEndLevel1, start, diff) - valueInPercents(rndBeginLevel1, start, diff)) + "0%");

            element = $(this).find(".bg.level2");
            element.css("left", valueInPercents(rndBeginLevel2, start, diff) + "0%");
            element.css("width", (valueInPercents(rndEndLevel2, start, diff) - valueInPercents(rndBeginLevel2, start, diff)) + "0%");

            element = $(this).find(".bg.level3");
            element.css("left", valueInPercents(rndBeginLevel3, start, diff) + "0%");
            element.css("width", (valueInPercents(rndEndLevel3, start, diff) - valueInPercents(rndBeginLevel3, start, diff)) + "0%");

            element = $(this).find(".bg.level4");
            element.css("left", valueInPercents(rndBeginLevel4, start, diff) + "0%");
            element.css("width", (valueInPercents(rndEndLevel4, start, diff) - valueInPercents(rndBeginLevel4, start, diff)) + "0%");

            element = $(this).find(".bg.level5");
            element.css("left", valueInPercents(rndBeginLevel5, start, diff) + "0%");
            element.css("width", (valueInPercents(rndEndLevel5, start, diff) - valueInPercents(rndBeginLevel5, start, diff)) + "0%");

            var rndScore = Math.floor(Math.random() * (rndEndLevel5 - rndBeginLevel1)) + rndBeginLevel1;

            var rndScoreLineColor = "level1";
            if (rndScore >= rndBeginLevel2) {
                rndScoreLineColor = "level2";
            }
            if (rndScore >= rndBeginLevel3) {
                rndScoreLineColor = "level3";
            }
            if (rndScore >= rndBeginLevel4) {
                rndScoreLineColor = "level4";
            }
            if (rndScore >= rndBeginLevel5) {
                rndScoreLineColor = "level5";
            }
            //debugger;
            //console.log(index + ") ");
            //console.log("rnd_score=" + rnd_score);
            //console.log("rnd_score_line_color=" + rnd_score_line_color);
            $(this).find(".score_line").addClass(rndScoreLineColor);
            $(this).find(".score_line").attr("data-transited-width", valueInPercents(rndScore, start, diff) + "%");
        }
    });
}

function initStudentProfiles(arrOfObjects) {
    var index = 0;
    arrOfObjects.forEach(function (obj) {
/*
        for (var key in obj) {
            console.log("Key: " + key + " value: " + obj[key]);
        }
*/
        initStudentProfile(index, obj.SS);
        index++;
    });
    
    //add transition to table charts
    //setTimeout(startTransitionOfTableBarCharts, 100);
    function startTransitionOfTableBarCharts() {
        $(".chart_bar_transited").each(function () {
            $(this).css("width", $(this).attr("data-transited-width"));
        });
    }
    startTransitionOfTableBarCharts();
}

$(document).on("click touchstart", ".pdf-export", function (e) {
    e.preventDefault();
    var pdfType = $(this).attr("pdf-type");
    console.log("pdf-print, type=" + pdfType);
});

$(document).on("click touchstart", ".pdf-print", function (e) {
    e.preventDefault();
    var pdfType = $(this).attr("pdf-type");

    $("#pdf_type").val(pdfType);
    if ($(this).attr("pdf-export")) {
        //$('#pdf_model').val(location.search + '&pdf-export=true');
        //var url = '../Pdf/?' + 'pdf_type=' + pdf_type + '&pdf_model=' + encodeURIComponent(location.search + '&pdf-export=true');
        var url = "../Pdf/" + location.search + "&pdf_type=student-profile&pdf_export=true";
        //console.log('pdf_export_url=' + url);
        document.location.href = url;
        //$('#pdf_model').val(location.search + '&pdf-export=true');
        //$('#print_preview_form').submit();
    } else {
/*
        $('#pdf_model').val(location.search);
        //$('#pdf_model').val(JSON.stringify(modelPieChartScore));
        window.open('', 'print_preview_popup', 'width=1000,height=600');
        $('#print_preview_form').submit();
*/

        //var 1) pdf conversion with GET request
        //window.open('../Pdf/' + location.search + '&pdf_type=student-profile', 'print_preview_popup', 'width=1000,height=600');

        //var 2) pdf conversion with POST request
        $("#pdf_post_parameters").empty();
        var params = window.location.search.substring(1).split("&");
        for (var index = 0; index < params.length; index++) {
            var valZ = params[index].split("=");
            $("#pdf_post_parameters").append("<input type=\"hidden\" name=\"" + valZ[0] + '" value="' + valZ[1] + '">');
        }
        window.open("", "print_preview_popup", "width=1000,height=600");
        $("#print_preview_form").submit();
    }
});