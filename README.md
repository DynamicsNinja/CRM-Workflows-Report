﻿# CRM Workflows Report
CRM Workflows Report is console application that sends email with information about failed/waiting workflows that happened yesterday for CRM organizations that you want to monitor. 
# Download
The latest releases can be found [here](https://github.com/DynamicsNinja/CRM-Workflows-Report/releases)
# Usage
First you need to configure the app via 2 JSON files.

`mail.json` example:

```json
{  
  "senderName": "John Doe",
  "email": "john.doe@gmail.com",
  "password": "SuperSecret123",
  "to": "jane.doe@gmail.com",
  "toName": "Jane Doe",
  "host": "smtp.gmail.com",
  "port": 587,
  "enableSsl": true 
}
```

| Parameter  | Description                              |
| ---------- | ---------------------------------------- |
| senderName | Name that will be displayed in from field of email |
| email      | Email address that you want to use for sending emails |
| password   | Password for sending email address       |
| to         | Email address that will recieve reports  |
| toName     | Name that will be displayed in to field of email |
| host       | SMTP address of your email provider      |
| port       | Port that is used by your email provider |
| enableSsl  | Use SSL when sending an email            |

`organizations.json` example:

```json
[
  {
    "url": "https://contoso.api.crm4.dynamics.com",
    "username": "crmadmin@contoso.onmicrosoft.com",
    "password": "SuperSecret12345"
  },
  ...
]
```

| Parameter | Description                            |
| --------- | -------------------------------------- |
| url       | Organization URL for your CRM instance |
| username  | Username for your CRM instance         |
| password  | Password for your CRM instance         |

After you configure app via those JSON files you are ready to run the console app.
