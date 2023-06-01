function getEmptyJsonStanine() {
    var json = {
        "values":
        [
            {
                "percent": 0,
                "percent_nationally": 0
            },
            {
                "percent": 0,
                "percent_nationally": 0
            },
            {
                "percent": 0,
                "percent_nationally": 0
            },
            {
                "percent": 0,
                "percent_nationally": 0
            },
            {
                "percent": 0,
                "percent_nationally": 0
            },
            {
                "percent": 0,
                "percent_nationally": 0
            },
            {
                "percent": 0,
                "percent_nationally": 0
            },
            {
                "percent": 0,
                "percent_nationally": 0
            },
            {
                "percent": 0,
                "percent_nationally": 0
            }
        ]
    };
    return json;
}

function getSampleJsonStanine() {
    var json = {
        "values":
        [
            {
                "percent": 5,
                "percent_nationally": 9
            },
            {
                "percent": 3,
                "percent_nationally": 12
            },
            {
                "percent": 2,
                "percent_nationally": 15
            },
            {
                "percent": 10,
                "percent_nationally": 21
            },
            {
                "percent": 16,
                "percent_nationally": 25
            },
            {
                "percent": 3,
                "percent_nationally": 21
            },
            {
                "percent": 2,
                "percent_nationally": 15
            },
            {
                "percent": 57,
                "percent_nationally": 12
            },
            {
                "percent": 48,
                "percent_nationally": 9
            }
        ]
    };
    return json;
}

function getEmptyJsonStanineTable(isScreener) {
    var values;

    if (typeof isScreener !== "undefined" && isScreener) {
        values = [
            {
                "content_area": "Total Score",
                "stanine1_per": 0,
                "stanine1_num": 0,
                "stanine2_per": 0,
                "stanine2_num": 0,
                "stanine3_per": 0,
                "stanine3_num": 0,
                "stanine4_per": 0,
                "stanine4_num": 0,
                "stanine5_per": 0,
                "stanine5_num": 0,
                "stanine6_per": 0,
                "stanine6_num": 0,
                "stanine7_per": 0,
                "stanine7_num": 0,
                "stanine8_per": 0,
                "stanine8_num": 0,
                "stanine9_per": 0,
                "stanine9_num": 0
            }
        ];
    } else {
        values = [
            {
                "content_area": "Verbal",
                "stanine1_per": 0,
                "stanine1_num": 0,
                "stanine2_per": 0,
                "stanine2_num": 0,
                "stanine3_per": 0,
                "stanine3_num": 0,
                "stanine4_per": 0,
                "stanine4_num": 0,
                "stanine5_per": 0,
                "stanine5_num": 0,
                "stanine6_per": 0,
                "stanine6_num": 0,
                "stanine7_per": 0,
                "stanine7_num": 0,
                "stanine8_per": 0,
                "stanine8_num": 0,
                "stanine9_per": 0,
                "stanine9_num": 0
            },
            {
                "content_area": "Quantitative",
                "stanine1_per": 0,
                "stanine1_num": 0,
                "stanine2_per": 0,
                "stanine2_num": 0,
                "stanine3_per": 0,
                "stanine3_num": 0,
                "stanine4_per": 0,
                "stanine4_num": 0,
                "stanine5_per": 0,
                "stanine5_num": 0,
                "stanine6_per": 0,
                "stanine6_num": 0,
                "stanine7_per": 0,
                "stanine7_num": 0,
                "stanine8_per": 0,
                "stanine8_num": 0,
                "stanine9_per": 0,
                "stanine9_num": 0
            },
            {
                "content_area": "Nonverbal",
                "stanine1_per": 0,
                "stanine1_num": 0,
                "stanine2_per": 0,
                "stanine2_num": 0,
                "stanine3_per": 0,
                "stanine3_num": 0,
                "stanine4_per": 0,
                "stanine4_num": 0,
                "stanine5_per": 0,
                "stanine5_num": 0,
                "stanine6_per": 0,
                "stanine6_num": 0,
                "stanine7_per": 0,
                "stanine7_num": 0,
                "stanine8_per": 0,
                "stanine8_num": 0,
                "stanine9_per": 0,
                "stanine9_num": 0
            },
            {
                //"content_area": "VQ Composite",
                "content_area": "Composite (VQ)",
                "stanine1_per": 0,
                "stanine1_num": 0,
                "stanine2_per": 0,
                "stanine2_num": 0,
                "stanine3_per": 0,
                "stanine3_num": 0,
                "stanine4_per": 0,
                "stanine4_num": 0,
                "stanine5_per": 0,
                "stanine5_num": 0,
                "stanine6_per": 0,
                "stanine6_num": 0,
                "stanine7_per": 0,
                "stanine7_num": 0,
                "stanine8_per": 0,
                "stanine8_num": 0,
                "stanine9_per": 0,
                "stanine9_num": 0
            },
            {
                //"content_area": "VN Composite",
                "content_area": "Composite (VN)",
                "stanine1_per": 0,
                "stanine1_num": 0,
                "stanine2_per": 0,
                "stanine2_num": 0,
                "stanine3_per": 0,
                "stanine3_num": 0,
                "stanine4_per": 0,
                "stanine4_num": 0,
                "stanine5_per": 0,
                "stanine5_num": 0,
                "stanine6_per": 0,
                "stanine6_num": 0,
                "stanine7_per": 0,
                "stanine7_num": 0,
                "stanine8_per": 0,
                "stanine8_num": 0,
                "stanine9_per": 0,
                "stanine9_num": 0
            },
            {
                //"content_area": "QN Composite",
                "content_area": "Composite (QN)",
                "stanine1_per": 0,
                "stanine1_num": 0,
                "stanine2_per": 0,
                "stanine2_num": 0,
                "stanine3_per": 0,
                "stanine3_num": 0,
                "stanine4_per": 0,
                "stanine4_num": 0,
                "stanine5_per": 0,
                "stanine5_num": 0,
                "stanine6_per": 0,
                "stanine6_num": 0,
                "stanine7_per": 0,
                "stanine7_num": 0,
                "stanine8_per": 0,
                "stanine8_num": 0,
                "stanine9_per": 0,
                "stanine9_num": 0
            },
            {
                //"content_area": "VQN Composite",
                "content_area": "Composite (VQN)",
                "stanine1_per": 0,
                "stanine1_num": 0,
                "stanine2_per": 0,
                "stanine2_num": 0,
                "stanine3_per": 0,
                "stanine3_num": 0,
                "stanine4_per": 0,
                "stanine4_num": 0,
                "stanine5_per": 0,
                "stanine5_num": 0,
                "stanine6_per": 0,
                "stanine6_num": 0,
                "stanine7_per": 0,
                "stanine7_num": 0,
                "stanine8_per": 0,
                "stanine8_num": 0,
                "stanine9_per": 0,
                "stanine9_num": 0
            }
        ];
    }

    var json = {
        "columns": [
            { "title": "Content Area", "field": "content_area" },
            { "title": "1", "field": "stanine1_num" },
            { "title": "2", "field": "stanine2_num" },
            { "title": "3", "field": "stanine3_num" },
            { "title": "4", "field": "stanine4_num" },
            { "title": "5", "field": "stanine5_num" },
            { "title": "6", "field": "stanine6_num" },
            { "title": "7", "field": "stanine7_num" },
            { "title": "8", "field": "stanine8_num" },
            { "title": "9", "field": "stanine9_num" }
        ],
        "percent_nationally":
        {
            "stanine1": 3,
            "stanine2": 7,
            "stanine3": 13,
            "stanine4": 17,
            "stanine5": 20,
            "stanine6": 17,
            "stanine7": 13,
            "stanine8": 7,
            "stanine9": 3
        },
        "values": values
    };
    return json;
}

