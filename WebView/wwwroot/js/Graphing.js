function makeBasicChart(raw_data) {

    var xScale = new Plottable.Scales.Linear();
    var yScale = new Plottable.Scales.Linear();

    var xAxis = new Plottable.Axes.Numeric(xScale, "bottom");
    var yAxis = new Plottable.Axes.Numeric(yScale, "left");

    var plot = new Plottable.Plots.Line();
    plot.x(function (d) { return d.x; }, xScale);
    plot.y(function (d) { return d.y; }, yScale);

    var data = [
    ];

    for (i = 0; i < raw_data[0].generations.length; i++) {
        var point = { x: raw_data[0].generations[i].run_position, y: raw_data[0].generations[i].population_size };
        data.push(point);
        console.log(point);
    }

    var dataset = new Plottable.Dataset(data);
    plot.addDataset(dataset);

    var chart = new Plottable.Components.Table([
        [yAxis, plot],
        [null, xAxis]
    ]);

    chart.renderTo("div#tutorial-result");

}

$(document).ready(function () {
    $.getJSON('/Results/API/Statistics', function (data) {
        //data is the JSON string
        console.log(data);
        makeBasicChart(data);
    });
    
});