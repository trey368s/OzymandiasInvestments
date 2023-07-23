document.addEventListener("DOMContentLoaded", function () {
    const bars = modelData.bars;
    if (bars && bars.length > 0) {
        document.getElementById("symbol").textContent = bars[0].symbol;
    }

    const chartCard = document.getElementById('chart-card');
    var chart = LightweightCharts.createChart(chartCard, {
        width: chartCard.clientWidth, 
        height: chartCard.clientWidth, 
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
    });

    var candleSeries = chart.addCandlestickSeries({
        upColor: '#3cf281',
        downColor: '#e44c55',
        borderDownColor: '#e44c55',
        borderUpColor: '#3cf281',
        wickDownColor: '#32fbe2',
        wickUpColor: '#32fbe2'
    });

    const formatDate = (utcTime) => {
        const date = new Date(utcTime);
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    };

    const data = bars.map((bar) => ({
        time: formatDate(bar.timeUtc), // Convert UTC time to 'YYYY-MM-DD' format
        open: bar.open, // Convert to numeric type
        high: bar.high, // Convert to numeric type
        low: bar.low, // Convert to numeric type
        close: bar.close, // Convert to numeric type
    }));


    candleSeries.setData(data);

});