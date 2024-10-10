// selectedUser for updating user profiles
let selectedUser = "";
// stores list of (unfiltered) profiles
let allProfiles = [];

// actions that occur when new user creation happens
$('#newUserForm').submit(function (e) {
    e.preventDefault();
    let selectedUser = $('#newUser-uname').val();
    if (confirm(`Are you sure you want to create a new profile for ${selectedUser}?`)) {
        let fullname = $('#newUser-fullname').val();
        let email = $('#newUser-email').val();
        let password = $('#newUser-password').val();
        let phone = $('#newUser-phone').val();
        let address = $('#newUser-address').val();

        $.ajax({
            url: `/api/BProfile/create`,
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                Username: selectedUser,
                Password: password,
                Name: fullname,
                Email: email,
                Address: address,
                Phone: phone,
                Picture: "404"
            }),
            success: function (response) {
                console.log("AJAX response:", response);
                retrieveProfilesList();
                // hide user update form
                $('#newUser').hide();
                // navigate to profiles table
                document.getElementById('allProfilesTable').scrollIntoView({ behavior: 'smooth' });
            },
            error: function (error) {
                console.error('Error creating profile: ', error);
                alert('Error creating profile: ' + error.responseText);
            }
        });
    }
});

function retrieveProfilesList() {
    $.ajax({
        url: `/api/BProfile/all`,
        method: 'GET',
        success: function (data) {
            allProfiles = data
            displayProfiles(allProfiles);
        },
        error: function (error) {
            console.error('Error fetching profile history:', error);
        }
    });
}

// Function to display profiles dynamically
function displayProfiles(profiles) {
    let profileTable = $('#allProfilesTable tbody');
    profileTable.empty();

    profiles.forEach(profile => {
        // Check if the profile's username is 'admin'
        if (profile.username === 'admin') {
            profileTable.append(`
                        <tr>
                            <td>${profile.username}</td>
                            <td>${profile.name}</td>
                            <td>${profile.email}</td>
                            <td>${profile.password}</td>
                            <td>${profile.address}</td>
                            <td>${profile.phone}</td>
                            <td>
                                <button class="btn btn-primary" onclick="document.getElementById('admin-dash').scrollIntoView({ behavior: 'smooth' });">View</button>
                                <button class="btn btn-secondary" onclick="document.getElementById('profileInfo').scrollIntoView({ behavior: 'smooth' });">Edit</button>
                            </td>
                        </tr>
                    `);
        } else {
            profileTable.append(`
                        <tr>
                            <td>${profile.username}</td>
                            <td>${profile.name}</td>
                            <td>${profile.email}</td>
                            <td>${profile.password}</td>
                            <td>${profile.address}</td>
                            <td>${profile.phone}</td>
                            <td>
                                <button class="btn btn-primary" onclick="selectUserAccounts('${profile.username}')">View Accounts</button>
                                <button class="btn btn-secondary" onclick="selectUserProfile('${profile.username}')">Edit</button>
                                <button class="btn btn-danger" onclick="deleteProfile('${profile.username}')">Delete</button>
                            </td>
                        </tr>
                    `);
        }
    });
}

// The code that runs when "Edit" is selected in the table
function selectUserProfile(username) {
    $.ajax({
        url: `/api/BProfile/retrieve/${username}`,
        method: 'GET',
        success: function (data) {
            // change the variable for selectedUser
            selectedUser = data.username
            console.log("selectedUser updated to: ", selectedUser)
            // update heading
            $('#updatingFor').text('Updating for: ' + selectedUser);
            // update user values
            $('#user-fullname').val(data.name);
            $('#user-email').val(data.email);
            $('#user-password').val(data.password);
            $('#user-phone').val(data.phone);
            $('#user-address').val(data.address);
            // show form
            $('#selectUserUpdate').show();
            // navigate to selectUserUpdate
            document.getElementById('selectUserUpdate').scrollIntoView({ behavior: 'smooth' });
        },
        error: function (error) {
            console.error('Error fetching profile history:', error);
        }
    });
}

// The function that runs when "Delete" is selected in the profile table
function deleteProfile(username) {
    if (confirm("Are you sure you want to delete this profile?")) {
        // Delete the profile first
        $.ajax({
            url: `/api/BProfile/delete/${username}`,
            method: 'POST',
        }).then(() => {
            console.log("Profile deleted successfully.");
            retrieveProfilesList(); // Refresh the list after deletion
            // Now retrieve and delete associated accounts
            return $.ajax({
                url: `/api/BAccount/retrieveByUsername/${username}`,
                method: 'GET'
            });
        }).then(response => {
            console.log("Response from server with associated accounts", response);

            // If no accounts found, log and resolve
            if (!Array.isArray(response) || response.length === 0) {
                console.log("No associated accounts found for this profile.");
                return Promise.resolve(); // Resolve if no accounts
            }

            // Prepare delete promises for each account
            const deletePromises = response.map(account => {
                return $.ajax({
                    url: `/api/BAccount/delete/${account.account_number}`,
                    method: 'POST',
                }).then(response => {
                    console.log("Account deleted:", response);
                }).catch(error => {
                    console.error('Error deleting account:', error);
                });
            });

            // Return a promise that resolves when all account deletions complete
            return Promise.all(deletePromises);
        }).catch(error => {
            console.error('Error retrieving associated accounts or deleting profile:', error);
        });
    }
}

// Filters users by username when search box is updated
function filterUsers() {
    const nameSearch = $('#user-search-name').val().toLowerCase();

    const filteredProfiles = allProfiles.filter(profile => {
        const matchesName = !nameSearch || profile.name.toLowerCase().includes(nameSearch);
        return matchesName
    });

    displayProfiles(filteredProfiles);
}

// Attach event listeners to the search inputs
$('#user-search-name').on('input', filterUsers);