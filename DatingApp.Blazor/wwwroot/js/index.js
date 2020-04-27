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