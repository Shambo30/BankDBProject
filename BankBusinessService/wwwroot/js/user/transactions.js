let currentAccountNumber = null; // Global variable to store the selected account number

// AJAX request to view transaction history
function viewTransactionHistory(accountNumber) {
    currentAccountNumber = accountNumber; // Store the selected account number globally
    $('#accountNumberLabel').text(accountNumber); // Update the label with the selected account number

    $.ajax({
        url: `/api/BTransaction/history/${accountNumber}`,
        method: 'GET',
        success: function (data) {
            displayTransactionHistory(data); // Display the transaction history for the selected account
        },
        error: function (error) {
            console.error('Error fetching transaction history:', error);
        }
    });
}


// AJAX request to filter transaction history by date range
function filterTransactions(accountNumber) {
    let startDate = $('#startDate').val();
    let endDate = $('#endDate').val();

    // Validate that both an account number and dates are selected
    if (!accountNumber) {
        alert('Please select an account to filter the transactions.');
        return;
    }

    if (!startDate || !endDate) {
        alert('Please enter both a start date and an end date to filter the transactions.');
        return;
    }

    // Reformat dates to yyyy-mm-dd to ensure consistency
    startDate = new Date(startDate).toISOString().split('T')[0];
    endDate = new Date(endDate).toISOString().split('T')[0];

    console.log(`Filtering transactions for account: ${accountNumber} from ${startDate} to ${endDate}`);

    $.ajax({
        url: `/api/BTransaction/history/filter/${accountNumber}`,
        method: 'GET',
        data: {
            startDate: startDate,
            endDate: endDate
        },
        success: function (data) {
            console.log('Filtered transactions data:', data);
            displayTransactionHistory(data);

            if (data.length === 0) {
                $('#transactionHistoryTable tbody').html('<tr><td colspan="3" class="text-center">No transaction history found between the selected dates.</td></tr>');
            }
        },
        error: function (error) {
            console.error('Error fetching filtered transactions:', error);
        }
    });
}


// Function to display transaction history dynamically
function displayTransactionHistory(transactions) {
    let transactionTable = $('#transactionHistoryTable tbody');
    transactionTable.empty();
    transactions.forEach(transaction => {
        const formattedDate = new Date(transaction.transaction_date).toLocaleDateString('en-GB'); // Formats the date to dd/mm/yyyy

        const formattedAmount = new Intl.NumberFormat('en-AU', {
            style: 'currency',
            currency: 'AUD',
        }).format(transaction.amount);

        transactionTable.append(`
                    <tr>
                        <td>${formattedDate}</td>
                        <td>${transaction.transaction_id}</td>
                        <td>${formattedAmount}</td>
                    </tr>
                `);
    });
}