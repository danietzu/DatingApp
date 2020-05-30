function createAlert(message) {
    alert(message);
}

function log(message) {
    console.log(message);
}

function saveToken(token) {
    localStorage.setItem("token", token);
}

function removeToken() {
    localStorage.removeItem("token");
}

function getToken() {
    return localStorage.getItem("token");
}

function saveUser(user) {
    localStorage.setItem("user", user);
}

function getUser() {
    return localStorage.getItem("user");
}

function removeUser() {
    localStorage.removeItem("user");
}

function confirm(message) {
    alertify.confirm(message, function () {
        alertify.success("Done.");
    });
}

function createConfirm() {
    return confirm("Are you sure?");
}

function scrollTo(elementClass) {
    var elements = document.getElementsByClassName(elementClass);
    if (elements.length !== 0) {
        var lastElement = elements[elements.length - 1];
        lastElement.scrollIntoView({ behavior: "smooth", block: "nearest", inline: "nearest" });
        return true;
    }
}