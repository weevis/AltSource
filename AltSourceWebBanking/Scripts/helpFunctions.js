/* Format the money */
Number.prototype.formatMoney = function (precision, decimal, comma) {
    var n = this,
        precision = isNaN(precision = Math.abs(precision)) ? 2 : precision,
        decimal = decimal == undefined ? "." : decimal,
        comma = comma == undefined ? "." : comma,
        s = n < 0 ? "-" : "",
        i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(precision))),
        j = (j = i.length) > 3 ? j % 3 : 0;

    return s + (j ? i.substr(0, j) + comma : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + comma) + (precision ? decimal + Math.abs(n - i).toFixed(precision).slice(2) : "");


};

/* Make ajax call to check our bank balance */
function checkBalance()
{
    $.ajax({
        url: "http://localhost:8000/api/account/balance",
        type: "GET",
        headers: {
            "Authorization": "Bearer: " + readCookie("api_key")
        },
        success: function (result) {
            var cashResult = result < 0 ? '$<span style="color: red;">(' + result.formatMoney(2, ".", ",") + ')</span>' : "$" + result.formatMoney(2, ".", ",");
            $('#balance').html(cashResult);
        }
    });
}

/* perform an api login request */
function doLogin()
{
    var data = {};
    var d = $('#form').serializeArray();
    $.each(d, function (k, v) {

        data[v.name] = v.value;
    });

    $.ajax({
        url: "http://localhost:8000/api/account/login",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        type: "POST",
        success: function (result) {
            var api_key = result['apI_KEY'];

            if (api_key == undefined || api_key == "undefined")
                alert("Error logging in, please try again");
            else {
                createCookie('logged_in', 'yes', 7);
                createCookie('api_key', api_key, 7);
                window.location = "/Bank/Index";
            }

        },
        error: function (xhr, resp, text) {
            alert("Error logging in.  Check your Email, UserName, and Password");
            console.log(xhr, resp, text);
        }
    });
}

/* send user registration to api */
function doRegister()
{
    var data = {};
    var d = $('#form').serializeArray();
    $.each(d, function (k, v) {
        data[v.name] = v.value;
    });

    $.ajax({
        url: "http://localhost:8000/api/account/create",
        type: "POST",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            console.log(result);
            window.location = "/Bank/Login";
        },
        error: function (xhr, resp, text) {
            var errors = xhr.responseJSON;

            for (var i = 0; i < errors.length; i++)
            {
                if (errors[i].indexOf("Email") != -1)
                {
                    $('#emailerror').html(errors[i]);
                    $('#emailerror').removeClass('field-validation-valid');
                }
                if (errors[i].indexOf("Password") != -1)
                {
                    $('#passworderror').html(errors[i]);
                }
            }
        }
    });
}

/* record a deposit through the api */
function doDeposit()
{
    var depositAmount = $('#amount').val();

    if (depositAmount == undefined || depositAmount == "") {
        alert("Please Enter a valid amount");
    }
    else if (isNaN(depositAmount)) {
        alert("Please enter a valid amount.")
    }
    else {
        $.ajax({
            url: "http://localhost:8000/api/account/deposit/" + depositAmount,
            type: "POST",
            headers: {
                "Authorization": "Bearer: " + readCookie("api_key")
            },
            success: function (result) {
                checkBalance();
            },
            error: function (data) {
                alert(data);
            }
        });
    }
}

/* record a withdrawal through the api */
function doWithdraw()
{
    var withdrawAmount = $('#amount').val();

    if (withdrawAmount == undefined || withdrawAmount == "") {
        alert("Please Enter a valid amount");
    }
    else if (isNaN(withdrawAmount)) {
        alert("Please enter a valid amount.")
    }
    else {
        $.ajax({
            url: "http://localhost:8000/api/account/withdraw/" + withdrawAmount,
            type: "POST",
            headers: {
                "Authorization": "Bearer: " + readCookie("api_key")
            },
            success: function (result) {
                checkBalance();
            },
            error: function (data) {
                alert("Failure to withdraw funds.");
            }
        });
    }
}

/* show transaction list through api */
function doTransactions()
{
    $.ajax({
        url: "http://localhost:8000/api/account/transactions",
        type: "GET",
        headers: {
            "Authorization": "Bearer: " + readCookie("api_key")
        },
        success: function (result) {
            $('#transaction_table').show();
            var table = $('#transaction_table').DataTable({
            });

            for (var i = 0; i < result.length; i++) {
                var obj = result[i];
                var dateData = obj['transactionTime'];
                var startBalance = "$" + obj['startBalance'].formatMoney(2, ".", ",");
                var changeAmount = "$" + obj['changeAmount'].formatMoney(2, ".", ",");
                var endBalance = "$" + obj['endBalance'].formatMoney(2, ".", ",");


                dateObject = new Date(Date.parse(dateData));
                dateReadable = dateObject.toLocaleString();
                table.row.add([dateReadable, startBalance, changeAmount, endBalance, obj['description']]).draw();
            }
        },
        error: function () {
            alert("Unable to retrieve recent transactions, try again later.");
        }
    });
    checkBalance();
}