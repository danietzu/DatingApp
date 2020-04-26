window.addEventListener("load", () => {
    if (localStorage.getItem("token") !== null &&
        localStorage.getItem("token") !== "") {
        DotNet.invokeMethodAsync("DatingApp.Blazor", "SetAsLoggedIn")
            .then(result => {
                console.log(result);
            });
    }
});

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