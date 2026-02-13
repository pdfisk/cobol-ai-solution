// This script handles communication between the WebView2 (JavaScript) and the Host (C#)

(function () {
    // Check if the WebView2 control is available
    if (window.chrome && window.chrome.webview) {
        
        // 1. Handle messages received FROM the C# Host
        // The Host calls CoreWebView2.PostWebMessageAsString() or PostWebMessageAsJson()
        window.chrome.webview.addEventListener('message', event => {
            // 'event.data' contains the message sent from C#
            console.log('Received message from Host:', event.data);
            
            // TODO: Add your logic here to process the message
        });

        // 2. Function to send messages TO the C# Host
        // The Host receives this via the CoreWebView2.WebMessageReceived event
        window.sendMessageToHost = function (message) {
            window.chrome.webview.postMessage(message);
        };
        
    } else {
        console.warn('WebView2 interface not found. This code is likely not running inside a WebView2 control.');
    }
})();