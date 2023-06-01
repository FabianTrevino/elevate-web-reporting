function getEmptyJsonDomains() {
    var json =
    [
        {
            "title": "",
            "values":
            [
                {
                    "caption": "High Performers",
                    "number": 0,
                    "percent": 0
                },
                {
                    "caption": "Mid Performers",
                    "number": 0,
                    "percent": 0
                },
                {
                    "caption": "Low Performers",
                    "number": 0,
                    "percent": 0
                }
            ]
        }
    ];
    return json;
}

function getSampleJsonPerformanceScoresKto1() {
    var json =
    {
        "graph_ql_query": "{ user(userId:34567, locationIds:[1282261], level:\"DISTRICT\"){ level testEvents(testEventId:1762, districts:[1282261], selectedGrade:\"7\") { isLongitudinal testScore (grades:[\"7\"],districts:[1282261],buildings:[1282264]) { id subject standardScore scores { id type value performanceBands { id lower upper nOfStudents percent name npr standardScore } } } } } }",
        "subject": "Mathematics",
        "totalCount": 10,
        "is_longitudinal": false,
        "values": [
            {
                "PLDStage": "Pre-Emerging",
                "PLDStageNum": 1,
                "studentCount": 1,
                "percent": 10
            },
            {
                "PLDStage": "Emerging",
                "PLDStageNum": 2,
                "studentCount": 3,
                "percent": 30
            },
            {
                "PLDStage": "Beginning",
                "PLDStageNum": 3,
                "studentCount": 4,
                "percent": 40
            },
            {
                "PLDStage": "Transitional",
                "PLDStageNum": 4,
                "studentCount": 2,
                "percent": 20
            }/*,
            {
                "PLDStage": "Independent",
                "PLDStageNum": 5,
                "studentCount": 3,
                "percent": 30
            }*/
        ]
    };
    return json;
}

function getSampleJsonDonuts() {
    var json =
    {
        "graph_ql_query": "{ user(userId:34567, locationIds:[635504], level:\"DISTRICT\") { level testEvents(testEventId:6, districts:[635504]) { domainScores (grades:[\"4\"],districts:[635504],buildings:[635538]) { id desc performanceLevels { id desc nOfStudents percent } } } } }",
        "cards": [
            {
                "PLDStage": "Pre-Emerging",
                "PLDStageNum": 1,
                "values": [
                    {
                        "PLDLevel": 1,
                        "studentCount": 6,
                        "percent": 60
                    },
                    {
                        "PLDLevel": 2,
                        "studentCount": 4,
                        "percent": 40
                    }
                ]
            },
            {
                "PLDStage": "Emerging",
                "PLDStageNum": 2,
                "values": [
                    {
                        "PLDLevel": 1,
                        "studentCount": 69,
                        "percent": 69
                    },
                    {
                        "PLDLevel": 2,
                        "studentCount": 23,
                        "percent": 23.23
                    },
                    {
                        "PLDLevel": 3,
                        "studentCount": 7,
                        "percent": 7.07
                    }
                ]
            },
            {
                "PLDStage": "Beginning",
                "PLDStageNum": 3,
                "values": [
                    {
                        "PLDLevel": 1,
                        "studentCount": 56,
                        "percent": 56.76
                    },
                    {
                        "PLDLevel": 2,
                        "studentCount": 20,
                        "percent": 20.72
                    },
                    {
                        "PLDLevel": 3,
                        "studentCount": 22,
                        "percent": 22.52
                    }
                ]
            },
            {
                "PLDStage": "Transitioning",
                "PLDStageNum": 4,
                "values": [
                    {
                        "PLDLevel": 1,
                        "studentCount": 40,
                        "percent": 40.74
                    },
                    {
                        "PLDLevel": 2,
                        "studentCount": 30,
                        "percent": 30.87
                    }
                ]
            }
        ]
    };
    return json;
}

function getSampleJsonPldStageInfo() {
    var json = [
        {
            "pldName": "Pre-Emerging",
            "pldAltName": "Pre-Emerging",
            "pldDesc": "A typical Pre-Emerging  has not yet demonstrated a developing mastery of the core mathematical standards and practices of the Kindergarten level. The student can solve some addition and subtraction problems and may demonstrate an emerging concept of place value. The student can classify objects and may be able identify and analyze basic geometric shapes. "
        },
        {
            "pldName": "Emerging",
            "pldAltName": "Emerging",
            "pldDesc": "A typical Emerging student has demonstrated a developing mastery of the core mathematical standards and practices of the Kindergarten level. The student can count and compare one- and two-digit whole numbers in the range 0 to 19, solve addition and subtraction problems with them, and demonstrate an emerging concept of place value. The student can describe and compare attributes of measurements in terms of absolute and relative magnitude. The student can classify and count objects, identify and analyze geometric shapes as well as compose new shapes from existing ones. "
        },
        {
            "pldName": "Beginning",
            "pldAltName": "Beginning",
            "pldDesc": "A typical Beginning student has demonstrated mastery of the core mathematical standards and practices of the Kindergarten level and somewhat beyond. The student can count and compare whole numbers with extended range and fluency, solve addition and subtraction problems with developing strategies by using concepts of place value and understanding of mathematical operations. The student can describe and compare attributes of measurements with respect to length, time and related representations of data. The student can identify and analyze geometric shapes and their attributes and begin to reason with them."
        },
        {
            "pldName": "Transitioning",
            "pldAltName": "Transitioning",
            "pldDesc": "A typical Transitioning student has demonstrated full mastery of core mathematical standards and practices of Grade 1. The student can count and compare whole numbers up to 120 with fluency, solve addition and subtraction problems with developing strategies by using knowledge of mathematical operations and concepts of place value within 100. The student can describe, compare, and interpret attributes of measurements with respect to length, time and related concepts using iterative methods and classifications of data. The student can reason with geometric shapes and their attributes with fluency and regularity."
        },
        {
            "pldName": "Independent Problem Solver",
            "pldAltName": "Problem-Solver ",
            "pldDesc": "An independent Problem-Solver has demonstrated mastery of core mathematical standards and practices of Grades 1 and 2.  Students performing at this level demonstrate a solid understanding of the standards by making connections across grade-level standards.  Students can connect mathematical models to real-word problems.  They can solve word problems and use a variety of methods to compute sums and differences of whole numbers.  "
        }
    ];
    return json;
}

