var Routing = function (appRoot, contentSelector, defaultRoute) {
    let folderPrefix = "partials";
    let extensionSuffix = ".html";
    let host = "localhost:44335";
    let tokenLocation = "authToken-io4f5oi4FJ$OIFJoi";
    let usernameLocation = "user-34otjg24i5gj3fqo3joijwOiof45";
    let adminStatusLocation = "admin-234ij329gj45u9fodoijOID#WFUi";
    function getUrlFromHash(hash) {
        var url = hash.replace('#/', '');
        if (url === appRoot)
            url = defaultRoute;
        return url;
    }
    function toggleLoggedIn(context)
    {
        if (localStorage.getItem(tokenLocation)) {
            $("#firstReplaceable").html("<a class=\"nav-link\" href=\"#/myAds\">My Ads</a>");
            $("#secondReplaceable").html("<a class=\"nav-link\" href=\"#/addAd\">Add Ad</a>");
            $("#thirdReplaceable").html("<a class=\"nav-link\" href=\"#/logout\">Logout</a>");
            if (localStorage.getItem(adminStatusLocation) == "true")
            {
                if ($("#adminViewLink").length == 0)
                {
                    $("#menu-list").append("<li class=\"nav-item\" id=\"adminViewLink\"><a class=\"nav-link\" href=\"#/adminView\"><b>Admin View</b></a></li>");
                }
            }
        }
        else
        {
            $("#firstReplaceable").html("<a class=\"nav-link\" href=\"#/\">Home</a>");
            $("#secondReplaceable").html("<a class=\"nav-link\" href=\"#/login\">Login</a>");
            $("#thirdReplaceable").html("<a class=\"nav-link\" href=\"#/register\">Register</a>");
            $("#adminViewLink").remove();
        }
    }
    return {
        init: function () {
            Sammy(contentSelector, function () {
                this.get("#/", function (context) {
                    toggleLoggedIn(context);
                    if (localStorage.getItem(tokenLocation))
                    {
                        context.load(folderPrefix + "/loggedIn" + extensionSuffix).swap();
                    }
                    else
                    {
                        context.load(folderPrefix + "/main" + extensionSuffix).swap();
                    }
                });
                this.get("#/adminView", function (context) {
                    if (!(localStorage.getItem(adminStatusLocation) == "true"))
                    {
                        context.redirect("#/");
                    }
                    toggleLoggedIn(context);
                    var url = getUrlFromHash(context.path);
                    context.load(folderPrefix + url + extensionSuffix).swap();

                });
                this.get("#/logout", function (context) {
                    let username = localStorage.getItem(usernameLocation);
                    let token = localStorage.getItem(tokenLocation);
                    localStorage.setItem(usernameLocation, "");
                    localStorage.setItem(tokenLocation, "");
                    localStorage.setItem(adminStatusLocation, "");
                    context.redirect("#/");
                    $.ajax({
                        type: "POST",
                        url: "/api/user/logout",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify({ Username: username, AuthToken: token }),
                        success: function (context) {
                        },
                        error: function (context) { }
                    });
                });

                this.get("#/viewAll", function (context) {
                    toggleLoggedIn(context);
                    var url = getUrlFromHash(context.path);
                    context.load(folderPrefix + url + extensionSuffix).swap();
                });
                this.get(/\#\/([^\/]+)/, function (context) {
                    toggleLoggedIn(context);
                    var url = getUrlFromHash(context.path);
                    context.load(folderPrefix + url + extensionSuffix).swap();
                });
                this.post("#/addAd", function (contextInit)
                 {
                    let adTitle = contextInit.params["adtitle"];
                    let adContent = contextInit.params["adcontent"];
                    let adCategory = contextInit.params["adcategory"];
                    let adImageURL = contextInit.params["adimageurl"];
                    let adTimeframe = contextInit.params["timeframe"];
                    if (!(adTitle && adContent && adImageURL && adTimeframe && adCategory))
                    {
                        alert("No fields can be left empty!");
                        return;
                    }
                    $.ajax({
                        type: "POST",
                        url: "/api/ad/create",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify(
                            {
                                Username: localStorage.getItem(usernameLocation),
                                AuthToken: localStorage.getItem(tokenLocation),
                                AdTitle: adTitle,
                                AdCategory: adCategory,
                                AdContent: adContent,
                                AdTimeframe: adTimeframe,
                                AdImageURL: adImageURL,
                            }),
                        success: function (context) {
                            contextInit.redirect("#/myAds");
                        },
                        error: function (context) {
                            if (context.status == 401) {
                                alert("Access denied!");
                            }
                        }
                    });
                });
                this.post("#/login", function (contextInit) {
                    let username = contextInit.params["username"];
                    let password = contextInit.params["password"];
                    if (!(username && password)) {
                        alert("No fields can be left empty!");
                        return;
                    }
                    $.ajax({
                        type: "POST",
                        url: "/api/user/login",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify({ Username: username, Password: password}),
                        success: function (context) {
                            localStorage.setItem(tokenLocation, context.authToken);
                            localStorage.setItem(usernameLocation, context.username);
                            localStorage.setItem(adminStatusLocation, context.isAdmin);
                            contextInit.redirect("#/");
                        },
                        error: function (context)
                        {
                            if (context.status == 401)
                            {
                                alert("Invalid username or password!");
                            }
                        }
                    });
                });
                this.post("#/register", function (contextInit) {
                    let username = contextInit.params["username"];
                    let password = contextInit.params["password"];
                    let confirmPassword = contextInit.params["confirmPassword"];
                    if (!(username && password && confirmPassword))
                    {
                        alert("No fields can be left empty!");
                        return;
                    }
                    if (confirmPassword != password)
                    {
                        alert("Passwords do not match!");
                        return;
                    }
                    $.ajax({
                        type: "POST",
                        url: "/api/user/register",
                        contentType: "application/json; charset=utf-8",  
                        data: JSON.stringify({ Username: username, Password: password, ConfirmPassword: confirmPassword }),
                        success: function (context) {
                            localStorage.setItem(tokenLocation, context.authToken);
                            localStorage.setItem(usernameLocation, context.username);
                            localStorage.setItem(adminStatusLocation, context.isAdmin);
                            contextInit.redirect("#/");
                        },
                        error: function (context)
                        {
                            if (context.status == 409)
                            {
                                alert("User with the same username already exists!");
                            }
                        }
                    });
                });
            }).run('#/');
        }
    };
}