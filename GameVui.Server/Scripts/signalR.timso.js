var titleAnimated = false;
function titlebar(val, msg)
{
    if (!titleAnimated) {
        document.title = null;
        return null;
    }
    var speed = 100;
    var pos = val;
    var timer;
    var le = msg.length;
    if (pos < le - 1) {
        pos = pos + 1;
        scroll = msg.substr(pos, le-pos);
        document.title = scroll;
        window.setTimeout("titlebar(" + pos + ",'" + msg + "')", speed);
    } else
    {
        document.title = "";
        pos = 0;
        window.setTimeout("titlebar(" + pos + ",'" + msg + "')", speed);
    }
}

$(function () {
    var x = new Image();
    x.src = '/Content/Images/TicTacToeX.png';
    var o = new Image();
    o.src = '/Content/Images/TicTacToeO.png';

    var game = $.connection.timSoHub;
    var playing = false;
    //Thông báo 1 người chơi khác online
    game.client.notifyOnline = function (user) {
        $("#listUser").append("<div class='col-sm-12' id='" + user.UserId + "'>" + user.DisplayName + "<button class='btn btn-danger pull-right' data-total='100' data-userid='" + user.UserId + "'>Thách đấu 100</button><button class='btn btn-warning pull-right' data-total='50' data-userid='" + user.UserId + "'>Thách đấu 50</button></li>");
    };

    game.client.receiveListUser = function (users) {
        $("#listUser").empty();
        for (i = 0; i < users.length; i++) {
            $("#listUser").append("<div class='col-sm-12' id='" + users[i].UserId + "'>" + users[i].DisplayName + "<button class='btn btn-danger pull-right' data-total='100' data-userid='" + users[i].UserId + "'>Thách đấu 100</button><button class='btn btn-warning pull-right' data-total='50' data-userid='" + users[i].UserId + "'>Thách đấu 50</button></li>");
        }
    };
    //Thông báo 1 người chơi khác offline
    game.client.notifyOffline = function (user) {
        $("#listUser").find("#" + user.UserId).remove();
    };
    //nhận lời mới từ người khác
    game.client.receiveInvitation = function (user, totalNumber) {
        var $invitationDiv = $('<div class="alert alert-block alert-success invitation" data-userid="' + user.UserId + '" data-connection-id="' + user.ConnectionId + '" data-total="' + totalNumber + '"></div>');
        $invitationDiv.append('<p>' + user.DisplayName + " đã gửi yêu cầu thách đấu. " + totalNumber + ' ô</p>');
        $invitationDiv.append('<p><button class="btn btn-sm btn-success acceptInviteBtn">Chơi</button><button class="btn btn-sm cancelInviteBtn">Hủy</button></p>');
        $invitationDiv.appendTo('#listInvitation');
        titleAnimated = true;
        titlebar(0, "Bạn có lời mời thách đấu");
    }
    //gửi lời mời thành công
    game.client.inviteSuccessful = function (userId) {
        $("#" + userId).find("button").addClass("disabled");
    };

    //lời mời bị hủy
    game.client.invitationCanceled = function (userId) {
        $('#' + userId).find("button").removeClass("disabled");
    };

    game.client.invitationTimeout = function (userId, host) {
        if (host == true)
            $('#' + userId).find("button").removeClass("disabled");
        else {
            $('div.invitation[data-userid="' + userId + '"]').hide();
            titleAnimated = false;
        }
    };

    //lời mời được chấp nhận
    game.client.opponentAcceptInvitation = function (connectionId) {
        $('#' + connectionId).find("button").removeClass("disabled");
    };

    //yêu cầu đăng nhập
    game.client.requireRegister = function () {
        $('#notifiMessage').html('Bạn chưa đăng nhập');
        $('#notifiModal').modal('show');
    }
    game.client.opponentOffline = function () {
        $('#notifiMessage').html('Đối thủ đã thoát');
        $('#notifiModal').modal('show');
    }
    game.client.opponentPlaying = function () {
        $('#notifiMessage').html('Đối thủ đang tham gia trận khác');
        $('#notifiModal').modal('show');
    }
    game.client.clientPlaying = function () {
        $('#notifiMessage').html('Bạn đang tham gia trận khác');
        $('#notifiModal').modal('show');
    }
    game.client.playingAnother = function()
    {
        $('#notifiMessage').html('Bạn đang chơi ở một nơi khác');
        $('#notifiModal').modal('show');

        $("#lobby").hide();
        $("#serverInformation").hide();
        $("#game").empty();
        $("#mainGame").addClass("hidden");

    }
    game.client.foundOpponent = function (field, currentNumber, message, simbolo) {
        gotoMainGame();
        titlebar(0, "Đang chơi");
        $("#chatWindow").data("opponent", message);
        $("#gameInformation").html("Đối thủ: " + message);
        $("#simbolo").html("Màu của bạn: <img src='" + simbolo + "' height='20' width='20' style='vertical-align:bottom'/>");
        $('#currentNumber').html(currentNumber);

        for (var i = 0; i < field.length; i++) {
            var $box = $("<span id=" + i + " class='box' />");
            $("#game").append($box);
            $box.attr('data-content', field[i]);
        }

        document.getElementById('sound').play();
        setTimeout(function ()
        {
            document.getElementById('sound').play();
        }, 1000);
    };
    game.client.noOpponents = function (message) {

        $("#waitingForOpponent").show();
    };
    game.client.addMarkerPlacement = function (message) {
        if (message.Simbolo == "O") {
            $("#" + message.MarkerPosition).addClass("mark2");
            $("#" + message.MarkerPosition).addClass("marked");
        }
        else {
            $("#" + message.MarkerPosition).addClass("mark1");
            $("#" + message.MarkerPosition).addClass("marked");
        }
        $("#progressMe .progress-bar").width(message.MyPoint + "%");
        $("#progressOpponent .progress-bar").width(message.OpponentPoint + "%");

        if (message.CurrentNumber >= 0) {
            $('#currentNumber').html(message.CurrentNumber);
            $('#currentNumber').fadeIn("slow");
        }
        else
            $('#currentNumber').html('');
        document.getElementById('sound').play();
    };
    game.client.opponentDisconnected = function (message) {
        //$("#gameInformation").html("<strong>Kết thúc trận đấu! Đối thủ " + message + " đã bỏ cuộc!</strong>");
        $('#gameOverMessage').html('Đối thủ thoát giữa trận. Bạn thắng');
        $('#gameOverModal').modal('show');
        titleAnimated = false;
        //gotoLobby();

    };
    //game.client.registerComplete = function (message) {
    //    gotoLobby();
    //};
    game.client.gameOver = function (checkWin) {
        if (checkWin === 1)
            $('#gameOverMessage').html('Bạn thắng');
        else if (checkWin === 2)
            $('#gameOverMessage').html('Bạn thua');
        else
            $('#gameOverMessage').html('Huề');

        $('#gameOverModal').modal('show');
        titleAnimated = false;
    };

    game.client.refreshAmountOfPlayers = function (message) {
        $("#amountOfGames").html(message.amountOfGames);
        $("#amountOfClients").html(message.amountOfClients);
        $("#totalAmountOfGames").html(message.totalGamesPlayed);
    };
    
    $("#game").on("click", ".box", function (event) {
        if ($(this).hasClass("marked")) return;

        game.server.play(event.target.id);
    });
 
    $(".findGame").click(function () {
        findGame();
    });

    function findGame() {
        game.server.findOpponent();
    }
    $("#listUser").on("click", "button", function () {
        var userid = $(this).data("userid");
        var totalNumber = $(this).data("total");
        game.server.invite(userid, totalNumber);
    });
    $('#listInvitation').on('click', '.acceptInviteBtn', function () {
        var invitationDiv = $(this).parents("div.invitation").get(0);
        var userid = $(invitationDiv).data("userid");
        var connectionId = $(invitationDiv).data("connection-id");
        var totalNumber = $(invitationDiv).data("total");
        $(invitationDiv).remove();
        game.server.acceptInvitation(userid, connectionId, totalNumber);
        titleAnimated = false;
    });
    $('#listInvitation').on('click', '.cancelInviteBtn', function () {
        var invitationDiv = $(this).parents("div.invitation").get(0);
        var userid = $(invitationDiv).data("userid");
        game.server.cancelInvitation(userid);
        $(invitationDiv).remove();
        titleAnimated = false;
    });
    $('#gameOverModal').on('hidden.bs.modal', function (e) {
        gotoLobby();
    });

    function gotoMainGame() {
        playing = true;
        $("#lobby").hide();
        $("#serverInformation").hide();
        $("#game").empty();
        $("#mainGame").removeClass("hidden");
        $("#waitingForOpponent").hide();
    }

    function gotoLobby() {
        playing = false;
        $("#lobby").show();
        $("#serverInformation").show();
        $("#game").empty();
        $("#chat-messages").empty();
        $("#mainGame").addClass("hidden");
        //
        game.server.getListUser();
    }
    var hideChat = true;
    $("#hideChatBtn").on('click', function () {
        if (hideChat) {
            $("#chatWindow .panel-body").show();
            $("#chatWindow .panel-footer").show();
        }
        else {
            $("#chatWindow .panel-body").hide();
            $("#chatWindow .panel-footer").hide();
        }
        hideChat = !hideChat;
    });

            

    $.connection.hub.start().done(function () {
        gotoLobby();
    });

    window.onbeforeunload = function () {
        if (playing) {
            return "Bạn đang trang trận đấu, nếu rời đi bạn sẽ bị xử thua. Xác nhận rời đi?";
        }
        else return null;
    };
});