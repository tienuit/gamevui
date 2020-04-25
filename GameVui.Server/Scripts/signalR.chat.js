function Chat(myId)
{
	//chatHub
	var chat = $.connection.chatHub;
	chat.client.notifyUserOnline = function(client)
	{
	    $(".chat-user-item[data-userid=" + client.UserId + "]").removeClass('offline').addClass('online');
	}
	chat.client.notifyUserOffline = function(client)
	{
	    $(".chat-user-item[data-userid=" + client.UserId + "]").removeClass('online').addClass('offline');
	}

	chat.client.receiveMessage = function (otherId, otherDisplayName, message)
	{
	    var existWindow = openChatWindow(otherId, otherDisplayName);
	    if (existWindow == true) {
	        appendOtherMessage(otherId, otherDisplayName, message);

	        var $chatWindow = $('.chat-window[data-userid=' + otherId + ']');
	        $chatWindow.find('.panel-body').animate({ scrollTop: $chatWindow.find('.panel-body')[0].scrollHeight }, 500);
	       
	    }
	}
	chat.client.receiveGroupMessage = function(userId, userDisplayName, message)
	{
	    appendGroupMessage(userId, userDisplayName, message);
        //scroll 
	    var $chatWindow = $('#chat-group');
	    $chatWindow.find('.modal-body .scroller').animate({ scrollTop: $chatWindow.find('.modal-body .scroller')[0].scrollHeight }, 500);
	}
	chat.client.sendMessageComplete = function (otherId, myDisplayName, message)
	{
	    appendMyMessage(otherId, myDisplayName, message);

	    var $chatWindow = $('.chat-window[data-userid=' + otherId + ']');
	    $chatWindow.find('.panel-body').animate({ scrollTop: $chatWindow.find('.panel-body')[0].scrollHeight }, 500);
	}
	chat.client.receiveOnlineUsers = function(clients)
	{
	    console.log(clients);
	    for (var i = 0; i < clients.length; i++)
	    {
	        $('.chat-user-item[data-userid='+clients[i].UserId+'] i').removeClass('offline').addClass('online');
	    }
	}

	$.connection.hub.start().done(function ()
	{
	    chat.server.getOnlineUsers();
	    $('tr.chat-user-item').on('click', function () {
           
	        var userId = $(this).data("userid");
	        var displayName = $(this).data("displayname");
	        openChatWindow(userId, displayName);
	    });
	    $("#group-chat-input").keypress(function (e) {
	        if (e.which == 13) {
	            var message = $("#group-chat-input").val();
	            $("#group-chat-input").val('');
	            sendGroupMessage(message);
	        }
	    });
	});

	var appendOtherMessage = function (userId, displayName, msg)
	{
	    var messElement = '<li class="left clearfix">'
                            + '<span class="chat-img pull-left">'
                                + '<img src="http://placehold.it/50/55C1E7/fff&amp;text=U" alt="User Avatar" class="img-circle">'
                            + '</span>'
                            + '<div class="chat-body clearfix">'
                                + '<div class="header">'
                                    + '<strong class="primary-font">'+ displayName+'</strong> <small class="pull-right text-muted">'
                                        + '<span class="glyphicon glyphicon-time"></span>time</small>'
                                + '</div>'
                                + '<p>'
                                    + msg
                                + '</p>'
                            + '</div>'
                        + '</li>';
	    //append message
	    $('.chat-window[data-userid=' + userId + ']').find('.chat-messages').append(messElement);
	}

	var appendMyMessage = function (userId, displayName, msg, isPrivate)
	{
	    var messElement =   '<li class="right clearfix">'
                                +'<span class="chat-img pull-right">'
                                    +'<img src="http://placehold.it/50/FA6F57/fff&amp;text=ME" alt="User Avatar" class="img-circle">'
                                +'</span>'
                                +'<div class="chat-body clearfix">'
                                    +'<div class="header">'
                                        +'<small class=" text-muted"><span class="glyphicon glyphicon-time"></span>time</small>'
                                        +'<strong class="pull-right primary-font">'+displayName+'</strong>'
                                    +'</div>'
                                    + '<p>'
                                        + msg
                                    +'</p>'
                                +'</div>'
                            + '</li>'
	    //append message
        $('.chat-window[data-userid=' + userId + ']').find('.chat-messages').append(messElement);
	   
	}
	var appendGroupMessage = function (userId, displayName, msg) {
	    var messElement =   '<div class="itemdiv dialogdiv">'
                                +'<div class="user">'
                                    + '<img alt="' + displayName + '" src="/Content/Images/user.png">'
                                +'</div>'

                                +'<div class="body">'
                                    +'<div class="time">'
                                        +'<i class="ace-icon fa fa-clock-o"></i>'
                                        +'<span class="green">4 sec</span>'
                                    +'</div>'

                                    +'<div class="name">'
                                        + '<a href="#">' + displayName + '</a>'
                                    +'</div>'
                                    + '<div class="text">' + msg + '</div>'

                                +'</div>'
                            +'</div>'
	    //append message
	    $('#chat-group').find('.chat-messages').append(messElement);
	}
	//Functions
	var sendMessage = function(userId, msg)
	{
		chat.server.sendMessage(userId, msg);
	}
	var sendGroupMessage = function (msg) {
	    chat.server.sendGroupMessage(msg);
	}

    //UI

	var $chatWindow = $('#chat-group');
	$chatWindow.find('.modal-body .scroller').scrollTop($chatWindow.find('.modal-body .scroller')[0].scrollHeight);

	function openChatWindow(userId, displayName) {
	    if ($('.chat-window[data-userid='+userId+']').length == 0)
	    {
	        var newChatWindow = template.supplant({ UserId: userId, DisplayName: displayName });
	        $("#chat-windows").append(newChatWindow);
	        //load messages
	        //chat.server.get10LastMessages(userId);
	        var $chatWindow = $('.chat-window[data-userid=' + userId + ']');
	        $chatWindow.find('.chat-messages').load('/Message/Get10Last?userId=' + userId, function ()
	        {
	            $chatWindow.find('.panel-body').scrollTop($chatWindow.find('.panel-body')[0].scrollHeight);
	        });

	        $(newChatWindow).chatwindow({
	            onChat: sendMessage
	        });
	        return false;
	    }
	    return true;
	}
	if (!String.prototype.supplant) {
	    String.prototype.supplant = function (o) {
	        return this.replace(/{([^{}]*)}/g,
                function (a, b) {
                    var r = o[b];
                    return typeof r === 'string' || typeof r === 'number' ? r : a;
                }
            );
	    };
	}
	var template = '<div class="hidden-xs chat-container chat-window" data-hide="false" data-userid="{UserId}">'
                        + '<div class="panel panel-primary panel-chat">'
                            + '<div class="panel-heading">'
                                + '<span class="glyphicon glyphicon-comment"></span> {DisplayName}'
                                + '<div class="btn-group pull-right">'
                                    + '<button type="button" class="btn btn-default btn-xs btn-hide">'
                                        + '<span class="fa fa-angle-down"></span>'
                                    + '</button>'
                                    + '<button type="button" class="btn btn-default btn-xs btn-close">'
                                        + '<span class="fa fa-close"></span>'
                                    + '</button>'
                                + '</div>'
                            + '</div>'
                            + '<div class="panel-body">'
                                + '<ul class="chat chat-messages"></ul>'
                            + '</div>'
                            + '<div class="panel-footer">'
                                + '<div class="input-group">'
                                    + '<input type="text" class="form-control input-sm input-chat"/>'
                                    + '<span class="input-group-btn">'
                                        + '<button class="btn btn-warning btn-sm btn-chat">Gửi</button>'
                                    + '</span>'
                                + '</div>'
                            + '</div>'
                        + '</div>'
                    + '</div>';
}