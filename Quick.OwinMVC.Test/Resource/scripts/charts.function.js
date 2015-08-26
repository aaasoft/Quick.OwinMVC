var getValue = function(value,defaultValue){
    return typeof value !== 'undefined' ? value : defaultValue;
}

var createPieCharts = function (data) {
    var divName = data.divName;
    var dataProvider = data.dataProvider;
    var balloonText = data.balloonText;
    var radius = getValue(data.radius,"");
    var colors = getValue(data.colors , [" #28AB17", "#FFCC00"]);
    

    //加载饼图
    var pieChart = new AmCharts.AmPieChart();
    pieChart.dataProvider = dataProvider;
    pieChart.titleField = "title";
    pieChart.valueField = "value";
    pieChart.colors = colors;
    pieChart.startDuration = 0;
    pieChart.labelsEnabled = false;
    pieChart.balloonText = balloonText;
    pieChart.radius = radius;
    pieChartLegend = new AmCharts.AmLegend();
    pieChartLegend.align = "center";
    pieChartLegend.markerType = "square";
    pieChartLegend.switchable = false;

    pieChart.addLegend(pieChartLegend);
    pieChart.write(divName);

    return pieChart;
}

var createSerialCharts = function (data) {
    var divName = data.divName;
    var dataProvider = data.dataProvider;
    var valueFieldList = getValue(data.valueFieldList, [{}]);
    var categoryField = getValue(data.categoryField, "time");
    var backgroundColor = getValue(data.backgroundColor, "#FFFFFF");
    var dateFormat = getValue(data.dateFormat, "JJ:NN");
    var titleList = getValue(data.titleList, []);
    var lineColor = getValue(data.lineColor, "#FFCC00");
    var lineColorList = getValue(data.lineColorList, []);
    var fillColor = getValue(data.fillColor, "#FFCC00");
    var fillColorList = getValue(data.fillColorList, []);
    var fillAlphas = getValue(data.fillAlphas, 0.6);
    var graphType = getValue(data.graphType, "line");
    var lineThickness = getValue(data.lineThickness, 1);
    var bullet = getValue(data.bullet, "none");
    var markerType = getValue(data.markerType, "square");
    var valAxisGridColor = getValue(data.valAxisGridColor, "#DADADA");
    var catAxisGridColor = getValue(data.catAxisGridColor, "#DADADA");
    var valueUnit = getValue(data.valueUnit, "");
    var valAxisMaximum = getValue(data.valAxisMaximum, "");
    var valAxisMinimum = getValue(data.valAxisMinimum, "");
    var minPeriod = getValue(data.minPeriod, "mm");
    var categoryBalloonDateFormat = getValue(data.categoryBalloonDateFormat, "");
    var legendSwitchable = getValue(data.legendSwitchable, "false");
    var legendSwitchType = getValue(data.legendSwitchType, "x");

    //加载折线图
    var serialChart = new AmCharts.AmSerialChart();
    serialChart.pathToImages = "../resource/amcharts/images/";
    if (titleList.length != 0) {
        serialChart.marginTop = 15;
        serialChart.marginLeft = 80;
        serialChart.marginRight = 50;
    } else {
        serialChart.marginTop = 0;
        serialChart.marginLeft = 0;
        serialChart.marginRight = 0;
        serialChart.marginBottom = 0;
    }
    serialChart.dataProvider = dataProvider;
    serialChart.categoryField = categoryField;
    serialChart.backgroundAlpha = 1;              //背景颜色透明度 同下行组合设置背景颜色
    serialChart.backgroundColor = backgroundColor;       //设置背景颜色
    serialChart.zoomOutText = "显示所有";

    var serialChartGraph0 = null;
    for (var index = 0; index < valueFieldList.length; index++) {
        var serialChartGraph = new AmCharts.AmGraph();
        if (index == 0) {
            serialChartGraph0 = serialChartGraph;
        }
        //线的颜色
        if (lineColorList.length == 0) {
            serialChartGraph.lineColor = lineColor;
        } else {
            serialChartGraph.lineColor = lineColorList[index];
        }
        //线型区域填充颜色
        if (fillColorList.length == 0) {
            serialChartGraph.fillColors = fillColor;
        } else {
            serialChartGraph.fillColors = fillColorList[index];
        }
        serialChartGraph.fillAlphas = fillAlphas;                       //填充透明度  
        serialChartGraph.valueField = valueFieldList[index];	//曲线值
        serialChartGraph.type = graphType;				//曲线类型
        serialChartGraph.lineThickness = lineThickness;                   //曲线宽度
        if (titleList.length != 0) {
            serialChartGraph.balloonColor = "#13CC00";                //弹出框颜色
            serialChartGraph.balloonText = titleList[index] + " [[value]] " + valueUnit;
            serialChartGraph.visibleInLegend = true;               //Legend是否可见
            serialChartGraph.legendAlpha = 1;                        //Legend透明度
            serialChartGraph.title = titleList[index];
        }
        serialChartGraph.bullet = bullet;                      //数据线上的点
        serialChartGraph.markerType = markerType;                //数据线上的点的样式
        serialChart.addGraph(serialChartGraph);
    }

    if (titleList.length != 0) {
        var serialChartLegend = new AmCharts.AmLegend();
        serialChartLegend.align = "left";
        serialChartLegend.markerType = "square";
        serialChartLegend.switchable = legendSwitchable;
        serialChartLegend.switchType = legendSwitchType;
        serialChart.addLegend(serialChartLegend);
    }

    var serialChartCatAxis = serialChart.categoryAxis;
    serialChartCatAxis.parseDates = true;
    serialChartCatAxis.minPeriod = minPeriod;
    serialChartCatAxis.axisColor = "#FFFFFF";
    serialChartCatAxis.gridColor = catAxisGridColor;
    serialChartCatAxis.gridAlpha = 0.5;
    serialChartCatAxis.dashLength = 5;
    serialChartCatAxis.autoGridCount = true;
    if (titleList.length == 0) {
        serialChartCatAxis.labelsEnabled = false;
    }

    var serialChartValAxis = new AmCharts.ValueAxis();
    serialChartValAxis.axisColor = "#FFFFFF";
    serialChartValAxis.gridColor = valAxisGridColor;
    serialChartValAxis.gridAlpha = "0.5";
    serialChartValAxis.dashLength = 0;
    if (valueUnit != "") {
        serialChartValAxis.unit = valueUnit;
    }

    if (valAxisMaximum != "") {
        serialChartValAxis.maximum = valAxisMaximum;
    }
    if (valAxisMinimum != "") {
        serialChartValAxis.minimum = valAxisMinimum;
    }
    if (titleList.length == 0) {
        serialChartValAxis.labelsEnabled = false;
    }

    serialChart.addValueAxis(serialChartValAxis);

    if (titleList.length != 0) {
        var serialChartCursor = new AmCharts.ChartCursor();           //显示一个跟随鼠标的光标
        serialChartCursor.cursorAlpha = 0;
        serialChartCursor.cursorPosition = "mouse";
        serialChartCursor.categoryBalloonDateFormat = dateFormat;
        serialChartCursor.cursorAlpha = 1;                             //设置光标线的透明度
        serialChart.addChartCursor(serialChartCursor);

        var serialChartScrollbar = new AmCharts.ChartScrollbar();   //滚动条
        serialChartScrollbar.graph = serialChartGraph0;
        serialChartScrollbar.backgroundAlpha = 0.1;
        serialChartScrollbar.backgroundColor = "#FFFFFF";
        serialChartScrollbar.scrollbarHeight = 10;
        serialChartScrollbar.graphFillColor = "#000000";				//滚动条波形图填充区域 颜色
        serialChartScrollbar.graphFillAlpha = 0;					    //滚动条波形图填充区域 可见度
        serialChartScrollbar.graphLineColor = "#000000";				//滚动条波形图线形区域 颜色
        serialChartScrollbar.graphLineAlpha = 0;						//滚动条波形图线形区域 可见度
        serialChartScrollbar.selectedBackgroundColor = "#DADADA";     //滚动条选中颜色
        serialChartScrollbar.selectedGraphLineAlpha = 0;
        serialChartScrollbar.selectedGraphFillAlpha = 0;
        serialChartScrollbar.gridCount = 0;
        serialChartScrollbar.gridColor = "#FFFFFF";
        serialChartScrollbar.gridAlpha = 0;
        serialChartScrollbar.autoGridCount = false;
        serialChartScrollbar.skipEvent = false;
        serialChartScrollbar.scrollbarCreated = false;
        serialChartScrollbar.resizeEnabled = false;
        serialChart.addChartScrollbar(serialChartScrollbar);
    }
    serialChart.write(divName);
    return serialChart;
}