function fillTable() {
    let tokenLocation = "authToken-io4f5oi4FJ$OIFJoi";
    let usernameLocation = "user-34otjg24i5gj3fqo3joijwOiof45";
    let username=localStorage.getItem(usernameLocation);
    let authToken = localStorage.getItem(tokenLocation);
    let columnCount = 7;
    $.ajax({
        type: "GET",
        url: "/api/ad/mine",
        data: {
            Username: localStorage.getItem(usernameLocation),
            AuthToken: localStorage.getItem(tokenLocation),
        },
        contentType: "application/json; charset=utf-8",
        success: function (array) {
            let container = $("#tbodyMine");
            $("#loadingMyAds").text("");
            for (let element of array) {
                let row = $("<tr>").attr("id", element.adId);
                for (let i = 0; i < columnCount; i++)
                    row.append("<td>");
                row.children().attr("scope", "col");
                $(row.children()[0]).text(element.adTitle);
                $(row.children()[1]).text(element.categoryName);
                $(row.children()[2]).text(element.adContent);
                if (element.adminApprovalState == 0)
                {
                    $(row.children()[3]).text("Pending Approval");
                } else if (element.adminApprovalState == 2)
                {
                    $(row.children()[3]).text("REJECTED");
                }
                else if (!element.hasExpired)
                {
                    $(row.children()[3]).text("Active");
                } else
                {
                    $(row.children()[3]).append($("<button>").text("Expired - Publish Again").
                        click(republish));
                }
                $(row.children()[4]).append($("<button>").text("Edit").
                    click(toggleEdit));
                $(row.children()[5]).append($("<button>").text("Delete").
                    click(deleteFunc));

                container.append(row);
            }
        },
        error: function (context) { }
    });
    function toggleEdit()
    {
        let row = $(this).closest("tr");
        let id = row.attr("id");
        row.children().not($("button")).not(":contains('Active')").attr("contentEditable", "true");
        row.children().not($("button")).not(":contains('Active')").css("color", "red");
        $(this).text("Submit Changes");
        $(this).parent().attr("contentEditable", "false");
        $("button:contains('Edit')").not(this).hide();
        $(this).click(edit);
    }
    function edit()
    {
        let row = $(this).closest("tr");
        let button = $(this);
        let id = row.attr("id");
        let adTitle = $(row.children()[0]).text();
        let adCategory = $(row.children()[1]).text();
        let adContent = $(row.children()[2]).text();
        $.ajax({
            type: "PUT",
            url: "/api/ad/edit",
            data: JSON.stringify({
                Username: localStorage.getItem(usernameLocation),
                AuthToken: localStorage.getItem(tokenLocation),
                AdId: id,
                AdTitle: adTitle,
                CategoryName: adCategory,
                AdContent:adContent
            }),
            contentType: "application/json; charset=utf-8",
            success: function (context) {
                row.children().attr("contentEditable", "false");
                row.children().not($("button")).not(":contains('Active')").css("color", "white");
                $("button:contains('Edit')").not(button).show();
                button.text("Edit");
                button.click(toggleEdit);
            },
            error: function (context) { debugger; }
        });
    }
    function deleteFunc()
    {
        let row = $(this).closest("tr");
        let id = row.attr("id");
        $.ajax({
            type: "DELETE",
            url: "/api/ad/delete",
            data: JSON.stringify({
                Username: localStorage.getItem(usernameLocation),
                AuthToken: localStorage.getItem(tokenLocation),
                AdId: id
            }),
            contentType: "application/json; charset=utf-8",
            success: function (context) {
                row.remove();
            },
            error: function (context) { debugger; }
        });
    }
    function republish()
    {
        let button = this;
        let id = $(this).closest("tr").attr("id");
        $.ajax({
            type: "PUT",
            url: "/api/ad/republish",
            data: JSON.stringify( {
                Username: localStorage.getItem(usernameLocation),
                AuthToken: localStorage.getItem(tokenLocation),
                AdId:id
            }),
            contentType: "application/json; charset=utf-8",
            success: function (context) {
                $(button).parent().text("Active");
            },
            error: function (context) { debugger;}
        });
    }
}
