var chart = null;
var data = [];
var graphNames = [];
var stripLines = [];
var phaseColor =
[
    "#FF0018",
    "#FFA52C",
    "#FFFF41",
    "#008018",
    "#0000F9",
    "#86007D",
];
const isObject = (obj) => {
    return Object.prototype.toString.call(obj) === "[object Object]";
};

window.onload = function() {
    initChart();

    //$.getJSON("../Assets/Graph/new-json-graph-test.graph", callback);
};

function initChart() {
    data = [];
    chart = new CanvasJS.Chart("chartContainer",
        {
            theme: "light2",
            exportEnabled: true,
            animationEnabled: true,
            zoomEnabled: true,
            zoomType: "xy",
            title: {
                text: "Accelerometer data"
            },
            toolTip: {
                content: "{name} <br/>{x}: <strong>{y}</strong>",
                animationEnabled: true 
            },
            data: data,
            axisY: {
                includeZero: false
            },
            axisX: {
                stripLines: stripLines
            }
        });
}

function drawGraph() {
    console.log(data);
    var text = document.getElementById("data").value;
    var json = JSON.parse(text);
    generatedCheckBoxes(json, "", "checkboxContainer");
    writeToDataPoint(json);

}

function createCheckbox(name, container) {
    var myDiv = document.getElementById(container);

    var checkbox = document.createElement("input");

    checkbox.type = "checkbox";
    checkbox.name = name;
    checkbox.value = "value";
    checkbox.checked = false;
    checkbox.id = name;

    var label = document.createElement("label");

    label.htmlFor = name;

    label.appendChild(document.createTextNode(name));

    myDiv.appendChild(checkbox);
    myDiv.appendChild(label);
    return checkbox;
}

function addCheckBox(name, container) {
    var checkbox = document.getElementById(name);
    if (!checkbox) {
        checkbox = createCheckbox(name, container);
        graphNames.push(name);
    }
    return checkbox;
}

function generatedCheckBoxes(json) {
    addCheckBox("X", "checkboxAxisContainer");
    addCheckBox("Y", "checkboxAxisContainer");
    addCheckBox("Z", "checkboxAxisContainer");
    Object.keys(json.frames[0]).forEach(key => {
        addCheckBox(key, "checkboxContainer");
    });
}

function updateVisibility() {
    graphNames.forEach(key => {
        // console.log(key);
        var checked = addCheckBox(key, "checkboxContainer").checked;
        var axis = "X";
        var checkedAxis = addCheckBox(axis, "checkboxAxisContainer").checked;
        if (data.find(data_ => data_.name == key + axis) != undefined) {
            var obj = data.find(data_ => data_.name == key + axis);
            obj.visible = checked && checkedAxis;
        }
        axis = "Y";
        checkedAxis = addCheckBox(axis, "checkboxAxisContainer").checked;
        if (data.find(data_ => data_.name == key + axis) != undefined) {
            var obj = data.find(data_ => data_.name == key + axis);
            obj.visible = checked && checkedAxis;
        }
        axis = "Z";
        checkedAxis = addCheckBox(axis, "checkboxAxisContainer").checked;
        if (data.find(data_ => data_.name == key + axis) != undefined) {
            var obj = data.find(data_ => data_.name == key + axis);
            obj.visible = checked && checkedAxis;
        }
        console.log(key);
        if (data.find(data_ => data_.name == key) != undefined) {
            console.log("find");
            var obj = data.find(data_ => data_.name == key);
            obj.visible = checked;
            console.log(obj.visible);
        }
    });
    chart.render();
    console.log(data);
}

