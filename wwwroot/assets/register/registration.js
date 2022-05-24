var existError = false;
var charsError = false;
var secondPasswordError = false
var emptyPasswordError = false
var emptyUserNameError = false
var displayNameError = false
var passwordLength = false

window.addEventListener("load",(event)=>{
    document.getElementById("ProfileImage").addEventListener("change",function (){
        const reader = new FileReader();
        reader.addEventListener("load",()=>{
        localStorage.setItem("recent-image", reader.result);
        document.getElementById("ProfileImageText").value = reader.result;
        })
        reader.readAsDataURL(this.files[0]);
    })
})

function register () {
  
    var canRegister = true;
  
    var userName = document.getElementById("UserId").value;
    var password = document.getElementById("Password").value;
    var displayName = document.getElementById("DisplayName").value;
    function containsNumber(str) {
        return /[0-9]/.test(str);
    }
    function containLetters(str) {
        return (/[a-z]/.test(str) || /[A-Z]/.test(str));
    }


//Password less than 8 characters
    if(password != "" && password.length <8) {
        canRegister = false;
        if (!passwordLength) {
            document.getElementById("passwordLength").style.display = "block"
            passwordLength = true;
        }
    }
    else {
        passwordLength = false
        document.getElementById("passwordLength").style.display = "none"
    }

// password contain_chars_numbers
    if (!(containLetters(password) && containsNumber(password)) && password != "") {
        canRegister = false;
        if (!charsError) {
            document.getElementById("charsError").style.display = "block"
            charsError = true;
        }
    }
    else{
        charsError = false
        document.getElementById("charsError").style.display = "none";
    }

//confirm password
    if (document.getElementById("confirmpassword").value != password) {
        canRegister = false;
        if (!secondPasswordError) {
            document.getElementById("confirmError").style.display = "block"
            secondPasswordError = true;
        }
    }
    else{
        secondPasswordError = false
        document.getElementById("confirmError").style.display = "none";
    }

//display name is empty
    if (displayName == "") {
        canRegister = false;
        if (!displayNameError) {
            document.getElementById("displayNameError").style.display = "block"
            displayNameError = true;
        }
    }
    else{
        displayNameError = false
        document.getElementById("displayNameError").style.display = "none";
    }

//add the user
    if (canRegister) {
        //if the user didnt add photo - default photo
        if( document.getElementById("img").files.length == 0 ) {
            localStorage.setItem("new_user", JSON.stringify({
                password: password,
                profile: "https://www.enduresc.com.au/wordpress/wp-content/uploads/2018/05/user-profile.jpg",
                chats: [],
                displayName: document.getElementById("name").value
            }));
        }
        else{
            localStorage.setItem("new_user", JSON.stringify({
                password: password,
                profile: localStorage.getItem("recent-image"),
                chats: [],
                displayName: document.getElementById("name").value
            }));
        }
        window.location.replace(`chat.html?${userName}`);
        };
}  