function getSampleJsonStanineTable() {
    var json = {
        "columns": [
            { "title": "Content Area", "field": "content_area" },
            { "title": "1", "field": "stanine1_num" },
            { "title": "2", "field": "stanine2_num" },
            { "title": "3", "field": "stanine3_num" },
            { "title": "4", "field": "stanine4_num" },
            { "title": "5", "field": "stanine5_num" },
            { "title": "6", "field": "stanine6_num" },
            { "title": "7", "field": "stanine7_num" },
            { "title": "8", "field": "stanine8_num" },
            { "title": "9", "field": "stanine9_num" }
        ],
        "percent_nationally":
        {
            "stanine1": 9,
            "stanine2": 12,
            "stanine3": 15,
            "stanine4": 21,
            "stanine5": 25,
            "stanine6": 21,
            "stanine7": 15,
            "stanine8": 12,
            "stanine9": 9
        },
        "values": [
            {
                "content_area": "Verbal",
                "stanine1_per": 7,
                "stanine1_num": 7,
                "stanine2_per": 15,
                "stanine2_num": 15,
                "stanine3_per": 18,
                "stanine3_num": 18,
                "stanine4_per": 20,
                "stanine4_num": 20,
                "stanine5_per": 19,
                "stanine5_num": 19,
                "stanine6_per": 18,
                "stanine6_num": 18,
                "stanine7_per": 16,
                "stanine7_num": 16,
                "stanine8_per": 13,
                "stanine8_num": 13,
                "stanine9_per": 9,
                "stanine9_num": 9
            },
            {
                "content_area": "Quantitative",
                "stanine1_per": 6,
                "stanine1_num": 6,
                "stanine2_per": 15,
                "stanine2_num": 15,
                "stanine3_per": 13,
                "stanine3_num": 13,
                "stanine4_per": 18,
                "stanine4_num": 18,
                "stanine5_per": 20,
                "stanine5_num": 20,
                "stanine6_per": 15,
                "stanine6_num": 15,
                "stanine7_per": 14,
                "stanine7_num": 14,
                "stanine8_per": 10,
                "stanine8_num": 10,
                "stanine9_per": 6,
                "stanine9_num": 6
            },
            {
                "content_area": "Nonverbal",
                "stanine1_per": 8,
                "stanine1_num": 8,
                "stanine2_per": 16,
                "stanine2_num": 16,
                "stanine3_per": 16,
                "stanine3_num": 16,
                "stanine4_per": 17,
                "stanine4_num": 17,
                "stanine5_per": 19,
                "stanine5_num": 19,
                "stanine6_per": 15,
                "stanine6_num": 15,
                "stanine7_per": 13,
                "stanine7_num": 13,
                "stanine8_per": 9,
                "stanine8_num": 9,
                "stanine9_per": 5,
                "stanine9_num": 5
            },
            {
                "content_area": "VQ Composite",
                "stanine1_per": 5,
                "stanine1_num": 5,
                "stanine2_per": 14,
                "stanine2_num": 14,
                "stanine3_per": 20,
                "stanine3_num": 20,
                "stanine4_per": 17,
                "stanine4_num": 17,
                "stanine5_per": 20,
                "stanine5_num": 20,
                "stanine6_per": 13,
                "stanine6_num": 13,
                "stanine7_per": 18,
                "stanine7_num": 18,
                "stanine8_per": 8,
                "stanine8_num": 8,
                "stanine9_per": 3,
                "stanine9_num": 3
            },
            {
                "content_area": "VN Composite",
                "stanine1_per": 2,
                "stanine1_num": 2,
                "stanine2_per": 12,
                "stanine2_num": 12,
                "stanine3_per": 13,
                "stanine3_num": 13,
                "stanine4_per": 15,
                "stanine4_num": 15,
                "stanine5_per": 20,
                "stanine5_num": 20,
                "stanine6_per": 14,
                "stanine6_num": 14,
                "stanine7_per": 10,
                "stanine7_num": 10,
                "stanine8_per": 9,
                "stanine8_num": 9,
                "stanine9_per": 4,
                "stanine9_num": 4
            },
            {
                "content_area": "QN Composite",
                "stanine1_per": 4,
                "stanine1_num": 4,
                "stanine2_per": 8,
                "stanine2_num": 8,
                "stanine3_per": 3,
                "stanine3_num": 3,
                "stanine4_per": 12,
                "stanine4_num": 12,
                "stanine5_per": 14,
                "stanine5_num": 14,
                "stanine6_per": 15,
                "stanine6_num": 15,
                "stanine7_per": 12,
                "stanine7_num": 12,
                "stanine8_per": 13,
                "stanine8_num": 13,
                "stanine9_per": 19,
                "stanine9_num": 19
            },
            {
                "content_area": "VQN Composite",
                "stanine1_per": 5,
                "stanine1_num": 5,
                "stanine2_per": 3,
                "stanine2_num": 3,
                "stanine3_per": 2,
                "stanine3_num": 2,
                "stanine4_per": 16,
                "stanine4_num": 16,
                "stanine5_per": 20,
                "stanine5_num": 20,
                "stanine6_per": 3,
                "stanine6_num": 3,
                "stanine7_per": 2,
                "stanine7_num": 2,
                "stanine8_per": 17,
                "stanine8_num": 17,
                "stanine9_per": 18,
                "stanine9_num": 18
            }
        ]
    };
    return json;
}

function getEmptyJsonRosterTable() {
    var json = {
        "columns": [
            {
                "title": "Student Name",
                "title_full": "Student Name",
                "multi": 0,
                "field": "node_name"
            },
            {
                "title": "V",
                "title_full": "Verbal",
                "multi": 1,
                "fields": [
                    { "field": "AS0", "title": "AS", "title_full": "Age Stanine" }
                ]
            },
            {
                "title": "Q",
                "title_full": "Quantitative",
                "multi": 1,
                "fields": [
                    { "field": "AS1", "title": "AS", "title_full": "Age Stanine" }
                ]
            },
            {
                "title": "N",
                "title_full": "Nonverbal",
                "multi": 1,
                "fields": [
                    { "field": "AS2", "title": "AS", "title_full": "Age Stanine" }
                ]
            },
            {
                "title": "VQ",
                "title_full": "Composite Verbal and Quantitative",
                "multi": 1,
                "fields": [
                    { "field": "AS3", "title": "AS", "title_full": "Age Stanine" }
                ]
            },
            {
                "title": "VN",
                "title_full": "Compsite Verbal and Nonverbal",
                "multi": 1,
                "fields": [
                    { "field": "AS4", "title": "AS", "title_full": "Age Stanine" }
                ]
            },
            {
                "title": "QN",
                "title_full": "Composite Quantiative and Nonverbal",
                "multi": 1,
                "fields": [
                    { "field": "AS5", "title": "AS", "title_full": "Age Stanine" }
                ]
            },
            {
                "title": "VQN",
                "title_full": "Total composite score for Verbal, Quantitative, and Nonverbal",
                "multi": 1,
                "fields": [
                    { "field": "AS6", "title": "AS", "title_full": "Age Stanine" }
                ]
            }
        ],        
        "values":
        [
        ]
    };
    return json;
}

