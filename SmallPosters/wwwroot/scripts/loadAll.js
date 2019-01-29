function fillTable() {
    $("#search-button").click(loadSearchResults);
    $.ajax({
        type: "GET",
        url: "/api/ad/all",
        contentType: "application/json; charset=utf-8",
        success: function (array) {
            let container = $("#tbody");
            $("#loadingAllAds").hide();
            for (let element of array)
            {
                let row = $("<tr>");
                row.append($("<td>")).append($("<td>")).append($("<td>")).append($("<td>"));
                row.children().attr("scope", "col");
                $(row.children()[0]).text(element.adTitle);
                $(row.children()[1]).text(element.creatorUsername);
                $(row.children()[2]).text(element.categoryName);
                $(row.children()[3]).text(element.adContent);
                container.append(row);
            }
        },
        error: function (context) { }
    });
    function loadSearchResults()
    {
        $("#loadingAllAds").show();
        let toSearchFor = $("#searchForAds").val();
        $.ajax({
            type: "GET",
            url: "/api/ad/search",
            contentType: "application/json; charset=utf-8",
            data: {"SearchQuery":toSearchFor},
            success: function (array) {
                let container = $("#tbody");
                $("#loadingAllAds").hide();
                container.text("");
                for (let element of array) {
                    let row = $("<tr>");
                    row.append($("<td>")).append($("<td>")).append($("<td>")).append($("<td>"));
                    row.children().attr("scope", "col");
                    $(row.children()[0]).text(element.adTitle);
                    $(row.children()[1]).text(element.creatorUsername);
                    $(row.children()[2]).text(element.categoryName);
                    $(row.children()[3]).text(element.adContent);
                    container.append(row);
                }
            },
            error: function (context) { debugger;}
        });
    }
}