function writeToDataPoint(json) {
    graphNames.forEach(key => {
        console.log(key);
        var dataPoints;
        if (isObject(json.frames[0][key]) && json.frames[0][key].x != undefined) {
            dataPoints = {};
            dataPoints["X"] = [];
            dataPoints["Y"] = [];
            dataPoints["Z"] = [];
            for (var i = 0; i < json.frames.length; i++) {
                dataPoints["X"].push({
                    x: json.frames[i].dt,
                    y: json.frames[i][key].x
                });
                dataPoints["Y"].push({
                    x: json.frames[i].dt,
                    y: json.frames[i][key].y
                });
                dataPoints["Z"].push({
                    x: json.frames[i].dt,
                    y: json.frames[i][key].z
                });
            }

            function addAxisData(axis, colorAxis) {
                console.log(key + axis);
                if (data.find(data_ => data_.name == key + axis) != undefined) {
                    var obj = data.find(data_ => data_.name == key + axis);
                    obj.dataPoints = dataPoints[axis];
                } else {
                    data.push({
                        type: "line",
                        showInLegend: true,
                        name: key + axis,
                        color: colorAxis,
                        dataPoints: dataPoints[axis]
                    });
                }
            }

            console.log(data);
            addAxisData("X", "red");
            addAxisData("Y", "green");
            addAxisData("Z", "blue");

        } else if (Array.isArray(json.frames[0][key])) {
            dataPoints = {};
            dataPoints["X"] = [];
            dataPoints["Y"] = [];
            dataPoints["Z"] = [];
            for (var i = 0; i < json.frames.length; i++) {
                dataPoints["X"].push({
                    label: json.frames[i].dt,
                    y: []
                });
                dataPoints["Y"].push({
                    label: json.frames[i].dt,
                    y: []
                });
                dataPoints["Z"].push({
                    label: json.frames[i].dt,
                    y: []
                });
                for (var j = 0; j < json.frames[i][key].length; j++) {
                    dataPoints["X"][i].y.push(json.frames[i][key][j].x);
                    dataPoints["Y"][i].y.push(json.frames[i][key][j].y);
                    dataPoints["Z"][i].y.push(json.frames[i][key][j].z);
                }
            }


            function addBoxAxisData(axis, colorAxis) {
                console.log(key + axis);
                if (data.find(data_ => data_.name == key + axis) != undefined) {
                    var obj = data.find(data_ => data_.name == key + axis);
                    obj.dataPoints = dataPoints[axis];
                } else {
                    data.push({
                        type: "boxAndWhisker",
                        showInLegend: true,
                        name: key + axis,
                        color: colorAxis,
                        dataPoints: dataPoints[axis]
                    });
                }
            }

            console.log(data);
            addBoxAxisData("X", "red");
            addBoxAxisData("Y", "green");
            addBoxAxisData("Z", "blue");

        } else {
            dataPoints = [];
            for (var i = 0; i < json.frames.length; i++) {
                dataPoints.push({
                    x: json.frames[i].dt,
                    y: json.frames[i][key]
                });
            }
            if (data.find(data_ => data_.name == key) != undefined) {
                var obj = data.find(data_ => data_.name == key);
                obj.dataPoints = dataPoints;

            } else {
                data.push({
                    type: "line",
                    showInLegend: true,
                    name: key,
                    dataPoints: dataPoints
                });
            }
        }
    });
    console.log(data);
    updateVisibility();
    chart.render();
}

function drawPhase() {
    stripLines = [];
    var text = document.getElementById("data").value;
    var json = JSON.parse(text);
    for (var i = 0; i < json.phases.length; i++) {
        var phase =
        {
            startValue: json.phases[i].startValue,
            endValue: json.phases[i].endValue,
            opacity: .3,
            color: phaseColor[i % 6],
            label: json.phases[i].phase + ":" + json.phases[i].valueCount,
            labelFontColor: "black",
            labelFontSize: "18",
            labelFontWeight: "bold"
        };
        console.log(stripLines.length);
        stripLines.push(phase);
    }
    chart.options.axisX.stripLines = stripLines;
    chart.render();
}

function generateAnalysis() {
}

function generateFixedAverage() {
}

function generatePhaseAverage() {
}