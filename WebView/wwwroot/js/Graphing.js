function makeBasicChart(raw_data, class_name) {

    var xScale = new Plottable.Scales.Linear();
    var yScale = new Plottable.Scales.Linear();

    var xAxis = new Plottable.Axes.Numeric(xScale, "bottom");
    var yAxis = new Plottable.Axes.Numeric(yScale, "left");

    var plot = new Plottable.Plots.Line();
    plot.x(function (d) { return d.x; }, xScale);
    plot.y(function (d) { return d.y; }, yScale);

    var data = [
    ];

    for (i = 0; i < raw_data.generations.length; i++) {
        var point = { x: raw_data.generations[i].run_position, y: raw_data.generations[i].population_size };
        data.push(point);
        console.log(point);
    }

    var dataset = new Plottable.Dataset(data);
    plot.addDataset(dataset);

    var chart = new Plottable.Components.Table([
        [yAxis, plot],
        [null, xAxis]
    ]);

    //"div#tutorial-result"
    chart.renderTo("div#" + class_name);
}

$(document).ready(function () {
    $.getJSON('/Results/API/Statistics', function (data) {
        // <div id="tutorial-result" class="plottable"  style="width:100%; min-height:15em"></div>
        console.log(data.length);
        for (j = 0; j < data.length; j++) {
            var element = document.createElement("div");
            element.id = "graph-child-" + j;
            console.log("Creating element: " + element.id);
            element.style.height = "15em";
            document.getElementById("graph-parent").appendChild(element);
            makeBasicChart(data[j], element.id);
        }
    });
    
});