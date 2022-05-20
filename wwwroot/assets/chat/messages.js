// MESSAGES
function renderTextMessage(content, direction) {
    return '  <div class="row message-body">\n' +
        `          <div class="col-sm-12 message-main-${direction}">\n` +
        `            <div class="${direction}">\n` +
        '              <div class="message-text">\n' +
        `               ${content}` +
        '              </div>\n' +
        // '              <span class="message-time pull-right">\n' +
        // '                Sun\n' +
        // '              </span>\n' +
        '            </div>\n' +
        '          </div>\n' +
        '        </div>';

}

function renderFileMessage(content, direction) {
    return '  <div class="row message-body">\n' +
        `          <div class="col-sm-12 message-main-${direction}">\n` +
        `            <div class="${direction}">\n` +
        '              <div class="message-text">\n' +
        `<img id="imgPreview" src="${content}" alt="pic" style="width: 50px; height: 50px;"/>` +
        '              </div>\n' +
        // '              <span class="message-time pull-right">\n' +
        // '                Sun\n' +
        // '              </span>\n' +
        '            </div>\n' +
        '          </div>\n' +
        '        </div>';

}

function renderMediaMessage(content, direction, type) {
    var media_element = document.createElement(type);
    if (type === "video") {
        media_element.style = "width: 1px";
        media_element.style = "hight: 1px";
    }
    media_element.id = 'audio-player';
    if (direction == "sender") {
        media_element.style = "float: right";
    }

    media_element.controls = 'controls';
    media_element.src = content;
    media_element.load();
    var div1 = document.createElement('div');
    var div2 = document.createElement('div');
    var div3 = document.createElement('div');
    var div4 = document.createElement('div');
    div1.classList = "row message-body"
    div2.classList = `col-sm-12 message-main-${direction}`
    div3.classList = `"${direction}"`
    div4.classList = `"message-text"`;
    div4.append(media_element);
    div3.append(div4);
    div2.append(div3);
    div1.append(div2);

    return div1
}

function getMessages(username, currentUser) {
    return $.ajax('/Chat/GetMessages', {
        data : JSON.stringify({ id: username, name: username, server: "", currentUser: currentUser}),
        contentType : 'application/json',
        type : 'POST',    
    }, // data to be submit
    function(data, status, jqXHR) {// success callback
            return data;
     });
    }


async function renderMessages(user, image, currentUser, inboxId) {
    messagesFromDb = await getMessages(user, currentUser);
    $("#side_two").removeAttr('hidden');
    if (image.length > "") {
    $("#currentChatUserImg").attr('src', image)
    }
    $("#conversation").empty();
    $("#conversation").attr('currentUser', user);
    $("#conversation").attr('inbox', inboxId);
    let messages = '        <div class="row message-previous">\n' +
        '          <div class="col-sm-12 previous">\n' +
        '          </div>\n' +
        '        </div>';

    // get messages
    
    for (const i in messagesFromDb) {
        const msg = messagesFromDb[i];
        let direction = "receiver";
        if (msg["sent"]) {
            direction = "sender";
        }

        if (msg["messageType"] === "text") {
            messages += renderTextMessage(msg["content"], direction);
        }
        if (msg["messageType"] === "file") {
            messages += renderFileMessage(msg["content"], direction);
        }
        if (msg["messageType"] === "audio" || msg["messageType"] === "video") {
            var div1 = renderMediaMessage(msg["content"], direction, msg["messageType"])
            $("#conversation").append(messages);
            $("#conversation").append(div1);
            messages = '';
        }
    }
    $("#conversation").append(messages);

    $("#username").empty().append(username);
}

function postMessage(username, inboxId, content, created) {
    return $.ajax('/Chat/CreateNewMessage', {
        data : JSON.stringify({ 
            id: username,
            content: content,
            inboxUID: inboxId,
            messageType: "text",
            created: created.toString(),
            sent: true}),
        contentType : 'application/json',
        type : 'POST',    
    }, // data to be submit
    function(data, status, jqXHR) {// success callback
            return data;
     });
    }

async function addNewMessage(user, currentUserId, inboxId, content) {
    await postMessage(user, inboxId, content, +new Date())
    renderMessages(user, "", currentUserId, inboxId);
    let side_two = document.getElementById("side_two");
    side_two.scrollTop = side_two.scrollHeight;

    let conversation = document.getElementById("conversation");
    conversation.scrollTop = conversation.scrollHeight;
}


// MESSAGES