"use strict";

console.log("hub");

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/notification-hub")
    .build();

connection.on("WalletShareRequest", function (message) {
    console.log("message received");
    console.log(message);
    alert(`${message.ownerName} invites you to collaborate on Wallet ${message.walletName}`);
});

connection.start().then(function () {
    console.log("connection started...");
}).catch(function (err) {
    return console.error(err.toString());
});