// Store unfiltered transactions table
let allTransactions = [];
// Stores filtered transactions (so that sorting can be done with search parameters considered)
let filteredTransactions = [];
// Track sorting order of transaction ID, default ascending
let sortOrder = { ascending: true };

// Function to retrieve all transactions
function retrieveTransactionsList() {
    $.ajax({
        url: `/api/BTransaction/all`,
        method: 'GET',
        success: function (data) {
            allTransactions = data
            displayTransactions(allTransactions);
        },
        error: function (error) {
            console.error('Error fetching transaction history:', error);
        }
    });
}

// Function to display retrieved transactions
function displayTransactions(transactions) {
    let transactionTable = $('#allTransactionsTable tbody');
    transactionTable.empty();
    transactions.forEach(transaction => {
        const formattedDate = new Date(transaction.transaction_date).toLocaleString(); // Formats the date to a more readable format

        const formattedAmount = new Intl.NumberFormat('en-AU', {
            style: 'currency',
            currency: 'AUD',
        }).format(transaction.amount);

        transactionTable.append(`
                    <tr>
                        <td>${transaction.transaction_id}</td>
                        <td>${transaction.account_number}</td>
                        <td>${formattedDate}</td>
                        <td>${formattedAmount}</td>
                    </tr>
                `);
    });
    filteredTransactions = transactions;
}

// Filter function for transaction ID and deposits/withdrawals
function filterTransactions() {
    const searchID = $('#transaction-search-id').val();
    const isDepositChecked = $('#filter-deposit').is(':checked');
    const isWithdrawalChecked = $('#filter-withdrawal').is(':checked');

    const filtered = allTransactions.filter(transaction => {
        const matchesID = !searchID || transaction.transaction_id.toString().startsWith(searchID);
        const isDeposit = transaction.amount > 0;
        const isWithdrawal = transaction.amount < 0;

        // Check filters based on checkbox states
        const matchesDeposit = isDepositChecked ? isDeposit : true;
        const matchesWithdrawal = isWithdrawalChecked ? isWithdrawal : true;

        return matchesID && matchesDeposit && matchesWithdrawal;
    });

    displayTransactions(filtered);
}

// Sort function for sorting  based on imported criterion parameters (i.e. is it sorted by amount or transaction ID?)
function sortTransactions(criterion) {
    sortOrder.ascending = !sortOrder.ascending; // Toggle sorting order

    const sortedTransactions = [...filteredTransactions].sort((a, b) => {
        if (criterion === 'transaction_id') {
            return sortOrder.ascending
                ? a.transaction_id - b.transaction_id // Ascending order
                : b.transaction_id - a.transaction_id; // Descending order
        } else if (criterion === 'amount') {
            return sortOrder.ascending
                ? a.amount - b.amount // Ascending order
                : b.amount - a.amount; // Descending order
        }
        return 0; // If no valid criterion, no sorting
    });

    displayTransactions(sortedTransactions);
}

// add event listener for search box
$('#transaction-search-id').on('input', filterTransactions);
// Event listeners for the checkboxes
$('#filter-deposit').on('change', filterTransactions);
$('#filter-withdrawal').on('change', filterTransactions);