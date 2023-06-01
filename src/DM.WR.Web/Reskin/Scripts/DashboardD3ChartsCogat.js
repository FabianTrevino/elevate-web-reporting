var DashboardD3Charts = function () {
    var siteRoot = DmUiLibrary.GetUiSettings().SiteRoot;
    return {
        //BarChartStanine: function (element, data, columnsNum) {
        BarChartStanine: function (element, data, stanineType) {
            //columnsNum = data.values.length;
            var columnsNum = 9;
            var isGenerateCurvedLine = false; //generate curved line & other stuff
            var isGenerateZeroLinks = false; //generate or not links on empty columns

            var wSvg = 660;
            var hSvg = 230;
            //var w = wSvg / 2 + 20;
            var w = wSvg;
            var paddingYTop = 10;
            var paddingYBottom = 20;
            var linkClass = "domain-band";
            //var arrColor = ["#8d0d0d", "#4e7a20", "#0d598b"];
            var arrColor = [];
            var arrLabelBarRange = ["1", "2", "3", "4", "5", "6", "7", "8", "9"];
            var svgClassName = "domain";
            var paddingXLeft = 25;
            var paddingXRight = 25;
            var paddingBarsBetween = 20;
            var paddingBarsLeft = 10;
            var paddingBarsRight = 10;
            var labelsFontSize = 12;
            var labelsFontFamily = "Arial";
            var labelsFill = "#484848";
            var tooltipWidth = 60, tooltipHeight = 20;
            var dotCenterX, dotCenterY;
            //var yAxisMultiplier = 1; // Y axis 0-100%
            var yAxisMultiplier = 2.5; // Y axis 0-40%
            //var yAxisMultiplier = 5; // Y axis 0-20%

            var h = hSvg - paddingYBottom - paddingYTop;
            var barWidth = (w - paddingXLeft - paddingXRight - paddingBarsLeft - paddingBarsRight - paddingBarsBetween * (columnsNum - 1)) / columnsNum;

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
            divWrapper = d3.select(element)
                .append("div")
                .attr("class", "bar-chart-wrapper " + svgClassName)
                .attr("width", wSvg)
                .attr("height", hSvg);

            svg = divWrapper.append("svg")
                .attr("class", "bar_chart " + svgClassName)
                .attr("role", "img")
                .attr("aria-labelledby", "stanine-graph-title")
                .attr("width", wSvg)
                .attr("height", hSvg);

            svg.append("title")
                .attr("id", "stanine-graph-title")
                .text(function () {
                    if (typeof stanineType === "undefined") return "Students by Age Stanine Graph";
                    else return stanineType + " Students by Age Stanine Graph";
                });

            //var dataset = data.values;
            var dataset = [];
            var tmpObj;
            for (var i = 0; i < data.values.length; i++) {
                if (data.values[i]["content_area"] === stanineType && stanineType !== undefined) {
                    for (var j = 1; j <= 9; j++) {
                        tmpObj = {
                            "percent": data.values[i]["stanine" + j + "_per"],
                            "number": data.values[i]["stanine" + j + "_num"],
                            "percent_nationally": data.percent_nationally["stanine" + j]
                        }
                        dataset.push(tmpObj);
                    }
                    break;
                }
            }

            var t = d3.transition()
                .duration(300)
                .ease(d3.easeLinear);            

            var labels = svg.selectAll("text")
                .data(dataset)
                .enter();

            var middleHeightOfAllColumns = 0;
            
            if (columnsNum >= 4 && isGenerateCurvedLine) {
                var multiplier = 1.5;
                var dotRadius = 5;
                var dataSetBarCenter = [];
                dataSetBarCenter.push(0);
                for (var i = 0; i < dataset.length; i++) {
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
                    .curve(d3.curveLinear);
                    //.curve(d3.curveMonotoneX); // apply smoothing to the line

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
                    .curve(d3.curveLinear);
                    //.curve(d3.curveMonotoneX); // apply smoothing to the line

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


            //dashed horizontal lines
            var strokeDashArray = "7,6";
            var dashedLinesX1 = paddingXLeft + paddingBarsLeft;
            var dashedLinesX2 = w - paddingXLeft - paddingBarsLeft;
            var dashedLinesY;
            //for (var i = 0; i < 10; i++) {
            for (var i = 0; i < (10 / yAxisMultiplier * 2); i++) {
                dashedLinesY = paddingYTop + i * h * (yAxisMultiplier / 2) / 10 + 0.5;
                svg.append("line")
                    .attr("class", "dashed-line")
                    .attr("x1", dashedLinesX1)
                    .attr("y1", dashedLinesY)
                    .attr("x2", dashedLinesX2)
                    .attr("y2", dashedLinesY)
                    .attr("stroke", "#979797")
                    .attr("fill", "transparent")
                    .attr("stroke-width", "1")
                    .attr("stroke-dasharray", strokeDashArray);
            }

            var rects = svg.selectAll("rect")
                .data(dataset)
                .enter();

            //add bars of chart
            rects.each(function (d, i) {
                d3.select(this).append("rect")
                .attr("class", "stanine-band")
                .attr("x", function () { return paddingXLeft + paddingBarsLeft + i * barWidth + i * paddingBarsBetween; })
                .attr("y", function () { return h + paddingYTop; }) //for transition start value
                .attr("width", barWidth)
                .attr("fill", function () { return arrColor[i]; })
                .attr("stroke", "#484848")
                .attr("height", 0) //for transition start value
                .transition(t)
                .attr("y", function () {
                    var barHeight = h * d.percent * yAxisMultiplier / 100;
                    if (d.percent > 0 && barHeight < 2) {
                        barHeight = 2;
                    }
                    return h - barHeight  + paddingYTop;
                })
                .attr("height", function() {
                    var barHeight = h * d.percent * yAxisMultiplier / 100;
                    middleHeightOfAllColumns += barHeight;
                    if (d.percent > 0 && barHeight < 2) {
                        barHeight = 2;
                    }
                    return barHeight;
                });

/*
                dotCenterX = paddingXLeft + paddingBarsLeft + i * barWidth + i * paddingBarsBetween + barWidth / 2;
                //dotCenterY = paddingYTop + h - h * (d.percent_nationally / 100);
                dotCenterY = h - h * d.percent * yAxisMultiplier / 100 + paddingYTop;

                //add tooltip
                d3.select(this).append("rect")
                    .attr("class", "svg-tooltip")
                    .attr("x", dotCenterX - tooltipWidth / 2)
                    .attr("y", dotCenterY - tooltipHeight - 10)
                    .attr("rx", "6")
                    .attr("width", tooltipWidth)
                    .attr("height", tooltipHeight);

                d3.select(this).append("rect")
                    .attr("class", "svg-tooltip")
                    .attr("x", dotCenterX)
                    .attr("y", dotCenterY - 22)
                    .attr("transform", "rotate(45, " + dotCenterX + ", " + (dotCenterY - 22) + ")")
                    .attr("width", "14")
                    .attr("height", "14");

                d3.select(this).append("text")
                    .attr("class", "svg-tooltip")
                    .attr("x", dotCenterX)
                    .attr("y", dotCenterY - tooltipHeight + tooltipHeight / 2 - 5)
                    .attr("text-anchor", "middle")
                    //.text(d.percent_nationally);
                    .text(d.number);
*/
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

            //add axis
            var xScale = d3.scaleLinear()
                .domain([0, 3])
                .range([0, w - paddingXLeft - paddingXRight]);

            var yScale = d3.scaleLinear()
                //.domain([0, 100])
                //.domain([0, 40])
                .domain([0, (100 / yAxisMultiplier)])
                .range([h, 0]);

            var yAxis = d3.axisLeft()
                .scale(yScale)
                //.tickFormat("")
                .tickSize(-4)
                //.ticks(10);
                .ticks(4);

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
            });

            var dotRadius = 5;
            //var plotsNum = dataset.length;            

            //add dots
            var dots = svg.selectAll("circle")
                .data(dataset)
                .enter()
                .append("g");
                //.attr("class", "svg-tooltip-wrapper"); //uncomment for tooltips of dots

            dots.each(function (d, i) {
                dotCenterX = paddingXLeft + paddingBarsLeft + i * barWidth + i * paddingBarsBetween + barWidth / 2;
                dotCenterY = paddingYTop + h - h * (d.percent_nationally * yAxisMultiplier / 100);

                d3.select(this).append("circle")
                    .attr("cx", dotCenterX)
                    .attr("cy", dotCenterY)
                    .attr("r", dotRadius + 1)
                    .attr("fill", "#ffffff");

                d3.select(this).append("circle")
                    .attr("cx", dotCenterX)
                    .attr("cy", dotCenterY)
                    .attr("r", dotRadius)
                    .attr("fill", "#898d8d");

/*
                //uncomment for tooltips of dots
                //add tooltip
                d3.select(this).append("rect")
                    .attr("class", "svg-tooltip")
                    .attr("x", dotCenterX - tooltipWidth / 2)
                    .attr("y", dotCenterY - tooltipHeight - 10)
                    .attr("rx", "6")
                    .attr("width", tooltipWidth)
                    .attr("height", tooltipHeight);

                d3.select(this).append("rect")
                    .attr("class", "svg-tooltip")
                    .attr("x", dotCenterX)
                    .attr("y", dotCenterY - 22)
                    .attr("transform", "rotate(45, " + dotCenterX + ", " + (dotCenterY - 22) + ")")
                    .attr("width", "14")
                    .attr("height", "14");

                d3.select(this).append("text")
                    .attr("class", "svg-tooltip")
                    .attr("x", dotCenterX)
                    .attr("y", dotCenterY - tooltipHeight + tooltipHeight/2 - 5)
                    .attr("text-anchor", "middle")
                    .text(d.percent_nationally);
*/
            });

            //add line
            var line = d3.line()
                .x(function (d, i) { return paddingXLeft + paddingBarsLeft + i * barWidth + i * paddingBarsBetween + barWidth / 2; }) // set the x values for the line generator
                .y(function (d) { return paddingYTop + h - h * (d.percent_nationally * yAxisMultiplier / 100); }) // set the y values for the line generator 
                //.curve(d3.curveLinear);
                .curve(d3.curveMonotoneX); // apply smoothing to the line

            svg.append("path")
                .datum(dataset) //binds data to the line 
                .attr("class", "line")
                .attr("stroke", "#898d8d")
                .attr("stroke-width", "1px")
                .attr("fill", "none")
                .attr("d", line); // calls the line generator 


            var rects2 = svg.selectAll("rect2")
                .data(dataset)
                .enter()
                .append("g")
                .attr("role", "button")
                .attr("class", "svg-tooltip-wrapper");

            rects2.each(function (d, i) {
                d3.select(this).attr("data-stanine", i + 1);

                d3.select(this).append("rect")
                    .attr("class", "stanine-band-transparent")
                    .attr("x", function () { return paddingXLeft + paddingBarsLeft + i * barWidth + i * paddingBarsBetween; })
                    .attr("y", function () { return h + paddingYTop; }) //for transition start value
                    .attr("width", barWidth)
                    .attr("fill", "transparent")
                    //.attr("stroke", "#484848")
                    .attr("height", 0) //for transition start value
                    .transition(t)
                    .attr("y", function () {
                        var barHeight = h * d.percent * yAxisMultiplier / 100;
                        if (d.percent > 0 && barHeight < 2) {
                            barHeight = 2;
                        }
                        return h - barHeight + paddingYTop;
                    })
                    .attr("height", function () {
                        var barHeight = h * d.percent * yAxisMultiplier / 100;
                        middleHeightOfAllColumns += barHeight;
                        if (d.percent > 0 && barHeight < 2) {
                            barHeight = 2;
                        }
                        return barHeight;
                    });

                dotCenterX = paddingXLeft + paddingBarsLeft + i * barWidth + i * paddingBarsBetween + barWidth / 2;
                //dotCenterY = paddingYTop + h - h * (d.percent_nationally / 100);
                dotCenterY = h - h * d.percent * yAxisMultiplier / 100 + paddingYTop;
                if (dotCenterY < paddingYTop + 40) {
                    dotCenterY = 32;
                }

                //add tooltip
                d3.select(this).append("rect")
                    .attr("class", "svg-tooltip")
                    .attr("x", dotCenterX - tooltipWidth / 2)
                    .attr("y", dotCenterY - tooltipHeight - 10)
                    .attr("rx", "6")
                    .attr("width", tooltipWidth)
                    .attr("height", tooltipHeight);

                d3.select(this).append("rect")
                    .attr("class", "svg-tooltip")
                    .attr("x", dotCenterX)
                    .attr("y", dotCenterY - 22)
                    .attr("transform", "rotate(45, " + dotCenterX + ", " + (dotCenterY - 22) + ")")
                    .attr("width", "14")
                    .attr("height", "14");

                d3.select(this).append("text")
                    .attr("class", "svg-tooltip")
                    .attr("x", dotCenterX)
                    .attr("y", dotCenterY - tooltipHeight + tooltipHeight / 2 - 5)
                    .attr("text-anchor", "middle")
                    //.text(d.percent_nationally);
                    //.text(d.number);
                    .text(d.percent + "%");
            });
        }
	}
}();


//jQuery Plugin
(function ($) {
    //Dashboard Charts
    $.fn.BarChartStanine = function (options) {
        var settings = $.extend({
            'data': ""
        }, options);
        return this.each(function () {
            DashboardD3Charts.BarChartStanine(this, settings.data, options.stanineType);
        });
    };
})(jQuery);
