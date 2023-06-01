var DmUiPlugins = function()
{

    var private = {

        InlineManualUserInfo: '',
        InlineManualKey: '',

        setupInlineManual: function()
        {
            //var inlineManualUserInfo = { "userId": "12345", "role": "1", "contractId": "456", "package": "2", "inlineManuakey": "https://inlinemanual.com/embed/player.2a92c873914cc192aa74afdb27376e9c.js", "enableInlineManual": "true" };
            var inlineManualUserInfo = private.InlineManualUserInfo;
            if (inlineManualUserInfo != undefined && inlineManualUserInfo != '') {
                if (inlineManualUserInfo.enableInlineManual == "true") {
                    try {
                        var script1 = "window.inlineManualTracking = {uid: " + inlineManualUserInfo.userId + ",created: " + (new Date().getTime() / 1000 | 0) + ",roles: " + "[" + inlineManualUserInfo.role + "]" + ",group: " + inlineManualUserInfo.contractId + ",plan: " + inlineManualUserInfo.package + "};";
                        var script2 = "window.inlineManualOptions = { language: 'en' };";
                        var script3 = "!function () { var e = document.createElement('script'), t = document.getElementsByTagName('script')[0]; e.async = 1, e.src ='" + inlineManualUserInfo.inlineManualSrc + "', e.charset = 'UTF-8', t.parentNode.insertBefore(e, t) }();";
                        var script = document.createElement("script");
                        script.setAttribute("type", "text/javascript");
                        script.appendChild(document.createTextNode(script1 + script2 + script3));
                        document.head.appendChild(script);
                    } catch (e) { }
                }
            }
        },

        setupFreshChat: function()
        {

            var inlineManualUserInfo = private.InlineManualUserInfo;

            if (inlineManualUserInfo != undefined && inlineManualUserInfo != '') {
                if (inlineManualUserInfo.enableInlineFreshChat == "true") {
                    initiateCall();
                }
            }
        }
    };

    function initFreshChat()
    {
        var freshChatJSON = JSON.parse(private.InlineManualUserInfo.freshChatJSON);
        window.fcWidget.init(freshChatJSON);
    }

    function initializeFreshChat(i, t) { var e; i.getElementById(t) ? initFreshChat() : ((e = i.createElement("script")).id = t, e.async = !0, e.src = "https://wchat.in.freshchat.com/js/widget.js", e.onload = initFreshChat, i.head.appendChild(e)) };

    function initiateCall() { initializeFreshChat(document, "freshchat-js-sdk") };


    return {
        InitInlineManual: function(imUserInfo)
        {
            private.InlineManualUserInfo = imUserInfo;
            private.setupInlineManual();
            private.setupFreshChat();
        },
        Init: function()
        {
            return false;
        }
    };
}();

