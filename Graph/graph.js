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
var XColor =
[
    "#FF0000",
    "#FF7A7A",
    "#800000",
    "#FF4D4D",
    "#B90000"
];
var YColor =
[
    "#00E60F",
    "#6EE676",
    "#006607",
    "#54FF60",
    "#00A30B"
];
var ZColor =
[
    "#0008FF",
    "#7A7FFF",
    "#000480",
    "#4D52FF",
    "#0005BD"
];
var realDataColor =
[
    "#FF4D4D",
    "#54FF60",
    "#B90000",
    "#00A30B"
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
                text: "Accelerometer data",
				fontSize: 30,
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
	if (name === "time") return false; 
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
            obj.showInLegend = checked && checkedAxis;
        }
        axis = "Y";
        checkedAxis = addCheckBox(axis, "checkboxAxisContainer").checked;
        if (data.find(data_ => data_.name == key + axis) != undefined) {
            var obj = data.find(data_ => data_.name == key + axis);
            obj.visible = checked && checkedAxis;
            obj.showInLegend = checked && checkedAxis;
        }
        axis = "Z";
        checkedAxis = addCheckBox(axis, "checkboxAxisContainer").checked;
        if (data.find(data_ => data_.name == key + axis) != undefined) {
            var obj = data.find(data_ => data_.name == key + axis);
            obj.visible = checked && checkedAxis;
            obj.showInLegend = checked && checkedAxis;
        }
        console.log(key);
        if (data.find(data_ => data_.name == key) != undefined) {
            console.log("find");
            var obj = data.find(data_ => data_.name == key);
            obj.visible = checked;
            obj.showInLegend = checked;
            console.log(obj.visible);
        }
    });
    chart.render();
    console.log(data);
}

function writeToDataPoint(json) {
	var timeMult = parseFloat(document.getElementById("timeMult").value);
	var timeAdd = parseFloat(document.getElementById("timeAdd").value);
	var keyIndex = 0;
    graphNames.forEach(key => {
        console.log(key);
		keyIndex++;
        var dataPoints;
        if (isObject(json.frames[0][key]) && json.frames[0][key].x != undefined) {
            dataPoints = {};
            dataPoints["X"] = [];
            dataPoints["Y"] = [];
            dataPoints["Z"] = [];
            for (var i = 0; i < json.frames.length; i++) {
                dataPoints["X"].push({
                    x: (json.frames[i].time + timeAdd) * timeMult,
                    y: json.frames[i][key].x
                });
                dataPoints["Y"].push({
                    x: (json.frames[i].time + timeAdd) * timeMult,
                    y: json.frames[i][key].y
                });
                dataPoints["Z"].push({
                    x: (json.frames[i].time + timeAdd) * timeMult,
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
            addAxisData("X", XColor[keyIndex%5]);
            addAxisData("Y", YColor[keyIndex%5]);
            addAxisData("Z", ZColor[keyIndex%5]);

        } else if (typeof json.frames[0][key] === 'number') {
            dataPoints = [];
            for (var i = 0; i < json.frames.length; i++) {
				var time = ((json.frames[i].time + timeAdd) * timeMult);
                dataPoints.push({
                    x: time,
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
                    color: realDataColor[keyIndex%4],
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
	var timeMult = parseFloat(document.getElementById("timeMult").value);
	var timeAdd = parseFloat(document.getElementById("timeAdd").value);
    stripLines = [];
    var text = document.getElementById("data").value;
    var json = JSON.parse(text);
    for (var i = 0; i < json.phases.length; i++) {
		if (isObject(json.phases[i]))
		{			
			var phase =
			{
				startValue: (json.phases[i].startValue + timeAdd) * timeMult,
				endValue: (json.phases[i].endValue + timeAdd) * timeMult,
				opacity: .3,
				color: phaseColor[i % 6],
				label: json.phases[i].phase + ":" + json.phases[i].valueCount,
				labelFontColor: "black",
				labelFontSize: "18",
				labelFontWeight: "bold"
			};
			console.log(phase);
			stripLines.push(phase);
		} else if (i < json.phases.length-1) {
			var phase =
			{
				startValue: (json.phases[i] + timeAdd) * timeMult,
				endValue: (json.phases[i+1] + timeAdd) * timeMult,
				opacity: .3,
				color: phaseColor[i % 6],
				labelFontColor: "black",
				labelFontSize: "18",
				labelFontWeight: "bold"
			};
			console.log(phase);
			stripLines.push(phase);
		}
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

function setTitle()
{
	chart.options.title.text = document.getElementById("titletext").value;
	chart.options.axisX.title = document.getElementById("xlegendtext").value;
	chart.options.axisY.title = document.getElementById("ylegendtext").value;
	chart.render();
	console.log(chart);
}