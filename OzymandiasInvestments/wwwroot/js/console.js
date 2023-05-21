var originalConsoleLog = console.log;

console.log = function (message) {
    var paragraph = document.createElement('p');
    paragraph.textContent = JSON.stringify(message); 

    document.getElementById('consoleOutput').appendChild(paragraph);

    originalConsoleLog.apply(console, arguments);
};
