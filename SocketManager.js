//https://addyosmani.com/resources/essentialjsdesignpatterns/book/#singletonpatternjavascript
var SocketManager = (function () {
    "use strict";

    // Instance stores a reference to the Singleton
    var socket_manager_instance;

    function socket_manager_init() {
        // Private variables
        var _socket = undefined;
        var _socket_open = false;
        var _reconnect_socket = true;
        var _reconnect_interval = 5; //Seconds
        var _reconnect_timer = undefined;


        // Private methods
        function _closeSocket() {
            if (_socket) { //Socket is initialized, so close and set to undefined
                _socket.close();
                _socket_open = false;
                _socket = undefined;
            }
        }

        function _openSocket(url, onSocketOpenFunc, onMessageFunc, onSocketCloseFunc) {
            _closeSocket();

            if (!_socket) {
                _socket = new SockJS(url);
            }

            _socket.onopen = function () {
                window.clearInterval(_reconnect_timer);
                _socket_open = true;
                _reconnect_socket = true;
                _reconnect_timer = undefined;

                if (onSocketOpenFunc) {
                    try {
                        onSocketOpenFunc();
                    } catch (errorMessage) {
                        console.error("SocketManager.openSocket > onSocketOpenFunc failed with: '" + errorMessage + "'.");
                    }
                }
            };
            _socket.onmessage = function (e) {
                if (onMessageFunc) {
                    try {
                        onMessageFunc(e.data);
                    } catch (errorMessage) {
                        console.error("SocketManager.openSocket > onMessageFunc failed with: '" + errorMessage + "'.");
                    }
                }
            };
            _socket.onclose = function () {
                if (_reconnect_socket) {
                    window.clearInterval(_reconnect_timer);

                    if (onSocketCloseFunc) {
                        try {
                            onSocketCloseFunc(_reconnect_timer ? "reconnect_failed" : "connection_failed");
                        } catch (errorMessage) {
                            console.error("SocketManager.openSocket > onSocketCloseFunc failed with: '" + errorMessage + "'.");
                        }
                    }

                    _reconnect_timer = window.setInterval(_openSocket, _reconnect_interval * 1000, url, onSocketOpenFunc, onMessageFunc, onSocketCloseFunc);
                } else {
                    if (onSocketCloseFunc) {
                        try {
                            onSocketCloseFunc("connection_closed");
                        } catch (errorMessage) {
                            console.error("SocketManager.openSocket > onSocketCloseFunc failed with: '" + errorMessage + "'.");
                        }
                    }
                }
            };
        }

        return {
            // Public variables

            // Public methods 
            openSocket: function (url, onSocketOpenFunc, onMessageFunc, onSocketCloseFunc) {
                _openSocket(url, onSocketOpenFunc, onMessageFunc, onSocketCloseFunc);
            },

            closeSocket: function () {
                _reconnect_socket = false;
                _closeSocket();
            },

            sendMessage: function (message) {
                if (!_socket_open)
                    throw "Socket not open, please open the socket using 'openSocket(url)'";

                _socket.send(message);
            }

        };

    };

    return {

        // Get the Singleton instance if one exists
        // or create one if it doesn't  
        getInstance: function () {
            if (!socket_manager_instance)
                socket_manager_instance = socket_manager_init();

            return socket_manager_instance;
        }

    }
}());