A bank data web service in ASP.NET MVC Core that uses SQLite to store data locally.
Created in collaboration with @mdickson05

This works by locally hosting a database which we have a dataseeder create profiles for and then everything runs through the business tier web application.

Below are screenshots of the main parts of the GUI.

**Main login page**

![image](https://github.com/user-attachments/assets/59240c1e-4d5b-4288-9c5a-e91ca1be68a4)

**Admin Dashboard**

![image](https://github.com/user-attachments/assets/5a4d4171-4305-4b6d-bbeb-845519dfaea9)

![image](https://github.com/user-attachments/assets/2e849576-1404-4046-a8e0-315c3bbb3afd)

![image](https://github.com/user-attachments/assets/638b37c0-72cb-445f-b7ad-c558cb9f6a3b)

![image](https://github.com/user-attachments/assets/4a14ce7f-4467-4904-b082-b7ed32d6aad3)


**User Dashboard**

![image](https://github.com/user-attachments/assets/8e767eb3-0f9c-4fcc-911b-04706b1238b5)

![image](https://github.com/user-attachments/assets/c973bf6f-04e0-4b8d-bfdb-b137bb2b08ea)

![image](https://github.com/user-attachments/assets/a3695d50-fcad-4ec3-8433-f367bc04e049)

![image](https://github.com/user-attachments/assets/ef99f16b-3b9e-4d1a-8f07-63962480aed3)

![image](https://github.com/user-attachments/assets/df0b55ac-a3b2-4edb-aad3-62fc6e2dfb06)


Use this JSON to test profile CRUD operations:
```
{
    "Username" : "johndoe",
    "Email" : "johndoe@fake.com",
    "Password" : "test123",
    "Name" : "John Doe",
    "Address" : "123 Main Street",
    "Phone" : "1234567890",
    "Picture" : "testing"
}
```
