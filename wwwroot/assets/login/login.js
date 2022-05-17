

var hadFirstError = false;

function checkUserName() {
    var userName = document.getElementById("username").value;
//user name is not empty
    if (userName != "") {
        if (userName in HardCoded) {
            if(checkPassword(userName) === true) {
                window.location.replace(`../chat/chat.html?${userName}`);

            }

        } else if(!hadFirstError) {
            document.getElementById("login_error").insertAdjacentHTML("afterend", "<h5 style='color: red'>username or password is incorrect</h5>")
            hadFirstError = true;
        }
    }
}

    function checkPassword(key) {
        var password = document.getElementById("password").value;
        //the password is correct
        if (HardCoded[key]["password"] === password) {
            return true;
        } else if (!hadFirstError) {
            document.getElementById("login_error").insertAdjacentHTML("afterend", "<h5 style='color: red'> username or password is incorrect</h5>");
            hadFirstError = true;
        }
    }