var DashboardD3Charts = function () {
    return {
        ChartGainsAnalysis: function (element, data, loggedUserLevel) {
            var wSvg = 352;
            var hSvg = 150;
            var paddingXLeft = 25;
            var paddingXRight = 0;
            var paddingYTop = 5;
            var paddingYBottom = 10;
            var w = wSvg - paddingXLeft - paddingXRight;
            var h = hSvg - paddingYBottom - paddingYTop;
            var arrColor = ["#f9e9e9", "#feefe5", "#f9f2e5", "#ebf2ef", "#e5f1f5"];
            var xBegin = 42;
            var xStep = 81;
            if (data.values.length < 3) {
                xBegin = 57;
                xStep = 105;
            }

/*
            var t = d3.transition()
                .duration(300)
                .ease(d3.easeLinear);
*/

            var svg = d3.select(element)
                .append("svg")
                .attr("role", "img")
                .attr("aria-labelledby", "gains-analysis-title")
                .attr("width", wSvg)
                .attr("height", hSvg);

            svg.append("title")
                .attr("id", "gains-analysis-title")
                .text("Gains Analysis Graph");

            var arr = [], j;
            for (j = 0; j < data.values.length; j++) {
                if (loggedUserLevel === "CLASS") {
                    if (typeof data.values[j].ca !== "undefined" && data.values[j].ca !== null) arr.push(data.values[j].ca);
                }
                if (loggedUserLevel === "CLASS" || loggedUserLevel === "BUILDING") {
                    if (typeof data.values[j].sa !== "undefined" && data.values[j].sa !== null) arr.push(data.values[j].sa);
                }
                if (loggedUserLevel === "CLASS" || loggedUserLevel === "BUILDING" || loggedUserLevel === "DISTRICT") {
                    if (typeof data.values[j].da !== "undefined" && data.values[j].da !== null) arr.push(data.values[j].da);
                }
            }
            //var minY = Math.min(...arr);
            var minY = Math.min.apply(this, arr);
            //var maxY = Math.max(...arr);
            var maxY = Math.max.apply(this, arr);
            var padding = 5;
            if (maxY - minY > 50) {
                padding = 10;
            }
            minY = minY - padding;
            maxY = maxY + padding;
            minY = minY - (minY % padding);
            maxY = maxY + (maxY % padding);
            var diffY = maxY - minY;
            var ticsNumY = Math.round((maxY - minY) / 5);
            if (ticsNumY > 6) {
                ticsNumY = 6;
            }

            if (typeof data.bands !== "undefined" && data.bands !== null)
            {
                //add color background (range band)
                var rect = svg.selectAll("rect")
                    .data(data.bands)
                    .enter();

                var bandBegin, bandEnd;
                rect.each(function (d, i) {
                    arr = d.range_band.split(":");
                    bandBegin = Number(arr[0]);
                    if (i > 0) {
                        bandBegin--;
                    }
                    bandEnd = Number(arr[1]);
                    if (bandBegin <= maxY && bandEnd >= minY) {
                        d3.select(this).append("rect")
                            .attr("fill", arrColor[i])
                            .attr("x", paddingXLeft)
                            .attr("width", w)
                            .attr("y", (function () {
                                if (bandEnd > maxY) {
                                    bandEnd = maxY;
                                }
                                return paddingYTop + h - h * ((bandEnd - minY) / diffY);
                            }))
                            .attr("height", (function () {
                                if (bandBegin < minY) {
                                    bandBegin = minY;
                                }
                                return h * ((bandEnd - bandBegin) / diffY);
                            }));
                    }
                });
            }


            //add axis
            var xScale = d3.scaleLinear()
                //.domain([0, 3])
                .range([0, w]);

            var yScale = d3.scaleLinear()
                //.domain([0, (100 / yAxisMultiplier)])
                .domain([minY, maxY])
                .range([h, 0]);

            var yAxis = d3.axisLeft()
                .scale(yScale)
                //.tickFormat("")
                .tickSize(-4)
                //.ticks(4);
                .ticks(ticsNumY);

            if (data.values.length) {
                svg.append("g")
                    .attr("class", "axis axis_y")
                    .attr("aria-hidden", "true")
                    .attr("transform", "translate(" + paddingXLeft + ", " + paddingYTop + ")")
                    .call(yAxis);
            }

            var xAxis = d3.axisBottom()
                .scale(xScale)
                .tickFormat("")
                .tickSize(0)
                .ticks(0);

            if (data.values.length) {
                svg.append("g")
                    .attr("class", "axis axis_x")
                    .attr("aria-hidden", "true")
                    .attr("transform", "translate(" + paddingXLeft + "," + (h + paddingYTop) + ")")
                    .call(xAxis);
            }

            //add lines for circles (ca), squares (sa), stars (da)
            if (loggedUserLevel === "CLASS") {
                var line = d3.line()
                    .x(function (d, i) { return paddingXLeft + xBegin + i * xStep; })
                    .y(function (d) { return paddingYTop + h - h * ((d.ca - minY) / diffY); })
                    .curve(d3.curveLinear);
                //.curve(d3.curveMonotoneX); //apply smoothing to the line

                svg.append("path")
                    .datum(data.values)
                    .attr("class", "line")
                    .attr("stroke", "#58812c")
                    .attr("stroke-width", "1px")
                    .attr("fill", "none")
                    .attr("d", line);
            }

            if (loggedUserLevel === "CLASS" || loggedUserLevel === "BUILDING") {
                var line2 = d3.line()
                    .x(function (d, i) { return paddingXLeft + xBegin + i * xStep; })
                    .y(function (d) { return paddingYTop + h - h * ((d.sa - minY) / diffY); })
                    .curve(d3.curveLinear);

                svg.append("path")
                    .datum(data.values)
                    .attr("class", "line")
                    .attr("stroke", "#0e5b8b")
                    .attr("stroke-width", "1px")
                    .attr("fill", "none")
                    .attr("d", line2);
            }

            if (loggedUserLevel === "CLASS" || loggedUserLevel === "BUILDING" || loggedUserLevel === "DISTRICT") {
                var line3 = d3.line()
                    .x(function (d, i) { return paddingXLeft + xBegin + i * xStep; })
                    .y(function (d) { return paddingYTop + h - h * ((d.da - minY) / diffY); })
                    .curve(d3.curveLinear);

                svg.append("path")
                    .datum(data.values)
                    .attr("class", "line")
                    .attr("stroke", "#8b0e0e")
                    .attr("stroke-width", "1px")
                    .attr("fill", "none")
                    .attr("d", line3);
            }

            //add symbols: circles (ca), squares (sa), stars (da)
            var symbolSize = Math.round((150 - (maxY - minY)) / 20);
            if (symbolSize < 4) {
                symbolSize = 4;
            }
            if (symbolSize > 7) {
                symbolSize = 7;
            }
            //symbolSize = 8;
            //console.log(symbolSize);
            var symbolStrokeWidth = symbolSize / 12;
            if (symbolStrokeWidth > 1) {
                symbolStrokeWidth = 1;
            }
            var dotCenterX, dotCenterY;
            var star = new D3Star();
            var dots = svg.selectAll("circle")
                .data(data.values)
                .enter()
                .append("g");
                //.attr("class", "svg-tooltip-wrapper"); //uncomment for tooltips of dots

            dots.each(function (d, i) {
                dotCenterX = paddingXLeft + xBegin + i * xStep;

                if (loggedUserLevel === "CLASS") {
                    dotCenterY = paddingYTop + h - h * ((d.ca - minY) / diffY);
                    d3.select(this).append("circle")
                        .attr("cx", dotCenterX)
                        .attr("cy", dotCenterY)
                        .attr("r", symbolSize + 2)
                        .attr("stroke-width", symbolStrokeWidth)
                        .attr("stroke", "#fff")
                        .attr("fill", "#58812c");
                }

                if (loggedUserLevel === "CLASS" || loggedUserLevel === "BUILDING") {
                    dotCenterY = paddingYTop + h - h * ((d.sa - minY) / diffY);
                    d3.select(this).append("rect")
                        .attr("x", dotCenterX - symbolSize)
                        .attr("y", dotCenterY - symbolSize)
                        .attr("width", symbolSize * 2)
                        .attr("height", symbolSize * 2)
                        .attr("stroke-width", symbolStrokeWidth)
                        .attr("stroke", "#fff")
                        .attr("fill", "#0e5b8b");
                }

                if (loggedUserLevel === "CLASS" || loggedUserLevel === "BUILDING" || loggedUserLevel === "DISTRICT") {
                    dotCenterY = paddingYTop + h - h * ((d.da - minY) / diffY);
                    star.x(dotCenterX - symbolSize * 1.25).y(dotCenterY - symbolSize * 0.375).size(symbolSize * 2.625).value(1.0).starColor("#8b0e0e").borderWidth(symbolStrokeWidth);
                    //star.x(dotCenterX - 10).y(dotCenterY - 3).size(20).value(1.0).starColor("#8b0e0e").borderWidth(symbolStrokeWidth);
                    //star.x(0).y(7).size(20).value(1.0).starColor("#8b0e0e").borderWidth(1); //example for SVG file
                    svg.call(star);
                }

            });
        },

        RosterScale: function (element, data) {
            var scaleWidth = data.width * 400 / (data.max - data.min);
            var ticksNumber = scaleWidth / 47;
            if (ticksNumber > 400) {
                ticksNumber = 400;
            }

            var svg = d3.select(element)
                .append("svg")
                .attr("width", scaleWidth)
                .attr("height", 16);

            //add axis
            var xScale = d3.scaleLinear()
                //.domain([data.min, data.max])
                .domain([0, 400])
                .range([0, scaleWidth]);

            var xAxis = d3.axisTop()
                .scale(xScale)
                .tickSize(4)
                //.tickFormat("")
                .ticks(ticksNumber);

            svg.append("g")
                .attr("class", "axis axis_x")
                .attr("aria-hidden", "true")
                .attr("transform", "translate(0, 17)")
                .call(xAxis);

            $("#roster-scale").attr("style", "left: " + (0 - (Number(data.min) / 400 * scaleWidth)) + "px");
        }
	}
}();


