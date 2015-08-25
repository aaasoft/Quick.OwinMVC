var createPieCharts = function (data) {
    var divName = data.divName;
    var dataProvider = data.dataProvider;
    var balloonText = data.balloonText;
    var radius = data.radius;
    var colors = data.colors;
    if (typeof colors === 'undefined') {
        colors = [" #28AB17", "#FFCC00"];
    }
    //º”‘ÿ±˝Õº
    var pieChart = new AmCharts.AmPieChart();
    pieChart.dataProvider = dataProvider;
    pieChart.titleField = "title";
    pieChart.valueField = "value";
    pieChart.colors = colors;
    pieChart.startDuration = 0;
    pieChart.labelsEnabled = false;
    pieChart.balloonText = balloonText;
    if (typeof radius !== 'undefined') {
        pieChart.radius = radius;
    }
    pieChartLegend = new AmCharts.AmLegend();
    pieChartLegend.align = "center";
    pieChartLegend.markerType = "square";
    pieChartLegend.switchable = false;

    pieChart.addLegend(pieChartLegend);
    pieChart.write(divName);

    return pieChart;
}