function getSampleJsonRosterTable(studentsNum) {
    var json = {
        //"roster_type": "compare",
        //"roster_level": "building",
        "columns": [
            {
                "title": "Student Name",
                "title_full": "Student Name",
                "multi": 0,
                "field": "node_name"
            },
            {
                "title": "V",
                "title_full": "Verbal",
                "multi": 1,
                "fields": [
                    { "field": "APR0", "title": "APR", "title_full": "Age Percentile" },
                    { "field": "AS0", "title": "AS", "title_full": "Age Stanine" },
                    { "field": "GPR0", "title": "GPR", "title_full": "Grade Percentile Rank" },
                    { "field": "GS0", "title": "GS", "title_full": "Grade Stanine" },
                    { "field": "USS0", "title": "USS", "title_full": "Uinversal Scale Score" },
                    { "field": "SAS0", "title": "SAS", "title_full": "Standard Age Score" },
                    { "field": "RS0", "title": "RS", "title_full": "Raw Score" },
                    { "field": "NA0", "title": "NA/NI", "title_full": "Number Attempted/Number Incorrect" }
                ]
            },
            {
                "title": "Q",
                "title_full": "Quantitative",
                "multi": 1,
                "fields": [
                    { "field": "APR1", "title": "APR", "title_full": "Age Percentile" },
                    { "field": "AS1", "title": "AS", "title_full": "Age Stanine" },
                    { "field": "GPR1", "title": "GPR", "title_full": "Grade Percentile Rank" },
                    { "field": "GS1", "title": "GS", "title_full": "Grade Stanine" },
                    { "field": "USS1", "title": "USS", "title_full": "Uinversal Scale Score" },
                    { "field": "SAS1", "title": "SAS", "title_full": "Standard Age Score" },
                    { "field": "RS1", "title": "RS", "title_full": "Raw Score" },
                    { "field": "NA1", "title": "NA/NI", "title_full": "Number Attempted/Number Incorrect" }
                ]
            },
            {
                "title": "N",
                "title_full": "Nonverbal",
                "multi": 1,
                "fields": [
                    { "field": "APR2", "title": "APR", "title_full": "Age Percentile" },
                    { "field": "AS2", "title": "AS", "title_full": "Age Stanine" },
                    { "field": "GPR2", "title": "GPR", "title_full": "Grade Percentile Rank" },
                    { "field": "GS2", "title": "GS", "title_full": "Grade Stanine" },
                    { "field": "USS2", "title": "USS", "title_full": "Uinversal Scale Score" },
                    { "field": "SAS2", "title": "SAS", "title_full": "Standard Age Score" },
                    { "field": "RS2", "title": "RS", "title_full": "Raw Score" },
                    { "field": "NA2", "title": "NA/NI", "title_full": "Number Attempted/Number Incorrect" }
                ]
            },
            {
                "title": "VQ",
                "title_full": "Composite Verbal and Quantitative",
                "multi": 1,
                "fields": [
                    { "field": "APR3", "title": "APR", "title_full": "Age Percentile" },
                    { "field": "AS3", "title": "AS", "title_full": "Age Stanine" },
                    { "field": "GPR3", "title": "GPR", "title_full": "Grade Percentile Rank" },
                    { "field": "GS3", "title": "GS", "title_full": "Grade Stanine" },
                    { "field": "USS3", "title": "USS", "title_full": "Uinversal Scale Score" },
                    { "field": "SAS3", "title": "SAS", "title_full": "Standard Age Score" },
                    { "field": "RS3", "title": "RS", "title_full": "Raw Score" },
                    { "field": "NA3", "title": "NA/NI", "title_full": "Number Attempted/Number Incorrect" }
                ]
            },
            {
                "title": "VN",
                "title_full": "Compsite Verbal and Nonverbal",
                "multi": 1,
                "fields": [
                    { "field": "APR4", "title": "APR", "title_full": "Age Percentile" },
                    { "field": "AS4", "title": "AS", "title_full": "Age Stanine" },
                    { "field": "GPR4", "title": "GPR", "title_full": "Grade Percentile Rank" },
                    { "field": "GS4", "title": "GS", "title_full": "Grade Stanine" },
                    { "field": "USS4", "title": "USS", "title_full": "Uinversal Scale Score" },
                    { "field": "SAS4", "title": "SAS", "title_full": "Standard Age Score" },
                    { "field": "RS4", "title": "RS", "title_full": "Raw Score" },
                    { "field": "NA4", "title": "NA/NI", "title_full": "Number Attempted/Number Incorrect" }
                ]
            },
            {
                "title": "QN",
                "title_full": "Composite Quantiative and Nonverbal",
                "multi": 1,
                "fields": [
                    { "field": "APR5", "title": "APR", "title_full": "Age Percentile" },
                    { "field": "AS5", "title": "AS", "title_full": "Age Stanine" },
                    { "field": "GPR5", "title": "GPR", "title_full": "Grade Percentile Rank" },
                    { "field": "GS5", "title": "GS", "title_full": "Grade Stanine" },
                    { "field": "USS5", "title": "USS", "title_full": "Uinversal Scale Score" },
                    { "field": "SAS5", "title": "SAS", "title_full": "Standard Age Score" },
                    { "field": "RS5", "title": "RS", "title_full": "Raw Score" },
                    { "field": "NA5", "title": "NA/NI", "title_full": "Number Attempted/Number Incorrect" }
                ]
            },
            {
                "title": "VQN",
                "title_full": "Total composite score for Verbal, Quantitative, and Nonverbal",
                "multi": 1,
                "fields": [
                    { "field": "APR6", "title": "APR", "title_full": "Age Percentile" },
                    { "field": "AS6", "title": "AS", "title_full": "Age Stanine" },
                    { "field": "GPR6", "title": "GPR", "title_full": "Grade Percentile Rank" },
                    { "field": "GS6", "title": "GS", "title_full": "Grade Stanine" },
                    { "field": "USS6", "title": "USS", "title_full": "Uinversal Scale Score" },
                    { "field": "SAS6", "title": "SAS", "title_full": "Standard Age Score" },
                    { "field": "RS6", "title": "RS", "title_full": "Raw Score" },
                    { "field": "NA6", "title": "NA/NI", "title_full": "Number Attempted/Number Incorrect" }
                ]
            }
        ],
        "values": [
            {
                "node_name": "Malin Quist",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 10, "AS1": 20, "AS2": 30, "AS3": 40, "AS4": 50, "AS5": 60, "AS6": 70,
                "APR0": 11, "APR1": 21, "APR2": 31, "APR3": 41, "APR4": 51, "APR5": 61, "APR6": 71,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Rustem Tolstobrov",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 11, "AS1": 22, "AS2": 33, "AS3": 44, "AS4": 55, "AS5": 66, "AS6": 77,
                "APR0": 12, "APR1": 23, "APR2": 34, "APR3": 45, "APR4": 56, "APR5": 67, "APR6": 78,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Kun Chang-Min",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 12, "AS1": 23, "AS2": 34, "AS3": 45, "AS4": 56, "AS5": 67, "AS6": 78,
                "APR0": 13, "APR1": 24, "APR2": 35, "APR3": 46, "APR4": 57, "APR5": 68, "APR6": 79,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Dameon Peterson",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 21, "AS1": 32, "AS2": 43, "AS3": 54, "AS4": 65, "AS5": 76, "AS6": 87,
                "APR0": 22, "APR1": 33, "APR2": 34, "APR3": 45, "APR4": 67, "APR5": 63, "APR6": 72,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Leslee Moss",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 31, "AS1": 42, "AS2": 53, "AS3": 64, "AS4": 75, "AS5": 86, "AS6": 97,
                "APR0": 32, "APR1": 6, "APR2": 45, "APR3": 52, "APR4": 57, "APR5": 12, "APR6": 24,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Cao Yu",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 14, "AS1": 25, "AS2": 36, "AS3": 47, "AS4": 58, "AS5": 69, "AS6": 70,
                "APR0": 1, "APR1": 21, "APR2": 14, "APR3": 16, "APR4": 18, "APR5": 32, "APR6": 54,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Kong Yijun",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 31, "AS1": 42, "AS2": 53, "AS3": 64, "AS4": 75, "AS5": 86, "AS6": 97,
                "APR0": 41, "APR1": 12, "APR2": 51, "APR3": 4, "APR4": 5, "APR5": 16, "APR6": 12,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Chongrak Narkhirunkanok",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 34, "AS1": 22, "AS2": 45, "AS3": 67, "AS4": 34, "AS5": 65, "AS6": 43,
                "APR0": 1, "APR1": 5, "APR2": 51, "APR3": 12, "APR4": 14, "APR5": 16, "APR6": 10,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Phakamile Sikali",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 21, "AS1": 32, "AS2": 43, "AS3": 54, "AS4": 65, "AS5": 76, "AS6": 87,
                "APR0": 3, "APR1": 6, "APR2": 12, "APR3": 13, "APR4": 19, "APR5": 23, "APR6": 31,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Hugo Assuncao",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 13, "AS1": 23, "AS2": 33, "AS3": 43, "AS4": 53, "AS5": 63, "AS6": 73,
                "APR0": 5, "APR1": 14, "APR2": 31, "APR3": 20, "APR4": 10, "APR5": 6, "APR6": 9,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Olivia Evans",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 13, "AS1": 23, "AS2": 33, "AS3": 43, "AS4": 53, "AS5": 63, "AS6": 73,
                "APR0": 14, "APR1": 5, "APR2": 15, "APR3": 10, "APR4": 21, "APR5": 34, "APR6": 9,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Bartolomej Dohnal",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 13, "AS1": 23, "AS2": 33, "AS3": 43, "AS4": 53, "AS5": 63, "AS6": 73,
                "APR0": 10, "APR1": 5, "APR2": 7, "APR3": 19, "APR4": 23, "APR5": 54, "APR6": 14,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Tiontay Carroll",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 13, "AS1": 23, "AS2": 33, "AS3": 43, "AS4": 53, "AS5": 63, "AS6": 73,
                "APR0": 1, "APR1": 19, "APR2": 20, "APR3": 13, "APR4": 5, "APR5": 8, "APR6": 1,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Jelena Denisova",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 13, "AS1": 23, "AS2": 33, "AS3": 43, "AS4": 53, "AS5": 63, "AS6": 73,
                "APR0": 9, "APR1": 19, "APR2": 14, "APR3": 23, "APR4": 10, "APR5": 8, "APR6": 9,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Monica Bottger",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 13, "AS1": 23, "AS2": 33, "AS3": 43, "AS4": 53, "AS5": 63, "AS6": 73,
                "APR0": 11, "APR1": 21, "APR2": 31, "APR3": 41, "APR4": 51, "APR5": 61, "APR6": 71,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Kwak Seong-Min",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 13, "AS1": 23, "AS2": 33, "AS3": 43, "AS4": 53, "AS5": 63, "AS6": 73,
                "APR0": 14, "APR1": 9, "APR2": 22, "APR3": 10, "APR4": 33, "APR5": 5, "APR6": 8,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Alex Walker",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 13, "AS1": 23, "AS2": 33, "AS3": 43, "AS4": 53, "AS5": 63, "AS6": 73,
                "APR0": 8, "APR1": 18, "APR2": 20, "APR3": 5, "APR4": 9, "APR5": 11, "APR6": 15,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Jacqueline Likoki",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 13, "AS1": 23, "AS2": 33, "AS3": 43, "AS4": 53, "AS5": 63, "AS6": 73,
                "APR0": 7, "APR1": 10, "APR2": 24, "APR3": 30, "APR4": 10, "APR5": 6, "APR6": 21,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Ren Xue",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 13, "AS1": 23, "AS2": 33, "AS3": 43, "AS4": 53, "AS5": 63, "AS6": 73,
                "APR0": 10, "APR1": 20, "APR2": 30, "APR3": 14, "APR4": 6, "APR5": 21, "APR6": 9,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            },
            {
                "node_name": "Gabriel Soares",
                "node_id": 635538,
                "node_type": 635538,
                "link": "/IowaFlexReporting/api/Dashboard/DrillDownLocations?id=635538&name=RIVB&type=building",
                "AS0": 13, "AS1": 23, "AS2": 33, "AS3": 43, "AS4": 53, "AS5": 63, "AS6": 73,
                "APR0": 10, "APR1": 14, "APR2": 5, "APR3": 1,"APR4": 19, "APR5": 20, "APR6": 11,
                "GPR0": 15, "GPR1": 25, "GPR2": 35, "GPR3": 46, "GPR4": 57, "GPR5": 68, "GPR6": 75,
                "GS0": 6, "GS1": 16, "GS2": 26, "GS3": 36, "GS4": 46, "GS5": 56, "GS6": 66,
                "USS0": 7, "USS1": 17, "USS2": 27, "USS3": 37, "USS4": 47, "USS5": 57, "USS6": 67,
                "SAS0": 8, "SAS1": 18, "SAS2": 28, "SAS3": 38, "SAS4": 48, "SAS5": 58, "SAS6": 68,
                "RS0": 9, "RS1": 19, "RS2": 29, "RS3": 39, "RS4": 49, "RS5": 59, "RS6": 69,
                "NA0": 90, "NA1": 91, "NA2": 92, "NA3": 93, "NA4": 94, "NA5": 95, "NA6": 96
            }
        ]
    };
    if (typeof studentsNum !== "undefined" && studentsNum !== null) {
        json.values.length = studentsNum;
    }    
    return json;
}

