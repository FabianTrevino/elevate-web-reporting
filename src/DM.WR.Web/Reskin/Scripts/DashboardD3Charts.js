var DashboardD3Charts = function () {
    var siteRoot = DmUiLibrary.GetUiSettings().SiteRoot;
    var private = {
        //tabIndex: "-1"
        tabIndex: "0"
    };
    return {
        BarChart: function (element, data, columnsNum) {
            var isGradeK1 = false;
            if (typeof data.totalCount !== "undefined") {
                isGradeK1 = true;
            }
            var isGenerateCurvedLine = false; //generate curved line & other stuff
            var isGenerateZeroLinks = false; //generate or not links on empty columns
            var isGenerateRightLinksOutsideSvg = true; //generate right links not inside SVG
            function compareRange(obj1, obj2) {
                return obj1.range - obj2.range;
            }
            function compareUrlParams(obj1, obj2) {
                if (typeof obj1.url_params !== "undefined") {
                    return Number(obj1.url_params.substr(6, 1)) - Number(obj2.url_params.substr(6, 1));
                }
                return 0;
            }

            var linkQuantileAdaptive = "";
            //if (isAdaptive) {
            if (typeof data.url !== "undefined") {
                //data.values.sort(compareUrlParams);
                data.values.sort(compareRange);
                linkQuantileAdaptive = data.url;
            } else {
                data.values.sort(compareRange);
            }

            var wSvg = 350;
            var hSvg = 170;
            var w = wSvg / 2 + 20;
            var paddingYTop = 10;
            var paddingYBottom = 20;
            var linkClass = "domain-band";
            var arrColor = ["#8d0d0d", "#4e7a20", "#0d598b"];
            var arrLabelBarRange = ["Low", "Mid", "High"];
            var svgClassName = "domain";
            var paddingXLeft = 25;
            var paddingBarsBetween = 9;
            var paddingBarsLeft = 17;
            var paddingBarsRight = 5;
            var labelsMarginTop = 10;
            var rangesFontSize = 13;
            var labelsFontSize = 12;
            var labelsLineHeight = 13;
            var rangesBlockHeight = 48;
            var rectLabelsLeftShift = 15;
            var labelsXPos = wSvg - 115,
                labelsFontFamily = "Arial",
                labelsFill = "#484848",
                //labelsTextAnchor = "end",
                labelsTextAnchor = "start",
                labelsLineTopShift = 3;
            //var quantileRangeTextLabel = "NPR";
            var arrQuantileRangeTextLabel = ["Level 1", "Level 2", "Level 3", "Level 4"];

            if (columnsNum >= 4) {
                wSvg = 510;
                if (isGradeK1) wSvg = 600;
                hSvg = 270;
                w = wSvg * 2 / 3 + 30;
                if (isGradeK1) w = wSvg * 3 / 4 + 20;
                paddingYTop = 45;
                paddingYBottom = 30;
                linkClass = "quantile-band";
                arrColor = ["#c32727", "#f56600", "#cc8500", "#3d8060", "#04739e"];
                if (isGradeK1) {
                    linkClass = "pld-performance-stage";
                    arrColor = ["#04739d", "#c1862d", "#c22627", "#3d8060", "#81589c"];
                }
                svgClassName = "quantile";
                paddingXLeft = 30;
                paddingBarsLeft = 20;
                paddingBarsRight = 17;
                paddingBarsBetween = 8;
                if (isGradeK1) paddingBarsBetween = 16;
                rangesFontSize = 14;
                labelsFontSize = 13;
                labelsLineHeight = 14;
                if (columnsNum === 4 && !isGradeK1) {
                    rangesBlockHeight = 55;
                    arrLabelBarRange = ["1-24", "25-49", "50-74", "75-99"];
                    svgClassName = "quartile";
                } else {
                    rangesBlockHeight = 45;
                    //arrLabelBarRange = ["1-20", "21-40", "41-60", "61-80", "81-99"];
                    arrLabelBarRange = data["values"].map(function (x) {
                        if (isGradeK1) return "";
                        return x.range_band.replace(":", "-");
                    });
                    arrQuantileRangeTextLabel = data["values"].map(function (x) {
                        if (isGradeK1) return x.PLDStage;
                        return x.caption.substr(0, x.caption.lastIndexOf(" "));
                    });
                    if (data.values.length < 5 && !isGradeK1) {
                        //adding missing ranges if not everything comes from API
                        /*
                        var arrRanges = [];
                        for (var i = 0; i < data.values.length; i++) {
                            arrRanges.push(data.values[i]["range_band"]);
                        }
                        //console.log(arrRanges);
                        if ($.inArray("1-20", arrRanges) === -1) {
                            data.values.push({ "caption": "NPR 1-20", "color": "", "number": null, "percent": null, "range_band": "1:20" });
                        }
                        if ($.inArray("21-40", arrRanges) === -1) {
                            data.values.push({ "caption": "NPR 21-40", "color": "", "number": null, "percent": null, "range_band": "21:40" });
                        }
                        if ($.inArray("41-60", arrRanges) === -1) {
                            data.values.push({ "caption": "NPR 41-60", "color": "", "number": null, "percent": null, "range_band": "41:60" });
                        }
                        if ($.inArray("61-80", arrRanges) === -1) {
                            data.values.push({ "caption": "NPR 61-80", "color": "", "number": null, "percent": null, "range_band": "61:80" });
                        }
                        if ($.inArray("81-100", arrRanges) === -1) {
                            data.values.push({ "caption": "NPR 81-99", "color": "", "number": null, "percent": null, "range_band": "81:99" });
                        }
                        */
                        function compare(a, b) {
                            var comparison = 0;
                            if (a["range_band"] > b["range_band"]) {
                                comparison = 1;
                            } else if (a["range_band"] < b["range_band"]) {
                                comparison = -1;
                            }
                            return comparison;
                        }
                        data.values.sort(compare);
                        //console.log(data.values);
                    }
/*
                    data.values = [
                        { "caption": "NPR 1-20", "color": "", "number": 2, "percent": 20, "range_band": "1:20" },
                        { "caption": "NPR 21-40", "color": "", "number": 1, "percent": 10, "range_band": "21:40" },
                        { "caption": "NPR 41-60", "color": "", "number": 2, "percent": 20, "range_band": "41:60" },
                        { "caption": "NPR 61-80", "color": "", "number": 4, "percent": 40, "range_band": "61:80" },
                        { "caption": "NPR 81-99", "color": "", "number": 1, "percent": 10, "range_band": "81:99" }
                    ];
*/
                }
                rectLabelsLeftShift = 80;
                if (isGradeK1) rectLabelsLeftShift = 150;
                labelsXPos = wSvg - 124;
                if (isGradeK1) labelsXPos = wSvg - 99;
                labelsTextAnchor = "start";
            }
            var h = hSvg - paddingYBottom - paddingYTop;
            var barWidth = (w - paddingXLeft - paddingBarsLeft - paddingBarsRight - paddingBarsBetween * (columnsNum - 1)) / columnsNum;

            var category = "";
            var hrefParamCategory = siteRoot + "/api/Dashboard/GetRoster?";
            if (typeof data.category !== "undefined") {
                hrefParamCategory += "category=" + data.category;
                category = data.category;
            } else {
                category = data.title;
                if (category === null) {
                    category = "null";
                }
            }
            hrefParamCategory = "#";

            var divWrapper, svg;
            if (isGenerateRightLinksOutsideSvg) {
                divWrapper = d3.select(element)
                    .append("div")
                    .attr("class", "bar-chart-wrapper " + svgClassName)
                    .attr("width", wSvg)
                    .attr("height", hSvg);

                svg = divWrapper.append("svg")
                    .attr("class", "bar_chart " + svgClassName)
                    .attr("width", wSvg)
                    .attr("height", hSvg);
            } else {
                svg = d3.select(element)
                    .attr("class", "bar_chart " + svgClassName)
                    .attr("width", wSvg)
                    .attr("height", hSvg);
            }

            svg.attr("role", "img")
                .attr("aria-labelledby", function () {
                    if (columnsNum >= 4) return "performance-band-graph-title";
                    else return data.title.replace(/ /g, "-").toLowerCase() + "-graph-title";
                });

            svg.append("title")
                .attr("id", function () {
                    if (columnsNum >= 4) return "performance-band-graph-title";
                    else return data.title.replace(/ /g, "-").toLowerCase() + "-graph-title";
                })
                .text(function() {
                    if (columnsNum >= 4) {
                        if (isGradeK1) return "Percent of Students in each Performance Stage Graph";
                        else return "Percent of Students in each Performance Band Graph";
                    }
                    else return data.title + " Graph";
                });

            var dataset = data.values;
            if (dataset.length > columnsNum) {
                console.log(dataset.length);
                if (columnsNum === 3) {
                    $("#" + element.id + " + .chart3bars-legend").append('<div class="debug-warning">' + "Incorrect data received (Card columns num <strong>" + dataset.length + "</strong>)" + '</div>');
                } else {
                    if (columnsNum === 5) {
                        //for (var i = 0; i < dataset.length; i++) {
                        //    console.log(i + ")");
                        //    console.log(dataset[i]);
                        //}
                        $("#chart4bars-debug-warning").html("Incorrect data received (Quantile columns num <strong>" + dataset.length + "</strong>)");
                    } else {
                        $("#chart4bars-debug-warning").html("Incorrect data received (Quartile columns num <strong>" + dataset.length + "</strong>)");
                    }
                }
                dataset.length = columnsNum;
            }

            var t = d3.transition()
                .duration(300)
                .ease(d3.easeLinear);

            var rects = svg.selectAll("rect")
                .data(dataset)
                .enter();

            var labels = svg.selectAll("text")
                .data(dataset)
                .enter();

            var divRects, divLabels;
            if (isGenerateRightLinksOutsideSvg) {
                divRects = divWrapper.selectAll("div")
                    .data(dataset)
                    .enter();
                divLabels = divWrapper.selectAll("div")
                    .data(dataset)
                    .enter();
            }

            var dashedLineY, middleHeightOfAllColumns = 0;


            if (columnsNum >= 4 && isGenerateCurvedLine) {
                var multiplier = 1.5;
                var dotRadius = 5;
                var dataSetBarCenter = [];
                dataSetBarCenter.push(0);
                for (var i = 0; i < data.values.length; i++) {
                    dataSetBarCenter.push(data.values[i].percent);
                }
                dataSetBarCenter.push(0);

                var dataSetHorizontalLines = [];
                for (var i = 1; i <= 10; i++) {
                    dataSetHorizontalLines.push(i * 10);
                }
                var dataSetVerticalLines = [];
                for (var i = 1; i <= 6; i++) {
                    dataSetVerticalLines.push(i);
                }

                var gridHorizontalLines = svg.selectAll("line")
                    .data(dataSetHorizontalLines)
                    .enter();
                var gridVerticalLines = svg.selectAll("line")
                    .data(dataSetVerticalLines)
                    .enter();

                gridHorizontalLines.append("line")
                    .attr("x1", paddingXLeft)
                    .attr("y1", function (d) { return h - h * d / 100 + paddingYBottom; })
                    .attr("x2", w)
                    .attr("y2", function (d) { return h - h * d / 100 + paddingYBottom; })
                    //.attr("stroke-opacity", "10%")
                    .attr("style", "opacity: 0.2;")
                    .attr("stroke", "#484848");

                var lineCurvedZero = d3.line()
                    .x(function (d, i) {
                        if (i === 0) {
                            return paddingXLeft;
                        } else if (i === dataSetBarCenter.length - 1) {
                            return w;
                        } else {
                            return paddingXLeft + paddingBarsLeft + (i - 1) * barWidth + (i - 1) * paddingBarsBetween + barWidth / 2;
                        }
                    })
                    .y(function (d, i) {return hSvg - paddingYBottom;})
                    //.curve(d3.curveLinear);
                    .curve(d3.curveMonotoneX); // apply smoothing to the line

                var lineCurved = d3.line()
                    .x(function (d, i) {
                        if (i === 0) {
                            return paddingXLeft;
                        } else if (i === dataSetBarCenter.length - 1) {
                            return w;
                        } else {
                            return paddingXLeft + paddingBarsLeft + (i - 1) * barWidth + (i - 1) * paddingBarsBetween + barWidth / 2;
                        }
                    })
                    .y(function (d, i) {
                        if (i === 0 || i === dataSetBarCenter.length - 1) {
                            return hSvg - paddingYBottom;
                        } else {
                            return h - h * d / 100 * multiplier + paddingYBottom;
                        }
                    })
                    //.curve(d3.curveLinear);
                    .curve(d3.curveMonotoneX); // apply smoothing to the line

                svg.append("path")
                    .datum(dataSetBarCenter) //binds data to the line 
                    .attr("class", "curved-background")
                    .attr("stroke", "#0e5b8b")
                    .attr("stroke-width", "0px")
                    .attr("fill", "#fff")
                    //.attr("fill", "url(#grad1)")
                    //.attr("filter", "url(#dropshadow)")
                    .attr("d", lineCurvedZero) // calls the line generator 
                    .transition(t)
                    .attr("d", lineCurved); // calls the line generator 

                svg.append("defs")
                    //.html('<linearGradient id="grad1" x1="0%" y1="0%" x2="100%" y2="0%"><stop offset="0%" style="stop-color:rgb(255,255,0);stop-opacity:1" /><stop offset="100%" style="stop-color:rgb(255,0,0);stop-opacity:1" /></linearGradient>' +
                    .html('<linearGradient id="grad1" x1="0%" y1="0%" x2="100%" y2="0%"><stop offset="0%" stop-color="white" /><stop offset="100%" stop-color="white" /></linearGradient>' +
                    '<filter xmlns="http://www.w3.org/2000/svg" id="dropshadow" height="130%"><feGaussianBlur in="SourceAlpha" stdDeviation="3"/><feOffset dx="2" dy="2" result="offsetblur" /><feMerge><feMergeNode /><feMergeNode in="SourceGraphic" /></feMerge></filter>');
            }


            //add bars of chart
            rects.each(function (d, i) {
                if ((d.number > 0 && !isGradeK1) || (d.studentCount > 0 && isGradeK1) || isGenerateZeroLinks) {
                    d3.select(this).append("a")
                    .attr("xlink:href", function () { return hrefParamCategory; })
                    .attr("class", function () {
                        if (isGradeK1) return linkClass + " stage" + d.PLDStageNum;
                        return linkClass;
                    })
                    .attr("role", "button")
                    .attr("data-link-name",
                        function () {
                            if (columnsNum >= 4) {
                                if (isGradeK1) return arrQuantileRangeTextLabel[i];
                                return arrQuantileRangeTextLabel[i] + " (" + arrLabelBarRange[i] + ")";
                            } else {
                                return d.caption;
                            }
                        }
                    )
                    .attr("tabindex", "-1")
                    .attr("tabindex-important", "true")
                    .attr("aria-hidden", "true")
                    .attr("data-category", category)
                    .attr("data-range", function (d) { return d.range; })
                    .attr("data-range-band", function (d) { return d.range_band; })
                    .attr("data-pld-stage-number", d.PLDStageNum)
                    .attr("data-students-number", d.studentCount)
                    .attr("data-students-percent", d.percent)
                    .attr("data-adaptive-url", function (d) { if (typeof d.url_params !== "undefined") return linkQuantileAdaptive + "?" + d.url_params; })
                    .attr("data-type", "Percent")
                    .append("rect")
                    .attr("x", function () { return paddingXLeft + paddingBarsLeft + i * barWidth + i * paddingBarsBetween; })
                    .attr("y", function () { return h + paddingYTop; }) //for transition start value
                    .attr("width", barWidth)
                    .attr("fill", function () { return arrColor[i]; })
                    .attr("stroke", "#484848")
                    .attr("height", 0) //for transition start value
                    .transition(t)
                        .attr("y", function () {
                            var barHeight = h * d.percent / 100;
                            if (d.number > 0 && barHeight < 2) {
                                barHeight = 2;
                            }
                            return h - barHeight + paddingYTop;
                        })
                    .attr("height", function() {
                        middleHeightOfAllColumns += h * d.percent / 100;
                        var barHeight = h * d.percent / 100;
                        if (d.number > 0 && barHeight < 2) {
                            barHeight = 2;
                        }
                        return barHeight;
                    });
                } else {
                    d3.select(this).append("rect")
                    .attr("x", function () { return paddingXLeft + paddingBarsLeft + i * barWidth + i * paddingBarsBetween; })
                    .attr("y", function () { return h + paddingYTop; }) //for transition start value
                    .attr("width", barWidth)
                    .attr("fill", function () { return arrColor[i]; })
                    .attr("stroke", "#484848")
                    .attr("height", 0) //for transition start value
                    .transition(t)
                    .attr("y", function() {
                        var barHeight = h * d.percent / 100;
                        if (d.number > 0 && barHeight < 2) {
                            barHeight = 2;
                        }
                        return h - barHeight + paddingYTop;
                    })
                    .attr("height", function() {
                        middleHeightOfAllColumns += h * d.percent / 100;
                        var barHeight = h * d.percent / 100;
                        if (d.number > 0 && barHeight < 2) {
                            barHeight = 2;
                        }
                        return barHeight;
                    });
                }
            });

            if (columnsNum >= 4 && isGenerateCurvedLine) {
                if (false) {
                    var dots = svg.selectAll("circle")
                        .data(dataSet)
                        .enter()
                        .append("circle")
                        .attr("cx", function (d, i) {
                            if (i === 0) {
                                return paddingXLeft;
                            } else if (i === dataSet.length - 1) {
                                return w;
                            } else {
                                return paddingXLeft + paddingBarsLeft + (i - 1) * barWidth + (i - 1) * paddingBarsBetween + barWidth / 2;
                            }
                        })
                        .attr("cy", function (d, i) {
                            if (i === 0 || i === dataSet.length - 1) {
                                return hSvg - paddingYBottom;
                            } else {
                                return h - h * d / 100 * multiplier + paddingYBottom;
                            }
                        })
                        .attr("r", dotRadius)
                        .attr("fill", "#0e5b8b");
                }

                gridVerticalLines.append("line")
                    .attr("x1", (function (d, i) { return paddingXLeft + paddingBarsLeft + (d - 1) * barWidth + (d - 1) * paddingBarsBetween + 2 - i * 2; }))
                    .attr("y1", paddingYTop)
                    .attr("x2", (function (d, i) { return paddingXLeft + paddingBarsLeft + (d - 1) * barWidth + (d - 1) * paddingBarsBetween + 2 - i * 2; }))
                    .attr("y2", h + paddingYTop - 5)
                    .attr("stroke-width", "2px")
                    //.attr("style", "opacity: 0.2;")
                    .attr("stroke", "#fff");

                svg.append("path")
                    .datum(dataSetBarCenter) //binds data to the line 
                    .attr("stroke", "#0e5b8b")
                    .attr("stroke-width", "2px")
                    .attr("fill", "none")
                    .attr("d", lineCurvedZero) // calls the line generator 
                    .transition(t)
                    .attr("d", lineCurved); // calls the line generator 
            }

            if (columnsNum >= 4) {
                var sum = 0;
                var x1 = paddingXLeft + paddingBarsLeft + barWidth/2 + 1;
                var x2 = paddingXLeft + paddingBarsLeft + (columnsNum - 1) * (barWidth + paddingBarsBetween) + barWidth / 2 + 1;
                var strokeDashArray = "11,6";
                if (columnsNum === 5) {
                    strokeDashArray = "11,6";
                }
                for (var k = 0; k < data.values.length; k++) {
                    sum += Number(data.values[k].number);
                }
                if (sum) {
                    dashedLineY = h - middleHeightOfAllColumns / columnsNum + paddingYTop + 1;
/*
                    svg.append("line")
                        .attr("class", "dashed-line")
                        .attr("x1", x1)
                        .attr("y1", dashedLineY)
                        .attr("x2", x2)
                        .attr("y2", dashedLineY)
                        .attr("stroke", "#000")
                        .attr("stroke-width", "3")
                        .attr("stroke-dasharray", strokeDashArray);
*/
                }
            }

            //add axis
            var xScale = d3.scaleLinear()
                .domain([0, 3])
                .range([0, w - paddingXLeft]);

            var yScale = d3.scaleLinear()
                .domain([0, 100])
                .range([h, 0]);

            var yAxis = d3.axisLeft()
                .scale(yScale)
                //.tickFormat("")
                .tickSize(-4)
                .ticks(10);

            svg.append("g")
                .attr("class", "axis axis_y")
                .attr("aria-hidden", "true")
                .attr("transform", "translate(" + paddingXLeft + ", " + paddingYTop + ")")
                .call(yAxis);

            var xAxis = d3.axisBottom()
                .scale(xScale)
                .tickFormat("")
                .tickSize(0)
                .ticks(0);

            svg.append("g")
                .attr("class", "axis axis_x")
                .attr("aria-hidden", "true")
                .attr("transform", "translate(" + paddingXLeft + "," + (h + paddingYTop) + ")")
                .call(xAxis);


            rects.each(function (d, i) {
                //legend under the graph X axis
                if (columnsNum >= 4) {
                    d3.select(this)
                        .append("text")
                        //.text(quantileRangeTextLabel)
                        .text(arrQuantileRangeTextLabel[i])
                        .attr("aria-hidden", "true")
                        .attr("x", function () { return paddingXLeft + paddingBarsLeft + i * barWidth + i * paddingBarsBetween + barWidth / 2; })
                        .attr("y", function () { return h + paddingYTop + 15; })
                        .attr("font-size", function () {
                            //if (isGradeK1) return 13;
                            return 12;
                        })
                        .attr("font-weight", function () {
                            if (isGradeK1) return "bold";
                            return "";
                        })
                        .attr("font-family", labelsFontFamily)
                        .attr("fill", labelsFill)
                        .attr("text-anchor", "middle");
                    d3.select(this)
                        .append("text")
                        .text(arrLabelBarRange[i])
                        .attr("aria-hidden", "true")
                        .attr("x", function () { return paddingXLeft + paddingBarsLeft + i * barWidth + i * paddingBarsBetween + barWidth / 2; })
                        .attr("y", function () { return h + paddingYTop + 27; })
                        .attr("font-size", 12)
                        .attr("font-family", labelsFontFamily)
                        .attr("fill", labelsFill)
                        .attr("text-anchor", "middle");
                } else {
                    d3.select(this)
                        .append("text")
                        .text(arrLabelBarRange[i])
                        .attr("aria-hidden", "true")
                        .attr("x", function () { return paddingXLeft + paddingBarsLeft + i * barWidth + i * paddingBarsBetween + barWidth / 2; })
                        .attr("y", function () { return h + paddingYTop + 14; })
                        .attr("font-size", labelsFontSize)
                        .attr("font-family", labelsFontFamily)
                        .attr("fill", labelsFill)
                        .attr("text-anchor", "middle");
                }
            });

            if (isGenerateRightLinksOutsideSvg) {
                //add color labels of chart
                divRects.each(function (d, i) {
                    if ((d.number > 0 && !isGradeK1) || (d.studentCount > 0 && isGradeK1) || isGenerateZeroLinks) {
                        d3.select(this).append("a")
                            .attr("href", function (d) { return hrefParamCategory; })
                            .attr("class", function () {
                                if (isGradeK1) return linkClass + " rect-label stage" + d.PLDStageNum;
                                 return linkClass + " rect-label band" + d.range;
                            })
                            .attr("data-link-name",
                                function () {
                                    if (columnsNum >= 4) {
                                        if (isGradeK1) return arrQuantileRangeTextLabel[i];
                                        return arrQuantileRangeTextLabel[i] + " (" + arrLabelBarRange[i] + ")";
                                    } else {
                                        return d.caption;
                                    }
                                }
                            )
                            .attr("tabindex", "-1")
                            .attr("tabindex-important", "true")
                            .attr("aria-hidden", "true")
                            .attr("data-category", category)
                            .attr("data-range", function (d) { return d.range; })
                            .attr("data-range-band", function (d) { return d.range_band; })
                            .attr("data-pld-stage-number", d.PLDStageNum)
                            .attr("data-students-number", d.studentCount)
                            .attr("data-students-percent", d.percent)
                            .attr("data-adaptive-url", function (d) { if (typeof d.url_params !== "undefined") return linkQuantileAdaptive + "?" + d.url_params; })
                            .attr("data-type", "Percent")
                            .attr("style", "display: block; position: absolute; width: 15px; height: 15px; left: " + (wSvg / 2 + paddingXLeft + rectLabelsLeftShift) + "px; top: " + (i * rangesBlockHeight + labelsMarginTop) + "px; background-color: " + arrColor[i] + ";" );
                    } else {
                        d3.select(this).append("div")
                            .attr("class", function (d) {
                                if (isGradeK1) return "rect-label stage" + d.PLDStageNum;
                                return "rect-label band" + d.range;
                            })
                            .attr("data-pld-stage-number", d.PLDStageNum)
                            .attr("data-range", function (d) { return d.range; })
                            .attr("style", "position: absolute; width: 15px; height: 15px; left: " + (wSvg / 2 + paddingXLeft + rectLabelsLeftShift) + "px; top: " + (i * rangesBlockHeight + labelsMarginTop) + "px; background-color: " + arrColor[i] + ";");
                    }
                });

                //band text links of chart
                divLabels.each(function (d, i) {
                    if ((d.number > 0 && !isGradeK1) || (d.studentCount > 0 && isGradeK1) || isGenerateZeroLinks) {
                        d3.select(this).append("div")
                            .attr("aria-label", function (d) {
                                if (isGradeK1) return d.PLDStage + ", Number " + d.studentCount + ", Percent " + d.percent;
                                return d.caption.replace("-", " through ").replace(" (", ", ").replace(")", "") + ", Number " + d.number + ", Percent " + d.percent;
                            })
                            .attr("role", "button")
                            //.attr("href", hrefParamCategory)
                            //.attr("class", linkClass)
                            .attr("class", function () {
                                if (i === 0 && linkClass === "domain-band") {
                                    if (private.tabIndex === "0") return linkClass + " tab noselect";
                                    else return linkClass + " first-tab-element tab noselect";
                                }
                                else if (i === Number(columnsNum) - 1) {
                                    if (private.tabIndex === "0") return linkClass + " tab noselect";
                                    else return linkClass + " last-tab-element tab noselect";
                                }
                                else return linkClass + " tab noselect";
                            })
                            .attr("data-link-name",
                                function () {
                                    if (columnsNum >= 4) {
                                        if (isGradeK1) return arrQuantileRangeTextLabel[i];
                                        return arrQuantileRangeTextLabel[i] + " (" + arrLabelBarRange[i] + ")";
                                    } else {
                                        return d.caption;
                                    }
                                }
                            )
                            .attr("tabindex", private.tabIndex)
                            .attr("data-category", category)
                            .attr("data-range", function (d) { return d.range; })
                            .attr("data-range-band", function (d) { return d.range_band; })
                            .attr("data-pld-stage-number", d.PLDStageNum)
                            .attr("data-students-number", d.studentCount)
                            .attr("data-students-percent", d.percent)

                            .attr("data-adaptive-url", function (d) { if (typeof d.url_params !== "undefined") return linkQuantileAdaptive + "?" + d.url_params; })
                            //.attr("data-type", "Number")
                            //.attr("aria-hidden", "true")
                            //.text(function (d) { return d.caption; })
                            .attr("style", "position: absolute; left: " + labelsXPos + "px; top: " + (i * rangesBlockHeight + labelsLineHeight + labelsLineTopShift - 18 + labelsMarginTop) + "px;")
                            .append("div")
                            .attr("aria-hidden", "true")
                            .text(function () {
                                if (columnsNum >= 4) {
                                    if (isGradeK1) return arrQuantileRangeTextLabel[i];
                                    return arrQuantileRangeTextLabel[i] + " (" + arrLabelBarRange[i] + ")";
                                } else {
                                    return d.caption;
                                }
                            });
                    } else {
                        d3.select(this).append("div")
                            .attr("aria-label", function (d) {
                                if (isGradeK1) return d.PLDStage + ", Number " + d.studentCount + ", Percent " + d.percent;
                                 return d.caption.replace("-", " through ").replace(" (", ", ").replace(")", "") + ", Number " + d.number + ", Percent " + d.percent;
                            })
                            //.attr("class", linkClass)
                            //.attr("class", "tab")
                            .attr("class", function () {
                                if (i === 0 && linkClass === "domain-band") return " tab first-tab-element band-link-simulation";
                                else if (i === Number(columnsNum) - 1) return " tab last-tab-element band-link-simulation";
                                else return "tab band-link-simulation";
                            })
                            //.attr("tabindex", private.tabIndex)
                            .attr("data-category", category)
                            .attr("data-range", function (d) { return d.range; })
                            .attr("data-range-band", function (d) { return d.range_band; })
                            .attr("data-adaptive-url", function (d) { if (typeof d.url_params !== "undefined") return linkQuantileAdaptive + "?" + d.url_params; })
                            .attr("style", "position: absolute; left: " + labelsXPos + "px; top: " + (i * rangesBlockHeight + labelsLineHeight + labelsLineTopShift - 18 + labelsMarginTop) + "px;")
                            .append("div")
                            .attr("aria-hidden", "true")
                            .text(function () {
                                if (columnsNum >= 4) {
                                    if (isGradeK1) return arrQuantileRangeTextLabel[i];
                                    return arrQuantileRangeTextLabel[i] + " (" + arrLabelBarRange[i] + ")";
                                } else {
                                    return d.caption;
                                }
                            });
                    }

                    //text Number:
                    d3.select(this)
                        .append("div")
                        //.html(function (d) { return 'Number: <a xlink:href="' + href_param_category + '&range=' + d.range + '" class="domain-band" data-category="' + category + '" data-range="' + d.range + '">' + d.number + '</a>' })
                        .text(function () {
                            if (isGradeK1) return "Number: " + d.studentCount;
                            return "Number: " + d.number;
                        })
                        /*
                        .attr("tabindex", private.tabIndex)
                        .attr("class", "tab")
                        */
                        .attr("aria-hidden", "true")
                        .attr("style", "position: absolute; left: " + labelsXPos + "px; top: " + (i * rangesBlockHeight + labelsLineHeight * 2 + labelsLineTopShift - 18 + labelsMarginTop) + "px;");

                    //text Percent:
                    d3.select(this)
                        .append("div")
                        //.html(function (d) { return 'Percent: <a xlink:href="' + href_param_category + '&range=' + d.range + '" class="domain-band" data-category="' + category + '" data-range="' + d.range + '">' + d.percent + '</a>' })
                        .text("Percent: " + d.percent)
                        /*
                        .attr("tabindex", private.tabIndex)
                        .attr("class", function () {
                            if (i === Number(columnsNum) - 1) return "tab last-tab-element";
                            else return "tab";
                        })
                        */
                        .attr("aria-hidden", "true")
                        .attr("style", "position: absolute; left: " + labelsXPos + "px; top: " + (i * rangesBlockHeight + labelsLineHeight * 3 + labelsLineTopShift - 18 + labelsMarginTop) + "px;");
                });
            } else {
                //add color labels of chart
                rects.each(function (d, i) {
                    if (d.number > 0 || isGenerateZeroLinks) {
                        d3.select(this).append("a")
                            .attr("xlink:href", function (d) { return hrefParamCategory; })
                            .attr("class", linkClass)
                            .attr("data-link-name",
                                function () {
                                    if (columnsNum >= 4) {
                                        if (isGradeK1) return arrQuantileRangeTextLabel[i];
                                        return arrQuantileRangeTextLabel[i] + " (" + arrLabelBarRange[i] + ")";
                                    } else {
                                        return d.caption;
                                    }
                                }
                            )
                            .attr("tabindex", "-1")
                            .attr("tabindex-important", "true")
                            .attr("aria-hidden", "true")
                            .attr("data-category", category)
                            .attr("data-range", function (d) { return d.range; })
                            .attr("data-range-band", function (d) { return d.range_band; })
                            .attr("data-adaptive-url", function (d) { if (typeof d.url_params !== "undefined") return linkQuantileAdaptive + "?" + d.url_params; })
                            .attr("data-type", "Percent")
                            .append("rect")
                            .attr("x", wSvg / 2 + paddingXLeft + rectLabelsLeftShift)
                            .attr("y", function () { return i * rangesBlockHeight + labelsMarginTop; })
                            .attr("width", 15)
                            .attr("height", 15)
                            .attr("fill", function () { return arrColor[i]; });
                    } else {
                        d3.select(this).append("rect")
                            .attr("class", function (d) { return "rect-label band" + d.range; })
                            .attr("data-range", function (d) { return d.range; })
                            .attr("x", wSvg / 2 + paddingXLeft + rectLabelsLeftShift)
                            .attr("y", function () { return i * rangesBlockHeight + labelsMarginTop; })
                            .attr("width", 15)
                            .attr("height", 15)
                            .attr("fill", function () { return arrColor[i]; });
                    }
                });

                //band text links of chart
                labels.each(function (d, i) {
                    if (d.number > 0 || isGenerateZeroLinks) {
                        d3.select(this).append("a")
                            .attr("aria-label", function (d) { return d.caption.replace("-", " through ").replace(" (", ", ").replace(")", "") + ", Number " + d.number + ", Percent " + d.percent; })
                            .attr("xlink:href", hrefParamCategory)
                            //.attr("class", linkClass)
                            .attr("class", function () {
                                if (i === 0) return linkClass + " first-tab-element";
                                else if (i === Number(columnsNum) - 1) return linkClass + " last-tab-element";
                                else return linkClass;
                            })
                            .attr("data-link-name",
                                function () {
                                    if (columnsNum >= 4) {
                                        if (isGradeK1) return arrQuantileRangeTextLabel[i];
                                        return arrQuantileRangeTextLabel[i] + " (" + arrLabelBarRange[i] + ")";
                                    } else {
                                        return d.caption;
                                    }
                                }
                            )
                            .attr("tabindex", private.tabIndex)
                            .attr("data-category", category)
                            .attr("data-range", function (d) { return d.range; })
                            .attr("data-range-band", function (d) { return d.range_band; })
                            .attr("data-adaptive-url", function (d) { if (typeof d.url_params !== "undefined") return linkQuantileAdaptive + "?" + d.url_params; })
                            //.attr("data-type", "Number")                
                            .append("text")
                            .attr("aria-hidden", "true")
                            //.text(function (d) { return d.caption; })
                            .text(function () {
                                if (columnsNum >= 4) {
                                    if (isGradeK1) return arrQuantileRangeTextLabel[i];
                                    return arrQuantileRangeTextLabel[i] + " (" + arrLabelBarRange[i] + ")";
                                } else {
                                    return d.caption;
                                }
                            })
                            .attr("x", labelsXPos)
                            .attr("y", i * rangesBlockHeight + labelsLineHeight + labelsLineTopShift - 5 + labelsMarginTop)
                            .attr("font-size", rangesFontSize)
                            .attr("font-family", labelsFontFamily)
                            .attr("fill", labelsFill)
                            .attr("text-anchor", labelsTextAnchor);
                    } else {
                        d3.select(this).append("text")
                            .attr("aria-label", function (d) { return d.caption.replace("-", " through ").replace(" (", ", ").replace(")", "") + ", Number " + d.number + ", Percent " + d.percent; })
                            //.attr("class", linkClass)
                            //.attr("class", "tab")
                            .attr("class", function () {
                                if (i === Number(columnsNum) - 1) return "tab last-tab-element";
                                else return "tab";
                            })
                            //.attr("tabindex", private.tabIndex)
                            .attr("data-category", category)
                            .attr("data-range", function (d) { return d.range; })
                            .attr("data-range-band", function (d) { return d.range_band; })
                            .attr("data-adaptive-url", function (d) { if (typeof d.url_params !== "undefined") return linkQuantileAdaptive + "?" + d.url_params; })
                            .text(function () {
                                if (columnsNum >= 4) {
                                    if (isGradeK1) return arrQuantileRangeTextLabel[i];
                                    return arrQuantileRangeTextLabel[i] + " (" + arrLabelBarRange[i] + ")";
                                } else {
                                    return d.caption;
                                }
                            })
                            .attr("x", labelsXPos)
                            .attr("y", i * rangesBlockHeight + labelsLineHeight + labelsLineTopShift - 5 + labelsMarginTop)
                            .attr("font-size", rangesFontSize)
                            .attr("font-family", labelsFontFamily)
                            .attr("font-weight", "bold")
                            //.attr("fill", labelsFill)
                            .attr("fill", "#2673b9")
                            .attr("text-anchor", labelsTextAnchor);
                    }

                    //text Number:
                    d3.select(this)
                        .append("text")
                        //.html(function (d) { return 'Number: <a xlink:href="' + href_param_category + '&range=' + d.range + '" class="domain-band" data-category="' + category + '" data-range="' + d.range + '">' + d.number + '</a>' })
                        .text("Number: " + d.number)
                        /*
                        .attr("tabindex", private.tabIndex)
                        .attr("class", "tab")
                        */
                        .attr("aria-hidden", "true")
                        .attr("x", labelsXPos)
                        .attr("y", i * rangesBlockHeight + labelsLineHeight * 2 + labelsLineTopShift - 5 + labelsMarginTop)
                        .attr("font-size", labelsFontSize)
                        .attr("font-family", labelsFontFamily)
                        .attr("fill", labelsFill)
                        .attr("text-anchor", labelsTextAnchor);

                    //text Percent:
                    d3.select(this)
                        .append("text")
                        //.html(function (d) { return 'Percent: <a xlink:href="' + href_param_category + '&range=' + d.range + '" class="domain-band" data-category="' + category + '" data-range="' + d.range + '">' + d.percent + '</a>' })
                        .text("Percent: " + d.percent)
                        /*
                        .attr("tabindex", private.tabIndex)
                        .attr("class", function () {
                            if (i === Number(columnsNum) - 1) return "tab last-tab-element";
                            else return "tab";
                        })
                        */
                        .attr("aria-hidden", "true")
                        .attr("x", labelsXPos)
                        .attr("y", i * rangesBlockHeight + labelsLineHeight * 3 + labelsLineTopShift - 5 + labelsMarginTop)
                        .attr("font-size", labelsFontSize)
                        .attr("font-family", labelsFontFamily)
                        .attr("fill", labelsFill)
                        .attr("text-anchor", labelsTextAnchor);
                });
            }
        },

        AverageStandardScore: function (element, dataObj) {
            var generateAsDiv = true; //generate SVG or DIV
            var padding = 1;
            var wSvg = 100;
            var hSvg = 42;
            var fontSize = 42;

            if (generateAsDiv) {
                var div = d3.select(element)
                    .append("div")
                    //.attr("tabindex", private.tabIndex)
                    .attr("class", "tab inline-block")
                    //.attr("class", "tab first-tab-element")
                    .attr("role",
                        function (d) {
                            if (dataObj.average_standard_score !== 0 || dataObj.total_number_students_selected !== 0) return "alert";
                            else return "";
                        })
                    .attr("aria-label",
                        function () {
                            if (typeof dataObj.average_standard_score !== "undefined" && dataObj.average_standard_score !== 0) return "Average Standard Score " + dataObj.average_standard_score;
                            if (typeof dataObj.total_number_students_selected !== "undefined" && dataObj.total_number_students_selected !== 0) return "Total Number of Students Selected " + dataObj.total_number_students_selected;
                            return "";
                        })
                    .attr("width", wSvg)
                    .attr("height", hSvg + padding * 2);

                    div.append("span")
                        .attr("aria-hidden", "true")
                        .attr("class", "gp-score")
                        .text(function () {
                            if (typeof dataObj.average_standard_score !== "undefined" && dataObj.average_standard_score !== 0) return ("" + dataObj.average_standard_score).replace(".0", "");
                            if (typeof dataObj.total_number_students_selected !== "undefined" && dataObj.total_number_students_selected !== 0) return dataObj.total_number_students_selected;
                            return "";
                        });
            } else {
                var svg = d3.select(element)
                    .append("svg")
                    .attr("tabindex", private.tabIndex)
                    .attr("class", "tab")
                    //.attr("class", "tab first-tab-element")
                    //.attr("width", wSvg + padding * 2)
                    .attr("width", wSvg)
                    .attr("height", hSvg + padding * 2);

                svg.append("title").text("Average Standard Score " + dataObj.average_standard_score);

                //if (isAdaptive) {
                if (false) {
                    svg.append("text")
                        .attr("x", wSvg / 2 + padding)
                        .attr("y", hSvg / 2 + padding + fontSize / 2.2)
                        .attr("font-size", fontSize + "px")
                        .attr("line-height", fontSize + "px")
                        .attr("font-weight", "bold")
                        .attr("font-family", "Arial")
                        .attr("fill", "#147bbd")
                        .attr("text-anchor", "middle")
                        .transition()
                        .duration(700)
                        .tween("text",
                            function(d) {
                                var v0 = this.textContent || "0";
                                var v1 = dataObj.average_standard_score;
                                //var i = d3.interpolateRound(v0, v1);
                                //var i = d3.interpolateNumber(v0, v1);
                                var i = d3.interpolateRound(v0, Math.floor(v1));
                                var self = this;
                                return function(t) { self.textContent = i(t) };
                            });
                } else {
                    svg.append("text")
                        //.attr("x", wSvg / 2 + padding)
                        .attr("x", "50%")
                        .attr("y", hSvg / 2 + padding + fontSize / 2.6)
                        .attr("font-size", fontSize + "px")
                        .attr("line-height", fontSize + "px")
                        .attr("font-weight", "bold")
                        .attr("font-family", "Arial")
                        .attr("fill", "#147bbd")
                        .attr("text-anchor", "middle")
                        .attr("aria-hidden", "true")
                        .transition()
                        //.duration(700)
                        //.text(dataObj.average_standard_score);
                        .text(("" + dataObj.average_standard_score).replace(".0", ""));
                }
            }
        },

        PieChartScore: function (element, dataObj, radius) {
            var generateAsDiv = true; //generate SVG or DIV
            //var padding = 3;
            var padding = 0;
            //var wSvg = fontSize + padding * 2;
            //var hSvg = fontSize + padding * 2;
            var wSvg = 80;
            var hSvg = 42;
            var fontSize = 42;

            if (radius !== 0) {
                wSvg = radius * 2;
                hSvg = radius * 2;
                //fontSize = Math.floor(radius * 0.8);
            }

            if (generateAsDiv) {
                var div = d3.select(element)
                    .append("div")
                    //.attr("tabindex", private.tabIndex)
                    .attr("class", "tab inline-block")
                    .attr("role",
                        function (d) {
                            if (dataObj.average_standard_score !== 0) return "alert";
                            else return "";
                        })
                    .attr("aria-label", "National Percentile Rank " + dataObj.national_percentile_rank)
                    .attr("width", wSvg)
                    .attr("height", hSvg + padding * 2);

                div.append("span")
                    .attr("aria-hidden", "true")
                    .attr("class", "gp-score")
                    .text(dataObj.national_percentile_rank);
            } else {
                var svg = d3.select(element)
                    //.html("")
                    .append("svg")
                    .attr("tabindex", private.tabIndex)
                    //.attr("aria-label", dataObj.national_percentile_rank)
                    //.attr("aria-hidden", "true")
                    .attr("class", "tab")
                    .attr("width", wSvg + padding * 2)
                    .attr("height", hSvg + padding * 2);

                svg.append("title").text("National Percentile Rank " + dataObj.national_percentile_rank);

                if (radius > 0) {
                    var fillColor = "#fff";
                    var arrColor = ["#c32727", "#f56600", "#cc8500", "#3d8060"];
                    if (dataObj.national_percentile_rank >= 0 && dataObj.national_percentile_rank <= 24) {
                        fillColor = arrColor[0];
                    }
                    if (dataObj.national_percentile_rank >= 25 && dataObj.national_percentile_rank <= 49) {
                        fillColor = arrColor[1];
                    }
                    if (dataObj.national_percentile_rank >= 50 && dataObj.national_percentile_rank <= 74) {
                        fillColor = arrColor[2];
                    }
                    if (dataObj.national_percentile_rank >= 75 && dataObj.national_percentile_rank <= 100) {
                        fillColor = arrColor[3];
                    }

                    var data = [
                        { "national_percentile_rank": dataObj.national_percentile_rank, "color": fillColor },
                        { "national_percentile_rank": 100 - dataObj.national_percentile_rank, "color": "#fff" }
                    ];

                    var data2Borders = [
                        { "national_percentile_rank": 100, "color": "#fff" }
                    ];

                    //Create group element to hold pie chart    
                    var g = svg.append("g")
                        .attr("transform", "translate(" + (radius + padding) + "," + (radius + padding) + ")");

                    var path = d3.arc()
                        .outerRadius(radius)
                        .innerRadius(radius * 0.8);

                    var pie = d3.pie()
                        .sort(null)
                        .startAngle(-Math.PI)
                        .value(function (d) {
                            return d.national_percentile_rank;
                        });

                    var arc = g.selectAll("arc")
                        .data(pie(data))
                        .enter()
                        .append("g");

                    var arcBorders = g.selectAll("arc")
                        .data(pie(data2Borders))
                        .enter()
                        .append("g");

                    arc.append("path")
                        //.attr("stroke", "#727375") //uncomment if need border of bar
                        .style("fill", function (d) { return d.data.color; })
                        .each(function (d, i) {
                            if (i === 0) {
                                d3.select(this)
                                    .transition()
                                    .duration(500)
                                    .attrTween('d',
                                        function (d) {
                                            var i = d3.interpolate(d.startAngle + 0.1, d.endAngle);
                                            return function (t) {
                                                d.endAngle = i(t);
                                                return path(d);
                                            }
                                        });
                            } else {
                                d3.select(this)
                                    .attr("d", path);
                            }
                        });

                    arcBorders.append("path")
                        .attr("stroke", "#727375")
                        .style("fill", "transparent")
                        .attr("d", path);
                }
                //if (isAdaptive) {
                if (false) {
                    svg.append("text")
                        .attr("x", wSvg / 2 + padding)
                        .attr("y", hSvg / 2 + padding + fontSize / 2.6)
                        .attr("font-size", fontSize)
                        .attr("line-height", fontSize)
                        .attr("font-weight", "bold")
                        .attr("font-family", "Arial")
                        .attr("fill", "#147bbd")
                        .attr("text-anchor", "middle")
                        //.text(data_obj.national_percentile_rank);
                        .transition()
                        .duration(500)
                        .tween("text", function (d) {
                            /*
                                                const v0 = this.textContent || "0";
                                                const v1 = data_obj.national_percentile_rank;
                                                const i = d3.interpolateRound(v0, v1);
                            */
                            var v0 = this.textContent || "0";
                            var v1 = dataObj.national_percentile_rank;
                            var i = d3.interpolateRound(v0, v1);
                            var self = this;
                            return function (t) { self.textContent = i(t) };
                        });
                } else {
                    svg.append("text")
                        .attr("x", wSvg / 2 + padding)
                        .attr("y", hSvg / 2 + padding + fontSize / 2.6)
                        .attr("aria-hidden", "true")
                        .attr("font-size", fontSize)
                        .attr("line-height", fontSize)
                        .attr("font-weight", "bold")
                        .attr("font-family", "Arial")
                        .attr("fill", "#147bbd")
                        .attr("text-anchor", "middle")
                        .text(dataObj.national_percentile_rank);
                }
            }            
        },

        PieChart: function (element, dataObj, radius, strokeWidth) {
            var data = dataObj.values;
            var paddingXLeft = 3;
            var paddingXRight = 3;
            var paddingYTop = 3;
            var paddingYBottom = 3;

            var wSvg = radius * 4 + paddingXLeft + paddingXRight;
            var hSvg = radius * 2 + paddingYTop + paddingYBottom;

            var svg = d3.select(element)
                .append("svg")
                .attr("width", wSvg)
                .attr("height", hSvg);

            //Create group element to hold pie chart    
            var g = svg.append("g")
                .attr("transform", "translate(" + (radius + paddingXLeft) + "," + (radius + paddingYTop) + ")");

            var pie = d3.pie()
                .sort(null)
                .value(function (d) {
                    return d.percent;
                });

            var path = d3.arc()
                .outerRadius(radius)
                .innerRadius(0);

            var arc = g.selectAll("arc")
                .data(pie(data))
                .enter()
                .append("g");

            arc.append("path")
                .attr("d", path)
                .attr("stroke", "#fff")
                .attr("stroke-width", strokeWidth)
                //.attr("fill", function (d) { return d.data.color; });
                .style("fill", function (d) { return d.data.color; })
                .transition()
                .duration(500)
                .attrTween('d', function (d) {
                    var i = d3.interpolate(d.startAngle + 0.1, d.endAngle);
                    return function (t) {
                        d.endAngle = i(t);
                        return path(d);
                    }
                });

            //add color labels of chart.
            var colorLabelShiftX = wSvg / 2 + 20;
            var colorLabelShiftYMult = 50;

            var rects = svg.selectAll("rect")
                .data(data)
                .enter();

            rects.append("rect")
                .attr("x", colorLabelShiftX)
                .attr("y", function (d, i) { return i * colorLabelShiftYMult + 10; })
                .attr("width", 16)
                .attr("height", 16)
                .attr("fill", function (d) { return d.color; })
                .attr("stroke", "#484848");

            var labels = svg.selectAll("text")
                .data(data)
                .enter();

            //add text labels of chart
            var labelsXPos = colorLabelShiftX + 22,
                labelsFontSize = 12,
                labelsFontFamily = "Arial",
                labelsFill = "#484848",
                labelsTextAnchor = "start",
                labelsLineHeight = 14,
                labelsLineTopShift = 6;

            labels.append("text")
                .text(function (d) { return d.caption; })
                .attr("x", labelsXPos)
                .attr("y", function (d, i) { return i * colorLabelShiftYMult + labelsLineHeight + labelsLineTopShift; })
                .attr("font-size", labelsFontSize)
                //.attr("font-weight", "bold")
                .attr("font-family", labelsFontFamily)
                .attr("fill", labelsFill)
                .attr("text-anchor", labelsTextAnchor);

            labels.append("text")
                //.text(function (d) { return "Number: " + d.number })
                .html(function (d) { return 'Number: <a xlink:href="#" class="domain-band">' + d.number + '</a>' })
                .attr("x", labelsXPos + 10)
                .attr("y", function (d, i) { return i * colorLabelShiftYMult + labelsLineHeight * 2 + labelsLineTopShift; })
                .attr("font-size", labelsFontSize)
                .attr("font-family", labelsFontFamily)
                .attr("fill", labelsFill)
                .attr("text-anchor", labelsTextAnchor);

            labels.append("text")
                //.text(function (d) { return "Percent: " + d.percent })
                .html(function (d) { return 'Percent: <a xlink:href="#" class="domain-band">' + d.percent + '</a>' })
                .attr("x", labelsXPos + 10)
                .attr("y", function (d, i) { return i * colorLabelShiftYMult + labelsLineHeight * 3 + labelsLineTopShift; })
                .attr("font-size", labelsFontSize)
                .attr("font-family", labelsFontFamily)
                .attr("fill", labelsFill)
                .attr("text-anchor", labelsTextAnchor);
        },






        StagePieChart: function (element, dataObj) {
            var data = dataObj.values;
            var width = 352;
            var height = 200;
            var margin = 0;
            var radius = Math.min(width, height) / 2 - margin;
            var circleShift = 0.8 * Math.PI;
            var stageNumber = 0;

            var arrColors = ["#222222", "#555555", "#888888", "#bbbbbb"];
            if (dataObj.PLDStage === "Pre-Emerging") {
                arrColors = ["#0f4460", "#427095"];
                stageNumber = 1;
            }
            if (dataObj.PLDStage === "Emerging") {
                arrColors = ["#794700", "#c1862d", "#fdcf97"];
                stageNumber = 2;
            }
            if (dataObj.PLDStage === "Beginning") {
                arrColors = ["#920311", "#c22627", "#ff777b"];
                stageNumber = 3;
            }
            if (dataObj.PLDStage === "Transitioning") {
                arrColors = ["#1f583c", "#3d8060"];
                stageNumber = 4;
            }
            if (dataObj.PLDStage === "Independent") {
                arrColors = ["#81589c"];
                stageNumber = 5;
            }


            var tmp = '<table class="piechart-table"><tr>';
            data.map(function (x, i) {
                var wcagClass = "";
                if (i === 0 && private.tabIndex === "-1") wcagClass = " first-tab-element";
                if (i === data.length - 1 && private.tabIndex === "-1") wcagClass = " last-tab-element";
                if (data[i].studentCount > 0) {
                    tmp += '<td class="piechart-link pld-level' + data[i].PLDLevel + " hover-underlined noselect tab" + wcagClass + '" tabindex="' + private.tabIndex + '" data-link-name="' + dataObj.PLDStage + '" data-pld-stage-number="' + stageNumber + '" data-pld-level-number="' + data[i].PLDLevel + '" data-students-number="' + data[i].studentCount + '" data-students-percent="' + data[i].percent + '" aria-label="PLD Level ' + data[i].PLDLevel + '" role="button"><span style="background-color: ' + arrColors[i] + ';"></span>' + "PLD Level " + x.PLDLevel + "</td>";
                } else {
                    tmp += '<td class="piechart-text pld-level' + data[i].PLDLevel + " noselect" + wcagClass + '" data-link-name="' + dataObj.PLDStage + '" data-pld-stage-number="' + stageNumber + '" data-pld-level-number="' + data[i].PLDLevel + '" data-students-number="' + data[i].studentCount + '" data-students-percent="' + data[i].percent + '"><span style="background-color: ' + arrColors[i] + ';"></span>' + "PLD Level " + x.PLDLevel + "</td>";
                }
            });
            if (data.length === 2) {
                tmp += "<td>&nbsp;</td>";
            }
            if (data.length === 1) {
                tmp += "<td>&nbsp;</td><td>&nbsp;</td>";
            }
            tmp += "</tr></table>";
            $(element).append(tmp);

            var svg = d3.select(element)
                .append("svg")
                .attr("width", width)
                .attr("height", height)
                .append("g")
                .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

            var data2 = {};
            for (var i = 0; i < data.length; i++) {
                data2[i] = data[i].percent;
            }

            var pie = d3.pie()
                .startAngle(circleShift)
                .endAngle(circleShift + 2 * Math.PI)
                .sort(null) // Do not sort group by size
                .value(function (d) {
                    //return d.percent;
                    return d.value;
                });

            var dataReady = pie(d3.entries(data2));

            var arc = d3.arc()
                .innerRadius(radius * 0.55)
                .outerRadius(radius * 0.8);

            var outerArc = d3.arc()
                .innerRadius(radius * 0.9)
                .outerRadius(radius * 0.9);

            var outerArc2 = d3.arc()
                .innerRadius(radius * 0.8)
                .outerRadius(radius * 0.8);


            var slices = svg.selectAll("allSlices")
                .data(dataReady)
                .enter();

            slices.each(function (d, i) {
                if (d.data.value > 0) {
                    d3.select(this).append("path")
                        .attr("d", arc)
                        .attr("class", "piechart-link pld-level" + data[i].PLDLevel)
                        .attr("data-link-name", dataObj.PLDStage)
                        .attr("data-pld-stage-number", stageNumber)
                        .attr("data-pld-level-number", data[i].PLDLevel)
                        .attr("data-students-number", data[i].studentCount)
                        .attr("data-students-percent", data[i].percent)
                        .attr('fill', arrColors[i] )
                        .attr("stroke", "white")
                        .style("stroke-width", "0.2px")
                        .transition()
                        .duration(500)
                        .attrTween("d", function () {
                            var i = d3.interpolate(d.startAngle + 0.1, d.endAngle);
                            return function (t) {
                                d.endAngle = i(t);
                                return arc(d);
                            }
                        });
                }
            });
            svg.selectAll("path").append("title").text(function () {
                    return "PLD Level " + this.parentNode.getAttribute("data-pld-level-number");
                }
            );


            var polylines = svg.selectAll("allPolylines")
                .data(dataReady)
                .enter();

            polylines.each(function (d, i) {
                if (d.data.value > 0) {
                    d3.select(this).append("polyline")
                        .attr("stroke", arrColors[i])
                        .style("fill", "none")
                        .attr("stroke-width", 1)
                        .attr("class", "piechart-polyline pld-level" + data[i].PLDLevel)
                        .attr("data-pld-level-number", data[i].PLDLevel)
                        .attr("points",
                            function () {
                                var posA = outerArc2.centroid(d);
                                var posB = outerArc.centroid(d);
                                var posC = outerArc.centroid(d);
                                var midAngle = d.startAngle + (d.endAngle - d.startAngle) / 2;
                                posC[0] = posC[0] + ((10 + Math.abs(posC[1]) / 8) * (midAngle < Math.PI || midAngle > 2 * Math.PI ? 1 : -1));
                                return [posA, posB, posC];
                            });
                }
            });


            var labels = svg.selectAll("allLabels")
                .data(dataReady)
                .enter();

            labels.each(function (d, i) {
                if (d.data.value > 0) {
                    d3.select(this).append("text")
                        .attr("class", "piechart-link hover-underlined noselect")
                        .attr("data-link-name", dataObj.PLDStage)
                        .attr("data-pld-stage-number", stageNumber)
                        .attr("data-pld-level-number", data[i].PLDLevel)
                        .attr("data-students-number", data[i].studentCount)
                        .attr("data-students-percent", data[i].percent)
                        .text(d.data.value + "%")
                        .attr("transform",
                            function () {
                                var pos = outerArc.centroid(d);
                                var midAngle = d.startAngle + (d.endAngle - d.startAngle) / 2;
                                pos[0] = pos[0] + ((15 + Math.abs(pos[1]) / 8) * (midAngle < Math.PI || midAngle > 2 * Math.PI ? 1 : -1)); // multiply by 1 or -1 to put it on the right or on the left
                                pos[1] += 6;
                                return "translate(" + pos + ")";
                            })
                        .style("text-anchor",
                            function () {
                                var midAngle = d.startAngle + (d.endAngle - d.startAngle) / 2;
                                return (midAngle < Math.PI || midAngle > 2 * Math.PI ? "start" : "end");
                            });
                }
            });
            svg.selectAll("text").append("title").text(function () {
                    return "PLD Level " + this.parentNode.getAttribute("data-pld-level-number");
                }
            );

        }
    }
}();


