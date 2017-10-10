/// <reference path="/scripts/vsdoc/jquery-2.0.3-vsdoc.js" />
/// <reference path="vsdoc/jquery.json-2.4.js" />
/// <reference path="/scripts/vsdoc/jquery.blockUI.js" />
/* File Created: September 11, 2013 */
$(document).ready(function () {
    Login.Init();
});

var Login = (function ($j) {
    var _this = null;
    var settings = {};
    settings.debug = false;
    settings.fieldErrorClass = "form-error";

    _this = {
        Init: function () {
            _int.Init();
            _this.BindEvents();
        },
        BindEvents: function () {
            $("#login-btn").on("click", _int.OnLoginButtonClick);
            $("#login-form input").on("keypress", _int.OnKeyPressed);
            $("#login-form input").on("blur.login", _int.OnFieldBlur);
        }
    };

    var _int = {
        //#region - Init -
        Init: function () {
            $.blockUI.defaults.css.border = '1px solid #aaa';
            $.blockUI.defaults.css.width = '80px';
            _int.InitAuthenticationType();
        },
        InitAuthenticationType: function () {
            var isIntranet = parseInt($("#is-intranet-user").val());
            var intranetLink = $("#intranet-login-link").hide();
            var supportEmailLink = $(".login-form .support-email-link");
            if (isIntranet) {
                if (!_int.IsLogoutFlag()) {
                    _int.TryIntranetLogin();
                    _int.BlockLoginBox();
                }
                else {
                    intranetLink.show();
                    supportEmailLink.hide();
                }
            }
            else {
                //  Forms authentication
            }
        },
        //#endregion

        //#region - Events -
        OnKeyPressed: function (e) {
            if (e.which == 13) {
                _int.OnLoginButtonClick();
            }
        },
        OnLoginButtonClick: function () {
            var credentials = _int.GetUserCredentials();
            _int.ClearErrorMessages();
            if (_int.ValidateFields(credentials)) {
                _int.TryLogin(credentials.username, credentials.password);
            }
        },
        OnFieldBlur: function (e) {
        },
        //#endregion

        //#region - Functions -
        TryIntranetLogin: function () {
            //  TODO: REPLACE remote address with web config
            var transport = new easyXDM.Socket({
                remote: $("#remote-web-auth-address").val(),
                container: "win-auth-frame",
                onMessage: function (message, origin) {
                    if (message.substring(0, 5) === "error") {
                        _int.ShowErrorMessage("Windows Authentication failed. Please contact support.");
                        _int.UnblockLoginBox();
                    }
                    else {
                        _int.Log("Token received: " + message);
                        _int.CompleteIntranetLogin(message);
                    }
                },
                onReady: function () {
                    _int.Log("RPC ready");
                }
            });
        },
        TryLogin: function (username, password) {
            _int.BlockLoginBox();
            $.ajax({
                url: '/homeapi/trylogin',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: $.toJSON({ username: username, password: password }),
                success: _int.OnTryLoginSucceed,
                complete: _int.UnblockLoginBox
            });
        },
        OnTryLoginSucceed: function (response) {
            if (response.State) {
                _int.BlockLoginBox();
                window.location = _int.GetRedirectUrl();
            }
            else {
                _int.ShowErrorMessage(response.Message);
            }
        },
        CompleteIntranetLogin: function (token) {
            $.ajax({
                url: '/Account/TryIntranetLogin',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: $.toJSON({ token: token }),
                success: _int.OnTryIntranetLoginSucceed
            });
        },
        OnTryIntranetLoginSucceed: function (response) {
            if (response.State) {
                window.location = _int.GetRedirectUrl();
            }
            else {

                _int.ShowErrorMessage(response.Message);
                _int.UnblockLoginBox();
            }
        },
        ValidateFields: function (credentials) {
            var usernameField = $("#usernameField").removeClass(settings.fieldErrorClass);
            var passwordField = $("#passwordField").removeClass(settings.fieldErrorClass);
            if (!credentials.username) {
                usernameField.addClass(settings.fieldErrorClass);
                usernameField.siblings(".error-msg").html("Enter your username.");
                return false;
            }

            if (!credentials.password) {
                passwordField.addClass(settings.fieldErrorClass);
                passwordField.siblings(".error-msg").html("Enter your password.");
                return false;
            }

            return true;
        },
        //#endregion

        //#region Helpers
        GetRedirectUrl: function () {
            var url = "/";

            var vars = [], hash;
            var q = document.URL.split('?')[1];
            if (q != undefined) {
                q = q.split('&');
                for (var i = 0; i < q.length; i++) {
                    hash = q[i].split('=');
                    vars.push(hash[1]);
                    vars[hash[0]] = hash[1];
                }
            }

            if ($.trim(vars["ReturnUrl"])) {
                url = $.trim(vars["ReturnUrl"]);
            }

            return decodeURIComponent(url);
        },
        IsLogoutFlag: function () {
            var vars = [], hash;
            var q = document.URL.split('?')[1];
            if (q != undefined) {
                q = q.split('&');
                for (var i = 0; i < q.length; i++) {
                    hash = q[i].split('=');
                    vars.push(hash[1]);
                    vars[hash[0]] = hash[1];
                }
            }

            var logoutFlag = $.trim(vars["logout"]);
            if (logoutFlag) {
                return true;
            }

            return false;
        },
        ShowErrorMessage: function (msg) {
            $("#form-error-msg").html(msg);
        },
        ClearErrorMessages: function () {
            $("#login-form .error-msg").html("");

        },
        GetUserCredentials: function () {
            var credentials = {
                username: $("#usernameField").val(),
                password: $("#passwordField").val()
            };

            return credentials;
        },
        BlockLoginBox: function () {
            var spinner = $("<img>");
            spinner.addClass("ui-block-spinner").attr("src", "/Content/images/pixel.png");

            $("#login-form").block({
                message: spinner,
                overlayCSS: { cursor: 'default' }
            });
        },
        UnblockLoginBox: function () {
            $("#login-form").unblock();
        },
        Log: function (msg) {
            if (settings.debug) { console.log("Login: " + msg); }
        }
        //#endregion
    };

    return _this;
})(jQuery);