//jQuery Plugin
(function ($) {
    //Dashboard Charts
    $.fn.ChartGainsAnalysis = function (options) {
        var settings = $.extend({
            'data': ""
        }, options);
        return this.each(function () {
            DashboardD3Charts.ChartGainsAnalysis(this, settings.data, options.loggedUserLevel);
        });
    };
    $.fn.RosterScale = function (options) {
        var settings = $.extend({
            'data': ""
        }, options);
        return this.each(function () {
            DashboardD3Charts.RosterScale(this, settings.data);
        });
    };
})(jQuery);



function D3Star() {
    var size = 20,
        x = 0,
        y = 0,
        value = 1.0, //Range is 0.0 - 1.0
        borderWidth = 3,
        borderColor = "#FFF",
        starColor = "#FFB500",
        backgroundColor = "white";

    function star(selection) {
        var line = d3.line().x(function (d) { return d.x; }).y(function (d) { return d.y; })
            //.interpolate("linear-closed"),
            .curve(d3.curveLinear),
            rad = function (deg) { return deg * Math.PI / 180; },
            cos = function (deg) { return Math.cos(rad(deg)); },
            sin = function (deg) { return Math.sin(rad(deg)); },
            tan = function (deg) { return Math.tan(rad(deg)); },
            n = size,
            m = n / 2,
            h = m * tan(36),
            k = h / sin(72),

            //(x, y) points at the leftmost point of the star, not the center
            coordinates = [
                { x: x, y: y },
                { x: x + k, y: y },
                { x: x + m, y: y - h },
                { x: x + n - k, y: y },
                { x: x + n, y: y },
                { x: x + n - k * cos(36), y: y + k * sin(36) },
                { x: x + n * cos(36), y: y + n * sin(36) },
                { x: x + m, y: y + h },
                { x: x + n - n * cos(36), y: y + n * sin(36) },
                { x: x + k * cos(36), y: y + k * sin(36) }
            ];

        //inside star
        //selection.append("path").attr("d", line(coordinates)).style({ "stroke-width": 0, "fill": starColor });
        selection.append("path").attr("d", line(coordinates)).attr("fill", starColor).attr("stroke-width", "0");

        //Rect for clipping
        //In order to avoid potential ID duplicates for clipping, clip-path is not used here
        //selection.append("rect").attr("x", x + (size * value)).attr("y", y - h).attr("width", size - size * value).attr("height", size).style("fill", backgroundColor);
        //selection.append("rect").attr("x", x + (size * value)).attr("y", y - h).attr("width", size - size * value).attr("height", size).attr("fill", backgroundColor);

        //border of the star
        //selection.append("path").attr("d", line(coordinates)).style({ "stroke-width": borderWidth, "fill": "none", "stroke": borderColor });
        selection.append("path").attr("d", line(coordinates)).attr("stroke-width", borderWidth).attr("fill", "none").attr("stroke", borderColor);
    }

    star.x = function (val) {
        x = val;
        return star;
    }

    star.y = function (val) {
        y = val;
        return star;
    }

    star.size = function (val) {
        size = val;
        return star;
    }

    //Range is 0.0 - 1.0. 0.0 shows, for example, an empty star
    star.value = function (val) {
        value = val;
        return star;
    }

    star.backgroundColor = function (val) {
        backgroundColor = val;
        return star;
    }

    star.borderWidth = function (val) {
        borderWidth = val;
        return star;
    }

    star.borderColor = function (val) {
        borderColor = val;
        return star;
    }

    star.starColor = function (val) {
        starColor = val;
        return star;
    }
/*
    star.isBorderRounded = function (val) {
        isBorderRounded = val;
        return star;
    }
*/
    return star;
}