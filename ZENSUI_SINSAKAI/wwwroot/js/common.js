var WebStorage =
{
    setItem: function (key, obj) {
        sessionStorage.setItem(key, JSON.stringify(obj));
    },
    getItem: function (key) {
        return JSON.parse(sessionStorage.getItem(key));
    },
    removeItem: function (key) {
        sessionStorage.removeItem(key);
    }
}
