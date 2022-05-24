


function getContacts(currentUser) {
    return $.ajax('/Chat/GetContacts', {
        data : JSON.stringify({ id: currentUser}),
        contentType : 'application/json',
        type : 'POST',    
    }, // data to be submit
    function(data, status, jqXHR) {// success callback
            return data;
        });
    }

async function renderUsers() {
    listOfUsers = await getContacts(currentUserId);
    let users = ""
    let date = "";
    let currentDate = ""
    let lastMessage = "";
    let lastMsgContent = ""
    let fullDate = ""
    let hour = ""
    for(const key in listOfUsers) {
            const contact = listOfUsers[key];
            if (contact["last"] !== null) {
                lastMessage = contact["last"]["content"];
                fullDate = new Date(contact["last"]["created"]);
                hour = fullDate.toLocaleTimeString(navigator.language,
                    {hour: '2-digit', minute: '2-digit'});
                date = fullDate.toLocaleDateString();
                lastMsgContent = (lastMessage.includes("blob:") ||
                    lastMessage.includes("base64")) ? "new media message" : lastMessage;

                if (date !== null) {
                    // a day has passed
                    currentDate = (new Date() - fullDate > 86400000) ? date : hour;
                }
            }

            else {
                date = "";
                currentDate = ""
                lastMessage = "";
                lastMsgContent = "";
                fullDate = "";
                hour = "";

            }
            
            users += `      <div class="row sideBar-body" onclick="renderMessages('${contact.UserId}', '${contact["image"]}', '${currentUserId}', '${contact["inboxUID"]}')">\n` +
                '            <div class="col-sm-3 col-xs-3 sideBar-avatar">\n' +
                '              <div class="avatar-icon">\n' +
                `              <img src="${contact["image"].length > 0 ? contact["image"]: "/assets/chat/red-color.png"}">\n\n` +
                '              </div>\n' +
                '            </div>\n' +
                '            <div class="col-sm-9 col-xs-9 sideBar-main">\n' +
                '              <div class="row">\n' +
                '                <div class="col-sm-8 col-xs-8 sideBar-name">\n' +
                `                  <span class="name-meta">${contact["UserId"]}</span>\n` +
                '                </div>\n' +
                                '<div class="col-sm-4 col-xs-4 pull-right sideBar-time">'+
                                    `<span class="time-meta pull-right"> ${currentDate}<br>${lastMsgContent}</span>`+
                '                 </div>'+
                '              </div>\n' +
                '            </div>\n' +
                '          </div>\n'
        }

        $(".row.sideBar").empty().append(users);
    }


async function addNewUser(username, currentUser, server) {
    $.ajax('/Chat/createContact', {
        data : JSON.stringify({ id: username,  name: username, server: server, currentUser: currentUser}),
        contentType : 'application/json',
        type : 'POST',    
    }, // data to be submit
    async function(data, status, jqXHR) {// success callback
        });
        setTimeout(async function(){    
            await renderUsers();    
        },500);
    }