function getSampleJsonPldLevelInfo() {
    var json = {
        "pldLvlName": "Emerging",
        "canStmt": "show and name upper- and lowercase letters (RF.K.1)\nhear and say rhyming words. (RF.K.2)\nsay the sounds in words. (RK.F.3)\nfigure out what words mean (L.K.4)\nuse action words correctly (L.K.1)\nadd /s/ or /es/ to show more than one (L.K.1)\nadd words to a sentence to make it clearer (L.K.1)\nchange sounds to make new words. (RK.K.2)\nlook at words and see how they are the same or different (RF.K.3)",
        "readyStmt": "read high frequency words (RF.1.3)\nread first grade site words (RF.1.3)",
        "practiceStmt": "",
        "iCanDesc": "I CAN",
        "needDesc": "I NEED MORE PRACTICE TO",
        "readyDesc": "I AM READY TO",
        "performanceLevelDescriptor": {
            "pldName": "Emerging",
            "pldAltName": "Emergent reader",
            "pldDesc": "A typical Emergent reader demonstrates mastery of early foundational Reading skills. They demonstrate understanding of important print concepts, including recognizing upper- and lower-case letters. They show understanding of early phonological awareness skills such as recognizing rhyming words. Emergent readers demonstrate basic knowledge of early phonics and word recognition skills such as one-to-one letter-sound correspondence. These readers show understanding of words, including concepts such as simple verb tenses, spatial/directional language, and singular/plural words, by making associations with pictures when read to. They show evidence of possessing a basic vocabulary and can read common, high-frequency words by sight. "
        }
    };
    return json;
}

function getSampleJsonDifferentiatedReportHierarchy() {
    var json = [
        {
            "studentName": "John Smith",
            "studentId": "1",
            "studentExternalId": "1001",
            "classId": "101",
            "className": "Class 1",
            "pldStage": "Pre-Emerging",
            "pldStageNum": "1",
            "pldLevel": "1"
        },
        {
            "studentName": "Robert Smith",
            "studentId": "2",
            "studentExternalId": "1002",
            "classId": "102",
            "className": "Class 2",
            "pldStage": "Emerging",
            "pldStageNum": "2",
            "pldLevel": "2"
        },
        {
            "studentName": "John Smith",
            "studentId": "3",
            "studentExternalId": "1003",
            "classId": "101",
            "className": "Class 1",
            "pldStage": "Pre-Emerging",
            "pldStageNum": "1",
            "pldLevel": "1"
        }
    ];
    return json;
}

function getEmptyJsonQuantiles() {
    var json = {
        "values":
        [
            {
                "caption": "Level 1 (120-168)",
                "number": 0,
                "percent": 0,
                "range_band": "120-168"
            },
            {
                "caption": "Level 2 (169-180)",
                "number": 0,
                "percent": 0,
                "range_band": "169-180"
            },
            {
                "caption": "Level 3 (181-191)",
                "number": 0,
                "percent": 0,
                "range_band": "181-191"
            },
            {
                "caption": "Level 4 (192-204)",
                "number": 0,
                "percent": 0,
                "range_band": "192-204"
            },
            {
                "caption": "Level 5 (205-245)",
                "number": 0,
                "percent": 0,
                "range_band": "205-245"
            }
        ]
    };    
    return json;
}

function getEmptyJsonPieChartScore() {
    var json = {
        "national_percentile_rank": 0,
        "average_standard_score": 0
    };
    return json;
}

function getEmptyJsonRoster() {
    var json = {
        "title": "DISTRICT TEST SCORES",
        "headers":
            ["Location", "SS", "NPR"],
        "info": {
            "node_id": 0,
            "node_type": ""
        },
        "values":
        [
        /*
            {
                "node_name": "...",
                "node_id": 0,
                "node_type": "",
                "link": "#",
                "align": "",
                "bgcolor": ""
            }
        */
        ]
    };
    return json;
}
