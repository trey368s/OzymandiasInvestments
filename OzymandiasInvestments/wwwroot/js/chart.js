document.addEventListener("DOMContentLoaded", function () {
    const bars = modelData.bars;
    const sma = modelData.sma;
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
        wickUpColor: '#32fbe2',
        crosshairMarkerVisible: true,
        crosshairMarkerRadius: 3,
    });

    var volumeSeries = chart.addHistogramSeries({
        color: '#45D8E7',
        crosshairMarkerVisible: true,
        crosshairMarkerRadius: 3,
        priceFormat: {
            type: 'volume',
        },
        priceScaleId: '',
    });
    chart.priceScale('').applyOptions({
        scaleMargins: {
            top: 0.9,
            bottom: 0,
        },
    });

    var lineSeries = chart.addLineSeries({
        color: '#de3bb3',
        lineStyle: 0,
        lineWidth: 1,
        crosshairMarkerVisible: true,
        crosshairMarkerRadius: 3,
        lineType: 2,
    });

    const formatDate = (utcTime) => {
        const date = new Date(utcTime);
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    };

    const barData = bars.map((bar) => ({
        time: formatDate(bar.timeUtc), // Convert UTC time to 'YYYY-MM-DD' format
        open: bar.open, // Convert to numeric type
        high: bar.high, // Convert to numeric type
        low: bar.low, // Convert to numeric type
        close: bar.close, // Convert to numeric type
    }));

    const volumeData = bars.map((bar) => ({
        time: formatDate(bar.timeUtc), 
        value: bar.volume
    }));

    const lineData = sma.map((smaClose) => ({
        time: formatDate(smaClose.timeUtc), 
        value: smaClose.close
    }));

    candleSeries.setData(barData);
    volumeSeries.setData(volumeData);
    lineSeries.setData(lineData);
});