function getSampleJsonRightCardTable() {
    var json = {
        "columns": [
            { "title": "School", "field": "content_area" },
            { "title": "No.", "field": "number" },
            { "title": "", "field": "percent" }
        ],
        "values": [
            {
                "content_area": "Elm Wood Elementary",
                "number": 28
            },
            {
                "content_area": "Oak Wood Elementary",
                "number": 7
            },
            {
                "content_area": "Apple Wood Elementary",
                "number": 48
            },
            {
                "content_area": "Plum Wood Elementary",
                "number": 20
            },
            {
                "content_area": "Ash Wood Elementary",
                "number": 27
            },
            {
                "content_area": "Peach Wood Elementary",
                "number": 17
            },
            {
                "content_area": "Cherry Wood Elementary",
                "number": 25
            },
            {
                "content_area": "Sandle Wood Elementary",
                "number": 31
            },
            {
                "content_area": "Pine Wood Elementary",
                "number": 45
            },
            {
                "content_area": "Peak Wood Elementary",
                "number": 40
            },
            {
                "content_area": "Tiger Wood Elementary",
                "number": 24
            }
        ]
    };
    return json;
}

function getEmptyJsonRightCardTable2() {
    var json = {
        "columns": [
            { "title": "Ability Profile", "field": "ability_profile" },
            { "title": "Number of Students", "field": "number" }
        ],
        "values": [
        ]
    };
    return json;
}

function getSampleJsonRightCardTable2() {
    var json = {
        "columns": [
            { "title": "Ability Profile", "field": "ability_profile" },
            { "title": "Number of Students", "field": "number" }
        ],
        "values": [
            {
                "ability_profile": "1A, 2A, 3A",
                "number": 7,
                "link": "https://www.riversideinsights.com/apps/cogat?stanine=1&profile=1&strength=0&weakness=0"
            },
            {
                "ability_profile": "4A, 5A, 6A",
                "number": 1,
                "link": "https://www.riversideinsights.com/apps/cogat?stanine=4&profile=1&strength=0&weakness=0"
            },
            {
                "ability_profile": "4B (V-), 5B (V-), 6B (V-)",
                "number": 6,
                "link": "https://www.riversideinsights.com/apps/cogat?stanine=4&profile=2&strength=0&weakness=1"
            },
            {
                "ability_profile": "6C (Q-N+)",
                "number": 5,
                "link": "https://www.riversideinsights.com/apps/cogat?stanine=1&profile=1&strength=0&weakness=0"
            },
            {
                "ability_profile": "6C (V+N-)",
                "number": 1,
                "link": "https://www.riversideinsights.com/apps/cogat?stanine=1&profile=1&strength=0&weakness=0"
            },
            {
                "ability_profile": "7B (N-)",
                "number": 3,
                "link": "https://www.riversideinsights.com/apps/cogat?stanine=1&profile=1&strength=0&weakness=0"
            },
            {
                "ability_profile": "7B (N+)",
                "number": 1,
                "link": "https://www.riversideinsights.com/apps/cogat?stanine=1&profile=1&strength=0&weakness=0"
            },
            {
                "ability_profile": "7E (N-)",
                "number": 7,
                "link": "https://www.riversideinsights.com/apps/cogat?stanine=1&profile=1&strength=0&weakness=0"
            },
            {
                "ability_profile": "7E (V-)",
                "number": 1,
                "link": "https://www.riversideinsights.com/apps/cogat?stanine=1&profile=1&strength=0&weakness=0"
            },
            {
                "ability_profile": "9A",
                "number": 1,
                "link": "https://www.riversideinsights.com/apps/cogat?stanine=1&profile=1&strength=0&weakness=0"
            },
            {
                "ability_profile": "9E (V-)",
                "number": 5,
                "link": "https://www.riversideinsights.com/apps/cogat?stanine=1&profile=1&strength=0&weakness=0"
            }
        ]
    };
    return json;
}

