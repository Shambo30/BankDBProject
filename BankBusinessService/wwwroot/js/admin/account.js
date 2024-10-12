// function that runs when selecting a user account from the profile table
function selectUserAccounts(username) {

    let button = $('#addAccount');
    button.html(`<button class="btn btn-primary" id="addAccount" onClick="addAccount('${username}')">Add New Account</button>`);

    $.ajax({
        url: `/api/BAccount/retrieveByUsername/${username}`,
        method: 'GET',
        success: function (data) {
            console.log(data.responseText);
            // select the table data
            let accountTable = $('#accountTable tbody');
            // Clear all existing table rows
            accountTable.empty();
            // if null, empty or undefined...
            if (!data || data === undefined || data.length === 0) {
                accountTable.html(`<tr><td colspan="3" class="text-center">No accounts found for ${username}</td></tr>`);
            }
            else {
                data.forEach(account => {
                    // Append rows to the account summary table
                    accountTable.append(`
                            <tr>
                                <td>${account.account_number}</td>
                                <td>${account.balance}</td>
                                <td><button class="btn btn-secondary" onclick="deleteAccount(${account.account_number}, '${username}')">Delete</button></td>
                            </tr>
                        `);
                });
            }

            $('#accountSummary').show();
            document.getElementById('accountSummary').scrollIntoView({ behavior: 'smooth' });
        },
        error: function (error) {
            console.error('Error fetching accounts:', error);
        }
    });
}

function addAccount(username) {
    $.ajax({
        url: `/api/BAccount/create`,
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            holder_username: username,
            balance: 0
        }),
        success: function (response) {
            alert('Account created successfully!');
            selectUserAccounts(username)
        },
        error: function (error) {
            console.error('Error creating account: ', error);
            alert('Error creating account: ' + error.responseText);
        }
    });

}

// function that runs when delete account button is clicked
function deleteAccount(id, username) {
    // Delete the account
    if (confirm("Are you sure you want to delete this profile?")) {
        $.ajax({
            url: `/api/BAccount/delete/${id}`,
            method: 'POST',
            success: function (data) {
                selectUserAccounts(username); // need to re-show new user list
                alert('Account deleted successfully!');
                console.log(data.responseText);
            },
            error: function (error) {
                console.error('Error deleting account:', error);
            }
        });
    }
}