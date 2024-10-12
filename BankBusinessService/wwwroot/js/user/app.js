// refresh profile details
function refreshProfileDetails(username) {
    $.ajax({
        url: `/api/BProfile/retrieve/${username}`, // Endpoint to get the user profile
        method: 'GET',
        success: function (data) {

            // Update the profile information in the DOM
            $('#profilePicture').attr('src', data.picture);
            $('#profileName').text(data.name);
            $('#profileEmail').attr('href', 'mailto:' + data.email).text(data.email);
            $('#profilePhone').text(data.phone);
            $('#profileAddress').text(data.address);
            // scroll updated profile into view
            document.getElementById('user-dash').scrollIntoView({ behavior: 'smooth' });
        },
        error: function (xhr) {
            console.error('Error fetching profile information:', xhr);
            alert('Error fetching profile information. Please try again.');
        }
    });

}
// Refresh account balances
function refreshAccountBalances(username) {
    $.ajax({
        url: `/api/BAccount/retrieveByUsername/${username}`,
        method: 'GET',
        success: function (data) {
            // Your Accounts
            let accountTable = $('#accountSummaryTable tbody');
            // Selectable account list
            let senderAccountSelect = $('#sender-account');
            // transaction history table
            let transactionTable = $('#transactionHistoryTable tbody');

            // Clear all existing table rows and select options
            accountTable.empty();
            senderAccountSelect.empty();
            transactionTable.empty();


            // Append the default option for the select
            senderAccountSelect.append('<option value="">Select your account</option>');

            data.forEach(account => {
                // Append rows to the account summary table
                accountTable.append(`
                            <tr>
                                <td>${account.account_number}</td>
                                <td>${account.balance}</td>
                                <td><button class="btn btn-secondary" onclick="viewTransactionHistory(${account.account_number})">View Transaction History</button></td>
                            </tr>
                        `);

                // Append options to the sender account select
                senderAccountSelect.append(`
                            <option value="${account.account_number}">Account: ${account.account_number} - Balance: ${account.balance}</option>
                        `);
            });
        },
        error: function (error) {
            console.error('Error refreshing account balances:', error);
        }
    });
}
// Logout function
function logout() {
    $.ajax({
        url: '/Login/UserLogout',
        method: 'POST',
        success: function () {
            window.location.href = '/Login/UserLogin';
            console.log('Logging out...')
        },
        error: function (error) {
            alert('Error logging out: ', error);
        }
    });
}