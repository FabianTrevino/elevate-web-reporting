function getSampleJsonPerformanceBand(i) {
    var json = [
        {
            "title": "8/23/18",
            "values": [
                {
                    "caption": "Level 1",
                    "number": 10,
                    "percent": 10,
                    "range_band": "120:168",
                    "range": 1
                },
                {
                    "caption": "Level 2",
                    "number": 12,
                    "percent": 12,
                    "range_band": "169:180",
                    "range": 2
                },
                {
                    "caption": "Level 3",
                    "number": 18,
                    "percent": 18,
                    "range_band": "181:191",
                    "range": 3
                },
                {
                    "caption": "Level 4",
                    "number": 30,
                    "percent": 30,
                    "range_band": "192:204",
                    "range": 4
                },
                {
                    "caption": "Level 5",
                    "number": 40,
                    "percent": 40,
                    "range_band": "205:245",
                    "range": 5
                }
            ]
        },
        {
            "title": "1/14/19",
            "values": [
                {
                    "caption": "Level 1",
                    "number": 7,
                    "percent": 7,
                    "range_band": "120:168",
                    "range": 1
                },
                {
                    "caption": "Level 2",
                    "number": 13,
                    "percent": 13,
                    "range_band": "169:180",
                    "range": 2
                },
                {
                    "caption": "Level 3",
                    "number": 10,
                    "percent": 10,
                    "range_band": "181:191",
                    "range": 3
                },
                {
                    "caption": "Level 4",
                    "number": 25,
                    "percent": 25,
                    "range_band": "192:204",
                    "range": 4
                },
                {
                    "caption": "Level 5",
                    "number": 45,
                    "percent": 45,
                    "range_band": "205:245",
                    "range": 5
                }
            ]
        },
        {
            "title": "4/16/19",
            "values": [
                {
                    "caption": "Level 1",
                    "number": 3,
                    "percent": 3,
                    "range_band": "120:168",
                    "range": 1
                },
                {
                    "caption": "Level 2",
                    "number": 7,
                    "percent": 7,
                    "range_band": "169:180",
                    "range": 2
                },
                {
                    "caption": "Level 3",
                    "number": 9,
                    "percent": 9,
                    "range_band": "181:191",
                    "range": 3
                },
                {
                    "caption": "Level 4",
                    "number": 29,
                    "percent": 29,
                    "range_band": "192:204",
                    "range": 4
                },
                {
                    "caption": "Level 5",
                    "number": 52,
                    "percent": 52,
                    "range_band": "205:245",
                    "range": 5
                }
            ]
        }
    ];
    return json[i];
}

function getEmptyJsonGainsAnalysis() {
    var json = {
        "bands": [
            {
                "range": 1,
                "range_band": "120:168"
            },
            {
                "range": 2,
                "range_band": "169:180"
            },
            {
                "range": 3,
                "range_band": "181:191"
            },
            {
                "range": 4,
                "range_band": "192:204"
            },
            {
                "range": 5,
                "range_band": "205:245"
            }
        ],
        "values": [
        ]
    };
    return json;
}

function getSampleJsonGainsAnalysis() {
    var json = {
        "bands": [
            {
                "range": 1,
                "range_band": "120:168"
            },
            {
                "range": 2,
                "range_band": "169:180"
            },
            {
                "range": 3,
                "range_band": "181:191"
            },
            {
                "range": 4,
                "range_band": "192:204"
            },
            {
                "range": 5,
                "range_band": "205:245"
            }
        ],
        "values": [
            {
                "title": "8/23/18",
                "title_full": "8/23/18 test event",
                "da": 166,
                "sa": 162,
                "ca": 168
            },
            {
                "title": "1/14/19",
                "title_full": "1/14/19 test event",
                "da": 172,
                "sa": 168,
                "ca": 173
            },
            {
                "title": "4/16/19",
                "title_full": "4/16/19 test event",
                "da": 176,
                "sa": 172,
                "ca": 179
            }
        ]
    };
    return json;
}

function getSampleJsonRosterStudents() {
    var json = {
        "roster_type": "students",
        "bands": [
            {
                "range": 1,
                "range_band": "120:168"
            },
            {
                "range": 2,
                "range_band": "169:180"
            },
            {
                "range": 3,
                "range_band": "181:191"
            },
            {
                "range": 4,
                "range_band": "192:204"
            },
            {
                "range": 5,
                "range_band": "205:245"
            }
        ],
        "columns": [
            {
                "title": "Student Name",
                "title_full": "Student Name",
                "field": "node_name"
            },
            {
                "title": "8/23/18",
                "title_full": "8/23/18 test event",
                "field": "SS0"
            },
            {
                "title": "1/14/19",
                "title_full": "1/14/19 test event",
                "field": "SS1"
            },
            {
                "title": "4/16/19",
                "title_full": "4/16/19 test event",
                "field": "SS2"
            },
            {
                "title": "Gain",
                "title_full": "Gain",
                "field": "gain"
            }
        ],
        "values": [
            {
                "node_id": 1,
                "node_name": "Assuncao, Hugo",
                //"SS0": 175,
                "SS0": null,
                "SS1": 210,
                "SS2": 245,
                "gain": 70
            },
            {
                "node_id": 2,
                "node_name": "Chakarvati, Indu",
                "SS0": 197,
                "SS1": 221,
                "SS2": 245,
                "gain": 48
            },
            {
                "node_id": 3,
                "node_name": "Bottger, Monica",
                "SS0": 201,
                "SS1": 223,
                "SS2": 245,
                "gain": 44
            },
            {
                "node_id": 4,
                "node_name": "Kortum, Quinten",
                "SS0": 218,
                "SS1": 239,
                //"SS2": 260,
                "SS2": null,
                "gain": 42
            },
            {
                "node_id": 5,
                "node_name": "Bharwana, Thakurjeet",
                "SS0": 131,
                "SS1": 152,
                "SS2": 173,
                "gain": 42
            }
        ]
    };
    return json;
}