function findAbilityGroup(ability) {
    var arrAbility = {
        "1A": "1A, 2A, 3A",
        "2A": "1A, 2A, 3A",
        "3A": "1A, 2A, 3A",

        "1B (N-)": "1B (N-), 2B (N-), 3B (N-)",
        "2B (N-)": "1B (N-), 2B (N-), 3B (N-)",
        "3B (N-)": "1B (N-), 2B (N-), 3B (N-)",

        "1B (Q-)": "1B (Q-), 2B (Q-), 3B (Q-)",
        "2B (Q-)": "1B (Q-), 2B (Q-), 3B (Q-)",
        "3B (Q-)": "1B (Q-), 2B (Q-), 3B (Q-)",

        "1B (V-)": "1B (V-), 2B (V-), 3B (V-)",
        "2B (V-)": "1B (V-), 2B (V-), 3B (V-)",
        "3B (V-)": "1B (V-), 2B (V-), 3B (V-)",

        "1B (N+)": "1B (N+), 2B (N+), 3B (N+)",
        "2B (N+)": "1B (N+), 2B (N+), 3B (N+)",
        "3B (N+)": "1B (N+), 2B (N+), 3B (N+)",

        "1B (Q+)": "1B (Q+), 2B (Q+), 3B (Q+)",
        "2B (Q+)": "1B (Q+), 2B (Q+), 3B (Q+)",
        "3B (Q+)": "1B (Q+), 2B (Q+), 3B (Q+)",

        "1B (V+)": "1B (V+), 2B (V+), 3B (V+)",
        "2B (V+)": "1B (V+), 2B (V+), 3B (V+)",
        "3B (V+)": "1B (V+), 2B (V+), 3B (V+)",

        "1C (N+Q-)": "1C (N+ Q-), 2C (N+ Q-), 3C (N+ Q-)",
        "2C (N+Q-)": "1C (N+ Q-), 2C (N+ Q-), 3C (N+ Q-)",
        "3C (N+Q-)": "1C (N+ Q-), 2C (N+ Q-), 3C (N+ Q-)",
        "1C (Q-N+)": "1C (N+ Q-), 2C (N+ Q-), 3C (N+ Q-)",
        "2C (Q-N+)": "1C (N+ Q-), 2C (N+ Q-), 3C (N+ Q-)",
        "3C (Q-N+)": "1C (N+ Q-), 2C (N+ Q-), 3C (N+ Q-)",

        "1C (N+V-)": "1C (N+ V-), 2C (N+ V-), 3C (N+ V-)",
        "2C (N+V-)": "1C (N+ V-), 2C (N+ V-), 3C (N+ V-)",
        "3C (N+V-)": "1C (N+ V-), 2C (N+ V-), 3C (N+ V-)",
        "1C (V-N+)": "1C (N+ V-), 2C (N+ V-), 3C (N+ V-)",
        "2C (V-N+)": "1C (N+ V-), 2C (N+ V-), 3C (N+ V-)",
        "3C (V-N+)": "1C (N+ V-), 2C (N+ V-), 3C (N+ V-)",

        "1C (Q+N-)": "1C (Q+ N-), 2C (Q+ N-), 3C (Q+ N-)",
        "2C (Q+N-)": "1C (Q+ N-), 2C (Q+ N-), 3C (Q+ N-)",
        "3C (Q+N-)": "1C (Q+ N-), 2C (Q+ N-), 3C (Q+ N-)",
        "1C (N-Q+)": "1C (Q+ N-), 2C (Q+ N-), 3C (Q+ N-)",
        "2C (N-Q+)": "1C (Q+ N-), 2C (Q+ N-), 3C (Q+ N-)",
        "3C (N-Q+)": "1C (Q+ N-), 2C (Q+ N-), 3C (Q+ N-)",

        "1C (Q+V-)": "1C (Q+ V-), 2C (Q+ V-), 3C (Q+ V-)",
        "2C (Q+V-)": "1C (Q+ V-), 2C (Q+ V-), 3C (Q+ V-)",
        "3C (Q+V-)": "1C (Q+ V-), 2C (Q+ V-), 3C (Q+ V-)",
        "1C (V-Q+)": "1C (Q+ V-), 2C (Q+ V-), 3C (Q+ V-)",
        "2C (V-Q+)": "1C (Q+ V-), 2C (Q+ V-), 3C (Q+ V-)",
        "3C (V-Q+)": "1C (Q+ V-), 2C (Q+ V-), 3C (Q+ V-)",

        "1C (V+N-)": "1C (V+ N-), 2C (V+ N-), 3C (V+ N-)",
        "2C (V+N-)": "1C (V+ N-), 2C (V+ N-), 3C (V+ N-)",
        "3C (V+N-)": "1C (V+ N-), 2C (V+ N-), 3C (V+ N-)",
        "1C (N-V+)": "1C (V+ N-), 2C (V+ N-), 3C (V+ N-)",
        "2C (N-V+)": "1C (V+ N-), 2C (V+ N-), 3C (V+ N-)",
        "3C (N-V+)": "1C (V+ N-), 2C (V+ N-), 3C (V+ N-)",

        "1C (V+Q-)": "1C (V+ Q-), 2C (V+ Q-), 3C (V+ Q-)",
        "2C (V+Q-)": "1C (V+ Q-), 2C (V+ Q-), 3C (V+ Q-)",
        "3C (V+Q-)": "1C (V+ Q-), 2C (V+ Q-), 3C (V+ Q-)",
        "1C (Q-V+)": "1C (V+ Q-), 2C (V+ Q-), 3C (V+ Q-)",
        "2C (Q-V+)": "1C (V+ Q-), 2C (V+ Q-), 3C (V+ Q-)",
        "3C (Q-V+)": "1C (V+ Q-), 2C (V+ Q-), 3C (V+ Q-)",

        "1E (N-)": "1E (N-), 2E (N-), 3E (N-)",
        "2E (N-)": "1E (N-), 2E (N-), 3E (N-)",
        "3E (N-)": "1E (N-), 2E (N-), 3E (N-)",

        "1E (Q-)": "1E (Q-), 2E (Q-), 3E (Q-)",
        "2E (Q-)": "1E (Q-), 2E (Q-), 3E (Q-)",
        "3E (Q-)": "1E (Q-), 2E (Q-), 3E (Q-)",

        "1E (V-)": "1E (V-), 2E (V-), 3E (V-)",
        "2E (V-)": "1E (V-), 2E (V-), 3E (V-)",
        "3E (V-)": "1E (V-), 2E (V-), 3E (V-)",

        "1E (N+)": "1E (N+), 2E (N+), 3E (N+)",
        "2E (N+)": "1E (N+), 2E (N+), 3E (N+)",
        "3E (N+)": "1E (N+), 2E (N+), 3E (N+)",

        "1E (N+Q-)": "1E (N+ Q-), 2E (N+ Q-), 3E (N+ Q-)",
        "2E (N+Q-)": "1E (N+ Q-), 2E (N+ Q-), 3E (N+ Q-)",
        "3E (N+Q-)": "1E (N+ Q-), 2E (N+ Q-), 3E (N+ Q-)",
        "1E (Q-N+)": "1E (N+ Q-), 2E (N+ Q-), 3E (N+ Q-)",
        "2E (Q-N+)": "1E (N+ Q-), 2E (N+ Q-), 3E (N+ Q-)",
        "3E (Q-N+)": "1E (N+ Q-), 2E (N+ Q-), 3E (N+ Q-)",

        "1E (N+V-)": "1E (N+ V-), 2E (N+ V-), 3E (N+ V-)",
        "2E (N+V-)": "1E (N+ V-), 2E (N+ V-), 3E (N+ V-)",
        "3E (N+V-)": "1E (N+ V-), 2E (N+ V-), 3E (N+ V-)",
        "1E (V-N+)": "1E (N+ V-), 2E (N+ V-), 3E (N+ V-)",
        "2E (V-N+)": "1E (N+ V-), 2E (N+ V-), 3E (N+ V-)",
        "3E (V-N+)": "1E (N+ V-), 2E (N+ V-), 3E (N+ V-)",

        "1E (Q+)": "1E (Q+), 2E (Q+), 3E (Q+)",
        "2E (Q+)": "1E (Q+), 2E (Q+), 3E (Q+)",
        "3E (Q+)": "1E (Q+), 2E (Q+), 3E (Q+)",

        "1E (Q+N-)": "1E (Q+ N-), 2E (Q+ N-), 3E (Q+ N-)",
        "2E (Q+N-)": "1E (Q+ N-), 2E (Q+ N-), 3E (Q+ N-)",
        "3E (Q+N-)": "1E (Q+ N-), 2E (Q+ N-), 3E (Q+ N-)",
        "1E (N-Q+)": "1E (Q+ N-), 2E (Q+ N-), 3E (Q+ N-)",
        "2E (N-Q+)": "1E (Q+ N-), 2E (Q+ N-), 3E (Q+ N-)",
        "3E (N-Q+)": "1E (Q+ N-), 2E (Q+ N-), 3E (Q+ N-)",

        "1E (Q+V-)": "1E (Q+ V-), 2E (Q+ V-), 3E (Q+ V-)",
        "2E (Q+V-)": "1E (Q+ V-), 2E (Q+ V-), 3E (Q+ V-)",
        "3E (Q+V-)": "1E (Q+ V-), 2E (Q+ V-), 3E (Q+ V-)",
        "1E (V-Q+)": "1E (Q+ V-), 2E (Q+ V-), 3E (Q+ V-)",
        "2E (V-Q+)": "1E (Q+ V-), 2E (Q+ V-), 3E (Q+ V-)",
        "3E (V-Q+)": "1E (Q+ V-), 2E (Q+ V-), 3E (Q+ V-)",

        "1E (V+)": "1E (V+), 2E (V+), 3E (V+)",
        "2E (V+)": "1E (V+), 2E (V+), 3E (V+)",
        "3E (V+)": "1E (V+), 2E (V+), 3E (V+)",

        "1E (V+N-)": "1E (V+ N-), 2E (V+ N-), 3E (V+ N-)",
        "2E (V+N-)": "1E (V+ N-), 2E (V+ N-), 3E (V+ N-)",
        "3E (V+N-)": "1E (V+ N-), 2E (V+ N-), 3E (V+ N-)",
        "1E (N-V+)": "1E (V+ N-), 2E (V+ N-), 3E (V+ N-)",
        "2E (N-V+)": "1E (V+ N-), 2E (V+ N-), 3E (V+ N-)",
        "3E (N-V+)": "1E (V+ N-), 2E (V+ N-), 3E (V+ N-)",

        "1E (V+Q-)": "1E (V+ Q-), 2E (V+ Q-), 3E (V+ Q-)",
        "2E (V+Q-)": "1E (V+ Q-), 2E (V+ Q-), 3E (V+ Q-)",
        "3E (V+Q-)": "1E (V+ Q-), 2E (V+ Q-), 3E (V+ Q-)",
        "1E (Q-V+)": "1E (V+ Q-), 2E (V+ Q-), 3E (V+ Q-)",
        "2E (Q-V+)": "1E (V+ Q-), 2E (V+ Q-), 3E (V+ Q-)",
        "3E (Q-V+)": "1E (V+ Q-), 2E (V+ Q-), 3E (V+ Q-)",

        "4A": "4A, 5A, 6A",
        "5A": "4A, 5A, 6A",
        "6A": "4A, 5A, 6A",

        "4B (N-)": "4B (N-), 5B (N-), 6B (N-)",
        "5B (N-)": "4B (N-), 5B (N-), 6B (N-)",
        "6B (N-)": "4B (N-), 5B (N-), 6B (N-)",

        "4B (Q-)": "4B (Q-), 5B (Q-), 6B (Q-)",
        "5B (Q-)": "4B (Q-), 5B (Q-), 6B (Q-)",
        "6B (Q-)": "4B (Q-), 5B (Q-), 6B (Q-)",

        "4B (V-)": "4B (V-), 5B (V-), 6B (V-)",
        "5B (V-)": "4B (V-), 5B (V-), 6B (V-)",
        "6B (V-)": "4B (V-), 5B (V-), 6B (V-)",

        "4B (N+)": "4B (N+), 5B (N+), 6B (N+)",
        "5B (N+)": "4B (N+), 5B (N+), 6B (N+)",
        "6B (N+)": "4B (N+), 5B (N+), 6B (N+)",

        "4B (Q+)": "4B (Q+), 5B (Q+), 6B (Q+)",
        "5B (Q+)": "4B (Q+), 5B (Q+), 6B (Q+)",
        "6B (Q+)": "4B (Q+), 5B (Q+), 6B (Q+)",

        "4B (V+)": "4B (V+), 5B (V+), 6B (V+)",
        "5B (V+)": "4B (V+), 5B (V+), 6B (V+)",
        "6B (V+)": "4B (V+), 5B (V+), 6B (V+)",

        "4C (N+Q-)": "4C (N+ Q-), 5C (N+ Q-), 6C (N+ Q-)",
        "5C (N+Q-)": "4C (N+ Q-), 5C (N+ Q-), 6C (N+ Q-)",
        "6C (N+Q-)": "4C (N+ Q-), 5C (N+ Q-), 6C (N+ Q-)",
        "4C (Q-N+)": "4C (N+ Q-), 5C (N+ Q-), 6C (N+ Q-)",
        "5C (Q-N+)": "4C (N+ Q-), 5C (N+ Q-), 6C (N+ Q-)",
        "6C (Q-N+)": "4C (N+ Q-), 5C (N+ Q-), 6C (N+ Q-)",

        "4C (N+V-)": "4C (N+ V-), 5C (N+ V-), 6C (N+ V-)",
        "5C (N+V-)": "4C (N+ V-), 5C (N+ V-), 6C (N+ V-)",
        "6C (N+V-)": "4C (N+ V-), 5C (N+ V-), 6C (N+ V-)",
        "4C (V-N+)": "4C (N+ V-), 5C (N+ V-), 6C (N+ V-)",
        "5C (V-N+)": "4C (N+ V-), 5C (N+ V-), 6C (N+ V-)",
        "6C (V-N+)": "4C (N+ V-), 5C (N+ V-), 6C (N+ V-)",

        "4C (Q+N-)": "4C (Q+ N-), 5C (Q+ N-), 6C (Q+ N-)",
        "5C (Q+N-)": "4C (Q+ N-), 5C (Q+ N-), 6C (Q+ N-)",
        "6C (Q+N-)": "4C (Q+ N-), 5C (Q+ N-), 6C (Q+ N-)",
        "4C (N-Q+)": "4C (Q+ N-), 5C (Q+ N-), 6C (Q+ N-)",
        "5C (N-Q+)": "4C (Q+ N-), 5C (Q+ N-), 6C (Q+ N-)",
        "6C (N-Q+)": "4C (Q+ N-), 5C (Q+ N-), 6C (Q+ N-)",

        "4C (Q+V-)": "4C (Q+ V-), 5C (Q+ V-), 6C (Q+ V-)",
        "5C (Q+V-)": "4C (Q+ V-), 5C (Q+ V-), 6C (Q+ V-)",
        "6C (Q+V-)": "4C (Q+ V-), 5C (Q+ V-), 6C (Q+ V-)",
        "4C (V-Q+)": "4C (Q+ V-), 5C (Q+ V-), 6C (Q+ V-)",
        "5C (V-Q+)": "4C (Q+ V-), 5C (Q+ V-), 6C (Q+ V-)",
        "6C (V-Q+)": "4C (Q+ V-), 5C (Q+ V-), 6C (Q+ V-)",

        "4C (V+N-)": "4C (V+ N-), 5C (V+ N-), 6C (V+ N-)",
        "5C (V+N-)": "4C (V+ N-), 5C (V+ N-), 6C (V+ N-)",
        "6C (V+N-)": "4C (V+ N-), 5C (V+ N-), 6C (V+ N-)",
        "4C (N-V+)": "4C (V+ N-), 5C (V+ N-), 6C (V+ N-)",
        "5C (N-V+)": "4C (V+ N-), 5C (V+ N-), 6C (V+ N-)",
        "6C (N-V+)": "4C (V+ N-), 5C (V+ N-), 6C (V+ N-)",

        "4C (V+Q-)": "4C (V+ Q-), 5C (V+ Q-), 6C (V+ Q-)",
        "5C (V+Q-)": "4C (V+ Q-), 5C (V+ Q-), 6C (V+ Q-)",
        "6C (V+Q-)": "4C (V+ Q-), 5C (V+ Q-), 6C (V+ Q-)",
        "4C (Q-V+)": "4C (V+ Q-), 5C (V+ Q-), 6C (V+ Q-)",
        "5C (Q-V+)": "4C (V+ Q-), 5C (V+ Q-), 6C (V+ Q-)",
        "6C (Q-V+)": "4C (V+ Q-), 5C (V+ Q-), 6C (V+ Q-)",

        "4E (N-)": "4E (N-), 5E (N-), 6E (N-)",
        "5E (N-)": "4E (N-), 5E (N-), 6E (N-)",
        "6E (N-)": "4E (N-), 5E (N-), 6E (N-)",

        "4E (Q-)": "4E (Q-), 5E (Q-), 6E (Q-)",
        "5E (Q-)": "4E (Q-), 5E (Q-), 6E (Q-)",
        "6E (Q-)": "4E (Q-), 5E (Q-), 6E (Q-)",

        "4E (V-)": "4E (V-), 5E (V-), 6E (V-)",
        "5E (V-)": "4E (V-), 5E (V-), 6E (V-)",
        "6E (V-)": "4E (V-), 5E (V-), 6E (V-)",

        "4E (N+)": "4E (N+), 5E (N+), 6E (N+)",
        "5E (N+)": "4E (N+), 5E (N+), 6E (N+)",
        "6E (N+)": "4E (N+), 5E (N+), 6E (N+)",

        "4E (N+Q-)": "4E (N+ Q-), 5E (N+ Q-), 6E (N+ Q-)",
        "5E (N+Q-)": "4E (N+ Q-), 5E (N+ Q-), 6E (N+ Q-)",
        "6E (N+Q-)": "4E (N+ Q-), 5E (N+ Q-), 6E (N+ Q-)",
        "4E (Q-N+)": "4E (N+ Q-), 5E (N+ Q-), 6E (N+ Q-)",
        "5E (Q-N+)": "4E (N+ Q-), 5E (N+ Q-), 6E (N+ Q-)",
        "6E (Q-N+)": "4E (N+ Q-), 5E (N+ Q-), 6E (N+ Q-)",

        "4E (N+V-)": "4E (N+ V-), 5E (N+ V-), 6E (N+ V-)",
        "5E (N+V-)": "4E (N+ V-), 5E (N+ V-), 6E (N+ V-)",
        "6E (N+V-)": "4E (N+ V-), 5E (N+ V-), 6E (N+ V-)",
        "4E (V-N+)": "4E (N+ V-), 5E (N+ V-), 6E (N+ V-)",
        "5E (V-N+)": "4E (N+ V-), 5E (N+ V-), 6E (N+ V-)",
        "6E (V-N+)": "4E (N+ V-), 5E (N+ V-), 6E (N+ V-)",

        "4E (Q+)": "4E (Q+), 5E (Q+), 6E (Q+)",
        "5E (Q+)": "4E (Q+), 5E (Q+), 6E (Q+)",
        "6E (Q+)": "4E (Q+), 5E (Q+), 6E (Q+)",

        "4E (Q+N-)": "4E (Q+ N-), 5E (Q+ N-), 6E (Q+ N-)",
        "5E (Q+N-)": "4E (Q+ N-), 5E (Q+ N-), 6E (Q+ N-)",
        "6E (Q+N-)": "4E (Q+ N-), 5E (Q+ N-), 6E (Q+ N-)",
        "4E (N-Q+)": "4E (Q+ N-), 5E (Q+ N-), 6E (Q+ N-)",
        "5E (N-Q+)": "4E (Q+ N-), 5E (Q+ N-), 6E (Q+ N-)",
        "6E (N-Q+)": "4E (Q+ N-), 5E (Q+ N-), 6E (Q+ N-)",

        "4E (Q+V-)": "4E (Q+ V-), 5E (Q+ V-), 6E (Q+ V-)",
        "5E (Q+V-)": "4E (Q+ V-), 5E (Q+ V-), 6E (Q+ V-)",
        "6E (Q+V-)": "4E (Q+ V-), 5E (Q+ V-), 6E (Q+ V-)",
        "4E (V-Q+)": "4E (Q+ V-), 5E (Q+ V-), 6E (Q+ V-)",
        "5E (V-Q+)": "4E (Q+ V-), 5E (Q+ V-), 6E (Q+ V-)",
        "6E (V-Q+)": "4E (Q+ V-), 5E (Q+ V-), 6E (Q+ V-)",

        "4E (V+)": "4E (V+), 5E (V+), 6E (V+)",
        "5E (V+)": "4E (V+), 5E (V+), 6E (V+)",
        "6E (V+)": "4E (V+), 5E (V+), 6E (V+)",

        "4E (V+N-)": "4E (V+ N-), 5E (V+ N-), 6E (V+ N-)",
        "5E (V+N-)": "4E (V+ N-), 5E (V+ N-), 6E (V+ N-)",
        "6E (V+N-)": "4E (V+ N-), 5E (V+ N-), 6E (V+ N-)",
        "4E (N-V+)": "4E (V+ N-), 5E (V+ N-), 6E (V+ N-)",
        "5E (N-V+)": "4E (V+ N-), 5E (V+ N-), 6E (V+ N-)",
        "6E (N-V+)": "4E (V+ N-), 5E (V+ N-), 6E (V+ N-)",

        "4E (V+Q-)": "4E (V+ Q-), 5E (V+ Q-), 6E (V+ Q-)",
        "5E (V+Q-)": "4E (V+ Q-), 5E (V+ Q-), 6E (V+ Q-)",
        "6E (V+Q-)": "4E (V+ Q-), 5E (V+ Q-), 6E (V+ Q-)",
        "4E (Q-V+)": "4E (V+ Q-), 5E (V+ Q-), 6E (V+ Q-)",
        "5E (Q-V+)": "4E (V+ Q-), 5E (V+ Q-), 6E (V+ Q-)",
        "6E (Q-V+)": "4E (V+ Q-), 5E (V+ Q-), 6E (V+ Q-)",

        "7A": "7A, 8A",
        "8A": "7A, 8A",

        "7B (N-)": "7B (N-), 8B (N-)",
        "8B (N-)": "7B (N-), 8B (N-)",

        "7B (Q-)": "7B (Q-), 8B (Q-)",
        "8B (Q-)": "7B (Q-), 8B (Q-)",

        "7B (V-)": "7B (V-), 8B (V-)",
        "8B (V-)": "7B (V-), 8B (V-)",

        "7B (N+)": "7B (N+), 8B (N+)",
        "8B (N+)": "7B (N+), 8B (N+)",

        "7B (Q+)": "7B (Q+), 8B (Q+)",
        "8B (Q+)": "7B (Q+), 8B (Q+)",

        "7B (V+)": "7B (V+), 8B (V+)",
        "8B (V+)": "7B (V+), 8B (V+)",

        "7C (N+Q-)": "7C (N+ Q-), 8C (N+ Q-), 9C (N+ Q-)",
        "8C (N+Q-)": "7C (N+ Q-), 8C (N+ Q-), 9C (N+ Q-)",
        "9C (N+Q-)": "7C (N+ Q-), 8C (N+ Q-), 9C (N+ Q-)",
        "7C (Q-N+)": "7C (N+ Q-), 8C (N+ Q-), 9C (N+ Q-)",
        "8C (Q-N+)": "7C (N+ Q-), 8C (N+ Q-), 9C (N+ Q-)",
        "9C (Q-N+)": "7C (N+ Q-), 8C (N+ Q-), 9C (N+ Q-)",

        "7C (N+V-)": "7C (N+ V-), 8C (N+ V-), 9C (N+ V-)",
        "8C (N+V-)": "7C (N+ V-), 8C (N+ V-), 9C (N+ V-)",
        "9C (N+V-)": "7C (N+ V-), 8C (N+ V-), 9C (N+ V-)",
        "7C (V-N+)": "7C (N+ V-), 8C (N+ V-), 9C (N+ V-)",
        "8C (V-N+)": "7C (N+ V-), 8C (N+ V-), 9C (N+ V-)",
        "9C (V-N+)": "7C (N+ V-), 8C (N+ V-), 9C (N+ V-)",

        "7C (Q+N-)": "7C (Q+ N-), 8C (Q+ N-), 9C (Q+ N-)",
        "8C (Q+N-)": "7C (Q+ N-), 8C (Q+ N-), 9C (Q+ N-)",
        "9C (Q+N-)": "7C (Q+ N-), 8C (Q+ N-), 9C (Q+ N-)",
        "7C (N-Q+)": "7C (Q+ N-), 8C (Q+ N-), 9C (Q+ N-)",
        "8C (N-Q+)": "7C (Q+ N-), 8C (Q+ N-), 9C (Q+ N-)",
        "9C (N-Q+)": "7C (Q+ N-), 8C (Q+ N-), 9C (Q+ N-)",

        "7C (Q+V-)": "7C (Q+ V-), 8C (Q+ V-), 9C (Q+ V-)",
        "8C (Q+V-)": "7C (Q+ V-), 8C (Q+ V-), 9C (Q+ V-)",
        "9C (Q+V-)": "7C (Q+ V-), 8C (Q+ V-), 9C (Q+ V-)",
        "7C (V-Q+)": "7C (Q+ V-), 8C (Q+ V-), 9C (Q+ V-)",
        "8C (V-Q+)": "7C (Q+ V-), 8C (Q+ V-), 9C (Q+ V-)",
        "9C (V-Q+)": "7C (Q+ V-), 8C (Q+ V-), 9C (Q+ V-)",

        "7C (V+N-)": "7C (V+ N-), 8C (V+ N-), 9C (V+ N-)",
        "8C (V+N-)": "7C (V+ N-), 8C (V+ N-), 9C (V+ N-)",
        "9C (V+N-)": "7C (V+ N-), 8C (V+ N-), 9C (V+ N-)",
        "7C (N-V+)": "7C (V+ N-), 8C (V+ N-), 9C (V+ N-)",
        "8C (N-V+)": "7C (V+ N-), 8C (V+ N-), 9C (V+ N-)",
        "9C (N-V+)": "7C (V+ N-), 8C (V+ N-), 9C (V+ N-)",

        "7C (V+Q-)": "7C (V+ Q-), 8C (V+ Q-), 9C (V+ Q-)",
        "8C (V+Q-)": "7C (V+ Q-), 8C (V+ Q-), 9C (V+ Q-)",
        "9C (V+Q-)": "7C (V+ Q-), 8C (V+ Q-), 9C (V+ Q-)",
        "7C (Q-V+)": "7C (V+ Q-), 8C (V+ Q-), 9C (V+ Q-)",
        "8C (Q-V+)": "7C (V+ Q-), 8C (V+ Q-), 9C (V+ Q-)",
        "9C (Q-V+)": "7C (V+ Q-), 8C (V+ Q-), 9C (V+ Q-)",

        "7E (N-)": "7E (N-), 8E (N-)",
        "8E (N-)": "7E (N-), 8E (N-)",

        "7E (Q-)": "7E (Q-), 8E (Q-)",
        "8E (Q-)": "7E (Q-), 8E (Q-)",

        "7E (V-)": "7E (V-), 8E (V-)",
        "8E (V-)": "7E (V-), 8E (V-)",

        "7E (N+)": "7E (N+), 8E (N+)",
        "8E (N+)": "7E (N+), 8E (N+)",

        "7E (N+Q-)": "7E (N+ Q-), 8E (N+ Q-), 9E (N+ Q-)",
        "8E (N+Q-)": "7E (N+ Q-), 8E (N+ Q-), 9E (N+ Q-)",
        "9E (N+Q-)": "7E (N+ Q-), 8E (N+ Q-), 9E (N+ Q-)",
        "7E (Q-N+)": "7E (N+ Q-), 8E (N+ Q-), 9E (N+ Q-)",
        "8E (Q-N+)": "7E (N+ Q-), 8E (N+ Q-), 9E (N+ Q-)",
        "9E (Q-N+)": "7E (N+ Q-), 8E (N+ Q-), 9E (N+ Q-)",

        "7E (N+V-)": "7E (N+ V-), 8E (N+ V-), 9E (N+ V-)",
        "8E (N+V-)": "7E (N+ V-), 8E (N+ V-), 9E (N+ V-)",
        "9E (N+V-)": "7E (N+ V-), 8E (N+ V-), 9E (N+ V-)",
        "7E (V-N+)": "7E (N+ V-), 8E (N+ V-), 9E (N+ V-)",
        "8E (V-N+)": "7E (N+ V-), 8E (N+ V-), 9E (N+ V-)",
        "9E (V-N+)": "7E (N+ V-), 8E (N+ V-), 9E (N+ V-)",

        "7E (Q+)": "7E (Q+), 8E (Q+)",
        "8E (Q+)": "7E (Q+), 8E (Q+)",

        "7E (Q+N-)": "7E (Q+ N-), 8E (Q+ N-), 9E (Q+ N-)",
        "8E (Q+N-)": "7E (Q+ N-), 8E (Q+ N-), 9E (Q+ N-)",
        "9E (Q+N-)": "7E (Q+ N-), 8E (Q+ N-), 9E (Q+ N-)",
        "7E (N-Q+)": "7E (Q+ N-), 8E (Q+ N-), 9E (Q+ N-)",
        "8E (N-Q+)": "7E (Q+ N-), 8E (Q+ N-), 9E (Q+ N-)",
        "9E (N-Q+)": "7E (Q+ N-), 8E (Q+ N-), 9E (Q+ N-)",

        "7E (Q+V-)": "7E (Q+ V-), 8E (Q+ V-), 9E (Q+ V-)",
        "8E (Q+V-)": "7E (Q+ V-), 8E (Q+ V-), 9E (Q+ V-)",
        "9E (Q+V-)": "7E (Q+ V-), 8E (Q+ V-), 9E (Q+ V-)",
        "7E (V-Q+)": "7E (Q+ V-), 8E (Q+ V-), 9E (Q+ V-)",
        "8E (V-Q+)": "7E (Q+ V-), 8E (Q+ V-), 9E (Q+ V-)",
        "9E (V-Q+)": "7E (Q+ V-), 8E (Q+ V-), 9E (Q+ V-)",

        "7E (V+)": "7E (V+), 8E (V+)",
        "8E (V+)": "7E (V+), 8E (V+)",

        "7E (V+N-)": "7E (V+ N-), 8E (V+ N-), 9E (V+ N-)",
        "8E (V+N-)": "7E (V+ N-), 8E (V+ N-), 9E (V+ N-)",
        "9E (V+N-)": "7E (V+ N-), 8E (V+ N-), 9E (V+ N-)",
        "7E (N-V+)": "7E (V+ N-), 8E (V+ N-), 9E (V+ N-)",
        "8E (N-V+)": "7E (V+ N-), 8E (V+ N-), 9E (V+ N-)",
        "9E (N-V+)": "7E (V+ N-), 8E (V+ N-), 9E (V+ N-)",

        "7E (V+Q-)": "7E (V+ Q-), 8E (V+ Q-), 9E (V+ Q-)",
        "8E (V+Q-)": "7E (V+ Q-), 8E (V+ Q-), 9E (V+ Q-)",
        "9E (V+Q-)": "7E (V+ Q-), 8E (V+ Q-), 9E (V+ Q-)",
        "7E (Q-V+)": "7E (V+ Q-), 8E (V+ Q-), 9E (V+ Q-)",
        "8E (Q-V+)": "7E (V+ Q-), 8E (V+ Q-), 9E (V+ Q-)",
        "9E (Q-V+)": "7E (V+ Q-), 8E (V+ Q-), 9E (V+ Q-)"
    };
    var result;
    if (arrAbility.hasOwnProperty(ability)) {
        result = arrAbility[ability];
    } else {
        result = ability;
    }
    return result;
}

