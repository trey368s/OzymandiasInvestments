document.addEventListener('DOMContentLoaded', function () {
    const requestEquityInfo = modelData;  // Assuming this is an asynchronous operation
    console.log(requestEquityInfo);

    var data = requestEquityInfo.map(function (equityModel) {
        return { time: equityModel.tradingDay.toString(), value: parseFloat(equityModel.equity) };
    });

    console.log(data);

    const chart = document.getElementById('doughnutChart');
    const chart2 = document.getElementById('chart');
    // Configuration for the area chart
    var areaChartOptions = {
        width: chart2.clientWidth,
        height: chart.clientHeight,
        layout: {
            background: {
                type: 'solid',
                color: '#1A0933',
            },
            textColor: '#32fbe2',
        },
        grid: {
            vertLines: {
                color: '#32fbe2',
            },
            horzLines: {
                color: '#32fbe2',
            },
        },
        crosshair: {
            mode: LightweightCharts.CrosshairMode.Normal,
        },
        rightPriceScale: {
            borderColor: '#32fbe2',
        },
        timeScale: {
            borderColor: '#32fbe2',
        },
    };

    // Create the area chart
    var areaChart = LightweightCharts.createChart(document.getElementById('areaChartContainer'), areaChartOptions);
    var areaSeries = areaChart.addAreaSeries();

    // Add data to the chart
    areaSeries.setData(data);
    });