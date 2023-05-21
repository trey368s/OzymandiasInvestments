document.querySelector('form').addEventListener('submit', function (event) {
    event.preventDefault();

    var searchInput = document.getElementById('searchInput');
    var tickerSymbol = document.getElementById('symbol');

    tickerSymbol.textContent = searchInput.value;

    var ticker = tickerSymbol.innerHTML
    const url = "wss://stream.data.alpaca.markets/v2/iex"
    const auth = { "action": "auth", "key": "PK4PC5CXDNCF4UTBITT4", "secret": "4xIewFitft6JCzwtdH9hVSAJZI6cHNBiA78f9IRz" }
    const subscribe = { "action": "subscribe", "trades": [ticker], "quotes": [ticker], "bars": [ticker] }

    const socket = new WebSocket(url);
    socket.onmessage = function (event) {
        const data = JSON.parse(event.data)
        const message = data[0]['msg']

        if (message === 'connected') {
            socket.send(JSON.stringify(auth))
        }

        if (message === 'authenticated') {
            socket.send(JSON.stringify(subscribe))
        }

        for (const key in data) {
            console.log(data[key])
            if (data[key]["T"] === 't') {
                console.log(data[key])
            }
            if (data[key]["T"] === 'q') {
                console.log(data[key])
            }
            if (data[key]["T"] === 'b') {
                console.log(data[key])
            }
        }
    }
});