function getRosterAllColumnsStructure(arrSelectedContent) {
//console.log(arrSelectedContent);
    var arrTitle = ["V", "Q", "N", "VQ", "VN", "QN", "VQN"];
    var arrTitleFull = ["Verbal", "Quantitative", "Nonverbal", "Composite (VQ)", "Composite (VN)", "Composite (QN)", "Composite (VQN)"];
    var objColumn;

    var json = {
        "columns": [
            {
                "title": "Student Name",
                "title_full": "Student Name",
                "multi": 0,
                "field": "node_name",
                "fields": null
            }
        ],
        "values": [
        ]
    };

    for (var i = 0; i < arrSelectedContent.length; i++) {
        objColumn = {
            "title": arrTitle[arrSelectedContent[i]],
            "title_full": arrTitleFull[arrSelectedContent[i]],
            "multi": 1,
            "field": null,
            "fields": [
                {
                    "field": "AS" + i,
                    "title": "AS",
                    "title_full": "Age Stanine"
                },
                {
                    "field": "APR" + i,
                    "title": "APR",
                    "title_full": "Age Percentile"
                },
                {
                    "field": "GPR" + i,
                    "title": "GPR",
                    "title_full": "Grade Percentile Rank"
                },
                {
                    "field": "GS" + i,
                    "title": "GS",
                    "title_full": "Grade Stanine"
                },
                {
                    "field": "USS" + i,
                    "title": "USS",
                    "title_full": "Universal Scale Score"
                },
                {
                    "field": "SAS" + i,
                    "title": "SAS",
                    "title_full": "Standard Age Score"
                },
                {
                    "field": "RS" + i,
                    "title": "RS",
                    "title_full": "Raw Score"
                }
            ]
        }
        json.columns.push(objColumn);
    }

/*
    var json = {
        "columns": [
            {
                "title": "Student Name",
                "title_full": "Student Name",
                "multi": 0,
                "field": "node_name",
                "fields": null
            },
            {
                "title": "V",
                "title_full": "Verbal",
                "multi": 1,
                "field": null,
                "fields": [
                    {
                        "field": "AS0",
                        "title": "AS",
                        "title_full": "Age Stanine"
                    },
                    {
                        "field": "APR0",
                        "title": "APR",
                        "title_full": "Age Percentile"
                    },
                    {
                        "field": "GPR0",
                        "title": "GPR",
                        "title_full": "Grade Percentile Rank"
                    },
                    {
                        "field": "GS0",
                        "title": "GS",
                        "title_full": "Grade Stanine"
                    },
                    {
                        "field": "USS0",
                        "title": "USS",
                        "title_full": "Universal Scale Score"
                    },
                    {
                        "field": "SAS0",
                        "title": "SAS",
                        "title_full": "Standard Age Score"
                    },
                    {
                        "field": "RS0",
                        "title": "RS",
                        "title_full": "Raw Score"
                    }
                ]
            },
            {
                "title": "Q",
                "title_full": "Quantitative",
                "multi": 1,
                "field": null,
                "fields": [
                    {
                        "field": "AS1",
                        "title": "AS",
                        "title_full": "Age Stanine"
                    },
                    {
                        "field": "APR1",
                        "title": "APR",
                        "title_full": "Age Percentile"
                    },
                    {
                        "field": "GPR1",
                        "title": "GPR",
                        "title_full": "Grade Percentile Rank"
                    },
                    {
                        "field": "GS1",
                        "title": "GS",
                        "title_full": "Grade Stanine"
                    },
                    {
                        "field": "USS1",
                        "title": "USS",
                        "title_full": "Universal Scale Score"
                    },
                    {
                        "field": "SAS1",
                        "title": "SAS",
                        "title_full": "Standard Age Score"
                    },
                    {
                        "field": "RS1",
                        "title": "RS",
                        "title_full": "Raw Score"
                    }
                ]
            },
            {
                "title": "N",
                "title_full": "Nonverbal",
                "multi": 1,
                "field": null,
                "fields": [
                    {
                        "field": "AS2",
                        "title": "AS",
                        "title_full": "Age Stanine"
                    },
                    {
                        "field": "APR2",
                        "title": "APR",
                        "title_full": "Age Percentile"
                    },
                    {
                        "field": "GPR2",
                        "title": "GPR",
                        "title_full": "Grade Percentile Rank"
                    },
                    {
                        "field": "GS2",
                        "title": "GS",
                        "title_full": "Grade Stanine"
                    },
                    {
                        "field": "USS2",
                        "title": "USS",
                        "title_full": "Universal Scale Score"
                    },
                    {
                        "field": "SAS2",
                        "title": "SAS",
                        "title_full": "Standard Age Score"
                    },
                    {
                        "field": "RS2",
                        "title": "RS",
                        "title_full": "Raw Score"
                    }
                ]
            },
            {
                "title": "VQ",
                "title_full": "Composite (VQ)",
                "multi": 1,
                "field": null,
                "fields": [
                    {
                        "field": "AS3",
                        "title": "AS",
                        "title_full": "Age Stanine"
                    },
                    {
                        "field": "APR3",
                        "title": "APR",
                        "title_full": "Age Percentile"
                    },
                    {
                        "field": "GPR3",
                        "title": "GPR",
                        "title_full": "Grade Percentile Rank"
                    },
                    {
                        "field": "GS3",
                        "title": "GS",
                        "title_full": "Grade Stanine"
                    },
                    {
                        "field": "USS3",
                        "title": "USS",
                        "title_full": "Universal Scale Score"
                    },
                    {
                        "field": "SAS3",
                        "title": "SAS",
                        "title_full": "Standard Age Score"
                    },
                    {
                        "field": "RS3",
                        "title": "RS",
                        "title_full": "Raw Score"
                    }
                ]
            },
            {
                "title": "VN",
                "title_full": "Composite (VN)",
                "multi": 1,
                "field": null,
                "fields": [
                    {
                        "field": "AS4",
                        "title": "AS",
                        "title_full": "Age Stanine"
                    },
                    {
                        "field": "APR4",
                        "title": "APR",
                        "title_full": "Age Percentile"
                    },
                    {
                        "field": "GPR4",
                        "title": "GPR",
                        "title_full": "Grade Percentile Rank"
                    },
                    {
                        "field": "GS4",
                        "title": "GS",
                        "title_full": "Grade Stanine"
                    },
                    {
                        "field": "USS4",
                        "title": "USS",
                        "title_full": "Universal Scale Score"
                    },
                    {
                        "field": "SAS4",
                        "title": "SAS",
                        "title_full": "Standard Age Score"
                    },
                    {
                        "field": "RS4",
                        "title": "RS",
                        "title_full": "Raw Score"
                    }
                ]
            },
            {
                "title": "QN",
                "title_full": "Composite (QN)",
                "multi": 1,
                "field": null,
                "fields": [
                    {
                        "field": "AS5",
                        "title": "AS",
                        "title_full": "Age Stanine"
                    },
                    {
                        "field": "APR5",
                        "title": "APR",
                        "title_full": "Age Percentile"
                    },
                    {
                        "field": "GPR5",
                        "title": "GPR",
                        "title_full": "Grade Percentile Rank"
                    },
                    {
                        "field": "GS5",
                        "title": "GS",
                        "title_full": "Grade Stanine"
                    },
                    {
                        "field": "USS5",
                        "title": "USS",
                        "title_full": "Universal Scale Score"
                    },
                    {
                        "field": "SAS5",
                        "title": "SAS",
                        "title_full": "Standard Age Score"
                    },
                    {
                        "field": "RS5",
                        "title": "RS",
                        "title_full": "Raw Score"
                    }
                ]
            },
            {
                "title": "VQN",
                "title_full": "Composite (VQN)",
                "multi": 1,
                "field": null,
                "fields": [
                    {
                        "field": "AS6",
                        "title": "AS",
                        "title_full": "Age Stanine"
                    },
                    {
                        "field": "APR6",
                        "title": "APR",
                        "title_full": "Age Percentile"
                    },
                    {
                        "field": "GPR6",
                        "title": "GPR",
                        "title_full": "Grade Percentile Rank"
                    },
                    {
                        "field": "GS6",
                        "title": "GS",
                        "title_full": "Grade Stanine"
                    },
                    {
                        "field": "USS6",
                        "title": "USS",
                        "title_full": "Universal Scale Score"
                    },
                    {
                        "field": "SAS6",
                        "title": "SAS",
                        "title_full": "Standard Age Score"
                    },
                    {
                        "field": "RS6",
                        "title": "RS",
                        "title_full": "Raw Score"
                    }
                ]
            }
        ],
        "values": [            
        ]
    };
*/
    return json;
}