//jQuery Plugin
(function ($) {
    //Dashboard Charts
    $.fn.BarChartVertical3 = function (options) {
        var settings = $.extend({
            'data': ""
        }, options);
        return this.each(function () {
            DashboardD3Charts.BarChart(this, settings.data, 3);
        });
    };
    $.fn.BarChartVertical4 = function (options) {
        var settings = $.extend({
            'data': ""
        }, options);
        return this.each(function () {
            DashboardD3Charts.BarChart(this, settings.data, options.columns_number);
        });
    };
    $.fn.AverageStandardScore = function (options) {
        var settings = $.extend({
            'data': "0"
        }, options);
        return this.each(function () {
            DashboardD3Charts.AverageStandardScore(this, settings.data);
        });
    };
    $.fn.PieChartScore = function (options) {
        var settings = $.extend({
            'data': "",
            'radius': 60
        }, options);
        return this.each(function () {
            DashboardD3Charts.PieChartScore(this, settings.data, 0);
           //DashboardD3Charts.PieChartScore(this, settings.data, settings.radius);
        });
    };
    $.fn.PieChart = function (options) {
        var settings = $.extend({
            'data': "",
            'radius': 100,
            'stroke_width': 2
        }, options);
        return this.each(function () {
            DashboardD3Charts.PieChart(this, settings.data, settings.radius, settings.stroke_width);
        });
    };
    $.fn.StagePieChart = function (options) {
        var settings = $.extend({
            'data': ""
        }, options);
        return this.each(function () {
            DashboardD3Charts.StagePieChart(this, settings.data);
        });
    };
})(jQuery);
