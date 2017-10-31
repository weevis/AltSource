function createCookie(name, value, days)
{
    var expires = "";
    if (days)
    {
        var expireDate = new Date();
        expireDate.setTime(expireDate.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + expireDate.toLocaleDateString();
    }
    else
    {
        expires = "";
    }
    document.cookie = name + "=" + value + expires + "; path=/";
}

function readCookie(name)
{
    var realName = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++)
    {
        var c = ca[i];
        while (c.charAt(0) == ' ')
            c = c.substring(1, c.length);
        if (c.indexOf(realName) == 0)
            return c.substring(realName.length, c.length);
    }
    return null;
}

function deleteCookie(name)
{
    createCookie(name, "", -1);
}

function isLoggedIn()
{
    var logged_in = readCookie("logged_in");
    var api_key = readCookie("api_key");

    if (logged_in == null || logged_in != "yes")
        return false;

    if (api_key == null)
        return false;

    return true;
}

function checkLogin(BalanceLink, DepositLink, WithdrawLink, TransactionsLink, LogoutLink)
{
    if (isLoggedIn() == true) {
        var newHTML = '<div id="mainbox" class="row">' +
            '<div class="col-md-3"><h2>Check Balance</h2><p>' + BalanceLink + '</p></div>' +
            '<div class="col-md-3"><h2>Deposit</h2><p>' + DepositLink + '</p></div>' +
            '<div class="col-md-3"><h2>Withdraw</h2><p>' + WithdrawLink + '</p></div>' +
            '<div class="col-md-3"><h2>Transactions</h2><p>' + TransactionsLink + '</p></div>' +
            '</div>';

        $('#mainbox').replaceWith(newHTML);
        var LoginHTML = '<li id="loginout">' + LogoutLink + '</li>';
        var BalanceHTML = '<li id="register">' + BalanceLink + '</li>';
        $('#loginout').replaceWith(LoginHTML);
        $('#register').replaceWith(BalanceHTML);
    }
}