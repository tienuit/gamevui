var titleAnimated = false;
function titlebar(val, msg) {
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
        scroll = msg.substr(pos, le - pos);
        document.title = scroll;
        window.setTimeout("titlebar(" + pos + ",'" + msg + "')", speed);
    } else {
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

    var game = $.connection.caroHub;
    var playing = false;
    //Thông báo 1 người chơi khác online
    game.client.notifyOnline = function (user) {
        $("#listUser").append("<div class='col-sm-12' id='" + user.UserId + "'>" + user.DisplayName + "<button class='btn btn-danger pull-right' data-userid='" + user.UserId + "'>Thách đấu</button></li>");
    }

    game.client.receiveListUser = function (users) {
        $("#listUser").empty();
        for (i = 0; i < users.length; i++) {
            $("#listUser").append("<div class='col-sm-12' id='" + users[i].UserId + "'>" + users[i].DisplayName + "<button class='btn btn-danger pull-right' data-userid='" + users[i].UserId + "'>Thách đấu</button></li>");
        }
    }
    game.client.receiveListMatch = function (matchs) {
        $("#listMatch").empty();
        for (i = 0; i < matchs.length; i++) {
            $("#listMatch").append("<div class='col-sm-12'>" + matchs[i].Player1Name + " - " + matchs[i].Player2Name+ "<button class='btn btn-danger pull-right'>Xem</button></li>");
        }
    }

    //Thông báo 1 người chơi khác offline
    game.client.notifyOffline = function (user) {
        $("#listUser").find("#" + user.UserId).remove();
    }
    //nhận lời mới từ người khác
    game.client.receiveInvitation = function (user) {
        var $invitationDiv = $('<div class="alert alert-block alert-success invitation" data-userid="' + user.UserId + '" data-connection-id="' + user.ConnectionId + '"></div>');
        $invitationDiv.append('<p>' + user.DisplayName + ' đã gửi yêu cầu thách đấu.</p>');
        $invitationDiv.append('<p><button class="btn btn-sm btn-success acceptInviteBtn">Chơi</button><button class="btn btn-sm cancelInviteBtn">Hủy</button></p>');
        $invitationDiv.appendTo('#listInvitation');
        titleAnimated = true;
        titlebar(0, "Bạn có lời mời thách đấu");
    }
    //gửi lời mời thành công
    game.client.inviteSuccessful = function (userId) {
        $("#" + userId).find("button").addClass("disabled");
    }
    //lời mời bị hủy
    game.client.invitationCanceled = function (userId) {
        $('#' + userId).find("button").removeClass("disabled");
    }
    game.client.invitationTimeout = function (userId, host) {
        if (host == true)
            $('#' + userId).find("button").removeClass("disabled");
        else {
            $('div.invitation[data-userid="' + userId + '"]').hide();
            titleAnimated = false;
        }
    }
    //lời mời được chấp nhận
    game.client.opponentAcceptInvitation = function (connectionId) {
        $('#' + connectionId).find("button").removeClass("disabled");
    }
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
    game.client.playingAnother = function () {
        $('#notifiMessage').html('Bạn đang chơi ở một nơi khác');
        $('#notifiModal').modal('show');

        $("#lobby").hide();
        $("#serverInformation").hide();
        $("#game").empty();
        $("#mainGame").addClass("hidden");

    }
    game.client.foundOpponent = function ( message) {
        gotoMainGame();
        titlebar(0, "Đang chơi");
        $("#chatWindow").data("opponent", message);
        $("#gameInformation").html("Đối thủ: " + message);
        
        for (var i = 0; i < 20; i++) {
            var $row = $("<div class='row'></div>")
            for (var j = 0; j < 20; j++) {
                var $box = $("<span class='box_caro' id='box_"+j+"_"+i+"' />");
                $row.append($box);
                $box.attr('data-col', j);
                $box.attr('data-row', i);
            };
            $("#game").append($row);
            
        }

        document.getElementById('sound').play();
        setTimeout(function () {
            document.getElementById('sound').play();
        }, 1000);
    };
    game.client.noOpponents = function (message) {

        $("#waitingForOpponent").show();
    };
    game.client.addMarkerPlacement = function (message) {
        var col = message.MarkerCol;
        var row = message.MarkerRow;
        var player = message.Player;
        if (player == 2) {
            $("#box_" + col+"_"+row).addClass("mark_caro2");
        }
        else {
            $("#box_" + col+"_"+row).addClass("mark_caro1");
        }

        $("#box_" + col + "_" + row).addClass("marked");
        $(".box_caro").removeClass("blink");
        $("#box_" + col + "_" + row).addClass("blink");

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
        if (checkWin == true)
            $('#gameOverMessage').html('Bạn thắng');
        else
            $('#gameOverMessage').html('Quá gà');
        
        $('#gameOverModal').modal('show');
        titleAnimated = false;
    };

    game.client.refreshAmountOfPlayers = function (message) {
        $("#amountOfGames").html(message.amountOfGames);
        $("#amountOfClients").html(message.amountOfClients);
        $("#totalAmountOfGames").html(message.totalGamesPlayed);
    };

    $("#game").on("click", ".box_caro", function (event) {
        if ($(this).hasClass("marked")) return;

        game.server.play($(event.target).data('col'), $(event.target).data('row'));
    });

    $(".findGame").click(function () {
        findGame();
    });

    function findGame() {
        game.server.findOpponent();
    }
    $("#listUser").on("click", "button", function () {
        var userid = $(this).data("userid");
        game.server.invite(userid);
    });
    $('#listInvitation').on('click', '.acceptInviteBtn', function () {
        var invitationDiv = $(this).parents("div.invitation").get(0);
        var userid = $(invitationDiv).data("userid");
        var connectionId = $(invitationDiv).data("connection-id");
        $(invitationDiv).remove();
        game.server.acceptInvitation(userid, connectionId);
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
    })

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
        game.server.getListMatch();
    }

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