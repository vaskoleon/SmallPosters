function fillTableAdmin() {
    let tokenLocation = "authToken-io4f5oi4FJ$OIFJoi";
    let usernameLocation = "user-34otjg24i5gj3fqo3joijwOiof45";
    let username = localStorage.getItem(usernameLocation);
    let authToken = localStorage.getItem(tokenLocation);
    let container = $("#tbodyAdmin");
    let idList = [];
    let columnCount = 7;
    $.ajax({
        type: "GET",
        url: "/api/ad/pending",
        data: {
            Username: localStorage.getItem(usernameLocation),
            AuthToken: localStorage.getItem(tokenLocation),
        },
        contentType: "application/json; charset=utf-8",
        success: function (array) {
            $("#adminViewLoading").text("");
            fillTable(array);
            var intervalID = setInterval(periodicFunction, 5000);
        },
        error: function (context) { }
    });
    function periodicFunction()
    {
        $.ajax({
            type: "GET",
            url: "/api/ad/pending",
            data: {
                Username: localStorage.getItem(usernameLocation),
                AuthToken: localStorage.getItem(tokenLocation),
            },
            contentType: "application/json; charset=utf-8",
            success: function (array)
            {
                array = array.filter(a => !idList.includes(a.adId));
                fillTable(array);
            },
            error: function () { }
        });
    }
    function fillTable(array) {
        for (let element of array) {
            let row = $("<tr>").attr("id", element.adId);
            idList.push(element.adId);
            for (let i = 0; i < columnCount; i++)
                row.append("<td>");
            row.children().attr("scope", "col");
            $(row.children()[0]).text(element.adTitle);
            $(row.children()[1]).text(element.creatorUsername);
            $(row.children()[2]).text(element.categoryName);
            $(row.children()[3]).text(element.adContent);
            $(row.children()[4]).text(element.imageURL);
            $(row.children()[5]).append($("<button>").text("Approve").
                click(approveFunc));
            $(row.children()[6]).append($("<button>").text("Reject").
                click(rejectFunc));
            container.append(row);
        }
    }
    function approveFunc() {
        let row = $(this).closest("tr");
        let id = row.attr("id");
        let first = localStorage.getItem(usernameLocation);
        let second = localStorage.getItem(tokenLocation);
        let third = id;
        $.ajax({
            type: "PUT",
            url: "/api/ad/judge",
            data: JSON.stringify({
                AdminUsername: localStorage.getItem(usernameLocation),
                AuthToken: localStorage.getItem(tokenLocation),
                AdId: id,
                AdDecision:"approve"
            }),
            contentType: "application/json; charset=utf-8",
            success: function (context) {
                row.remove();
            },
            error: function (context) { debugger; }
        });
    }
    function rejectFunc() {
        let row = $(this).closest("tr");
        let id = row.attr("id");
        $.ajax({
            type: "PUT",
            url: "/api/ad/judge",
            data: JSON.stringify({
                AdminUsername: localStorage.getItem(usernameLocation),
                AuthToken: localStorage.getItem(tokenLocation),
                AdId: id,
                AdDecision: "reject"
            }),
            contentType: "application/json; charset=utf-8",
            success: function (context) {
                row.remove();
            },
            error: function (context) { debugger; }
        });
    }
}
