// Navigation helper functions for Blazor
window.navigationHelper = {
    redirectTo: function (url) {
        console.log("Redirecting to: " + url);
        window.location.href = url;
    }
};

// Garantir que a função está disponível
console.log("Navigation helper loaded successfully");
