$(document).ready(function () {
    updatePageContents();
   
});
function updatePageContents() {
    // clear existing tables (just in case)
    clearAllTables()
    // retrieve list of all profiles
    retrieveProfilesList()
    // refresh list of all transactions
    retrieveTransactionsList()
    // update displayed admin profile
    updateAdminProfileDisplay()
    // retrieve logs
    retrieveActivityLogs();
}

// Function to clear all tables
function clearAllTables() {
    let profileTable = $('#allProfilesTable tbody');
    let transactionTable = $('#allTransactionsTable tbody');

    profileTable.empty();
    transactionTable.empty();
}

// Logout function
function logout() {
    $.ajax({
        url: '/Login/AdminLogout',
        method: 'POST',
        success: function () {
            window.location.href = '/Login/AdminLogin';
            console.log('Logging out...')
        },
        error: function (error) {
            alert('Error logging out: ', error);
        }
    });
}